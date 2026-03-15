using System.Text.Json;
using System.Text.RegularExpressions;
using ApogeeVGC.Data;
using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Tracks the state of a live Showdown battle from protocol lines and |request| JSON.
/// Builds BattlePerspective for model inference and provides legal action info for MCTS.
/// </summary>
public sealed class ShowdownState
{
    private readonly ShowdownNameResolver _resolver;

    // Identity
    public string? MySide { get; private set; }
    public string? OppSide { get; private set; }

    // Active pokemon: slot ("p1a", "p2b") -> tracked pokemon
    private readonly Dictionary<string, TrackedPokemon> _active = new();

    // Bench pokemon: side -> species_key -> tracked pokemon
    private readonly Dictionary<string, Dictionary<string, TrackedPokemon>> _bench = new()
    {
        ["p1"] = new(), ["p2"] = new(),
    };

    // Nickname resolution: "p1a: Nickname" -> species name
    private readonly Dictionary<string, string> _nicknameToSpecies = new();
    private readonly Dictionary<string, string> _slotSpecies = new(); // slot -> species

    // Revealed info (opponent progressive): side -> species -> revealed data
    private readonly Dictionary<string, Dictionary<string, RevealedData>> _revealed = new()
    {
        ["p1"] = new(), ["p2"] = new(),
    };

    // Team preview: side -> list of species
    private readonly Dictionary<string, List<string>> _teamPreview = new()
    {
        ["p1"] = new(), ["p2"] = new(),
    };

    // Own team from |request|
    private readonly List<OwnTeamPokemon> _ownTeam = new();

    // Field state
    private ConditionId _weather = ConditionId.None;
    private ConditionId _terrain = ConditionId.None;
    private bool _trickRoom;
    private readonly Dictionary<string, HashSet<ConditionId>> _sideConditions = new()
    {
        ["p1"] = new(), ["p2"] = new(),
    };

    // Tera tracking
    private readonly HashSet<string> _teraUsed = new(); // sides that have used tera

    // Turn tracking
    public int CurrentTurn { get; private set; }
    public string Phase { get; private set; } = "init"; // init / teampreview / battle / ended

    // Last request
    public JsonDocument? LastRequest { get; private set; }

    // --- Shadow Battle: choice inference ---
    // Tracks inferred choices for each active slot during the current turn.
    // Key: slot like "p1a", "p1b". Value: inferred action for that slot.
    private readonly Dictionary<string, InferredAction> _turnActions = new();

    // Tracks HP values observed this turn for post-sim correction.
    // Key: slot like "p1a". Value: (hp, maxHp) as reported by protocol.
    private readonly Dictionary<string, (int Hp, int MaxHp)> _observedHp = new();

    // Track which slots had forced switches this turn (from |drag| or faint replacement)
    private readonly HashSet<string> _forcedSwitchSlots = new();

    // Team order chosen at team preview (e.g. "1,2,3,4" for each side)
    private readonly Dictionary<string, string> _teamOrder = new()
    {
        ["p1"] = "", ["p2"] = "",
    };

    // Callback invoked on |turn| boundary to advance the shadow battle
    public event Action<int>? OnTurnBoundary;

    private static readonly Regex ReFromAbility = new(@"\[from\] ability: (.+?)(?:\||\[|$)");
    private static readonly Regex ReOfSlot = new(@"\[of\] (p[12][ab]): (.+?)(?:\||$)");
    private static readonly Regex ReFromItem = new(@"\[from\] item: (.+?)(?:\||\[|$)");

    public ShowdownState(Library library)
    {
        _resolver = new ShowdownNameResolver(library);
    }

    /// <summary>
    /// Process protocol lines from Showdown.
    /// </summary>
    public void Update(string[] lines)
    {
        foreach (string line in lines)
        {
            if (!line.StartsWith('|'))
                continue;

            string[] parts = line.Split('|');
            if (parts.Length < 2) continue;
            string cmd = parts[1];

            try
            {
                ProcessLine(cmd, parts, line);
            }
            catch (Exception)
            {
                // Skip malformed lines
            }
        }
    }

    /// <summary>
    /// Process |request| JSON from Showdown.
    /// </summary>
    public void UpdateRequest(JsonDocument request)
    {
        LastRequest?.Dispose();
        LastRequest = request;

        JsonElement root = request.RootElement;

        // Identify our side
        if (root.TryGetProperty("side", out JsonElement side))
        {
            if (side.TryGetProperty("id", out JsonElement id))
            {
                string sideId = id.GetString()!;
                MySide = sideId;
                OppSide = sideId == "p1" ? "p2" : "p1";
            }

            // Parse own team
            _ownTeam.Clear();
            if (side.TryGetProperty("pokemon", out JsonElement pokemon))
            {
                foreach (JsonElement poke in pokemon.EnumerateArray())
                {
                    _ownTeam.Add(ParseOwnPokemon(poke));
                }
            }
        }

        // Update phase
        if (root.TryGetProperty("teamPreview", out _))
            Phase = "teampreview";
        else if (root.TryGetProperty("active", out _))
            Phase = "battle";
    }

    private OwnTeamPokemon ParseOwnPokemon(JsonElement poke)
    {
        string details = poke.GetProperty("details").GetString() ?? "";
        ParseSpeciesDetail(details, out string species, out int level);

        string condition = poke.TryGetProperty("condition", out JsonElement cond)
            ? cond.GetString() ?? "100/100" : "100/100";
        ParseHp(condition, out int hp, out int maxHp);
        string? status = ParseStatusFromHp(condition);
        bool fainted = condition.Contains("fnt");

        var moves = new List<string>();
        if (poke.TryGetProperty("moves", out JsonElement movesEl))
        {
            foreach (JsonElement m in movesEl.EnumerateArray())
                moves.Add(m.GetString() ?? "");
        }

        string ability = poke.TryGetProperty("ability", out JsonElement ab)
            ? ab.GetString() ?? "" : "";
        string baseAbility = poke.TryGetProperty("baseAbility", out JsonElement ba)
            ? ba.GetString() ?? "" : ability;
        string item = poke.TryGetProperty("item", out JsonElement it)
            ? it.GetString() ?? "" : "";
        string teraType = poke.TryGetProperty("teraType", out JsonElement tt)
            ? tt.GetString() ?? "" : "";
        bool active = poke.TryGetProperty("active", out JsonElement act) && act.GetBoolean();

        // Parse computed stats: {atk, def, spa, spd, spe}
        var stats = new Dictionary<string, int>();
        if (poke.TryGetProperty("stats", out JsonElement statsEl))
        {
            foreach (JsonProperty prop in statsEl.EnumerateObject())
            {
                if (prop.Value.TryGetInt32(out int statVal))
                    stats[prop.Name] = statVal;
            }
        }

        return new OwnTeamPokemon
        {
            Species = species,
            Moves = moves,
            Ability = ability,
            BaseAbility = baseAbility,
            Item = item,
            TeraType = teraType,
            Hp = hp,
            MaxHp = maxHp,
            Status = status,
            Fainted = fainted,
            Active = active,
            Level = level,
            Stats = stats,
        };
    }

