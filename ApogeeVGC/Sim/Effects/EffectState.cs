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
    public PokemonType? TypeWas { get; set; }
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
    public PokemonSlot? TargetSlot { get; set; } // For Future Sight target tracking
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
    public int? TotalDamage { get; set; } // For Counter/Mirror Coat damage tracking
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
}