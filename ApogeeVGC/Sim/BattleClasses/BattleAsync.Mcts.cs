using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Turns;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync : IBattleMctsOperations
{
    void IBattleMctsOperations.ApplyChoiceSync(PlayerId playerId, BattleChoice choice)
    {
        // Apply the choice synchronously based on current battle state
        switch (CurrentTurn)
        {
            case TeamPreviewTurn:
                ApplyTeamPreviewChoiceSync(playerId, choice);
                break;
                
            case GameplayTurn:
                ApplyGameplayChoiceSync(playerId, choice);
                break;
        }
    }
    
    Random IBattleMctsOperations.GetRandom()
    {
        return BattleRandom;
    }
    
    GameplayExecutionStage IBattleMctsOperations.GetExecutionStage()
    {
        return ExecutionStage;
    }
    
    void IBattleMctsOperations.SetExecutionStage(GameplayExecutionStage stage)
    {
        ExecutionStage = stage;
    }
    
    private void ApplyTeamPreviewChoiceSync(PlayerId playerId, BattleChoice choice)
    {
        // Use the existing team preview logic
        ApplyTeamPreviewChoice(playerId, choice);
        
        // For MCTS, we need simpler advancement logic
        if (ShouldAdvanceToGameplay())
        {
            AdvanceToGameplaySync();
        }
    }
    
    private void ApplyGameplayChoiceSync(PlayerId playerId, BattleChoice choice)
    {
        // Apply choice based on current execution stage
        switch (ExecutionStage)
        {
            case GameplayExecutionStage.TurnStart:
                ApplyTurnStartChoiceSync(playerId, choice);
                break;
                
            case GameplayExecutionStage.ForceSwitch:
                ApplyForcedSwitchChoiceSync(playerId, choice);
                break;
                
            case GameplayExecutionStage.FaintedSwitch:
                ApplyFaintedSwitchChoiceSync(playerId, choice);
                break;
        }
    }
    
    private void ApplyTurnStartChoiceSync(PlayerId playerId, BattleChoice choice)
    {
        if (choice is not SlotChoice slotChoice) return;
        
        // Execute the choice directly on the battle state
        switch (slotChoice)
        {
            case SlotChoice.MoveChoice moveChoice:
                ExecuteMoveChoiceSync(moveChoice);
                break;
                
            case SlotChoice.SwitchChoice switchChoice:
                ExecuteSwitchChoiceSync(switchChoice, playerId);
                break;
        }
        
        // Simulate turn progression
        SimulateTurnProgressionSync();
    }
    
    private void ApplyForcedSwitchChoiceSync(PlayerId playerId, BattleChoice choice)
    {
        if (choice is SlotChoice.SwitchChoice switchChoice)
        {
            ExecuteSwitchChoiceSync(switchChoice, playerId);
        }
    }
    
    private void ApplyFaintedSwitchChoiceSync(PlayerId playerId, BattleChoice choice)
    {
        if (choice is SlotChoice.SwitchChoice switchChoice)
        {
            ExecuteSwitchChoiceSync(switchChoice, playerId);
        }
    }
    
    private void ExecuteMoveChoiceSync(SlotChoice.MoveChoice moveChoice)
    {
        // Simplified move execution for MCTS simulation
        Pokemon attacker = moveChoice.Attacker;
        Move move = moveChoice.Move;
        
        // Basic validation
        if (attacker.IsFainted || move.UsedPp >= move.Pp) return;
        
        // Use PP
        move.UsedPp++;
        
        // Get targets
        var targets = ResolveTargetsSync(moveChoice);
        
        // Apply damage/effects to targets
        foreach (Pokemon target in targets.Where(t => !t.IsFainted))
        {
            // Simplified damage calculation for simulation
            if (move.Category != MoveCategory.Status)
            {
                int damage = CalculateDamageSync(attacker, target, move);
                target.Damage(damage);
            }
            
            // Apply status conditions if any (simplified probability)
            if (move.Condition != null && BattleRandom.NextDouble() < 0.3)
            {
                target.AddCondition(move.Condition, Context, attacker, move);
            }
        }
    }
    
    private void ExecuteSwitchChoiceSync(SlotChoice.SwitchChoice switchChoice, PlayerId playerId)
    {
        Side side = GetSide(playerId);
        Pokemon outPokemon = switchChoice.SwitchOutPokemon;
        Pokemon inPokemon = switchChoice.SwitchInPokemon;
        
        // Perform the switch
        outPokemon.OnSwitchOut();
        side.SwitchSlots(switchChoice.SwitchOutSlot, switchChoice.SwitchInSlot);
        
        // Trigger switch-in effects (simplified)
        Field.OnPokemonSwitchIn(inPokemon, playerId, Context);
        inPokemon.OnSwitchIn(Field, AllActivePokemonArray, Context);
    }
    
    private List<Pokemon> ResolveTargetsSync(SlotChoice.MoveChoice moveChoice)
    {
        // Simplified target resolution for MCTS
        Move move = moveChoice.Move;
        Pokemon attacker = moveChoice.Attacker;
        var targets = new List<Pokemon>();
        
        switch (move.Target)
        {
            case MoveTarget.Normal:
                // Call the existing private method directly
                targets.AddRange(ResolveNormalTargets(moveChoice));
                break;
                
            case MoveTarget.AllAdjacentFoes:
                Side opponentSide = GetOpponentSide(GetSide(attacker.SideId));
                targets.AddRange(opponentSide.AliveActivePokemon);
                break;
                
            case MoveTarget.Self:
                targets.Add(attacker);
                break;
                
            case MoveTarget.AdjacentAlly:
                Pokemon? ally = GetSide(attacker.SideId).GetAliveAlly(attacker.SlotId);
                if (ally != null) targets.Add(ally);
                break;
        }
        
        return targets.Where(t => !t.IsFainted).ToList();
    }
    
    private int CalculateDamageSync(Pokemon attacker, Pokemon defender, Move move)
    {
        // Simplified damage calculation for MCTS simulation
        if (move.BasePower <= 0) return 0;
        
        int level = attacker.Level;
        int attack = attacker.GetAttackStat(move, false);
        int defense = defender.GetDefenseStat(move, false);
        
        // Simplified damage formula
        double baseDamage = (2.0 * level / 5.0 + 2) * move.BasePower * attack / defense / 50.0 + 2;
        
        // Random factor
        double random = 0.85 + BattleRandom.NextDouble() * 0.15;
        
        // Type effectiveness
        double effectiveness = Library.TypeChart.GetMoveEffectiveness(defender.DefensiveTypes, move.Type).
            GetMultiplier();
        
        int finalDamage = (int)(baseDamage * random * effectiveness);
        return Math.Max(1, Math.Min(finalDamage, defender.CurrentHp));
    }
    
    private void SimulateTurnProgressionSync()
    {
        // Simplified turn progression for MCTS
        var faintedPokemon = AllActivePokemon.Where(p => p.IsFainted).ToList();
        
        // If there are fainted Pokemon, we might need to handle switches
        if (faintedPokemon.Any())
        {
            ExecutionStage = GameplayExecutionStage.FaintedSwitch;
        }
        else
        {
            // Check for game end conditions
            if (IsGameOverMcts())
            {
                CreatePostGameTurnSync();
            }
            else
            {
                // Advance to next turn
                AdvanceToNextTurnSync();
            }
        }
    }
    
    private bool ShouldAdvanceToGameplay()
    {
        // Check if both sides have made their team preview choices
        return Side1.HasMadeTeamPreviewChoice() && Side2.HasMadeTeamPreviewChoice();
    }
    
    private void AdvanceToGameplaySync()
    {
        // Trigger the same effects as the async version but synchronously
        HandleEndOfTeamPreviewTurn(); // Existing method
        
        // Create the first gameplay turn
        var firstGameplayTurn = new GameplayTurn
        {
            Side1Start = Side1.Copy(),
            Side2Start = Side2.Copy(), 
            FieldStart = Field.Copy(),
            TurnCounter = 1,
        };
        
        Turns.Add(firstGameplayTurn);
        TurnCounter = 1;
        
        // Set initial execution stage
        ExecutionStage = GameplayExecutionStage.TurnStart;
    }
    
    private bool IsGameOverMcts()
    {
        // Check if all Pokemon on one side are fainted
        return Side1.AllSlots.All(p => p.IsFainted) || 
               Side2.AllSlots.All(p => p.IsFainted);
    }
    
    private void CreatePostGameTurnSync()
    {
        // Determine winner
        PlayerId winner;
        if (Side1.AllSlots.All(p => p.IsFainted))
        {
            winner = PlayerId.Player2;
        }
        else if (Side2.AllSlots.All(p => p.IsFainted))
        {
            winner = PlayerId.Player1;
        }
        else
        {
            // Tie-break based on remaining HP
            int side1Health = Side1.AllSlots.Sum(p => p.CurrentHp);
            int side2Health = Side2.AllSlots.Sum(p => p.CurrentHp);
            winner = side1Health > side2Health ? PlayerId.Player1 : PlayerId.Player2;
        }
        
        // Create PostGameTurn
        var postGameTurn = new PostGameTurn
        {
            Winner = winner,
            Side1Start = Side1.Copy(),
            Side2Start = Side2.Copy(),
            FieldStart = Field.Copy(),
            TurnCounter = TurnCounter,
        };
        
        Turns.Add(postGameTurn);
    }
    
    private void AdvanceToNextTurnSync()
    {
        // Create next gameplay turn
        var nextTurn = new GameplayTurn
        {
            Side1Start = Side1.Copy(),
            Side2Start = Side2.Copy(),
            FieldStart = Field.Copy(),
            TurnCounter = TurnCounter + 1,
        };
        
        Turns.Add(nextTurn);
        TurnCounter++;
    }
}