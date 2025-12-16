using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData901To950()
    {
        return new Dictionary<SpecieId, Species>();
    }
}
