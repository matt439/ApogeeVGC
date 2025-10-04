//using ApogeeVGC.Player;
//using ApogeeVGC.Sim.Choices;
//using ApogeeVGC.Sim.Core;
//using ApogeeVGC.Sim.PokemonClasses;
//using ApogeeVGC.Sim.Turns;

//namespace ApogeeVGC.Sim.BattleClasses;

//public partial class BattleAsync
//{
//    /// <summary>
//    /// Process team preview turn - both players select simultaneously
//    /// </summary>
//    public async Task ProcessTeamPreviewTurnAsync(TeamPreviewTurn turn, CancellationToken cancellationToken)
//    {
//        if (PrintDebug)
//            Console.WriteLine("Processing Team Preview turn...");

//        try
//        {
//            // Generate team preview choices for both players
//            var player1Choices = GetTeamPreviewChoices(Side1);
//            var player2Choices = GetTeamPreviewChoices(Side2);

//            if (PrintDebug)
//            {
//                Console.WriteLine($"Player 1 has {player1Choices.Length} team preview options");
//                Console.WriteLine($"Player 2 has {player2Choices.Length} team preview options");
//            }

//            // Request choices from both players simultaneously
//            TimeSpan teamPreviewTimeLimit = TimeSpan.FromSeconds(TeamPreviewLimitSeconds);
//            var player1Task = RequestChoiceFromPlayerAsync(PlayerId.Player1, player1Choices,
//                BattleRequestType.TeamPreview, teamPreviewTimeLimit, cancellationToken);

//            var player2Task = RequestChoiceFromPlayerAsync(PlayerId.Player2, player2Choices,
//                BattleRequestType.TeamPreview, teamPreviewTimeLimit, cancellationToken);

//            // Wait for both players to submit choices
//            var choices = await Task.WhenAll(player1Task, player2Task);

//            if (PrintDebug)
//            {
//                Console.WriteLine($"Player 1 selected: {choices[0]}");
//                Console.WriteLine($"Player 2 selected: {choices[1]}");
//            }

//            // Process the team preview choices
//            await ProcessTeamPreviewChoicesAsync(choices[0], choices[1]);
//        }
//        catch (TimeoutException ex)
//        {
//            if (PrintDebug)
//                Console.WriteLine($"Team preview timeout: {ex.Message}");
//            await HandleTeamPreviewTimeoutAsync();
//        }
//        catch (Exception ex)
//        {
//            if (PrintDebug)
//                Console.WriteLine($"Team preview error: {ex.Message}");
//            throw;
//        }
//    }

//    private static BattleChoice[] GetTeamPreviewChoices(Side side)
//    {
//        List<BattleChoice> choices = [];
//        int count = side.Team.PokemonSet.PokemonCount;
//        if (count != 6)
//        {
//            throw new InvalidOperationException("Team preview choices can only be generated" +
//                                                "for full teams of 6 Pokémon.");
//        }

//        // Get all 6 Pokémon from the team
//        var pokemon = side.Team.PokemonSet.Pokemons;

//        switch (side.GameType)
//        {
//            // Check battle format to determine how many Pokémon to select
//            case GameType.Singles:
//            {
//                // Singles: Generate all permutations of the 6 Pokémon (720 choices)
//                var permutations = GeneratePermutations(pokemon.ToArray());

//                choices.AddRange(permutations.Select(permutation =>
//                        permutation.Select((p, index) =>
//                            CreatePokemonWithSlot(p, (SlotId)(index + 1))).ToList()).
//                    Select(TeamPreviewChoice.CreateSinglesTeamPreview));
//                break;
//            }
//            case GameType.Doubles:
//            {
//                // Doubles: Select 4 out of 6 Pokémon and arrange them in slots 1-4
//                // Slots 5-6 are not used in doubles format
//                var combinations = GenerateCombinations(pokemon.ToArray(), 4);

//                foreach (var combination in combinations)
//                {
//                    // For each combination of 4 Pokémon, generate all permutations
//                    var permutations = GeneratePermutations(combination);

//                    foreach (var permutation in permutations)
//                    {
//                        // Create team arrangement with only the 4 selected Pokémon in slots 1-4
//                        var teamArrangement = new List<Pokemon>();

//                        // Add the 4 selected Pokémon in slots 1-4 only
//                        for (int i = 0; i < 4; i++)
//                        {
//                            teamArrangement.Add(CreatePokemonWithSlot(permutation[i], (SlotId)(i + 1)));
//                        }

//                        // Note: Slots 5-6 are deliberately not populated for doubles format
//                        choices.Add(TeamPreviewChoice.CreateDoublesTeamPreview(teamArrangement));
//                    }
//                }
//                break;
//            }
//            default:
//                throw new InvalidOperationException($"Unsupported battle format: {side.GameType}");
//        }

//        return choices.ToArray();
//    }

//    /// <summary>
//    /// Generates all combinations of r elements from the given array.
//    /// For Doubles: generates all ways to choose 4 Pokémon out of 6 (15 combinations).
//    /// </summary>
//    private static IEnumerable<T[]> GenerateCombinations<T>(T[] array, int r)
//    {
//        if (r > array.Length || r < 0)
//            yield break;

