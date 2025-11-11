using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Type-safe builder for creating EventHandlerInfo instances.
/// Provides overloaded methods for common event signatures to ensure
/// consistent metadata and type validation across all effect types.
/// </summary>
public static class EventHandlerInfoBuilder
{
    // ========== Action<Battle, Pokemon> ==========
    
    public static EventHandlerInfo? Create(
        EventId id,
   Action<Battle, Pokemon>? handler,
 int? priority = null,
int? order = null,
        int? subOrder = null,
        bool usesSpeed = false,
  bool usesEffectOrder = false)
    {
 if (handler == null) return null;
        
   return new EventHandlerInfo
        {
     Id = id,
      Handler = handler,
       Priority = priority,
    Order = order,
      SubOrder = subOrder,
    UsesSpeed = usesSpeed,
            UsesEffectOrder = usesEffectOrder,
        ExpectedParameterTypes = new[] { typeof(Battle), typeof(Pokemon) },
            ExpectedReturnType = typeof(void),
        };
    }

    // ========== Action<Battle, int, Pokemon, Pokemon, ActiveMove> - OnDamagingHit ==========
    
    public static EventHandlerInfo? Create(
        EventId id,
  Action<Battle, int, Pokemon, Pokemon, ActiveMove>? handler,
        int? priority = null,
   int? order = null,
      int? subOrder = null,
        bool usesSpeed = false)
    {
        if (handler == null) return null;
        
    return new EventHandlerInfo
        {
      Id = id,
            Handler = handler,
            Priority = priority,
            Order = order,
       SubOrder = subOrder,
  UsesSpeed = usesSpeed,
      ExpectedParameterTypes = new[] 
        { 
       typeof(Battle), 
              typeof(int), 
      typeof(Pokemon), 
      typeof(Pokemon), 
      typeof(ActiveMove) 
            },
   ExpectedReturnType = typeof(void),
      };
    }

    // ========== Action<Battle, Pokemon, Pokemon, IEffect> - OnResidual ==========
    
    public static EventHandlerInfo? Create(
        EventId id,
  Action<Battle, Pokemon, Pokemon, IEffect>? handler,
        int? priority = null,
        int? order = null,
  int? subOrder = null,
        bool usesSpeed = false)
    {
        if (handler == null) return null;
        
    return new EventHandlerInfo
        {
     Id = id,
            Handler = handler,
     Priority = priority,
     Order = order,
            SubOrder = subOrder,
            UsesSpeed = usesSpeed,
            ExpectedParameterTypes = new[] 
       { 
    typeof(Battle), 
     typeof(Pokemon), 
  typeof(Pokemon), 
            typeof(IEffect) 
       },
      ExpectedReturnType = typeof(void),
        };
    }

    // ========== Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect> ==========
    
    public static EventHandlerInfo? Create(
        EventId id,
      Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? handler,
        int? priority = null,
        bool usesSpeed = false)
    {
  if (handler == null) return null;
      
     return new EventHandlerInfo
        {
       Id = id,
            Handler = handler,
    Priority = priority,
UsesSpeed = usesSpeed,
     ExpectedParameterTypes = new[] 
            { 
      typeof(Battle), 
             typeof(SparseBoostsTable), 
          typeof(Pokemon), 
            typeof(Pokemon), 
       typeof(IEffect) 
     },
            ExpectedReturnType = typeof(void),
        };
    }

    // ========== ModifierSourceMoveHandler - OnBasePower, etc. ==========
    
    public static EventHandlerInfo? CreateModifierSourceMove(
        EventId id,
        ModifierSourceMoveHandler? handler,
        int? priority = null,
        bool usesSpeed = false)
    {
        if (handler == null) return null;
        
      return new EventHandlerInfo
        {
  Id = id,
         Handler = handler,
            Priority = priority,
       UsesSpeed = usesSpeed,
       ExpectedParameterTypes = new[] 
            { 
             typeof(Battle), 
         typeof(int), // relayVar
        typeof(Pokemon), // source
      typeof(Pokemon), // target
         typeof(ActiveMove) 
 },
            ExpectedReturnType = typeof(DoubleVoidUnion),
        };
 }

