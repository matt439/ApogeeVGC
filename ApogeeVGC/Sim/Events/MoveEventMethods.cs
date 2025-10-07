using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

public delegate IntFalseUnion? BasePowerCallbackHandler(IBattle battle, Pokemon pokemon, Pokemon target, ActiveMove move);
public delegate bool? BeforeMoveCallbackHandler(IBattle battle, Pokemon pokemon, Pokemon? target, ActiveMove move);
public delegate void BeforeTurnCallbackHandler(IBattle battle, Pokemon pokemon, Pokemon target, ActiveMove move);
public delegate IntFalseUnion? DamageCallbackHandler(IBattle battle, Pokemon pokemon, Pokemon target, ActiveMove move);
public delegate void PriorityChargeCallbackHandler(IBattle battle, Pokemon pokemon);

public delegate void OnDisableMoveHandler(IBattle battle, Pokemon pokemon);
public delegate void OnAfterSubDamageHandler(IBattle battle, int damage, Pokemon target, Pokemon source, ActiveMove move);
public delegate IntBoolUnion? OnDamageHandler(IBattle battle, int damage, Pokemon target, Pokemon source, IEffect effect);

public delegate int? OnEffectivenessHandler(IBattle battle, int typeMod, Pokemon? target, PokemonType type,
    ActiveMove move);
public delegate bool? OnHitSideHandler(IBattle battle, Side side, Pokemon source, ActiveMove move);
public delegate void OnModifyMoveHandler(IBattle battle, ActiveMove move, Pokemon pokemon, Pokemon? target);
public delegate void OnModifyTypeHandler(IBattle battle, ActiveMove move, Pokemon pokemon, Pokemon target);
public delegate void OnModifyTargetHandler(IBattle battle, Pokemon relayTarget, Pokemon pokemon, Pokemon target, ActiveMove move);
public delegate bool? OnTryHitSideHandler(IBattle battle, Side side, Pokemon source, ActiveMove move);

public interface IMoveEventHandlers
{
    BasePowerCallbackHandler? BasePowerCallback { get; }
    BeforeMoveCallbackHandler? BeforeMoveCallback { get; }
    BeforeTurnCallbackHandler? BeforeTurnCallback { get; }
    DamageCallbackHandler? DamageCallback { get; }
    PriorityChargeCallbackHandler? PriorityChargeCallback { get; }

    OnDisableMoveHandler? OnDisableMove { get; }
    VoidSourceMoveHandler? OnAfterHit { get; }
    OnAfterSubDamageHandler? OnAfterSubDamage { get; }
    VoidSourceMoveHandler? OnAfterMoveSecondarySelf { get; }
    VoidMoveHandler? OnAfterMoveSecondary { get; }
    VoidSourceMoveHandler? OnAfterMove { get; }
    OnDamageHandler? OnDamage { get; }
    ModifierSourceMoveHandler? OnBasePower { get; }
    OnEffectivenessHandler? OnEffectiveness { get; }
    ResultMoveHandler? OnHit { get; }
    ResultMoveHandler? OnHitField { get; }
    OnHitSideHandler? OnHitSide { get; }
    OnModifyMoveHandler? OnModifyMove { get; }
    ModifierSourceMoveHandler? OnModifyPriority { get; }
    VoidMoveHandler? OnMoveFail { get; }
    OnModifyTypeHandler? OnModifyType { get; }
    OnModifyTargetHandler? OnModifyTarget { get; }
    ResultMoveHandler? OnPrepareHit { get; }
    ResultSourceMoveHandler? OnTry { get; }
    ExtResultSourceMoveHandler? OnTryHit { get; }
    ResultMoveHandler? OnTryHitField { get; }
    OnTryHitSideHandler? OnTryHitSide { get; }
    ResultMoveHandler? OnTryImmunity { get; }
    ResultSourceMoveHandler? OnTryMove { get; }
    VoidSourceMoveHandler? OnUseMoveMessage { get; }
}