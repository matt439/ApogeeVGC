
//using ApogeeVGC.Sim.Moves;
//using ApogeeVGC.Sim.PokemonClasses;
//using ApogeeVGC.Sim.Utils;
//using static System.Runtime.InteropServices.JavaScript.JSType;

///** NOTE: includes single-target moves */
//trySpreadMoveHit(targets: Pokemon[], pokemon: Pokemon, move: ActiveMove, notActive ?: boolean) {
//    if (targets.length > 1 && !move.smartTarget) move.spreadHit = true;

//    const moveSteps: ((targets: Pokemon[], pokemon: Pokemon, move: ActiveMove) =>
//		(number | boolean | "" | undefined)[] | undefined)[] = [
//			// 0. check for semi invulnerability
//			this.hitStepInvulnerabilityEvent,

//			// 1. run the 'TryHit' event (Protect, Magic Bounce, Volt Absorb, etc.) (this is step 2 in gens 5 & 6, and step 4 in gen 4)
//			this.hitStepTryHitEvent,

//			// 2. check for type immunity (this is step 1 in gens 4-6)
//			this.hitStepTypeImmunity,

//			// 3. check for various move-specific immunities
//			this.hitStepTryImmunity,

//			// 4. check accuracy
//			this.hitStepAccuracy,

//			// 5. break protection effects
//			this.hitStepBreakProtect,

//			// 6. steal positive boosts (Spectral Thief)
//			this.hitStepStealBoosts,

//			// 7. loop that processes each hit of the move (has its own steps per iteration)
//			this.hitStepMoveHitLoop,
//        ];
//    if (this.battle.gen <= 6)
//    {
//    // Swap step 1 with step 2
//    [moveSteps[1], moveSteps[2]] = [moveSteps[2], moveSteps[1]];
//    }
//    if (this.battle.gen === 4)
//    {
//    // Swap step 4 with new step 2 (old step 1)
//    [moveSteps[2], moveSteps[4]] = [moveSteps[4], moveSteps[2]];
//    }

//    if (notActive) this.battle.setActiveMove(move, pokemon, targets[0]);

//    const hitResult = this.battle.singleEvent('Try', move, null, pokemon, targets[0], move) &&
//        this.battle.singleEvent('PrepareHit', move, { }, targets[0], pokemon, move) &&
//        this.battle.runEvent('PrepareHit', pokemon, targets[0], move);
//    if (!hitResult)
//    {
//        if (hitResult === false)
//        {
//            this.battle.add('-fail', pokemon);
//            this.battle.attrLastMove('[still]');
//        }
//        return hitResult === this.battle.NOT_FAIL;
//    }

//    let atLeastOneFailure = false;
//    for (const step of moveSteps) {
//        const hitResults: (number | boolean | "" | undefined)[] | undefined = step.call(this, targets, pokemon, move);
//        if (!hitResults) continue;
//        targets = targets.filter((val, i) => hitResults[i] || hitResults[i] === 0);
//        atLeastOneFailure = atLeastOneFailure || hitResults.some(val => val === false);
//        if (move.smartTarget && atLeastOneFailure) move.smartTarget = false;
//        if (!targets.length)
//        {
//            // console.log(step.name);
//            break;
//        }
//    }

//    move.hitTargets = targets;
//    const moveResult = !!targets.length;
//    if (!moveResult && !atLeastOneFailure) pokemon.moveThisTurnResult = null;
//    const hitSlot = targets.map(p => p.getSlot());
//    if (move.spreadHit) this.battle.attrLastMove('[spread] ' + hitSlot.join(','));
//    return moveResult;
//}