//getSpreadDamage(
//    damage: SpreadMoveDamage, targets: SpreadMoveTargets, source: Pokemon,
//    move: ActiveMove, moveData: ActiveMove, isSecondary ?: boolean, isSelf ?: boolean
//    ): SpreadMoveDamage {
//    for (const [i, target] of targets.entries()) {
//        if (!target) continue;
//        this.battle.activeTarget = target;
//        damage[i] = undefined;
//        const curDamage = this.getDamage(source, target, moveData);
//        // getDamage has several possible return values:
//        //
//        //   a number:
//        //     means that much damage is dealt (0 damage still counts as dealing
//        //     damage for the purposes of things like Static)
//        //   false:
//        //     gives error message: "But it failed!" and move ends
//        //   null:
//        //     the move ends, with no message (usually, a custom fail message
//        //     was already output by an event handler)
//        //   undefined:
//        //     means no damage is dealt and the move continues
//        //
//        // basically, these values have the same meanings as they do for event
//        // handlers.

//        if (curDamage === false || curDamage === null)
//        {
//            if (damage[i] === false && !isSecondary && !isSelf)
//            {
//                this.battle.add('-fail', source);
//                this.battle.attrLastMove('[still]');
//            }
//            this.battle.debug('damage calculation interrupted');
//            damage[i] = false;
//            continue;
//        }
//        damage[i] = curDamage;
//    }
//    return damage;
//}