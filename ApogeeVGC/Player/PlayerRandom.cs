using ApogeeVGC.Sim;

namespace ApogeeVGC.Player;

public enum PlayerRandomStrategy
{
    AllChoices,
    MoveChoices,
}

public class PlayerRandom(PlayerId playerId, Battle battle, PlayerRandomStrategy strategy,
    int? seed = null) : IPlayer
{
    public PlayerId PlayerId { get; } = playerId;
    private Battle Battle { get; } = battle;
    private PlayerRandomStrategy Strategy {get; } = strategy;

    private readonly Random _random = seed is null ? new Random() : new Random(seed.Value);

    public Choice GetNextChoice(Choice[] availableChoices)
    {
        return Strategy switch
        {
            PlayerRandomStrategy.AllChoices => GetNextChoiceFromAll(availableChoices),
            PlayerRandomStrategy.MoveChoices => GetNextMoveChoice(availableChoices),
            _ => throw new ArgumentOutOfRangeException(nameof(Strategy), Strategy, null)
        };
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

    private Choice GetNextMoveChoice(Choice[] availableChoices)
    {
        // Filter for move choices
        var moveChoices = availableChoices.Where(c => c.IsMoveChoice()).ToArray();
        if (moveChoices.Length == 0)
        {
            // No move choices available. Select a random choice from all available choices
            return GetNextChoiceFromAll(availableChoices);
        }
        int randomIndex = _random.Next(moveChoices.Length);
        return moveChoices[randomIndex];
    }
}