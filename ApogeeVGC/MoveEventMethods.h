#pragma once


#include "Battle.h"
#include "Pokemon.h"
#include "Effect.h"
#include "Side.h"
#include "ActiveMove.h"
// CommonHandlers

struct MoveEventMethods
{

};

//export interface MoveEventMethods{
//	basePowerCallback ? : (this: Battle, pokemon : Pokemon, target : Pokemon, move : ActiveMove) = > number | false | null;
//	/** Return true to stop the move from being used */
//	beforeMoveCallback ? : (this: Battle, pokemon : Pokemon, target : Pokemon | null, move : ActiveMove) = > boolean | void;
//	beforeTurnCallback ? : (this: Battle, pokemon : Pokemon, target : Pokemon) = > void;
//	damageCallback ? : (this: Battle, pokemon : Pokemon, target : Pokemon) = > number | false;
//	priorityChargeCallback ? : (this: Battle, pokemon : Pokemon) = > void;
//
//	onDisableMove ? : (this: Battle, pokemon : Pokemon) = > void;
//
//	onAfterHit ? : CommonHandlers['VoidSourceMove'];
//	onAfterSubDamage ? : (this: Battle, damage : number, target : Pokemon, source : Pokemon, move : ActiveMove) = > void;
//	onAfterMoveSecondarySelf ? : CommonHandlers['VoidSourceMove'];
//	onAfterMoveSecondary ? : CommonHandlers['VoidMove'];
//	onAfterMove ? : CommonHandlers['VoidSourceMove'];
//	onDamagePriority ? : number;
//	onDamage ? : (
//		this: Battle, damage : number, target : Pokemon, source : Pokemon, effect : Effect
//	) = > number | boolean | null | void;
//
//	/* Invoked by the global BasePower event (onEffect = true) */
//	onBasePower ? : CommonHandlers['ModifierSourceMove'];
//
//	onEffectiveness ? : (
//		this: Battle, typeMod : number, target : Pokemon | null, type : string, move : ActiveMove
//	) = > number | void;
//	onHit ? : CommonHandlers['ResultMove'];
//	onHitField ? : CommonHandlers['ResultMove'];
//	onHitSide ? : (this: Battle, side : Side, source : Pokemon, move : ActiveMove) = > boolean | null | "" | void;
//	onModifyMove ? : (this: Battle, move : ActiveMove, pokemon : Pokemon, target : Pokemon | null) = > void;
//	onModifyPriority ? : CommonHandlers['ModifierSourceMove'];
//	onMoveFail ? : CommonHandlers['VoidMove'];
//	onModifyType ? : (this: Battle, move : ActiveMove, pokemon : Pokemon, target : Pokemon) = > void;
//	onModifyTarget ? : (
//		this: Battle, relayVar : { target: Pokemon }, pokemon : Pokemon, target : Pokemon, move : ActiveMove
//	) = > void;
//	onPrepareHit ? : CommonHandlers['ResultMove'];
//	onTry ? : CommonHandlers['ResultSourceMove'];
//	onTryHit ? : CommonHandlers['ExtResultSourceMove'];
//	onTryHitField ? : CommonHandlers['ResultMove'];
//	onTryHitSide ? : (this: Battle, side : Side, source : Pokemon, move : ActiveMove) = > boolean | null | "" | void;
//	onTryImmunity ? : CommonHandlers['ResultMove'];
//	onTryMove ? : CommonHandlers['ResultSourceMove'];
//	onUseMoveMessage ? : CommonHandlers['VoidSourceMove'];
//}