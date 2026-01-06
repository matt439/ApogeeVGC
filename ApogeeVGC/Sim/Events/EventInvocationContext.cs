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
            TargetPokemon = Target switch
          {
     PokemonSingleEventTarget p => p.Pokemon,
                _ => null
          },
   TargetSide = Target switch
 {
     SideSingleEventTarget s => s.Side,
              PokemonSingleEventTarget p => p.Pokemon.Side,
     _ => null
      },
     TargetField = Target switch
  {
      FieldSingleEventTarget f => f.Field,
          _ => null
   },
     SourcePokemon = Source switch
       {
           PokemonSingleEventSource p => p.Pokemon,
       _ => null
      },
  SourceEffect = SourceEffect,
    Move = SourceEffect as Moves.ActiveMove,
 RelayVar = RelayVar
 };
    }
}
