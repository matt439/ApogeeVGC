using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

//runMoveEffects(
//        damage: SpreadMoveDamage, targets: SpreadMoveTargets, source: Pokemon,
//        move: ActiveMove, moveData: ActiveMove, isSecondary ?: boolean, isSelf ?: boolean
//    ) {
//    let didAnything: number | boolean | null | undefined = damage.reduce(this.combineResults);
//    for (const [i, target] of targets.entries()) {
//        if (target === false) continue;
//        let hitResult;
//        let didSomething: number | boolean | null | undefined = undefined;

//        if (target)
//        {
//            if (moveData.boosts && !target.fainted)
//            {
//                hitResult = this.battle.boost(moveData.boosts, target, source, move, isSecondary, isSelf);
//                didSomething = this.combineResults(didSomething, hitResult);
//            }
//            if (moveData.heal && !target.fainted)
//            {
//                if (target.hp >= target.maxhp)
//                {
//                    this.battle.add('-fail', target, 'heal');
//                    this.battle.attrLastMove('[still]');
//                    damage[i] = this.combineResults(damage[i], false);
//                    didAnything = this.combineResults(didAnything, null);
//                    continue;
//                }
//                const amount = target.baseMaxhp * moveData.heal[0] / moveData.heal[1];
//                const d = this.battle.heal((this.battle.gen < 5 ? Math.floor : Math.round)(amount), target, source, move);
//                if (!d && d !== 0)
//                {
//                    if (d !== null)
//                    {
//                        this.battle.add('-fail', source);
//                        this.battle.attrLastMove('[still]');
//                    }
//                    this.battle.debug('heal interrupted');
//                    damage[i] = this.combineResults(damage[i], false);
//                    didAnything = this.combineResults(didAnything, null);
//                    continue;
//                }
//                didSomething = true;
//            }
//            if (moveData.status)
//            {
//                hitResult = target.trySetStatus(moveData.status, source, moveData.ability ? moveData.ability : move);
//                if (!hitResult && move.status)
//                {
//                    damage[i] = this.combineResults(damage[i], false);
//                    didAnything = this.combineResults(didAnything, null);
//                    continue;
//                }
//                didSomething = this.combineResults(didSomething, hitResult);
//            }
//            if (moveData.forceStatus)
//            {
//                hitResult = target.setStatus(moveData.forceStatus, source, move);
//                didSomething = this.combineResults(didSomething, hitResult);
//            }
//            if (moveData.volatileStatus)
//            {
//                hitResult = target.addVolatile(moveData.volatileStatus, source, move);
//                didSomething = this.combineResults(didSomething, hitResult);
//            }
//            if (moveData.sideCondition)
//            {
//                hitResult = target.side.addSideCondition(moveData.sideCondition, source, move);
//                didSomething = this.combineResults(didSomething, hitResult);
//            }
//            if (moveData.slotCondition)
//            {
//                hitResult = target.side.addSlotCondition(target, moveData.slotCondition, source, move);
//                didSomething = this.combineResults(didSomething, hitResult);
//            }
//            if (moveData.weather)
//            {
//                hitResult = this.battle.field.setWeather(moveData.weather, source, move);
//                didSomething = this.combineResults(didSomething, hitResult);
//            }
//            if (moveData.terrain)
//            {
//                hitResult = this.battle.field.setTerrain(moveData.terrain, source, move);
//                didSomething = this.combineResults(didSomething, hitResult);
//            }
//            if (moveData.pseudoWeather)
//            {
//                hitResult = this.battle.field.addPseudoWeather(moveData.pseudoWeather, source, move);
//                didSomething = this.combineResults(didSomething, hitResult);
//            }
//            if (moveData.forceSwitch)
//            {
//                hitResult = !!this.battle.canSwitch(target.side);
//                didSomething = this.combineResults(didSomething, hitResult);
//            }
//            // Hit events
//            //   These are like the TryHit events, except we don't need a FieldHit event.
//            //   Scroll up for the TryHit event documentation, and just ignore the "Try" part. ;)
//            if (move.target === 'all' && !isSelf)
//            {
//                if (moveData.onHitField)
//                {
//                    hitResult = this.battle.singleEvent('HitField', moveData, { }, target, source, move);
//                    didSomething = this.combineResults(didSomething, hitResult);
//                }
//            }
//            else if ((move.target === 'foeSide' || move.target === 'allySide') && !isSelf)
//            {
//                if (moveData.onHitSide)
//                {
//                    hitResult = this.battle.singleEvent('HitSide', moveData, { }, target.side, source, move);
//                    didSomething = this.combineResults(didSomething, hitResult);
//                }
//            }
//            else
//            {
//                if (moveData.onHit)
//                {
//                    hitResult = this.battle.singleEvent('Hit', moveData, { }, target, source, move);
//                    didSomething = this.combineResults(didSomething, hitResult);
//                }
//                if (!isSelf && !isSecondary)
//                {
//                    this.battle.runEvent('Hit', target, source, move);
//                }
//            }
//        }
//        if (moveData.selfdestruct === 'ifHit' && damage[i] !== false)
//        {
//            this.battle.faint(source, source, move);
//        }
//        if (moveData.selfSwitch)
//        {
//            if (this.battle.canSwitch(source.side) && !source.volatiles['commanded'])
//            {
//                didSomething = true;
//            }
//            else
//            {
//                didSomething = this.combineResults(didSomething, false);
//            }
//        }
//        // Move didn't fail because it didn't try to do anything
//        if (didSomething === undefined) didSomething = true;
//        damage[i] = this.combineResults(damage[i], didSomething === null ? false : didSomething);
//        didAnything = this.combineResults(didAnything, didSomething);
//    }

//    if (!didAnything && didAnything !== 0 && !moveData.self && !moveData.selfdestruct)
//    {
//        if (!isSelf && !isSecondary)
//        {
//            if (didAnything === false)
//            {
//                this.battle.add('-fail', source);
//                this.battle.attrLastMove('[still]');
//            }
//        }
//        this.battle.debug('move failed because it did nothing');
//    }
//    else if (move.selfSwitch && source.hp && !source.volatiles['commanded'])
//    {
//        source.switchFlag = move.id;
//    }

//    return damage;
//}