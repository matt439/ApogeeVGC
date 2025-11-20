using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    public bool MaybeTriggerEndlessBattleClause(List<bool> trappedBySide, List<StalenessId?> stalenessBySide)
    {
        // Skip if still under the 100 turn minimum
        if (Turn <= 100) return false;

        // Turn limit check (not part of Endless Battle Clause, but hard limit)
        // Only enforced if MaxTurns is set (non-null and > 0)
        // Throws an exception instead of ending as a tie so infinite loops can be debugged
        if (MaxTurns.HasValue && MaxTurns.Value > 0 && Turn >= MaxTurns.Value)
        {
            if (DisplayUi)
            {
                Add("-message", $"It is turn {MaxTurns.Value}. Turn limit exceeded!");
            }
            
            // Throw exception to surface the infinite loop for debugging
            throw new BattleTurnLimitException(Turn, MaxTurns.Value);
        }

        // Warning messages for approaching turn limit (only if MaxTurns is enforced)
        if (MaxTurns.HasValue && MaxTurns.Value > 0)
        {
            int turnsLeft = MaxTurns.Value - Turn;
            bool showWarning = (Turn >= 500 && Turn % 100 == 0) || // Every 100 turns past turn 500
                               (Turn >= MaxTurns.Value - 100 && Turn % 10 == 0) || // Every 10 turns in last 100
                               (turnsLeft <= 10 && turnsLeft > 0); // Every turn in last 10

            if (showWarning && DisplayUi)
            {
                string turnsLeftText = turnsLeft == 1 ? "1 turn" : $"{turnsLeft} turns";
                Add("bigerror", $"You will auto-tie if the battle doesn't end in {turnsLeftText} (on turn {MaxTurns.Value}).");
            }
        }

        // Check if Endless Battle Clause rule is enabled
        if (!RuleTable.Has(RuleId.EndlessBattleClause)) return false;

        // Are all Pokemon on every side stale, with at least one side containing an externally stale Pokemon?
        if (!stalenessBySide.All(s => s.HasValue) ||
            stalenessBySide.All(s => s != StalenessId.External))
        {
            return false;
        }

        // Can both sides switch to a non-stale Pokemon?
        var canSwitch = new List<bool>();
        for (int i = 0; i < trappedBySide.Count; i++)
        {
            bool trapped = trappedBySide[i];
            canSwitch.Add(false);

            if (trapped) continue;

            Side side = Sides[i];

            foreach (Pokemon pokemon in side.Pokemon)
            {
                if (pokemon is not { Fainted: false, VolatileStaleness: null, Staleness: null }) continue;
                canSwitch[i] = true;
                break;
            }
        }

        // If both sides can switch to non-stale Pokemon, clause doesn't trigger
        if (canSwitch.All(s => s)) return false;

        // Endless Battle Clause activates - determine winner by checking for restorative berry cycling
        var losers = new List<Side>();

        foreach (Side side in Sides)
        {
            bool berry = false;  // Has restorative berry
            bool cycle = false;  // Has Harvest/Pickup ability or Recycle move

            foreach (Pokemon pokemon in side.Pokemon)
            {
                // Check if Pokemon has a restorative berry
                berry = Pokemon.RestorativeBerries.Contains(pokemon.Set.Item);

                // Check if Pokemon has cycling ability (Harvest/Pickup)
                if (pokemon.Set.Ability is AbilityId.Harvest or AbilityId.Pickup)
                {
                    cycle = true;
                }

                // Check if Pokemon has Recycle move
                if (pokemon.Set.Moves.Contains(MoveId.Recycle))
                {
                    cycle = true;
                }

                // If both conditions are met, this side loses
                if (berry && cycle) break;
            }

            if (berry && cycle)
            {
                losers.Add(side);
            }
        }

        // Determine outcome based on number of losing sides
        if (losers.Count == 1)
        {
            Side loser = losers[0];
            if (DisplayUi)
            {
                Add("-message",
                    $"{loser.Name}'s team started with the rudimentary means to perform " +
                    "restorative berry-cycling and thus loses.");
            }
            return Win(loser.Foe);
        }

        if (losers.Count == Sides.Count)
        {
            if (DisplayUi)
            {
                Add("-message",
                    "Each side's team started with the rudimentary means to perform " +
                    "restorative berry-cycling.");
            }
        }

        return Tie();
    }
}