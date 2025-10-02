using System.Text;
using System.Linq;
using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Turns;

namespace ApogeeVGC.Mcts;

public class PokemonMcts(
    int maxIterations,
    double explorationParameter,
    PlayerId mctsPlayerId,
    Library library,
    int? seed = null,
    int? maxDegreeOfParallelism = null,
    int? maxTimer = null)
{
    private readonly Random _random = seed.HasValue ? new Random(seed.Value) : new Random();
    private readonly int _maxDegreeOfParallelism = maxDegreeOfParallelism ?? Environment.ProcessorCount;
    
    private int _battleSeedCounter;

    public struct MoveResult
    {
        public BattleChoice OptimalChoice;
        public int OptimalChoiceNodeIndex = -1;
        public string Children = string.Empty;

        public MoveResult(BattleChoice optimalChoice, int optimalChoiceNodeIndex, string children)
        {
            OptimalChoice = optimalChoice;
            OptimalChoiceNodeIndex = optimalChoiceNodeIndex;
            Children = children;
        }

        public override string ToString()
        {
            string result = string.Empty;
            result += Children;
            result += $"Optimal Choice: {OptimalChoice}, Optimal Choice Node Index: {OptimalChoiceNodeIndex}";
            return result;
        }
    }

    public MoveResult FindBestChoice(IBattle battle, BattleChoice[] availableChoices)
    {
        // Create a seeded copy of the battle for deterministic simulation
        IBattle battleCopy = CopyBattle(battle);

        // Ensure deterministic simulation
        battleCopy.BattleSeed ??= GenerateUniqueBattleSeed();

        var rootNode = new Node(battleCopy, null, null!, explorationParameter, mctsPlayerId, library);
        GenerateChildren(rootNode, availableChoices);

        // Ensure root node has children before starting MCTS
        if (rootNode.ChildNodes.Count == 0)
        {
            Console.WriteLine($"Warning: No children generated for root node. Available choices: {availableChoices.Length}");
            return new MoveResult(
                availableChoices.Length > 0 ? availableChoices[0] : throw new InvalidOperationException(),
                -1,
                "No children generated for root node"
            );
        }

        // Timer setup for time limits (if specified)
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        bool timedOut = false;

        int completedIterations = 0;
        
        // Run MCTS iterations sequentially instead of in parallel
        for (int i = 0; i < maxIterations; i++)
        {
            // Check for timeout if maxTimer is specified
            if (maxTimer.HasValue && stopwatch.ElapsedMilliseconds > maxTimer.Value)
            {
                timedOut = true;
                break;
            }

            // Use a consistent random generator for deterministic behavior
            var iterationRandom = new Random(_random.Next() + i);
            
            try
            {
                // Selection phase - find the best leaf node to expand
                Node node = Selection(rootNode);

                // Expansion phase - add new child nodes if possible
                Node expandedNode = Expansion(node, iterationRandom);

                // Simulation phase - run random playout from expanded node
                IBattle simulationBattle = CopyBattle(expandedNode.Battle);
                SimulatorResult simulationResult = Simulation(simulationBattle, iterationRandom);

                // Backpropagation phase - update statistics up the tree
                BackPropagate(expandedNode, simulationResult);
                
                completedIterations++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MCTS iteration {i}: {ex.Message}");
                // Continue with next iteration rather than failing completely
                continue;
            }
        }

        // Calculate optimal choice (always runs whether completed naturally or timed out)
        var result = new MoveResult();

        int optimalChoiceIndex = rootNode.SelectMostVisitedChild();
        result.OptimalChoiceNodeIndex = optimalChoiceIndex;
        if (optimalChoiceIndex == -1)
        {
            // Fallback: if no child was visited, return first available choice
            result.OptimalChoice = availableChoices.Length > 0 ? availableChoices[0] :
                throw new InvalidOperationException("No available choices to select from.");
            result.Children = "No optimal choice found, using fallback";
            return result;
        }
        
        var optimalChild = rootNode.ChildNodes[optimalChoiceIndex];
        
        result.OptimalChoice = optimalChild.Choice;
        result.Children = rootNode.ToStringWithChildren();
        return result;
    }

    private static Node Selection(Node root)
    {
        Node current = root;
        int depth = 0;
        const int maxDepth = 50; // Prevent infinite loops
        
        while (current is { IsLeaf: false, IsTerminal: false } && depth < maxDepth)
        {
            Node? child = current.SelectHighestValueChild();
            if (child == null)
            {
                // No children available, return current node
                break;
            }
            current = child;
            depth++;
        }
        
        // Only log if we actually hit the depth limit (rare case)
        if (depth >= maxDepth)
        {
            Console.WriteLine($"Warning: MCTS selection reached maximum depth ({maxDepth})");
        }
        
        return current;
    }

    private Node Expansion(Node node, Random threadRandom)
    {
        if (node.IsTerminal)
        {
            return node; // Return terminal node for immediate backpropagation
        }

        if (node.Visits < 1)
        {
            return node; // Return unvisited node for simulation
        }

        GenerateChildren(node);

        if (node.ChildNodes.Count == 0)
        {
            return node; // Return node if no children can be generated
        }

        int randomChildIndex = threadRandom.Next(node.ChildNodes.Count);
        return node.ChildNodes[randomChildIndex];
    }

    private SimulatorResult Simulation(IBattle battle, Random threadRandom)
    {
        // For MCTS, we don't need to run a full battle simulation
        // Instead, we can use a simplified approach that evaluates the current position
        
        try
        {
            // Check if the battle is already in a terminal state
            var requestState = GetBattleRequestState(battle);
            if (Node.IsGameTerminal(requestState))
            {
                return requestState switch
                {
                    BattleRequestState.Player1Win => SimulatorResult.Player1Win,
                    BattleRequestState.Player2Win => SimulatorResult.Player2Win,
                    _ => throw new InvalidOperationException("Invalid terminal state")
                };
            }

            // For non-terminal states, use a heuristic evaluation instead of full simulation
            // This prevents the stack overflow while still providing useful information
            
            // Simple heuristic: count remaining Pokemon and HP
            double player1Score = EvaluatePosition(battle.Side1);
            double player2Score = EvaluatePosition(battle.Side2);
            
            // Add some randomness to prevent deterministic behavior
            player1Score += (threadRandom.NextDouble() - 0.5) * 0.1;
            player2Score += (threadRandom.NextDouble() - 0.5) * 0.1;
            
            // Return result based on which side has better position
            return player1Score > player2Score ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;
        }
        catch (Exception)
        {
            // Silently return random result to prevent MCTS from breaking
            return threadRandom.Next(2) == 0 ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;
        }
    }

    private double EvaluatePosition(Side side)
    {
        double score = 0.0;
        
        try
        {
            // Count alive Pokemon (heavily weighted)
            int alivePokemon = side.AllSlots.Count(p => !p.IsFainted);
            score += alivePokemon * 100.0;
            
            // Sum remaining HP (simplified approach)
            foreach (var pokemon in side.AllSlots.Where(p => !p.IsFainted))
            {
                // Simple HP score - just use current HP value
                score += pokemon.CurrentHp * 0.5;
            }
            
            // Bonus for active Pokemon (they can act immediately)
            foreach (var pokemon in side.ActivePokemon.Where(p => !p.IsFainted))
            {
                score += 25.0;
            }
        }
        catch (Exception)
        {
            // Return neutral score on error without logging
            score = 50.0;
        }
        
        return score;
    }

    private void BackPropagate(Node node, SimulatorResult result)
    {
        Node? currentNode = node;
        while (currentNode != null)
        {
            currentNode.Update(result, mctsPlayerId);
            currentNode = currentNode.Parent;
        }
    }

    private void GenerateChildren(Node parent, BattleChoice[]? availableChoices = null)
    {
        if (availableChoices == null)
        {
            // Get available choices for the current player to move
            BattleRequestState requestState = GetBattleRequestState(parent.Battle);
            PlayerId? playerToMove = requestState switch
            {
                BattleRequestState.RequestingPlayer1Input => PlayerId.Player1,
                BattleRequestState.RequestingPlayer2Input => PlayerId.Player2,
                BattleRequestState.RequestingBothPlayersInput => mctsPlayerId, // MCTS player moves when both can move
                _ => null, // Terminal or invalid state
            };

            if (playerToMove == null)
            {
                // No one can move, don't generate children
                return;
            }

            // Generate choices for whoever needs to move (MCTS player OR opponent)
            // This allows the tree to properly represent the full game sequence
            availableChoices = GetAvailableChoices(parent.Battle, playerToMove.Value);
        }

        // Check if children have already been generated
        if (parent.ChildNodes.Count > 0)
        {
            return; // Children already generated
        }

        // For the root node, we need to initialize UntriedChoices with the available choices
        // because they come from outside the node (from the calling MCTS algorithm)
        if (parent.Parent == null) // Root node
        {
            parent.UntriedChoices.Clear();
            parent.UntriedChoices.AddRange(availableChoices);
        }

        // Limit the number of children created to prevent memory explosion
        // For large choice spaces, only create a subset of the most promising choices
        var choicesToCreate = availableChoices;
        const int maxChildrenPerNode = 50; // Limit to prevent memory issues
        
        if (availableChoices.Length > maxChildrenPerNode)
        {
            // Only log warning for root node with very large choice spaces
            if (parent.Parent == null && availableChoices.Length > 200)
            {
                Console.WriteLine($"Warning: Large choice space ({availableChoices.Length}), limiting to {maxChildrenPerNode} children");
            }
            choicesToCreate = availableChoices.Take(maxChildrenPerNode).ToArray();
        }

        foreach (BattleChoice choice in choicesToCreate)
        {
            if (!parent.UntriedChoices.Contains(choice)) 
            {
                continue; // Skip without logging
            }

            try
            {
                // Create child node with lazy battle state creation
                var childNode = new Node(parent.Battle, parent, choice, explorationParameter, mctsPlayerId, library);
                parent.ChildNodes.Add(childNode);
                parent.UntriedChoices.Remove(choice);
            }
            catch (Exception)
            {
                // Remove the problematic choice from untried choices without logging
                parent.UntriedChoices.Remove(choice);
            }
        }
    }

    private static IBattle CopyBattle(IBattle original)
    {
        try
        {
            return original.Copy();
        }
        catch (StackOverflowException)
        {
            // Silently handle stack overflow and return original
            return original;
        }
        catch (Exception)
        {
            // Silently handle other copy failures
            return original;
        }
    }

    private int GenerateUniqueBattleSeed()
    {
        long seed = (long)_random.Next() + Interlocked.Increment(ref _battleSeedCounter);
        return (int)(seed % int.MaxValue);
    }

    private static BattleRequestState GetBattleRequestState(IBattle battle)
    {
        if (battle.IsGameComplete)
        {
            // Check who won from the PostGameTurn
            if (battle.CurrentTurn is PostGameTurn postGameTurn)
            {
                return postGameTurn.Winner == PlayerId.Player1 
                    ? BattleRequestState.Player1Win 
                    : BattleRequestState.Player2Win;
            }
        }

        // Check if game should end due to all Pokemon fainted
        if (IsAllPokemonFainted(battle.Side1))
            return BattleRequestState.Player2Win;
        if (IsAllPokemonFainted(battle.Side2))
            return BattleRequestState.Player1Win;

        // Battle is ongoing - determine who needs to input
        // For simplicity, assume both players need input during gameplay turns
        return BattleRequestState.RequestingBothPlayersInput;
    }

    private static bool IsAllPokemonFainted(Side side)
    {
        return side.AllSlots.All(pokemon => pokemon.IsFainted);
    }

    private static BattleChoice[] GetAvailableChoices(IBattle battle, PlayerId playerId)
    {
        return battle.GenerateChoicesForMcts(playerId);
    }

    private static SimulatorResult GetBattleResult(IBattle battle)
    {
        BattleRequestState state = GetBattleRequestState(battle);
        return state switch
        {
            BattleRequestState.Player1Win => SimulatorResult.Player1Win,
            BattleRequestState.Player2Win => SimulatorResult.Player2Win,
            BattleRequestState.RequestingPlayer1Input or BattleRequestState.RequestingPlayer2Input
                or BattleRequestState.RequestingBothPlayersInput =>
                // Battle is still ongoing, cannot determine result
                throw new InvalidOperationException("Battle is still ongoing"),
            _ => throw new InvalidOperationException("Battle is not in terminal state")
        };
    }

    private class Node
    {
        public Node? Parent { get; }
        public BattleChoice Choice { get; }
        public List<BattleChoice> UntriedChoices { get; }
        public List<Node> ChildNodes { get; }
        
        // Lazy battle state management
        private readonly IBattle _parentBattle;
        private IBattle? _battleCache;
        
        public IBattle Battle
        {
            get
            {
                if (_battleCache == null)
                {
                    if (IsRoot)
                    {
                        _battleCache = _parentBattle; // Root uses original battle state
                    }
                    else
                    {
                        // Create battle state only when needed
                        _battleCache = CreateBattleWithChoice(_parentBattle, Choice, _mctsPlayerId);
                    }
                }
                return _battleCache;
            }
        }
        
        // Simplified properties without thread-safety overhead
        public int Wins { get; private set; }
        public int Losses { get; private set; }
        public int Visits => Wins + Losses;
        
        public bool IsLeaf => ChildNodes.Count < 1;
        
        private bool IsRoot => Parent == null;
        public bool IsTerminal => IsGameTerminal(GetBattleRequestState(Battle));
        
        private float WinRate => Visits > 0 ? Wins / (float)Visits : 0f;
        private double Value => CalculateValue();

        private readonly double _explorationParameter;
        private readonly PlayerId _mctsPlayerId;
        private readonly Library _library;

        public Node(IBattle battle, Node? parent, BattleChoice choice, double explorationParameter,
            PlayerId mctsPlayerId, Library library)
        {
            Parent = parent;
            Choice = choice;
            _explorationParameter = explorationParameter;
            _mctsPlayerId = mctsPlayerId;
            _library = library;

            // Store the parent battle state but don't apply choice immediately
            _parentBattle = battle;
            _battleCache = null; // Battle will be created lazily when first accessed

            // Initialize collections and statistics
            ChildNodes = [];
            UntriedChoices = [];
            Wins = 0;
            Losses = 0;

            // Don't initialize available choices for non-root nodes immediately
            // This will be done lazily when the node is first expanded
        }

        private double CalculateValue()
        {
            if (IsRoot)
            {
                return -1.0;
            }
            
            if (Visits < 1)
            {
                return double.MaxValue; // Unvisited nodes get highest priority
            }
            
            // At this point we know we're not the root node, so Parent should not be null
            if (Parent == null)
            {
                throw new InvalidOperationException("Non-root node should have a parent");
            }
            
            double winRate = Wins / (double)Visits;
            double explorationTerm = _explorationParameter * Math.Sqrt(Math.Log(Parent.Visits) / Visits);
            
            return winRate + explorationTerm;
        }

        private BattleChoice[] GetAvailableChoicesForNode()
        {
            // Only generate choices when the node is actually being expanded
            // This prevents unnecessary computation for nodes that are never visited
            
            try
            {
                BattleRequestState state = GetBattleRequestState(Battle);
                if (IsGameTerminal(state)) return [];

                // Determine which player should move based on current battle state
                PlayerId? playerToMove = state switch
                {
                    BattleRequestState.RequestingPlayer1Input => PlayerId.Player1,
                    BattleRequestState.RequestingPlayer2Input => PlayerId.Player2,
                    BattleRequestState.RequestingBothPlayersInput => _mctsPlayerId, // MCTS player moves when both can move
                    _ => null, // Terminal or invalid state
                };

                // Generate choices for whoever needs to move (not just MCTS player)
                // This ensures the tree can represent the complete game sequence
                return playerToMove != null ? GetAvailableChoices(Battle, playerToMove.Value) : []; // No valid player to move
            }
            catch (Exception)
            {
                // Return empty choices to prevent crashes without logging
                return [];
            }
        }

        public static bool IsGameTerminal(BattleRequestState state)
        {
            return state is BattleRequestState.Player1Win or BattleRequestState.Player2Win;
        }

        public Node? SelectHighestValueChild()
        {
            if (ChildNodes.Count == 0) return null;
            
            Node? bestChild = null;
            double maxUcb = double.MinValue;
            
            foreach (Node child in ChildNodes)
            {
                double ucb = child.Value;
                if (ucb > maxUcb)
                {
                    maxUcb = ucb;
                    bestChild = child;
                }
            }
            return bestChild;
        }

        public int SelectMostVisitedChild()
        {
            if (ChildNodes.Count == 0) return -1;

            int mostVisitedIndex = -1;
            int maxVisits = 0;

            for (int i = 0; i < ChildNodes.Count; i++)
            {
                Node child = ChildNodes[i];
                
                // For root node selection, consider all children regardless of player
                if (child.Visits > maxVisits)
                {
                    maxVisits = child.Visits;
                    mostVisitedIndex = i;
                }
            }
            return mostVisitedIndex;
        }

        public void Update(SimulatorResult result, PlayerId mctsPlayerId)
        {
            bool mctsWon = (result == SimulatorResult.Player1Win && mctsPlayerId == PlayerId.Player1) ||
                          (result == SimulatorResult.Player2Win && mctsPlayerId == PlayerId.Player2);

            // Simple update without thread-safety overhead
            if (mctsWon)
            {
                Wins++;
            }
            else
            {
                Losses++;
            }
        }

        private PlayerId? GetPlayerToMove()
        {
            if (IsRoot) return null; // Root doesn't represent a move
            
            BattleRequestState state = GetBattleRequestState(Parent!.Battle);
            return state switch
            {
                BattleRequestState.RequestingPlayer1Input => PlayerId.Player1,
                BattleRequestState.RequestingPlayer2Input => PlayerId.Player2,
                BattleRequestState.RequestingBothPlayersInput => _mctsPlayerId,
                _ => null,
            };
        }

        public override string ToString()
        {
            PlayerId? playerToMove = GetPlayerToMove();
            string playerInfo = playerToMove != null ? $" (Player: {playerToMove})" : "";
            
            // Simple property access without locking
            float winRate = Visits > 0 ? Wins / (float)Visits : 0f;
            return $"Choice: {Choice}{playerInfo}, Wins: {Wins}, Visits: {Visits}, WinRate: {winRate:P2}, ChildNodes: {ChildNodes.Count}, Value: {Value:F3}";
        }

        public string ToStringWithChildren()
        {
            var sb = new StringBuilder();
            
            // Simple access without thread-safety concerns
            var children = ChildNodes.ToArray();
            int untriedCount = UntriedChoices.Count;
            var untriedChoices = UntriedChoices.Select(c => c.ToString()).ToArray();
            
            sb.AppendLine($"Root: {ToString()} (Children: {children.Length}, UntriedChoices: {untriedCount})");
            sb.AppendLine("Root's children:");
            for (int i = 0; i < children.Length; i++)
            {
                sb.AppendLine($"{i}: {children[i]}");
            }
            if (untriedCount > 0)
            {
                sb.AppendLine($"Untried choices: {string.Join(", ", untriedChoices)}");
            }
            return sb.ToString();
        }

        private IBattle CreateBattleWithChoice(IBattle battle, BattleChoice choice, PlayerId playerId)
        {
            try
            {
                // Create a deep copy of the battle for simulation
                IBattle asyncBattle = battle.Copy();
                
                if (asyncBattle is IBattleMctsOperations mctsOps)
                {
                    mctsOps.ApplyChoiceSync(playerId, choice);
                }
                
                return asyncBattle;
            }
            catch (StackOverflowException)
            {
                // Return original battle to prevent crash without logging
                return battle;
            }
            catch (Exception)
            {
                // Return the original battle instead of throwing, without logging
                return battle;
            }
        }
    }
}