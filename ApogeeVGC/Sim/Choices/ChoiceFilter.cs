using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Extensions;
using Battle = ApogeeVGC.Sim.Core.Battle;

namespace ApogeeVGC.Sim.Choices;

public enum ChoiceFilterStrategy
{
    /// <summary>No filtering; all choices are allowed.</summary>
    None,
    /// <summary>Reduces switching by allowing only one random switch choice.</summary>
    ReducedSwitching,
    /// <summary>Reduces switching and prefers super effective or STAB damaging moves.</summary>
    ReducedSwitchingAndSuperEffectiveOrStabDamagingMoves,
    /// <summary>Prefers super effective moves from switch-in and super effective or STAB damaging moves.</summary>
    SuperEffectiveMovesFromSwitchInAndSuperEffectiveOrStabDamagingMoves,
}

//public class ChoiceFilter
//{
//    // In singles battles, there are at most 5 possible switch choices (6 Pokemon - 1 active).
//    private const int MaxSwitchCount = 5;
//    private const int ReducedSwitchingMaxSwitchCount = 1;
//    // In battles, there are at most 4 possible move choices. Each move can be used with or without Tera.
//    private const int MaxMoveCount = 8;
//    private const int SuperEffectiveMaxMoveCount = 4;

//    public static Choice FilterAndRandomlySelectChoice(Choice[] choices, ChoiceFilterStrategy strategy,
//        Battle battle, PlayerId player, Random random)
//    {
//        if (choices.Length == 0)
//        {
//            return Choice.Invalid; // No choices available
//        }

//        // check if in team preview phase
//        if (battle.IsTeamPreview)
//        {
//            // In Team Preview phase, select a random choice from all available choices
//            int randomIndexPreview = random.Next(choices.Length);
//            return choices[randomIndexPreview];
//        }

//        var filteredChoices = Filter(choices, strategy, battle, player, random);
//        if (filteredChoices.Length == 0)
//        {
//            return Choice.Invalid; // No choices available
//        }
//        int randomIndex = random.Next(filteredChoices.Length);
//        return filteredChoices[randomIndex];
//    }

//    public static Choice[] Filter(Choice[] choices, ChoiceFilterStrategy strategy,
//        Battle battle, PlayerId player, Random random)
//    {
//        if (choices.Length == 0)
//        {
//            return choices;
//        }

//        return strategy switch
//        {
//            ChoiceFilterStrategy.None => choices,
//            ChoiceFilterStrategy.ReducedSwitching =>
//                ApplyReducedSwitchingStrategy(choices, random),

//            ChoiceFilterStrategy.ReducedSwitchingAndSuperEffectiveOrStabDamagingMoves =>
//                ApplyReducedSwitchingAndSuperEffectiveOrStabDamagingMoves(choices, battle, player, random),

//            ChoiceFilterStrategy.SuperEffectiveMovesFromSwitchInAndSuperEffectiveOrStabDamagingMoves =>
//                ApplySuperEffectiveMovesFromSwitchInAndSuperEffectiveOrStabDamagingMoves(choices, battle,
//                    player, random),

//            _ => throw new InvalidOperationException("Invalid choice filter strategy"),
//        };
//    }

//    private static Choice[] ApplyReducedSwitchingStrategy(Choice[] choices, Random random)
//    {
//        // Filter for move choices
//        var moveChoices = FilterMoveChoices(choices);
//        // Filter for switch choices with a maximum of MaxSwitchCount random switches
//        var switchChoices = FilterSwitchChoices(choices, random, ReducedSwitchingMaxSwitchCount);

//        if (moveChoices.Length == 0 && switchChoices.Length == 0)
//        {
//            // No move or switch choices available
//            return choices;
//        }
//        if (moveChoices.Length == 0)
//        {
//            // No move choices available, return switch choices
//            return switchChoices;
//        }
//        if (switchChoices.Length == 0)
//        {
//            // No switch choices available, return move choices
//            return moveChoices;
//        }
//        // Combine move and switch choices
//        return moveChoices.Concat(switchChoices).ToArray();
//    }

//    private static Choice[] ApplyReducedSwitchingAndSuperEffectiveOrStabDamagingMoves(Choice[] choices,
//        Battle battle, PlayerId player, Random random)
//    {
//        var moveChoices = FilterSuperEffectiveOrStabDamagingMoves(choices, battle, player,
//            SuperEffectiveMaxMoveCount);
//        var switchChoices = FilterSwitchChoices(choices, random, ReducedSwitchingMaxSwitchCount);
//        if (moveChoices.Length == 0 && switchChoices.Length == 0)
//        {
//            // No move or switch choices available
//            return choices;
//        }
//        if (moveChoices.Length == 0)
//        {
//            // No move choices available, return switch choices
//            return switchChoices;
//        }
//        if (switchChoices.Length == 0)
//        {
//            // No switch choices available, return move choices
//            return moveChoices;
//        }
//        // Combine move and switch choices
//        return moveChoices.Concat(switchChoices).ToArray();
//    }

