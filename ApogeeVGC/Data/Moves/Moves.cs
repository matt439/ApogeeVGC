using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    public IReadOnlyDictionary<MoveId, Move> MovesData { get; }
    private readonly Library _library;

    public Moves(Library library)
    {
        _library = library;
        MovesData = new ReadOnlyDictionary<MoveId, Move>(CreateMoves());
    }

    private Dictionary<MoveId, Move> CreateMoves()
    {
        var moves = new Dictionary<MoveId, Move>();

        // Combine all partial move dictionaries
        foreach (var kvp in CreateMovesAbc())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesDef())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesGhi())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesIjk())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesJkl())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesMno())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesPqr())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesStu())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesVwx())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesYz())
            moves[kvp.Key] = kvp.Value;

        return moves;
    }
}