    /// <summary>
    /// Build a BattlePerspective for model inference from the current tracked state.
    /// </summary>
    public BattlePerspective BuildPerspective()
    {
        if (MySide == null) throw new InvalidOperationException("Side not identified yet");

        string mySide = MySide;
        string oppSide = OppSide!;

        // Own active
        var myActiveA = GetActivePerspective(mySide, "a", isOwn: true);
        var myActiveB = GetActivePerspective(mySide, "b", isOwn: true);

        // Opponent active
        var oppActiveA = GetActivePerspective(oppSide, "a", isOwn: false);
        var oppActiveB = GetActivePerspective(oppSide, "b", isOwn: false);

        // Own bench (from own_team, not active, not fainted)
        var ownActiveSpecies = new HashSet<string>();
        if (myActiveA != null) ownActiveSpecies.Add(GetSpeciesName(myActiveA.Species));
        if (myActiveB != null) ownActiveSpecies.Add(GetSpeciesName(myActiveB.Species));

        var myBench = new List<PokemonPerspective>();
        int benchPos = 3;
        foreach (OwnTeamPokemon p in _ownTeam)
        {
            if (!p.Active && !p.Fainted && !ownActiveSpecies.Contains(p.Species))
            {
                myBench.Add(BuildOwnPerspective(p, benchPos++, isActive: false));
                if (myBench.Count >= 2) break;
            }
        }

        // Opponent bench
        var oppActiveSpecies = new HashSet<string>();
        if (oppActiveA != null) oppActiveSpecies.Add(GetSpeciesName(oppActiveA.Species));
        if (oppActiveB != null) oppActiveSpecies.Add(GetSpeciesName(oppActiveB.Species));

        var oppBench = new List<PokemonPerspective>();
        benchPos = 3;
        foreach (var (species, info) in _bench[oppSide])
        {
            if (!info.Fainted && !oppActiveSpecies.Contains(species))
            {
                oppBench.Add(BuildOpponentPerspective(species, info, benchPos++, isActive: false));
                if (oppBench.Count >= 2) break;
            }
        }

        // Build full pokemon lists
        var myPokemon = new List<PokemonPerspective>();
        if (myActiveA != null) myPokemon.Add(myActiveA);
        if (myActiveB != null) myPokemon.Add(myActiveB);
        myPokemon.AddRange(myBench);

        var oppPokemon = new List<PokemonPerspective>();
        if (oppActiveA != null) oppPokemon.Add(oppActiveA);
        if (oppActiveB != null) oppPokemon.Add(oppActiveB);
        oppPokemon.AddRange(oppBench);

        // Side conditions
        var mySideConditions = BuildSideConditions(mySide);
        var oppSideConditions = BuildSideConditions(oppSide);

        // Pseudo weather
        var pseudoWeather = new List<ConditionId>();
        if (_trickRoom) pseudoWeather.Add(ConditionId.TrickRoom);

        return new BattlePerspective
        {
            PerspectiveType = BattlePerspectiveType.InBattle,
            TurnCounter = CurrentTurn,
            PlayerSide = new SidePlayerPerspective
            {
                Team = [],
                Pokemon = myPokemon,
                Active = [myActiveA, myActiveB],
                SideConditionsWithDuration = mySideConditions,
            },
            OpponentSide = new SideOpponentPerspective
            {
                Pokemon = oppPokemon,
                Active = [oppActiveA, oppActiveB],
                SideConditionsWithDuration = oppSideConditions,
            },
            Field = new FieldPerspective
            {
                Weather = _weather,
                Terrain = _terrain,
                PseudoWeather = pseudoWeather,
                PseudoWeatherWithDuration = new Dictionary<ConditionId, int?>(),
            },
        };
    }

    /// <summary>
    /// Build a BattlePerspective for team preview.
    /// </summary>
    public BattlePerspective BuildTeamPreviewPerspective()
    {
        if (MySide == null) throw new InvalidOperationException("Side not identified yet");

        var myPokemon = new List<PokemonPerspective>();
        int pos = 1;
        foreach (OwnTeamPokemon p in _ownTeam)
        {
            myPokemon.Add(BuildOwnPerspective(p, pos++, isActive: false));
        }

        var oppPokemon = new List<PokemonPerspective>();
        pos = 1;
        foreach (string species in _teamPreview[OppSide!])
        {
            SpecieId specieId = _resolver.ResolveSpecies(species);
            oppPokemon.Add(new PokemonPerspective
            {
                Name = species,
                Species = specieId,
                Level = 50,
                Gender = GenderId.N,
                Shiny = false,
                Hp = 100,
                MaxHp = 100,
                Fainted = false,
                Status = ConditionId.None,
                MoveSlots = [],
                Boosts = new BoostsTable(),
                StoredStats = new StatsExceptHpTable(),
                Ability = AbilityId.None,
                Item = ItemId.None,
                Types = [],
                Terastallized = null,
                TeraType = default,
                Volatiles = [],
                VolatilesWithDuration = new Dictionary<ConditionId, int?>(),
                Position = pos++,
                IsActive = false,
            });
        }

        return new BattlePerspective
        {
            PerspectiveType = BattlePerspectiveType.TeamPreview,
            TurnCounter = 0,
            PlayerSide = new SidePlayerPerspective
            {
                Team = [],
                Pokemon = myPokemon,
                Active = [],
                SideConditionsWithDuration = new Dictionary<ConditionId, int?>(),
            },
            OpponentSide = new SideOpponentPerspective
            {
                Pokemon = oppPokemon,
                Active = [],
                SideConditionsWithDuration = new Dictionary<ConditionId, int?>(),
            },
            Field = new FieldPerspective
            {
                Weather = ConditionId.None,
                Terrain = ConditionId.None,
                PseudoWeather = [],
                PseudoWeatherWithDuration = new Dictionary<ConditionId, int?>(),
            },
        };
    }