//        if (r == 0)
//        {
//            yield return [];
//            yield break;
//        }

//        if (r == array.Length)
//        {
//            yield return (T[])array.Clone();
//            yield break;
//        }

//        // Generate combinations using bit manipulation
//        int n = array.Length;
//        int totalCombinations = 1 << n; // 2^n

//        for (int mask = 0; mask < totalCombinations; mask++)
//        {
//            // Count set bits to see if this mask represents r elements
//            if (CountSetBits(mask) != r) continue;

//            var combination = new T[r];
//            int index = 0;

//            for (int i = 0; i < n; i++)
//            {
//                if ((mask & (1 << i)) != 0)
//                {
//                    combination[index++] = array[i];
//                }
//            }

//            yield return combination;
//        }
//    }

//    /// <summary>
//    /// Helper method to count set bits in an integer
//    /// </summary>
//    private static int CountSetBits(int n)
//    {
//        int count = 0;
//        while (n != 0)
//        {
//            count++;
//            n &= (n - 1); // Clear the lowest set bit
//        }
//        return count;
//    }

//    /// <summary>
//    /// Generates all permutations of the given array using Heap's algorithm.
//    /// For 6 Pokémon, this generates 6! = 720 permutations.
//    /// </summary>
//    private static IEnumerable<T[]> GeneratePermutations<T>(T[] array)
//    {
//        int n = array.Length;
//        int[] c = new int[n];
//        var result = new T[n];
//        Array.Copy(array, result, n);

//        yield return (T[])result.Clone();

//        int i = 0;
//        while (i < n)
//        {
//            if (c[i] < i)
//            {
//                if (i % 2 == 0)
//                {
//                    // Swap first and i-th element
//                    (result[0], result[i]) = (result[i], result[0]);
//                }
//                else
//                {
//                    // Swap c[i]-th and i-th element
//                    (result[c[i]], result[i]) = (result[i], result[c[i]]);
//                }

//                yield return (T[])result.Clone();
//                c[i]++;
//                i = 0;
//            }
//            else
//            {
//                c[i] = 0;
//                i++;
//            }
//        }
//    }

//    /// <summary>
//    /// Creates a copy of a Pokémon with a new SlotId.
//    /// This is needed because team preview determines the final slot ordering.
//    /// </summary>
//    private static Pokemon CreatePokemonWithSlot(Pokemon original, SlotId newSlotId)
//    {
//        Pokemon copy = original.Copy();
//        copy.SlotId = newSlotId;
//        return copy;
//    }

//    /// <summary>
//    /// Process team preview selections and advance to gameplay
//    /// </summary>
//    private async Task ProcessTeamPreviewChoicesAsync(BattleChoice player1Choice, BattleChoice player2Choice)
//    {
//        if (PrintDebug)
//            Console.WriteLine("Applying team preview choices...");

//        // Apply team preview choices to current battle state
//        ApplyTeamPreviewChoice(PlayerId.Player1, player1Choice);
//        ApplyTeamPreviewChoice(PlayerId.Player2, player2Choice);

//        // Complete the current team preview turn
//        CompleteTurnWithEndStates();

//        // Create the first gameplay turn
//        await CreateNextTurnAsync();

//        if (PrintDebug)
//            Console.WriteLine("Team preview completed, advancing to gameplay");

//        await Task.CompletedTask;
//    }

//    /// <summary>
//    /// Apply a single team preview choice to the battle state
//    /// </summary>
//    private void ApplyTeamPreviewChoice(PlayerId playerId, BattleChoice choice)
//    {
//        if (choice is not TeamPreviewChoice tpChoice)
//            throw new InvalidOperationException($"Expected TeamPreviewChoice, got {choice.GetType().Name}");

//        Side side = GetSide(playerId);

//        if (PrintDebug)
//            Console.WriteLine($"Applying team preview choice for {playerId}");

//        side.SetSlotsWithCopies(tpChoice.Pokemon);

//        if (PrintDebug)
//            Console.WriteLine($"Team preview choice applied for {playerId}");
//    }

//    /// <summary>
//    /// Handle team preview timeout - use default choices for players who didn't respond
//    /// </summary>
//    private async Task HandleTeamPreviewTimeoutAsync()
//    {
//        if (PrintDebug)
//            Console.WriteLine("Handling team preview timeout, using default choices");

//        // Use default team preview choices for both players
//        BattleChoice defaultPlayer1Choice = GetDefaultTeamPreviewChoice(Side1);
//        BattleChoice defaultPlayer2Choice = GetDefaultTeamPreviewChoice(Side2);

//        await ProcessTeamPreviewChoicesAsync(defaultPlayer1Choice, defaultPlayer2Choice);
//    }

//    /// <summary>
//    /// Get default team preview choice for a player (when they timeout)
//    /// </summary>
//    private BattleChoice GetDefaultTeamPreviewChoice(Side side)
//    {
//        var choices = GetTeamPreviewChoices(side);
        
//        // Return the first available choice (index 0 behavior as specified)
//        if (choices.Length > 0)
//            return choices[0];

//        throw new InvalidOperationException($"No team preview choices available for {side.PlayerId}");
//    }
//}