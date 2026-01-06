using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// SparseBoostsTable | false
/// </summary>
public abstract record ItemBoosts
{
 public static implicit operator ItemBoosts(SparseBoostsTable table) =>
     new SparseBoostsTableItemBoosts(table);

 public static ItemBoosts FromFalse() => new FalseItemBoosts();
}

public record SparseBoostsTableItemBoosts(SparseBoostsTable Table) : ItemBoosts;
public record FalseItemBoosts : ItemBoosts;
