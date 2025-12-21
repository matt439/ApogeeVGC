using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data;

// Auto-generated from pokemon-showdown/data/learnsets.ts
// Do not edit manually - use LearnsetConverter.ConvertToMultipleFiles() to regenerate
// Split into 8 partial files for compiler performance
public partial record Learnsets
{
    public IReadOnlyDictionary<SpecieId, Learnset> LearnsetsData { get; }

    public Learnsets()
    {
        var learnsets = new Dictionary<SpecieId, Learnset>();
        InitializeLearnsets1(learnsets);
        InitializeLearnsets2(learnsets);
        InitializeLearnsets3(learnsets);
        InitializeLearnsets4(learnsets);
        InitializeLearnsets5(learnsets);
        InitializeLearnsets6(learnsets);
        InitializeLearnsets7(learnsets);
        InitializeLearnsets8(learnsets);
        LearnsetsData = new ReadOnlyDictionary<SpecieId, Learnset>(learnsets);
    }
}
