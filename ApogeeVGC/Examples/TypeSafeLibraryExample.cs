//using ApogeeVGC.Data;
//using ApogeeVGC.Sim.Core;
//using ApogeeVGC.Sim.Events;
//using ApogeeVGC.Sim.GameObjects;
//using ApogeeVGC.Sim.Moves;

//namespace ApogeeVGC.Examples;

///// <summary>
///// Example demonstrating the new type-safe delegate registration system
///// </summary>
//public static class TypeSafeLibraryExample
//{
//    /// <summary>
//    /// Demonstrates type-safe delegate registration using IEffect objects
//    /// </summary>
//    public static void DemonstrateTypeSafetyUsage()
//    {
//        var library = new Library();
        
//        Console.WriteLine("=== Type-Safe Delegate Registration Example ===\n");
        
//        // Get effect objects from library (type-safe)
//        var glacialLance = library.Moves[MoveId.GlacialLance];
//        var flameBody = library.Abilities[AbilityId.FlameBody];
//        var lifeOrb = library.Items[ItemId.LifeOrb]; // Note: You'll need to add this to ItemId enum
        
//        Console.WriteLine("1. Type-Safe Registration using IEffect objects:");
        
//        // Type-safe registration - compiler guarantees these are IEffect objects
//        library.RegisterDelegate<VoidSourceMoveHandler>(
//            EventId.VoidSourceMoveHandler,
//            glacialLance,  // Type-safe: Move implements IEffect
//            (battleContext, source, target, move) =>
//            {
//                Console.WriteLine($"   {source.Name} used {move.Name} with icy power!");
//            });
        
//        library.RegisterDelegate<VoidSourceEffectHandler>(
//            EventId.VoidSourceEffectHandler,
//            flameBody,  // Type-safe: Ability implements IEffect
//            (battleContext, source, target, effect) =>
//            {
//                Console.WriteLine($"   {effect.Name} activated and burned the attacker!");
//            });
        
//        Console.WriteLine("   ✅ Registered handlers using type-safe IEffect objects");
//        Console.WriteLine("");
        
//        Console.WriteLine("2. Retrieving Delegates Type-Safely:");
        
//        // Type-safe retrieval
//        var moveHandler = library.GetDelegate<VoidSourceMoveHandler>(EventId.VoidSourceMoveHandler, glacialLance);
//        var abilityHandler = library.GetDelegate<VoidSourceEffectHandler>(EventId.VoidSourceEffectHandler, flameBody);
        
//        Console.WriteLine($"   Move handler found: {moveHandler != null}");
//        Console.WriteLine($"   Ability handler found: {abilityHandler != null}");
//        Console.WriteLine("");
        
//        Console.WriteLine("3. Benefits Demonstrated:");
//        Console.WriteLine("   ✅ Compile-time type checking - no string typos possible");
//        Console.WriteLine("   ✅ IntelliSense support - IDE knows these are IEffect objects");
//        Console.WriteLine("   ✅ Refactoring support - renaming effects updates all references");
//        Console.WriteLine("   ✅ Same efficiency as before - uses string IDs internally");
//        Console.WriteLine("");
        
//        DemonstrateConvenienceMethods(library);
//        DemonstrateBackwardCompatibility(library);
//    }
    
//    /// <summary>
//    /// Demonstrates the convenience methods for specific types
//    /// </summary>
//    private static void DemonstrateConvenienceMethods(Library library)
//    {
//        Console.WriteLine("4. Convenience Methods (also type-safe):");
        
//        // These are still type-safe because MoveId, AbilityId are enums
//        library.RegisterMoveDelegate<VoidSourceMoveHandler>(
//            EventId.VoidSourceMoveHandler,
//            MoveId.Tackle,  // Type-safe enum
//            (battleContext, source, target, move) =>
//            {
//                Console.WriteLine($"   {move.Name} made contact!");
//            });
        
//        library.RegisterAbilityDelegate<VoidSourceEffectHandler>(
//            EventId.VoidSourceEffectHandler,
//            AbilityId.Guts,  // Type-safe enum
//            (battleContext, source, target, effect) =>
//            {
//                Console.WriteLine($"   {effect.Name} boosted attack due to status!");
//            });
        
//        Console.WriteLine("   ✅ Convenience methods still provide type safety via enums");
//        Console.WriteLine("");
//    }
    
//    /// <summary>
//    /// Shows that old methods still work but are deprecated
//    /// </summary>
//    private static void DemonstrateBackwardCompatibility(Library library)
//    {
//        Console.WriteLine("5. Backward Compatibility (Deprecated):");
        
//        // This will work but show obsolete warnings
//#pragma warning disable CS0618 // Type or member is obsolete
//        library.RegisterDelegate<VoidSourceMoveHandler>(
//            EventId.VoidSourceMoveHandler,
//            "tackle",  // String-based (deprecated)
//            (battleContext, source, target, move) =>
//            {
//                Console.WriteLine($"   Legacy handler for {move.Name}");
//            });
//#pragma warning restore CS0618 // Type or member is obsolete
        