    /// <summary>
    /// Build legal action masks from the last |request| JSON.
    /// Returns (maskA, maskB) boolean arrays over the action vocabulary.
    /// Also returns the LegalAction lists for building MCTS choices.
    /// </summary>
    public (LegalActionSet Actions, bool[] MaskA, bool[] MaskB) BuildLegalActions(
        Vocab vocab, ActionMapper mapper)
    {
        if (LastRequest == null || MySide == null)
            throw new InvalidOperationException("No request available");

        JsonElement root = LastRequest.RootElement;

        // Build actions from the Showdown request JSON
        var slotAActions = new List<LegalAction>();
        var slotBActions = new List<LegalAction>();

        if (root.TryGetProperty("active", out JsonElement active))
        {
            // Move request
            if (active.GetArrayLength() > 0)
                slotAActions = BuildSlotActions(active[0], vocab, slotIndex: 0);
            if (active.GetArrayLength() > 1)
                slotBActions = BuildSlotActions(active[1], vocab, slotIndex: 1);
        }
        else if (root.TryGetProperty("forceSwitch", out JsonElement forceSwitch))
        {
            // Force switch request
            bool slotANeeds = forceSwitch.GetArrayLength() > 0 && forceSwitch[0].GetBoolean();
            bool slotBNeeds = forceSwitch.GetArrayLength() > 1 && forceSwitch[1].GetBoolean();

            if (slotANeeds)
                slotAActions = BuildSwitchActions(vocab);
            else if (slotBNeeds)
                slotAActions.Add(new LegalAction
                    { VocabIndex = Vocab.NoneActionIndex, ChoiceType = Sim.Choices.ChoiceType.Pass });

            if (slotBNeeds)
                slotBActions = BuildSwitchActions(vocab);
        }

        // Fallback: pass if no actions
        if (slotAActions.Count == 0)
            slotAActions.Add(new LegalAction
                { VocabIndex = Vocab.NoneActionIndex, ChoiceType = Sim.Choices.ChoiceType.Pass });

        var actionSet = new LegalActionSet { SlotA = slotAActions, SlotB = slotBActions };

        bool[] maskA = mapper.BuildLegalMask(slotAActions);
        bool[] maskB = slotBActions.Count > 0
            ? mapper.BuildLegalMask(slotBActions)
            : new bool[vocab.NumActions];

        return (actionSet, maskA, maskB);
    }

    /// <summary>
    /// Get human-readable action labels for top recommendations.
    /// </summary>
    public static List<ActionRecommendation> GetTopActions(
        float[] probs, IReadOnlyList<LegalAction> actions, Vocab vocab, int topK = 5)
    {
        var results = new List<ActionRecommendation>();
        foreach (LegalAction action in actions)
        {
            float prob = probs[action.VocabIndex];
            if (prob <= 0) continue;

            string label = vocab.GetActionKey(action.VocabIndex);
            if (action.Terastallize.HasValue)
                label += " [tera]";

            results.Add(new ActionRecommendation { Action = label, Prob = prob });
        }

        results.Sort((a, b) => b.Prob.CompareTo(a.Prob));
        return results.Count > topK ? results.GetRange(0, topK) : results;
    }

    public IReadOnlyList<OwnTeamPokemon> OwnTeam => _ownTeam;

    // ── Private helpers ─────────────────────────────────────────────────────

    private void ProcessLine(string cmd, string[] parts, string line)
    {
        switch (cmd)
        {
            case "poke" when parts.Length >= 4:
            {
                string side = parts[2].Trim();
                ParseSpeciesDetail(parts[3].Trim(), out string species, out _);
                if (!_teamPreview.ContainsKey(side))
                    _teamPreview[side] = new List<string>();
                _teamPreview[side].Add(species);
                Phase = "teampreview";
                break;
            }

            case "turn" when parts.Length >= 3:
            {
                int newTurn = int.Parse(parts[2].Trim());
                // Fire turn boundary event before updating turn counter.
                // This allows the shadow battle to advance using the current turn's inferred actions.
                if (CurrentTurn > 0)
                    OnTurnBoundary?.Invoke(CurrentTurn);
                CurrentTurn = newTurn;
                Phase = "battle";
                break;
            }

            case "switch" when parts.Length >= 5:
                HandleSwitch(parts, forced: false);
                break;

            case "drag" when parts.Length >= 5:
                HandleSwitch(parts, forced: true);
                break;

            case "move" when parts.Length >= 4:
                HandleMove(parts);
                break;

            case "-damage" or "-heal" when parts.Length >= 4:
                HandleHpChange(parts);
                break;

            case "faint" when parts.Length >= 3:
                HandleFaint(parts);
                break;

            case "-boost" when parts.Length >= 5:
                HandleBoost(parts, positive: true);
                break;

            case "-unboost" when parts.Length >= 5:
                HandleBoost(parts, positive: false);
                break;

            case "-status" when parts.Length >= 4:
            {
                ParseSlot(parts[2].Trim(), out string slot, out _);
                string status = parts[3].Trim();
                if (_active.TryGetValue(slot, out TrackedPokemon? p))
                    p.Status = _resolver.ResolveStatus(status);
                break;
            }

            case "-curestatus" when parts.Length >= 4:
            {
                ParseSlot(parts[2].Trim(), out string slot, out _);
                if (_active.TryGetValue(slot, out TrackedPokemon? p))
                    p.Status = ConditionId.None;
                break;
            }

            case "-terastallize" when parts.Length >= 4:
                HandleTera(parts);
                break;

            case "-weather" when parts.Length >= 3:
                HandleWeather(parts, line);
                break;

            case "-fieldstart" when parts.Length >= 3:
                HandleFieldStart(parts);
                break;

            case "-fieldend" when parts.Length >= 3:
                HandleFieldEnd(parts);
                break;

            case "-sidestart" when parts.Length >= 4:
                HandleSideCondition(parts, start: true);
                break;

            case "-sideend" when parts.Length >= 4:
                HandleSideCondition(parts, start: false);
                break;

            case "-item" or "-enditem" when parts.Length >= 4:
            {
                ParseSlot(parts[2].Trim(), out string slot, out string nickname);
                string side = PlayerSide(slot);
                string species = ResolveSpecies(slot, nickname);
                GetRevealed(side, species).Item = parts[3].Trim();
                break;
            }

            case "-ability" when parts.Length >= 4:
            {
                ParseSlot(parts[2].Trim(), out string slot, out string nickname);
                string side = PlayerSide(slot);
                string species = ResolveSpecies(slot, nickname);
                GetRevealed(side, species).Ability = parts[3].Trim();
                break;
            }

            case "swap" when parts.Length >= 4:
                HandleSwap(parts);
                break;

            case "detailschange" when parts.Length >= 4:
            {
                ParseSlot(parts[2].Trim(), out string slot, out _);
                ParseSpeciesDetail(parts[3].Trim(), out string newSpecies, out _);
                if (_active.TryGetValue(slot, out TrackedPokemon? p))
                    p.Species = newSpecies;
                _slotSpecies[slot] = newSpecies;
                break;
            }

            case "win":
                Phase = "ended";
                break;
        }

        // Handle [from] ability/item tags
        if (line.Contains("[from] ability:"))
        {
            Match m = ReFromAbility.Match(line);
            if (m.Success)
            {
                Match ofMatch = ReOfSlot.Match(line);
                if (ofMatch.Success)
                {
                    string slot = ofMatch.Groups[1].Value;
                    string side = PlayerSide(slot);
                    string species = ResolveSpecies(slot, ofMatch.Groups[2].Value.Trim());
                    GetRevealed(side, species).Ability = m.Groups[1].Value.Trim();
                }
            }
        }

        if (line.Contains("[from] item:"))
        {
            Match m = ReFromItem.Match(line);
            if (m.Success && parts.Length >= 3)
            {
                try
                {
                    ParseSlot(parts[2].Trim(), out string slot, out string nickname);
                    string side = PlayerSide(slot);
                    string species = ResolveSpecies(slot, nickname);
                    GetRevealed(side, species).Item = m.Groups[1].Value.Trim();
                }
                catch { /* skip */ }
            }
        }
    }

