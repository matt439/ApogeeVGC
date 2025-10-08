using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Effects;

public class EffectState
{
    public required EffectStateId Id { get; init; }
    public int EffectOrder { get; init; }
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
    public PokemonSlotId? SourceSlot { get; set; }
    public int? Counter { get; set; }
    public Condition? LinkedStatus { get; set; }
    public List<Pokemon>? LinkedPokemon { get; set; }
    public bool? Started { get; set; }
    public Pokemon? Source { get; set; }
    public bool? KnockedOff { get; set; }
    public bool? Ending { get; set; }
}