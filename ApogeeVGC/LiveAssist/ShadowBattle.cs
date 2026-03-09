using ApogeeVGC.Data;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Maintains a shadow Battle object that mirrors a live Showdown game.
/// Created at team preview, advanced each turn via inferred choices from protocol.
/// Provides the Battle for MCTS to clone and simulate forward.
/// </summary>
public sealed class ShadowBattle
{
    private Battle? _battle;
    private readonly Library _library;
    private readonly FormatId _formatId;
    private bool _failed;

    public ShadowBattle(Library library, FormatId formatId)
    {
        _library = library;
        _formatId = formatId;
    }

    /// <summary>
    /// Create the shadow Battle at team preview time.
    /// Builds teams from ShowdownState data, advances through team preview to RequestState.Move.
    /// </summary>
    public void Initialize(ShowdownState state)
    {
        try
        {
            _failed = false;

            PokemonSet[] ownTeam = BuildOwnTeam(state);
            PokemonSet[] oppTeam = BuildOpponentTeam(state);

            // Determine which team is P1 and P2
            bool weAreP1 = state.MySide == "p1";
            PokemonSet[] p1Team = weAreP1 ? ownTeam : oppTeam;
            PokemonSet[] p2Team = weAreP1 ? oppTeam : ownTeam;

            var options = new BattleOptions
            {
                Id = _formatId,
                Sync = true,
                DisplayUi = false,
                Player1Options = new PlayerOptions
                {
                    Name = "Player1",
                    Team = p1Team,
                },
                Player2Options = new PlayerOptions
                {
                    Name = "Player2",
                    Team = p2Team,
                },
            };

            _battle = new Battle(options, _library);
            _battle.Start();

            // Advance through team preview
            // Use default order for now; will be updated when we see actual team order
            string p1Order = state.TeamOrder.GetValueOrDefault("p1") is { Length: > 0 } o1
                ? o1 : DefaultTeamOrder(p1Team.Length);
            string p2Order = state.TeamOrder.GetValueOrDefault("p2") is { Length: > 0 } o2
                ? o2 : DefaultTeamOrder(p2Team.Length);

            _battle.P1.ChooseTeam(p1Order);
            _battle.P2.ChooseTeam(p2Order);
            _battle.CommitChoices();

            Console.WriteLine("[ShadowBattle] Initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ShadowBattle] Initialization failed: {ex.Message}");
            _battle = null;
            _failed = true;
        }
    }

    /// <summary>
    /// Advance the shadow Battle one turn using inferred choices.
    /// Called on |turn| boundary.
    /// </summary>
    public void AdvanceTurn(string? p1Choice, string? p2Choice, ShowdownState state)
    {
        if (_battle == null || _failed) return;

        try
        {
            // Apply choices
            if (p1Choice != null)
                _battle.P1.Choose(p1Choice);
            else
                _battle.P1.AutoChoose();

            if (p2Choice != null)
                _battle.P2.Choose(p2Choice);
            else
                _battle.P2.AutoChoose();

            _battle.CommitChoices();

            // Apply HP corrections from protocol
            ApplyHpCorrections(state);

            Console.WriteLine($"[ShadowBattle] Advanced to turn {_battle.Turn}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ShadowBattle] Turn advance failed: {ex.Message}");
            Console.WriteLine($"  P1 choice: {p1Choice ?? "(null)"}");
            Console.WriteLine($"  P2 choice: {p2Choice ?? "(null)"}");
            _battle = null;
            _failed = true;
        }
    }

    /// <summary>
    /// Get the current Battle for MCTS. Returns null if not initialized or failed.
    /// </summary>
    public Battle? GetBattle() => _battle;

    /// <summary>Whether the shadow battle has failed and fallen back to policy-only.</summary>
    public bool HasFailed => _failed;

    /// <summary>Reset on game end or new game.</summary>
    public void Reset()
    {
        _battle = null;
        _failed = false;
    }

