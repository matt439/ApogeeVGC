using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    public class PokemonDetails
    {
        public SpecieId Id { get; init; }
        public int Level { get; init; }
        public GenderId Gender { get; init; }
        public bool Shiny { get; init; }
        public MoveType? TeraType { get; set; }

        /// <summary>
        /// Converts PokemonDetails to the string format used by battle protocol.
        /// Format: "SpeciesName, L##, Gender, shiny, tera:Type"
        /// </summary>
        public override string ToString()
        {
            var parts = new List<string> {
                // Add species name
                Id.ToString() };

            // Add level (omit L100 as it's the default)
            if (Level != 100)
            {
                parts.Add($"L{Level}");
            }

            // Add gender (omit if genderless/unknown)
            if (Gender != GenderId.N)
            {
                parts.Add(Gender.ToString());
            }

            // Add shiny indicator
            if (Shiny)
            {
                parts.Add("shiny");
            }

            // Add Tera type if present
            if (TeraType != null)
            {
                parts.Add($"tera:{TeraType}");
            }

            return string.Join(", ", parts);
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

    public Pokemon Copy()
    {
        throw new NotImplementedException();
    }
}