    private void HandleSwitch(string[] parts, bool forced)
    {
        ParseSlot(parts[2].Trim(), out string slot, out string nickname);
        ParseSpeciesDetail(parts[3].Trim(), out string species, out _);
        ParseHp(parts[4].Trim(), out int hp, out int maxHp);
        string? status = ParseStatusFromHp(parts[4].Trim());

        string side = PlayerSide(slot);

        // Save outgoing to bench
        if (_active.TryGetValue(slot, out TrackedPokemon? old))
        {
            _bench[side][old.Species] = old;
            old.IsActive = false;
        }

        // Remove incoming from bench
        _bench[side].Remove(species);

        // Track nickname
        _nicknameToSpecies[$"{slot}: {nickname}"] = species;
        _slotSpecies[slot] = species;

        // Shadow battle: record inferred switch action
        if (forced)
        {
            _forcedSwitchSlots.Add(slot);
        }
        else if (!_turnActions.ContainsKey(slot))
        {
            int teamSlot = FindTeamSlot(side, species);
            _turnActions[slot] = new InferredAction
            {
                IsSwitch = true,
                SwitchSlot = teamSlot,
            };
        }

        // Record observed HP for post-sim correction
        _observedHp[slot] = (hp, maxHp);

        // Create new tracked pokemon
        var tracked = new TrackedPokemon
        {
            Species = species,
            Hp = hp,
            MaxHp = maxHp,
            Status = _resolver.ResolveStatus(status),
            IsActive = true,
        };
        _active[slot] = tracked;

        if (Phase == "init") Phase = "battle";
    }

    private void HandleMove(string[] parts)
    {
        ParseSlot(parts[2].Trim(), out string slot, out string nickname);
        string moveName = parts[3].Trim();
        string side = PlayerSide(slot);
        string species = ResolveSpecies(slot, nickname);
        RevealedData r = GetRevealed(side, species);
        if (!r.Moves.Contains(moveName))
            r.Moves.Add(moveName);

        // Shadow battle: record inferred move action (only if not already recorded this turn)
        if (!_turnActions.ContainsKey(slot) && !_forcedSwitchSlots.Contains(slot))
        {
            // Determine target location from protocol target field
            int targetLoc = 0;
            if (parts.Length >= 5)
            {
                string targetStr = parts[4].Trim();
                // Target format: "p2a: Nickname" or "[spread]" etc.
                if (targetStr.Length >= 3 && targetStr[0] == 'p' &&
                    char.IsDigit(targetStr[1]) && char.IsLetter(targetStr[2]))
                {
                    string targetSlot = targetStr[..3]; // e.g. "p2a"
                    targetLoc = InferTargetLoc(slot, targetSlot);
                }
            }

            // Convert move name to showdown ID format
            string showdownMove = Regex.Replace(moveName.ToLowerInvariant(), @"[^a-z0-9]", "");

            _turnActions[slot] = new InferredAction
            {
                IsSwitch = false,
                MoveName = showdownMove,
                TargetLoc = targetLoc,
            };
        }
    }

    private void HandleHpChange(string[] parts)
    {
        ParseSlot(parts[2].Trim(), out string slot, out _);
        ParseHp(parts[3].Trim(), out int hp, out int maxHp);
        string? status = ParseStatusFromHp(parts[3].Trim());

        if (_active.TryGetValue(slot, out TrackedPokemon? p))
        {
            p.Hp = hp;
            p.MaxHp = maxHp;
            if (status != null) p.Status = _resolver.ResolveStatus(status);
            if (hp == 0) p.Fainted = true;
        }

        // Shadow battle: record observed HP for post-sim correction
        _observedHp[slot] = (hp, maxHp);
    }

    private void HandleFaint(string[] parts)
    {
        ParseSlot(parts[2].Trim(), out string slot, out _);
        if (_active.TryGetValue(slot, out TrackedPokemon? p))
        {
            p.Hp = 0;
            p.Fainted = true;
            string side = PlayerSide(slot);
            _bench[side][p.Species] = p;
        }
    }

    private void HandleBoost(string[] parts, bool positive)
    {
        ParseSlot(parts[2].Trim(), out string slot, out _);
        string stat = parts[3].Trim();
        int amount = int.Parse(parts[4].Trim());
        if (!positive) amount = -amount;

        if (_active.TryGetValue(slot, out TrackedPokemon? p))
        {
            switch (stat)
            {
                case "atk": p.AtkBoost += amount; break;
                case "def": p.DefBoost += amount; break;
                case "spa": p.SpABoost += amount; break;
                case "spd": p.SpDBoost += amount; break;
                case "spe": p.SpeBoost += amount; break;
            }
        }
    }

    private void HandleTera(string[] parts)
    {
        ParseSlot(parts[2].Trim(), out string slot, out string nickname);
        string teraType = parts[3].Trim();
        if (_active.TryGetValue(slot, out TrackedPokemon? p))
        {
            p.TeraType = _resolver.ResolveTeraType(teraType);
            p.IsTerastallized = true;
        }
        string side = PlayerSide(slot);
        _teraUsed.Add(side);
        string species = ResolveSpecies(slot, nickname);
        GetRevealed(side, species).TeraType = teraType;

        // Shadow battle: mark the inferred action as having tera
        if (_turnActions.TryGetValue(slot, out InferredAction? action))
            action.Tera = true;
    }

    private void HandleWeather(string[] parts, string line)
    {
        string weather = parts[2].Trim();
        if (weather == "none")
            _weather = ConditionId.None;
        else if (!line.Contains("[upkeep]"))
            _weather = _resolver.ResolveWeather(weather);
    }

    private void HandleFieldStart(string[] parts)
    {
        string effect = parts[2].Trim().ToLowerInvariant();
        if (effect.Contains("terrain"))
            _terrain = _resolver.ResolveTerrain(parts[2].Trim().Replace("move: ", ""));
        else if (effect.Contains("trick room"))
            _trickRoom = true;
    }

    private void HandleFieldEnd(string[] parts)
    {
        string effect = parts[2].Trim().ToLowerInvariant();
        if (effect.Contains("terrain"))
            _terrain = ConditionId.None;
        else if (effect.Contains("trick room"))
            _trickRoom = false;
    }

    private void HandleSideCondition(string[] parts, bool start)
    {
        string sideStr = parts[2].Trim();
        string side = sideStr.Contains("p1") ? "p1" : "p2";
        string effect = parts[3].Trim().ToLowerInvariant();

        ConditionId? cond = null;
        if (effect.Contains("tailwind")) cond = ConditionId.Tailwind;
        else if (effect.Contains("reflect")) cond = ConditionId.Reflect;
        else if (effect.Contains("light screen")) cond = ConditionId.LightScreen;
        else if (effect.Contains("aurora veil")) cond = ConditionId.AuroraVeil;

        if (cond.HasValue)
        {
            if (start)
                _sideConditions[side].Add(cond.Value);
            else
                _sideConditions[side].Remove(cond.Value);
        }
    }

