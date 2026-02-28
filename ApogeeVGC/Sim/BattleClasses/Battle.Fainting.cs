using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    public static void Faint(Pokemon pokemon, Pokemon? source = null, IEffect? effect = null)
    {
        pokemon.Faint(source, effect);
    }

    public void CheckFainted()
    {
        // Iterate through all sides in the battle
        foreach (Pokemon pokemon in Sides.SelectMany(side => side.Active.OfType<Pokemon>()
                     .Where(pokemon => pokemon.Fainted)))
        {
            // Mark that this Pokémon needs to be switched out
            pokemon.SwitchFlag = true;
        }
        
        // CRITICAL: After setting switch flags, verify battle state is consistent
        // If any side has all active Pokemon fainted and no Pokemon left to switch in,
        // they should lose the battle immediately
        foreach (Side side in Sides)
        {
            // Check if all active slots have fainted Pokemon
            bool allActiveFainted = side.Active.All(p => p == null || p.Fainted);
            
            if (allActiveFainted && side.PokemonLeft <= 0)
            {
                Debug($"[CheckFainted] {side.Name} has all active Pokemon fainted and no Pokemon left to switch");
                Debug($"[CheckFainted] This side should lose the battle");
                // Don't call Win/Lose here - just ensure CheckWin will be called next
                // The battle loop will handle this properly
            }
        }
    }

    public bool? FaintMessages(bool lastFirst = false, bool forceCheck = false, bool checkWin = true)
    {
        // Battle already ended
        if (Ended) return null;

        int length = FaintQueue.Count;

        // Empty queue
        if (length == 0)
        {
            return forceCheck && CheckWin() == true;
        }

        // Reorder queue if requested (move last to first)
        if (lastFirst && FaintQueue.Count > 0)
        {
            FaintQueue lastFaintData = FaintQueue[^1];
            FaintQueue.RemoveAt(FaintQueue.Count - 1);
            FaintQueue.Insert(0, lastFaintData);
        }

        FaintQueue? faintData = null;

        // Process all faints in queue
        while (FaintQueue.Count > 0)
        {
            int faintQueueLeft = FaintQueue.Count;
            faintData = FaintQueue[0];
            FaintQueue.RemoveAt(0);

            Pokemon pokemon = faintData.Target;

            // Run BeforeFaint event - allows abilities/items to trigger
            RelayVar? beforeFaintResult = RunEvent(
                EventId.BeforeFaint,
                pokemon,
                RunEventSource.FromNullablePokemon(faintData.Source),
                faintData.Effect
            );

            // If Pokemon hasn't fainted yet and BeforeFaint didn't prevent it
            if (!pokemon.Fainted && (beforeFaintResult == null || IsRelayVarTruthy(beforeFaintResult)))
            {
                // Print faint message
                if (DisplayUi)
                {
                    Add("faint", pokemon);
                }

                // Record faint in history
                History.RecordFaint(pokemon.Name);

                // Update side's Pokemon count
                if (pokemon.Side.PokemonLeft > 0)
                {
                    pokemon.Side.PokemonLeft--;
                }

                // Track total fainted (capped at 100)
                if (pokemon.Side.TotalFainted < 100)
                {
                    pokemon.Side.TotalFainted++;
                }

                // Run Faint event - triggers fainting abilities/items
                RunEvent(
                    EventId.Faint,
                    pokemon,
                    RunEventSource.FromNullablePokemon(faintData.Source),
                    faintData.Effect
                );

                // End ability state
                Ability ability = pokemon.GetAbility();
                SingleEvent(EventId.End, ability, pokemon.AbilityState, (SingleEventTarget)pokemon);

                // End item state
                Item item = pokemon.GetItem();
                SingleEvent(EventId.End, item, pokemon.ItemState, (SingleEventTarget)pokemon);

                // Handle forme regression (e.g., Mega Evolution reverting)
                if (pokemon is { FormeRegression: true, Transformed: false })
                {
                    // BEFORE clearing volatiles: restore base species and ability
                    pokemon.BaseSpecies = Library.Species[pokemon.Set.Species];
                    pokemon.BaseAbility = pokemon.Set.Ability;
                }

                // Clear all volatile conditions
                pokemon.ClearVolatile(false);

                // Mark as fainted and inactive
                pokemon.Fainted = true;
                pokemon.Illusion = null;
                pokemon.IsActive = false;
                pokemon.IsStarted = false;
                pokemon.Terastallized = null;

                // Complete forme regression
                if (pokemon.FormeRegression)
                {
                    // AFTER clearing volatiles: update details and stats
                    pokemon.Details = pokemon.GetUpdatedDetails();
                    if (DisplayUi)
                    {
                        Add("detailschange", pokemon, pokemon.Details.ToString(), "[silent]");
                    }
                    pokemon.UpdateMaxHp();
                    pokemon.FormeRegression = false;
                }

                // Track this faint for the current turn
                pokemon.Side.FaintedThisTurn = pokemon;

                // If queue grew during processing, we need to check win
                if (FaintQueue.Count >= faintQueueLeft)
                {
                    checkWin = true;
                }
            }
        }

        // Check for battle end
        if (checkWin && CheckWin(faintData) == true)
        {
            return true;
        }

        // Run AfterFaint event with original queue length
        if (faintData != null && length > 0)
        {
            RunEvent(
                EventId.AfterFaint,
                faintData.Target,
                RunEventSource.FromNullablePokemon(faintData.Source),
                faintData.Effect,
                IntRelayVar.Get(length)
            );
        }

        // Check for fainted Pokemon and set their switch flags
        CheckFainted();

        // Flush events so faint messages are sent before switch requests
        FlushEvents();

        return false;
    }

    public bool? CheckWin(FaintQueue? faintData = null)
    {
        // If all sides have no Pokemon left, it's a tie (or last-faint-wins in Gen 5+)
        if (Sides.All(side => side.PokemonLeft <= 0))
        {
            // In Gen 5+, the side whose Pokemon fainted last loses (so their foe wins)
            // In earlier gens, it's a tie
            if (faintData != null && Gen > 4)
            {
                return Win(faintData.Target.Side.Foe);
            }
            else
            {
                return Win((Side?)null); // Tie
            }
        }

        // Check if any side has defeated all opposing Pokemon
        foreach (Side side in Sides)
        {
            if (side.FoePokemonLeft() <= 0)
            {
                return Win(side);
            }
        }

        // Battle hasn't ended yet
        return null;
    }

    public bool Tiebreak()
    {
        if (Ended) return false;

        InputLog.Add("> tiebreak");

        if (DisplayUi)
        {
            Add("message", "Time's up! Going to tiebreaker...");
        }

        // Count non-fainted Pokemon for each side
        var notFainted = Sides.Select(side =>
            side.Pokemon.Count(pokemon => !pokemon.Fainted)
        ).ToList();

        // Display Pokemon count per side
        if (DisplayUi)
        {
            string pokemonCountMessage = string.Join("; ",
                Sides.Select((side, i) => $"{side.Name}: {notFainted[i]} Pokemon left")
            );
            Add("-message", pokemonCountMessage);
        }

        // Filter sides with maximum Pokemon count
        int maxNotFainted = notFainted.Max();
        var tiedSides = Sides.Where((_, i) => notFainted[i] == maxNotFainted).ToList();

        if (tiedSides.Count <= 1)
        {
            return Win(tiedSides.FirstOrDefault());
        }

        // Calculate HP percentages
        var hpPercentage = tiedSides.Select(side =>
            side.Pokemon.Sum(pokemon => (double)pokemon.Hp / pokemon.MaxHp) * 100 / 6
        ).ToList();

        // Display HP percentage per side
        if (DisplayUi)
        {
            string hpPercentageMessage = string.Join("; ",
                tiedSides.Select((side, i) => $"{side.Name}: {Math.Round(hpPercentage[i])}% total HP left")
            );
            Add("-message", hpPercentageMessage);
        }

        // Filter sides with maximum HP percentage
        double maxPercentage = hpPercentage.Max();
        tiedSides = tiedSides.Where((_, i) => Math.Abs(hpPercentage[i] - maxPercentage) < double.Epsilon).ToList();

        if (tiedSides.Count <= 1)
        {
            return Win(tiedSides.FirstOrDefault());
        }

        // Calculate total HP
        var hpTotal = tiedSides.Select(side =>
            side.Pokemon.Sum(pokemon => pokemon.Hp)
        ).ToList();

        // Display total HP per side
        if (DisplayUi)
        {
            string hpTotalMessage = string.Join("; ",
                tiedSides.Select((side, i) => $"{side.Name}: {hpTotal[i]} total HP left")
            );
            Add("-message", hpTotalMessage);
        }

        // Filter sides with maximum total HP
        int maxTotal = hpTotal.Max();
        tiedSides = tiedSides.Where((_, i) => hpTotal[i] == maxTotal).ToList();

        if (tiedSides.Count <= 1)
        {
            return Win(tiedSides.FirstOrDefault());
        }

        return Tie();
    }

    public bool ForceWin(SideId? side = null)
    {
        // Battle already ended - cannot force a win
        if (Ended) return false;

        // Log the force win/tie command to input log
        string logEntry = side.HasValue ? $"> forcewin {side.Value}" : "> forcetie";
        InputLog.Add(logEntry);

        // Delegate to the Win method to handle the actual logic
        return Win(side);
    }

    public bool Tie()
    {
        return Win((Side?)null);
    }

    public bool Win(SideId? side = null)
    {
        // Convert Side to Side if provided
        Side? winningSide = side.HasValue ? GetSide(side.Value) : null;
        return Win(winningSide);
    }

    public bool Win(Side? side = null)
    {
        // Battle already ended
        if (Ended) return false;

        // Validate the side exists in the battle
        if (side != null && !Sides.Contains(side))
        {
            side = null;
        }

        Winner = side?.Name ?? string.Empty;

        // Record battle end in history
        History.RecordBattleEnd(Winner);

        if (DisplayUi)
        {
            // Print empty line for formatting
            Add(string.Empty);

            // Print the appropriate win/tie message
            // Note: AllySide is not implemented in this codebase (see Side class)
            // The original TypeScript code checks for side?.allySide here
            if (side != null)
            {
                // Single side wins
                Add("win", side.Name);
            }
            else
            {
                // Tie
                Add("tie");
            }
        }

        // End the battle
        Ended = true;
        RequestState = RequestState.None;

        // Clear all active requests
        foreach (Side s in Sides)
        {
            s.ActiveRequest = null;
        }

        // Emit battle ended event to notify simulators
        EmitBattleEnded();

        return true;
    }

    public bool Lose(SideId sideId)
    {
        Side side = GetSide(sideId);
        return Lose(side);
    }

    public bool Lose(Side? side)
    {
        // Can happen if a battle crashes
        if (side is null) return false;

        // Already no Pokémon left
        if (side.PokemonLeft <= 0) return false;

        // Force the side to lose by setting their Pokémon count to 0
        side.PokemonLeft = 0;

        // Faint the first active Pokémon if present
        Pokemon? firstActive = side.Active.FirstOrDefault(p => p != null);
        firstActive?.Faint();

        // Show faint messages (lastFirst: false, forceCheck: true)
        FaintMessages(lastFirst: false, forceCheck: true);

        // Update requests if battle hasn't ended and this side had an active request
        if (!Ended && side.ActiveRequest != null)
        {
            // Send a wait request
            side.EmitRequest(new WaitRequest
            {
                Side = side.GetRequestData(),
            });

            // Clear any pending choices
            side.ClearChoice();

            // Commit choices if all sides are done choosing
            if (AllChoicesDone())
            {
                CommitChoices();
            }
        }

        return true;
    }
}