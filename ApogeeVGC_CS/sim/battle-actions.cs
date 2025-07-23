using System;
using System.Collections.Generic;

namespace ApogeeVGC_CS.sim
{
    public static class ChoosableTargets
    {
        public static readonly HashSet<string> Targets = new()
        {
            "normal", "any", "adjacentAlly", "adjacentAllyOrSelf", "adjacentFoe"
        };
    }

    public class BattleActions
    {
        public Battle Battle { get; }
        public ModdedDex Dex { get; }

        public static readonly Dictionary<string, string> MaxMoves = new()
        {
            ["Flying"] = "Max Airstream",
            ["Dark"] = "Max Darkness",
            ["Fire"] = "Max Flare",
            ["Bug"] = "Max Flutterby",
            ["Water"] = "Max Geyser",
            ["Status"] = "Max Guard",
            ["Ice"] = "Max Hailstorm",
            ["Fighting"] = "Max Knuckle",
            ["Electric"] = "Max Lightning",
            ["Psychic"] = "Max Mindstorm",
            ["Poison"] = "Max Ooze",
            ["Grass"] = "Max Overgrowth",
            ["Ghost"] = "Max Phantasm",
            ["Ground"] = "Max Quake",
            ["Rock"] = "Max Rockfall",
            ["Fairy"] = "Max Starfall",
            ["Steel"] = "Max Steelspike",
            ["Normal"] = "Max Strike",
            ["Dragon"] = "Max Wyrmwind"
        };

        public static readonly Dictionary<string, string> ZMoves = new()
        {
            ["Poison"] = "Acid Downpour",
            ["Fighting"] = "All-Out Pummeling",
            ["Dark"] = "Black Hole Eclipse",
            ["Grass"] = "Bloom Doom",
            ["Normal"] = "Breakneck Blitz",
            ["Rock"] = "Continental Crush",
            ["Steel"] = "Corkscrew Crash",
            ["Dragon"] = "Devastating Drake",
            ["Electric"] = "Gigavolt Havoc",
            ["Water"] = "Hydro Vortex",
            ["Fire"] = "Inferno Overdrive",
            ["Ghost"] = "Never-Ending Nightmare",
            ["Bug"] = "Savage Spin-Out",
            ["Psychic"] = "Shattered Psyche",
            ["Ice"] = "Subzero Slammer",
            ["Flying"] = "Supersonic Skystrike",
            ["Ground"] = "Tectonic Rage",
            ["Fairy"] = "Twinkle Tackle"
        };

        public BattleActions(Battle battle)
        {
            // TODO
        }
    }
}