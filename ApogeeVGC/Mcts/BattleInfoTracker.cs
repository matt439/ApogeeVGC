using ApogeeVGC.Data;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Tracks what information about each opponent Pokemon has been revealed
/// during battle through game events (moves used, abilities triggered,
/// items consumed, terastallization, etc.).
/// </summary>
public sealed class RevealedPokemonInfo(SpecieId species)
{
    public SpecieId Species { get; } = species;
    public HashSet<MoveId> RevealedMoves { get; } = [];
    public AbilityId? RevealedAbility { get; set; }
    public ItemId? RevealedItem { get; set; }
    public bool ItemConsumed { get; set; }
    public MoveType? RevealedTeraType { get; set; }
    public bool HasBeenActive { get; set; }
}

/// <summary>
/// Standalone information tracker that sits between the battle engine and the MCTS.
/// Maintains per-opponent-Pokemon knowledge of what has been revealed through game events,
/// and provides a filtered perspective where unrevealed information is masked.
/// <para>
/// Under Open Team Sheets (OTS), moves/ability/item/tera are known from team preview.
/// Under Closed Team Sheets (CTS), these are hidden until revealed by game events.
/// The tracker supports both modes.
/// </para>
/// </summary>
public sealed class BattleInfoTracker
{
    private readonly SideId _mySideId;
    private readonly SideId _oppSideId;
    private readonly Library _library;

    /// <summary>Reverse lookup: move display name → MoveId.</summary>
    private readonly Dictionary<string, MoveId> _moveNameToId;

    /// <summary>Reverse lookup: ability display name → AbilityId.</summary>
    private readonly Dictionary<string, AbilityId> _abilityNameToId;

    /// <summary>Reverse lookup: item display name → ItemId.</summary>
    private readonly Dictionary<string, ItemId> _itemNameToId;

    /// <summary>Opponent Pokemon info, keyed by SpecieId.</summary>
    private readonly Dictionary<SpecieId, RevealedPokemonInfo> _opponentInfo = new();

    /// <summary>Map opponent Pokemon display names to SpecieId (updated from perspectives).</summary>
    private readonly Dictionary<string, SpecieId> _oppNameToSpecies = new();

    /// <summary>Whether the tracker has been initialized from a perspective.</summary>
    private bool _initialized;

    /// <summary>Whether the opponent has used their terastallization this game.</summary>
    public bool OpponentTeraUsed { get; private set; }

    public BattleInfoTracker(SideId mySideId, Library library)
    {
        _mySideId = mySideId;
        _oppSideId = mySideId == SideId.P1 ? SideId.P2 : SideId.P1;
        _library = library;

        // Build reverse lookup tables from Library
        _moveNameToId = new Dictionary<string, MoveId>(library.Moves.Count);
        foreach ((MoveId moveId, Move move) in library.Moves)
            _moveNameToId[move.Name] = moveId;

        _abilityNameToId = new Dictionary<string, AbilityId>(library.Abilities.Count);
        foreach ((AbilityId abilityId, Ability ability) in library.Abilities)
            _abilityNameToId[ability.Name] = abilityId;

        _itemNameToId = new Dictionary<string, ItemId>(library.Items.Count);
        foreach ((ItemId itemId, Item item) in library.Items)
            _itemNameToId[item.Name] = itemId;
    }

    /// <summary>
    /// Get the revealed info for a specific opponent Pokemon, or null if not tracked.
    /// </summary>
    public RevealedPokemonInfo? GetInfo(SpecieId species)
    {
        return _opponentInfo.GetValueOrDefault(species);
    }

    /// <summary>
    /// Get all tracked opponent Pokemon info.
    /// </summary>
    public IReadOnlyDictionary<SpecieId, RevealedPokemonInfo> OpponentInfo => _opponentInfo;

    // ── Event Processing ────────────────────────────────────────────────

    /// <summary>
    /// Process battle events to update the revealed information state.
    /// Called from PlayerMcts.UpdateEvents each time the battle emits events.
    /// </summary>
    public void ProcessEvents(IEnumerable<BattleEvent> events)
    {
        foreach (BattleEvent e in events)
        {
            // Lazy initialization from the first perspective we see
            if (!_initialized)
                Initialize(e.Perspective);

            // Update name→species mapping and track active opponents from perspective
            UpdateFromPerspective(e.Perspective);

            // Process the message for specific information reveals
            if (e.Message != null)
                ProcessMessage(e.Message);
        }
    }

