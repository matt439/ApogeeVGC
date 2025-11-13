using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    public void RunPickTeam()
    {
        if (DebugMode)
        {
            Console.WriteLine("[RunPickTeam] STARTING");
        }

        UpdateAllPlayersUi(BattlePerspectiveType.TeamPreview);
        MakeRequest(RequestState.TeamPreview);

        if (DebugMode)
        {
            Console.WriteLine("[RunPickTeam] About to call RequestPlayerChoices");
        }

        // Request player choices - Battle returns immediately
        // Simulator will call Choose() -> ProcessTeamPreviewChoices() to continue
        RequestPlayerChoices(onComplete: () =>
        {
            if (DebugMode)
            {
                Console.WriteLine("[RunPickTeam CALLBACK] Team preview callback invoked!");
                Debug("Team preview choices received, processing");
            }

            // Process team preview choices without calling TurnLoop
            // Battle.Start() will handle adding StartGameAction and calling TurnLoop
            ProcessTeamPreviewChoices();

            if (DebugMode)
            {
                Console.WriteLine("[RunPickTeam CALLBACK] Team preview callback completed");
            }
        });

        if (DebugMode)
        {
            Console.WriteLine("[RunPickTeam] RequestPlayerChoices returned, exiting RunPickTeam");
            Debug("Exiting RunPickTeam - Battle returns, waiting for team preview choices");
        }

        // Return immediately - Battle doesn't wait
        // Simulator handles coordination
    }

    /// <summary>
    /// Processes team preview choices by adding team actions to the queue
    /// and starting the turn loop. Used during battle startup.
    /// </summary>
    private void ProcessTeamPreviewChoices()
    {
        if (DebugMode)
        {
            Console.WriteLine("[ProcessTeamPreviewChoices] STARTING");
            Debug("ProcessTeamPreviewChoices starting");
        }

        UpdateSpeed();

        if (!AllChoicesDone())
        {
            if (DebugMode)
            {
                Console.WriteLine("[ProcessTeamPreviewChoices] ERROR: Not all choices done!");
            }

            throw new InvalidOperationException("Not all choices done");
        }

        if (DebugMode)
        {
            Console.WriteLine("[ProcessTeamPreviewChoices] All choices done, processing...");
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

        if (DebugMode)
        {
            Console.WriteLine(
                $"[ProcessTeamPreviewChoices] Queue before sort: {Queue.List.Count} actions");
            for (int i = 0; i < Queue.List.Count; i++)
            {
                var action = Queue.List[i];
                Console.WriteLine(
                    $"  [{i}] {action.Choice}, Priority={action.Priority}, Order={action.Order}");
            }
        }

        // Sort the new actions by priority/speed
        Queue.Sort();

        if (DebugMode)
        {
            Console.WriteLine(
                $"[ProcessTeamPreviewChoices] Queue after sort: {Queue.List.Count} actions");
            for (int i = 0; i < Queue.List.Count; i++)
            {
                var action = Queue.List[i];
                Console.WriteLine(
                    $"  [{i}] {action.Choice}, Priority={action.Priority}, Order={action.Order}");
            }
        }

        ClearRequest();

        if (DebugMode)
        {
            Console.WriteLine(
                $"[ProcessTeamPreviewChoices] Queue size: {Queue.List.Count}, calling TurnLoop");
            Debug("Team actions added to queue, starting turn loop");
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
                Add("bigerror",
                    "Warning: One player isn't adhering to a 510 EV limit, and the other player is.");
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
                    TeraType = set.TeraType,
                };

                // Special handling for Zacian/Zamazenta with their signature items
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
            SpecieId.Zacian => SpecieId.Zacian,
            SpecieId.Zamazenta => SpecieId.Zamazenta,

            // Xerneas formes should be masked
            SpecieId.Xerneas or SpecieId.XerneasNeutral or SpecieId.XerneasActive => SpecieId
                .Xerneas,

            // Don't mask Crowned formes
            SpecieId.ZacianCrowned => SpecieId.ZacianCrowned,
            SpecieId.ZamazentaCrowned => SpecieId.ZamazentaCrowned,

            // All other species pass through unchanged
            _ => speciesId,
        };
    }
}