using ApogeeVGC.Sim;

namespace ApogeeVGC.Player;

public enum PlayerRandomStrategy
{
    AllChoices,
    MoveChoices,
    ReducedSwitching,
    SuperEffectiveOrStabMoves,
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
            PlayerRandomStrategy.ReducedSwitching => GetNextChoiceReducedSwitching(availableChoices),
            PlayerRandomStrategy.SuperEffectiveOrStabMoves =>
                GetNextChoiceSuperEffectiveStab(availableChoices),
            _ => throw new InvalidOperationException("Invalid player random strategy"),
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
        // check if in team preview phase
        if (Battle.IsTeamPreview)
        {
            // In Team Preview phase, select a random choice from all available choices
            return GetNextChoiceFromAll(availableChoices);
        }

        // Filter for move choices
        var moveChoices = FilterMoveChoices(availableChoices);
        if (moveChoices.Length == 0)
        {
            // No move choices available. Select a random choice from all available choices
            return GetNextChoiceFromAll(availableChoices);
        }
        int randomIndex = _random.Next(moveChoices.Length);
        return moveChoices[randomIndex];
    }

    private Choice GetNextChoiceReducedSwitching(Choice[] availableChoices)
    {
        // check if in team preview phase
        if (Battle.IsTeamPreview)
        {
            // In Team Preview phase, select a random choice from all available choices
            return GetNextChoiceFromAll(availableChoices);
        }

        // Filter for move choices
        var moveChoices = FilterMoveChoices(availableChoices);
        // Filter for switch choices
        var switchChoices = FilterSwitchChoices(availableChoices);

    }

    private Choice GetNextChoiceSuperEffectiveStab(Choice[] availableChoices)
    {
        // check if in team preview phase
        if (Battle.IsTeamPreview)
        {
            // In Team Preview phase, select a random choice from all available choices
            return GetNextChoiceFromAll(availableChoices);
        }
    }

    private static Choice[] FilterMoveChoices(Choice[] availableChoices)
    {
        var moveChoices = availableChoices.Where(c => c.IsMoveChoice()).ToArray();
        return moveChoices;
    }

    private static Choice[] FilterSwitchChoices(Choice[] availableChoices)
    {
        var switchChoices = availableChoices.Where(c => c.IsSwitchChoice()).ToArray();
        return switchChoices;
    }
}