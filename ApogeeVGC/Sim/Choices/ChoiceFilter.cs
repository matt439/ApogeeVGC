//using ApogeeVGC.Data;
//using ApogeeVGC.Player;
//using ApogeeVGC.Sim.Core;
//using ApogeeVGC.Sim.Moves;
//using ApogeeVGC.Sim.PokemonClasses;

//namespace ApogeeVGC.Sim.Choices;

//public enum ChoiceFilterStrategy
//{
//    /// <summary>No filtering; all choices are allowed.</summary>
//    None,
//    /// <summary>Reduces switching by allowing only one random switch choice.</summary>
//    ReducedSwitching,
//    /// <summary>Reduces switching and prefers super effective or STAB damaging moves.</summary>
//    ReducedSwitchingAndSuperEffectiveOrStabDamagingMoves,
//    /// <summary>Prefers super effective moves from switch-in and super effective or STAB damaging moves.</summary>
//    SuperEffectiveMovesFromSwitchInAndSuperEffectiveOrStabDamagingMoves,
//}

////public class ChoiceFilter
////{
////    // In singles battles, there are at most 5 possible switch choices (6 Pokemon - 1 active).
////    private const int MaxSwitchCount = 5;
////    private const int ReducedSwitchingMaxSwitchCount = 1;
////    // In battles, there are at most 4 possible move choices. Each move can be used with or without Tera.
////    private const int MaxMoveCount = 8;
////    private const int SuperEffectiveMaxMoveCount = 4;

////    public static BattleChoice FilterAndRandomlySelectChoice(BattleChoice[] choices, ChoiceFilterStrategy strategy,
////        Battle battle, PlayerId player, Random random)
////    {
////        if (choices.Length == 0)
////        {
////            throw new ArgumentException("Choices array cannot be empty.", nameof(choices));
////        }

////        // check if in team preview phase
////        if (battle.IsTeamPreview)
////        {
////            // In Team Preview phase, select a random choice from all available choices
////            int randomIndexPreview = random.Next(choices.Length);
////            return choices[randomIndexPreview];
////        }

////        var filteredChoices = Filter(choices, strategy, battle, player, random);
////        if (filteredChoices.Length == 0)
////        {
////            throw new InvalidOperationException("Filtering resulted in no available choices.");
////        }
////        int randomIndex = random.Next(filteredChoices.Length);
////        return filteredChoices[randomIndex];
////    }

////    public static BattleChoice[] Filter(BattleChoice[] choices, ChoiceFilterStrategy strategy,
////        Battle battle, PlayerId player, Random random)
////    {
////        if (choices.Length == 0)
////        {
////            return choices;
////        }

////        return strategy switch
////        {
////            ChoiceFilterStrategy.None => choices,
////            ChoiceFilterStrategy.ReducedSwitching =>
////                ApplyReducedSwitchingStrategy(choices, random),

////            ChoiceFilterStrategy.ReducedSwitchingAndSuperEffectiveOrStabDamagingMoves =>
////                ApplyReducedSwitchingAndSuperEffectiveOrStabDamagingMoves(choices, battle, player, random),

////            ChoiceFilterStrategy.SuperEffectiveMovesFromSwitchInAndSuperEffectiveOrStabDamagingMoves =>
////                ApplySuperEffectiveMovesFromSwitchInAndSuperEffectiveOrStabDamagingMoves(choices, battle,
////                    player, random),

////            _ => throw new InvalidOperationException("Invalid choice filter strategy"),
////        };
////    }

////    private static BattleChoice[] ApplyReducedSwitchingStrategy(BattleChoice[] choices, Random random)
////    {
////        // Filter for move choices
////        var moveChoices = FilterMoveChoices(choices);
////        // Filter for switch choices with a maximum of MaxSwitchCount random switches
////        var switchChoices = FilterSwitchChoices(choices, random, ReducedSwitchingMaxSwitchCount);

////        if (moveChoices.Length == 0 && switchChoices.Length == 0)
////        {
////            // No move or switch choices available
////            return choices;
////        }
////        if (moveChoices.Length == 0)
////        {
////            // No move choices available, return switch choices
////            return switchChoices;
////        }
////        if (switchChoices.Length == 0)
////        {
////            // No switch choices available, return move choices
////            return moveChoices;
////        }
////        // Combine move and switch choices
////        return moveChoices.Concat(switchChoices).ToArray();
////    }

