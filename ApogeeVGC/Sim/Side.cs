using ApogeeVGC.Player;

namespace ApogeeVGC.Sim;

public class Side
{
    public required Team Team { get; init; }
    public required PlayerId PlayerId { get; init; }
    public bool PrintDebug { get; init; }

    /// <summary>
    /// Creates a deep copy of this Side for MCTS simulation purposes.
    /// </summary>
    /// <returns>A new Side instance with copied state</returns>
    public Side Copy()
    {
        return new Side
        {
            PlayerId = PlayerId, // Value type, safe to copy
            Team = Team.Copy(),
            PrintDebug = PrintDebug, // Added missing PrintDebug
        };
    }
}