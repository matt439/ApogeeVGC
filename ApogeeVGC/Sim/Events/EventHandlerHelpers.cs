using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Helper methods for creating and working with context-based event handlers.
/// </summary>
public static class EventHandlerHelpers
{
    // === Handler Creation Helpers ===
    
    /// <summary>
    /// Creates a simple event handler that doesn't return a value.
    /// </summary>
    public static EventHandlerDelegate CreateVoidHandler(Action<EventContext> action)
    {
        return context =>
        {
  action(context);
          return null;
        };
    }
    
    /// <summary>
    /// Creates an event handler that returns a boolean.
    /// </summary>
    public static EventHandlerDelegate CreateBoolHandler(Func<EventContext, bool> func)
    {
        return context => new BoolRelayVar(func(context));
 }
    
    /// <summary>
    /// Creates an event handler that returns an integer.
    /// </summary>
    public static EventHandlerDelegate CreateIntHandler(Func<EventContext, int> func)
    {
        return context => new IntRelayVar(func(context));
 }
    
    /// <summary>
    /// Creates an event handler that modifies and returns a relay variable.
    /// </summary>
    public static EventHandlerDelegate CreateRelayVarHandler(Func<EventContext, RelayVar?> func)
    {
        return func.Invoke;
    }
    
    /// <summary>
    /// Creates an event handler that returns a boolean or void.
    /// </summary>
    public static EventHandlerDelegate CreateBoolOrVoidHandler(Func<EventContext, bool?> func)
    {
    return context =>
        {
            bool? result = func(context);
            return result.HasValue ? new BoolRelayVar(result.Value) : null;
        };
    }
    
    // === Chaining Helpers ===
    
    /// <summary>
    /// Chains multiple handlers together, executing them in sequence.
    /// The relay variable is passed through the chain.
    /// </summary>
    public static EventHandlerDelegate ChainHandlers(params EventHandlerDelegate[] handlers)
    {
   return context =>
  {
    RelayVar? result = context.RelayVar;
   
         foreach (var handler in handlers)
            {
      // Update context with new relay var
       var newContext = new EventContext
        {
 Battle = context.Battle,
     EventId = context.EventId,
    TargetPokemon = context.TargetPokemon,
        TargetSide = context.TargetSide,
              TargetField = context.TargetField,
     SourcePokemon = context.SourcePokemon,
          SourceEffect = context.SourceEffect,
      Move = context.Move,
  RelayVar = result
            };
           
          result = handler(newContext);
    
       // Stop if result becomes falsy
   if (result is BoolRelayVar { Value: false })
    {
           break;
}
            }
  
            return result;
        };
  }
    
    /// <summary>
    /// Creates a conditional handler that only executes if a condition is met.
    /// </summary>
    public static EventHandlerDelegate CreateConditionalHandler(
   Func<EventContext, bool> condition,
  EventHandlerDelegate handler)
    {
        return context =>
        {
            if (condition(context))
            {
      return handler(context);
            }
      return context.RelayVar;
     };
    }
    
 // === Type-Safe Accessors ===
    
    /// <summary>
    /// Creates a handler that requires a target Pokemon.
    /// Throws if no target Pokemon is available.
    /// </summary>
    public static EventHandlerDelegate RequireTargetPokemon(
        Func<EventContext, RelayVar?> handler)
    {
        return context =>
     {
  if (!context.HasTargetPokemon)
            {
       throw new InvalidOperationException(
             $"Event {context.EventId} requires a target Pokemon");
    }
      return handler(context);
    };
    }
    
    /// <summary>
    /// Creates a handler that requires a source Pokemon.
    /// Throws if no source Pokemon is available.
    /// </summary>
    public static EventHandlerDelegate RequireSourcePokemon(
        Func<EventContext, RelayVar?> handler)
  {
        return context =>
        {
            if (!context.HasSourcePokemon)
    {
             throw new InvalidOperationException(
         $"Event {context.EventId} requires a source Pokemon");
    }
   return handler(context);
  };
    }
    
    /// <summary>
    /// Creates a handler that requires a move.
    /// Throws if no move is available.
    /// </summary>
    public static EventHandlerDelegate RequireMove(
        Func<EventContext, RelayVar?> handler)
    {
        return context =>
        {
 if (!context.HasMove)
            {
  throw new InvalidOperationException(
            $"Event {context.EventId} requires a move");
            }
            return handler(context);
        };
    }
}
