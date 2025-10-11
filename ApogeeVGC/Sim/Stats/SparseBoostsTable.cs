namespace ApogeeVGC.Sim.Stats;

public record SparseBoostsTable
{
    public int? Atk { get; set; }
    public int? Def { get; set; }
    public int? SpA { get; set; }
    public int? SpD { get; set; }
    public int? Spe { get; set; }
    public int? Accuracy { get; set; }
    public int? Evasion { get; set; }
}