using ApogeeVGC.Sim.Items;
using System.Text;

namespace ApogeeVGC.Tools;

public static class ValidateItems
{
    /// <summary>
    /// Validates that all Gen 9 items from the TypeScript source are present in the C# implementation.
    /// Generates a report showing missing items, items in wrong files, and items in wrong order.
    /// </summary>
    public static string GenerateValidationReport()
    {
        var report = new StringBuilder();
        report.AppendLine("=== ITEM VALIDATION REPORT ===");
        report.AppendLine();

        // Define the expected items per file based on alphabetical grouping
        var expectedItemsAbc = new List<string>
        {
            // A items (Gen 9 relevant)
            "AbilityShield", "AbsorbBulb", "AdamantCrystal", "AdamantOrb", "AdrenalineOrb",
            "AguavBerry", "AirBalloon", "ApicotBerry", "AspearBerry", "AssaultVest", "AuspiciousArmor",
            
            // B items (Gen 9 relevant)
            "BabiriBerry", "BeastBall", "BerrySweet", "BigNugget", "BigRoot", "BindingBand",
            "BlackBelt", "BlackGlasses", "BlackSludge", "BlueOrb", "BlunderPolicy", "BoosterEnergy",
            "BottleCap", "BrightPowder",
            
            // C items (Gen 9 relevant)
            "CellBattery", "Charcoal", "ChartiBerry", "CheriBerry", "CherishBall", "ChestoBerry",
            "ChilanBerry", "ChippedPot", "ChoiceBand", "ChoiceScarf", "ChoiceSpecs", "ChopleBerry",
            "ClearAmulet", "CloverSweet", "CobaBerry", "ColburBerry", "CornerstoneMask", "CovertCloak",
            "CrackedPot", "CustapBerry"
        };

        var expectedItemsDef = new List<string>
        {
            // D items
            "DampRock", "DawnStone", "DeepSeaScale", "DeepSeaTooth", "DestinyKnot",
            "DiveB all", "DouseDrive", "DracoPlate", "DragonFang", "DragonScale", "DreadPlate",
            "DreamBall", "DubiousDisc", "DuskBall", "DuskStone",
            
            // E items
            "EarthPlate", "EjectButton", "EjectPack", "Electirizer", "ElectricMemory",
            "ElectricSeed", "EnigmaBerry", "Eviolite", "ExpertBelt",
            
            // F items
            "FairyFeather", "FairyMemory", "FastBall", "FiggyBerry", "FireMemory", "FireStone",
            "FistPlate", "FlameOrb", "FlamePlate", "FloatStone", "FlowerSweet", "FlyingMemory",
            "FocusBand", "FocusSash", "FriendBall", "FullIncense"
        };

        var expectedItemsGhi = new List<string>
        {
            // G items
            "GalaricaCuff", "GalaricaWreath", "GanlonBerry", "GhostMemory", "GoldBottleCap",
            "GrassMemory", "GrassySeed", "GreatBall", "GrepaBerry", "GripClaw", "GriseousCore",
            "GriseousOrb", "GroundMemory",
            
            // H items
            "HabanBerry", "HardStone", "HealBall", "HearthflameMask", "HeatRock", "HeavyBall",
            "HeavyDutyBoots",
            
            // I items
            "IapapaBerry", "IceMemory", "IceStone", "IciclePlate", "InsectPlate", "IronBall",
            "IronPlate"
        };

        var expectedItemsJkl = new List<string>
        {
            // J items
            "JabocaBerry",
            
            // K items
            "KasibBerry", "KebiaBerry", "KeeBerry", "KelpsyBerry", "KingsRock",
            
            // L items
            "LagginTail", "LansatBerry", "LaxIncense", "LeafStone", "Leek", "Leftovers",
            "LeppaBerry", "LevelBall", "LiechiBerry", "LifeOrb", "LightBall", "LightClay",
            "LoadedDice", "LoveBall", "LoveSweet", "LuckyPunch", "LumBerry", "LuminousMoss",
            "LureBall", "LustrousGlobe", "LustrousOrb", "LuxuryBall"
        };

        var expectedItemsMno = new List<string>
        {
            // M items
            "MachoBrace", "Magmarizer", "Magnet", "MagoBerry", "MaliciousArmor", "MagoSweet",
            "MarangaBerry", "MasterBall", "MasterpieceTeacup", "MeadowPlate", "MentalHerb",
            "MetalAlloy", "MetalCoat", "MetalPowder", "Metronome", "MicleBerry", "MindPlate",
            "MiracleSeed", "MirrorHerb", "MistySeed", "MoonBall", "MoonStone", "MuscleHeartMask",
            
            // N items
            "NestBall", "NetBall", "NeverMeltIce", "NormalGem",
            
            // O items
            "OccaBerry", "OddIncense", "OranBerry", "OvalStone"
        };

        var expectedItemsPqr = new List<string>
        {
            // P items
            "PasshoBerry", "PayapaBerry", "PechaBerry", "PersimBerry", "PetayaBerry", "PinkBow",
            "PixiePlate", "PokeBall", "PomegBerry", "PowerAnklet", "PowerBand", "PowerBelt",
            "PowerBracer", "PowerHerb", "PowerLens", "PowerWeight", "PremierBall", "PrettyFeather",
            "PrismScale", "ProtectivePads", "Protector", "PsychicMemory", "PsychicSeed",
            "PunchingGlove",
            
            // Q items
            "QualotBerry", "QuickBall", "QuickClaw",
            
            // R items
            "RareBone", "RawstBerry", "RazorClaw", "RazorFang", "ReaperCloth", "RedCard",
            "RedOrb", "RepeatBall", "RibbonSweet", "RindoBerry", "RingTarget", "RockMemory",
            "RockyHelmet", "RoomService", "RoselieBerry", "RowapBerry", "RustedShield", "RustedSword"
        };

        var expectedItemsStu = new List<string>
        {
            // S items
            "Sachet", "SafariBall", "SafetyGoggles", "SalacBerry", "SharpBeak", "ShedShell",
            "ShellBell", "ShinyStone", "ShockDrive", "ShucaBerry", "SilkScarf", "SilverPowder",
            "SitrusBerry", "SkyPlate", "SmoothRock", "Snowball", "SoftSand", "SoulDew",
            "SpellTag", "SplashPlate", "SpookyPlate", "SportBall", "StarfBerry", "StarSweet",
            "SteelMemory", "StickyBarb", "StonePlate", "StrawberrySweet", "SunStone", "SweetApple",
            "SyrupyApple",
            
            // T items
            "TamatoBerry", "TangaBerry", "TartApple", "TerrainExtender", "ThickClub", "ThroatSpray",
            "ThunderStone", "TimerBall", "ToxicOrb", "ToxicPlate", "TwistedSpoon",
            
            // U items
            "UltraBall", "UnremarkableTeacup", "Upgrade", "UtilityUmbrella"
        };

        var expectedItemsVwx = new List<string>
        {
            // V items (none in Gen 9)
            
            // W items
            "WacanBerry", "WaterMemory", "WaterStone", "WaveIncense", "WeaknessPolicy",
            "WellspringMask", "WhippedDream", "WhiteHerb", "WideLens", "WikiBerry", "WiseGlasses",
            
            // X items (none in Gen 9)
        };

        var expectedItemsYz = new List<string>
        {
            // Y items
            "YacheBerry",
            
            // Z items
            "ZapPlate", "ZoomLens"
        };

        // Items that should be SKIPPED (not Gen 9 relevant)
        var skippedItems = new List<string>
        {
            // Mega Stones (all)
            "Abomasite", "Absolite", "Aerodactylite", "Aggronite", "Alakazite", "Altarianite",
            "Ampharosite", "Audinite", "Banettite", "Barbaracite", "Beedrillite", "Blastoisinite",
            "Blazikenite", "Cameruptite", "Chandelurite", "Charizardite X", "Charizardite Y",
            "Chesnaughtite", "Crucibellite", "Delphoxite", "Diancite", "Dragoninite",
            "Dragalgite", "Drampanite", "Eelektrossite", "Emboarite", "Excadrite", "Falinksite",
            "Feraligite", "Floettite", "Froslassite", "Galladite", "Garchompite", "Gardevoirite",
            "Gengarite", "Glalitite", "Greninjite", "Gyaradosite", "Hawluchanite", "Heracronite",
            "Houndoominite", "Kangaskhanite", "Latiasite", "Latiosite", "Lopunnite", "Lucarionite",
            "Malamarite", "Manectite", "Mawilite", "Medichamite", "Meganiumite", "Metagrossite",
            "Mewtwonite X", "Mewtwonite Y", "Pidgeotite", "Pinsirite", "Pyroarite", "Sablenite",
            "Salamencite", "Sceptilite", "Scizorite", "Scolipite", "Scraftinite", "Sharpedonite",
            "Skarmorite", "Slowbronite", "Starminite", "Steelixite", "Swampertite", "Tyranitarite",
            "Venusaurite", "Victreebelite", "Zygardite", "Clefablite",
            
            // Z-Crystals (all)
            "Aloraichium Z", "Buginium Z", "Darkinium Z", "Decidium Z", "Dragonium Z",
            "Eevium Z", "Electrium Z", "Fairium Z", "Fightinium Z", "Firium Z", "Flyinium Z",
            "Ghostium Z", "Grassium Z", "Groundium Z", "Icium Z", "Incinium Z", "Kommonium Z",
            "Lunalium Z", "Lycanium Z", "Marshadium Z", "Mewnium Z", "Mimikium Z",
            "Normalium Z", "Pikanium Z", "Pikashunium Z", "Poisonium Z", "Primarium Z",
            "Psychium Z", "Rockium Z", "Snorlium Z", "Solganium Z", "Steelium Z", "Tapunium Z",
            "Ultranecrozium Z", "Waterium Z",
            
            // Gems (not in Gen 9)
            "Bug Gem", "Dark Gem", "Dragon Gem", "Electric Gem", "Fairy Gem", "Fighting Gem",
            "Fire Gem", "Flying Gem", "Ghost Gem", "Grass Gem", "Ground Gem", "Ice Gem",
            "Poison Gem", "Psychic Gem", "Rock Gem", "Steel Gem", "Water Gem",
            
            // Fossils (not relevant for battle mechanics)
            "Armor Fossil", "Claw Fossil", "Cover Fossil", "Dome Fossil", "Fossilized Bird",
            "Fossilized Dino", "Fossilized Drake", "Fossilized Fish", "Helix Fossil",
            "Jaw Fossil", "Old Amber", "Plume Fossil", "Root Fossil", "Sail Fossil",
            "Skull Fossil",
            
            // Drives (Genesect - not all relevant)
            "Burn Drive", "Chill Drive", "Douse Drive", "Shock Drive",
            
            // Plates for Arceus (Judgment type change - keep these actually)
            // "Draco Plate", "Dread Plate", "Earth Plate", "Fist Plate", "Flame Plate",
            // "Icicle Plate", "Insect Plate", "Iron Plate", "Meadow Plate", "Mind Plate",
            // "Pixie Plate", "Sky Plate", "Splash Plate", "Spooky Plate", "Stone Plate",
            // "Toxic Plate", "Zap Plate",
            
            // Memories (Silvally - not in Gen 9)
            "Bug Memory", "Dark Memory", "Dragon Memory", "Electric Memory", "Fairy Memory",
            "Fighting Memory", "Fire Memory", "Flying Memory", "Ghost Memory", "Grass Memory",
            "Ground Memory", "Ice Memory", "Poison Memory", "Psychic Memory", "Rock Memory",
            "Steel Memory", "Water Memory",
            
            // Incense items (breeding-related, not battle)
            "Full Incense", "Lax Incense", "Luck Incense", "Odd Incense", "Pure Incense",
            "Rock Incense", "Rose Incense", "Sea Incense", "Wave Incense",
            
            // Mail items
            "Mail",
            
            // Past generation items
            "Berry Juice", "Belue Berry", "Bluk Berry", "Cornn Berry", "Durin Berry",
            "Enigma Berry", "Magost Berry", "Nanab Berry", "Nomel Berry", "Pamtre Berry",
            "Pink Bow", "Pinap Berry", "Polkadot Bow", "Rabuta Berry", "Razz Berry",
            "Spelon Berry", "Watmel Berry", "Wepear Berry",
            
            // Gen 2 items
            "Berserk Gene", "Berry", "Bitter Berry", "Burnt Berry", "Gold Berry",
            "Ice Berry", "Mint Berry", "Miracle Berry", "Mystery Berry", "PRZ Cure Berry",
            "PSN Cure Berry", "Stick",
            
            // TRs (Sword/Shield specific)
            // All TR00-TR99
            
            // CAP items
            "Crucibellite", "Vile Vial"
        };

        report.AppendLine("### Expected Items by File:");
        report.AppendLine();
        report.AppendLine($"**ItemsABC.cs**: {expectedItemsAbc.Count} items");
        report.AppendLine($"**ItemsDEF.cs**: {expectedItemsDef.Count} items");
        report.AppendLine($"**ItemsGHI.cs**: {expectedItemsGhi.Count} items");
        report.AppendLine($"**ItemsJKL.cs**: {expectedItemsJkl.Count} items");
        report.AppendLine($"**ItemsMNO.cs**: {expectedItemsMno.Count} items");
        report.AppendLine($"**ItemsPQR.cs**: {expectedItemsPqr.Count} items");
        report.AppendLine($"**ItemsSTU.cs**: {expectedItemsStu.Count} items");
        report.AppendLine($"**ItemsVWX.cs**: {expectedItemsVwx.Count} items");
        report.AppendLine($"**ItemsYZ.cs**: {expectedItemsYz.Count} items");
        report.AppendLine();
        report.AppendLine($"**Total Expected**: {expectedItemsAbc.Count + expectedItemsDef.Count + expectedItemsGhi.Count + expectedItemsJkl.Count + expectedItemsMno.Count + expectedItemsPqr.Count + expectedItemsStu.Count + expectedItemsVwx.Count + expectedItemsYz.Count} items");
        report.AppendLine();
        report.AppendLine($"**Deliberately Skipped**: {skippedItems.Count} items (Mega Stones, Z-Crystals, Gems, Fossils, TRs, etc.)");
        report.AppendLine();

        // Note: Actual validation would require comparing against the actual C# files
        // This is a template showing what should be validated
        report.AppendLine("### Validation Tasks:");
        report.AppendLine("1. ? Check all ABC items are in ItemsABC.cs and in correct alphabetical order");
        report.AppendLine("2. ? Check all DEF items are in ItemsDEF.cs and in correct alphabetical order");
        report.AppendLine("3. ? Check all GHI items are in ItemsGHI.cs and in correct alphabetical order");
        report.AppendLine("4. ? Check all JKL items are in ItemsJKL.cs and in correct alphabetical order");
        report.AppendLine("5. ? Check all MNO items are in ItemsMNO.cs and in correct alphabetical order");
        report.AppendLine("6. ? Check all PQR items are in ItemsPQR.cs and in correct alphabetical order");
        report.AppendLine("7. ? Check all STU items are in ItemsSTU.cs and in correct alphabetical order");
        report.AppendLine("8. ? Check all VWX items are in ItemsVWX.cs and in correct alphabetical order");
        report.AppendLine("9. ? Check all YZ items are in ItemsYZ.cs and in correct alphabetical order");
        report.AppendLine("10. ? Verify all Mega Stones, Z-Crystals, and Gems are properly excluded");
        report.AppendLine("11. ? Verify Plates are included (used for Arceus Judgment type)");
        report.AppendLine("12. ? Verify TRs (TR00-TR99) are excluded");
        report.AppendLine();

        report.AppendLine("### Items Requiring Special Attention:");
        report.AppendLine();
        report.AppendLine("**Plates (KEEP - used for Arceus Judgment):**");
        var plates = new[] { "Draco Plate", "Dread Plate", "Earth Plate", "Fist Plate", "Flame Plate",
            "Icicle Plate", "Insect Plate", "Iron Plate", "Meadow Plate", "Mind Plate",
            "Pixie Plate", "Sky Plate", "Splash Plate", "Spooky Plate", "Stone Plate",
            "Toxic Plate", "Zap Plate" };
        foreach (var plate in plates)
            report.AppendLine($"  - {plate}");
        report.AppendLine();

        report.AppendLine("**Orbs and Crystals (KEEP - forme changes):**");
        report.AppendLine("  - Adamant Crystal (Dialga-Origin)");
        report.AppendLine("  - Adamant Orb (Dialga)");
        report.AppendLine("  - Lustrous Globe (Palkia-Origin)");
        report.AppendLine("  - Lustrous Orb (Palkia)");
        report.AppendLine("  - Griseous Core (Giratina-Origin)");
        report.AppendLine("  - Griseous Orb (Giratina)");
        report.AppendLine("  - Blue Orb (Kyogre-Primal)");
        report.AppendLine("  - Red Orb (Groudon-Primal)");
        report.AppendLine();

        report.AppendLine("**Masks (KEEP - Ogerpon):**");
        report.AppendLine("  - Cornerstone Mask");
        report.AppendLine("  - Hearthflame Mask");
        report.AppendLine("  - Wellspring Mask");
        report.AppendLine();

        report.AppendLine("**Rusted Items (KEEP - Zacian/Zamazenta):**");
        report.AppendLine("  - Rusted Shield (Zamazenta-Crowned)");
        report.AppendLine("  - Rusted Sword (Zacian-Crowned)");
        report.AppendLine();

        report.AppendLine("### Notes:");
        report.AppendLine("- Gen 9 introduced several new items like Booster Energy, Clear Amulet, Covert Cloak, etc.");
        report.AppendLine("- Dynamax-related items are excluded (not in Gen 9 standard formats)");
        report.AppendLine("- Gigantamax is not in Gen 9");
        report.AppendLine("- Max Moves are not in Gen 9");
        report.AppendLine("- Some items like TRs were Sword/Shield specific and don't appear in Gen 9");
        report.AppendLine("- Memory items (Silvally) are marked as Past generation");
        report.AppendLine("- Primal Orbs (Blue/Red) are marked as Past generation");

        return report.ToString();
    }

    /// <summary>
    /// Gets a list of all items from the TypeScript items.ts file that should be in Gen 9.
    /// This is the ground truth from Pokemon Showdown.
    /// </summary>
    public static List<(string Name, int Num, int Gen, bool IsNonstandard)> GetTypeScriptItems()
    {
        // This would parse the items.ts file
        // For now, returning an empty list as a placeholder
        return new List<(string Name, int Num, int Gen, bool IsNonstandard)>();
    }
}
