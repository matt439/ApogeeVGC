using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Internal context used during event invocation.
/// Contains raw event data before conversion to EventContext.
/// </summary>
internal sealed class EventInvocationContext
{
    public required Battle Battle { get; init; }
    public required EventId EventId { get; init; }
    public IEffect? Effect { get; init; }
    public SingleEventTarget? Target { get; init; }
    public SingleEventSource? Source { get; init; }
    public IEffect? SourceEffect { get; init; }
    public RelayVar? RelayVar { get; init; }
    public bool HasRelayVar { get; init; }
    
    /// <summary>
    /// Converts this internal context to a public EventContext.
    /// </summary>
    public EventContext ToEventContext()
    {
        return new EventContext
        {
         Battle = Battle,
       EventId = EventId,
            Effect = Effect,
            TargetPokemon = Target is { Kind: SingleEventTargetKind.Pokemon } tp ? tp.Pokemon : null,
   TargetSide = Target switch
 {
      { Kind: SingleEventTargetKind.Side } s => s.Side,
               { Kind: SingleEventTargetKind.Pokemon } p => p.Pokemon.Side,
      _ => null
       },
     TargetField = Target is { Kind: SingleEventTargetKind.Field } tf ? tf.Field : null,
     SourcePokemon = Source switch
       {
           PokemonSingleEventSource p => p.Pokemon,
       _ => null
      },
  SourceType = Source switch
      {
          PokemonTypeSingleEventSource t => t.Type,
          _ => null
      },
  SourceEffect = SourceEffect,
    Move = SourceEffect as Moves.ActiveMove,
 RelayVar = RelayVar
 };
    }
}
