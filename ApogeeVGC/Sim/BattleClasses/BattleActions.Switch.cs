using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleActions
{
    public bool SwitchIn(Pokemon pokemon, int pos, IEffect? sourceEffect = null, bool isDrag = false)
    {
        // Validate the Pokemon exists and is not already active
        if (pokemon.IsActive)
        {
            Battle.Hint("A switch failed because the Pokémon trying to switch in is already in.");
            return false;
        }

        Side side = pokemon.Side;

        // Validate the switch position
        if (pos >= side.Active.Count)
        {
            throw new ArgumentException($"Invalid switch position {pos} / {side.Active.Count}");
        }

        // Handle initial switch-in where Active[pos] is null
        Pokemon? oldActive = side.Active[pos];
        Pokemon? unfaintedActive = oldActive?.Hp > 0 ? oldActive : null;

        if (unfaintedActive != null)
        {
            oldActive!.BeingCalledBack = true;

            // Determine if we need to copy volatiles (for moves like U-turn, Shed Tail, etc.)
            MoveSelfSwitch? switchCopyFlag = null;
            if (sourceEffect is ActiveMove { SelfSwitch: not null } move)
            {
                switchCopyFlag = move.SelfSwitch;
            }

            // Run BeforeSwitchOut event unless it's a drag or the flag is set
            if (!oldActive.SkipBeforeSwitchOutEventFlag && !isDrag)
            {
                Battle.RunEvent(EventId.BeforeSwitchOut, oldActive);
                if (Battle.Gen >= 5)
                {
                    Battle.EachEvent(EventId.Update);
                }
            }
            oldActive.SkipBeforeSwitchOutEventFlag = false;

            // Run SwitchOut event - can be interrupted in custom formats
            RelayVar? switchOutResult = Battle.RunEvent(EventId.SwitchOut, oldActive);
            if (switchOutResult is BoolRelayVar { Value: false })
            {
                // Warning: DO NOT interrupt a switch-out if you just want to trap a pokemon.
                // To trap a pokemon and prevent it from switching out, (e.g. Mean Look, Magnet Pull)
                // use the 'trapped' flag instead.

                // Note: Nothing in the real games can interrupt a switch-out (except Pursuit KOing,
                // which is handled elsewhere); this is just for custom formats.
                return false;
            }

            // Will definitely switch out at this point

            // Clear Illusion
            oldActive.Illusion = null;

            // End ability and item effects
            Battle.SingleEvent(EventId.End, oldActive.GetAbility(), oldActive.AbilityState, (SingleEventTarget)oldActive);
            Battle.SingleEvent(EventId.End, oldActive.GetItem(), oldActive.ItemState, (SingleEventTarget)oldActive);

            // If a pokemon is forced out by Whirlwind/etc or Eject Button/Pack, it can't use its chosen move
            Battle.Queue.CancelAction(oldActive);

            // Gen 4 special case: preserve last move
            Move? newMove = null;
            if (Battle.Gen == 4 && sourceEffect != null)
            {
                newMove = oldActive.LastMove;
            }

            // Copy volatiles if needed (U-turn, Shed Tail, etc.)
            if (switchCopyFlag != null)
            {
                switch (switchCopyFlag)
                {
                    case ShedTailMoveSelfSwitch:
                        pokemon.CopyVolatileFrom(oldActive, ConditionId.ShedTail);
                        break;
                    default:
                        pokemon.CopyVolatileFrom(oldActive, false);
                        break;
                }
            }

            // Restore last move if we preserved it
            if (newMove != null)
            {
                pokemon.LastMove = newMove;
            }

            // Clear all volatiles from the old active Pokemon
            oldActive.ClearVolatile();
        }

        // Update Pokemon states and positions
        if (oldActive != null)
        {
            oldActive.IsActive = false;
            oldActive.IsStarted = false;
            oldActive.UsedItemThisTurn = false;
            oldActive.StatsRaisedThisTurn = false;
            oldActive.StatsLoweredThisTurn = false;
            oldActive.Position = pokemon.Position;

            // Clear status if fainted
            if (oldActive.Fainted)
            {
                oldActive.Status = ConditionId.None;
            }

            // Gen 4 and earlier: transfer last item
            if (Battle.Gen <= 4)
            {
                pokemon.LastItem = oldActive.LastItem;
                oldActive.LastItem = ItemId.None;
            }

            // Swap positions in the side's Pokemon list
            side.Pokemon[oldActive.Position] = oldActive;
        }

        // CRITICAL: Capture perspective BEFORE placing new Pokemon in active slot
        // This ensures the switch message shows the "before" state (fainted Pokemon still visible)
        BattlePerspective? preSwitchPerspective = null;
        if (Battle.DisplayUi)
        {
            BattlePerspectiveType perspectiveType = Battle.RequestState == RequestState.TeamPreview
                ? BattlePerspectiveType.TeamPreview
                : BattlePerspectiveType.InBattle;
            preSwitchPerspective = Battle.GetPerspectiveForSide(SideId.P1, perspectiveType);
        }

        // Swap positions in the side's Pokemon list
        pokemon.Position = pos;
        side.Pokemon[pokemon.Position] = pokemon;

        // Activate the new Pokemon
        pokemon.IsActive = true;
        side.Active[pos] = pokemon;
        pokemon.ActiveTurns = 0;
        pokemon.ActiveMoveActions = 0;

        // Reset move usage flags
        foreach (MoveSlot moveSlot in pokemon.MoveSlots)
        {
            moveSlot.Used = false;
        }

        pokemon.AbilityState.EffectOrder = Battle.EffectOrder++;
        pokemon.ItemState.EffectOrder = Battle.EffectOrder++;

        // Run BeforeSwitchIn event
        Battle.RunEvent(EventId.BeforeSwitchIn, pokemon);

        // Add switch/drag message to battle log with pre-captured perspective
        if (Battle.DisplayUi && preSwitchPerspective != null)
        {
            if (sourceEffect != null)
            {
                Battle.AddWithPerspective(preSwitchPerspective, isDrag ? "drag" : "switch", pokemon, pokemon.GetFullDetails,
                    $"[from] {sourceEffect.EffectStateId}");
            }
            else
            {
                Battle.AddWithPerspective(preSwitchPerspective, isDrag ? "drag" : "switch", pokemon, pokemon.GetFullDetails);
            }
        }

        pokemon.PreviouslySwitchedIn++;

        // Schedule RunSwitch action
        if (isDrag && Battle.Gen >= 5)
        {
            // runSwitch happens immediately so that Mold Breaker can make hazards bypass Clear Body and Levitate
            RunSwitch(pokemon);
        }
        else
        {
            Battle.Queue.InsertChoice(new RunSwitchAction
            {
                Pokemon = pokemon,
            });
        }

        return true;
    }

    public bool DragIn(Side side, int pos)
    {
        Pokemon? pokemon = Battle.GetRandomSwitchable(side);
        if (pokemon == null || pokemon.IsActive)
        {
            return false;
        }

        Pokemon? oldActive = side.Active[pos];
        if (oldActive == null)
        {
            throw new InvalidOperationException("Nothing to drag out");
        }

        if (oldActive.Hp <= 0)
        {
            return false;
        }

        // Pass default relayVar of true so "no handler" = "allow drag-out"
        RelayVar? dragOutResult = Battle.RunEvent(EventId.DragOut, oldActive, null, null,
            BoolRelayVar.True);

        // null = prevent silently (Suction Cups), false = prevent
        if (dragOutResult is null or BoolRelayVar { Value: false })
        {
            return false;
        }

        return SwitchIn(pokemon, pos, null, true);
    }

    public bool RunSwitch(Pokemon pokemon)
    {
        // Flush any pending events (like faint messages) BEFORE processing the switch
        // This ensures the GUI sees the fainted Pokemon before the replacement appears
        Battle.FlushEvents();

        List<Pokemon> switchersIn = [pokemon];

        while (Battle.Queue.Peek() is RunSwitchAction)
        {
            IAction? nextSwitch = Battle.Queue.Shift();
            if (nextSwitch is RunSwitchAction runSwitchAction)
            {
                switchersIn.Add(runSwitchAction.Pokemon ?? throw new InvalidOperationException("Pokemon must" +
                    "not be null here."));
            }
        }

        var allActive = Battle.GetAllActive(true);
        Battle.SpeedSort(allActive);
        Battle.SpeedOrder = allActive.Select(a => a.Side.N * a.Battle.Sides.Count + a.Position).ToList();
        Battle.FieldEvent(EventId.SwitchIn, switchersIn);

        foreach (Pokemon poke in switchersIn.Where(poke => poke.Hp > 0))
        {
            poke.IsStarted = true;
            poke.DraggedIn = null;
        }

        // Don't flush here - switch messages were already added with correct pre-captured perspectives
        // Flushing here would send a perspective showing the new Pokemon already in place
        // The next FlushEvents() call will happen naturally in the battle loop

        return true;
    }
}