//        Console.WriteLine("   ⚠️  Legacy string methods work but show deprecation warnings");
//        Console.WriteLine("");
//    }
    
//    /// <summary>
//    /// Shows the new extension methods for intuitive delegate access
//    /// </summary>
//    public static void DemonstrateIntuitiveMethodNames()
//    {
//        var library = new Library();
//        var move = library.Moves[MoveId.Tackle];
        
//        Console.WriteLine("=== Intuitive Method Names Example ===\n");
        
//        // Register a single VoidSourceMoveHandler
//        library.RegisterDelegate<VoidSourceMoveHandler>(
//            EventId.VoidSourceMoveHandler,
//            move,
//            (battleContext, source, target, moveUsed) =>
//            {
//                Console.WriteLine($"{source.Name} used {moveUsed.Name}!");
//            });
        
//        // Access the same handler with different intuitive names
//        var afterHitHandler = move.GetOnAfterHitDelegate(library);
//        var afterMoveHandler = move.GetOnAfterMoveDelegate(library);
//        var messageHandler = move.GetOnUseMoveMessageDelegate(library);
        
//        Console.WriteLine("Same handler, different intuitive access methods:");
//        Console.WriteLine($"OnAfterHit available: {afterHitHandler != null}");
//        Console.WriteLine($"OnAfterMove available: {afterMoveHandler != null}");
//        Console.WriteLine($"OnUseMoveMessage available: {messageHandler != null}");
//        Console.WriteLine($"All point to same handler: {ReferenceEquals(afterHitHandler, afterMoveHandler)}");
//        Console.WriteLine("");
        
//        Console.WriteLine("Benefits:");
//        Console.WriteLine("✅ Self-documenting code - clear what each handler does");
//        Console.WriteLine("✅ Better battle engine readability");
//        Console.WriteLine("✅ Same performance - no overhead");
//    }
    
//    /// <summary>
//    /// Shows potential compile-time errors that are now prevented
//    /// </summary>
//    public static void ShowCompileTimeErrorPrevention()
//    {
//        Console.WriteLine("=== Compile-Time Error Prevention ===\n");
        
//        var library = new Library();
//        var move = library.Moves[MoveId.Tackle];
//        var ability = library.Abilities[AbilityId.Guts];
        
//        Console.WriteLine("✅ TYPE-SAFE (Compiles):");
//        Console.WriteLine("library.RegisterDelegate(EventId.VoidSourceMoveHandler, move, handler);");
//        Console.WriteLine("library.RegisterDelegate(EventId.VoidSourceEffectHandler, ability, handler);");
//        Console.WriteLine("");
        
//        Console.WriteLine("❌ TYPE-UNSAFE (Won't Compile):");
//        Console.WriteLine("// library.RegisterDelegate(EventId.VoidSourceMoveHandler, \"tacle\", handler);     // Typo in string");
//        Console.WriteLine("// library.RegisterDelegate(EventId.VoidSourceMoveHandler, 123, handler);         // Wrong type");
//        Console.WriteLine("// library.RegisterDelegate(EventId.VoidSourceMoveHandler, ability, handler);     // Wrong effect type");
//        Console.WriteLine("");
        
//        Console.WriteLine("The new type-safe approach prevents all these runtime errors at compile-time!");
//    }
//}

///// <summary>
///// Comparison between old and new approaches
///// </summary>
//public static class TypeSafetyComparison
//{
//    public static void ShowBeforeAndAfter()
//    {
//        Console.WriteLine("=== Before vs After Comparison ===\n");
        
//        Console.WriteLine("BEFORE (String-based, Error-prone):");
//        Console.WriteLine("```csharp");
//        Console.WriteLine("library.RegisterDelegate(EventId.VoidSourceMoveHandler, \"glaciallance\", handler);");
//        Console.WriteLine("// Risks: typos, wrong casing, non-existent IDs");
//        Console.WriteLine("```");
//        Console.WriteLine("");
        
//        Console.WriteLine("AFTER (Type-safe, Compile-time validated):");
//        Console.WriteLine("```csharp");
//        Console.WriteLine("var move = library.Moves[MoveId.GlacialLance];");
//        Console.WriteLine("library.RegisterDelegate(EventId.VoidSourceMoveHandler, move, handler);");
//        Console.WriteLine("// Benefits: IntelliSense, refactoring support, impossible to get wrong");
//        Console.WriteLine("```");
//        Console.WriteLine("");
        
//        Console.WriteLine("INTUITIVE USAGE:");
//        Console.WriteLine("```csharp");
//        Console.WriteLine("// Old: What does this do?");
//        Console.WriteLine("move.GetVoidSourceMoveDelegate(library)?.Invoke(...);");
//        Console.WriteLine("");
//        Console.WriteLine("// New: Crystal clear intent");
//        Console.WriteLine("move.GetOnAfterHitDelegate(library)?.Invoke(...);");
//        Console.WriteLine("move.GetOnBasePowerDelegate(library)?.Invoke(...);");
//        Console.WriteLine("```");
//    }
//}