////    private static BattleChoice[] ApplyReducedSwitchingAndSuperEffectiveOrStabDamagingMoves(BattleChoice[] choices,
////        Battle battle, PlayerId player, Random random)
////    {
////        var moveChoices = FilterSuperEffectiveOrStabDamagingMoves(choices, battle, player,
////            SuperEffectiveMaxMoveCount);
////        var switchChoices = FilterSwitchChoices(choices, random, ReducedSwitchingMaxSwitchCount);
////        return moveChoices.Length switch
////        {
////            0 when switchChoices.Length == 0 =>
////                // No move or switch choices available
////                choices,
////            0 =>
////                // No move choices available, return switch choices
////                switchChoices,

////            _ => switchChoices.Length == 0
////                ?
////                // No switch choices available, return move choices
////                moveChoices
////                :
////                // Combine move and switch choices
////                moveChoices.Concat(switchChoices).ToArray(),
////        };
////    }

////    private static BattleChoice[] ApplySuperEffectiveMovesFromSwitchInAndSuperEffectiveOrStabDamagingMoves(
////        BattleChoice[] choices, Battle battle, PlayerId player, Random random)
////    {
////        var moveChoices = FilterSuperEffectiveOrStabDamagingMoves(choices, battle, player,
////            SuperEffectiveMaxMoveCount);
////        var switchChoices = FilterSuperEffectiveMovesFromSwitchIn(choices, battle, player, random);
////        return moveChoices.Length switch
////        {
////            0 when switchChoices.Length == 0 => choices,
////            0 => switchChoices,
////            _ => switchChoices.Length == 0
////                ?
////                // No switch choices available, return move choices
////                moveChoices
////                :
////                // Combine move and switch choices
////                moveChoices.Concat(switchChoices).ToArray(),
////        };
////    }

////    private static BattleChoice[] FilterSuperEffectiveOrStabDamagingMoves(BattleChoice[] choices, Battle battle,
////        PlayerId player, int maxMoves = MaxMoveCount)
////    {
////        if (maxMoves < 0)
////        {
////            throw new ArgumentOutOfRangeException(nameof(maxMoves), "maxMoves must be at least 0.");
////        }

////        // Filter for damaging move choices
////        var moveChoices = FilterDamagingMoveChoices(choices);

////        if (moveChoices.Length == 0)
////        {
////            // No move choices available
////            return choices;
////        }

////        Side playerSide = battle.GetSide(player);
////        Side opponentSide = battle.GetSide(player.OpposingPlayerId());
////        Pokemon attackingPokemon = playerSide.Slot1;
////        Pokemon defendingPokemon = opponentSide.Slot1;

////        List<MoveEffectiveness> effectivenessList = [];
////        List<bool> isStabList = [];

////        foreach (BattleChoice choice in moveChoices)
////        {
////            if (choice is not SlotChoice.MoveChoice moveChoice)
////            {
////                continue; // Skip non-move choices
////            }

////            Move move = moveChoice.Move;

////            MoveEffectiveness effectiveness = battle.Library.TypeChart.GetMoveEffectiveness(
////                defendingPokemon.Specie.Types, move.Type);
////            effectivenessList.Add(effectiveness);

////            bool isStab = attackingPokemon.IsStab(move);
////            isStabList.Add(isStab);
////        }

////        // Order of preference:
////        // 1. Super effective 4x moves with STAB
////        // 2. Super effective 4x moves without STAB
////        // 3. Super effective 2x moves with STAB
////        // 4. Super effective 2x moves without STAB
////        // 5. Neutral moves with STAB
////        // 6. Neutral moves without STAB
////        var preferredChoices = moveChoices
////            .Zip(effectivenessList, (choice, effectiveness) => (choice, effectiveness))
////            .Zip(isStabList, (ce, isStab) => (ce.choice, ce.effectiveness, isStab))
////            .OrderByDescending(x => x.effectiveness.GetMultiplier())
////            .ThenByDescending(x => x.isStab)
////            .ToArray();

////        return preferredChoices
////            .Take(Math.Min(maxMoves, preferredChoices.Length))
////            .Select(x => x.choice)
////            .ToArray();
////    }

