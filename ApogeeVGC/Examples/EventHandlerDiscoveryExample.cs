//using ApogeeVGC.Data;
//using ApogeeVGC.Sim.Core;
//using ApogeeVGC.Sim.Events;
//using ApogeeVGC.Sim.GameObjects;
//using ApogeeVGC.Sim.Moves;
//using ApogeeVGC.Sim.PokemonClasses;

//namespace ApogeeVGC.Examples;

///// <summary>
///// Example showing how to use the event handler discovery system similar to Showdown
///// </summary>
//public static class EventHandlerDiscoveryExample
//{
//    public static void ShowEventHandlerDiscovery()
//    {
//        var library = new Library();
//        var battleContext = CreateMockBattleContext(library);
        
//        // Register some event handlers
//        RegisterExampleHandlers(library);
        
//        // Create mock Pokemon
//        var attacker = CreateMockPokemon("Charizard");
//        var defender = CreateMockPokemon("Pikachu");
        
//        Console.WriteLine("=== Event Handler Discovery Example ===\n");
        
//        // Example 1: Find all damage event handlers
//        Console.WriteLine("1. Finding Damage Event Handlers:");
//        var damageHandlers = battleContext.FindEventHandlers(EventId.OnDamage, defender, attacker);
        
//        Console.WriteLine($"   Found {damageHandlers.Count} damage handlers:");
//        foreach (var handler in damageHandlers)
//        {
//            Console.WriteLine($"   - {handler.Effect.Name} ({handler.Effect.EffectType}) Priority: {handler.Priority}");
//        }
        
//        // Example 2: Find base power modification handlers
//        Console.WriteLine("\n2. Finding Base Power Event Handlers:");
//        var basePowerHandlers = battleContext.FindEventHandlers(EventId.ModifierSourceMoveHandler, defender, attacker);
        
//        Console.WriteLine($"   Found {basePowerHandlers.Count} base power handlers:");
//        foreach (var handler in basePowerHandlers)
//        {
//            Console.WriteLine($"   - {handler.Effect.Name} ({handler.Effect.EffectType}) Priority: {handler.Priority}");
//        }
        
//        // Example 3: Run an event with discovered handlers
//        Console.WriteLine("\n3. Running Damage Event with Handlers:");
//        int initialDamage = 100;
        
//        // This is similar to Showdown's runEvent
//        int finalDamage = battleContext.RunEvent(EventId.OnDamage, initialDamage, defender, attacker);
        
//        Console.WriteLine($"   Initial damage: {initialDamage}");
//        Console.WriteLine($"   Final damage after handlers: {finalDamage}");
        
//        // Example 4: Multi-target event handling
//        Console.WriteLine("\n4. Multi-Target Event Handling:");
//        var targets = new[] { defender, CreateMockPokemon("Blastoise") };
//        var multiHandlers = battleContext.FindEventHandlers(EventId.OnAfterSubDamage, targets, attacker);
        
//        Console.WriteLine($"   Found {multiHandlers.Count} handlers for multi-target event:");
//        foreach (var handler in multiHandlers.GroupBy(h => h.Index))
//        {
//            Console.WriteLine($"   Target {handler.Key}: {handler.Count()} handlers");
//        }
//    }
    
//    private static void RegisterExampleHandlers(Library library)
//    {
//        // Register ability handlers
//        library.RegisterAbilityDelegate<VoidSourceEffectHandler>(
//            EventId.VoidSourceEffectHandler,
//            AbilityId.FlameBody,
//            (battleContext, source, target, effect) =>
//            {
//                Console.WriteLine($"      Flame Body activated on {target.Name}!");
//            });
        
//        // Register item handlers  
//        library.RegisterItemDelegate<ModifierSourceEffectHandler>(
//            EventId.ModifierSourceEffectHandler,
//            ItemId.LifeOrb,
//            (battleContext, damage, source, target, effect) =>
//            {
//                Console.WriteLine($"      Life Orb boosting damage by 30%!");
//                return (int)(damage * 1.3);
//            });
        
//        // Register move handlers
//        library.RegisterMoveDelegate<VoidSourceMoveHandler>(
//            EventId.VoidSourceMoveHandler,
//            MoveId.GlacialLance,
//            (battleContext, source, target, move) =>
//            {
//                Console.WriteLine($"      {move.Name} hit {target.Name}!");
//            });
//    }
    
//    private static BattleContext CreateMockBattleContext(Library library)
//    {
//        return new BattleContext
//        {
//            Library = library,
//            Random = new Random(),
//            Field = CreateMockField()
//        };
//    }
    
//    private static Pokemon CreateMockPokemon(string name)
//    {
//        // In real implementation, this would create a proper Pokemon
//        Console.WriteLine($"Mock Pokemon created: {name}");
//        return null!; // Placeholder
//    }
    
//    private static Field CreateMockField()
//    {
//        // In real implementation, this would create a proper Field
//        return null!; // Placeholder
//    }
//}

