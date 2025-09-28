//using ApogeeVGC.Data;
//using ApogeeVGC.Sim.Core;
//using ApogeeVGC.Sim.Events;
//using ApogeeVGC.Sim.GameObjects;
//using ApogeeVGC.Sim.Moves;
//using ApogeeVGC.Sim.PokemonClasses;
//using ApogeeVGC.Sim.Utils;

//namespace ApogeeVGC.Examples;

///// <summary>
///// Example showing how to use the Library delegate system with intuitive method names
///// </summary>
//public static class DelegateSystemExample
//{
//    /// <summary>
//    /// Example of registering and using delegates with the new intuitive method names
//    /// </summary>
//    public static void RunBattleEngineExample()
//    {
//        var library = new Library();

//        RegisterMoveExamples(library);
//        DemonstrateBattleEngineUsage(library);
//    }

//    private static void RegisterMoveExamples(Library library)
//    {
//        // Register a VoidSourceMove delegate that works for multiple events
//        library.RegisterMoveDelegate<VoidSourceMoveHandler>(
//            EventId.VoidSourceMoveHandler,
//            MoveId.GlacialLance,
//            (battleContext, source, target, move) =>
//            {
//                Console.WriteLine($"{source.Name} used {move.Name} and dealt ice damage to {target.Name}!");
//                // This single handler serves OnAfterHit, OnAfterMove, OnUseMoveMessage, etc.
//            });

//        // Register a ModifierSourceMove delegate for base power/priority modifications
//        library.RegisterMoveDelegate<ModifierSourceMoveHandler>(
//            EventId.ModifierSourceMoveHandler,
//            MoveId.GlacialLance,
//            (battleContext, value, source, target, move) =>
//            {
//                Console.WriteLine($"{move.Name} powered up by weather!");
//                return (int)(value * 1.5); // 50% boost
//            });

//        // Register a ResultMove delegate for hit/immunity checks
//        library.RegisterMoveDelegate<ResultMoveHandler>(
//            EventId.ResultMoveHandler,
//            MoveId.Protect,
//            (battleContext, target, source, move) =>
//            {
//                Console.WriteLine($"{source.Name} is protected from attack!");
//                return false; // Block the move
//            });
//    }

//    private static void DemonstrateBattleEngineUsage(Library library)
//    {
//        var battleContext = CreateMockBattleContext(library);
//        var source = CreateMockPokemon("Glastrier");
//        var target = CreateMockPokemon("Charizard");
//        var move = library.Moves[MoveId.GlacialLance];
//        var protectMove = library.Moves[MoveId.Protect];

//        Console.WriteLine("=== Battle Engine with Intuitive Delegate Names ===\n");

//        // Example 1: After Hit Processing
//        Console.WriteLine("1. After Hit Processing:");
//        var onAfterHitHandler = move.GetOnAfterHitDelegate(library);
//        if (onAfterHitHandler != null)
//        {
//            Console.WriteLine("   Executing OnAfterHit...");
//            onAfterHitHandler(battleContext, source, target, move);
//        }

//        // Example 2: Base Power Calculation
//        Console.WriteLine("\n2. Base Power Calculation:");
//        var onBasePowerHandler = move.GetOnBasePowerDelegate(library);
//        if (onBasePowerHandler != null)
//        {
//            Console.WriteLine("   Executing OnBasePower...");
//            int originalPower = move.BasePower;
//            int modifiedPower = onBasePowerHandler(battleContext, originalPower, source, target, move) ?? originalPower;
//            Console.WriteLine($"   Base power: {originalPower} -> {modifiedPower}");
//        }

//        // Example 3: Hit Check
//        Console.WriteLine("\n3. Hit Check:");
//        var onHitHandler = protectMove.GetOnHitDelegate(library);
//        if (onHitHandler != null)
//        {
//            Console.WriteLine("   Executing OnHit...");
//            bool? hitResult = onHitHandler(battleContext, target, source, protectMove);
//            Console.WriteLine($"   Hit result: {(hitResult == false ? "Blocked" : "Hit")}");
//        }

