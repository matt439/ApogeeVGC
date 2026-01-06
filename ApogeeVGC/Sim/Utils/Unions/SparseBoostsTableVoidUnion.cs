using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// SparseBoostsTable | void
/// </summary>
public abstract record SparseBoostsTableVoidUnion
{
    public static implicit operator SparseBoostsTableVoidUnion(SparseBoostsTable table) =>
    new SparseBoostsTableSparseBoostsTableVoidUnion(table);
    public static implicit operator SparseBoostsTableVoidUnion(VoidReturn value) =>
        new VoidSparseBoostsTableVoidUnion(value);
    public static SparseBoostsTableVoidUnion FromVoid() => new VoidSparseBoostsTableVoidUnion(new VoidReturn());
}

public record SparseBoostsTableSparseBoostsTableVoidUnion(SparseBoostsTable Table) :
    SparseBoostsTableVoidUnion;
public record VoidSparseBoostsTableVoidUnion(VoidReturn Value) : SparseBoostsTableVoidUnion;