//    private static Choice[] ApplySuperEffectiveMovesFromSwitchInAndSuperEffectiveOrStabDamagingMoves(
//        Choice[] choices, Battle battle, PlayerId player, Random random)
//    {
//        var moveChoices = FilterSuperEffectiveOrStabDamagingMoves(choices, battle, player,
//            SuperEffectiveMaxMoveCount);
//        var switchChoices = FilterSuperEffectiveMovesFromSwitchIn(choices, battle, player, random);
//        return moveChoices.Length switch
//        {
//            0 when switchChoices.Length == 0 => choices,
//            0 => switchChoices,
//            _ => switchChoices.Length == 0
//                ?
//                // No switch choices available, return move choices
//                moveChoices
//                :
//                // Combine move and switch choices
//                moveChoices.Concat(switchChoices).ToArray(),
//        };
//    }

//    private static Choice[] FilterSuperEffectiveOrStabDamagingMoves(Choice[] choices, Battle battle,
//        PlayerId player, int maxMoves = MaxMoveCount)
//    {
//        if (maxMoves < 0)
//        {
//            throw new ArgumentOutOfRangeException(nameof(maxMoves), "maxMoves must be at least 0.");
//        }

//        // Filter for damaging move choices
//        var moveChoices = FilterDamagingMoveChoices(choices, battle, player);

//        if (moveChoices.Length == 0)
//        {
//            // No move choices available
//            return choices;
//        }

//        Side playerSide = battle.GetSide(player);
//        Side opponentSide = battle.GetSide(player.OpposingPlayerId());
//        Pokemon attackingPokemon = playerSide.Team.ActivePokemon;
//        Pokemon defendingPokemon = opponentSide.Team.ActivePokemon;

//        List<MoveEffectiveness> effectivenessList = [];
//        List<bool> isStabList = [];

//        foreach (Choice choice in moveChoices)
//        {
//            if (!choice.IsMoveChoice() && !choice.IsMoveWithTeraChoice())
//            {
//                throw new InvalidOperationException("Choice must be a move choice.");
//            }

//            int moveIndex;
//            if (choice.IsMoveChoice())
//            {
//                moveIndex = choice.GetMoveIndexFromChoice();

//            }
//            else
//            {
//                moveIndex = choice.GetMoveWithTeraIndexFromChoice();
//            }

//            if (moveIndex < 0 || moveIndex >= playerSide.Team.ActivePokemon.Moves.Length)
//            {
//                throw new ArgumentOutOfRangeException(nameof(choice), "Invalid move choice.");
//            }
//            Move move = playerSide.Team.ActivePokemon.Moves[moveIndex];
//            if (move == null)
//            {
//                throw new InvalidOperationException("Move choice cannot be made to a null Move.");
//            }

//            MoveEffectiveness effectiveness = battle.Library.TypeChart.GetMoveEffectiveness(
//                defendingPokemon.Specie.Types, move.Type);
//            effectivenessList.Add(effectiveness);

//            bool isStab = attackingPokemon.IsStab(move);
//            isStabList.Add(isStab);
//        }

//        // Order of preference:
//        // 1. Super effective 4x moves with STAB
//        // 2. Super effective 4x moves without STAB
//        // 3. Super effective 2x moves with STAB
//        // 4. Super effective 2x moves without STAB
//        // 5. Neutral moves with STAB
//        // 6. Neutral moves without STAB
//        var preferredChoices = moveChoices
//            .Zip(effectivenessList, (choice, effectiveness) => (choice, effectiveness))
//            .Zip(isStabList, (ce, isStab) => (ce.choice, ce.effectiveness, isStab))
//            .OrderByDescending(x => x.effectiveness.GetMultiplier())
//            .ThenByDescending(x => x.isStab)
//            .ToArray();

//        return preferredChoices
//            .Take(Math.Min(maxMoves, preferredChoices.Length))
//            .Select(x => x.choice)
//            .ToArray();
//    }

//    /// <summary>
//    /// Filters switch choices to only include those that would switch in a Pokemon
//    /// that has at least one super effective move against the opponent's active Pokemon.
//    /// </summary>
//    private static Choice[] FilterSuperEffectiveMovesFromSwitchIn(Choice[] choices, Battle battle,
//        PlayerId player, Random random)
//    {
//        // Get all switch choices first (without random filtering)
//        var allSwitchChoices = choices.Where(c => c.IsSwitchChoice()).ToArray();
//        if (allSwitchChoices.Length == 0)
//        {
//            return choices; // No switch choices available, return original choices
//        }

//        Side playerSide = battle.GetSide(player);
//        Side opponentSide = battle.GetSide(player.OpposingPlayerId());
//        Pokemon defendingPokemon = opponentSide.Team.ActivePokemon;

