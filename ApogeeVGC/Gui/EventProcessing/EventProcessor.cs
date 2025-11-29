using ApogeeVGC.Gui.Animations;
using ApogeeVGC.Gui.State;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Gui.EventProcessing;

/// <summary>
/// Processes battle events sequentially, updating state and triggering animations.
/// Ensures animations complete before processing the next event.
/// </summary>
public class EventProcessor
{
    private readonly GuiBattleState _battleState;
    private readonly AnimationCoordinator? _animationCoordinator;
    private bool _isProcessingEvent;
    private bool _waitingForAnimation;
    
    public EventProcessor(GuiBattleState battleState, AnimationCoordinator? animationCoordinator)
    {
        _battleState = battleState;
        _animationCoordinator = animationCoordinator;
    }
    
    /// <summary>
    /// Process a single battle event, updating state and triggering animation.
    /// Returns true if an animation was started (caller should wait for completion).
    /// Returns false if no animation was needed (can immediately process next event).
    /// </summary>
    public bool ProcessEvent(BattleMessage message)
    {
        if (_isProcessingEvent)
        {
            Console.WriteLine("[EventProcessor] WARNING: ProcessEvent called while already processing an event");
            return false;
        }
        
        _isProcessingEvent = true;
        bool animationStarted = false;
        
        try
        {
            Console.WriteLine($"[EventProcessor] Processing event: {message.GetType().Name}");
            
            // Update battle state based on message
            _battleState.ProcessMessage(message);
            
            // Trigger animation via coordinator
            if (_animationCoordinator != null)
            {
                // AnimationCoordinator.ProcessMessage will trigger appropriate animations
                _animationCoordinator.ProcessMessage(message);
                
                // Check if any animations are now active
                animationStarted = _animationCoordinator.HasActiveAnimations();
                if (animationStarted)
                {
                    Console.WriteLine($"[EventProcessor] Animation started for event: {message.GetType().Name}");
                    _waitingForAnimation = true;
                }
            }
            
            return animationStarted;
        }
        finally
        {
            _isProcessingEvent = false;
        }
    }
    
    /// <summary>
    /// Check if animations are still active (caller should keep waiting)
    /// </summary>
    public bool IsWaitingForAnimation()
    {
        if (!_waitingForAnimation)
        {
            return false;
        }
        
        // Check if animation coordinator still has active animations
        bool hasActiveAnimations = _animationCoordinator?.HasActiveAnimations() ?? false;
        
        if (!hasActiveAnimations)
        {
            Console.WriteLine("[EventProcessor] All animations complete");
            _waitingForAnimation = false;
        }
        
        return _waitingForAnimation;
    }
    
    /// <summary>
    /// Validate the current battle state against the authoritative end-of-turn perspective.
    /// Returns true if state matches, false if discrepancies are found.
    /// </summary>
    public bool ValidateState(BattlePerspective endPerspective)
    {
        Console.WriteLine("[EventProcessor] Validating battle state against end-of-turn perspective");
        
        bool isValid = true;
        
        // Validate player Pokemon
        foreach (var (slot, stateP) in _battleState.PlayerActive)
        {
            if (slot >= endPerspective.PlayerSide.Active.Count)
            {
                Console.WriteLine($"[EventProcessor] ERROR: Player slot {slot} exists in state but not in perspective");
                isValid = false;
                continue;
            }
            
            PokemonPerspective? perspP = endPerspective.PlayerSide.Active[slot];
            if (perspP == null && stateP != null)
            {
                Console.WriteLine($"[EventProcessor] ERROR: Player slot {slot} has Pokemon in state but not in perspective");
                isValid = false;
                continue;
            }
            
            if (perspP != null && stateP != null)
            {
                // Validate HP
                if (stateP.Hp != perspP.Hp)
                {
                    Console.WriteLine($"[EventProcessor] ERROR: Player {stateP.Name} HP mismatch - State: {stateP.Hp}, Perspective: {perspP.Hp}");
                    isValid = false;
                }
                
                // Validate fainted status
                bool stateFainted = stateP.Hp <= 0;
                bool perspFainted = perspP.Hp <= 0;
                if (stateFainted != perspFainted)
                {
                    Console.WriteLine($"[EventProcessor] ERROR: Player {stateP.Name} fainted status mismatch - State: {stateFainted}, Perspective: {perspFainted}");
                    isValid = false;
                }
            }
        }
        
        // Validate opponent Pokemon
        foreach (var (slot, stateP) in _battleState.OpponentActive)
        {
            if (slot >= endPerspective.OpponentSide.Active.Count)
            {
                Console.WriteLine($"[EventProcessor] ERROR: Opponent slot {slot} exists in state but not in perspective");
                isValid = false;
                continue;
            }
            
            PokemonPerspective? perspP = endPerspective.OpponentSide.Active[slot];
            if (perspP == null && stateP != null)
            {
                Console.WriteLine($"[EventProcessor] ERROR: Opponent slot {slot} has Pokemon in state but not in perspective");
                isValid = false;
                continue;
            }
            
            if (perspP != null && stateP != null)
            {
                // Validate HP
                if (stateP.Hp != perspP.Hp)
                {
                    Console.WriteLine($"[EventProcessor] ERROR: Opponent {stateP.Name} HP mismatch - State: {stateP.Hp}, Perspective: {perspP.Hp}");
                    isValid = false;
                }
                
                // Validate fainted status
                bool stateFainted = stateP.Hp <= 0;
                bool perspFainted = perspP.Hp <= 0;
                if (stateFainted != perspFainted)
                {
                    Console.WriteLine($"[EventProcessor] ERROR: Opponent {stateP.Name} fainted status mismatch - State: {stateFainted}, Perspective: {perspFainted}");
                    isValid = false;
                }
            }
        }
        
        if (isValid)
        {
            Console.WriteLine("[EventProcessor] ? State validation passed");
        }
        else
        {
            Console.WriteLine("[EventProcessor] ? State validation FAILED - see errors above");
        }
        
        return isValid;
    }
}
