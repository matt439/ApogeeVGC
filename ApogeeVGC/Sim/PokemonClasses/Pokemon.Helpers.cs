using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    public class PokemonDetails
    {
        public SpecieId Id { get; init; }
        public int Level { get; init; }
        public GenderId Gender { get; init; }
        public bool Shiny { get; init; }

        private MoveType? _teraType;
        public MoveType? TeraType
        {
            get => _teraType;
            set
            {
                _teraType = value;
                _cachedString = null; // invalidate cache when TeraType changes
            }
        }

        /// <summary>
        /// Cached base string (everything except tera suffix). Built once since Id, Level, Gender, Shiny are init-only.
        /// </summary>
        private string? _cachedBaseString;

        /// <summary>
        /// Cached full ToString result. Invalidated when TeraType changes.
        /// </summary>
        private string? _cachedString;

        /// <summary>
        /// Builds the base string from init-only properties (Id, Level, Gender, Shiny).
        /// Called once and cached for the lifetime of this instance.
        /// </summary>
        private string GetBaseString()
        {
            if (_cachedBaseString is not null) return _cachedBaseString;

            var parts = new List<string> { Id.ToString() };

            if (Level != 100)
            {
                parts.Add($"L{Level}");
            }

            if (Gender != GenderId.N)
            {
                parts.Add(Gender.ToString());
            }

            if (Shiny)
            {
                parts.Add("shiny");
            }

            _cachedBaseString = string.Join(", ", parts);
            return _cachedBaseString;
        }

        /// <summary>
        /// Converts PokemonDetails to the string format used by battle protocol.
        /// Format: "SpeciesName, L##, Gender, shiny, tera:Type"
        /// Caches the result to avoid repeated Enum.ToString() reflection calls.
        /// </summary>
        public override string ToString()
        {
            if (_cachedString is not null) return _cachedString;

            string baseStr = GetBaseString();
            _cachedString = _teraType != null
                ? $"{baseStr}, tera:{_teraType}"
                : baseStr;
            return _cachedString;
        }
    }

    public record MoveTargets
    {
        public required List<Pokemon> Targets { get; init; }
        public required List<Pokemon> PressureTargets { get; init; }
    }

    public static readonly HashSet<ItemId> RestorativeBerries =
    [
        ItemId.LeppaBerry,
        ItemId.AguavBerry,
        ItemId.EnigmaBerry,
        ItemId.FigyBerry,
        ItemId.IapapaBerry,
        ItemId.MagoBerry,
        ItemId.SitrusBerry,
    ];

    /// <summary>
    /// Creates a deep copy of this Pokemon on the given new side, and registers
    /// the mapping in pokemonMap. Pokemon references are deferred to Pass 2.
    /// </summary>
    internal Pokemon Copy(Side newSide, Dictionary<Pokemon, Pokemon> pokemonMap)
    {
        var copy = new Pokemon(newSide, this);
        pokemonMap[this] = copy;
        return copy;
    }

    /// <summary>
    /// Pass 2: Remaps all Pokemon references in this Pokemon's EffectStates,
    /// Illusion, and AttackedBy using the completed pokemonMap.
    /// Must be called after all Pokemon on both sides have been copied.
    /// </summary>
    internal void RemapPokemonReferences(Dictionary<Pokemon, Pokemon> pokemonMap, Pokemon source)
    {
        // Remap Illusion
        Illusion = EffectState.RemapPokemon(source.Illusion, pokemonMap);

        // Remap AttackedBy — create new Attacker records with remapped Source
        AttackedBy = source.AttackedBy
            .Select(a =>
            {
                var remappedSource = EffectState.RemapPokemon(a.Source, pokemonMap);
                return remappedSource is not null
                    ? a with { Source = remappedSource }
                    : null;
            })
            .Where(a => a is not null)
            .ToList()!;

        // Remap EffectState Pokemon references
        SpeciesState.RemapPokemonReferences(pokemonMap);
        StatusState.RemapPokemonReferences(pokemonMap);
        AbilityState.RemapPokemonReferences(pokemonMap);
        ItemState.RemapPokemonReferences(pokemonMap);
        foreach (var es in Volatiles.Values)
            es.RemapPokemonReferences(pokemonMap);
    }
}