using ApogeeVGC_CS.sim;

namespace ApogeeVGC_CS.data
{
    public class TagData
    {
        public required string Name { get; init; }
        public string? Desc { get; init; }
        public Func<Species, bool>? SpeciesFilter { get; init; }
        public Func<Move, bool>? MoveFilter { get; init; }
        public Func<object, bool>? GenericFilter { get; init; }
        public Func<Species, int>? SpeciesNumCol { get; init; }
        public Func<Move, int>? MoveNumCol { get; init; }
        public Func<object, int>? GenericNumCol { get; init; }
    }

    public static class Tags
    {
        public static Dictionary<IdEntry, TagData> TagsData { get; } = new()
        {
            // Categories
            // ----------
            [new IdEntry("physical")] = new TagData
            {
                Name = "Physical",
                Desc = "Move deals damage with the Attack and Defense stats.",
                MoveFilter = move => move.Category == MoveCategory.Physical,
            },
            [new IdEntry("special")] = new TagData
            {
                Name = "Special",
                Desc = "Move deals damage with the Special Attack and Special Defense stats.",
                MoveFilter = move => move.Category == MoveCategory.Special,
            },
            [new IdEntry("status")] = new TagData
            {
                Name = "Status",
                Desc = "Move does not deal damage.",
                MoveFilter = move => move.Category == MoveCategory.Status,
            },

            // Pokemon tags
            // ------------
            [new IdEntry("mega")] = new TagData
            {
                Name = "Mega",
                SpeciesFilter = species => species.IsMega == true,
            },
            [new IdEntry("mythical")] = new TagData
            {
                Name = "Mythical",
                SpeciesFilter = species => species.Tags.Contains(SpeciesTag.Mythical),
            },
            [new IdEntry("sublegendary")] = new TagData
            {
                Name = "Sub-Legendary",
                SpeciesFilter = species => species.Tags.Contains(SpeciesTag.SubLegendary),
            },
            [new IdEntry("restrictedlegendary")] = new TagData
            {
                Name = "Restricted Legendary",
                SpeciesFilter = species => species.Tags.Contains(SpeciesTag.RestrictedLegendary),
            },
            [new IdEntry("ultrabeast")] = new TagData
            {
                Name = "Ultra Beast",
                SpeciesFilter = species => species.Tags.Contains(SpeciesTag.UltraBeast),
            },
            [new IdEntry("paradox")] = new TagData
            {
                Name = "Paradox",
                SpeciesFilter = species => species.Tags.Contains(SpeciesTag.Paradox),
            },

            // Move tags
            // ---------
            [new IdEntry("zmove")] = new TagData
            {
                Name = "Z-Move",
                MoveFilter = move => move.IsZ != null && (bool?)move.IsZ == true,
            },
            [new IdEntry("maxmove")] = new TagData
            {
                Name = "Max Move",
                MoveFilter = move => move.IsMax != null && (bool?)move.IsMax == true,
            },
            [new IdEntry("contact")] = new TagData
            {
                Name = "Contact",
                Desc = "Affected by a variety of moves, abilities, and items. Moves affected by contact moves include: Spiky Shield, King's Shield. Abilities affected by contact moves include: Iron Barbs, Rough Skin, Gooey, Flame Body, Static, Tough Claws. Items affected by contact moves include: Rocky Helmet, Sticky Barb.",
                MoveFilter = move => move.Flags.Contact == true,
            },
            [new IdEntry("sound")] = new TagData
            {
                Name = "Sound",
                Desc = "Doesn't affect Soundproof Pokémon. (All sound moves also bypass Substitute.)",
                MoveFilter = move => move.Flags.Sound == true,
            },
            [new IdEntry("powder")] = new TagData
            {
                Name = "Powder",
                Desc = "Doesn't affect Grass-type Pokémon, Overcoat Pokémon, or Safety Goggles holders.",
                MoveFilter = move => move.Flags.Powder == true,
            },
            [new IdEntry("fist")] = new TagData
            {
                Name = "Fist",
                Desc = "Boosted 1.2x by Iron Fist.",
                MoveFilter = move => move.Flags.Punch == true,
            },
            [new IdEntry("pulse")] = new TagData
            {
                Name = "Pulse",
                Desc = "Boosted 1.5x by Mega Launcher.",
                MoveFilter = move => move.Flags.Pulse == true,
            },
            [new IdEntry("bite")] = new TagData
            {
                Name = "Bite",
                Desc = "Boosted 1.5x by Strong Jaw.",
                MoveFilter = move => move.Flags.Bite == true,
            },
            [new IdEntry("ballistic")] = new TagData
            {
                Name = "Ballistic",
                Desc = "Doesn't affect Bulletproof Pokémon.",
                MoveFilter = move => move.Flags.Bullet == true,
            },
            [new IdEntry("bypassprotect")] = new TagData
            {
                Name = "Bypass Protect",
                Desc = "Bypasses Protect, Detect, King's Shield, and Spiky Shield.",
                MoveFilter = move => move.Target != MoveTarget.Self && move.Flags.Protect != true,
            },
            [new IdEntry("nonreflectable")] = new TagData
            {
                Name = "Nonreflectable",
                Desc = "Can't be bounced by Magic Coat or Magic Bounce.",
                MoveFilter = move => move.Target != MoveTarget.Self && move.Category == MoveCategory.Status && move.Flags.Reflectable != true,
            },
            [new IdEntry("nonmirror")] = new TagData
            {
                Name = "Nonmirror",
                Desc = "Can't be copied by Mirror Move.",
                MoveFilter = move => move.Target != MoveTarget.Self && move.Flags.Mirror != true,
            },
            [new IdEntry("nonsnatchable")] = new TagData
            {
                Name = "Nonsnatchable",
                Desc = "Can't be copied by Snatch.",
                MoveFilter = move => (move.Target == MoveTarget.AllyTeam || move.Target == MoveTarget.Self || move.Target == MoveTarget.AdjacentAllyOrSelf) && move.Flags.Snatch != true,
            },
            [new IdEntry("bypasssubstitute")] = new TagData
            {
                Name = "Bypass Substitute",
                Desc = "Bypasses but does not break a Substitute.",
                MoveFilter = move => move.Flags.BypassSub == true,
            },
            [new IdEntry("gmaxmove")] = new TagData
            {
                Name = "G-Max Move",
                MoveFilter = move => move.IsMax is string,
            },

            // Tiers
            // -----
            [new IdEntry("uber")] = new TagData
            {
                Name = "Uber",
                SpeciesFilter = species => species.Tier == Tier.Uber || species.Tier == Tier.UberAlt || species.Tier == Tier.Ag,
            },
            [new IdEntry("ou")] = new TagData
            {
                Name = "OU",
                SpeciesFilter = species => species.Tier == Tier.Ou || species.Tier == Tier.OuAlt,
            },
            [new IdEntry("uubl")] = new TagData
            {
                Name = "UUBL",
                SpeciesFilter = species => species.Tier == Tier.Uubl,
            },
            [new IdEntry("uu")] = new TagData
            {
                Name = "UU",
                SpeciesFilter = species => species.Tier == Tier.Uu,
            },
            [new IdEntry("rubl")] = new TagData
            {
                Name = "RUBL",
                SpeciesFilter = species => species.Tier == Tier.Rubl,
            },
            [new IdEntry("ru")] = new TagData
            {
                Name = "RU",
                SpeciesFilter = species => species.Tier == Tier.Ru,
            },
            [new IdEntry("nubl")] = new TagData
            {
                Name = "NUBL",
                SpeciesFilter = species => species.Tier == Tier.Nubl,
            },
            [new IdEntry("nu")] = new TagData
            {
                Name = "NU",
                SpeciesFilter = species => species.Tier == Tier.Nu,
            },
            [new IdEntry("publ")] = new TagData
            {
                Name = "PUBL",
                SpeciesFilter = species => species.Tier == Tier.Publ,
            },
            [new IdEntry("pu")] = new TagData
            {
                Name = "PU",
                SpeciesFilter = species => species.Tier == Tier.Pu || species.Tier == Tier.NuAlt,
            },
            [new IdEntry("zubl")] = new TagData
            {
                Name = "ZUBL",
                SpeciesFilter = species => species.Tier == Tier.Zubl,
            },
            [new IdEntry("zu")] = new TagData
            {
                Name = "ZU",
                SpeciesFilter = species => species.Tier == Tier.PuAlt || species.Tier == Tier.Zu,
            },
            [new IdEntry("nfe")] = new TagData
            {
                Name = "NFE",
                SpeciesFilter = species => species.Tier == Tier.Nfe,
            },
            [new IdEntry("lc")] = new TagData
            {
                Name = "LC",
                SpeciesFilter = species => species.DoublesTier == Tier.Lc,
            },
            [new IdEntry("captier")] = new TagData
            {
                Name = "CAP Tier",
                SpeciesFilter = species => species.IsNonstandard == Nonstandard.Cap,
            },
            [new IdEntry("caplc")] = new TagData
            {
                Name = "CAP LC",
                SpeciesFilter = species => species.Tier == Tier.CapLc,
            },
            [new IdEntry("capnfe")] = new TagData
            {
                Name = "CAP NFE",
                SpeciesFilter = species => species.Tier == Tier.CapNfe,
            },
            [new IdEntry("ag")] = new TagData
            {
                Name = "AG",
                SpeciesFilter = species => species.Tier == Tier.Ag,
            },

            // Doubles tiers
            // -------------
            [new IdEntry("duber")] = new TagData
            {
                Name = "DUber",
                SpeciesFilter = species => species.DoublesTier == Tier.DUber || species.DoublesTier == Tier.DUberAlt,
            },
            [new IdEntry("dou")] = new TagData
            {
                Name = "DOU",
                SpeciesFilter = species => species.DoublesTier == Tier.Dou || species.DoublesTier == Tier.DouAlt,
            },
            [new IdEntry("dbl")] = new TagData
            {
                Name = "DBL",
                SpeciesFilter = species => species.DoublesTier == Tier.Dbl,
            },
            [new IdEntry("duu")] = new TagData
            {
                Name = "DUU",
                SpeciesFilter = species => species.DoublesTier == Tier.Duu,
            },
            [new IdEntry("dnu")] = new TagData
            {
                Name = "DNU",
                SpeciesFilter = species => species.DoublesTier == Tier.DuuAlt,
            },

            // Nat Dex tiers
            // -------------
            [new IdEntry("ndag")] = new TagData
            {
                Name = "ND AG",
                SpeciesFilter = species => species.NatDexTier == Tier.Ag,
            },
            [new IdEntry("nduber")] = new TagData
            {
                Name = "ND Uber",
                SpeciesFilter = species => species.NatDexTier == Tier.Uber || species.NatDexTier == Tier.UberAlt,
            },
            [new IdEntry("ndou")] = new TagData
            {
                Name = "ND OU",
                SpeciesFilter = species => species.NatDexTier == Tier.Ou || species.NatDexTier == Tier.OuAlt,
            },
            [new IdEntry("nduubl")] = new TagData
            {
                Name = "ND UUBL",
                SpeciesFilter = species => species.NatDexTier == Tier.Uubl,
            },
            [new IdEntry("nduu")] = new TagData
            {
                Name = "ND UU",
                SpeciesFilter = species => species.NatDexTier == Tier.Uu,
            },
            [new IdEntry("ndrubl")] = new TagData
            {
                Name = "ND RUBL",
                SpeciesFilter = species => species.NatDexTier == Tier.Rubl,
            },
            [new IdEntry("ndru")] = new TagData
            {
                Name = "ND RU",
                SpeciesFilter = species => species.NatDexTier == Tier.Ru,
            },
            [new IdEntry("ndnfe")] = new TagData
            {
                Name = "ND NFE",
                SpeciesFilter = species => species.NatDexTier == Tier.Nfe,
            },
            [new IdEntry("ndlc")] = new TagData
            {
                Name = "ND LC",
                SpeciesFilter = species => species.NatDexTier == Tier.Lc,
            },

            // Legality tags
            [new IdEntry("past")] = new TagData
            {
                Name = "Past",
                GenericFilter = thing => (thing as IBasicEffect)?.IsNonstandard == Nonstandard.Past,
            },
            [new IdEntry("future")] = new TagData
            {
                Name = "Future",
                GenericFilter = thing => (thing as IBasicEffect)?.IsNonstandard == Nonstandard.Future,
            },
            [new IdEntry("lgpe")] = new TagData
            {
                Name = "LGPE",
                GenericFilter = thing => (thing as IBasicEffect)?.IsNonstandard == Nonstandard.Lgpe,
            },
            [new IdEntry("unobtainable")] = new TagData
            {
                Name = "Unobtainable",
                GenericFilter = thing => (thing as IBasicEffect)?.IsNonstandard == Nonstandard.Unobtainable,
            },
            [new IdEntry("cap")] = new TagData
            {
                Name = "CAP",
                SpeciesFilter = species => species.IsNonstandard == Nonstandard.Cap,
            },
            [new IdEntry("custom")] = new TagData
            {
                Name = "Custom",
                GenericFilter = thing => (thing as IBasicEffect)?.IsNonstandard == Nonstandard.Custom,
            },
            [new IdEntry("nonexistent")] = new TagData
            {
                Name = "Nonexistent",
                GenericFilter = thing => (thing as IBasicEffect)?.IsNonstandard != null && (thing as IBasicEffect)?.IsNonstandard != Nonstandard.Unobtainable,
            },

            // Filter columns
            // --------------
            [new IdEntry("introducedgen")] = new TagData
            {
                Name = "Introduced Gen",
                GenericNumCol = thing => (thing as IBasicEffect)?.Gen ?? 0,
            },

            [new IdEntry("height")] = new TagData
            {
                Name = "Height",
                SpeciesNumCol = species => (int)(species.HeightM * 10), // Convert to decimeters for integer sorting
            },
            [new IdEntry("weight")] = new TagData
            {
                Name = "Weight",
                SpeciesNumCol = species => (int)species.WeightKg,
            },
            [new IdEntry("hp")] = new TagData
            {
                Name = "HP",
                Desc = "Hit Points",
                SpeciesNumCol = species => species.BaseStats.Hp,
            },
            [new IdEntry("atk")] = new TagData
            {
                Name = "Atk",
                Desc = "Attack",
                SpeciesNumCol = species => species.BaseStats.Atk,
            },
            [new IdEntry("def")] = new TagData
            {
                Name = "Def",
                Desc = "Defense",
                SpeciesNumCol = species => species.BaseStats.Def,
            },
            [new IdEntry("spa")] = new TagData
            {
                Name = "SpA",
                Desc = "Special Attack",
                SpeciesNumCol = species => species.BaseStats.Spa,
            },
            [new IdEntry("spd")] = new TagData
            {
                Name = "SpD",
                Desc = "Special Defense",
                SpeciesNumCol = species => species.BaseStats.Spd,
            },
            [new IdEntry("spe")] = new TagData
            {
                Name = "Spe",
                Desc = "Speed",
                SpeciesNumCol = species => species.BaseStats.Spe,
            },
            [new IdEntry("bst")] = new TagData
            {
                Name = "BST",
                Desc = "Base Stat Total",
                SpeciesNumCol = species => species.Bst,
            },

            [new IdEntry("basepower")] = new TagData
            {
                Name = "Base Power",
                MoveNumCol = move => move.BasePower,
            },
            [new IdEntry("priority")] = new TagData
            {
                Name = "Priority",
                MoveNumCol = move => move.Priority,
            },
            [new IdEntry("accuracy")] = new TagData
            {
                Name = "Accuracy",
                MoveNumCol = move => move.Accuracy is true ? 101 : (move.Accuracy as int? ?? 0),
            },
            [new IdEntry("maxpp")] = new TagData
            {
                Name = "Max PP",
                MoveNumCol = move => move.Pp,
            },
        };
    }
}