//        // Example 4: Different Event Types for Same Move
//        Console.WriteLine("\n4. Multiple Event Types for Same Move:");

//        var afterMoveHandler = move.GetOnAfterMoveDelegate(library);
//        if (afterMoveHandler != null)
//        {
//            Console.WriteLine("   OnAfterMove: Available");
//        }

//        var useMoveMessageHandler = move.GetOnUseMoveMessageDelegate(library);
//        if (useMoveMessageHandler != null)
//        {
//            Console.WriteLine("   OnUseMoveMessage: Available");
//        }

//        var modifyPriorityHandler = move.GetOnModifyPriorityDelegate(library);
//        if (modifyPriorityHandler != null)
//        {
//            Console.WriteLine("   OnModifyPriority: Available");
//        }

//        Console.WriteLine("   (All these use the same registered VoidSourceMove/ModifierSourceMove handlers)");
//    }

//    private static BattleContext CreateMockBattleContext(Library library)
//    {
//        return new BattleContext
//        {
//            Library = library,
//            Random = new Random()
//        };
//    }

//    private static Pokemon CreateMockPokemon(string name)
//    {
//        Console.WriteLine($"Mock Pokemon: {name}");
//        return null!; // Placeholder - replace with actual Pokemon creation
//    }
//}

///// <summary>
///// Example showing how to use the new delegate system in actual battle engine code
///// </summary>
//public static class BattleEngineIntegrationExample
//{
//    /// <summary>
//    /// Example battle engine method showing clean delegate usage
//    /// </summary>
//    public static void ExecuteMoveSequence(Move move, Pokemon source, Pokemon target, BattleContext battleContext)
//    {
//        var library = battleContext.Library;

//        // 1. Before Move Check
//        var beforeMoveHandler = move.GetBeforeMoveCallbackDelegate(library);
//        if (beforeMoveHandler != null)
//        {
//            bool? shouldStopResult = beforeMoveHandler(battleContext, source, target, move);
//            if (shouldStopResult == true)
//            {
//                Console.WriteLine($"{move.Name} was stopped before execution!");
//                return;
//            }
//        }

//        // 2. Try Move Check
//        var onTryHandler = move.GetOnTryDelegate(library);
//        if (onTryHandler != null)
//        {
//            bool? canUse = onTryHandler(battleContext, source, target, move);
//            if (canUse == false)
//            {
//                Console.WriteLine($"{source.Name} cannot use {move.Name}!");

//                // Handle move failure
//                var onMoveFailHandler = move.GetOnMoveFailDelegate(library);
//                onMoveFailHandler?.Invoke(battleContext, target, source, move);
//                return;
//            }
//        }

//        // 3. Prepare Hit Check
//        var onPrepareHitHandler = move.GetOnPrepareHitDelegate(library);
//        if (onPrepareHitHandler != null)
//        {
//            bool? prepared = onPrepareHitHandler(battleContext, target, source, move);
//            if (prepared == false)
//            {
//                Console.WriteLine($"{move.Name} failed to prepare hit!");
//                return;
//            }
//        }

//        // 4. Try Hit Check
//        var onTryHitHandler = move.GetOnTryHitDelegate(library);
//        if (onTryHitHandler != null)
//        {
//            var hitResult = onTryHitHandler(battleContext, source, target, move);
//            // Note: hitResult is IntBoolUnion?, check for failure appropriately
//            if (hitResult != null) // Simplified check - in real code you'd check the union properly
//            {
//                Console.WriteLine($"{move.Name} hit successfully!");
//            }
//            else
//            {
//                Console.WriteLine($"{move.Name} failed to hit!");
//                return;
//            }
//        }

//        // 5. Base Power Calculation
//        int finalBasePower = move.BasePower;
//        var onBasePowerHandler = move.GetOnBasePowerDelegate(library);
//        if (onBasePowerHandler != null)
//        {
//            finalBasePower = onBasePowerHandler(battleContext, finalBasePower, source, target, move) ?? finalBasePower;
//        }

