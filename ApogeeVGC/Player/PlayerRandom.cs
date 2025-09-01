using System.Runtime.CompilerServices;
using ApogeeVGC.Data;
using ApogeeVGC.Sim;

namespace ApogeeVGC.Player;

public enum PlayerRandomStrategy
{
    AllChoices,
    MoveChoices,
    ReducedSwitching,
    SuperEffectiveOrStabMoves,
}

public class PlayerRandom(PlayerId playerId, Battle battle, Library library,
    PlayerRandomStrategy strategy, int? seed = null) : IPlayer
{
    public PlayerId PlayerId { get; } = playerId;
    private Battle Battle { get; } = battle;
    private Library Library { get; } = library;
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
        return GetNextChoiceFromAll(moveChoices.Length == 0 ?
            availableChoices : moveChoices);
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
        Choice? switchChoice = null;
        if (switchChoices.Length > 1)
        {
            // select a single random switch choice
            int randomIndex = _random.Next(switchChoices.Length);
            switchChoice = switchChoices[randomIndex];
        }

        var possibleChoices = moveChoices;
        if (switchChoice is not null)
        {
            possibleChoices = possibleChoices.Append(switchChoice.Value).ToArray();
        }

        return GetNextChoiceFromAll(possibleChoices.Length == 0 ?
            availableChoices : possibleChoices);
    }

    private Choice GetNextChoiceSuperEffectiveStab(Choice[] availableChoices)
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
            // No move choices available, select from all available choices
            return GetNextChoiceFromAll(availableChoices);
        }

        Side playerSide = Battle.GetSide(PlayerId);
        Side opponentSide = Battle.GetSide(PlayerId.OpposingPlayerId());
        Pokemon attackingPokemon = playerSide.Team.ActivePokemon;
        Pokemon defendingPokemon = opponentSide.Team.ActivePokemon;

        List<MoveEffectiveness> effectivenessList = [];
        List<bool> isStabList = [];

        foreach (Choice choice in moveChoices)
        {
            int moveIndex = choice.GetMoveIndexFromChoice();
            if (moveIndex < 0 || moveIndex >= playerSide.Team.ActivePokemon.Moves.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(choice), "Invalid move choice.");
            }
            Move move = playerSide.Team.ActivePokemon.Moves[moveIndex];
            if (move == null)
            {
                throw new InvalidOperationException("Move choice cannot be made to a null Move.");
            }

            MoveEffectiveness effectiveness = Library.TypeChart.GetMoveEffectiveness(
                defendingPokemon.Specie.Types, move.Type);
            effectivenessList.Add(effectiveness);

            bool isStab = attackingPokemon.IsStab(move);
            isStabList.Add(isStab);
        }

        // Order of preference:
        // 1. Super effective 4x moves with STAB
        // 2. Super effective 4x moves without STAB
        // 3. Super effective 2x moves with STAB
        // 4. Super effective 2x moves without STAB
        // 5. Neutral moves with STAB
        // 6. Neutral moves without STAB
        var preferredChoices = moveChoices
            .Zip(effectivenessList, (choice, effectiveness) =>
                (choice, effectiveness))
            .Zip(isStabList, (ce, isStab) =>
                (ce.choice, ce.effectiveness, isStab))
            .OrderByDescending(x =>
                x.effectiveness.GetMultiplier())
            .ThenByDescending(x =>
                x.isStab)
            .ToArray();

        MoveEffectiveness topEffectiveness = preferredChoices[0].effectiveness;
        var topChoices = preferredChoices.Where(x =>
                x.effectiveness == topEffectiveness)
            .Select(x => x.choice).ToArray();

        return GetNextChoiceFromAll(topChoices);
    }

    private static Choice[] FilterMoveChoices(Choice[] availableChoices)
    {
        var moveChoices = availableChoices.Where(c => c.IsMoveChoice() || c.IsMoveWithTeraChoice()).ToArray();
        return moveChoices;
    }

    private static Choice[] FilterSwitchChoices(Choice[] availableChoices)
    {
        var switchChoices = availableChoices.Where(c => c.IsSwitchChoice()).ToArray();
        return switchChoices;
    }
}