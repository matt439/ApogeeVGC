using System.Collections.ObjectModel;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    public IReadOnlyDictionary<SpecieId, Species> SpeciesDataDictionary { get; }

    public SpeciesData()
    {
        var combinedSpecies = new Dictionary<SpecieId, Species>();

        // Combine data from all methods
        foreach (var kvp in GenerateSpeciesData1To50())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData51To100())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData101To150())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData151To200())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData201To250())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData251To300())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData301To350())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData351To400())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData401To450())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData451To500())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData501To550())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData551To600())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData601To650())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData651To700())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData701To750())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData751To800())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData801To850())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData851To900())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData901To950())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData951To1000())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData1001To1050())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        SpeciesDataDictionary = new ReadOnlyDictionary<SpecieId, Species>(combinedSpecies);
    }
}