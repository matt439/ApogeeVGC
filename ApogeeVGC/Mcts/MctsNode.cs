namespace ApogeeVGC.Mcts;

/// <summary>
/// MCTS search configuration.
/// </summary>
public sealed class MctsConfig
{
    /// <summary>Exploration constant for PUCT formula.</summary>
    public float CPuct { get; init; } = 1.5f;

    /// <summary>Number of MCTS iterations per search call.</summary>
    public int NumIterations { get; init; } = 200;

    /// <summary>Number of determinizations (K). 1 = perfect info mode.</summary>
    public int NumDeterminizations { get; init; } = 1;

    /// <summary>Dirichlet noise alpha for root exploration.</summary>
    public float DirichletAlpha { get; init; } = 0.3f;

    /// <summary>Fraction of Dirichlet noise mixed into root priors.</summary>
    public float DirichletEpsilon { get; init; } = 0.25f;
}

/// <summary>
/// An edge in the MCTS tree, representing a joint action pair (slotA, slotB).
/// </summary>
public sealed class MctsEdge
{
    public required LegalAction ActionA { get; init; }
    public required LegalAction? ActionB { get; init; }

    /// <summary>Prior probability P(s,a) from the policy network.</summary>
    public float PriorP { get; set; }

    /// <summary>Sum of backed-up values W(s,a).</summary>
    public float TotalValue { get; set; }

    /// <summary>Visit count N(s,a).</summary>
    public int VisitCount { get; set; }

    /// <summary>Child node (lazily expanded).</summary>
    public MctsNode? Child { get; set; }

    /// <summary>Mean action value Q(s,a) = W(s,a) / N(s,a).</summary>
    public float Q => VisitCount > 0 ? TotalValue / VisitCount : 0f;
}

/// <summary>
/// A node in the MCTS tree, representing a game state.
/// </summary>
public sealed class MctsNode
{
    public List<MctsEdge> Edges { get; } = [];

    /// <summary>Total visits to this node N(s).</summary>
    public int VisitCount { get; set; }

    /// <summary>Whether this node's edges have been populated.</summary>
    public bool IsExpanded { get; set; }

    /// <summary>Whether this is a terminal game state (win/loss/draw).</summary>
    public bool IsTerminal { get; set; }

    /// <summary>Value of a terminal state (1=win, 0=loss).</summary>
    public float TerminalValue { get; set; }
}
