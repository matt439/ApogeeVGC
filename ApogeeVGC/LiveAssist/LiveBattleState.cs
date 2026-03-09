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
/// Tracks the state of a single Pokemon during a live-assisted battle.
/// </summary>
public class LivePokemonState
{
    public SpecieId Species { get; set; }
    public string Name { get; set; } = "";
    public MoveId[] Moves { get; set; } = [MoveId.None, MoveId.None, MoveId.None, MoveId.None];
    public AbilityId Ability { get; set; } = AbilityId.None;
    public ItemId Item { get; set; } = ItemId.None;
    public MoveType TeraType { get; set; }
    public int HpPercent { get; set; } = 100;
    public bool Fainted { get; set; }
    public ConditionId Status { get; set; } = ConditionId.None;
    public int AtkBoost { get; set; }
    public int DefBoost { get; set; }
    public int SpABoost { get; set; }
    public int SpDBoost { get; set; }
    public int SpeBoost { get; set; }
    public bool IsTerastallized { get; set; }
    public bool IsActive { get; set; }

    /// <summary>
    /// Convert to PokemonPerspective for model encoding.
    /// </summary>
    public PokemonPerspective ToPerspective(int position, bool isOwn)
    {
        var moveSlots = new List<MoveSlot>();
        foreach (var moveId in Moves)
        {
            if (moveId != MoveId.None)
            {
                moveSlots.Add(new MoveSlot
                {
                    Id = moveId,
                    Move = moveId,
                    Pp = 10,
                    MaxPp = 10,
                });
            }
        }

        return new PokemonPerspective
        {
            Name = Name,
            Species = Species,
            Level = 50,
            Gender = GenderId.N,
            Shiny = false,
            Hp = HpPercent,
            MaxHp = 100,
            Fainted = Fainted || HpPercent <= 0,
            Status = Status,
            MoveSlots = moveSlots,
            Boosts = new BoostsTable
            {
                Atk = AtkBoost,
                Def = DefBoost,
                SpA = SpABoost,
                SpD = SpDBoost,
                Spe = SpeBoost,
            },
            StoredStats = new StatsExceptHpTable(),
            Ability = Ability,
            Item = Item,
            Types = [],
            Terastallized = IsTerastallized ? TeraType : null,
            TeraType = TeraType,
            Volatiles = [],
            VolatilesWithDuration = new Dictionary<ConditionId, int?>(),
            Position = position,
            IsActive = IsActive,
        };
    }

    public void ResetBoosts()
    {
        AtkBoost = 0;
        DefBoost = 0;
        SpABoost = 0;
        SpDBoost = 0;
        SpeBoost = 0;
    }
}

/// <summary>
/// Tracks the full state of a live-assisted battle across turns.
/// </summary>
public class LiveBattleState
{
    // Full team data (6 Pokemon each)
    public LivePokemonState[] MyTeam { get; } = new LivePokemonState[6];
    public LivePokemonState[] OppTeam { get; } = new LivePokemonState[6];

    // Which 4 indices (into MyTeam) were brought to battle
    public int[] MyBrought { get; set; } = [];

    // Which of the brought Pokemon are currently active (indices into MyBrought)
    public int[] MyActiveSlots { get; set; } = [0, 1]; // default: first 2 brought are leads

    // Opponent active slots (indices into OppTeam of seen Pokemon)
    public int[] OppActiveSlots { get; set; } = [];

    // Field state
    public ConditionId Weather { get; set; } = ConditionId.None;
    public ConditionId Terrain { get; set; } = ConditionId.None;
    public bool TrickRoom { get; set; }
    public bool MyTailwind { get; set; }
    public bool OppTailwind { get; set; }
    public bool MyReflect { get; set; }
    public bool OppReflect { get; set; }
    public bool MyLightScreen { get; set; }
    public bool OppLightScreen { get; set; }
    public bool MyAuroraVeil { get; set; }
    public bool OppAuroraVeil { get; set; }

    // Tera tracking (once per game)
    public bool MyTeraUsed { get; set; }
    public bool OppTeraUsed { get; set; }

    public int TurnCounter { get; set; } = 1;

    public LiveBattleState()
    {
        for (int i = 0; i < 6; i++)
        {
            MyTeam[i] = new LivePokemonState();
            OppTeam[i] = new LivePokemonState();
        }
    }

    /// <summary>
    /// Get the brought Pokemon (4) for my side.
    /// </summary>
    public LivePokemonState[] GetMyBrought()
    {
        var result = new LivePokemonState[MyBrought.Length];
        for (int i = 0; i < MyBrought.Length; i++)
            result[i] = MyTeam[MyBrought[i]];
        return result;
    }