    private void HandleSwap(string[] parts)
    {
        ParseSlot(parts[2].Trim(), out string slot, out _);
        string side = PlayerSide(slot);
        string slotA = $"{side}a";
        string slotB = $"{side}b";
        if (_active.ContainsKey(slotA) && _active.ContainsKey(slotB))
        {
            (_active[slotA], _active[slotB]) = (_active[slotB], _active[slotA]);
            (_slotSpecies[slotA], _slotSpecies[slotB]) = (
                _slotSpecies.GetValueOrDefault(slotB, ""),
                _slotSpecies.GetValueOrDefault(slotA, ""));
        }
    }

    private PokemonPerspective? GetActivePerspective(string side, string slot, bool isOwn)
    {
        string fullSlot = $"{side}{slot}";
        if (!_active.TryGetValue(fullSlot, out TrackedPokemon? p) || p.Fainted)
            return null;

        if (isOwn)
        {
            // Find matching own team pokemon for full data
            OwnTeamPokemon? own = _ownTeam.Find(o => o.Species == p.Species);
            if (own != null)
                return BuildOwnPerspective(own, slot == "a" ? 1 : 2, isActive: true, tracked: p);
        }

        return BuildOpponentPerspective(p.Species, p, slot == "a" ? 1 : 2, isActive: true);
    }

    private PokemonPerspective BuildOwnPerspective(
        OwnTeamPokemon own, int position, bool isActive, TrackedPokemon? tracked = null)
    {
        SpecieId specieId = _resolver.ResolveSpecies(own.Species);

        var moveSlots = new List<MoveSlot>();
        foreach (string moveId in own.Moves)
        {
            MoveId resolved = _resolver.ResolveMove(moveId);
            if (resolved != MoveId.None)
            {
                moveSlots.Add(new MoveSlot
                {
                    Id = resolved,
                    Move = resolved,
                    Pp = 10,
                    MaxPp = 10,
                });
            }
        }

        MoveType teraType = _resolver.ResolveTeraType(own.TeraType);
        bool isTera = tracked?.IsTerastallized ?? false;

        return new PokemonPerspective
        {
            Name = own.Species,
            Species = specieId,
            Level = own.Level,
            Gender = GenderId.N,
            Shiny = false,
            Hp = tracked?.Hp ?? own.Hp,
            MaxHp = tracked?.MaxHp ?? own.MaxHp,
            Fainted = tracked?.Fainted ?? own.Fainted,
            Status = tracked?.Status ?? _resolver.ResolveStatus(own.Status),
            MoveSlots = moveSlots,
            Boosts = tracked != null
                ? new BoostsTable
                {
                    Atk = tracked.AtkBoost,
                    Def = tracked.DefBoost,
                    SpA = tracked.SpABoost,
                    SpD = tracked.SpDBoost,
                    Spe = tracked.SpeBoost,
                }
                : new BoostsTable(),
            StoredStats = new StatsExceptHpTable(),
            Ability = _resolver.ResolveAbility(own.BaseAbility),
            Item = _resolver.ResolveItem(own.Item),
            Types = [],
            Terastallized = isTera ? teraType : null,
            TeraType = teraType,
            Volatiles = [],
            VolatilesWithDuration = new Dictionary<ConditionId, int?>(),
            Position = position,
            IsActive = isActive,
        };
    }

    private PokemonPerspective BuildOpponentPerspective(
        string species, TrackedPokemon tracked, int position, bool isActive)
    {
        SpecieId specieId = _resolver.ResolveSpecies(species);
        string side = OppSide ?? "p2";
        RevealedData? revealed = _revealed[side].GetValueOrDefault(species);

        // Opponent moves stay at empty (matching training distribution)
        // unless we want to fill revealed moves for opponent encoding

        return new PokemonPerspective
        {
            Name = species,
            Species = specieId,
            Level = 50,
            Gender = GenderId.N,
            Shiny = false,
            Hp = tracked.Hp,
            MaxHp = tracked.MaxHp,
            Fainted = tracked.Fainted,
            Status = tracked.Status,
            MoveSlots = [],
            Boosts = new BoostsTable
            {
                Atk = tracked.AtkBoost,
                Def = tracked.DefBoost,
                SpA = tracked.SpABoost,
                SpD = tracked.SpDBoost,
                Spe = tracked.SpeBoost,
            },
            StoredStats = new StatsExceptHpTable(),
            Ability = AbilityId.None, // Opponent ability not encoded (matches training)
            Item = ItemId.None,       // Opponent item not encoded (matches training)
            Types = [],
            Terastallized = tracked.IsTerastallized ? tracked.TeraType : null,
            TeraType = tracked.TeraType,
            Volatiles = [],
            VolatilesWithDuration = new Dictionary<ConditionId, int?>(),
            Position = position,
            IsActive = isActive,
        };
    }

    private List<LegalAction> BuildSlotActions(JsonElement slotData, Vocab vocab, int slotIndex)
    {
        var actions = new List<LegalAction>();

        // Check tera availability
        MoveType? teraType = null;
        if (slotData.TryGetProperty("canTerastallize", out JsonElement canTera))
        {
            string teraStr = canTera.GetString() ?? "";
            if (!string.IsNullOrEmpty(teraStr))
                teraType = _resolver.ResolveTeraType(teraStr);
        }

        // Parse moves
        if (slotData.TryGetProperty("moves", out JsonElement moves))
        {
            foreach (JsonElement move in moves.EnumerateArray())
            {
                bool disabled = move.TryGetProperty("disabled", out JsonElement dis)
                    && dis.ValueKind == JsonValueKind.True;
                if (disabled) continue;

                string moveId = move.TryGetProperty("id", out JsonElement id)
                    ? id.GetString() ?? "" : "";
                string moveName = move.TryGetProperty("move", out JsonElement mn)
                    ? mn.GetString() ?? "" : "";

                MoveId resolvedMove = _resolver.ResolveMove(moveId);
                if (resolvedMove == MoveId.None)
                    resolvedMove = _resolver.ResolveMove(moveName);

                int vocabIdx = vocab.GetMoveActionIndex(resolvedMove);

                // Determine target
                string target = move.TryGetProperty("target", out JsonElement tgt)
                    ? tgt.GetString() ?? "" : "";
                int targetLoc = ResolveTargetLocation(target);

                actions.Add(new LegalAction
                {
                    VocabIndex = vocabIdx,
                    ChoiceType = Sim.Choices.ChoiceType.Move,
                    MoveId = resolvedMove,
                    TargetLoc = targetLoc,
                });

                // Tera variant
                if (teraType.HasValue)
                {
                    actions.Add(new LegalAction
                    {
                        VocabIndex = vocabIdx,
                        ChoiceType = Sim.Choices.ChoiceType.Move,
                        MoveId = resolvedMove,
                        TargetLoc = targetLoc,
                        Terastallize = teraType,
                    });
                }
            }
        }

        // Check if trapped
        bool trapped = slotData.TryGetProperty("trapped", out JsonElement tr)
            && tr.ValueKind == JsonValueKind.True;

        // Switch actions (if not trapped)
        if (!trapped)
        {
            actions.AddRange(BuildSwitchActions(vocab));
        }

        // Fallback
        if (actions.Count == 0)
        {
            actions.Add(new LegalAction
                { VocabIndex = Vocab.NoneActionIndex, ChoiceType = Sim.Choices.ChoiceType.Pass });
        }

        return actions;
    }

