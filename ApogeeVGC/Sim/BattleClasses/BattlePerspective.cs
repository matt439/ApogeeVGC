using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.BattleClasses;

/// <summary>
/// Represents the perspective of a battle from one player's point of view
/// Some of the opponent's information may be hidden (e.g., moves, items)
/// </summary>
public record BattlePerspective
{
    public required Side PlayerSide { get; init; }
    public required Side OpponentSide { get; init; }
    public required Field Field { get; init; }
    public required int TurnCounter { get; init; }

    // Reference to the full battle (not copied). Needed by MCTS to simulate future turns.
    public required BattleNew Battle { get; init; } 

    /// <summary>
    /// Creates a battle perspective with deep copies to prevent external modification
    /// </summary>
    public static BattlePerspective CreateSafe(Side playerSide, Side opponentSide, Field field,
        int turnCounter, BattleNew battle)
    {
        return new BattlePerspective
        {
            PlayerSide = playerSide.Copy(),
            OpponentSide = opponentSide.Copy(),
            Field = field.Copy(),
            TurnCounter = turnCounter,
            Battle = battle,
        };
    }

    ///// <summary>
    ///// Creates a battle perspective with direct references (use with caution)
    ///// </summary>
    //public static BattlePerspective CreateUnsafe(Side playerSide, Side opponentSide, Field field,
    //    int turnCounter)
    //{
    //    return new BattlePerspective
    //    {
    //        PlayerSide = playerSide,
    //        OpponentSide = opponentSide,
    //        Field = field,
    //        TurnCounter = turnCounter,
    //    };
    //}
}