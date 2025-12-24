using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Abilities;

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    public IReadOnlyDictionary<AbilityId, Ability> AbilitiesData { get; }
    private readonly Library _library;

    public Abilities(Library library)
    {
        _library = library;
        AbilitiesData = new ReadOnlyDictionary<AbilityId, Ability>(CreateAbilities());
    }

    private Dictionary<AbilityId, Ability> CreateAbilities()
    {
        var abilities = new Dictionary<AbilityId, Ability>();

        // Combine all partial ability dictionaries
        foreach (var kvp in CreateAbilitiesAbc())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesDef())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesGhi())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesJkl())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesMno())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesPqr())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesStu())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesVwx())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesYz())
            abilities[kvp.Key] = kvp.Value;

        return abilities;
    }

    // Partial methods to be implemented in other files
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesAbc();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesDef();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesGhi();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesJkl();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesMno();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesPqr();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesStu();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesVwx();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesYz();
}