    private List<LegalAction> BuildSwitchActions(Vocab vocab)
    {
        var actions = new List<LegalAction>();

        for (int i = 0; i < _ownTeam.Count; i++)
        {
            OwnTeamPokemon poke = _ownTeam[i];
            if (poke.Active || poke.Fainted) continue;

            SpecieId specieId = _resolver.ResolveSpecies(poke.Species);
            int switchIdx = vocab.GetSwitchActionIndex(specieId);

            actions.Add(new LegalAction
            {
                VocabIndex = switchIdx,
                ChoiceType = Sim.Choices.ChoiceType.Switch,
                SwitchIndex = i,
            });
        }

        return actions;
    }

    private Dictionary<ConditionId, int?> BuildSideConditions(string side)
    {
        var dict = new Dictionary<ConditionId, int?>();
        if (_sideConditions.TryGetValue(side, out HashSet<ConditionId>? conds))
        {
            foreach (ConditionId c in conds)
                dict[c] = null;
        }
        return dict;
    }

    private string ResolveSpecies(string slot, string nickname)
    {
        if (_slotSpecies.TryGetValue(slot, out string? species))
            return species;
        string key = $"{slot}: {nickname}";
        return _nicknameToSpecies.GetValueOrDefault(key, nickname);
    }

    private RevealedData GetRevealed(string side, string species)
    {
        if (!_revealed[side].TryGetValue(species, out RevealedData? data))
        {
            data = new RevealedData();
            _revealed[side][species] = data;
        }
        return data;
    }

    private string GetSpeciesName(SpecieId id)
    {
        return _resolver.GetSpeciesName(id);
    }

    private static string PlayerSide(string slot) => slot[..2]; // "p1a" -> "p1"

    private static int ResolveTargetLocation(string target)
    {
        return target switch
        {
            "normal" or "any" or "adjacentFoe" => 1, // Default to targeting slot 1
            "adjacentAlly" or "adjacentAllyOrSelf" => -1,
            _ => 0, // Auto-targeting (all, allySide, foeSide, etc.)
        };
    }

    // ── Protocol parsing helpers ─────────────────────────────────────────

    private static void ParseSlot(string raw, out string slot, out string nickname)
    {
        // "p1a: Incineroar" -> slot="p1a", nickname="Incineroar"
        int colonIdx = raw.IndexOf(':');
        if (colonIdx >= 0)
        {
            slot = raw[..colonIdx].Trim();
            nickname = raw[(colonIdx + 1)..].Trim();
        }
        else
        {
            slot = raw.Trim();
            nickname = "";
        }
    }

    private static void ParseSpeciesDetail(string detail, out string species, out int level)
    {
        // "Incineroar, L50, M" -> species="Incineroar", level=50
        string[] parts = detail.Split(',');
        species = parts[0].Trim();
        level = 50;
        for (int i = 1; i < parts.Length; i++)
        {
            string part = parts[i].Trim();
            if (part.StartsWith('L') && int.TryParse(part[1..], out int l))
                level = l;
        }
    }

    private static void ParseHp(string hpStr, out int hp, out int maxHp)
    {
        // "267/267" or "0 fnt" or "267/267 par" or "100/100"
        string numPart = hpStr.Split(' ')[0];
        int slashIdx = numPart.IndexOf('/');
        if (slashIdx >= 0)
        {
            int.TryParse(numPart[..slashIdx], out hp);
            int.TryParse(numPart[(slashIdx + 1)..], out maxHp);
            if (maxHp == 0) maxHp = 100;
        }
        else
        {
            hp = int.TryParse(numPart, out int v) ? v : 0;
            maxHp = 100;
        }
    }

    private static string? ParseStatusFromHp(string hpStr)
    {
        // "267/267 par" -> "par"
        string[] parts = hpStr.Split(' ');
        if (parts.Length >= 2 && parts[1] != "fnt")
            return parts[1];
        return null;
    }

    // --- Shadow Battle: public accessors ---
    // Note: OwnTeam is already exposed above as a public property.

    /// <summary>Team preview species per side. Key: "p1"/"p2".</summary>
    public IReadOnlyDictionary<string, List<string>> TeamPreview => _teamPreview;

    /// <summary>Active pokemon per slot. Key: "p1a", "p2b", etc.</summary>
    public IReadOnlyDictionary<string, TrackedPokemon> ActivePokemon => _active;

    /// <summary>Bench pokemon per side. Key: side -> species -> TrackedPokemon.</summary>
    public IReadOnlyDictionary<string, Dictionary<string, TrackedPokemon>> BenchPokemon => _bench;

    /// <summary>Revealed opponent info. Key: side -> species -> RevealedData.</summary>
    public IReadOnlyDictionary<string, Dictionary<string, RevealedData>> Revealed => _revealed;

    /// <summary>Which sides have used tera. Values: "p1", "p2".</summary>
    public IReadOnlySet<string> TeraUsed => _teraUsed;

    /// <summary>Current weather condition.</summary>
    public ConditionId Weather => _weather;

    /// <summary>Current terrain condition.</summary>
    public ConditionId Terrain => _terrain;

    /// <summary>Whether trick room is active.</summary>
    public bool TrickRoom => _trickRoom;

    /// <summary>Side conditions per side. Key: "p1"/"p2".</summary>
    public IReadOnlyDictionary<string, HashSet<ConditionId>> SideConditions => _sideConditions;

    /// <summary>The ShowdownNameResolver for external use.</summary>
    public ShowdownNameResolver Resolver => _resolver;

    /// <summary>Team order chosen at team preview per side.</summary>
    public IReadOnlyDictionary<string, string> TeamOrder => _teamOrder;

    // --- Shadow Battle: choice inference methods ---

    /// <summary>
    /// Build a Showdown-format choice string for the given side from inferred turn actions.
    /// Returns e.g. "move fakeout 1, move protect" or "switch 3, move thunderbolt 1"
    /// Returns null if no actions were inferred for this side.
    /// </summary>
    public string? BuildChoiceString(string side)
    {
        string slotA = $"{side}a";
        string slotB = $"{side}b";

        var choices = new List<string>();

        if (_turnActions.TryGetValue(slotA, out InferredAction? actionA))
            choices.Add(FormatInferredAction(actionA));
        else if (_active.TryGetValue(slotA, out TrackedPokemon? pA) && !pA.Fainted)
            choices.Add("pass"); // Active but no action observed — forced pass

        if (_turnActions.TryGetValue(slotB, out InferredAction? actionB))
            choices.Add(FormatInferredAction(actionB));
        else if (_active.TryGetValue(slotB, out TrackedPokemon? pB) && !pB.Fainted)
            choices.Add("pass"); // Active but no action observed — forced pass

        return choices.Count > 0 ? string.Join(", ", choices) : null;
    }

