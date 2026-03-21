using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using ApogeeVGC.Data;
using ApogeeVGC.LiveAssist;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Core;

/// <summary>
/// Shared helpers for equivalence testing.
/// Extracted from Driver so both the interactive single-debug mode (Driver.EquivalenceTest)
/// and the xUnit batch tests can reuse the same parsing and comparison logic.
/// </summary>
public static class EquivalenceTestHelper
{
    /// <summary>
    /// Filter protocol lines to game-state-affecting entries only.
    /// Strips animation hints, empty lines, and channel markers.
    /// </summary>
    public static List<string> FilterProtocolLines(IEnumerable<string> lines)
    {
        List<string> result = [];
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (!line.StartsWith('|')) continue;
            if (line == "|") continue;

            // Skip metadata/cosmetic lines
            if (line.StartsWith("|t:|")) continue;
            if (line.StartsWith("|split|")) continue;
            if (line.StartsWith("|upkeep")) continue;
            if (line.StartsWith("|gametype|")) continue;
            if (line.StartsWith("|player|")) continue;
            if (line.StartsWith("|gen|")) continue;
            if (line.StartsWith("|tier|")) continue;
            if (line.StartsWith("|rule|")) continue;
            if (line.StartsWith("|teamsize|")) continue;
            if (line.StartsWith("|-anim|")) continue;

            // Skip non-game-state lines from Showdown's omniscient stream
            if (line.StartsWith("update")) continue;
            if (line.StartsWith("sideupdate")) continue;

            // Strip C#-only tags: [dmg]N, [heal]N (these are parser hints not in Showdown output)
            string cleaned = Regex.Replace(line, @"\|\[dmg\]\d+", "");
            cleaned = Regex.Replace(cleaned, @"\|\[heal\]\d+", "");
            // Remove trailing pipe if stripping left one
            cleaned = cleaned.TrimEnd('|');

            result.Add(cleaned);
        }
        return result;
    }

    /// <summary>
    /// Walk up from a directory to find the solution root (contains .sln file).
    /// </summary>
    public static string FindSolutionRoot(string startDir)
    {
        string? dir = startDir;
        while (dir != null)
        {
            if (Directory.GetFiles(dir, "*.sln").Length > 0)
                return dir;
            dir = Directory.GetParent(dir)?.FullName;
        }

        // No .sln found (e.g. headless deployment) — fall back to exe directory
        return startDir;
    }

    /// <summary>
    /// Maps a Showdown format string to the corresponding <see cref="FormatId"/>.
    /// </summary>
    public static FormatId ResolveFormatId(string formatId) => formatId switch
    {
        "gen9vgc2024regg" => FormatId.Gen9VgcRegulationG,
        "gen9vgc2024regh" => FormatId.Gen9VgcRegulationH,
        "gen9vgc2024regi" or "gen9vgc2025regi" => FormatId.Gen9VgcRegulationI,
        "gen9vgcmega" => FormatId.Gen9VgcMega,
        "gen9randombattle" => FormatId.Gen9RandomBattle,
        "gen9randomdoublesbattle" => FormatId.Gen9RandomDoublesBattle,
        _ => throw new ArgumentException($"Unknown format: {formatId}"),
    };

    /// <summary>
    /// Parse a Showdown JSON team array into C# PokemonSet[].
    /// Expects the unpacked team format from Teams.unpack() (species, moves, ability, item, etc.).
    /// </summary>
    public static PokemonSet[] ParseShowdownTeam(
        JsonElement teamArray, ShowdownNameResolver resolver, Library library)
    {
        List<PokemonSet> sets = [];
        foreach (JsonElement mon in teamArray.EnumerateArray())
        {
            string species = mon.GetProperty("species").GetString()!;
            SpecieId specieId = resolver.ResolveSpecies(species);
            if (specieId == default)
            {
                Console.WriteLine($"  WARNING: Unknown species '{species}', skipping");
                continue;
            }

            // Moves
            List<MoveId> moves = [];
            foreach (JsonElement m in mon.GetProperty("moves").EnumerateArray())
            {
                string moveName = m.GetString()!;
                MoveId moveId = resolver.ResolveMove(moveName);
                if (moveId != MoveId.None)
                    moves.Add(moveId);
                else
                    Console.WriteLine($"  WARNING: Unknown move '{moveName}' on {species}");
            }
            if (moves.Count == 0) moves.Add(MoveId.Tackle);

            // Ability
            string abilityName = mon.GetProperty("ability").GetString() ?? "";
            AbilityId abilityId = resolver.ResolveAbility(abilityName);

            // Item
            string itemName = mon.GetProperty("item").GetString() ?? "";
            ItemId itemId = resolver.ResolveItem(itemName);

            // Nature
            string natureName = mon.TryGetProperty("nature", out JsonElement natElem)
                ? natElem.GetString() ?? "Serious" : "Serious";
            NatureId natureId = Enum.TryParse<NatureId>(natureName, true, out NatureId nid) ? nid : NatureId.Serious;
            Nature nature = library.Natures[natureId];

            // EVs
            StatsTable evs = new();
            if (mon.TryGetProperty("evs", out JsonElement evsElem))
            {
                evs = new StatsTable
                {
                    Hp = evsElem.TryGetProperty("hp", out JsonElement hp) ? hp.GetInt32() : 0,
                    Atk = evsElem.TryGetProperty("atk", out JsonElement atk) ? atk.GetInt32() : 0,
                    Def = evsElem.TryGetProperty("def", out JsonElement def) ? def.GetInt32() : 0,
                    SpA = evsElem.TryGetProperty("spa", out JsonElement spa) ? spa.GetInt32() : 0,
                    SpD = evsElem.TryGetProperty("spd", out JsonElement spd) ? spd.GetInt32() : 0,
                    Spe = evsElem.TryGetProperty("spe", out JsonElement spe) ? spe.GetInt32() : 0,
                };
            }

            // IVs
            StatsTable ivs = StatsTable.PerfectIvs;
            if (mon.TryGetProperty("ivs", out JsonElement ivsElem))
            {
                ivs = new StatsTable
                {
                    Hp = ivsElem.TryGetProperty("hp", out JsonElement hp) ? hp.GetInt32() : 31,
                    Atk = ivsElem.TryGetProperty("atk", out JsonElement atk) ? atk.GetInt32() : 31,
                    Def = ivsElem.TryGetProperty("def", out JsonElement def) ? def.GetInt32() : 31,
                    SpA = ivsElem.TryGetProperty("spa", out JsonElement spa) ? spa.GetInt32() : 31,
                    SpD = ivsElem.TryGetProperty("spd", out JsonElement spd) ? spd.GetInt32() : 31,
                    Spe = ivsElem.TryGetProperty("spe", out JsonElement spe) ? spe.GetInt32() : 31,
                };
            }

            // Level
            int level = mon.TryGetProperty("level", out JsonElement lvl) ? lvl.GetInt32() : 100;

            // Tera type
            MoveType teraType = default;
            if (mon.TryGetProperty("teraType", out JsonElement tera))
            {
                string teraStr = tera.GetString() ?? "";
                teraType = resolver.ResolveTeraType(teraStr);
            }

            // Gender
            GenderId gender = GenderId.N;
            if (mon.TryGetProperty("gender", out JsonElement genderElem))
            {
                string g = genderElem.GetString() ?? "";
                gender = g switch
                {
                    "M" => GenderId.M,
                    "F" => GenderId.F,
                    _ => GenderId.N,
                };
            }

            // Shiny
            bool shiny = mon.TryGetProperty("shiny", out JsonElement shinyElem) && shinyElem.GetBoolean();

            // Check if species name from fixture differs from library name (cosmetic forms)
            string libraryName = library.Species[specieId].Name;
            string? speciesOverride = !string.Equals(species, libraryName, StringComparison.OrdinalIgnoreCase)
                ? species : null;

            sets.Add(new PokemonSet
            {
                Name = mon.TryGetProperty("name", out JsonElement nameElem)
                    ? nameElem.GetString() ?? species : species,
                Species = specieId,
                Item = itemId,
                Ability = abilityId,
                Moves = moves,
                Nature = nature,
                Evs = evs,
                Ivs = ivs,
                Level = level,
                TeraType = teraType,
                Gender = gender,
                Shiny = shiny,
                SpeciesOverrideName = speciesOverride,
            });
        }

        return sets.ToArray();
    }

    /// <summary>
    /// Generate a Showdown battle fixture by running Node.js.
    /// Returns true if successful, false if the process failed.
    /// </summary>
    public static bool GenerateShowdownFixture(
        string toolDir, string format, string seed, string p1Seed, string p2Seed,
        string outputBase, string logFile)
    {
        string scriptPath = Path.Combine(toolDir, "run_showdown_battle.js");

        ProcessStartInfo psi = new()
        {
            FileName = "node",
            Arguments = $"\"{scriptPath}\" --random --format {format} --seed {seed} --p1seed {p1Seed} --p2seed {p2Seed} --out \"{outputBase}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = toolDir,
        };

        using Process? process = Process.Start(psi);
        if (process == null) return false;

        // Read stdout/stderr asynchronously to avoid deadlocks
        Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync();
        Task<string> stderrTask = process.StandardError.ReadToEndAsync();

        if (!process.WaitForExit(30_000))
        {
            try { process.Kill(true); } catch { /* ignore */ }
            return false;
        }

        string stdout = stdoutTask.GetAwaiter().GetResult();
        string stderr = stderrTask.GetAwaiter().GetResult();

        if (process.ExitCode != 0)
        {
            Console.WriteLine($"    Node.js error: {stderr}");
            return false;
        }

        // Write protocol log to file
        File.WriteAllText(logFile, stdout);
        return File.Exists(outputBase + ".fixture.json");
    }

    /// <summary>
    /// Gets the current git commit ID of the pokemon-showdown checkout.
    /// </summary>
    public static string GetShowdownCommitId(string showdownDir)
    {
        try
        {
            ProcessStartInfo psi = new()
            {
                FileName = "git",
                Arguments = "rev-parse HEAD",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = showdownDir,
            };
            using Process? process = Process.Start(psi);
            if (process == null) return "unknown";
            string output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit(5_000);
            return process.ExitCode == 0 ? output : "unknown";
        }
        catch
        {
            return "unknown";
        }
    }

    /// <summary>
    /// Truncate a string to a maximum length, appending "..." if truncated.
    /// </summary>
    public static string Truncate(string s, int maxLen) =>
        s.Length <= maxLen ? s : s[..maxLen] + "...";
}