    /// <summary>
    /// Ensure the tracker is initialized. Called automatically from ProcessEvents,
    /// but can also be called explicitly with the first perspective (e.g., from team preview).
    /// </summary>
    public void EnsureInitialized(BattlePerspective perspective)
    {
        if (!_initialized)
            Initialize(perspective);
    }

    private void Initialize(BattlePerspective perspective)
    {
        // Record all opponent species from the perspective (visible from team preview)
        foreach (PokemonPerspective pokemon in perspective.OpponentSide.Pokemon)
        {
            if (!_opponentInfo.ContainsKey(pokemon.Species))
            {
                _opponentInfo[pokemon.Species] = new RevealedPokemonInfo(pokemon.Species);
            }

            _oppNameToSpecies[pokemon.Name] = pokemon.Species;
        }

        _initialized = true;
    }

    private void UpdateFromPerspective(BattlePerspective perspective)
    {
        // Keep name→species mapping current (handles nicknames seen for the first time)
        foreach (PokemonPerspective pokemon in perspective.OpponentSide.Pokemon)
            _oppNameToSpecies[pokemon.Name] = pokemon.Species;

        // Track which opponent Pokemon are currently active (reveals they were brought)
        foreach (PokemonPerspective? active in perspective.OpponentSide.Active)
        {
            if (active == null) continue;

            if (_opponentInfo.TryGetValue(active.Species, out RevealedPokemonInfo? info))
            {
                info.HasBeenActive = true;

                // Detect terastallization from perspective state
                if (active.Terastallized != null && info.RevealedTeraType == null)
                {
                    info.RevealedTeraType = active.Terastallized;
                    OpponentTeraUsed = true;
                }
            }
        }
    }

    private void ProcessMessage(BattleMessage message)
    {
        switch (message)
        {
            case MoveUsedMessage m:
                if (IsOpponentSide(m.SideId))
                    RecordMove(m.PokemonName, m.MoveName);
                break;

            case AbilityMessage m:
                if (IsOpponentSide(m.SideId))
                    RecordAbility(m.PokemonName, m.AbilityName);
                break;

            case ItemMessage m:
                if (IsOpponentSide(m.SideId))
                    RecordItem(m.PokemonName, m.ItemName, consumed: false);
                break;

            case EndItemMessage m:
                if (IsOpponentSide(m.SideId))
                    RecordItem(m.PokemonName, m.ItemName, consumed: true);
                break;

            case TerastallizeMessage m:
                if (IsOpponentSide(m.SideId))
                    RecordTeraType(m.PokemonName, m.TeraTypeName);
                break;

            case SwitchMessage m:
                if (IsOpponentSide(m.SideId))
                    RecordSwitchIn(m.PokemonName);
                break;
        }
    }

    // ── Recording ───────────────────────────────────────────────────────

    private void RecordMove(string pokemonName, string moveName)
    {
        if (!TryGetOpponentInfo(pokemonName, out RevealedPokemonInfo? info)) return;
        if (_moveNameToId.TryGetValue(moveName, out MoveId moveId))
            info!.RevealedMoves.Add(moveId);
    }

    private void RecordAbility(string pokemonName, string abilityName)
    {
        if (!TryGetOpponentInfo(pokemonName, out RevealedPokemonInfo? info)) return;
        if (_abilityNameToId.TryGetValue(abilityName, out AbilityId abilityId))
            info!.RevealedAbility = abilityId;
    }

    private void RecordItem(string pokemonName, string itemName, bool consumed)
    {
        if (!TryGetOpponentInfo(pokemonName, out RevealedPokemonInfo? info)) return;
        if (_itemNameToId.TryGetValue(itemName, out ItemId itemId))
        {
            info!.RevealedItem = itemId;
            if (consumed) info.ItemConsumed = true;
        }
    }

    private void RecordTeraType(string pokemonName, string teraTypeName)
    {
        if (!TryGetOpponentInfo(pokemonName, out RevealedPokemonInfo? info)) return;
        if (Enum.TryParse(teraTypeName, ignoreCase: true, out MoveType teraType))
        {
            info!.RevealedTeraType = teraType;
            OpponentTeraUsed = true;
        }
    }

