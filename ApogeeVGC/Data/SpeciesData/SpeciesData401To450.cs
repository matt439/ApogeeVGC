using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData401To450()
    {
        return new Dictionary<SpecieId, Species>();
    }
}
