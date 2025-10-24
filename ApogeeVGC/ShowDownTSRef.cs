//useMoveInner(
//        moveOrMoveName: Move | string, pokemon: Pokemon, options ?: {
//    target ?: Pokemon | null, sourceEffect ?: Effect | null,
//			zMove ?: string, maxMove ?: string,
//		},
//	) {
//    let target = options?.target;
//    let sourceEffect = options?.sourceEffect;
//    const zMove = options?.zMove;
//    const maxMove = options?.maxMove;
//    if (!sourceEffect && this.battle.effect.id) sourceEffect = this.battle.effect;
//    if (sourceEffect && ['instruct', 'custapberry'].includes(sourceEffect.id)) sourceEffect = null;

//    let move = this.dex.getActiveMove(moveOrMoveName);
//    pokemon.lastMoveUsed = move;
//    if (move.id === 'weatherball' && zMove)
//    {
//        // Z-Weather Ball only changes types if it's used directly,
//        // not if it's called by Z-Sleep Talk or something.
//        this.battle.singleEvent('ModifyType', move, null, pokemon, target, move, move);
//        if (move.type !== 'Normal') sourceEffect = move;
//    }
//    if (zMove || (move.category !== 'Status' && sourceEffect && (sourceEffect as ActiveMove).isZ))
//    {
//        move = this.getActiveZMove(move, pokemon);
//    }
//    if (maxMove && move.category !== 'Status')
//    {
//        // Max move outcome is dependent on the move type after type modifications from ability and the move itself
//        this.battle.singleEvent('ModifyType', move, null, pokemon, target, move, move);
//        this.battle.runEvent('ModifyType', pokemon, target, move, move);
//    }
//    if (maxMove || (move.category !== 'Status' && sourceEffect && (sourceEffect as ActiveMove).isMax))
//    {
//        move = this.getActiveMaxMove(move, pokemon);
//    }

//    if (this.battle.activeMove)
//    {
//        move.priority = this.battle.activeMove.priority;
//        if (!move.hasBounced) move.pranksterBoosted = this.battle.activeMove.pranksterBoosted;
//    }
//    const baseTarget = move.target;
//    let targetRelayVar = { target };
//    targetRelayVar = this.battle.runEvent('ModifyTarget', pokemon, target, move, targetRelayVar, true);
//    if (targetRelayVar.target !== undefined) target = targetRelayVar.target;
//    if (target === undefined) target = this.battle.getRandomTarget(pokemon, move);
//    if (move.target === 'self' || move.target === 'allies')
//    {
//        target = pokemon;
//    }
//    if (sourceEffect)
//    {
//        move.sourceEffect = sourceEffect.id;
//        move.ignoreAbility = (sourceEffect as ActiveMove).ignoreAbility;
//    }
//    let moveResult = false;

//    this.battle.setActiveMove(move, pokemon, target);

//    this.battle.singleEvent('ModifyType', move, null, pokemon, target, move, move);
//    this.battle.singleEvent('ModifyMove', move, null, pokemon, target, move, move);
//    if (baseTarget !== move.target)
//    {
//        // Target changed in ModifyMove, so we must adjust it here
//        // Adjust before the next event so the correct target is passed to the
//        // event
//        target = this.battle.getRandomTarget(pokemon, move);
//    }
//    move = this.battle.runEvent('ModifyType', pokemon, target, move, move);
//    move = this.battle.runEvent('ModifyMove', pokemon, target, move, move);
//    if (baseTarget !== move.target)
//    {
//        // Adjust again
//        target = this.battle.getRandomTarget(pokemon, move);
//    }
//    if (!move || pokemon.fainted)
//    {
//        return false;
//    }

//    let attrs = '';

//    let movename = move.name;
//    if (move.id === 'hiddenpower') movename = 'Hidden Power';
//    if (sourceEffect) attrs += `| [from] ${ sourceEffect.fullname}`;
//    if (zMove && move.isZ === true)
//    {
//        attrs = `| [anim]${ movename}${ attrs}`;
//        movename = `Z -${ movename}`;
//    }
//    this.battle.addMove('move', pokemon, movename, `${ target}${ attrs}`);

