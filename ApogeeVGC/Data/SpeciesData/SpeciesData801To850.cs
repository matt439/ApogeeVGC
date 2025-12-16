using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData801To850()
    {
        return new Dictionary<SpecieId, Species>();
    }
}
