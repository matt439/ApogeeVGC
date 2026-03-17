using System.Text;
using System.Text.RegularExpressions;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Parses Showdown's teambuilder export format and packs teams into
/// the packed wire format used by /utm.
/// Reference: pokemon-showdown-client/src/battle-teams.ts
/// </summary>
public static partial class ShowdownTeamPacker
{
    // ───────── Packing (PokemonSet → packed string for /utm) ─────────

    /// <summary>
    /// Pack a team into Showdown's packed format string.
    /// Follows the reference implementation in battle-teams.ts:60-129.
    /// </summary>
    public static string Pack(List<PokemonSet> team, Library library)
    {
        var buf = new StringBuilder();
        for (int i = 0; i < team.Count; i++)
        {
            if (i > 0) buf.Append(']');
            PackPokemon(buf, team[i], library);
        }
        return buf.ToString();
    }

    private static void PackPokemon(StringBuilder buf, PokemonSet set, Library library)
    {
        string speciesName = set.SpeciesOverrideName ?? library.Species[set.Species].Name;
        string name = set.Name;

        // name
        buf.Append(string.IsNullOrEmpty(name) ? speciesName : name);

        // species (blank if same as name after packName normalization)
        string packedSpecies = PackName(speciesName);
        string packedName = PackName(string.IsNullOrEmpty(name) ? speciesName : name);
        buf.Append('|');
        if (packedName != packedSpecies)
            buf.Append(packedSpecies);

        // item
        buf.Append('|');
        if (set.Item != ItemId.None)
            buf.Append(PackName(library.Items[set.Item].Name));

        // ability
        buf.Append('|');
        if (set.Ability != AbilityId.None)
            buf.Append(PackName(library.Abilities[set.Ability].Name));

        // moves
        buf.Append('|');
        for (int i = 0; i < set.Moves.Count; i++)
        {
            if (i > 0) buf.Append(',');
            buf.Append(PackName(library.Moves[set.Moves[i]].Name));
        }

        // nature
        buf.Append('|');
        if (set.Nature.Id != NatureId.None)
            buf.Append(set.Nature.Id.ToString());

        // evs
        buf.Append('|');
        StatsTable evs = set.Evs;
        string evsStr = $"{Ev(evs.Hp)},{Ev(evs.Atk)},{Ev(evs.Def)},{Ev(evs.SpA)},{Ev(evs.SpD)},{Ev(evs.Spe)}";
        if (evsStr != ",,,,,")
            buf.Append(evsStr);

        // gender
        buf.Append('|');
        if (set.Gender is GenderId.M or GenderId.F)
            buf.Append(set.Gender.ToString());

        // ivs
        buf.Append('|');
        StatsTable ivs = set.Ivs;
        string ivsStr = $"{Iv(ivs.Hp)},{Iv(ivs.Atk)},{Iv(ivs.Def)},{Iv(ivs.SpA)},{Iv(ivs.SpD)},{Iv(ivs.Spe)}";
        if (ivsStr != ",,,,,")
            buf.Append(ivsStr);

        // shiny
        buf.Append('|');
        if (set.Shiny)
            buf.Append('S');

        // level
        buf.Append('|');
        if (set.Level != 100)
            buf.Append(set.Level);

        // happiness (+ trailing optional section)
        buf.Append('|');
        if (set.Happiness != 255 && set.Happiness != 0)
            buf.Append(set.Happiness);

        // trailing optional: hpType,pokeball,gigantamax,dynamaxlevel,teraType
        if (set.TeraType != default)
        {
            buf.Append(','); // hpType (blank)
            buf.Append(','); // pokeball (blank)
            buf.Append(','); // gigantamax (blank)
            buf.Append(','); // dynamaxLevel (blank)
            buf.Append(',');
            buf.Append(set.TeraType.ToString());
        }
    }

