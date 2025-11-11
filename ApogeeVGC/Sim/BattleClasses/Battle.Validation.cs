using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    public void RunPickTeam()
    {
        //// onTeamPreview handlers are expected to show full teams to all active sides,
        //// and send a 'teampreview' request for players to pick their leads / team order.
        //Format.OnTeamPreview?.Invoke(this);

        //foreach (RuleId rule in RuleTable.Keys)
        //{
        //    string ruleString = rule.ToString();
        //    if (ruleString.Length > 0 && "+*-!".Contains(ruleString[0])) continue;
        //    Format subFormat = Library.Rulesets[rule];
        //    subFormat.OnTeamPreview?.Invoke(this);
        //}

        //if (RequestState == RequestState.TeamPreview)
        //{
        //    return;
        //}

        //if (RuleTable.PickedTeamSize > 0)
        //{
        //    // There was no onTeamPreview handler (e.g. Team Preview rule missing).
        //    // Players must still pick their own Pokémon, so we show them privately.
        //    if (DisplayUi)
        //    {
        //        Add("clearpoke");
        //    }

        //    foreach (Pokemon pokemon in GetAllPokemon())
        //    {
        //        // Get the details object and convert to string
        //        Pokemon.PokemonDetails detailsObj = pokemon.Details;

        //        // Create a modified copy for display (hide certain formes)
        //        var maskedDetails = new Pokemon.PokemonDetails
        //        {
        //            Id = MaskSpeciesForTeamPreview(detailsObj.Id),
        //            Level = detailsObj.Level,
        //            Gender = detailsObj.Gender,
        //            Shiny = false, // Always hide shiny in team preview
        //            TeraType = detailsObj.TeraType,
        //        };

        //        // Convert to protocol string
        //        string detailsString = maskedDetails.ToString();

        //        AddSplit(pokemon.Side.Id,
        //            [
        //                new StringPart("poke"),
        //            new StringPart(pokemon.Side.Id.ToString()),
        //            new StringPart(detailsString),
        //            new StringPart(string.Empty),
        //            ]);
        //    }

        //    MakeRequest(RequestState.TeamPreview);
        //}

        UpdateAllPlayersUi(BattlePerspectiveType.TeamPreview);
        MakeRequest(RequestState.TeamPreview);
        
    // Request player choices - Battle will pause until callback is invoked
        RequestPlayerChoices(onComplete: () =>
        {
  Console.WriteLine("[RunPickTeam] Team preview choices received, processing");
         // Process team preview choices without calling TurnLoop
        // Battle.Start() will handle adding StartGameAction and calling TurnLoop
            ProcessTeamPreviewChoices();
    });

        Debug("Exiting RunPickTeam().");
    
        // WAIT here until team preview choices are complete and processed
    Console.WriteLine("[RunPickTeam] Waiting for team preview choices...");
  _choiceWaitHandle.Wait();
    Console.WriteLine("[RunPickTeam] Team preview complete, continuing to battle start");
    }

    /// <summary>
    /// Processes team preview choices by adding team actions to the queue
    /// without starting the turn loop. Used during battle startup.
    /// </summary>
    private void ProcessTeamPreviewChoices()
    {
        Console.WriteLine("[ProcessTeamPreviewChoices] Starting");
        
     UpdateSpeed();

    if (!AllChoicesDone())
{
            throw new InvalidOperationException("Not all choices done");
        }

   // Log each side's choice to the input log and history
        foreach (Side side in Sides)
        {
  string? choice = side.GetChoice().ToString();
            if (!string.IsNullOrEmpty(choice))
            {
     InputLog.Add($"> {side.Id} {choice}");
      History.RecordChoice(side.Id, choice);
            }
  }

        // Add each side's actions to the queue
        foreach (Side side in Sides)
        {
 Queue.AddChoice(side.Choice.Actions);
        }

   // Sort the new actions by priority/speed
        Queue.Sort();

      ClearRequest();

   Console.WriteLine("[ProcessTeamPreviewChoices] Team actions added to queue, setting wait handle");
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

            // Don't mask Crowned formes
            SpecieId.ZacianCrowned => SpecieId.ZacianCrowned,
            SpecieId.ZamazentaCrowned => SpecieId.ZamazentaCrowned,

            // All other species pass through unchanged
            _ => speciesId,
        };
    }
}