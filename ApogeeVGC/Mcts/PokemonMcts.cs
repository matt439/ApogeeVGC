using System.Text;
using ApogeeVGC.Data;
using ApogeeVGC.Sim;
using ApogeeVGC.Player;

namespace ApogeeVGC.Mcts;

public class PokemonMonteCarloTreeSearch(
    int maxIterations,
    double explorationParameter,
    PlayerId mctsPlayerId,
    Library library,
    int? seed = null,
    int? maxDegreeOfParallelism = null)
{
    private readonly Random _random = seed.HasValue ? new Random(seed.Value) : new Random();
    private readonly int _maxDegreeOfParallelism = maxDegreeOfParallelism ?? Environment.ProcessorCount;
    
    private int _battleSeedCounter;

    public struct MoveResult
    {
        public Choice OptimalChoice = Choice.Invalid;
        public int OptimalChoiceNodeIndex = -1;
        public string Children = string.Empty;

        public MoveResult(Choice optimalChoice, int optimalChoiceNodeIndex, string children)
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

    public MoveResult FindBestChoice(Battle battle, Choice[] availableChoices)
    {
        int uniqueBattleSeed = _random.Next() + Interlocked.Increment(ref _battleSeedCounter);
        
        // Create a seeded copy of the battle for deterministic simulation
        Battle battleCopy = battle.DeepCopy(false);
        
        // If the original battle doesn't have a seed, create a new battle with the unique seed
        // This ensures all random operations in this MCTS run are deterministic
        if (battle.BattleSeed == null)
        {
            battleCopy = BattleGenerator.GenerateTestBattle(
                battle.Library, 
                "MCTS_Player1", 
                "MCTS_Player2", 
                false, 
                uniqueBattleSeed);
            
            // Copy the current battle state to the seeded battle
        }

        var rootNode = new Node(battleCopy, null, Choice.Invalid, explorationParameter, mctsPlayerId);
        GenerateChildren(rootNode, availableChoices);

        // Ensure root node has children before starting MCTS
        if (rootNode.ChildNodes.Count == 0)
        {
            // If no children, just return the first available choice or Invalid
            return new MoveResult(
                availableChoices.Length > 0 ? availableChoices[0] : Choice.Invalid,
                -1,
                "No children generated for root node"
            );
        }

        // **PARALLEL MCTS IMPLEMENTATION**
        // Use Parallel.For to run MCTS iterations concurrently
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = _maxDegreeOfParallelism,
        };

        Parallel.For(0, maxIterations, parallelOptions, i =>
        {
            // Each thread gets its own random number generator to avoid contention
            var threadRandom = new Random(_random.Next() + i);
            
            // Selection
            Node node = Selection(rootNode);

            // Expansion
            Node? expandedNode = Expansion(node, threadRandom);
            if (expandedNode == null)
            {
                // Check if the node is terminal before trying to get battle result
                if (!node.IsTerminal) return;

                SimulatorResult battleResult = GetBattleResult(node.Battle);
                BackPropagate(node, battleResult);
                // If not terminal and expandedNode is null, skip this iteration
                return;
            }

            // Simulation
            Battle simulationBattle = CopyBattle(expandedNode.Battle);
            SimulatorResult simulationResult = Simulation(simulationBattle, threadRandom);

            // Backpropagation
            BackPropagate(expandedNode, simulationResult);
        });

        var result = new MoveResult();

        int optimalChoiceIndex = rootNode.SelectMostVisitedChild();
        result.OptimalChoiceNodeIndex = optimalChoiceIndex;
        if (optimalChoiceIndex == -1)
        {
            // Fallback: if no child was visited, return first available choice
            result.OptimalChoice = availableChoices.Length > 0 ? availableChoices[0] : Choice.Invalid;
            result.Children = "No optimal choice found, using fallback";
            return result;
        }
        result.OptimalChoice = rootNode.ChildNodes[optimalChoiceIndex].Choice;
        result.Children = rootNode.ToStringWithChildren();
        return result;
    }

    private static Node Selection(Node root)
    {
        Node current = root;
        while (current is { IsLeaf: false, IsTerminal: false })
        {
            // Thread-safe read of children - no lock needed for reading
            Node? child = current.SelectHighestValueChild();
            if (child == null)
            {
                // No children available, return current node
                break;
            }
            current = child;
        }
        return current;
    }

    private Node? Expansion(Node node, Random threadRandom)
    {
        if (node.IsTerminal)
        {
            return null; // Terminal node, no expansion possible
        }

        // If this node hasn't been visited yet, we should simulate it directly
        // rather than expanding it (standard MCTS behavior)
        if (node.Visits < 1)
        {
            return node; // Return the node itself for simulation
        }

        // Thread-safe expansion - GenerateChildren handles synchronization internally
        GenerateChildren(node);

        // Thread-safe read of children count
        if (node.ChildNodes.Count == 0)
        {
            return null;
        }

        // Select a random unvisited child node using thread-local random
        int randomChildIndex = threadRandom.Next(node.ChildNodes.Count);
        return node.ChildNodes[randomChildIndex];
    }

    private SimulatorResult Simulation(Battle battle, Random threadRandom)
    {
        // Random playout using random players with thread-local random seeds
        var randomPlayer1 = new PlayerRandom(PlayerId.Player1, battle, library,
            PlayerRandomStrategy.AllChoices,
            threadRandom.Next());
        var randomPlayer2 = new PlayerRandom(PlayerId.Player2, battle, library,
            PlayerRandomStrategy.AllChoices,
            threadRandom.Next());
        
        var simulator = new Simulator
        {
            Battle = battle,
            Player1 = randomPlayer1,
            Player2 = randomPlayer2,
            PrintDebug = false,
        };

        return simulator.Run();
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

    private void GenerateChildren(Node parent, Choice[]? availableChoices = null)
    {
        if (availableChoices == null)
        {
            // Get available choices for the current player to move
            BattleRequestState requestState = parent.Battle.GetRequestState();
            PlayerId playerToMove = requestState switch
            {
                BattleRequestState.RequestingPlayer1Input => PlayerId.Player1,
                BattleRequestState.RequestingPlayer2Input => PlayerId.Player2,
                BattleRequestState.RequestingBothPlayersInput => mctsPlayerId, // MCTS player moves when both can move
                _ => PlayerId.None, // Terminal or invalid state
            };

            if (playerToMove == PlayerId.None)
            {
                // No one can move, don't generate children
                return;
            }

            // Generate choices for whoever needs to move (MCTS player OR opponent)
            // This allows the tree to properly represent the full game sequence
            availableChoices = parent.Battle.GetAvailableChoices(playerToMove);
        }

        lock (parent.LockObject)
        {
            // Check if children have already been generated by another thread
            if (parent.ChildNodes.Count > 0)
            {
                return; // Another thread already generated children
            }

            foreach (Choice choice in availableChoices)
            {
                if (!parent.UntriedChoices.Contains(choice)) continue;

                var childNode = new Node(parent.Battle, parent, choice, explorationParameter, mctsPlayerId);
                parent.ChildNodes.Add(childNode);
                parent.UntriedChoices.Remove(choice);
            }
        }
    }

    private static Battle CopyBattle(Battle original)
    {
        return original.DeepCopy(false);
    }

    private static SimulatorResult GetBattleResult(Battle battle)
    {
        BattleRequestState state = battle.GetRequestState();
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
        public Choice Choice { get; }
        public Battle Battle { get; }
        public List<Choice> UntriedChoices { get; }
        public List<Node> ChildNodes { get; }
        
        public readonly object LockObject = new(); // Make it public for external locking
        private int _wins;
        private int _losses;
        
        // Thread-safe properties
        public int Wins 
        { 
            get { lock (LockObject) { return _wins; } }
            private set { lock (LockObject) { _wins = value; } }
        }
        
        public int Losses 
        { 
            get { lock (LockObject) { return _losses; } }
            private set { lock (LockObject) { _losses = value; } }
        }
        
        public int Visits 
        { 
            get { lock (LockObject) { return _wins + _losses; } }
        }
        
        public bool IsLeaf 
        { 
            get { lock (LockObject) { return ChildNodes.Count < 1; } }
        }
        
        private bool IsRoot => Parent == null;
        public bool IsTerminal => IsGameTerminal(Battle.GetRequestState());
        
        private float WinRate 
        { 
            get 
            { 
                lock (LockObject) 
                { 
                    int visits = _wins + _losses;
                    return visits > 0 ? _wins / (float)visits : 0f; 
                } 
            }
        }
        
        private double Value => CalculateValue();

        private readonly double _explorationParameter;
        private readonly PlayerId _mctsPlayerId;

        public Node(Battle battle, Node? parent, Choice choice, double explorationParameter,
            PlayerId mctsPlayerId)
        {
            Parent = parent;
            Choice = choice;
            _explorationParameter = explorationParameter;
            _mctsPlayerId = mctsPlayerId;

            if (!IsRoot)
            {
                // This prevents applying choices for the wrong player (e.g., move choices when in FaintedSelect state)
                BattleRequestState requestState = battle.GetRequestState();
                PlayerId playerToMove = requestState switch
                {
                    BattleRequestState.RequestingPlayer1Input => PlayerId.Player1,
                    BattleRequestState.RequestingPlayer2Input => PlayerId.Player2,
                    BattleRequestState.RequestingBothPlayersInput => _mctsPlayerId, // In this case, use MCTS player
                    _ => throw new InvalidOperationException($"Cannot apply choice in battle state: {requestState}")
                };
                
                Battle = battle.DeepCopyAndApplyChoice(playerToMove, choice, false);
            }
            else
            {
                Battle = battle; // Root uses original battle state
            }

            // Initialize available choices
            if (IsTerminal)
            {
                UntriedChoices = [];
            }
            else
            {
                UntriedChoices = GetAvailableChoicesForNode().ToList();
            }

            ChildNodes = [];
            _wins = 0;
            _losses = 0;
        }

        private double CalculateValue()
        {
            if (IsRoot)
            {
                return -1.0;
            }
            
            lock (LockObject)
            {
                int visits = _wins + _losses;
                if (visits < 1)
                {
                    return double.MaxValue;
                }
                
                // At this point we know we're not the root node, so Parent should not be null
                if (Parent == null)
                {
                    throw new InvalidOperationException("Non-root node should have a parent");
                }
                
                return _wins / (double)visits +
                       _explorationParameter * Math.Sqrt(Math.Log(Parent.Visits) / visits);
            }
        }

        private Choice[] GetAvailableChoicesForNode()
        {
            try
            {
                BattleRequestState state = Battle.GetRequestState();
                if (IsGameTerminal(state)) return [];

                // Determine which player should move based on current battle state
                PlayerId playerToMove = state switch
                {
                    BattleRequestState.RequestingPlayer1Input => PlayerId.Player1,
                    BattleRequestState.RequestingPlayer2Input => PlayerId.Player2,
                    BattleRequestState.RequestingBothPlayersInput => _mctsPlayerId, // MCTS player moves when both can move
                    _ => PlayerId.None, // Terminal or invalid state
                };

                // Generate choices for whoever needs to move (not just MCTS player)
                // This ensures the tree can represent the complete game sequence
                return playerToMove != PlayerId.None ? Battle.GetAvailableChoices(playerToMove) : []; // No valid player to move
            }
            catch
            {
                return [];
            }
        }

        private static bool IsGameTerminal(BattleRequestState state)
        {
            return state is BattleRequestState.Player1Win or BattleRequestState.Player2Win;
        }

        public Node? SelectHighestValueChild()
        {
            // Thread-safe reading of children - use a snapshot to avoid collection modification issues
            Node[] children;
            lock (LockObject)
            {
                if (ChildNodes.Count == 0) return null;
                children = ChildNodes.ToArray();
            }
            
            Node? bestChild = null;
            double maxUcb = double.MinValue;
            
            foreach (Node child in children)
            {
                double ucb = child.Value;
                if (!(ucb > maxUcb)) continue;
                maxUcb = ucb;
                bestChild = child;
            }
            return bestChild;
        }

        public int SelectMostVisitedChild()
        {
            // Thread-safe reading of children
            Node[] children;
            lock (LockObject)
            {
                if (ChildNodes.Count == 0) return -1;
                children = ChildNodes.ToArray();
            }
            
            int mostVisitedIndex = -1;
            int maxVisits = 0;
            
            foreach (Node child in children)
            {
                // Only consider nodes that represent MCTS player moves for final selection
                // This ensures we're selecting among our own choices, not opponent responses
                PlayerId nodePlayer = child.GetPlayerToMove();
                if (nodePlayer != _mctsPlayerId && nodePlayer != PlayerId.None) continue;

                if (child.Visits <= maxVisits) continue;
                maxVisits = child.Visits;
                // Find the original index in the actual ChildNodes list
                lock (LockObject)
                {
                    mostVisitedIndex = ChildNodes.IndexOf(child);
                }
            }
            return mostVisitedIndex;
        }

        public void Update(SimulatorResult result, PlayerId mctsPlayerId)
        {
            bool mctsWon = (result == SimulatorResult.Player1Win && mctsPlayerId == PlayerId.Player1) ||
                          (result == SimulatorResult.Player2Win && mctsPlayerId == PlayerId.Player2);

            // Thread-safe update of win/loss counts
            lock (LockObject)
            {
                if (mctsWon)
                {
                    _wins++;
                }
                else
                {
                    _losses++;
                }
            }
        }

        private PlayerId GetPlayerToMove()
        {
            if (IsRoot) return PlayerId.None; // Root doesn't represent a move
            
            BattleRequestState state = Parent!.Battle.GetRequestState();
            return state switch
            {
                BattleRequestState.RequestingPlayer1Input => PlayerId.Player1,
                BattleRequestState.RequestingPlayer2Input => PlayerId.Player2,
                BattleRequestState.RequestingBothPlayersInput => _mctsPlayerId,
                _ => PlayerId.None,
            };
        }

        public override string ToString()
        {
            PlayerId playerToMove = GetPlayerToMove();
            string playerInfo = playerToMove != PlayerId.None ? $" (Player: {playerToMove})" : "";
            
            // Thread-safe reading of statistics
            lock (LockObject)
            {
                int visits = _wins + _losses;
                float winRate = visits > 0 ? _wins / (float)visits : 0f;
                return $"Choice: {Choice}{playerInfo}, Wins: {_wins}, Visits: {visits}, WinRate: {winRate:P2}, ChildNodes: {ChildNodes.Count}, Value: {Value:F3}";
            }
        }

        public string ToStringWithChildren()
        {
            var sb = new StringBuilder();
            
            // Thread-safe snapshot of current state
            Node[] children;
            int untriedCount;
            string[] untriedChoices;
            
            lock (LockObject)
            {
                children = ChildNodes.ToArray();
                untriedCount = UntriedChoices.Count;
                untriedChoices = UntriedChoices.Select(c => c.ToString()).ToArray();
            }
            
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
    }
}