//using ApogeeVGC.Player;
//using ApogeeVGC.Sim.Choices;
//using ApogeeVGC.Sim.PokemonClasses;
//using ApogeeVGC.Sim.Turns;
//using ApogeeVGC.Data;
//using ApogeeVGC.Sim.Moves;

//namespace ApogeeVGC.Sim.BattleClasses;

//public partial class BattleAsync : IBattleMctsOperations
//{
//    void IBattleMctsOperations.ApplyChoiceSync(PlayerId playerId, BattleChoice choice)
//    {
//        // Apply the choice synchronously based on current battle state
//        switch (CurrentTurn)
//        {
//            case TeamPreviewTurn:
//                ApplyTeamPreviewChoiceSync(playerId, choice);
//                break;
                
//            case GameplayTurn:
//                ApplyGameplayChoiceSync(playerId, choice);
//                break;
//        }
//    }
    
//    Random IBattleMctsOperations.GetRandom()
//    {
//        return BattleRandom;
//    }
    
//    GameplayExecutionStage IBattleMctsOperations.GetExecutionStage()
//    {
//        return ExecutionStage;
//    }
    
//    void IBattleMctsOperations.SetExecutionStage(GameplayExecutionStage stage)
//    {
//        ExecutionStage = stage;
//    }
    
//    private void ApplyTeamPreviewChoiceSync(PlayerId playerId, BattleChoice choice)
//    {
//        // Use the existing team preview logic
//        ApplyTeamPreviewChoice(playerId, choice);
        
//        // For MCTS, we need simpler advancement logic
//        if (ShouldAdvanceToGameplay())
//        {
//            AdvanceToGameplaySync();
//        }
//    }
    
//    private void ApplyGameplayChoiceSync(PlayerId playerId, BattleChoice choice)
//    {
//        // Apply choice based on current execution stage
//        switch (ExecutionStage)
//        {
//            case GameplayExecutionStage.TurnStart:
//                ApplyTurnStartChoiceSync(playerId, choice);
//                break;
                
//            case GameplayExecutionStage.ForceSwitch:
//                ApplyForcedSwitchChoiceSync(playerId, choice);
//                break;
                
//            case GameplayExecutionStage.FaintedSwitch:
//                ApplyFaintedSwitchChoiceSync(playerId, choice);
//                break;
//        }
//    }
    
//    private void ApplyTurnStartChoiceSync(PlayerId playerId, BattleChoice choice)
//    {
//        if (choice is not SlotChoice slotChoice) return;
        
//        // Execute the choice directly on the battle state
//        switch (slotChoice)
//        {
//            case SlotChoice.MoveChoice moveChoice:
//                ExecuteMoveChoiceSync(moveChoice);
//                break;
                
//            case SlotChoice.SwitchChoice switchChoice:
//                ExecuteSwitchChoiceSync(switchChoice, playerId);
//                break;
//        }
        
//        // Simulate turn progression
//        SimulateTurnProgressionSync();
//    }
    
//    private void ApplyForcedSwitchChoiceSync(PlayerId playerId, BattleChoice choice)
//    {
//        if (choice is SlotChoice.SwitchChoice switchChoice)
//        {
//            ExecuteSwitchChoiceSync(switchChoice, playerId);
//        }
//    }
    
//    private void ApplyFaintedSwitchChoiceSync(PlayerId playerId, BattleChoice choice)
//    {
//        if (choice is SlotChoice.SwitchChoice switchChoice)
//        {
//            ExecuteSwitchChoiceSync(switchChoice, playerId);
//        }
//    }
    
//    private void ExecuteMoveChoiceSync(SlotChoice.MoveChoice moveChoice)
//    {
//        // Use BattleCore for move execution in MCTS
//        Pokemon attacker = moveChoice.Attacker;
//        Move move = moveChoice.Move;
        
//        // Basic validation
//        if (attacker.IsFainted || move.UsedPp >= move.Pp) return;
        