    /// <summary>
    /// Strips all non-alphanumeric characters, preserving case.
    /// Matches JS: name.replace(/[^A-Za-z0-9]+/g, '')
    /// </summary>
    private static string PackName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "";
        return PackNameRegex().Replace(name, "");
    }

    private static string Ev(int val) => val == 0 ? "" : val.ToString();
    private static string Iv(int val) => val == 31 ? "" : val.ToString();

    [GeneratedRegex(@"[^A-Za-z0-9]+")]
    private static partial Regex PackNameRegex();

    // ───────── Importing (export paste text → PokemonSet list) ─────────

    /// <summary>
    /// Parse a Showdown teambuilder export paste into a list of PokemonSets.
    /// Reference: battle-teams.ts parseExportedTeamLine + import.
    /// </summary>
    public static List<PokemonSet> ImportExportPaste(string text, Library library)
    {
        var resolver = new ShowdownNameResolver(library);
        string[] lines = text.Split('\n');

        var sets = new List<PokemonSetBuilder>();
        PokemonSetBuilder? cur = null;

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            if (line == "" || line == "---")
            {
                cur = null;
                continue;
            }
            if (line.StartsWith("==="))
                continue; // team backup separator

            if (cur == null)
            {
                cur = new PokemonSetBuilder();
                sets.Add(cur);
                ParseFirstLine(line, cur, resolver);
            }
            else
            {
                ParseSubsequentLine(line, cur, resolver, library);
            }
        }

        return sets.Select(b => b.Build(library)).ToList();
    }

    private static void ParseFirstLine(string line, PokemonSetBuilder set, ShowdownNameResolver resolver)
    {
        // Split on @ for item
        int atIdx = line.IndexOf(" @ ", StringComparison.Ordinal);
        if (atIdx >= 0)
        {
            set.ItemName = line[(atIdx + 3)..].Trim();
            line = line[..atIdx].Trim();
        }

        // Gender suffix
        if (line.EndsWith(" (M)"))
        {
            set.Gender = GenderId.M;
            line = line[..^4];
        }
        else if (line.EndsWith(" (F)"))
        {
            set.Gender = GenderId.F;
            line = line[..^4];
        }

        // Nickname (Species) or just Species
        int parenIdx = line.LastIndexOf(" (", StringComparison.Ordinal);
        if (line.EndsWith(')') && parenIdx >= 0)
        {
            set.SpeciesName = line[(parenIdx + 2)..^1];
            set.Nickname = line[..parenIdx];
        }
        else
        {
            set.SpeciesName = line;
            set.Nickname = "";
        }
    }

    private static void ParseSubsequentLine(
        string line, PokemonSetBuilder set, ShowdownNameResolver resolver, Library library)
    {
        if (line.StartsWith("Ability: "))
        {
            set.AbilityName = line[9..];
        }
        else if (line.StartsWith("Level: "))
        {
            if (int.TryParse(line[7..], out int lvl))
                set.Level = lvl;
        }
        else if (line.StartsWith("Tera Type: "))
        {
            set.TeraTypeName = line[11..];
        }
        else if (line is "Shiny: Yes" or "Shiny")
        {
            set.Shiny = true;
        }
        else if (line.StartsWith("Happiness: "))
        {
            if (int.TryParse(line[11..], out int h))
                set.Happiness = h;
        }
        else if (line.StartsWith("EVs: "))
        {
            set.Evs = ParseStatLine(line[5..]);
        }
        else if (line.StartsWith("IVs: "))
        {
            set.Ivs = ParseStatLine(line[5..], defaultVal: 31);
        }
        else if (NatureRegex().IsMatch(line))
        {
            int idx = line.IndexOf(" Nature", StringComparison.OrdinalIgnoreCase);
            if (idx > 0)
                set.NatureName = line[..idx];
        }
        else if (line.StartsWith('-') || line.StartsWith('~'))
        {
            // Move line: "- Move Name" or "~ Move Name"
            string moveName = line[(line[1] == ' ' ? 2 : 1)..].Trim();
            set.MoveNames.Add(moveName);
        }
    }

    private static StatsTable ParseStatLine(string text, int defaultVal = 0)
    {
        var table = new StatsTable
        {
            Hp = defaultVal, Atk = defaultVal, Def = defaultVal,
            SpA = defaultVal, SpD = defaultVal, Spe = defaultVal,
        };

        foreach (string part in text.Split('/'))
        {
            string trimmed = part.Trim();
            int spaceIdx = trimmed.IndexOf(' ');
            if (spaceIdx < 0) continue;

            if (!int.TryParse(trimmed[..spaceIdx], out int val)) continue;
            string statName = trimmed[(spaceIdx + 1)..].Trim();

            switch (statName)
            {
                case "HP": table.Hp = val; break;
                case "Atk": table.Atk = val; break;
                case "Def": table.Def = val; break;
                case "SpA": table.SpA = val; break;
                case "SpD": table.SpD = val; break;
                case "Spe": table.Spe = val; break;
            }
        }
        return table;
    }

    [GeneratedRegex(@"^[A-Za-z]+ [Nn]ature")]
    private static partial Regex NatureRegex();

    /// <summary>
    /// Mutable builder used during import parsing, then frozen into a PokemonSet.
    /// </summary>
    private sealed class PokemonSetBuilder
    {
        public string SpeciesName = "";
        public string Nickname = "";
        public string? ItemName;
        public string? AbilityName;
        public List<string> MoveNames = [];
        public string? NatureName;
        public GenderId Gender = GenderId.N;
        public StatsTable? Evs;
        public StatsTable? Ivs;
        public int Level = 50;
        public bool Shiny;
        public int Happiness = 255;
        public string? TeraTypeName;

        public PokemonSet Build(Library library)
        {
            var resolver = new ShowdownNameResolver(library);

            SpecieId species = resolver.ResolveSpecies(SpeciesName);
            if (species == default)
                throw new InvalidOperationException($"Unknown species: '{SpeciesName}'");

            ItemId item = ItemId.None;
            if (!string.IsNullOrEmpty(ItemName))
            {
                item = resolver.ResolveItem(ItemName);
                if (item == ItemId.None)
                    throw new InvalidOperationException($"Unknown item: '{ItemName}'");
            }

            AbilityId ability = AbilityId.None;
            if (!string.IsNullOrEmpty(AbilityName))
            {
                ability = resolver.ResolveAbility(AbilityName);
                if (ability == AbilityId.None)
                    throw new InvalidOperationException($"Unknown ability: '{AbilityName}'");
            }

            var moves = new List<MoveId>();
            foreach (string moveName in MoveNames)
            {
                MoveId move = resolver.ResolveMove(moveName);
                if (move == MoveId.None)
                    throw new InvalidOperationException($"Unknown move: '{moveName}'");
                moves.Add(move);
            }

            NatureId natureId = NatureId.None;
            if (!string.IsNullOrEmpty(NatureName) &&
                Enum.TryParse(NatureName, ignoreCase: true, out NatureId nid))
                natureId = nid;

            Nature nature = library.Natures.TryGetValue(natureId, out Nature? n) ? n
                : library.Natures[NatureId.Serious];

            MoveType teraType = default;
            if (!string.IsNullOrEmpty(TeraTypeName))
                teraType = resolver.ResolveTeraType(TeraTypeName);

            return new PokemonSet
            {
                Name = string.IsNullOrEmpty(Nickname) ? library.Species[species].Name : Nickname,
                Species = species,
                Item = item,
                Ability = ability,
                Moves = moves,
                Nature = nature,
                Gender = Gender,
                Evs = Evs ?? new StatsTable(),
                Ivs = Ivs ?? StatsTable.PerfectIvs,
                Level = Level,
                Shiny = Shiny,
                Happiness = Happiness,
                TeraType = teraType,
            };
        }
    }
}
