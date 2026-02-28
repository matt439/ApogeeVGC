using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Effects;

public enum EffectStateKey
{
    Duration,
}


public class EffectState
{
    public required EffectStateId Id { get; set; }
    public int EffectOrder { get; set; }
    public int? Duration { get; set; }

    // other properties that might be relevant to effect state

    public bool? FromBooster { get; set; }
    public StatIdExceptHp? BestStat { get; set; }
    public EffectStateTarget? Target { get; set; }
    public bool? Unnerved { get; set; }
    public int? StartTime { get; set; }
    public int? Time { get; set; }
    public int? Stage { get; set; }
    public MoveId? Move { get; set; }
    public IEffect? SourceEffect { get; set; }
    public PokemonSlot? SourceSlot { get; set; }
    public int? Counter { get; set; }
    public Condition? LinkedStatus { get; set; }
    public List<Pokemon>? LinkedPokemon { get; set; }
    public bool? Started { get; set; }
    public Pokemon? Source { get; set; }
    public bool? KnockedOff { get; set; }
    public bool? Ending { get; set; }
    public bool? Resisted { get; set; }
    public bool? IsSlotCondition { get; set; }
    public PokemonType[]? TypeWas { get; set; }
    public bool? HasDragonType { get; set; }
    public int? ContactHitCount { get; set; }
    public bool? CheckedAngerShell { get; set; }
    public bool? CheckedBerserk { get; set; }
    public Item? Berry { get; set; }
    public bool? Embodied { get; set; }
    public bool? Gluttony { get; set; }
    public MoveId? ChoiceLock { get; set; }
    public bool? Busted { get; set; } // For Ice Face / Disguise
    public bool? Libero { get; set; } // For Libero ability
    public bool? Protean { get; set; } // For Protean ability
    public SparseBoostsTable? Boosts { get; set; } // For Opportunist ability
    public bool? BerryWeaken { get; set; } // For Ripen ability
    public bool? Seek { get; set; } // For Trace ability
    public bool? Ready { get; set; } // For Mirror Herb and other trigger items
    public string? LastMove { get; set; } // For Metronome item
    public int? NumConsecutive { get; set; } // For Metronome item
    public bool? Eject { get; set; } // For Eject Pack tracking
    public bool? Inactive { get; set; } // For Utility Umbrella tracking

    // Additional properties for condition states
    public bool? LostFocus { get; set; } // For Focus Punch
    public int? HitCount { get; set; } // For Fury Cutter
    public int? Multiplier { get; set; } // For Echoed Voice
    public double? DoubleMultiplier { get; set; } // For Helping Hand
    public PokemonSlot? TargetSlot { get; set; } // For Future Sight target tracking
    public int? TargetLoc { get; set; } // For two-turn move target tracking (relative location)
    public int? EndingTurn { get; set; } // For Future Sight timing
    public int? TrueDuration { get; set; } // For LockedMove (Outrage, etc.)
    public int? Layers { get; set; } // For entry hazards (Spikes, Toxic Spikes)
    public int? Def { get; set; } // For Stockpile stat tracking
    public int? Spd { get; set; } // For Stockpile stat tracking
    public int? BoundDivisor { get; set; } // For PartiallyTrapped damage calculation
    public int? Hp { get; set; } // For Wish and similar healing effects
    public int? StartingTurn { get; set; } // For Wish timing
    public Move? MoveData { get; set; } // For Future Move attack data storage

    // Counter and Mirror Coat tracking
    public PokemonSlot? Slot { get; set; } // For Counter/Mirror Coat damage source slot

    //public int? TotalDamage { get; set; } // For Counter/Mirror Coat damage tracking
    public int? Damage { get; set; }
    public Pokemon? LastDamageSource { get; set; } // For Counter/Mirror Coat damage source