    private static string FormatInferredAction(InferredAction action)
    {
        if (action.IsSwitch)
            return $"switch {action.SwitchSlot}";

        string result = $"move {action.MoveName}";
        if (action.TargetLoc != 0)
            result += $" {action.TargetLoc}";
        if (action.Tera)
            result += " terastallize";
        return result;
    }

    /// <summary>
    /// Get observed HP values this turn for post-sim correction.
    /// Key: slot. Value: (hp, maxHp).
    /// </summary>
    public IReadOnlyDictionary<string, (int Hp, int MaxHp)> GetObservedHp() => _observedHp;

    /// <summary>
    /// Clear turn action tracking for the next turn.
    /// </summary>
    public void ClearTurnActions()
    {
        _turnActions.Clear();
        _observedHp.Clear();
        _forcedSwitchSlots.Clear();
    }

    /// <summary>
    /// Record the team order chosen during team preview.
    /// Called by ShowdownServer when it sees the team preview choice.
    /// </summary>
    public void SetTeamOrder(string side, string order)
    {
        _teamOrder[side] = order;
    }

    /// <summary>
    /// Map a protocol target identifier (e.g. "p2a: Flutter Mane") to a target location
    /// relative to the source slot.
    /// In doubles: 1 = foe left (slot a), 2 = foe right (slot b),
    ///            -1 = self/ally left, -2 = ally right
    /// </summary>
    private static int InferTargetLoc(string sourceSlot, string targetSlot)
    {
        string sourceSide = sourceSlot[..2]; // "p1" or "p2"
        string targetSide = targetSlot[..2];
        char targetPos = targetSlot[2]; // 'a' or 'b'

        if (sourceSide != targetSide)
        {
            // Targeting opponent
            return targetPos == 'a' ? 1 : 2;
        }
        else
        {
            // Targeting ally or self
            return targetPos == 'a' ? -1 : -2;
        }
    }

    /// <summary>
    /// Find the 1-based team slot index of a pokemon by species name for a given side.
    /// Uses team preview order if available, otherwise searches own team.
    /// </summary>
    private int FindTeamSlot(string side, string species)
    {
        if (side == MySide)
        {
            for (int i = 0; i < _ownTeam.Count; i++)
            {
                if (string.Equals(_ownTeam[i].Species, species, StringComparison.OrdinalIgnoreCase))
                    return i + 1; // 1-based
            }
        }
        else
        {
            // Opponent team — use team preview order
            var preview = _teamPreview.GetValueOrDefault(side);
            if (preview != null)
            {
                for (int i = 0; i < preview.Count; i++)
                {
                    if (string.Equals(preview[i], species, StringComparison.OrdinalIgnoreCase))
                        return i + 1;
                }
            }
        }

        return 1; // Fallback
    }
}

/// <summary>
/// Tracks a pokemon's battle state from protocol lines.
/// </summary>
public sealed class TrackedPokemon
{
    public string Species { get; set; } = "";
    public int Hp { get; set; } = 100;
    public int MaxHp { get; set; } = 100;
    public ConditionId Status { get; set; } = ConditionId.None;
    public bool Fainted { get; set; }
    public bool IsActive { get; set; }
    public int AtkBoost { get; set; }
    public int DefBoost { get; set; }
    public int SpABoost { get; set; }
    public int SpDBoost { get; set; }
    public int SpeBoost { get; set; }
    public MoveType TeraType { get; set; }
    public bool IsTerastallized { get; set; }
}

/// <summary>
/// Full own-team pokemon data from |request| JSON.
/// </summary>
public sealed class OwnTeamPokemon
{
    public string Species { get; init; } = "";
    public List<string> Moves { get; init; } = [];
    public string Ability { get; init; } = "";
    public string BaseAbility { get; init; } = "";
    public string Item { get; init; } = "";
    public string TeraType { get; init; } = "";
    public int Hp { get; init; } = 100;
    public int MaxHp { get; init; } = 100;
    public string? Status { get; init; }
    public bool Fainted { get; init; }
    public bool Active { get; init; }
    public int Level { get; init; } = 50;

    /// <summary>Computed stats from |request| JSON: {atk, def, spa, spd, spe}.</summary>
    public Dictionary<string, int> Stats { get; init; } = new();
}

/// <summary>
/// Progressively revealed info about a pokemon.
/// </summary>
public sealed class RevealedData
{
    public List<string> Moves { get; } = [];
    public string? Ability { get; set; }
    public string? Item { get; set; }
    public string? TeraType { get; set; }
}

/// <summary>
/// A single action recommendation with label and probability.
/// </summary>
public readonly struct ActionRecommendation
{
    public string Action { get; init; }
    public float Prob { get; init; }
}

/// <summary>
/// An action inferred from Showdown protocol output for shadow battle choice replay.
/// </summary>
public sealed class InferredAction
{
    /// <summary>True if this is a switch action, false for a move.</summary>
    public bool IsSwitch { get; init; }

    /// <summary>Showdown-format move name (lowercase, no special chars). Only for moves.</summary>
    public string MoveName { get; init; } = "";

    /// <summary>Target location for moves in doubles (1=foe left, 2=foe right, -1=ally left, -2=ally right, 0=no target).</summary>
    public int TargetLoc { get; init; }

    /// <summary>Whether the player terastallized with this action.</summary>
    public bool Tera { get; set; }

    /// <summary>1-based team slot index for switches.</summary>
    public int SwitchSlot { get; init; }
}

/// <summary>
/// Resolves Showdown string names to C# enum IDs using the Library.
/// </summary>
public sealed class ShowdownNameResolver
{
    private readonly Dictionary<string, SpecieId> _speciesByName;
    private readonly Dictionary<string, MoveId> _movesByName;
    private readonly Dictionary<string, MoveId> _movesByShowdownId;
    private readonly Dictionary<string, AbilityId> _abilitiesByName;
    private readonly Dictionary<string, AbilityId> _abilitiesByShowdownId;
    private readonly Dictionary<string, ItemId> _itemsByName;
    private readonly Dictionary<string, ItemId> _itemsByShowdownId;
    private readonly Library _library;

