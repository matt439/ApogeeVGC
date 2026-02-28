using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Mcts;

public sealed class StateEncoder
{
    private readonly Vocab _vocab;

    public const int NumSpeciesSlots = 8;
    public const int ActiveDim = 35;
    public const int BenchDim = 10;
    public const int FieldDim = 20;
    public const int NumericDim = 4 * ActiveDim + 4 * BenchDim + FieldDim; // 200

    public StateEncoder(Vocab vocab)
    {
        _vocab = vocab;
    }

    public (long[] SpeciesIds, float[] Numeric) Encode(BattlePerspective perspective)
    {
        var speciesIds = new long[NumSpeciesSlots];
        var numeric = new float[NumericDim];

        var playerSide = perspective.PlayerSide;
        var opponentSide = perspective.OpponentSide;

        // Active Pokemon
        var myActiveA = playerSide.Active.Count > 0 ? playerSide.Active[0] : null;
        var myActiveB = playerSide.Active.Count > 1 ? playerSide.Active[1] : null;
        var oppActiveA = opponentSide.Active.Count > 0 ? opponentSide.Active[0] : null;
        var oppActiveB = opponentSide.Active.Count > 1 ? opponentSide.Active[1] : null;

        // Species IDs for active slots
        speciesIds[0] = myActiveA != null ? _vocab.GetSpeciesIndex(myActiveA.Species) : Vocab.PadIndex;
        speciesIds[1] = myActiveB != null ? _vocab.GetSpeciesIndex(myActiveB.Species) : Vocab.PadIndex;
        speciesIds[2] = oppActiveA != null ? _vocab.GetSpeciesIndex(oppActiveA.Species) : Vocab.PadIndex;
        speciesIds[3] = oppActiveB != null ? _vocab.GetSpeciesIndex(oppActiveB.Species) : Vocab.PadIndex;

        // Encode active numeric features
        EncodeActive(numeric, 0, myActiveA);
        EncodeActive(numeric, ActiveDim, myActiveB);
        EncodeActive(numeric, 2 * ActiveDim, oppActiveA);
        EncodeActive(numeric, 3 * ActiveDim, oppActiveB);

        // Bench Pokemon: filter out active from the full team
        var myBench = GetBenchPokemon(playerSide.Pokemon);
        var oppBench = GetBenchPokemon(opponentSide.Pokemon);

        // Species IDs for bench slots
        speciesIds[4] = myBench.Count > 0 ? _vocab.GetSpeciesIndex(myBench[0].Species) : Vocab.PadIndex;
        speciesIds[5] = myBench.Count > 1 ? _vocab.GetSpeciesIndex(myBench[1].Species) : Vocab.PadIndex;
        speciesIds[6] = oppBench.Count > 0 ? _vocab.GetSpeciesIndex(oppBench[0].Species) : Vocab.PadIndex;
        speciesIds[7] = oppBench.Count > 1 ? _vocab.GetSpeciesIndex(oppBench[1].Species) : Vocab.PadIndex;

        // Encode bench numeric features
        int benchOffset = 4 * ActiveDim; // 140
        EncodeBench(numeric, benchOffset, myBench.Count > 0 ? myBench[0] : null);
        EncodeBench(numeric, benchOffset + BenchDim, myBench.Count > 1 ? myBench[1] : null);
        EncodeBench(numeric, benchOffset + 2 * BenchDim, oppBench.Count > 0 ? oppBench[0] : null);
        EncodeBench(numeric, benchOffset + 3 * BenchDim, oppBench.Count > 1 ? oppBench[1] : null);

        // Field features
        int fieldOffset = 4 * ActiveDim + 4 * BenchDim; // 180
        EncodeField(
            numeric, fieldOffset,
            perspective.Field,
            playerSide.SideConditionsWithDuration,
            opponentSide.SideConditionsWithDuration,
            perspective.TurnCounter);

        return (speciesIds, numeric);
    }

    private static List<PokemonPerspective> GetBenchPokemon(IReadOnlyList<PokemonPerspective> pokemon)
    {
        var bench = new List<PokemonPerspective>(2);
        for (int i = 0; i < pokemon.Count; i++)
        {
            if (!pokemon[i].IsActive)
            {
                bench.Add(pokemon[i]);
                if (bench.Count >= 2) break;
            }
        }
        return bench;
    }

    private static void EncodeActive(float[] feat, int off, PokemonPerspective? p)
    {
        if (p == null) return; // All zeros for empty/fainted slot

        // [0] HP fraction
        feat[off] = p.MaxHp > 0 ? (float)p.Hp / p.MaxHp : 0f;

        // [1] Fainted
        feat[off + 1] = p.Fainted ? 1f : 0f;

        // [2..8] Status one-hot (7 slots)
        feat[off + 2 + GetStatusIndex(p.Status)] = 1f;

        // [9..13] Boosts normalized by /6
        feat[off + 9] = p.Boosts.Atk / 6f;
        feat[off + 10] = p.Boosts.Def / 6f;
        feat[off + 11] = p.Boosts.SpA / 6f;
        feat[off + 12] = p.Boosts.SpD / 6f;
        feat[off + 13] = p.Boosts.Spe / 6f;

        // [14] Is terastallized
        // [15..34] Tera type one-hot (20 slots: 0=none, 1..19=types)
        if (p.Terastallized != null)
        {
            feat[off + 14] = 1f;
            feat[off + 15 + GetTeraTypeIndex(p.Terastallized.Value)] = 1f;
        }
        else
        {
            feat[off + 15] = 1f; // "none" tera slot
        }
    }