//        // Resolve targets using BattleCore
//        Side attackingSide = GetSide(attacker.SideId);
//        Side defendingSide = GetOpponentSide(attackingSide);
//        var targets = BattleCore.ResolveActualTargets(moveChoice, attackingSide, defendingSide);
        
//        // Execute move using BattleCore
//        var result = BattleCore.ExecuteMove(attacker, move, targets, Context, GetPlayerIdFromSide(attackingSide));
        
//        // Result success is handled internally by BattleCore
//    }
    
//    private void ExecuteSwitchChoiceSync(SlotChoice.SwitchChoice switchChoice, PlayerId playerId)
//    {
//        // Use BattleCore for switch execution in MCTS
//        Side side = GetSide(playerId);
        
//        var result = BattleCore.ExecuteSwitch(
//            side,
//            switchChoice.SwitchOutSlot,
//            switchChoice.SwitchInSlot,
//            Field,
//            AllActivePokemonArray,
//            Context,
//            playerId
//        );
        
//        // Result success is handled internally by BattleCore
//    }
    
//    private PlayerId GetPlayerIdFromSide(Side side)
//    {
//        return side.PlayerId;
//    }
    
//    private void SimulateTurnProgressionSync()
//    {
//        // Simplified turn progression for MCTS
//        var faintedPokemon = AllActivePokemon.Where(p => p.IsFainted).ToList();
        
//        // If there are fainted Pokemon, we might need to handle switches
//        if (faintedPokemon.Any())
//        {
//            ExecutionStage = GameplayExecutionStage.FaintedSwitch;
//        }
//        else
//        {
//            // Check for game end conditions using BattleCore
//            if (BattleCore.CheckForGameEndConditions(Side1, Side2))
//            {
//                CreatePostGameTurnSync();
//            }
//            else
//            {
//                // Advance to next turn
//                AdvanceToNextTurnSync();
//            }
//        }
//    }
    
//    private bool ShouldAdvanceToGameplay()
//    {
//        // Check if both sides have made their team preview choices
//        return Side1.HasMadeTeamPreviewChoice() && Side2.HasMadeTeamPreviewChoice();
//    }
    
//    private void AdvanceToGameplaySync()
//    {
//        // Trigger the same effects as the async version but synchronously
//        HandleEndOfTeamPreviewTurn(); // Existing method
        
//        // Create the first gameplay turn
//        var firstGameplayTurn = new GameplayTurn
//        {
//            Side1Start = Side1.Copy(),
//            Side2Start = Side2.Copy(), 
//            FieldStart = Field.Copy(),
//            TurnCounter = 1,
//        };
        
//        Turns.Add(firstGameplayTurn);
//        TurnCounter = 1;
        
//        // Set initial execution stage
//        ExecutionStage = GameplayExecutionStage.TurnStart;
//    }
    
//    private void CreatePostGameTurnSync()
//    {
//        // Use BattleCore to determine winner
//        PlayerId? winner = BattleCore.DetermineWinner(Side1, Side2);
        
//        if (winner == null)
//        {
//            // Tie-break based on remaining HP if BattleCore can't determine winner
//            int side1Health = Side1.AllSlots.Sum(p => p.CurrentHp);
//            int side2Health = Side2.AllSlots.Sum(p => p.CurrentHp);
//            winner = side1Health > side2Health ? PlayerId.Player1 : PlayerId.Player2;
//        }
        
//        // Create PostGameTurn
//        var postGameTurn = new PostGameTurn
//        {
//            Winner = winner.Value,
//            Side1Start = Side1.Copy(),
//            Side2Start = Side2.Copy(),
//            FieldStart = Field.Copy(),
//            TurnCounter = TurnCounter,
//        };
        
//        Turns.Add(postGameTurn);
//    }
    
//    private void AdvanceToNextTurnSync()
//    {
//        // Create next gameplay turn
//        var nextTurn = new GameplayTurn
//        {
//            Side1Start = Side1.Copy(),
//            Side2Start = Side2.Copy(),
//            FieldStart = Field.Copy(),
//            TurnCounter = TurnCounter + 1,
//        };
        
//        Turns.Add(nextTurn);
//        TurnCounter++;
//    }
//}