    private void RecordSwitchIn(string pokemonName)
    {
        if (!TryGetOpponentInfo(pokemonName, out RevealedPokemonInfo? info)) return;
        info!.HasBeenActive = true;
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    private bool IsOpponentSide(SideId? sideId)
    {
        // If SideId is present, use it directly
        if (sideId.HasValue)
            return sideId.Value == _oppSideId;

        // Without SideId, we can't determine the side from the message alone
        return false;
    }

    private bool TryGetOpponentInfo(string pokemonName, out RevealedPokemonInfo? info)
    {
        info = null;
        if (!_oppNameToSpecies.TryGetValue(pokemonName, out SpecieId species))
            return false;
        return _opponentInfo.TryGetValue(species, out info);
    }

    // ── Perspective Filtering ───────────────────────────────────────────

    /// <summary>
    /// Create a filtered copy of the perspective where unrevealed opponent information
    /// is masked. Player-side information is unchanged (always fully known).
    /// </summary>
    public BattlePerspective FilterPerspective(BattlePerspective perspective)
    {
        if (!_initialized)
            Initialize(perspective);

        return perspective with
        {
            OpponentSide = FilterOpponentSide(perspective.OpponentSide)
        };
    }

    private SideOpponentPerspective FilterOpponentSide(SideOpponentPerspective opp)
    {
        return opp with
        {
            Pokemon = opp.Pokemon.Select(FilterOpponentPokemon).ToList(),
            Active = opp.Active.Select(a => a != null ? FilterOpponentPokemon(a) : null).ToList(),
        };
    }

    private PokemonPerspective FilterOpponentPokemon(PokemonPerspective p)
    {
        RevealedPokemonInfo? info = _opponentInfo.GetValueOrDefault(p.Species);

        return p with
        {
            // Moves: only show moves that have been revealed; mask the rest
            MoveSlots = FilterMoveSlots(p.MoveSlots, info?.RevealedMoves),

            // Ability: show only if revealed, otherwise mask to None
            Ability = info?.RevealedAbility ?? AbilityId.None,

            // Item: show only if revealed, otherwise mask to None
            // If item was consumed, still record what it was (it's known)
            // but the current item on the pokemon is gone
            Item = info?.RevealedItem != null ? p.Item : ItemId.None,

            // Stats: never directly observable by the opponent
            StoredStats = new StatsExceptHpTable(),

            // Tera type: hidden until terastallization occurs
            TeraType = info?.RevealedTeraType ?? MoveType.Unknown,
        };
    }

    /// <summary>
    /// Filter move slots to only include revealed moves.
    /// Unrevealed slots are replaced with MoveId.None placeholders.
    /// </summary>
    private static IReadOnlyList<MoveSlot> FilterMoveSlots(
        IReadOnlyList<MoveSlot> moveSlots, HashSet<MoveId>? revealedMoves)
    {
        if (revealedMoves == null || revealedMoves.Count == 0)
        {
            // No moves revealed: return 4 empty slots
            return CreateEmptyMoveSlots(moveSlots.Count);
        }

        var filtered = new List<MoveSlot>(moveSlots.Count);
        foreach (MoveSlot slot in moveSlots)
        {
            if (revealedMoves.Contains(slot.Id))
            {
                // This move has been revealed: show it (but mask PP info)
                filtered.Add(new MoveSlot
                {
                    Id = slot.Id,
                    Move = slot.Move,
                    Pp = 0, // PP is not observable
                    MaxPp = 0, // PP is not observable
                    Target = slot.Target,
                    Disabled = slot.Disabled,
                });
            }
            else
            {
                // This move has not been revealed: mask it
                filtered.Add(new MoveSlot
                {
                    Id = MoveId.None,
                    Move = MoveId.None,
                    Pp = 0,
                    MaxPp = 0,
                });
            }
        }

        return filtered;
    }

    private static IReadOnlyList<MoveSlot> CreateEmptyMoveSlots(int count)
    {
        var empty = new List<MoveSlot>(count);
        for (var i = 0; i < count; i++)
        {
            empty.Add(new MoveSlot
            {
                Id = MoveId.None,
                Move = MoveId.None,
                Pp = 0,
                MaxPp = 0,
            });
        }

        return empty;
    }
}