    public ShowdownNameResolver(Library library)
    {
        _library = library;

        _speciesByName = new Dictionary<string, SpecieId>(StringComparer.OrdinalIgnoreCase);
        foreach (var (id, species) in library.Species)
            _speciesByName.TryAdd(species.Name, id);

        _movesByName = new Dictionary<string, MoveId>(StringComparer.OrdinalIgnoreCase);
        _movesByShowdownId = new Dictionary<string, MoveId>(StringComparer.OrdinalIgnoreCase);
        foreach (var (id, move) in library.Moves)
        {
            _movesByName.TryAdd(move.Name, id);
            // Showdown ID: lowercase, no spaces/hyphens (e.g. "fakeout")
            _movesByShowdownId.TryAdd(ToShowdownId(move.Name), id);
        }

        _abilitiesByName = new Dictionary<string, AbilityId>(StringComparer.OrdinalIgnoreCase);
        _abilitiesByShowdownId = new Dictionary<string, AbilityId>(StringComparer.OrdinalIgnoreCase);
        foreach (var (id, ability) in library.Abilities)
        {
            _abilitiesByName.TryAdd(ability.Name, id);
            _abilitiesByShowdownId.TryAdd(ToShowdownId(ability.Name), id);
        }

        _itemsByName = new Dictionary<string, ItemId>(StringComparer.OrdinalIgnoreCase);
        _itemsByShowdownId = new Dictionary<string, ItemId>(StringComparer.OrdinalIgnoreCase);
        foreach (var (id, item) in library.Items)
        {
            _itemsByName.TryAdd(item.Name, id);
            _itemsByShowdownId.TryAdd(ToShowdownId(item.Name), id);
        }
    }

    public SpecieId ResolveSpecies(string name)
    {
        if (_speciesByName.TryGetValue(name, out SpecieId id)) return id;

        // Handle cosmetic form variants that map to their base species
        string? baseName = GetCosmeticFormBase(name);
        if (baseName != null && _speciesByName.TryGetValue(baseName, out id)) return id;

        return default;
    }

    /// <summary>
    /// Maps cosmetic form names (e.g. "Minior-Indigo", "Vivillon-Continental") to their
    /// base species name. Returns null if the name is not a known cosmetic form.
    /// </summary>
    private static string? GetCosmeticFormBase(string name)
    {
        // Minior color forms (all map to base Minior except Minior-Meteor which is separate)
        if (name.StartsWith("Minior-", StringComparison.OrdinalIgnoreCase) &&
            !name.Equals("Minior-Meteor", StringComparison.OrdinalIgnoreCase))
            return "Minior";

        // Vivillon regional pattern forms (Fancy and Pokeball are separate entries)
        if (name.StartsWith("Vivillon-", StringComparison.OrdinalIgnoreCase) &&
            !name.Equals("Vivillon-Fancy", StringComparison.OrdinalIgnoreCase) &&
            !name.Equals("Vivillon-Pokeball", StringComparison.OrdinalIgnoreCase))
            return "Vivillon";

        // Florges color forms
        if (name.StartsWith("Florges-", StringComparison.OrdinalIgnoreCase))
            return "Florges";

        // Furfrou trim forms
        if (name.StartsWith("Furfrou-", StringComparison.OrdinalIgnoreCase))
            return "Furfrou";

        // Alcremie flavor/topping forms
        if (name.StartsWith("Alcremie-", StringComparison.OrdinalIgnoreCase))
            return "Alcremie";

        // Sawsbuck seasonal forms
        if (name.StartsWith("Sawsbuck-", StringComparison.OrdinalIgnoreCase))
            return "Sawsbuck";

        // Gastrodon forms
        if (name.StartsWith("Gastrodon-", StringComparison.OrdinalIgnoreCase))
            return "Gastrodon";

        // Shellos forms
        if (name.StartsWith("Shellos-", StringComparison.OrdinalIgnoreCase))
            return "Shellos";

        // Deerling seasonal forms
        if (name.StartsWith("Deerling-", StringComparison.OrdinalIgnoreCase))
            return "Deerling";

        return null;
    }

    public MoveId ResolveMove(string nameOrId)
    {
        if (string.IsNullOrEmpty(nameOrId)) return MoveId.None;
        if (_movesByName.TryGetValue(nameOrId, out MoveId id)) return id;
        if (_movesByShowdownId.TryGetValue(nameOrId, out id)) return id;
        return MoveId.None;
    }

    public AbilityId ResolveAbility(string nameOrId)
    {
        if (string.IsNullOrEmpty(nameOrId)) return AbilityId.None;
        if (_abilitiesByName.TryGetValue(nameOrId, out AbilityId id)) return id;
        if (_abilitiesByShowdownId.TryGetValue(nameOrId, out id)) return id;
        return AbilityId.None;
    }

    public ItemId ResolveItem(string nameOrId)
    {
        if (string.IsNullOrEmpty(nameOrId)) return ItemId.None;
        if (_itemsByName.TryGetValue(nameOrId, out ItemId id)) return id;
        if (_itemsByShowdownId.TryGetValue(nameOrId, out id)) return id;
        return ItemId.None;
    }

    public ConditionId ResolveStatus(string? status)
    {
        if (string.IsNullOrEmpty(status)) return ConditionId.None;
        return status switch
        {
            "par" => ConditionId.Paralysis,
            "brn" => ConditionId.Burn,
            "slp" => ConditionId.Sleep,
            "psn" => ConditionId.Poison,
            "tox" => ConditionId.Toxic,
            "frz" => ConditionId.Freeze,
            _ => ConditionId.None,
        };
    }

    public MoveType ResolveTeraType(string type)
    {
        if (string.IsNullOrEmpty(type)) return default;
        if (Enum.TryParse<MoveType>(type, ignoreCase: true, out MoveType mt))
            return mt;
        return default;
    }

    public ConditionId ResolveWeather(string weather)
    {
        string lower = weather.ToLowerInvariant();
        if (lower.Contains("sunnyday") || lower.Contains("sun")) return ConditionId.SunnyDay;
        if (lower.Contains("raindance") || lower.Contains("rain")) return ConditionId.RainDance;
        if (lower.Contains("sandstorm") || lower.Contains("sand")) return ConditionId.Sandstorm;
        if (lower.Contains("snow") || lower.Contains("hail")) return ConditionId.Snowscape;
        if (lower.Contains("desolateland")) return ConditionId.DesolateLand;
        if (lower.Contains("primordialsea")) return ConditionId.PrimordialSea;
        return ConditionId.None;
    }

    public ConditionId ResolveTerrain(string terrain)
    {
        string lower = terrain.ToLowerInvariant();
        if (lower.Contains("electric")) return ConditionId.ElectricTerrain;
        if (lower.Contains("grassy")) return ConditionId.GrassyTerrain;
        if (lower.Contains("psychic")) return ConditionId.PsychicTerrain;
        if (lower.Contains("misty")) return ConditionId.MistyTerrain;
        return ConditionId.None;
    }

    public string GetSpeciesName(SpecieId id)
    {
        return _library.Species.TryGetValue(id, out var s) ? s.Name : id.ToString();
    }

    /// <summary>
    /// Convert display name to Showdown ID (lowercase, no spaces/hyphens/special chars).
    /// "Fake Out" -> "fakeout", "King's Rock" -> "kingsrock"
    /// </summary>
    private static string ToShowdownId(string name)
    {
        return Regex.Replace(name.ToLowerInvariant(), @"[^a-z0-9]", "");
    }
}