    // ========== VoidSourceMoveHandler - OnBeforeMove, OnAfterMove ==========
  
    public static EventHandlerInfo? CreateVoidSourceMove(
        EventId id,
     VoidSourceMoveHandler? handler,
     int? priority = null,
 bool usesSpeed = false)
    {
        if (handler == null) return null;
        
        return new EventHandlerInfo
     {
       Id = id,
         Handler = handler,
   Priority = priority,
            UsesSpeed = usesSpeed,
 ExpectedParameterTypes = new[] 
  { 
        typeof(Battle), 
             typeof(Pokemon), // target
          typeof(Pokemon), // source
           typeof(ActiveMove) 
},
ExpectedReturnType = typeof(BoolVoidUnion),
        };
    }

    // ========== Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?> - OnDamage ==========
    
    public static EventHandlerInfo? Create(
        EventId id,
        Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? handler,
        int? priority = null,
        bool usesSpeed = false)
    {
        if (handler == null) return null;
   
     return new EventHandlerInfo
     {
     Id = id,
      Handler = handler,
     Priority = priority,
            UsesSpeed = usesSpeed,
            ExpectedParameterTypes = new[] 
            { 
           typeof(Battle), 
                typeof(int), 
         typeof(Pokemon), 
      typeof(Pokemon), 
    typeof(IEffect) 
   },
            ExpectedReturnType = typeof(IntBoolVoidUnion),
        };
}

    // ========== Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?> - OnSetStatus ==========
    
    public static EventHandlerInfo? CreateSetStatus(
        EventId id,
     Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? handler,
   int? priority = null,
        bool usesSpeed = false)
    {
        if (handler == null) return null;
        
 return new EventHandlerInfo
        {
     Id = id,
            Handler = handler,
      Priority = priority,
            UsesSpeed = usesSpeed,
        ExpectedParameterTypes = new[] 
   { 
        typeof(Battle), 
  typeof(Condition), 
    typeof(Pokemon), 
   typeof(Pokemon), 
        typeof(IEffect) 
            },
            ExpectedReturnType = typeof(BoolVoidUnion),
        };
    }

    // ========== Action<Battle, Condition, Pokemon, Pokemon, IEffect> - OnAfterSetStatus ==========
    
    public static EventHandlerInfo? CreateAfterSetStatus(
        EventId id,
    Action<Battle, Condition, Pokemon, Pokemon, IEffect>? handler,
        int? priority = null,
        bool usesSpeed = false)
    {
        if (handler == null) return null;
        
 return new EventHandlerInfo
        {
            Id = id,
            Handler = handler,
       Priority = priority,
  UsesSpeed = usesSpeed,
   ExpectedParameterTypes = new[] 
    { 
      typeof(Battle), 
      typeof(Condition), 
  typeof(Pokemon), 
       typeof(Pokemon), 
        typeof(IEffect) 
       },
            ExpectedReturnType = typeof(void),
  };
    }

 // ========== Generic wrapper for delegates we haven't typed yet ==========
    
    /// <summary>
    /// Generic fallback for delegates that don't have a specific builder method yet.
    /// Use this temporarily during migration, then create specific typed methods.
    /// </summary>
    public static EventHandlerInfo? CreateGeneric(
   EventId id,
        Delegate? handler,
        Type[]? expectedParameterTypes = null,
        Type? expectedReturnType = null,
 int? priority = null,
        int? order = null,
        int? subOrder = null,
        bool usesSpeed = false,
        bool usesEffectOrder = false)
    {
        if (handler == null) return null;
        
      return new EventHandlerInfo
        {
   Id = id,
     Handler = handler,
     Priority = priority,
       Order = order,
   SubOrder = subOrder,
  UsesSpeed = usesSpeed,
   UsesEffectOrder = usesEffectOrder,
  ExpectedParameterTypes = expectedParameterTypes,
    ExpectedReturnType = expectedReturnType,
        };
    }
}
