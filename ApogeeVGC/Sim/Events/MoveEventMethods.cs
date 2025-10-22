using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

public delegate IntFalseUnion? BasePowerCallbackHandler(IBattle battle, Pokemon pokemon, Pokemon target,
    ActiveMove move);

public delegate BoolVoidUnion BeforeMoveCallbackHandler(IBattle battle, Pokemon pokemon, Pokemon? target,
    ActiveMove move);

public delegate void BeforeTurnCallbackHandler(IBattle battle, Pokemon pokemon, Pokemon target, ActiveMove move);
public delegate IntFalseUnion DamageCallbackHandler(IBattle battle, Pokemon pokemon, Pokemon target);
public delegate void PriorityChargeCallbackHandler(IBattle battle, Pokemon pokemon);

public delegate void OnDisableMoveHandler(IBattle battle, Pokemon pokemon);
public delegate void OnAfterSubDamageHandler(IBattle battle, int damage, Pokemon target, Pokemon source,
    ActiveMove move);

public delegate IntBoolVoidUnion? OnDamageHandler(IBattle battle, int damage, Pokemon target, Pokemon source,
    IEffect effect);

public delegate IntVoidUnion OnEffectivenessHandler(IBattle battle, int typeMod, Pokemon? target, PokemonType type,
    ActiveMove move);
public delegate BoolUndefinedVoidUnion? OnHitSideHandler(IBattle battle, Side side, Pokemon source, ActiveMove move);
public delegate void OnModifyMoveHandler(IBattle battle, ActiveMove move, Pokemon pokemon, Pokemon? target);
public delegate void OnModifyTypeHandler(IBattle battle, ActiveMove move, Pokemon pokemon, Pokemon target);

public delegate void OnModifyTargetHandler(IBattle battle, Pokemon relayTarget, Pokemon pokemon, Pokemon target,
    ActiveMove move);

public delegate BoolUndefinedVoidUnion? OnTryHitSideHandler(IBattle battle, Side side, Pokemon source,
    ActiveMove move);

public interface IMoveEventHandlers
{
    TypeUndefinedUnion<BasePowerCallbackHandler>? BasePowerCallback { get; }
    TypeUndefinedUnion<BeforeMoveCallbackHandler>? BeforeMoveCallback { get; }
    TypeUndefinedUnion<BeforeTurnCallbackHandler>? BeforeTurnCallback { get; }
    TypeUndefinedUnion<DamageCallbackHandler>? DamageCallback { get; }
    TypeUndefinedUnion<PriorityChargeCallbackHandler>? PriorityChargeCallback { get; }

    TypeUndefinedUnion<OnDisableMoveHandler>? OnDisableMove { get; }
    TypeUndefinedUnion<VoidSourceMoveHandler>? OnAfterHit { get; }
    TypeUndefinedUnion<OnAfterSubDamageHandler>? OnAfterSubDamage { get; }
    TypeUndefinedUnion<VoidSourceMoveHandler>? OnAfterMoveSecondarySelf { get; }
    TypeUndefinedUnion<VoidMoveHandler>? OnAfterMoveSecondary { get; }
    TypeUndefinedUnion<VoidSourceMoveHandler>? OnAfterMove { get; }
    TypeUndefinedUnion<OnDamageHandler>? OnDamage { get; }
    TypeUndefinedUnion<ModifierSourceMoveHandler>? OnBasePower { get; }
    TypeUndefinedUnion<OnEffectivenessHandler>? OnEffectiveness { get; }
    TypeUndefinedUnion<ResultMoveHandler>? OnHit { get; }
    TypeUndefinedUnion<ResultMoveHandler>? OnHitField { get; }
    TypeUndefinedUnion<OnHitSideHandler>? OnHitSide { get; }
    TypeUndefinedUnion<OnModifyMoveHandler>? OnModifyMove { get; }
    TypeUndefinedUnion<ModifierSourceMoveHandler>? OnModifyPriority { get; }
    TypeUndefinedUnion<VoidMoveHandler>? OnMoveFail { get; }
    TypeUndefinedUnion<OnModifyTypeHandler>? OnModifyType { get; }
    TypeUndefinedUnion<OnModifyTargetHandler>? OnModifyTarget { get; }
    TypeUndefinedUnion<ResultMoveHandler>? OnPrepareHit { get; }
    TypeUndefinedUnion<ResultSourceMoveHandler>? OnTry { get; }
    TypeUndefinedUnion<ExtResultSourceMoveHandler>? OnTryHit { get; }
    TypeUndefinedUnion<ResultMoveHandler>? OnTryHitField { get; }
    TypeUndefinedUnion<OnTryHitSideHandler>? OnTryHitSide { get; }
    TypeUndefinedUnion<ResultMoveHandler>? OnTryImmunity { get; }
    TypeUndefinedUnion<ResultSourceMoveHandler>? OnTryMove { get; }
    TypeUndefinedUnion<VoidSourceMoveHandler>? OnUseMoveMessage { get; }
}