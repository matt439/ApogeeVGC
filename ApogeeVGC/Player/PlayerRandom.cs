using ApogeeVGC.Data;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Player;

public class PlayerRandom(PlayerId playerId, Battle battle, Library library,
    ChoiceFilterStrategy strategy, int? seed = null) : IPlayer
{
    public PlayerId PlayerId { get; } = playerId;
    private Battle Battle { get; } = battle;
    private Library Library { get; } = library;
    private ChoiceFilterStrategy Strategy { get; } = strategy;

    private readonly Random _random = seed is null ? new Random() : new Random(seed.Value);

    public BattleChoice GetNextChoice(BattleChoice[] availableChoices)
    {
        if (availableChoices.Length == 0)
        {
            throw new ArgumentException("No available choices to select from.", nameof(availableChoices));
        }

        BattleChoice selectedChoice = Strategy switch
        {
            ChoiceFilterStrategy.None => GetNextChoiceFromAll(availableChoices),
            ChoiceFilterStrategy.ReducedSwitching
                or ChoiceFilterStrategy.ReducedSwitchingAndSuperEffectiveOrStabDamagingMoves
                or ChoiceFilterStrategy.SuperEffectiveMovesFromSwitchInAndSuperEffectiveOrStabDamagingMoves =>
                ChoiceFilter.FilterAndRandomlySelectChoice(availableChoices, Strategy, Battle, PlayerId, _random),
            _ => throw new NotImplementedException($"Strategy {Strategy} not implemented."),
        };

        return selectedChoice;
    }

    private BattleChoice GetNextChoiceFromAll(BattleChoice[] availableChoices)
    {
        // Select a random choice from all available choices
        if (availableChoices.Length == 0)
        {
            throw new ArgumentException("No available choices to select from.", nameof(availableChoices));
        }
        int randomIndex = _random.Next(availableChoices.Length);
        return availableChoices[randomIndex];
    }
}