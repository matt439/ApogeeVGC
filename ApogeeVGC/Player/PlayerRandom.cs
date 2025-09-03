using System.Runtime.CompilerServices;
using ApogeeVGC.Data;
using ApogeeVGC.Sim;
using ApogeeVGC.Sim.Choices;
using Battle = ApogeeVGC.Sim.Core.Battle;

namespace ApogeeVGC.Player;

//public enum PlayerRandomStrategy
//{
//    AllChoices,
//    MoveChoices,
//    ReducedSwitching,
//    SuperEffectiveOrStabMoves,
//}

public class PlayerRandom(PlayerId playerId, Battle battle, Library library,
    ChoiceFilterStrategy strategy, int? seed = null) : IPlayer
{
    public PlayerId PlayerId { get; } = playerId;
    private Battle Battle { get; } = battle;
    private Library Library { get; } = library;
    private ChoiceFilterStrategy Strategy { get; } = strategy;

    private readonly Random _random = seed is null ? new Random() : new Random(seed.Value);

    public Choice GetNextChoice(Choice[] availableChoices)
    {
        Choice selectedChoice = Strategy switch
        {
            ChoiceFilterStrategy.None => GetNextChoiceFromAll(availableChoices),
            ChoiceFilterStrategy.ReducedSwitching
                or ChoiceFilterStrategy.ReducedSwitchingAndSuperEffectiveOrStabDamagingMoves
                or ChoiceFilterStrategy.SuperEffectiveMovesFromSwitchInAndSuperEffectiveOrStabDamagingMoves =>
                ChoiceFilter.FilterAndRandomlySelectChoice(availableChoices, Strategy, Battle, PlayerId, _random),
            _ => throw new NotImplementedException($"Strategy {Strategy} not implemented."),
        };

        if (selectedChoice == Choice.Invalid)
        {
            throw new InvalidOperationException("Filtered choice resulted in Invalid choice." +
                                                "This should not happen.");
        }
        return selectedChoice;
    }

    private Choice GetNextChoiceFromAll(Choice[] availableChoices)
    {
        // Select a random choice from all available choices
        if (availableChoices.Length == 0)
        {
            return Choice.Invalid; // No choices available
        }
        int randomIndex = _random.Next(availableChoices.Length);
        return availableChoices[randomIndex];
    }
}