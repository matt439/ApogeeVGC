using ApogeeVGC.Sim.Actions;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC.Sim.BattleClasses;

public class BattleQueue(IBattle battle)
{
    public IBattle Battle { get; init; } = battle;
    public List<IAction> List { get; init; } = [];

    public IAction? Shift()
    {
        if (List.Count == 0) return null;
        IAction action = List[0];
        List.RemoveAt(0);
        return action;
    }

    public IAction? Peek(bool end = false)
    {
        if (List.Count == 0) return null;
        return end ? List[^1] : List[0];
    }

    public void Push(IAction action)
    {
        List.Add(action);
    }

    public void Unshift(IAction action)
    {
        List.Insert(0, action);
    }

    public IEnumerator<IAction> GetEnumerator()
    {
        return List.GetEnumerator();
    }

    public IEnumerable<(int Index, IAction Action)> Entries()
    {
        return List.Select((action, index) => (index, action));
    }

    //    /**
    //	 * Takes an ActionChoice, and fills it out into a full Action object.
    //	 *
    //	 * Returns an array of Actions because some ActionChoices (like mega moves)
    //	 * resolve to two Actions (mega evolution + use move)
    //	 */
    //    resolveAction(action: ActionChoice, midTurn = false) : Action[] {
    //		if (!action) throw new Error(`Action not passed to resolveAction`);
    //		if (action.choice === 'pass') return [];
    //		const actions = [action];

    //		if (!action.side && action.pokemon) action.side = action.pokemon.side;
    //		if (!action.move && action.moveid) action.move = this.battle.dex.getActiveMove(action.moveid);
    //		if (!action.order) {
    //			const orders: { [choice: string]: number
    //} = {
    //				team: 1,
    //				start: 2,
    //				instaswitch: 3,
    //				beforeTurn: 4,
    //				beforeTurnMove: 5,
    //				revivalblessing: 6,

    //				runSwitch: 101,
    //				switch: 103,
    //				megaEvo: 104,
    //				megaEvoX: 104,
    //				megaEvoY: 104,
    //				runDynamax: 105,
    //				terastallize: 106,
    //				priorityChargeMove: 107,

    //				shift: 200,
    //				// default is 200 (for moves)

    //				residual: 300,
    //			};
    //if (action.choice in orders) {
    //				action.order = orders[action.choice] ;
    //			} else
    //{
    //    action.order = 200;
    //    if (! ['move', 'event'].includes(action.choice))
    //    {
    //        throw new Error(`Unexpected orderless action ${ action.choice }`);
    //    }
    //}
    //		}
    //		if (!midTurn)
    //{
    //    if (action.choice === 'move')
    //    {
    //        if (!action.maxMove && !action.zmove && action.move.beforeTurnCallback)
    //        {
    //            actions.unshift(...this.resolveAction({
    //            choice: 'beforeTurnMove', pokemon: action.pokemon, move: action.move, targetLoc: action.targetLoc,
    //					}));
    //        }
    //        if (action.mega && !action.pokemon.isSkyDropped())
    //        {
    //            actions.unshift(...this.resolveAction({
    //            choice: 'megaEvo',
    //						pokemon: action.pokemon,
    //					}));
    //        }
    //        if (action.megax && !action.pokemon.isSkyDropped())
    //        {
    //            actions.unshift(...this.resolveAction({
    //            choice: 'megaEvoX',
    //						pokemon: action.pokemon,
    //					}));
    //        }
    //        if (action.megay && !action.pokemon.isSkyDropped())
    //        {
    //            actions.unshift(...this.resolveAction({
    //            choice: 'megaEvoY',
    //						pokemon: action.pokemon,
    //					}));
    //        }
    //        if (action.terastallize && !action.pokemon.terastallized)
    //        {
    //            actions.unshift(...this.resolveAction({
    //            choice: 'terastallize',
    //						pokemon: action.pokemon,
    //					}));
    //        }
    //        if (action.maxMove && !action.pokemon.volatiles['dynamax'])
    //        {
    //            actions.unshift(...this.resolveAction({
    //            choice: 'runDynamax',
    //						pokemon: action.pokemon,
    //					}));
    //        }
    //        if (!action.maxMove && !action.zmove && action.move.priorityChargeCallback)
    //        {
    //            actions.unshift(...this.resolveAction({
    //            choice: 'priorityChargeMove',
    //						pokemon: action.pokemon,
    //						move: action.move,
    //					}));
    //        }
    //        action.fractionalPriority = this.battle.runEvent('FractionalPriority', action.pokemon, null, action.move, 0);
    //    }
    //    else if (['switch', 'instaswitch'].includes(action.choice))
    //    {
    //        if (typeof action.pokemon.switchFlag === 'string')
    //        {
    //            action.sourceEffect = this.battle.dex.moves.get(action.pokemon.switchFlag as ID) as any;
    //        }
    //        action.pokemon.switchFlag = false;
    //    }
    //}

    //const deferPriority = this.battle.gen === 7 && action.mega && action.mega !== 'done';
    //if (action.move)
    //{
    //    let target = null;
    //    action.move = this.battle.dex.getActiveMove(action.move);

    //    if (!action.targetLoc)
    //    {
    //        target = this.battle.getRandomTarget(action.pokemon, action.move);
    //        // TODO: what actually happens here?
    //        if (target) action.targetLoc = action.pokemon.getLocOf(target);
    //    }
    //    action.originalTarget = action.pokemon.getAtLoc(action.targetLoc);
    //}
    //if (!deferPriority) this.battle.getActionSpeed(action);
    //return actions as any;
    //	}

    public List<IAction> ResolveAction(IActionChoice action, bool midTurn = false)
    {
        throw new NotImplementedException();
    }

    public IAction? WillAct()
    {
        foreach (IAction action in List)
        {
            if (action.Choice is ActionId.Move or ActionId.Switch or ActionId.InstaSwitch or ActionId.Shift)
            {
                return action;
            }
        }
        return null;
    }
}