    private void ApplyHpCorrections(ShowdownState state)
    {
        if (_battle == null) return;

        var observedHp = state.GetObservedHp();
        foreach (var (slot, (hp, maxHp)) in observedHp)
        {
            string side = slot[..2];
            char pos = slot[2]; // 'a' or 'b'

            Side battleSide = side == "p1" ? _battle.P1 : _battle.P2;

            // Find the pokemon in the active slot
            int activeIdx = pos == 'a' ? 0 : 1;
            if (activeIdx < battleSide.Active.Count)
            {
                Pokemon? pokemon = battleSide.Active[activeIdx];
                if (pokemon != null && !pokemon.Fainted)
                {
                    // Scale HP: Showdown reports HP/MaxHP, battle has its own maxHP
                    if (maxHp > 0 && pokemon.MaxHp > 0)
                    {
                        int correctedHp = (int)((double)hp / maxHp * pokemon.MaxHp);
                        if (correctedHp != pokemon.Hp)
                        {
                            pokemon.Hp = Math.Max(0, Math.Min(correctedHp, pokemon.MaxHp));
                        }
                    }

                    if (hp == 0)
                    {
                        pokemon.Hp = 0;
                        pokemon.Fainted = true;
                    }
                }
            }
        }
    }

    private PokemonSet[] BuildOwnTeam(ShowdownState state)
    {
        var sets = new List<PokemonSet>();
        ShowdownNameResolver resolver = state.Resolver;

        foreach (OwnTeamPokemon own in state.OwnTeam)
        {
            SpecieId specieId = resolver.ResolveSpecies(own.Species);
            if (specieId == default) continue;

            var moves = new List<MoveId>();
            foreach (string moveName in own.Moves)
            {
                MoveId moveId = resolver.ResolveMove(moveName);
                if (moveId != MoveId.None)
                    moves.Add(moveId);
            }
            if (moves.Count == 0) moves.Add(MoveId.Tackle);

            AbilityId abilityId = resolver.ResolveAbility(own.BaseAbility);
            ItemId itemId = resolver.ResolveItem(own.Item);
            MoveType teraType = resolver.ResolveTeraType(own.TeraType);

            // Use 252/252/4 spread as default; exact EVs don't matter much for MCTS
            var evs = new StatsTable { Hp = 4, Atk = 252, Def = 0, SpA = 252, SpD = 0, Spe = 0 };
            Nature nature = _library.Natures[NatureId.Serious]; // Neutral nature

            sets.Add(new PokemonSet
            {
                Name = own.Species,
                Species = specieId,
                Item = itemId,
                Ability = abilityId,
                Moves = moves,
                Nature = nature,
                Evs = evs,
                Level = own.Level,
                TeraType = teraType,
            });
        }

        return sets.ToArray();
    }

    private PokemonSet[] BuildOpponentTeam(ShowdownState state)
    {
        var sets = new List<PokemonSet>();
        ShowdownNameResolver resolver = state.Resolver;
        string oppSide = state.OppSide ?? "p2";

        // Use team preview species
        var previewSpecies = state.TeamPreview.GetValueOrDefault(oppSide);
        if (previewSpecies == null || previewSpecies.Count == 0)
            return [];

        foreach (string species in previewSpecies)
        {
            SpecieId specieId = resolver.ResolveSpecies(species);
            if (specieId == default) continue;

            // Check for revealed data
            RevealedData? revealed = state.Revealed[oppSide].GetValueOrDefault(species);

            var moves = new List<MoveId>();
            if (revealed != null)
            {
                foreach (string moveName in revealed.Moves)
                {
                    MoveId moveId = resolver.ResolveMove(moveName);
                    if (moveId != MoveId.None)
                        moves.Add(moveId);
                }
            }
            // Pad with Tackle if fewer than 1 move
            if (moves.Count == 0) moves.Add(MoveId.Tackle);

            AbilityId abilityId = revealed?.Ability != null
                ? resolver.ResolveAbility(revealed.Ability)
                : default;

            ItemId itemId = revealed?.Item != null
                ? resolver.ResolveItem(revealed.Item)
                : default;

            MoveType teraType = revealed?.TeraType != null
                ? resolver.ResolveTeraType(revealed.TeraType)
                : default;

            // Generic EV spread and neutral nature
            var evs = new StatsTable { Hp = 4, Atk = 252, Def = 0, SpA = 252, SpD = 0, Spe = 0 };
            Nature nature = _library.Natures[NatureId.Serious];

            sets.Add(new PokemonSet
            {
                Name = species,
                Species = specieId,
                Item = itemId,
                Ability = abilityId,
                Moves = moves,
                Nature = nature,
                Evs = evs,
                Level = 50,
                TeraType = teraType,
            });
        }

        return sets.ToArray();
    }

    private static string DefaultTeamOrder(int teamSize)
    {
        // Default: pick first 4 in order (VGC brings 4 of 6)
        int pick = Math.Min(teamSize, 4);
        return string.Join(",", Enumerable.Range(1, pick));
    }
}