//        // 6. Execute the move (damage calculation, etc.)
//        Console.WriteLine($"{source.Name} used {move.Name} with {finalBasePower} base power!");

//        // 7. After Hit Effects
//        var onAfterHitHandler = move.GetOnAfterHitDelegate(library);
//        onAfterHitHandler?.Invoke(battleContext, source, target, move);

//        // 8. After Move Effects
//        var onAfterMoveHandler = move.GetOnAfterMoveDelegate(library);
//        onAfterMoveHandler?.Invoke(battleContext, source, target, move);

//        // 9. Move Message
//        var onUseMoveMessageHandler = move.GetOnUseMoveMessageDelegate(library);
//        onUseMoveMessageHandler?.Invoke(battleContext, source, target, move);
//    }

//    /// <summary>
//    /// Example of handling different move event contexts
//    /// </summary>
//    public static void HandleMoveEventContexts(Move move, Pokemon source, Pokemon target, BattleContext battleContext)
//    {
//        var library = battleContext.Library;

//        // The beauty of this system: Same registered handler, different contexts
//        var voidSourceMoveHandler = move.GetVoidSourceMoveDelegate(library);

//        if (voidSourceMoveHandler != null)
//        {
//            // Use the same handler for different event contexts
//            Console.WriteLine("Calling handler for OnAfterHit context...");
//            voidSourceMoveHandler(battleContext, source, target, move);

//            Console.WriteLine("Calling handler for OnAfterMove context...");
//            voidSourceMoveHandler(battleContext, source, target, move);

//            Console.WriteLine("Calling handler for OnUseMoveMessage context...");
//            voidSourceMoveHandler(battleContext, source, target, move);
//        }

//        // Or use the specific named methods for clarity
//        var afterHitHandler = move.GetOnAfterHitDelegate(library);
//        var afterMoveHandler = move.GetOnAfterMoveDelegate(library);
//        var messageHandler = move.GetOnUseMoveMessageDelegate(library);

//        // These all return the same handler instance, but the intent is clear
//        Console.WriteLine($"OnAfterHit handler: {afterHitHandler != null}");
//        Console.WriteLine($"OnAfterMove handler: {afterMoveHandler != null}");
//        Console.WriteLine($"OnUseMoveMessage handler: {messageHandler != null}");
//        Console.WriteLine($"All handlers are the same instance: {ReferenceEquals(afterHitHandler, afterMoveHandler)}");
//    }
//}

///// <summary>
///// Comparison showing the improvement in readability
///// </summary>
//public static class ReadabilityComparison
//{
//    public static void ShowComparison()
//    {
//        Console.WriteLine("=== Readability Comparison ===\n");

//        Console.WriteLine("OLD WAY (Generic):");
//        Console.WriteLine("var handler = move.GetVoidSourceMoveDelegate(library);");
//        Console.WriteLine("// What does this handler do? Unclear from the name.");
//        Console.WriteLine("");

//        Console.WriteLine("NEW WAY (Intuitive):");
//        Console.WriteLine("var afterHitHandler = move.GetOnAfterHitDelegate(library);");
//        Console.WriteLine("var basePowerHandler = move.GetOnBasePowerDelegate(library);");
//        Console.WriteLine("var hitCheckHandler = move.GetOnHitDelegate(library);");
//        Console.WriteLine("// Crystal clear what each handler does!");
//        Console.WriteLine("");

//        Console.WriteLine("BATTLE ENGINE CODE:");
//        Console.WriteLine("// OLD:");
//        Console.WriteLine("move.GetVoidSourceMoveDelegate(library)?.Invoke(...);");
//        Console.WriteLine("");
//        Console.WriteLine("// NEW:");
//        Console.WriteLine("move.GetOnAfterHitDelegate(library)?.Invoke(...);");
//        Console.WriteLine("move.GetOnBasePowerDelegate(library)?.Invoke(...);");
//        Console.WriteLine("move.GetOnTryHitDelegate(library)?.Invoke(...);");
//        Console.WriteLine("");
//        Console.WriteLine("The new way makes the battle engine code self-documenting!");
//    }
//}