    /// <summary>
    /// Build a BattlePerspective from the current tracked state for model inference.
    /// </summary>
    public BattlePerspective BuildPerspective()
    {
        // Mark active/inactive for encoding
        var myBrought = GetMyBrought();
        foreach (var p in myBrought) p.IsActive = false;
        foreach (var slot in MyActiveSlots)
        {
            if (slot >= 0 && slot < myBrought.Length)
                myBrought[slot].IsActive = true;
        }

        foreach (var p in OppTeam) p.IsActive = false;
        foreach (var slot in OppActiveSlots)
        {
            if (slot >= 0 && slot < OppTeam.Length)
                OppTeam[slot].IsActive = true;
        }

        // Build own Pokemon perspectives (all 4 brought)
        var myPokemon = new List<PokemonPerspective>();
        for (int i = 0; i < myBrought.Length; i++)
            myPokemon.Add(myBrought[i].ToPerspective(i + 1, isOwn: true));

        // Build own active list
        var myActive = new PokemonPerspective?[2];
        for (int i = 0; i < MyActiveSlots.Length && i < 2; i++)
        {
            int slot = MyActiveSlots[i];
            if (slot >= 0 && slot < myPokemon.Count)
                myActive[i] = myPokemon[slot];
        }

        // Build opponent Pokemon perspectives (all seen)
        var oppPokemon = new List<PokemonPerspective>();
        for (int i = 0; i < OppTeam.Length; i++)
        {
            if (OppTeam[i].Species != default)
                oppPokemon.Add(OppTeam[i].ToPerspective(i + 1, isOwn: false));
        }

        // Build opponent active list
        var oppActive = new PokemonPerspective?[2];
        for (int i = 0; i < OppActiveSlots.Length && i < 2; i++)
        {
            int slot = OppActiveSlots[i];
            if (slot >= 0 && slot < oppPokemon.Count)
                oppActive[i] = oppPokemon[slot];
        }

        // Side conditions
        var mySideConditions = new Dictionary<ConditionId, int?>();
        if (MyTailwind) mySideConditions[ConditionId.Tailwind] = null;
        if (MyReflect) mySideConditions[ConditionId.Reflect] = null;
        if (MyLightScreen) mySideConditions[ConditionId.LightScreen] = null;
        if (MyAuroraVeil) mySideConditions[ConditionId.AuroraVeil] = null;

        var oppSideConditions = new Dictionary<ConditionId, int?>();
        if (OppTailwind) oppSideConditions[ConditionId.Tailwind] = null;
        if (OppReflect) oppSideConditions[ConditionId.Reflect] = null;
        if (OppLightScreen) oppSideConditions[ConditionId.LightScreen] = null;
        if (OppAuroraVeil) oppSideConditions[ConditionId.AuroraVeil] = null;

        // Field
        var pseudoWeather = new List<ConditionId>();
        if (TrickRoom) pseudoWeather.Add(ConditionId.TrickRoom);

        return new BattlePerspective
        {
            PerspectiveType = BattlePerspectiveType.InBattle,
            TurnCounter = TurnCounter,
            PlayerSide = new SidePlayerPerspective
            {
                Team = [],
                Pokemon = myPokemon,
                Active = myActive,
                SideConditionsWithDuration = mySideConditions,
            },
            OpponentSide = new SideOpponentPerspective
            {
                Pokemon = oppPokemon,
                Active = oppActive,
                SideConditionsWithDuration = oppSideConditions,
            },
            Field = new FieldPerspective
            {
                Weather = Weather,
                Terrain = Terrain,
                PseudoWeather = pseudoWeather,
                PseudoWeatherWithDuration = new Dictionary<ConditionId, int?>(),
            },
        };
    }

    /// <summary>
    /// Build a BattlePerspective for team preview (all 6 Pokemon per side).
    /// </summary>
    public BattlePerspective BuildTeamPreviewPerspective()
    {
        var myPokemon = new List<PokemonPerspective>();
        for (int i = 0; i < 6; i++)
        {
            if (MyTeam[i].Species != default)
                myPokemon.Add(MyTeam[i].ToPerspective(i + 1, isOwn: true));
        }

        var oppPokemon = new List<PokemonPerspective>();
        for (int i = 0; i < 6; i++)
        {
            if (OppTeam[i].Species != default)
                oppPokemon.Add(OppTeam[i].ToPerspective(i + 1, isOwn: false));
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
}
