using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Methods;

public interface ITestMethods
{
    // From ability

    /// <summary>battle, length, target, source, effect</summary>
    public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; }
    // OnStart
    // OnModifyPriority
    // OnTerrainChange
    // Adding electric terrain
    // Adding Hadron engine conditon
    // Adding guts condition
    // adding flame body condition
    // adding quark drive condition

    // From condition
    // OnStart
    // OnResidual
    // OnResidualOrder
    // OnResidualSubOrder
    // OnModifySpe
    // OnModifySpePriority
    // OnBeforeMove
    // OnBeforeMovePriority
    // OnDisableMove
    // OnStallMove
    // OnRestart
    // OnTryHit
    // OnTryHitPriority
    // OnAnyModifyDamage
    // OnModifySpA
    // OnModifySpAPriority
    // OnModifySpD
    // OnModifySpDPriority
    // OnModifyAtk
    // OnModifyAtkPriority
    // OnDamagingHit
    // OnModifyDef
    // OnModifyDefPriority

    // From Item
    // OnBeforeResiduals
    // OnStart
    // OnModifyMove
    // OnDamagingHit
    // OnDamagingHitOrder

    // From Move
    // OnTryImmunity
    // OnHit,
    // OnBasePower
    // OnTryImmunity
    // OnTry
    // BasePowerCallback
}