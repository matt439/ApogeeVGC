using System.Text;
using ApogeeVGC.Sim;
using ApogeeVGC.Player;

namespace ApogeeVGC.Mcts;

public class PokemonMonteCarloTreeSearch(
    int maxIterations,
    double explorationParameter,
    PlayerId mctsPlayerId,
    int? seed = null)
{
    private readonly Random _random = seed.HasValue ? new Random(seed.Value) : new Random();

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
        var rootNode = new Node(battle, null, Choice.Invalid, explorationParameter, mctsPlayerId);
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

        for (int i = 0; i < maxIterations; i++)
        {
            // Selection
            Node node = Selection(rootNode);
            if (node == null)
            {
                throw new Exception("No node selected");
            }

            // Expansion
            Node? expandedNode = Expansion(node);
            if (expandedNode == null)
            {
                // Check if the node is terminal before trying to get battle result
                if (node.IsTerminal)
                {
                    SimulatorResult battleResult = GetBattleResult(node.Battle);
                    BackPropagate(node, battleResult);
                }
                // If not terminal and expandedNode is null, it means the node hasn't been visited enough
                // or has no children to expand - skip this iteration
                continue;
            }

            // Simulation
            Battle simulationBattle = CopyBattle(expandedNode.Battle);
            SimulatorResult simulationResult = Simulation(simulationBattle);

            // Backpropagation
            BackPropagate(expandedNode, simulationResult);
        }

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
        while (!current.IsLeaf && !current.IsTerminal)
        {
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

    private Node? Expansion(Node node)
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

        // Node has been visited, now we can expand it
        GenerateChildren(node);

        // If no children were generated (e.g., not our turn), return null
        if (node.ChildNodes.Count == 0)
        {
            return null;
        }

        // Select a random unvisited child node
        int randomChildIndex = _random.Next(node.ChildNodes.Count);
        return node.ChildNodes[randomChildIndex];
    }

    private SimulatorResult Simulation(Battle battle)
    {
        // Random playout using random players
        var randomPlayer1 = new PlayerRandom(PlayerId.Player1, battle, PlayerRandomStrategy.AllChoices,
            _random.Next());
        var randomPlayer2 = new PlayerRandom(PlayerId.Player2, battle, PlayerRandomStrategy.AllChoices,
            _random.Next());
        
        var simulator = new Simulator
        {
            Battle = battle,
            Player1 = randomPlayer1,
            Player2 = randomPlayer2,
            PrintDebug = false // Set to true if you want debug output
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
                _ => PlayerId.None // Terminal or invalid state
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

        foreach (Choice choice in availableChoices)
        {
            if (!parent.UntriedChoices.Contains(choice)) continue;

            var childNode = new Node(parent.Battle, parent, choice, explorationParameter, mctsPlayerId);
            parent.ChildNodes.Add(childNode);
            parent.UntriedChoices.Remove(choice);
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
        private int Wins { get; set; }
        private int Losses { get; set; }
        public int Visits => Wins + Losses;
        public bool IsLeaf => ChildNodes.Count < 1;
        private bool IsRoot => Parent == null;
        public bool IsTerminal => IsGameTerminal(Battle.GetRequestState());
        private float WinRate => Visits > 0 ? Wins / (float)Visits : 0f;
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
                // CRITICAL FIX: Determine which player should make this choice based on current battle state
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
            Wins = 0;
            Losses = 0;
        }

        private double CalculateValue()
        {
            if (IsRoot)
            {
                return -1.0;
            }
            if (Visits < 1)
            {
                return double.MaxValue;
            }
            
            // At this point we know we're not the root node, so Parent should not be null
            if (Parent == null)
            {
                throw new InvalidOperationException("Non-root node should have a parent");
            }
            
            return Wins / (double)Visits +
                   _explorationParameter * Math.Sqrt(Math.Log(Parent.Visits) / Visits);
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
                    _ => PlayerId.None // Terminal or invalid state
                };

                // Generate choices for whoever needs to move (not just MCTS player)
                // This ensures the tree can represent the complete game sequence
                if (playerToMove != PlayerId.None)
                {
                    return Battle.GetAvailableChoices(playerToMove);
                }

                return []; // No valid player to move
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
                
                // Only consider nodes that represent MCTS player moves for final selection
                // This ensures we're selecting among our own choices, not opponent responses
                PlayerId nodePlayer = child.GetPlayerToMove();
                if (nodePlayer != _mctsPlayerId && nodePlayer != PlayerId.None) continue;
                
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

            // For nodes representing MCTS player moves: wins/losses are direct
            // For nodes representing opponent moves: we want to track what's good for MCTS player
            // The outcome is always from MCTS player's perspective regardless of whose move this node represents
            if (mctsWon)
            {
                Wins++;
            }
            else
            {
                Losses++;
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
                _ => PlayerId.None
            };
        }

        public override string ToString()
        {
            PlayerId playerToMove = GetPlayerToMove();
            string playerInfo = playerToMove != PlayerId.None ? $" (Player: {playerToMove})" : "";
            return $"Choice: {Choice}{playerInfo}, Wins: {Wins}, Visits: {Visits}, WinRate: {WinRate:P2}, ChildNodes: {ChildNodes.Count}, Value: {Value:F3}";
        }

        public string ToStringWithChildren()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Root: {ToString()} (Children: {ChildNodes.Count}, UntriedChoices: {UntriedChoices.Count})");
            sb.AppendLine("Root's children:");
            for (int i = 0; i < ChildNodes.Count; i++)
            {
                sb.AppendLine($"{i}: {ChildNodes[i]}");
            }
            if (UntriedChoices.Count > 0)
            {
                sb.AppendLine($"Untried choices: {string.Join(", ", UntriedChoices)}");
            }
            return sb.ToString();
        }
    }
}