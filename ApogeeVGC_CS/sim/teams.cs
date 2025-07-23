using System.Collections.Generic;

namespace ApogeeVGC_CS.sim
{
    public class PokemonSet
    {
        // Nickname, defaults to base species if not specified
        public string Name { get; set; } = string.Empty;
        // Species name (including forme if applicable)
        public string Species { get; set; } = string.Empty;
        // Item (id or full name)
        public string Item { get; set; } = string.Empty;
        // Ability (id or full name)
        public string Ability { get; set; } = string.Empty;
        // Moves (ids or full names)
        public List<string> Moves { get; set; } = new();
        // Nature (id or full name)
        public string Nature { get; set; } = string.Empty;
        // Gender
        public string Gender { get; set; } = string.Empty;
        // Effort Values (0-255)
        public StatsTable Evs { get; set; } = new();
        // Individual Values (0-31)
        public StatsTable Ivs { get; set; } = new();
        // Level (1-9999)
        public int Level { get; set; }
        // Optional: shiny
        public bool? Shiny { get; set; }
        // Optional: happiness (friendship, 0-255)
        public int? Happiness { get; set; }
        // Optional: pokeball
        public string? Pokeball { get; set; }
        // Optional: Hidden Power type
        public string? HpType { get; set; }
        // Optional: Dynamax Level (0-10)
        public int? DynamaxLevel { get; set; }
        // Optional: Gigantamax
        public bool? Gigantamax { get; set; }
        // Optional: Tera Type
        public string? TeraType { get; set; }
    }

    public class Teams { }
}