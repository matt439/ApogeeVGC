//hitStepTryHitEvent(targets: Pokemon[], pokemon: Pokemon, move: ActiveMove) {
//    const hitResults = this.battle.runEvent('TryHit', targets, pokemon, move);
//    if (!hitResults.includes(true) && hitResults.includes(false))
//    {
//        this.battle.add('-fail', pokemon);
//        this.battle.attrLastMove('[still]');
//    }
//    for (const i of targets.keys()) {
//        if (hitResults[i] !== this.battle.NOT_FAIL) hitResults[i] = hitResults[i] || false;
//    }
//    return hitResults;
//}
//hitStepTypeImmunity(targets: Pokemon[], pokemon: Pokemon, move: ActiveMove) {
//    if (move.ignoreImmunity === undefined)
//    {
//        move.ignoreImmunity = (move.category === 'Status');
//    }

//    const hitResults = [];
//    for (const i of targets.keys()) {
//        hitResults[i] = targets[i].runImmunity(move, !move.smartTarget);
//    }

//    return hitResults;
//}