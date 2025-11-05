using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync
{
    public void RunPickTeam()
    {
        Console.WriteLine($"[RunPickTeam] Starting, DisplayUi={DisplayUi}");

        Console.WriteLine($"[RunPickTeam] Calling Format.OnTeamPreview");
  
        // onTeamPreview handlers are expected to show full teams to all active sides,
        // and send a 'teampreview' request for players to pick their leads / team order.
        Format.OnTeamPreview?.Invoke(this);
 
        Console.WriteLine($"[RunPickTeam] Format.OnTeamPreview completed");

        foreach (RuleId rule in RuleTable.Keys)
        {
            string ruleString = rule.ToString();
            if (ruleString.Length > 0 && "+*-!".Contains(ruleString[0])) continue;
            Format subFormat = Library.Rulesets[rule];
            subFormat.OnTeamPreview?.Invoke(this);
        }

        if (RequestState == RequestState.TeamPreview)
        {
            Console.WriteLine("[RunPickTeam] RequestState already TeamPreview, returning early");
            return;
        }

        if (RuleTable.PickedTeamSize.HasValue && RuleTable.PickedTeamSize.Value > 0)
        {
   Console.WriteLine($"[RunPickTeam] PickedTeamSize={RuleTable.PickedTeamSize.Value}, sending poke messages");
      // There was no onTeamPreview handler (e.g. Team Preview rule missing).
            // Players must still pick their own Pokémon, so we show them privately.
            if (DisplayUi)
            {
                Add("clearpoke");
            }

            foreach (Pokemon pokemon in GetAllPokemon())
            {
                // Get the details object
                Pokemon.PokemonDetails detailsObj = pokemon.Details;

                // Create a modified copy for team preview display:
                // - Hide shiny status
                // - Mask certain species formes
                // - Hide level (team preview always shows level 100 so level text doesn't appear)
                // - Keep gender
                var maskedDetails = new Pokemon.PokemonDetails
                {
                    Id = MaskSpeciesForTeamPreview(detailsObj.Id),
                    Name = Library.Species[MaskSpeciesForTeamPreview(detailsObj.Id)].Name,
                    Level = 100, // Always use 100 so level doesn't appear in the string
                    Gender = detailsObj.Gender,
                    Shiny = false, // Always hide shiny in team preview
                    TeraType = null, // Hide Tera type in team preview
                };

                // Convert to protocol string (will not include L100, shiny, or tera type)
                string detailsString = maskedDetails.ToString();

                // Add the poke message
                // Item parameter: "item" if holding item, omit parameter entirely if not
                if (DisplayUi)
                {
                    Console.WriteLine($"[RunPickTeam] Pokemon {pokemon.Name} has item: {pokemon.Item}");

                    if (pokemon.Item != ItemId.None)
                    {
                        // Has item - send "item" as 4th parameter
                        Add("poke", pokemon.Side.Id.ToString(), detailsString, "item");
                    }
                    else
                    {
                        // No item - only send 3 parameters (no trailing |)
                        Add("poke", pokemon.Side.Id.ToString(), detailsString);
                    }
                }
            }

            MakeRequest(RequestState.TeamPreview);
        }
    }

    public void CheckEvBalance()
    {
        if (!DisplayUi) return;

        bool? limitedEVs = null;

        foreach (Side side in Sides)
        {
            // Check if this side's Pokémon all have 510 or fewer total EVs
            bool sideLimitedEVs = !side.Pokemon.Any(pokemon =>
            {
                // Sum all EV values for this Pokémon
                int totalEvs = pokemon.Set.Evs.Values.Sum();
                return totalEvs > 510;
            });

            if (limitedEVs == null)
            {
                // First side - just record the limit status
                limitedEVs = sideLimitedEVs;
            }
            else if (limitedEVs != sideLimitedEVs)
            {
                // Sides have different EV limit adherence - show warning
                Add("bigerror", "Warning: One player isn't adhering to a 510 EV limit, and the other player is.");
            }
        }
    }

    /// <summary>
    /// Generates and sends team preview information at the start of a battle.
    /// Creates sanitized team data that can be safely shown to both players,
    /// hiding sensitive information like exact EVs/IVs while showing species,
    /// items, abilities, and moves.
    /// </summary>
    public void ShowOpenTeamSheets()
    {
        // Only show team sheets at the very start of the battle
        if (Turn != 0) return;

        foreach (Side side in Sides)
        {
            var team = new List<PokemonSet>();

            foreach (Pokemon pokemon in side.Pokemon)
            {
                PokemonSet set = pokemon.Set;

                // Create sanitized set with visible information only
                var newSet = new PokemonSet
                {
                    Name = string.Empty, // Hide nicknames
                    Species = set.Species,
                    Item = set.Item,
                    Ability = set.Ability,
                    Moves = set.Moves,
                    Nature = new Nature { Id = NatureId.None }, // Hide nature
                    Gender = pokemon.Gender,
                    Evs = new StatsTable(), // Hide exact EVs
                    Ivs = new StatsTable(), // Hide exact IVs
                    Level = set.Level,
                    Shiny = set.Shiny,
                    TeraType = set.TeraType, // Will be set below for Gen 9
                };

                // Special handling for Zacian/Zamazenta with their signature items
                // This prevents the client from flagging them as illusions when they use their signature move
                if (set is { Species: SpecieId.Zacian, Item: ItemId.RustedSword } or
                    { Species: SpecieId.Zamazenta, Item: ItemId.RustedShield })
                {
                    // Convert to Crowned forme
                    SpecieId crownedSpecies = set.Species == SpecieId.Zacian
                        ? SpecieId.ZacianCrowned
                        : SpecieId.ZamazentaCrowned;

                    newSet = newSet with { Species = crownedSpecies };

                    // Replace Iron Head with signature move
                    var crownedMoves = new Dictionary<SpecieId, MoveId>
                    {
                        { SpecieId.ZacianCrowned, MoveId.BehemothBlade },
                        { SpecieId.ZamazentaCrowned, MoveId.BehemothBash },
                    };

                    int ironHeadIndex = newSet.Moves.ToList().IndexOf(MoveId.IronHead);
                    if (ironHeadIndex >= 0)
                    {
                        var movesList = newSet.Moves.ToList();
                        movesList[ironHeadIndex] = crownedMoves[crownedSpecies];
                        newSet = newSet with { Moves = movesList };
                    }
                }

                team.Add(newSet);
            }

            // Send the sanitized team data to the client
            // Note: You'll need to implement a Teams.Pack() equivalent method
            // that serializes the team data into the format expected by the client
            string packedTeam = Teams.Pack(team);
            Add("showteam", side.Id.ToString(), packedTeam);
        }
    }

    /// <summary>
    /// Masks species IDs that change forme on battle start to prevent information leakage.
    /// Returns a masked version (with -*) for species that need hiding, otherwise returns the original.
    /// </summary>
    private static SpecieId MaskSpeciesForTeamPreview(SpecieId speciesId)
    {
        return speciesId switch
        {
            // Zacian/Zamazenta without Crowned forme should be masked
            SpecieId.Zacian => SpecieId.Zacian, // Keep as-is, will add masking in SpecieId enum if needed
            SpecieId.Zamazenta => SpecieId.Zamazenta, // Keep as-is

            // Xerneas formes should be masked
            SpecieId.Xerneas or SpecieId.XerneasNeutral or SpecieId.XerneasActive => SpecieId.Xerneas,

            // Don't mask Crownedformes
            SpecieId.ZacianCrowned => SpecieId.ZacianCrowned,
            SpecieId.ZamazentaCrowned => SpecieId.ZamazentaCrowned,

            // All other species pass through unchanged
            _ => speciesId,
        };
    }
}