    public int? GetProperty(EffectStateKey? key)
    {
        return key switch
        {
            EffectStateKey.Duration => Duration,
            null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, null),
        };
    }

    public EffectState ShallowClone()
    {
        return new EffectState
        {
            Id = Id,
            Target = Target,
            Source = Source,
            SourceSlot = SourceSlot,
            SourceEffect = SourceEffect,
            Duration = Duration,
            Started = Started,
            Ending = Ending,
            LinkedPokemon = LinkedPokemon?.ToList(), // Create new list reference
            LinkedStatus = LinkedStatus,
            // TODO: Copy any other properties
        };
    }

    /// <summary>
    /// Creates a complete deep clone of this EffectState.
    /// Pokemon references (Source, LinkedPokemon, LastDamageSource) are set to null
    /// and must be remapped via <see cref="RemapPokemonReferences"/> after all Pokemon are copied.
    /// </summary>
    public EffectState DeepClone()
    {
        return new EffectState
        {
            // Core
            Id = Id,
            EffectOrder = EffectOrder,
            Duration = Duration,

            // Shared references (library objects / structs)
            Target = Target,
            SourceEffect = SourceEffect,
            SourceSlot = SourceSlot,
            LinkedStatus = LinkedStatus,
            Berry = Berry,
            MoveData = MoveData,

            // Pokemon references → null (remapped in Pass 2)
            Source = null,
            LinkedPokemon = null,
            LastDamageSource = null,

            // Value-type fields
            Started = Started,
            Ending = Ending,
            FromBooster = FromBooster,
            BestStat = BestStat,
            Unnerved = Unnerved,
            StartTime = StartTime,
            Time = Time,
            Stage = Stage,
            Move = Move,
            Counter = Counter,
            KnockedOff = KnockedOff,
            Resisted = Resisted,
            IsSlotCondition = IsSlotCondition,
            HasDragonType = HasDragonType,
            ContactHitCount = ContactHitCount,
            CheckedAngerShell = CheckedAngerShell,
            CheckedBerserk = CheckedBerserk,
            Embodied = Embodied,
            Gluttony = Gluttony,
            ChoiceLock = ChoiceLock,
            Busted = Busted,
            Libero = Libero,
            Protean = Protean,
            BerryWeaken = BerryWeaken,
            Seek = Seek,
            Ready = Ready,
            LastMove = LastMove,
            NumConsecutive = NumConsecutive,
            Eject = Eject,
            Inactive = Inactive,
            LostFocus = LostFocus,
            HitCount = HitCount,
            Multiplier = Multiplier,
            DoubleMultiplier = DoubleMultiplier,
            TargetSlot = TargetSlot,
            TargetLoc = TargetLoc,
            EndingTurn = EndingTurn,
            TrueDuration = TrueDuration,
            Layers = Layers,
            Def = Def,
            Spd = Spd,
            BoundDivisor = BoundDivisor,
            Hp = Hp,
            StartingTurn = StartingTurn,
            Slot = Slot,
            Damage = Damage,

            // Deep copy reference types
            TypeWas = TypeWas?.ToArray(),
            Boosts = Boosts?.Copy(),
        };
    }

    /// <summary>
    /// Remaps Pokemon references in this EffectState using the provided mapping.
    /// Called during Pass 2 of deep copy after all Pokemon have been copied.
    /// </summary>
    internal void RemapPokemonReferences(Dictionary<Pokemon, Pokemon> pokemonMap)
    {
        Source = RemapPokemon(Source, pokemonMap);
        LastDamageSource = RemapPokemon(LastDamageSource, pokemonMap);
        if (LinkedPokemon is not null)
        {
            for (int i = 0; i < LinkedPokemon.Count; i++)
            {
                var remapped = RemapPokemon(LinkedPokemon[i], pokemonMap);
                if (remapped is not null)
                    LinkedPokemon[i] = remapped;
            }
        }
    }

    internal static Pokemon? RemapPokemon(Pokemon? pokemon, Dictionary<Pokemon, Pokemon> map)
        => pokemon is not null && map.TryGetValue(pokemon, out var copy) ? copy : null;
}