    private static void EncodeBench(float[] feat, int off, PokemonPerspective? p)
    {
        if (p == null) return; // All zeros — includes present=0

        // [0] HP fraction
        feat[off] = p.MaxHp > 0 ? (float)p.Hp / p.MaxHp : 0f;

        // [1] Fainted
        feat[off + 1] = p.Fainted ? 1f : 0f;

        // [2..8] Status one-hot (7 slots)
        feat[off + 2 + GetStatusIndex(p.Status)] = 1f;

        // [9] Present on team
        feat[off + 9] = 1f;
    }

    private static void EncodeField(
        float[] feat, int off,
        FieldPerspective field,
        IReadOnlyDictionary<ConditionId, int?> mySideConditions,
        IReadOnlyDictionary<ConditionId, int?> oppSideConditions,
        int turnCounter)
    {
        // [0..4] Weather one-hot (5 slots)
        feat[off + GetWeatherIndex(field.Weather)] = 1f;

        // [5..9] Terrain one-hot (5 slots)
        feat[off + 5 + GetTerrainIndex(field.Terrain)] = 1f;

        // [10] Trick Room
        if (field.PseudoWeather.Contains(ConditionId.TrickRoom))
            feat[off + 10] = 1f;

        // [11..18] Side conditions (my/opp × tailwind/reflect/light_screen/aurora_veil)
        feat[off + 11] = mySideConditions.ContainsKey(ConditionId.Tailwind) ? 1f : 0f;
        feat[off + 12] = oppSideConditions.ContainsKey(ConditionId.Tailwind) ? 1f : 0f;
        feat[off + 13] = mySideConditions.ContainsKey(ConditionId.Reflect) ? 1f : 0f;
        feat[off + 14] = oppSideConditions.ContainsKey(ConditionId.Reflect) ? 1f : 0f;
        feat[off + 15] = mySideConditions.ContainsKey(ConditionId.LightScreen) ? 1f : 0f;
        feat[off + 16] = oppSideConditions.ContainsKey(ConditionId.LightScreen) ? 1f : 0f;
        feat[off + 17] = mySideConditions.ContainsKey(ConditionId.AuroraVeil) ? 1f : 0f;
        feat[off + 18] = oppSideConditions.ContainsKey(ConditionId.AuroraVeil) ? 1f : 0f;

        // [19] Turn number normalized
        feat[off + 19] = turnCounter / 20f;
    }

    /// <summary>
    /// Maps ConditionId status to one-hot index (0=none, 1=par, 2=brn, 3=slp, 4=psn, 5=tox, 6=frz).
    /// </summary>
    private static int GetStatusIndex(ConditionId status) => status switch
    {
        ConditionId.Paralysis => 1,
        ConditionId.Burn => 2,
        ConditionId.Sleep => 3,
        ConditionId.Poison => 4,
        ConditionId.Toxic => 5,
        ConditionId.Freeze => 6,
        _ => 0,
    };

    /// <summary>
    /// Maps ConditionId weather to one-hot index (0=none, 1=sun, 2=rain, 3=sand, 4=snow).
    /// </summary>
    private static int GetWeatherIndex(ConditionId weather) => weather switch
    {
        ConditionId.SunnyDay or ConditionId.DesolateLand => 1,
        ConditionId.RainDance or ConditionId.PrimordialSea => 2,
        ConditionId.Sandstorm => 3,
        ConditionId.Snowscape => 4,
        _ => 0,
    };

    /// <summary>
    /// Maps ConditionId terrain to one-hot index (0=none, 1=electric, 2=grassy, 3=psychic, 4=misty).
    /// </summary>
    private static int GetTerrainIndex(ConditionId terrain) => terrain switch
    {
        ConditionId.ElectricTerrain => 1,
        ConditionId.GrassyTerrain => 2,
        ConditionId.PsychicTerrain => 3,
        ConditionId.MistyTerrain => 4,
        _ => 0,
    };

    /// <summary>
    /// Maps MoveType to tera type one-hot index (1-based; 0 is used for "none" at the call site).
    /// MoveType enum order matches Python TYPES list: Normal=0→1, Fire=1→2, ..., Stellar=18→19.
    /// </summary>
    private static int GetTeraTypeIndex(MoveType teraType)
    {
        int idx = (int)teraType + 1;
        return idx is >= 1 and <= 19 ? idx : 0;
    }
}