//    if (zMove) this.runZPower(move, pokemon);

//    if (!target)
//    {
//        this.battle.attrLastMove('[notarget]');
//        this.battle.add(this.battle.gen >= 5 ? '-fail' : '-notarget', pokemon);
//        return false;
//    }

//    const { targets, pressureTargets } = pokemon.getMoveTargets(move, target);
//    if (targets.length)
//    {
//        target = targets[targets.length - 1]; // in case of redirection
//    }

//    const callerMoveForPressure = sourceEffect && (sourceEffect as ActiveMove).pp ? sourceEffect as ActiveMove : null;
//    if (!sourceEffect || callerMoveForPressure || sourceEffect.id === 'pursuit')
//    {
//        let extraPP = 0;
//        for (const source of pressureTargets) {
//            const ppDrop = this.battle.runEvent('DeductPP', source, pokemon, move);
//            if (ppDrop !== true)
//            {
//                extraPP += ppDrop || 0;
//            }
//        }
//        if (extraPP > 0)
//        {
//            pokemon.deductPP(callerMoveForPressure || moveOrMoveName, extraPP);
//        }
//    }

//    if (!this.battle.singleEvent('TryMove', move, null, pokemon, target, move) ||
//        !this.battle.runEvent('TryMove', pokemon, target, move))
//    {
//        move.mindBlownRecoil = false;
//        return false;
//    }

//    this.battle.singleEvent('UseMoveMessage', move, null, pokemon, target, move);

//    if (move.ignoreImmunity === undefined)
//    {
//        move.ignoreImmunity = (move.category === 'Status');
//    }

//    if (this.battle.gen !== 4 && move.selfdestruct === 'always')
//    {
//        this.battle.faint(pokemon, pokemon, move);
//    }

//    let damage: number | false | undefined | '' = false;
//    if (move.target === 'all' || move.target === 'foeSide' || move.target === 'allySide' || move.target === 'allyTeam')
//    {
//        damage = this.tryMoveHit(targets, pokemon, move);
//        if (damage === this.battle.NOT_FAIL) pokemon.moveThisTurnResult = null;
//        if (damage || damage === 0 || damage === undefined) moveResult = true;
//    }
//    else
//    {
//        if (!targets.length)
//        {
//            this.battle.attrLastMove('[notarget]');
//            this.battle.add(this.battle.gen >= 5 ? '-fail' : '-notarget', pokemon);
//            return false;
//        }
//        if (this.battle.gen === 4 && move.selfdestruct === 'always')
//        {
//            this.battle.faint(pokemon, pokemon, move);
//        }
//        moveResult = this.trySpreadMoveHit(targets, pokemon, move);
//    }
//    if (move.selfBoost && moveResult) this.moveHit(pokemon, pokemon, move, move.selfBoost, false, true);
//    if (!pokemon.hp)
//    {
//        this.battle.faint(pokemon, pokemon, move);
//    }

//    if (!moveResult)
//    {
//        this.battle.singleEvent('MoveFail', move, null, target, pokemon, move);
//        return false;
//    }

//    if (!(move.hasSheerForce && pokemon.hasAbility('sheerforce')) && !move.flags['futuremove'])
//    {
//        const originalHp = pokemon.hp;
//        this.battle.singleEvent('AfterMoveSecondarySelf', move, null, pokemon, target, move);
//        this.battle.runEvent('AfterMoveSecondarySelf', pokemon, target, move);
//        if (pokemon && pokemon !== target && move.category !== 'Status')
//        {
//            if (pokemon.hp <= pokemon.maxhp / 2 && originalHp > pokemon.maxhp / 2)
//            {
//                this.battle.runEvent('EmergencyExit', pokemon, pokemon);
//            }
//        }
//    }

//    return true;
//}