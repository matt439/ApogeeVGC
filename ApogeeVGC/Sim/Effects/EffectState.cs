using ApogeeVGC.Sim.Conditions;
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