//        var superEffectiveSwitchChoices = allSwitchChoices.Where(choice =>
//        {
//            int switchIndex = choice.GetSwitchIndexFromChoice();
//            if (switchIndex < 0 || switchIndex >= playerSide.Team.PokemonSet.Pokemons.Length)
//            {
//                return false; // Invalid choice, exclude it
//            }

//            Pokemon switchingInPokemon = playerSide.Team.PokemonSet.Pokemons[switchIndex];

//            // Check if any move is super effective
//            return switchingInPokemon.Moves.Any(move =>
//            {
//                MoveEffectiveness effectiveness = battle.Library.TypeChart.GetMoveEffectiveness(
//                    defendingPokemon.Specie.Types, move.Type);
//                return effectiveness is MoveEffectiveness.SuperEffective4X or MoveEffectiveness.SuperEffective2X;
//            });
//        }).ToArray();

//        return superEffectiveSwitchChoices.Length switch
//        {
//            // If no super effective switches found, return original choices
//            0 => choices,
//            // Apply random selection if there are too many good switches
//            <= MaxSwitchCount => superEffectiveSwitchChoices,
//            _ => FilterSwitchChoices(superEffectiveSwitchChoices, random),
//        };
//    }

//    private static Choice[] FilterMoveChoices(Choice[] availableChoices)
//    {
//        var moveChoices = availableChoices.Where(c => c.IsMoveChoice() ||
//                                                      c.IsMoveWithTeraChoice()).ToArray();
//        return moveChoices;
//    }

//    private static Choice[] FilterPhysicalMoveChoices(Choice[] availableChoices, Battle battle,
//        PlayerId player)
//    {
//        return FilterMoveChoicesByCategory(availableChoices, battle, player, MoveCategory.Physical);
//    }

//    private static Choice[] FilterSpecialMoveChoices(Choice[] availableChoices, Battle battle,
//        PlayerId player)
//    {
//        return FilterMoveChoicesByCategory(availableChoices, battle, player, MoveCategory.Special);
//    }

//    private static Choice[] FilterStatusMoveChoices(Choice[] availableChoices, Battle battle,
//        PlayerId player)
//    {
//        return FilterMoveChoicesByCategory(availableChoices, battle, player, MoveCategory.Status);
//    }

//    private static Choice[] FilterDamagingMoveChoices(Choice[] availableChoices, Battle battle,
//        PlayerId player)
//    {
//        var physicalMoves = FilterPhysicalMoveChoices(availableChoices, battle, player);
//        var specialMoves = FilterSpecialMoveChoices(availableChoices, battle, player);
//        return physicalMoves.Concat(specialMoves).ToArray();
//    }

//    private static Choice[] FilterMoveChoicesByCategory(Choice[] availableChoices, Battle battle,
//        PlayerId player, MoveCategory category)
//    {
//        var moveChoices = FilterMoveChoices(availableChoices);
//        if (moveChoices.Length == 0)
//        {
//            return [];
//        }
//        Side playerSide = battle.GetSide(player);
//        Pokemon attackingPokemon = playerSide.Team.ActivePokemon;
//        var statusMoveChoices = moveChoices.Where(choice =>
//        {
//            if (!choice.IsMoveChoice() && !choice.IsMoveWithTeraChoice())
//            {
//                throw new InvalidOperationException("Choice must be a move choice.");
//            }

//            int moveIndex;
//            if (choice.IsMoveChoice())
//            {
//                moveIndex = choice.GetMoveIndexFromChoice();
//            }
//            else
//            {
//                moveIndex = choice.GetMoveWithTeraIndexFromChoice();
//            }

//            if (moveIndex < 0 || moveIndex >= attackingPokemon.Moves.Length)
//            {
//                throw new ArgumentOutOfRangeException(nameof(choice), "Invalid move choice.");
//            }
//            Move move = attackingPokemon.Moves[moveIndex];
//            if (move == null)
//            {
//                throw new InvalidOperationException("Move choice cannot be made to a null Move.");
//            }
//            return move.Category == category;
//        }).ToArray();
//        return statusMoveChoices;
//    }

//    private static Choice[] FilterSwitchChoices(Choice[] availableChoices, Random random,
//        int maxSwitches = MaxSwitchCount)
//    {
//        switch (maxSwitches)
//        {
//            case < 0:
//                throw new ArgumentOutOfRangeException(nameof(maxSwitches), "maxSwitches must be at least 0.");
//            case 0:
//                return [];
//        }

//        var switchChoices = availableChoices.Where(c => c.IsSwitchChoice()).ToArray();
//        if (maxSwitches >= switchChoices.Length)
//            return switchChoices;

//        var selectedChoices = new HashSet<Choice>();
//        while (selectedChoices.Count < maxSwitches)
//        {
//            int randomIndex = random.Next(switchChoices.Length);
//            selectedChoices.Add(switchChoices[randomIndex]);
//        }
//        return selectedChoices.ToArray();
//    }
//}