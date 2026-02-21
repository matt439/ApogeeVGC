using System.Collections.Frozen;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    public FrozenDictionary<SpecieId, Species> SpeciesDataDictionary { get; }

    public SpeciesData()
    {
        var combinedSpecies = new Dictionary<SpecieId, Species>();

        // Combine data from all methods
        foreach (var kvp in GenerateSpeciesData0001to0050())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0051to0100())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0101to0150())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0151to0200())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0201to0250())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0251to0300())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0301to0350())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0351to0400())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0401to0450())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0451to0500())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0501to0550())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0551to0600())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0601to0650())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0651to0700())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0701to0750())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0751to0800())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0801to0850())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0851to0900())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0901to0950())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData0951to1000())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData1001to1050())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        SpeciesDataDictionary = combinedSpecies.ToFrozenDictionary();
    }
}