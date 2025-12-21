using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

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
        foreach (var kvp in CreateMovesABC())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesDEF())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesGHI())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesJKL())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesMNO())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesPQR())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesSTU())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesVWX())
            moves[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateMovesYZ())
            moves[kvp.Key] = kvp.Value;

        return moves;
    }
}