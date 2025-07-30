namespace ApogeeVGC_CS.sim
{
    /**
     * Describes the acceptable target(s) of a move.
     * adjacentAlly - Only relevant to Doubles or Triples, the move only targets an ally of the user.
     * adjacentAllyOrSelf - The move can target the user or its ally.
     * adjacentFoe - The move can target a foe, but not (in Triples) a distant foe.
     * all - The move targets the field or all Pokémon at once.
     * allAdjacent - The move is a spread move that also hits the user's ally.
     * allAdjacentFoes - The move is a spread move.
     * allies - The move affects all active Pokémon on the user's team.
     * allySide - The move adds a side condition on the user's side.
     * allyTeam - The move affects all unfainted Pokémon on the user's team.
     * any - The move can hit any other active Pokémon, not just those adjacent.
     * foeSide - The move adds a side condition on the foe's side.
     * normal - The move can hit one adjacent Pokémon of your choice.
     * randomNormal - The move targets an adjacent foe at random.
     * scripted - The move targets the foe that damaged the user.
     * self - The move affects the user of the move.
     */
    public enum MoveTarget
    {
        AdjacentAlly,
        AdjacentAllyOrSelf,
        AdjacentFoe,
        All,
        AllAdjacent,
        AllAdjacentFoes,
        Allies,
        AllySide,
        AllyTeam,
        Any,
        FoeSide,
        Normal,
        RandomNormal,
        Scripted,
        Self,
        None, // Added for DataMove.SelfSwitch
    }

    public class MoveFlags
    {
        public bool? AllyAnim { get; init; }
        public bool? BypassSub { get; init; }
        public bool? Bite { get; init; }
        public bool? Bullet { get; init; }
        public bool? CantUinitwice { get; init; }
        public bool? Charge { get; init; }
        public bool? Contact { get; init; }
        public bool? Dance { get; init; }
        public bool? Defrost { get; init; }
        public bool? Distance { get; init; }
        public bool? FailCopycat { get; init; }
        public bool? FailEncore { get; init; }
        public bool? FailInstruct { get; init; }
        public bool? FailMeFirst { get; init; }
        public bool? FailMimic { get; init; }
        public bool? FutureMove { get; init; }
        public bool? Gravity { get; init; }
        public bool? Heal { get; init; }
        public bool? Metronome { get; init; }
        public bool? Mirror { get; init; }
        public bool? MustPressure { get; init; }
        public bool? NoAssist { get; init; }
        public bool? NonSky { get; init; }
        public bool? NoParentalBond { get; init; }
        public bool? NoSketch { get; init; }
        public bool? NoSleepTalk { get; init; }
        public bool? PledgeCombo { get; init; }
        public bool? Powder { get; init; }
        public bool? Protect { get; init; }
        public bool? Pulse { get; init; }
        public bool? Punch { get; init; }
        public bool? Recharge { get; init; }
        public bool? Reflectable { get; init; }
        public bool? Slicing { get; init; }
        public bool? Snatch { get; init; }
        public bool? Sound { get; init; }
        public bool? Wind { get; init; }
    }

    public interface IHitEffect
    {
        // public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; }
        public SparseBoostsTable? Boosts { get; }
        public Id? Status { get; }
        public Id? VolatileStatus { get; }
        public string? SideCondition { get; }
        public string? SlotCondition { get; }
        public string? PseudoWeather { get; }
        public string? Terrain { get; }
        public Id? Weather { get; }
    }
    public interface ISecondaryEffect : IHitEffect
    {
        public int? Chance { get;}
        public Ability? Ability { get; }
        public bool? KingsRock { get; }
        public IHitEffect? Self { get; }
        // Added this because I commented it out of IHitEffect
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; }
    }

    public class SecondaryEffect : ISecondaryEffect
    {
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; init; }
        public SparseBoostsTable? Boosts { get; init; }
        public Id? Status { get; init; }
        public Id? VolatileStatus { get; init; }
        public string? SideCondition { get; init; }
        public string? SlotCondition { get; init; }
        public string? PseudoWeather { get; init; }
        public string? Terrain { get; init; }
        public Id? Weather { get; init; }
        public int? Chance { get; init; }
        public Ability? Ability { get; init; }
        public bool? KingsRock { get; init; }
        public IHitEffect? Self { get; init; }
    }

    public interface IMoveEventMethods
    {
        // Return int, false (null), or null
        public Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? BasePowerCallback { get; }
        // Return true to stop the move from being used
        public Func<Battle, Pokemon, Pokemon?, ActiveMove, bool?>? BeforeMoveCallback { get; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? BeforeTurnCallback { get; }
        // Return int or false (null)
        public Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? DamageCallback { get; }
        public Action<Battle, Pokemon>? PriorityChargeCallback { get; }
        public Action<Battle, Pokemon>? OnDisableMove { get; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; }
        public int? OnDamagePriority { get; }
        // Return int, bool, or null
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; }
        // Return int or void (null)
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHitField { get; }
        // Return bool, null, empty string, or void
        public Func<Battle, Side, Pokemon, ActiveMove, object?>? OnHitSide { get; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveFail { get; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTry { get; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; }
        public Func<Battle, Side, Pokemon, ActiveMove, object?>? OnTryHitSide { get; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryImmunity { get; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnUseMoveMessage { get; }
    }

    public enum PokemonType
    {
        Normal,
        Fire,
        Water,
        Electric,
        Grass,
        Ice,
        Fighting,
        Poison,
        Ground,
        Flying,
        Psychic,
        Bug,
        Rock,
        Ghost,
        Dragon,
        Dark,
        Steel,
        Fairy,
        Unknown,
    }

    public enum ContestType
    {
        Cool,
        Beauty,
        Cute,
        Smart,
        Tough,
        Sheen
    }

    public enum PokemonOrigin
    {
        Target,
        Source,
    }

    public interface IMoveData : IEffectData, IMoveEventMethods, IHitEffect;

    public class MoveData : IMoveData
    {
        public int? Num { get; init; }
        public bool? SpreadHit { get; init; }
        public IConditionData? Condition { get; init; }
        public int BasePower { get; init; }
        public required object Accuracy
        {
            get;
            init
            {
                if (value is bool or double or int)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Accuracy must be a boolean, integer or double.");
                }
            }
        }
        public required int Pp { get; init; }
        public required MoveCategory Category { get; init; }
        public required PokemonType Type { get; init; }
        public required int Priority { get; init; }
        public required MoveTarget Target { get; init; }
        public required MoveFlags Flags { get; init; }
        public string? RealMove { get; init; } // For Hidden Power
        public object? Damage
        {
            get;
            init
            {
                if (field is int or double or string or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Damage must be a number, string, or an array of numbers.");
                }
            }
        }
        public ContestType? ContestType { get; init; }
        public bool? NoPpBoosts { get; init; }
        public object? IsZ
        {
            get;
            init
            {
                if (value is bool or IdEntry or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("IsZ must be bool or IdEntry");
                }
            }
        }
        public ZMoveData? ZMove { get; init; }
        public object? IsMax
        {
            get;
            init
            {
                if (value is bool or string or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("IsMaz must be bool, string or null");
                }
            }
        }
        public MaxMoveData? MaxMove { get; init; }

        public object? Ohko // bool or "Ice"
        {
            get;
            init
            {
                switch (value)
                {
                    case bool or null:
                        field = value;
                        break;
                    case string s:
                    {
                        if (s == "Ice")
                        {
                            field = s;
                        }

                        break;
                    }
                    default:
                        throw new ArgumentException("Ohko must be bool or 'Ice'");
                }
            }
        }
        public bool? ThawsTarget { get; init; }
        public int[]? Heal { get; init; }
        public bool? ForceSwitch { get; init; }
        public object? SelfSwitch // "copyvolatile", "shedtail", or bool
        {
            get;
            init
            {
                switch (value)
                {
                    case bool or null:
                        field = value;
                        break;
                    case string s:
                    {
                        if (s is "copyvolatile" or "shedtail")
                        {
                            field = s;
                        }
                        break;
                    }
                    default:
                        throw new ArgumentException("Ohko must be bool, 'copyvolatile', or 'shedtail'");
                }
            }
        } 
        public SelfBoostData? SelfBoost { get; init; }
        public object? SelfDestruct // "always", "ifHit", or bool
        {
            get;
            init
            {
                switch (value)
                {
                    case bool or null:
                        field = value;
                        break;
                    case string s:
                    {
                        if (s is "always" or "ifHit")
                        {
                            field = s;
                        }
                        break;
                    }
                    default:
                        throw new ArgumentException("Ohko must be bool, 'always', or 'ifHit'");
                }
            }
        }
        public bool? BreaksProtect { get; init; }
        public (int, int)? Recoil { get; init; }
        public (int, int)? Drain { get; init; }
        public bool? MindBlownRecoil { get; init; }
        public bool? StealsBoosts { get; init; }
        public bool? StruggleRecoil { get; init; }
        public SecondaryEffect? Secondary { get; init; }
        public SecondaryEffect[]? Secondaries { get; init; }
        public SecondaryEffect? Self { get; init; }
        public bool? HasSheerForce { get; init; }
        public bool? AlwaysHit { get; init; }
        public PokemonType? BaseMoveType { get; init; }
        public int? BasePowerModifier { get; init; }
        public int? CritModifier { get; init; }
        public int? CritRatio { get; init; }
        public PokemonOrigin? OverrideOffensivePokemon { get; init; }
        public StatIdExceptHp? OverrideOffensiveStat { get; init; }
        public PokemonOrigin? OverrideDefensivePokemon { get; init; }
        public StatIdExceptHp? OverrideDefensiveStat { get; init; }
        public bool? ForceStab { get; init; }
        public bool? IgnoreAbility { get; init; }
        public bool? IgnoreAccuracy { get; init; }
        public bool? IgnoreDefensive { get; init; }
        public bool? IgnoreEvasion { get; init; }
        public object? IgnoreImmunity // boolean | { [typeName: string]: boolean };
        {
            get;
            init
            {
                if (value is bool or (PokemonType, bool) or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("IgnoreImmunity must be bool or (PokemonType, bool) tuple");
                }
            }
        } 
        public bool? IgnoreNegativeOffensive { get; init; }
        public bool? IgnoreOffensive { get; init; }
        public bool? IgnorePositiveDefensive { get; init; }
        public bool? IgnorePositiveEvasion { get; init; }
        public bool? MultiAccuracy { get; init; }
        public int[]? MultiHit { get; init; } // number | number[];
        public string? MultiHitType { get; init; } // "parentalbond"
        public bool? NoDamageVariance { get; init; }
        public MoveTarget? NonGhostTarget { get; init; }
        public double? SpreadModifier { get; init; }
        public bool? SleepUsable { get; init; }
        public bool? SmartTarget { get; init; }
        public bool? TracksTarget { get; init; }
        public bool? WillCrit { get; init; }
        public bool? CallsMove { get; init; }
        public bool? HasCrashDamage { get; init; }
        public bool? IsConfusionSelfHit { get; init; }
        public bool? StallingMove { get; init; }
        public Id? BaseMove { get; init; }
        public required string Name { get; init; }
        public string? Desc { get; init; }
        public int? Duration { get; init; }
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; init; }
        public required EffectType EffectType { get; init; }
        public bool? Infiltrates { get; init; }
        public Nonstandard? IsNonstandard { get; init; }
        public string? ShortDesc { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? BasePowerCallback { get; init; }
        public Func<Battle, Pokemon, Pokemon?, ActiveMove, bool?>? BeforeMoveCallback { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? BeforeTurnCallback { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? DamageCallback { get; init; }
        public Action<Battle, Pokemon>? PriorityChargeCallback { get; init; }
        public Action<Battle, Pokemon>? OnDisableMove { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; init; }
        public int? OnDamagePriority { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; init; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; init; }
        public SparseBoostsTable? Boosts { get; init; }
        public Id? Status { get; init; }
        public Id? VolatileStatus { get; init; }
        public string? SideCondition { get; init; }
        public string? SlotCondition { get; init; }
        public string? PseudoWeather { get; init; }
        public string? Terrain { get; init; }
        public Id? Weather { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHitField { get; init; }
        public Func<Battle, Side, Pokemon, ActiveMove, object?>? OnHitSide { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveFail { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; init; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTry { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; init; }
        public Func<Battle, Side, Pokemon, ActiveMove, object?>? OnTryHitSide { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryImmunity { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnUseMoveMessage { get; init; }
    }

    // helper class for MoveData
    public class ZMoveData
    {
        public int? BasePower { get; init; }
        public string? Effect { get; init; }
        public SparseBoostsTable? Boost { get; init; }
    }

    // Helper class for MoveData
    public class MaxMoveData
    {
        public required int BasePower { get; init; }
    }

    // helper class for MoveData
    public class SelfBoostData
    {
        public SparseBoostsTable? Boosts { get; init; }
    }

    public class ModdedMoveData : MoveData
    {
        public static bool Inherit => true;
        public bool? IgniteBoosted { get; init; }
        public bool? SettleBoosted { get; init; }
        public bool? BodyOfWaterBoosted { get; init; }
        public bool? LongWhipBoost { get; init; }
        public int? Gen { get; init; }
    }

    // MoveDataTable and ModdedMoveDataTable
    public class MoveDataTable : Dictionary<IdEntry, MoveData>;

    public class ModdedMoveDataTable : Dictionary<IdEntry, ModdedMoveData>;

    public class Move : MutableMove
    {
        public override required EffectType EffectType
        {
            get => EffectType.Move;
            init { }
        }
    }


    // Element of MoveHitData
    public class MoveHitResult
    {
        public required bool Crit { get; init; }
        public required int TypeMod { get; init; }
        public required bool ZBrokeProtect { get; init; }
    }

    public class MoveHitData : Dictionary<string, MoveHitResult>;

    public class MutableMove : BasicEffect, IMoveData
    {
        // public int? Num { get; init; }
        public bool? SpreadHit { get; init; }
        public IConditionData? Condition { get; init; }
        public int BasePower { get; init; }
        public required object Accuracy
        {
            get;
            init
            {
                if (value is bool or double or int)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Accuracy must be a boolean, integer or double.");
                }
            }
        }
        public required int Pp { get; init; }
        public required MoveCategory Category { get; init; }
        public required PokemonType Type { get; init; }
        public required int Priority { get; init; }
        public required MoveTarget Target { get; init; }
        public required MoveFlags Flags { get; init; }
        //public string? RealMove { get; init; } // For Hidden Power
        public object? Damage
        {
            get;
            init
            {
                if (field is int or double or string or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Damage must be a number, string, or an array of numbers.");
                }
            }
        }
        public ContestType? ContestType { get; init; }
        public bool? NoPpBoosts { get; init; }
        public object? IsZ
        {
            get;
            init
            {
                if (value is bool or IdEntry or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("IsZ must be bool or IdEntry");
                }
            }
        }
        public ZMoveData? ZMove { get; init; }
        public object? IsMax
        {
            get;
            init
            {
                if (value is bool or string or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("IsMaz must be bool, string or null");
                }
            }
        }
        public MaxMoveData? MaxMove { get; init; }
        public object? Ohko // bool or "Ice"
        {
            get;
            init
            {
                switch (value)
                {
                    case bool or null:
                        field = value;
                        break;
                    case string s:
                        {
                            if (s == "Ice")
                            {
                                field = s;
                            }

                            break;
                        }
                    default:
                        throw new ArgumentException("Ohko must be bool or 'Ice'");
                }
            }
        }
        public bool? ThawsTarget { get; init; }
        public int[]? Heal { get; init; }
        public bool? ForceSwitch { get; init; }
        public object? SelfSwitch // "copyvolatile", "shedtail", or bool
        {
            get;
            init
            {
                switch (value)
                {
                    case bool or null:
                        field = value;
                        break;
                    case string s:
                        {
                            if (s is "copyvolatile" or "shedtail")
                            {
                                field = s;
                            }
                            break;
                        }
                    default:
                        throw new ArgumentException("Ohko must be bool, 'copyvolatile', or 'shedtail'");
                }
            }
        }
        public SelfBoostData? SelfBoost { get; init; }
        public object? SelfDestruct // "always", "ifHit", or bool
        {
            get;
            init
            {
                switch (value)
                {
                    case bool or null:
                        field = value;
                        break;
                    case string s:
                        {
                            if (s is "always" or "ifHit")
                            {
                                field = s;
                            }
                            break;
                        }
                    default:
                        throw new ArgumentException("Ohko must be bool, 'always', or 'ifHit'");
                }
            }
        }
        public bool? BreaksProtect { get; init; }
        public (int, int)? Recoil { get; init; }
        public (int, int)? Drain { get; init; }
        public bool? MindBlownRecoil { get; init; }
        public bool? StealsBoosts { get; init; }
        public bool? StruggleRecoil { get; init; }
        public SecondaryEffect? Secondary { get; init; }
        public SecondaryEffect[]? Secondaries { get; init; }
        public SecondaryEffect? Self { get; init; }
        public bool? HasSheerForce { get; init; }
        public bool? AlwaysHit { get; init; }
        public PokemonType? BaseMoveType { get; init; }
        public int? BasePowerModifier { get; init; }
        public int? CritModifier { get; init; }
        public int? CritRatio { get; init; }
        public PokemonOrigin? OverrideOffensivePokemon { get; init; }
        public StatIdExceptHp? OverrideOffensiveStat { get; init; }
        public PokemonOrigin? OverrideDefensivePokemon { get; init; }
        public StatIdExceptHp? OverrideDefensiveStat { get; init; }
        public bool? ForceStab { get; init; }
        public bool? IgnoreAbility { get; init; }
        public bool? IgnoreAccuracy { get; init; }
        public bool? IgnoreDefensive { get; init; }
        public bool? IgnoreEvasion { get; init; }
        public object? IgnoreImmunity // boolean | { [typeName: string]: boolean };
        {
            get;
            init
            {
                if (value is bool or (PokemonType, bool) or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("IgnoreImmunity must be bool or (PokemonType, bool) tuple");
                }
            }
        }
        public bool? IgnoreNegativeOffensive { get; init; }
        public bool? IgnoreOffensive { get; init; }
        public bool? IgnorePositiveDefensive { get; init; }
        public bool? IgnorePositiveEvasion { get; init; }
        public bool? MultiAccuracy { get; init; }
        public int[]? MultiHit { get; init; } // number | number[];
        public string? MultiHitType { get; init; } // "parentalbond"
        public bool? NoDamageVariance { get; init; }
        public MoveTarget? NonGhostTarget { get; init; }
        public double? SpreadModifier { get; init; }
        public bool? SleepUsable { get; init; }
        public bool? SmartTarget { get; init; }
        public bool? TracksTarget { get; init; }
        public bool? WillCrit { get; init; }
        public bool? CallsMove { get; init; }
        public bool? HasCrashDamage { get; init; }
        public bool? IsConfusionSelfHit { get; init; }
        public bool? StallingMove { get; init; }
        public Id? BaseMove { get; init; }
        // public required string Name { get; init; }
        // public string? Desc { get; init; }
        // public int? Duration { get; init; }
        // public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; init; }
        // public required EffectType EffectType { get; init; }
        // public bool? Infiltrates { get; init; }
        // public Nonstandard? IsNonstandard { get; init; }
        // public string? ShortDesc { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? BasePowerCallback { get; init; }
        public Func<Battle, Pokemon, Pokemon?, ActiveMove, bool?>? BeforeMoveCallback { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? BeforeTurnCallback { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? DamageCallback { get; init; }
        public Action<Battle, Pokemon>? PriorityChargeCallback { get; init; }
        public Action<Battle, Pokemon>? OnDisableMove { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; init; }
        public int? OnDamagePriority { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; init; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; init; }
        public SparseBoostsTable? Boosts { get; init; }
        //public string? Status { get; init; }
        public Id? VolatileStatus { get; init; }
        public string? SideCondition { get; init; }
        public string? SlotCondition { get; init; }
        public string? PseudoWeather { get; init; }
        public string? Terrain { get; init; }
        //public string? Weather { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHitField { get; init; }
        public Func<Battle, Side, Pokemon, ActiveMove, object?>? OnHitSide { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveFail { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; init; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTry { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; init; }
        public Func<Battle, Side, Pokemon, ActiveMove, object?>? OnTryHitSide { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryImmunity { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnUseMoveMessage { get; init; }
    }

    public class ActiveMove : MutableMove, IEffect
    {
        public int Hit { get; init; }
        public MoveHitData? MoveHitData { get; init; }
        public List<Pokemon>? HitTargets { get; init; }
        public Ability? Ability { get; init; }
        public List<Pokemon>? Allies { get; init; }
        public Pokemon? AuraBooster { get; init; }
        public bool? CausedCrashDamage { get; init; }
        public string? ForceStatus { get; init; }
        public bool? HasAuraBreak { get; init; }
        public bool? HasBounced { get; init; }
        //public bool? HasSheerForce { get; init; }
        public bool? IsExternal { get; init; }
        public bool? LastHit { get; init; }
        public int? Magnitude { get; init; }
        public bool? PranksterBoosted { get; init; }
        public bool? SelfDropped { get; init; }
        //public object? SelfSwitch { get; init; } // "copyvolatile", "shedtail", or bool

        public string? StatusRoll { get; init; }
        public bool? StellarBoosted { get; init; }
        public object? TotalDamage // int or false
        {
            get;
            init
            {
                if (value is int or double or string or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("TotalDamage must be a number or null.");
                }
            }
        } 
        public IEffect? TypeChangerBoosted { get; init; }
        public bool? WillChangeForme { get; init; }
        public Pokemon? RuinedAtk { get; init; }
        public Pokemon? RuinedDef { get; init; }
        public Pokemon? RuinedSpA { get; init; }
        public Pokemon? RuinedSpD { get; init; }
        public bool? IsZOrMaxPowered { get; init; }
    }

    public enum MoveCategory
    {
        Physical,
        Special,
        Status
    }

    public class DataMove : BasicEffect, IBasicEffect, IMoveData
    {
        // IMoveData properties
        public bool? SpreadHit
        {
            get => field ?? false;
            init;
        }
        public IConditionData? Condition { get; init; }
        public int BasePower { get; init; }
        public required object Accuracy
        {
            get;
            init
            {
                if (value is bool or double or int)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Accuracy must be a boolean, integer or double.");
                }
            }
        }
        public required int Pp { get; init; }
        public required MoveCategory Category { get; init; }
        public required PokemonType Type { get; init; }
        public required int Priority { get; init; }
        public required MoveTarget Target { get; init; }
        public required MoveFlags Flags { get; init; }
        public object? Damage
        {
            get;
            init
            {
                if (field is int or double or string or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Damage must be a number, string, or an array of numbers.");
                }
            }
        }
        public ContestType? ContestType { get; init; }

        public bool? NoPpBoosts
        {
            get => (field ?? IsZ) == (object)true;
            init;
        }
        public object? IsZ
        {
            get => field ?? false;
            init
            {
                if (value is bool or IdEntry or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("IsZ must be bool or IdEntry");
                }
            }
        }

        public ZMoveData? ZMove
        {
            get
            {
                // Only initialize if not Status, not already a Z/Max move, and not "struggle"
                if (Category == MoveCategory.Status || field != null || IsZ is true || IsMax is true ||
                    Id.Value == "struggle") return field;

                int basePower = BasePower;

                // If multi-hit, triple the base power
                if (MultiHit is { Length: > 1 })
                    basePower *= 3;

                int zBasePower = basePower switch
                {
                    0 => 100,
                    >= 140 => 200,
                    >= 130 => 195,
                    >= 120 => 190,
                    >= 110 => 185,
                    >= 100 => 180,
                    >= 90 => 175,
                    >= 80 => 160,
                    >= 70 => 140,
                    >= 60 => 120,
                    _ => 100
                };

                return new ZMoveData { BasePower = zBasePower };
            }
            init;
        }
        public object? IsMax
        {
            get => field ?? false;
            init
            {
                if (value is bool or string or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("IsMaz must be bool, string or null");
                }
            }
        }

        public MaxMoveData? MaxMove
        {
            get
            {
                if (Category == MoveCategory.Status || field is not null || Id == new Id("struggle"))
                    return field;

                field = new MaxMoveData
                {
                    BasePower = 1, // Default value, will be adjusted later
                };

                if ((IsMax is true) || (IsZ is true))
                {
                    return field;
                }
                if (BasePower == 0)
                {
                    return new MaxMoveData { BasePower = 100 };
                }
                if (Type is PokemonType.Fighting or PokemonType.Poison)
                {
                    return BasePower switch
                    {
                        >= 150 => new MaxMoveData { BasePower = 100 },
                        >= 110 => new MaxMoveData { BasePower = 95 },
                        >= 75 => new MaxMoveData { BasePower = 90 },
                        >= 65 => new MaxMoveData { BasePower = 85 },
                        >= 55 => new MaxMoveData { BasePower = 80 },
                        >= 45 => new MaxMoveData { BasePower = 75 },
                        _ => new MaxMoveData { BasePower = 70 }
                    };
                }

                return BasePower switch
                {
                    >= 150 => new MaxMoveData { BasePower = 150 },
                    >= 110 => new MaxMoveData { BasePower = 140 },
                    >= 75 => new MaxMoveData { BasePower = 130 },
                    >= 65 => new MaxMoveData { BasePower = 120 },
                    >= 55 => new MaxMoveData { BasePower = 110 },
                    >= 45 => new MaxMoveData { BasePower = 100 },
                    _ => new MaxMoveData { BasePower = 90 }
                };
            }
            init;
        }

        public object? Ohko // bool or "Ice"
        {
            get;
            init
            {
                switch (value)
                {
                    case bool or null:
                        field = value;
                        break;
                    case string s:
                        {
                            if (s == "Ice")
                            {
                                field = s;
                            }

                            break;
                        }
                    default:
                        throw new ArgumentException("Ohko must be bool or 'Ice'");
                }
            }
        }
        public bool? ThawsTarget { get; init; }
        public int[]? Heal { get; init; }
        public bool? ForceSwitch { get; init; }
        public object? SelfSwitch // "copyvolatile", "shedtail", or bool
        {
            get => field as string ?? field;
            init
            {
                switch (value)
                {
                    case bool or null:
                        field = value;
                        break;
                    case string s:
                        {
                            if (s is "copyvolatile" or "shedtail")
                            {
                                field = s;
                            }
                            break;
                        }
                    default:
                        throw new ArgumentException("Ohko must be bool, 'copyvolatile', or 'shedtail'");
                }
            }
        }
        public SelfBoostData? SelfBoost { get; init; }
        public object? SelfDestruct // "always", "ifHit", or bool
        {
            get;
            init
            {
                switch (value)
                {
                    case bool or null:
                        field = value;
                        break;
                    case string s:
                        {
                            if (s is "always" or "ifHit")
                            {
                                field = s;
                            }
                            break;
                        }
                    default:
                        throw new ArgumentException("Ohko must be bool, 'always', or 'ifHit'");
                }
            }
        }
        public bool? BreaksProtect { get; init; }
        public (int, int)? Recoil { get; init; }
        public (int, int)? Drain { get; init; }
        public bool? MindBlownRecoil { get; init; }
        public bool? StealsBoosts { get; init; }
        public bool? StruggleRecoil { get; init; }
        public SecondaryEffect? Secondary { get; init; }
        public SecondaryEffect[]? Secondaries 
        {
            get
            {
                if (field is not null)
                {
                    return field;
                }
                if (field == null && Secondary != null)
                {
                    return [Secondary];
                }
                return field;
            }
            init;
        }
        public SecondaryEffect? Self { get; init; }
        public bool? HasSheerForce
        {
            // this.hasSheerForce = !!(data.hasSheerForce && !this.secondaries);
            get
            {
                if (field is { } b)
                {
                    return b && Secondaries is null;
                }
                return field is null ? false : throw new ArgumentException("HasSheerForce must be a boolean.");
            }
            init;
        }
        public bool? AlwaysHit { get; init; }
        public PokemonType? BaseMoveType
        { 
            get => field ?? Type;
            init;

        }
        public int? BasePowerModifier { get; init; }
        public int? CritModifier { get; init; }
        public int? CritRatio
        {
            get => field ?? 1;
            init;
        }
        public PokemonOrigin? OverrideOffensivePokemon { get; init; }
        public StatIdExceptHp? OverrideOffensiveStat { get; init; }
        public PokemonOrigin? OverrideDefensivePokemon { get; init; }
        public StatIdExceptHp? OverrideDefensiveStat { get; init; }

        public bool? ForceStab
        {
            get => field ?? false;
            init;
        }

        public bool? IgnoreAbility
        {
            get => field ?? false;
            init;
        }
        public bool? IgnoreAccuracy { get; init; }

        public bool? IgnoreDefensive
        {
            get => field ?? false;
            init;
        }
        public bool? IgnoreEvasion { get; init; }
        public object? IgnoreImmunity // boolean | { [typeName: string]: boolean };
        {
            get => field ?? (Category == MoveCategory.Status);
            init
            {
                if (value is bool or (PokemonType, bool) or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("IgnoreImmunity must be bool or (PokemonType, bool) tuple");
                }
            }
        }

        public bool? IgnoreNegativeOffensive
        {
            get => field ?? false;
            init;
        }

        public bool? IgnoreOffensive
        {
            get => field ?? false;
            init;
        }

        public bool? IgnorePositiveDefensive
        {
            get => field ?? false;
            init;
        }
        public bool? IgnorePositiveEvasion { get; init; }
        public bool? MultiAccuracy { get; init; }
        public int[]? MultiHit { get; init; } // number | number[];
        public string? MultiHitType { get; init; } // "parentalbond"
        public bool? NoDamageVariance { get; init; }

        public MoveTarget? NonGhostTarget
        {
            get => field ?? MoveTarget.None;
            init;
        }
        public double? SpreadModifier { get; init; }
        public bool? SleepUsable { get; init; }
        public bool? SmartTarget { get; init; }
        public bool? TracksTarget { get; init; }
        public bool? WillCrit { get; init; }
        public bool? CallsMove { get; init; }
        public bool? HasCrashDamage { get; init; }
        public bool? IsConfusionSelfHit { get; init; }
        public bool? StallingMove { get; init; }
        public Id? BaseMove { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? BasePowerCallback { get; init; }
        public Func<Battle, Pokemon, Pokemon?, ActiveMove, bool?>? BeforeMoveCallback { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? BeforeTurnCallback { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? DamageCallback { get; init; }
        public Action<Battle, Pokemon>? PriorityChargeCallback { get; init; }
        public Action<Battle, Pokemon>? OnDisableMove { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; init; }
        public int? OnDamagePriority { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; init; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; init; }
        public SparseBoostsTable? Boosts { get; init; }

        public Id? VolatileStatus { get; init; }
        public string? SideCondition { get; init; }
        public string? SlotCondition { get; init; }
        public string? PseudoWeather { get; init; }
        public string? Terrain { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHitField { get; init; }
        public Func<Battle, Side, Pokemon, ActiveMove, object?>? OnHitSide { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveFail { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; init; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTry { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; init; }
        public Func<Battle, Side, Pokemon, ActiveMove, object?>? OnTryHitSide { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryImmunity { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnUseMoveMessage { get; init; }

        public override required string Fullname
        {
            get => $"move: {Name}";
            init { }
        }

        public override required EffectType EffectType
        {
            get => EffectType.Move;
            init { }
        }

        public override required int Gen
        {
            get
            {
                if (field != 0) return field;
                return field switch
                {
                    // Special handling for Gen 8 G-Max moves (num 1000 but part of Gen 8)
                    >= 827 when (bool?)IsMax != true => 9,
                    >= 743 => 8,
                    >= 622 => 7,
                    >= 560 => 6,
                    >= 468 => 5,
                    >= 355 => 4,
                    >= 252 => 3,
                    >= 166 => 2,
                    >= 1 => 1,
                    _ => field
                };
            }
            init;
        }
    }

    public class DexMoves(ModdedDex dex)
    {
        private ModdedDex Dex { get; } = dex;
        private Dictionary<string, Move> MoveCache { get; init; } = new();
        private IReadOnlyList<Move>? AllCache { get; init; } = null;

        public Move Get(string? name)
        {
            throw new NotImplementedException();
        }

        public Move Get(Move? move)
        {
            throw new NotImplementedException();
        }

        public Move GetById(Id id)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<Move> All()
        {
            throw new NotImplementedException();
        }
    }
}