using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;

namespace ApogeeVGC.Sim.Core;

public partial class Battle
{
    /// <summary>
    /// Creates a deep copy of the battle state for MCTS simulation purposes.
    /// This method creates independent copies of all mutable state while sharing immutable references.
    /// </summary>
    /// <returns>A new Battle instance with copied state</returns>
    public Battle DeepCopy(bool? printDebug = null)
    {
        return new Battle
        {
            // Shared immutable references
            Library = Library, // Library is read-only, safe to share

            // Deep copy mutable components using their Copy methods
            Field = Field.Copy(),
            Side1 = Side1.Copy(),
            Side2 = Side2.Copy(),

            // Copy simple state
            Turn = Turn,
            Player1State = Player1State,
            Player2State = Player2State,
            Player1PendingChoice = Player1PendingChoice,
            Player2PendingChoice = Player2PendingChoice,
            PrintDebug = printDebug ?? PrintDebug,
            BattleSeed = BattleSeed,
            // Note: ChoiceLock gets a new instance automatically
            // Note: _battleRandom will be initialized with the same seed when first accessed
        };
    }

    /// <summary>
    /// Creates a deep copy of the battle and applies a choice to it.
    /// This is the main method used by MCTS for creating child nodes.
    /// </summary>
    /// <param name="playerId">The player making the choice</param>
    /// <param name="choice">The choice to apply</param>
    /// <param name="printDebug">Manually set debug printing</param>
    /// <returns>A new Battle instance with the choice applied</returns>
    public Battle DeepCopyAndApplyChoice(PlayerId playerId, BattleChoice choice, bool? printDebug = null)
    {
        Core.Battle copy = DeepCopy(printDebug);

        try
        {
            // Apply the choice to the copied battle
            copy.SubmitChoice(playerId, choice);
            return copy;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to apply choice {choice} for player {playerId}: {ex.Message}", ex);
        }
    }

    private static int RoundedDownAtHalf(double value)
    {
        return (int)(value + 0.5 - double.Epsilon);
    }
}