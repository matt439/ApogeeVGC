using ApogeeVGC.Data;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Resolves user-typed names to enum IDs with case-insensitive and fuzzy matching.
/// Built from the Library data at startup.
/// </summary>
public sealed class NameResolver
{
    private readonly Dictionary<string, SpecieId> _speciesByName;
    private readonly Dictionary<string, MoveId> _movesByName;
    private readonly Dictionary<string, AbilityId> _abilitiesByName;
    private readonly Dictionary<string, ItemId> _itemsByName;

    // For autocomplete suggestions
    public IReadOnlyCollection<string> SpeciesNames => _speciesByName.Keys;
    public IReadOnlyCollection<string> MoveNames => _movesByName.Keys;
    public IReadOnlyCollection<string> AbilityNames => _abilitiesByName.Keys;
    public IReadOnlyCollection<string> ItemNames => _itemsByName.Keys;

    public NameResolver(Library library)
    {
        _speciesByName = new Dictionary<string, SpecieId>(StringComparer.OrdinalIgnoreCase);
        foreach (var (id, species) in library.Species)
        {
            _speciesByName.TryAdd(species.Name, id);
        }

        _movesByName = new Dictionary<string, MoveId>(StringComparer.OrdinalIgnoreCase);
        foreach (var (id, move) in library.Moves)
        {
            _movesByName.TryAdd(move.Name, id);
        }

        _abilitiesByName = new Dictionary<string, AbilityId>(StringComparer.OrdinalIgnoreCase);
        foreach (var (id, ability) in library.Abilities)
        {
            _abilitiesByName.TryAdd(ability.Name, id);
        }

        _itemsByName = new Dictionary<string, ItemId>(StringComparer.OrdinalIgnoreCase);
        foreach (var (id, item) in library.Items)
        {
            _itemsByName.TryAdd(item.Name, id);
        }
    }

    public bool TryResolveSpecies(string input, out SpecieId result)
    {
        if (_speciesByName.TryGetValue(input.Trim(), out result))
            return true;

        // Fuzzy: find best substring match
        return TryFuzzyMatch(input, _speciesByName, out result);
    }

    public bool TryResolveMove(string input, out MoveId result)
    {
        if (_movesByName.TryGetValue(input.Trim(), out result))
            return true;

        return TryFuzzyMatch(input, _movesByName, out result);
    }

    public bool TryResolveAbility(string input, out AbilityId result)
    {
        if (_abilitiesByName.TryGetValue(input.Trim(), out result))
            return true;

        return TryFuzzyMatch(input, _abilitiesByName, out result);
    }

    public bool TryResolveItem(string input, out ItemId result)
    {
        if (_itemsByName.TryGetValue(input.Trim(), out result))
            return true;

        return TryFuzzyMatch(input, _itemsByName, out result);
    }

    public string GetSpeciesName(SpecieId id, Library library) =>
        library.Species.TryGetValue(id, out var s) ? s.Name : id.ToString();

    public string GetMoveName(MoveId id, Library library) =>
        library.Moves.TryGetValue(id, out var m) ? m.Name : id.ToString();

    public string GetAbilityName(AbilityId id, Library library) =>
        library.Abilities.TryGetValue(id, out var a) ? a.Name : id.ToString();

    public string GetItemName(ItemId id, Library library) =>
        library.Items.TryGetValue(id, out var i) ? i.Name : id.ToString();

    /// <summary>
    /// Fuzzy match: case-insensitive contains, then shortest match wins.
    /// </summary>
    private static bool TryFuzzyMatch<T>(string input, Dictionary<string, T> dict, out T result)
    {
        var trimmed = input.Trim();
        T? best = default;
        int bestLen = int.MaxValue;
        bool found = false;

        foreach (var (name, value) in dict)
        {
            if (name.Contains(trimmed, StringComparison.OrdinalIgnoreCase) && name.Length < bestLen)
            {
                best = value;
                bestLen = name.Length;
                found = true;
            }
        }

        result = best!;
        return found;
    }

    /// <summary>
    /// Get suggestions matching a partial input (for display to user).
    /// </summary>
    public List<string> GetSpeciesSuggestions(string partial, int maxResults = 5)
        => GetSuggestions(partial, _speciesByName.Keys, maxResults);

    public List<string> GetMoveSuggestions(string partial, int maxResults = 5)
        => GetSuggestions(partial, _movesByName.Keys, maxResults);

    private static List<string> GetSuggestions(string partial, IEnumerable<string> names, int maxResults)
    {
        var trimmed = partial.Trim();
        if (string.IsNullOrEmpty(trimmed)) return [];

        return names
            .Where(n => n.Contains(trimmed, StringComparison.OrdinalIgnoreCase))
            .OrderBy(n => !n.StartsWith(trimmed, StringComparison.OrdinalIgnoreCase)) // prefix matches first
            .ThenBy(n => n.Length)
            .Take(maxResults)
            .ToList();
    }
}