///// <summary>
///// Example showing direct comparison with Showdown's event system
///// </summary>
//public static class ShowdownComparisonExample
//{
//    public static void ShowComparison()
//    {
//        Console.WriteLine("=== Showdown vs Your Implementation Comparison ===\n");
        
//        Console.WriteLine("SHOWDOWN (TypeScript):");
//        Console.WriteLine("```typescript");
//        Console.WriteLine("const handlers = this.findEventHandlers(target, 'Damage', source);");
//        Console.WriteLine("for (const handler of handlers) {");
//        Console.WriteLine("    returnVal = handler.callback.apply(this, args);");
//        Console.WriteLine("    if (returnVal !== undefined) relayVar = returnVal;");
//        Console.WriteLine("    if (!relayVar) break;");
//        Console.WriteLine("}");
//        Console.WriteLine("```");
//        Console.WriteLine("");
        
//        Console.WriteLine("YOUR IMPLEMENTATION (C#):");
//        Console.WriteLine("```csharp");
//        Console.WriteLine("var handlers = battleContext.FindEventHandlers(EventId.OnDamage, target, source);");
//        Console.WriteLine("foreach (var handler in handlers) {");
//        Console.WriteLine("    var result = ExecuteHandler(handler, relayVar, target, source, effect);");
//        Console.WriteLine("    if (result != null) relayVar = result;");
//        Console.WriteLine("    if (IsFalsy(relayVar)) break;");
//        Console.WriteLine("}");
//        Console.WriteLine("```");
//        Console.WriteLine("");
        
//        Console.WriteLine("KEY SIMILARITIES:");
//        Console.WriteLine("✓ Both discover handlers from multiple sources");
//        Console.WriteLine("✓ Both sort handlers by priority/speed");
//        Console.WriteLine("✓ Both execute handlers in sequence");
//        Console.WriteLine("✓ Both support relay variables");
//        Console.WriteLine("✓ Both support early exit on falsy values");
//        Console.WriteLine("✓ Both handle multi-target scenarios");
//        Console.WriteLine("");
        
//        Console.WriteLine("YOUR ADVANTAGES:");
//        Console.WriteLine("✓ Type-safe delegates instead of dynamic callbacks");
//        Console.WriteLine("✓ Enum-based event IDs instead of strings");
//        Console.WriteLine("✓ Compile-time validation of handler signatures");
//        Console.WriteLine("✓ Better IntelliSense and debugging support");
//        Console.WriteLine("✓ More efficient execution (no reflection for every call)");
//    }
//}

///// <summary>
///// Example of how to integrate with your existing battle engine
///// </summary>
//public static class BattleEngineIntegrationExample
//{
//    /// <summary>
//    /// Example damage calculation using event handler discovery
//    /// </summary>
//    public static int CalculateDamage(
//        BattleContext battleContext,
//        Move move,
//        Pokemon attacker,
//        Pokemon defender,
//        int baseDamage)
//    {
//        // 1. Find all damage modification handlers
//        var damageHandlers = battleContext.FindEventHandlers(
//            EventId.ModifierSourceMoveHandler, 
//            defender, 
//            attacker);
        
//        int finalDamage = baseDamage;
        
//        // 2. Execute each handler in priority order
//        foreach (var handler in damageHandlers)
//        {
//            if (handler.Callback is ModifierSourceMoveHandler modifier)
//            {
//                var result = modifier(battleContext, finalDamage, attacker, defender, move);
//                if (result.HasValue)
//                {
//                    finalDamage = result.Value;
//                    Console.WriteLine($"{handler.Effect.Name} modified damage: {baseDamage} -> {finalDamage}");
//                }
//            }
//        }
        
//        return finalDamage;
//    }
    
//    /// <summary>
//    /// Example status application using event handler discovery  
//    /// </summary>
//    public static bool TryApplyStatus(
//        BattleContext battleContext,
//        Pokemon target,
//        Pokemon source,
//        Status status)
//    {
//        // Find all immunity/status handlers
//        var immunityHandlers = battleContext.FindEventHandlers(
//            EventId.ResultMoveHandler,
//            target,
//            source);
        
//        // Check each immunity handler
//        foreach (var handler in immunityHandlers)
//        {
//            if (handler.Callback is ResultMoveHandler immunityCheck)
//            {
//                var result = immunityCheck(battleContext, target, source, null); // No move for status
//                if (result == false)
//                {
//                    Console.WriteLine($"{handler.Effect.Name} prevented {status.Name}!");
//                    return false; // Status blocked
//                }
//            }
//        }
        
//        // Apply status
//        target.SetStatus(status);
        
//        // Trigger after-status handlers
//        var afterStatusHandlers = battleContext.FindEventHandlers(
//            EventId.VoidSourceEffectHandler,
//            target,
//            source);
        
//        foreach (var handler in afterStatusHandlers)
//        {
//            if (handler.Callback is VoidSourceEffectHandler afterStatus)
//            {
//                afterStatus(battleContext, source, target, status);
//            }
//        }
        
//        return true;
//    }
//}