using System.Collections.ObjectModel;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Data;

public record Formats
{
    public IReadOnlyDictionary<FormatId, Format> FormatData { get; }

    public Formats()
    {
        FormatData = new ReadOnlyDictionary<FormatId, Format>(_formatData);
    }

    private readonly Dictionary<FormatId, Format> _formatData = new()
    {
        [FormatId.Gen9Ou] = new Format
        {
            Name = "[Gen 9] OU",
            FormatId = FormatId.Gen9Ou,
            Ruleset = [RuleId.Standard], //TODO: fill in rules
            Banlist = [], //TODO: fill in bans
        },
        [FormatId.Gen9CustomGame] = new Format
        {
            Name = "[Gen 9] Custom Game",
            FormatId = FormatId.Gen9CustomGame,
            GameType = GameType.Doubles,
            Ruleset = [RuleId.TeamPreview, RuleId.StandardOts],
            Banlist = [],
            OnTeamPreview = battle =>
            {
                // Cast to BattleAsync to access full API
                if (battle is not BattleAsync battleAsync) return;

                // Show full teams to all players
                if (battleAsync.DisplayUi)
                {
                    battleAsync.Add("clearpoke");
                }

                foreach (Pokemon pokemon in battleAsync.GetAllPokemon())
                {
                    // Format the Pokemon details string manually for team preview
                    // In team preview, we show: Species, Level (if not 100), Gender, but NOT shiny
                    string speciesName = pokemon.Species.Name;

                    // Build details string
                    List<string> detailsParts = [speciesName];

                    if (pokemon.Level != 100)
                    {
                        detailsParts.Add($"L{pokemon.Level}");
                    }

                    if (pokemon.Gender == GenderId.M)
                    {
                        detailsParts.Add("M");
                    }
                    else if (pokemon.Gender == GenderId.F)
                    {
                        detailsParts.Add("F");
                    }

                    string detailsString = string.Join(", ", detailsParts);

                    // Determine if Pokemon has item (show as "item" without revealing which)
                    string itemStatus = pokemon.Item != null && pokemon.Item != ItemId.None ? "item" : "";

                    if (battleAsync.DisplayUi)
                    {
                        battleAsync.Add("poke", pokemon.Side.Id.ToString().ToLowerInvariant(), detailsString, itemStatus);
                    }
                }

                // Don't add teampreview here - it will be added by MakeRequest
                // Send team preview request
                battleAsync.MakeRequest(RequestState.TeamPreview);
            },
        },
    };

    /// <summary>
    /// Masks species ID for team preview by hiding certain forme information.
    /// </summary>
    private static SpecieId MaskSpeciesForTeamPreview(SpecieId speciesId)
    {
        // For most Pokemon, show the species as-is
        // For forme-changing Pokemon where the forme isn't revealed in team preview,
        // we would mask it with a generic forme indicator

        // Examples that would need masking (not implemented yet):
        // - Arceus formes -> Arceus-*
        // - Genesect formes -> Genesect-*
        // - Silvally formes -> Silvally-*

        // For now, return the species as-is
        return speciesId;
    }
}