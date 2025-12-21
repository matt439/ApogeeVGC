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
        foreach (var kvp in CreateAbilitiesABC())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesDEF())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesGHI())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesJKL())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesMNO())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesPQR())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesSTU())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesVWX())
            abilities[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateAbilitiesYZ())
            abilities[kvp.Key] = kvp.Value;

        return abilities;
    }

    // Partial methods to be implemented in other files
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesABC();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesDEF();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesGHI();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesJKL();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesMNO();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesPQR();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesSTU();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesVWX();
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesYZ();
}