////    /// <summary>
////    /// Filters switch choices to only include those that would switch in a Pokemon
////    /// that has at least one super effective move against the opponent's active Pokemon.
////    /// </summary>
////    private static BattleChoice[] FilterSuperEffectiveMovesFromSwitchIn(BattleChoice[] choices, Battle battle,
////        PlayerId player, Random random)
////    {
////        // Get all switch choices first (without random filtering)
////        var allSwitchChoices = choices.Where(c => c is SlotChoice.SwitchChoice).ToArray();
////        if (allSwitchChoices.Length == 0)
////        {
////            return choices; // No switch choices available, return original choices
////        }

////        Side opponentSide = battle.GetSide(player.OpposingPlayerId());
////        Pokemon defendingPokemon = opponentSide.Slot1;

////        var superEffectiveSwitchChoices = allSwitchChoices.Where(choice =>
////        {
////            if (choice is not SlotChoice.SwitchChoice switchChoice)
////                return false;

////            Pokemon switchingInPokemon = switchChoice.SwitchInPokemon;

////            // Check if any move is super effective
////            return switchingInPokemon.Moves.Any(move =>
////            {
////                MoveEffectiveness effectiveness = battle.Library.TypeChart.GetMoveEffectiveness(
////                    defendingPokemon.Specie.Types, move.Type);
////                return effectiveness is MoveEffectiveness.SuperEffective4X or MoveEffectiveness.SuperEffective2X;
////            });
////        }).ToArray();

////        return superEffectiveSwitchChoices.Length switch
////        {
////            // If no super effective switches found, return original choices
////            0 => choices,
////            // Apply random selection if there are too many good switches
////            <= MaxSwitchCount => superEffectiveSwitchChoices,
////            _ => FilterSwitchChoices(superEffectiveSwitchChoices, random),
////        };
////    }

////    private static BattleChoice[] FilterMoveChoices(BattleChoice[] availableChoices)
////    {
////        return availableChoices.Where(choice => choice is SlotChoice.MoveChoice).ToArray();
////    }

////    private static BattleChoice[] FilterPhysicalMoveChoices(BattleChoice[] availableChoices)
////    {
////        return FilterMoveChoicesByCategory(availableChoices, MoveCategory.Physical);
////    }

////    private static BattleChoice[] FilterSpecialMoveChoices(BattleChoice[] availableChoices)
////    {
////        return FilterMoveChoicesByCategory(availableChoices, MoveCategory.Special);
////    }

////    private static BattleChoice[] FilterStatusMoveChoices(BattleChoice[] availableChoices)
////    {
////        return FilterMoveChoicesByCategory(availableChoices, MoveCategory.Status);
////    }

////    private static BattleChoice[] FilterDamagingMoveChoices(BattleChoice[] availableChoices)
////    {
////        var physicalMoves = FilterPhysicalMoveChoices(availableChoices);
////        var specialMoves = FilterSpecialMoveChoices(availableChoices);
////        return physicalMoves.Concat(specialMoves).ToArray();
////    }

////    private static BattleChoice[] FilterMoveChoicesByCategory(BattleChoice[] availableChoices, MoveCategory category)
////    {
////        var moveChoices = availableChoices.Where(choice => choice is SlotChoice.MoveChoice).ToArray();
////        if (moveChoices.Length == 0)
////        {
////            return [];
////        }

////        var categoryMoveChoices = moveChoices.Where(choice =>
////        {
////            if (choice is not SlotChoice.MoveChoice moveChoice)
////                return false;

////            Move move = moveChoice.Move;
////            return move.Category == category;
////        }).ToArray();
        
////        return categoryMoveChoices;
////    }

////    private static BattleChoice[] FilterSwitchChoices(BattleChoice[] availableChoices, Random random,
////        int maxSwitches = MaxSwitchCount)
////    {
////        switch (maxSwitches)
////        {
////            case < 0:
////                throw new ArgumentOutOfRangeException(nameof(maxSwitches), "maxSwitches must be at least 0.");
////            case 0:
////                return [];
////        }

////        var switchChoices = availableChoices.Where(c => c is SlotChoice.SwitchChoice).ToArray();
////        if (maxSwitches >= switchChoices.Length)
////            return switchChoices;

////        var selectedChoices = new HashSet<BattleChoice>();
////        while (selectedChoices.Count < maxSwitches)
////        {
////            int randomIndex = random.Next(switchChoices.Length);
////            selectedChoices.Add(switchChoices[randomIndex]);
////        }
////        return selectedChoices.ToArray();
////    }
////}