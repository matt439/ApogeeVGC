using ApogeeVGC_CS.sim;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    public interface IEventMethods
    {
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnDamagingHit { get; set; }
        Action<Battle, Pokemon>? OnEmergencyExit { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterEachBoost { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; set; }
        Action<Battle, Pokemon>? OnAfterMega { get; set; }
        Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAfterSetStatus { get; set; }
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; set; }
        Action<Battle, Pokemon>? OnAfterSwitchInSelf { get; set; }
        Action<Battle, Pokemon>? OnAfterTerastallization { get; set; }
        Action<Battle, Item, Pokemon>? OnAfterUseItem { get; set; }
        Action<Battle, Item, Pokemon>? OnAfterTakeItem { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterBoost { get; set; }
        Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAfterFaint { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSelf { get; set; }
        Action<Battle, Pokemon, Pokemon>? OnAttract { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAccuracy { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; set; }
        Action<Battle, Pokemon, IEffect>? OnBeforeFaint { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnBeforeMove { get; set; }
        Action<Battle, Pokemon>? OnBeforeSwitchIn { get; set; }
        Action<Battle, Pokemon>? OnBeforeSwitchOut { get; set; }
        Action<Battle, Pokemon>? OnBeforeTurn { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnChangeBoost { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnTryBoost { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnChargeMove { get; set; }
        Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnCriticalHit { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; set; }
        Func<Battle, Pokemon, Pokemon, int?>? OnDeductPP { get; set; }
        Action<Battle, Pokemon>? OnDisableMove { get; set; }
        Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnDragOut { get; set; }
        Action<Battle, Item, Pokemon>? OnEatItem { get; set; }
        Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; set; }
        Action<Battle, Pokemon>? OnEntryHazard { get; set; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnFaint { get; set; }
        Func<Battle, Pokemon, bool?>? OnFlinch { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, double?>? OnFractionalPriority { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; set; }
        Action<Battle, string, Pokemon>? OnImmunity { get; set; }
        Func<Battle, Pokemon, string?>? OnLockMove { get; set; }
        Action<Battle, Pokemon>? OnMaybeTrapPokemon { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyAccuracy { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyAtk { get; set; }
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnModifyBoost { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyCritRatio { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamage { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyDef { get; set; }
        Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; set; }
        Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnModifySecondaries { get; set; }
        Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; set; }
        Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifySpA { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifySpD { get; set; }
        Func<Battle, int, Pokemon, int?>? OnModifySpe { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifySTAB { get; set; }
        Func<Battle, int, Pokemon, int?>? OnModifyWeight { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveAborted { get; set; }
        Func<Battle, Pokemon, string, bool?>? OnNegateImmunity { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnOverrideAction { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; set; }
        Action<Battle, Pokemon, Pokemon, Condition>? OnPseudoWeatherChange { get; set; }
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnRedirectTarget { get; set; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnResidual { get; set; }
        Func<Battle, string, Pokemon, Pokemon, IEffect, object?>? OnSetAbility { get; set; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSetStatus { get; set; }
        Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSetWeather { get; set; }
        Action<Battle, Side, Pokemon, Condition>? OnSideConditionStart { get; set; }
        Func<Battle, Pokemon, bool?>? OnStallMove { get; set; }
        Action<Battle, Pokemon>? OnSwitchIn { get; set; }
        Action<Battle, Pokemon>? OnSwitchOut { get; set; }
        Action<Battle, Pokemon, Pokemon>? OnSwap { get; set; }
        Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnTakeItem { get; set; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnWeatherChange { get; set; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; set; }
        Action<Battle, Pokemon>? OnTrapPokemon { get; set; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnTryAddVolatile { get; set; }
        Func<Battle, Item, Pokemon, bool?>? OnTryEatItem { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnTryHeal { get; set; }



        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitSide { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnInvulnerability { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnTryPrimaryHit { get; set; }
        Func<Battle, List<string>, Pokemon, List<string>?>? OnType { get; set; }
        Action<Battle, Item, Pokemon>? OnUseItem { get; set; }
        Action<Battle, Pokemon>? OnUpdate { get; set; }
        Action<Battle, Pokemon, object?, Condition>? OnWeather { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnWeatherModifyDamage { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase1 { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase2 { get; set; }

        // Foe event handlers (triggered by opponent actions)
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeDamagingHit { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnFoeAfterEachBoost { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterHit { get; set; }
        Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnFoeAfterSetStatus { get; set; }
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeAfterSubDamage { get; set; }
        Action<Battle, Pokemon>? OnFoeAfterSwitchInSelf { get; set; }
        Action<Battle, Item, Pokemon>? OnFoeAfterUseItem { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeAfterBoost { get; set; }
        Action<Battle, int, Pokemon, Pokemon, IEffect>? OnFoeAfterFaint { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondarySelf { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondary { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMove { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSelf { get; set; }
        Action<Battle, Pokemon, Pokemon>? OnFoeAttract { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnFoeAccuracy { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeBasePower { get; set; }
        Action<Battle, Pokemon, IEffect>? OnFoeBeforeFaint { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeBeforeMove { get; set; }
        Action<Battle, Pokemon>? OnFoeBeforeSwitchIn { get; set; }
        Action<Battle, Pokemon>? OnFoeBeforeSwitchOut { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeChargeMove { get; set; }
        Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnFoeCriticalHit { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnFoeDamage { get; set; }
        Func<Battle, Pokemon, Pokemon, int?>? OnFoeDeductPP { get; set; }
        Action<Battle, Pokemon>? OnFoeDisableMove { get; set; }
        Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnFoeDragOut { get; set; }
        Action<Battle, Item, Pokemon>? OnFoeEatItem { get; set; }
        Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnFoeEffectiveness { get; set; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnFoeFaint { get; set; }
        Func<Battle, Pokemon, bool?>? OnFoeFlinch { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeHit { get; set; }
        Action<Battle, string, Pokemon>? OnFoeImmunity { get; set; }
        Func<Battle, Pokemon, string?>? OnFoeLockMove { get; set; }
        Action<Battle, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyAccuracy { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyAtk { get; set; }
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnFoeModifyBoost { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyCritRatio { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamage { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyDef { get; set; }
        Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnFoeModifyMove { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyPriority { get; set; }
        Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnFoeModifySecondaries { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifySpA { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifySpD { get; set; }
        Func<Battle, int, Pokemon, int?>? OnFoeModifySpe { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifySTAB { get; set; }
        Action<Battle, ActiveMove, Pokemon, Pokemon>? OnFoeModifyType { get; set; }
        Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnFoeModifyTarget { get; set; }
        Func<Battle, int, Pokemon, int?>? OnFoeModifyWeight { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeMoveAborted { get; set; }
        Func<Battle, Pokemon, string, bool?>? OnFoeNegateImmunity { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnFoeOverrideAction { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoePrepareHit { get; set; }
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnFoeRedirectTarget { get; set; }
        Action<Battle, object, Pokemon, IEffect>? OnFoeResidual { get; set; }
        Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetAbility { get; set; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetStatus { get; set; }
        Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnFoeSetWeather { get; set; }
        Func<Battle, Pokemon, bool?>? OnFoeStallMove { get; set; }
        Action<Battle, Pokemon>? OnFoeSwitchOut { get; set; }
        Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnFoeTakeItem { get; set; }
        Action<Battle, Pokemon>? OnFoeTerrain { get; set; }
        Action<Battle, Pokemon>? OnFoeTrapPokemon { get; set; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeTryAddVolatile { get; set; }
        Func<Battle, Item, Pokemon, bool?>? OnFoeTryEatItem { get; set; }
        Func<Battle, object, object?, object?, object?, object?>? OnFoeTryHeal { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeTryHit { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitField { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitSide { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeInvulnerability { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryMove { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnFoeTryPrimaryHit { get; set; }


        Func<Battle, List<string>, Pokemon, List<string>?>? OnFoeType { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeWeatherModifyDamage { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase1 { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase2 { get; set; }

        // Source event handlers (triggered when this Pokémon is the source)
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceDamagingHit { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnSourceAfterEachBoost { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterHit { get; set; }
        Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnSourceAfterSetStatus { get; set; }
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceAfterSubDamage { get; set; }
        Action<Battle, Pokemon>? OnSourceAfterSwitchInSelf { get; set; }
        Action<Battle, Item, Pokemon>? OnSourceAfterUseItem { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceAfterBoost { get; set; }
        Action<Battle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondarySelf { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondary { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMove { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSelf { get; set; }
        Action<Battle, Pokemon, Pokemon>? OnSourceAttract { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnSourceAccuracy { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceBasePower { get; set; }
        Action<Battle, Pokemon, IEffect>? OnSourceBeforeFaint { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceBeforeMove { get; set; }
        Action<Battle, Pokemon>? OnSourceBeforeSwitchIn { get; set; }
        Action<Battle, Pokemon>? OnSourceBeforeSwitchOut { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceChargeMove { get; set; }
        Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnSourceCriticalHit { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnSourceDamage { get; set; }
        Func<Battle, Pokemon, Pokemon, int?>? OnSourceDeductPP { get; set; }
        Action<Battle, Pokemon>? OnSourceDisableMove { get; set; }
        Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnSourceDragOut { get; set; }
        Action<Battle, Item, Pokemon>? OnSourceEatItem { get; set; }
        Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnSourceEffectiveness { get; set; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnSourceFaint { get; set; }
        Func<Battle, Pokemon, bool?>? OnSourceFlinch { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceHit { get; set; }
        Action<Battle, string, Pokemon>? OnSourceImmunity { get; set; }
        Func<Battle, Pokemon, string?>? OnSourceLockMove { get; set; }
        Action<Battle, Pokemon>? OnSourceMaybeTrapPokemon { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyAccuracy { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyAtk { get; set; }
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnSourceModifyBoost { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyCritRatio { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamage { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyDef { get; set; }
        Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnSourceModifyMove { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyPriority { get; set; }
        Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnSourceModifySecondaries { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifySpA { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifySpD { get; set; }
        Func<Battle, int, Pokemon, int?>? OnSourceModifySpe { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifySTAB { get; set; }
        Action<Battle, ActiveMove, Pokemon, Pokemon>? OnSourceModifyType { get; set; }
        Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnSourceModifyTarget { get; set; }
        Func<Battle, int, Pokemon, int?>? OnSourceModifyWeight { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceMoveAborted { get; set; }
        Func<Battle, Pokemon, string, bool?>? OnSourceNegateImmunity { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnSourceOverrideAction { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourcePrepareHit { get; set; }
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnSourceRedirectTarget { get; set; }
        Action<Battle, object, Pokemon, IEffect>? OnSourceResidual { get; set; }
        Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetAbility { get; set; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetStatus { get; set; }
        Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSourceSetWeather { get; set; }
        Func<Battle, Pokemon, bool?>? OnSourceStallMove { get; set; }
        Action<Battle, Pokemon>? OnSourceSwitchOut { get; set; }
        Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnSourceTakeItem { get; set; }
        Action<Battle, Pokemon>? OnSourceTerrain { get; set; }
        Action<Battle, Pokemon>? OnSourceTrapPokemon { get; set; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceTryAddVolatile { get; set; }
        Func<Battle, Item, Pokemon, bool?>? OnSourceTryEatItem { get; set; }
        Func<Battle, object, object?, object?, object?, object?>? OnSourceTryHeal { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceTryHit { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitField { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitSide { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceInvulnerability { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryMove { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnSourceTryPrimaryHit { get; set; }
        Func<Battle, List<string>, Pokemon, List<string>?>? OnSourceType { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceWeatherModifyDamage { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase1 { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase2 { get; set; }

        // Any event handlers (triggered for any Pokémon action in battle)
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyDamagingHit { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAnyAfterEachBoost { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterHit { get; set; }
        Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAnyAfterSetStatus { get; set; }
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyAfterSubDamage { get; set; }
        Action<Battle, Pokemon>? OnAnyAfterSwitchInSelf { get; set; }
        Action<Battle, Item, Pokemon>? OnAnyAfterUseItem { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyAfterBoost { get; set; }
        Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAnyAfterFaint { get; set; }
        Action<Battle, Pokemon>? OnAnyAfterMega { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondarySelf { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondary { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMove { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSelf { get; set; }
        Action<Battle, Pokemon>? OnAnyAfterTerastallization { get; set; }
        Action<Battle, Pokemon, Pokemon>? OnAnyAttract { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAnyAccuracy { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyBasePower { get; set; }
        Action<Battle, Pokemon, IEffect>? OnAnyBeforeFaint { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyBeforeMove { get; set; }
        Action<Battle, Pokemon>? OnAnyBeforeSwitchIn { get; set; }
        Action<Battle, Pokemon>? OnAnyBeforeSwitchOut { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyChargeMove { get; set; }
        Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAnyCriticalHit { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAnyDamage { get; set; }
        Func<Battle, Pokemon, Pokemon, int?>? OnAnyDeductPP { get; set; }
        Action<Battle, Pokemon>? OnAnyDisableMove { get; set; }
        Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAnyDragOut { get; set; }
        Action<Battle, Item, Pokemon>? OnAnyEatItem { get; set; }
        Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAnyEffectiveness { get; set; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnAnyFaint { get; set; }
        Func<Battle, Pokemon, bool?>? OnAnyFlinch { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyHit { get; set; }
        Action<Battle, string, Pokemon>? OnAnyImmunity { get; set; }
        Func<Battle, Pokemon, string?>? OnAnyLockMove { get; set; }
        Action<Battle, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyAccuracy { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyAtk { get; set; }
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAnyModifyBoost { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyCritRatio { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamage { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyDef { get; set; }
        Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAnyModifyMove { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyPriority { get; set; }
        Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAnyModifySecondaries { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifySpA { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifySpD { get; set; }
        Func<Battle, int, Pokemon, int?>? OnAnyModifySpe { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifySTAB { get; set; }
        Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAnyModifyType { get; set; }
        Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAnyModifyTarget { get; set; }
        Func<Battle, int, Pokemon, int?>? OnAnyModifyWeight { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyMoveAborted { get; set; }
        Func<Battle, Pokemon, string, bool?>? OnAnyNegateImmunity { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAnyOverrideAction { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyPrepareHit { get; set; }
        Action<Battle, Pokemon, Pokemon, Condition>? OnAnyPseudoWeatherChange { get; set; }
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAnyRedirectTarget { get; set; }
        Action<Battle, object, Pokemon, IEffect>? OnAnyResidual { get; set; }
        Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAnySetAbility { get; set; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnySetStatus { get; set; }
        Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAnySetWeather { get; set; }
        Func<Battle, Pokemon, bool?>? OnAnyStallMove { get; set; }
        Action<Battle, Pokemon>? OnAnySwitchIn { get; set; }
        Action<Battle, Pokemon>? OnAnySwitchOut { get; set; }
        Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAnyTakeItem { get; set; }
        Action<Battle, Pokemon>? OnAnyTerrain { get; set; }
        Action<Battle, Pokemon>? OnAnyTrapPokemon { get; set; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnyTryAddVolatile { get; set; }
        Func<Battle, Item, Pokemon, bool?>? OnAnyTryEatItem { get; set; }
        Func<Battle, object, object?, object?, object?, object?>? OnAnyTryHeal { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyTryHit { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitField { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitSide { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyInvulnerability { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryMove { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAnyTryPrimaryHit { get; set; }
        Func<Battle, List<string>, Pokemon, List<string>?>? OnAnyType { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyWeatherModifyDamage { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase1 { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase2 { get; set; }

        // Priorities (incomplete list)
        int? OnAccuracyPriority { get; set; }
        int? OnDamagingHitOrder { get; set; }
        int? OnAfterMoveSecondaryPriority { get; set; }
        int? OnAfterMoveSecondarySelfPriority { get; set; }
        int? OnAfterMoveSelfPriority { get; set; }
        int? OnAfterSetStatusPriority { get; set; }
        int? OnAnyBasePowerPriority { get; set; }
        int? OnAnyInvulnerabilityPriority { get; set; }
        int? OnAnyModifyAccuracyPriority { get; set; }
        int? OnAnyFaintPriority { get; set; }
        int? OnAnyPrepareHitPriority { get; set; }
        int? OnAnySwitchInPriority { get; set; }
        int? OnAnySwitchInSubOrder { get; set; }
        int? OnAllyBasePowerPriority { get; set; }
        int? OnAllyModifyAtkPriority { get; set; }
        int? OnAllyModifySpAPriority { get; set; }
        int? OnAllyModifySpDPriority { get; set; }
        int? OnAttractPriority { get; set; }
        int? OnBasePowerPriority { get; set; }
        int? OnBeforeMovePriority { get; set; }
        int? OnBeforeSwitchOutPriority { get; set; }
        int? OnChangeBoostPriority { get; set; }
        int? OnDamagePriority { get; set; }
        int? OnDragOutPriority { get; set; }
        int? OnEffectivenessPriority { get; set; }
        int? OnFoeBasePowerPriority { get; set; }
        int? OnFoeBeforeMovePriority { get; set; }
        int? OnFoeModifyDefPriority { get; set; }
        int? OnFoeModifySpDPriority { get; set; }
        int? OnFoeRedirectTargetPriority { get; set; }
        int? OnFoeTrapPokemonPriority { get; set; }
        int? OnFractionalPriorityPriority { get; set; }
        int? OnHitPriority { get; set; }
        int? OnInvulnerabilityPriority { get; set; }
        int? OnModifyAccuracyPriority { get; set; }
        int? OnModifyAtkPriority { get; set; }
        int? OnModifyCritRatioPriority { get; set; }
        int? OnModifyDefPriority { get; set; }
        int? OnModifyMovePriority { get; set; }
        int? OnModifyPriorityPriority { get; set; }
        int? OnModifySpAPriority { get; set; }
        int? OnModifySpDPriority { get; set; }
        int? OnModifySpePriority { get; set; }
        int? OnModifySTABPriority { get; set; }
        int? OnModifyTypePriority { get; set; }
        int? OnModifyWeightPriority { get; set; }
        int? OnRedirectTargetPriority { get; set; }
        int? OnResidualOrder { get; set; }
        int? OnResidualPriority { get; set; }
        int? OnResidualSubOrder { get; set; }
        int? OnSourceBasePowerPriority { get; set; }
        int? OnSourceInvulnerabilityPriority { get; set; }
        int? OnSourceModifyAccuracyPriority { get; set; }
        int? OnSourceModifyAtkPriority { get; set; }
        int? OnSourceModifyDamagePriority { get; set; }
        int? OnSourceModifySpAPriority { get; set; }
        int? OnSwitchInPriority { get; set; }
        int? OnSwitchInSubOrder { get; set; }
        int? OnTrapPokemonPriority { get; set; }
        int? OnTryBoostPriority { get; set; }
        int? OnTryEatItemPriority { get; set; }
        int? OnTryHealPriority { get; set; }
        int? OnTryHitPriority { get; set; }
        int? OnTryMovePriority { get; set; }
        int? OnTryPrimaryHitPriority { get; set; }
        int? OnTypePriority { get; set; }
    }

    public interface IPokemonEventMethods : IEventMethods
    {
        // Ally event handlers (triggered by ally Pokémon actions)
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyDamagingHit { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAllyAfterEachBoost { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterHit { get; set; }
        Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAllyAfterSetStatus { get; set; }
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyAfterSubDamage { get; set; }
        Action<Battle, Pokemon>? OnAllyAfterSwitchInSelf { get; set; }
        Action<Battle, Item, Pokemon>? OnAllyAfterUseItem { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyAfterBoost { get; set; }
        Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAllyAfterFaint { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondarySelf { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondary { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMove { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSelf { get; set; }
        Action<Battle, Pokemon, Pokemon>? OnAllyAttract { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAllyAccuracy { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyBasePower { get; set; }
        Action<Battle, Pokemon, IEffect>? OnAllyBeforeFaint { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyBeforeMove { get; set; }
        Action<Battle, Pokemon>? OnAllyBeforeSwitchIn { get; set; }
        Action<Battle, Pokemon>? OnAllyBeforeSwitchOut { get; set; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyTryBoost { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyChargeMove { get; set; }
        Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAllyCriticalHit { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAllyDamage { get; set; }
        Func<Battle, Pokemon, Pokemon, int?>? OnAllyDeductPP { get; set; }
        Action<Battle, Pokemon>? OnAllyDisableMove { get; set; }
        Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAllyDragOut { get; set; }
        Action<Battle, Item, Pokemon>? OnAllyEatItem { get; set; }
        Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAllyEffectiveness { get; set; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnAllyFaint { get; set; }
        Func<Battle, Pokemon, bool?>? OnAllyFlinch { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyHit { get; set; }
        Action<Battle, string, Pokemon>? OnAllyImmunity { get; set; }
        Func<Battle, Pokemon, string?>? OnAllyLockMove { get; set; }
        Action<Battle, Pokemon>? OnAllyMaybeTrapPokemon { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyAccuracy { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyAtk { get; set; }
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAllyModifyBoost { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyCritRatio { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamage { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyDef { get; set; }
        Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAllyModifyMove { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyPriority { get; set; }
        Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAllyModifySecondaries { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifySpA { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifySpD { get; set; }
        Func<Battle, int, Pokemon, int?>? OnAllyModifySpe { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifySTAB { get; set; }
        Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAllyModifyType { get; set; }
        Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAllyModifyTarget { get; set; }
        Func<Battle, int, Pokemon, int?>? OnAllyModifyWeight { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyMoveAborted { get; set; }
        Func<Battle, Pokemon, string, bool?>? OnAllyNegateImmunity { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAllyOverrideAction { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyPrepareHit { get; set; }
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAllyRedirectTarget { get; set; }
        Action<Battle, object, Pokemon, IEffect>? OnAllyResidual { get; set; }
        Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAllySetAbility { get; set; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllySetStatus { get; set; }
        Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAllySetWeather { get; set; }
        Func<Battle, Pokemon, bool?>? OnAllyStallMove { get; set; }
        Action<Battle, Pokemon>? OnAllySwitchOut { get; set; }
        Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAllyTakeItem { get; set; }
        Action<Battle, Pokemon>? OnAllyTerrain { get; set; }
        Action<Battle, Pokemon>? OnAllyTrapPokemon { get; set; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllyTryAddVolatile { get; set; }
        Func<Battle, Item, Pokemon, bool?>? OnAllyTryEatItem { get; set; }
        Func<Battle, object, object?, object?, object?, object?>? OnAllyTryHeal { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyTryHit { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitField { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitSide { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyInvulnerability { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryMove { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAllyTryPrimaryHit { get; set; }
        Func<Battle, List<string>, Pokemon, List<string>?>? OnAllyType { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyWeatherModifyDamage { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase1 { get; set; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase2 { get; set; }
    }

    public interface ISideEventMethods : IEventMethods
    {
        // Side condition lifecycle events
        Action<Battle, Side, Pokemon, IEffect>? OnSideStart { get; set; }
        Action<Battle, Side, Pokemon, IEffect>? OnSideRestart { get; set; }
        Action<Battle, Side, Pokemon, IEffect>? OnSideResidual { get; set; }
        Action<Battle, Side>? OnSideEnd { get; set; }

        // Side event priorities and ordering
        int? OnSideResidualOrder { get; set; }
        int? OnSideResidualPriority { get; set; }
        int? OnSideResidualSubOrder { get; set; }
    }

    public interface IFieldEventMethods : IEventMethods
    {
        // Field condition lifecycle events
        Action<Battle, Field, Pokemon, IEffect>? OnFieldStart { get; set; }
        Action<Battle, Field, Pokemon, IEffect>? OnFieldRestart { get; set; }
        Action<Battle, Field, Pokemon, IEffect>? OnFieldResidual { get; set; }
        Action<Battle, Field>? OnFieldEnd { get; set; }

        // Field event priorities and ordering
        int? OnFieldResidualOrder { get; set; }
        int? OnFieldResidualPriority { get; set; }
        int? OnFieldResidualSubOrder { get; set; }
    }

    /// <summary>
    /// PokemonConditionData, SideConditionData, and FieldConditionData
    /// inherit from this interface.
    /// </summary>
    public interface IConditionData { }

    public interface IPokemonConditionData : IPokemonEventMethods, IConditionData
    {
        // Add properties from Condition as needed
    }

    public interface ISideConditionData : ISideEventMethods, IConditionData
    {
        // Add properties from Condition as needed, excluding onStart/onRestart/onEnd
    }

    public interface IFieldConditionData : IFieldEventMethods, IConditionData
    {
        // Add properties from Condition as needed, excluding onStart/onRestart/onEnd
    }
    public interface IModdedConditionData : IConditionData
    {
        public bool Inherit { get; set; }
    }
    public class ConditionDataTable : Dictionary<IdEntry, IConditionData> { }
    public class ModdedConditionDataTable : Dictionary<IdEntry, IModdedConditionData> { }

    public class Condition : BasicEffect, ISideConditionData, IFieldConditionData, IPokemonConditionData,
        ICondition
    {
        public new string EffectType { get; set; } = "Condition";
        public int? CounterMax { get; set; }
        public int? EffectOrder { get; set; }

        // Event handler delegates (add as needed)
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; set; }
        public Action<Battle, Pokemon>? OnCopy { get; set; }
        public Action<Battle, Pokemon>? OnEnd { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, bool?>? OnRestart { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, bool?>? OnStart { get; set; }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideStart { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideRestart { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideResidual { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Side>? OnSideEnd { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnSideResidualOrder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnSideResidualPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnSideResidualSubOrder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnDamagingHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnEmergencyExit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterEachBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAfterMega { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAfterSetStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAfterSwitchInSelf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAfterTerastallization { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Item, Pokemon>? OnAfterUseItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Item, Pokemon>? OnAfterTakeItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAfterFaint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSelf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon>? OnAttract { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAccuracy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, IEffect>? OnBeforeFaint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnBeforeMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnBeforeSwitchIn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnBeforeSwitchOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnBeforeTurn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnChangeBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnTryBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnChargeMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnCriticalHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, int?>? OnDeductPP { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnDisableMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnDragOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Item, Pokemon>? OnEatItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnEntryHazard { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnFaint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, bool?>? OnFlinch { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, double?>? OnFractionalPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, string, Pokemon>? OnImmunity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, string?>? OnLockMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnMaybeTrapPokemon { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyAccuracy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyAtk { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnModifyBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyCritRatio { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyDef { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnModifySecondaries { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifySpA { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifySpD { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, int?>? OnModifySpe { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifySTAB { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, int?>? OnModifyWeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveAborted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, string, bool?>? OnNegateImmunity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnOverrideAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, Condition>? OnPseudoWeatherChange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnRedirectTarget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnResidual { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, object?>? OnSetAbility { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSetStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSetWeather { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Side, Pokemon, Condition>? OnSideConditionStart { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, bool?>? OnStallMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnSwitchIn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnSwitchOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon>? OnSwap { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnTakeItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnWeatherChange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnTrapPokemon { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnTryAddVolatile { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Item, Pokemon, bool?>? OnTryEatItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnTryHeal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitSide { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnInvulnerability { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnTryPrimaryHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Item, Pokemon>? OnUseItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnUpdate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, object?, Condition>? OnWeather { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnWeatherModifyDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeDamagingHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnFoeAfterEachBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnFoeAfterSetStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeAfterSubDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnFoeAfterSwitchInSelf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Item, Pokemon>? OnFoeAfterUseItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeAfterBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnFoeAfterFaint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondarySelf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondary { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSelf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon>? OnFoeAttract { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnFoeAccuracy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeBasePower { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, IEffect>? OnFoeBeforeFaint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeBeforeMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnFoeBeforeSwitchIn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnFoeBeforeSwitchOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeChargeMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnFoeCriticalHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnFoeDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, int?>? OnFoeDeductPP { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnFoeDisableMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnFoeDragOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Item, Pokemon>? OnFoeEatItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnFoeEffectiveness { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnFoeFaint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, bool?>? OnFoeFlinch { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, string, Pokemon>? OnFoeImmunity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, string?>? OnFoeLockMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyAccuracy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyAtk { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnFoeModifyBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyCritRatio { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyDef { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnFoeModifyMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnFoeModifySecondaries { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifySpA { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifySpD { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, int?>? OnFoeModifySpe { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifySTAB { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnFoeModifyType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnFoeModifyTarget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, int?>? OnFoeModifyWeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeMoveAborted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, string, bool?>? OnFoeNegateImmunity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnFoeOverrideAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoePrepareHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnFoeRedirectTarget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, object, Pokemon, IEffect>? OnFoeResidual { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetAbility { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnFoeSetWeather { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, bool?>? OnFoeStallMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnFoeSwitchOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnFoeTakeItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnFoeTerrain { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnFoeTrapPokemon { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeTryAddVolatile { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Item, Pokemon, bool?>? OnFoeTryEatItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, object, object?, object?, object?, object?>? OnFoeTryHeal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeTryHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitField { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitSide { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeInvulnerability { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnFoeTryPrimaryHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnFoeType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeWeatherModifyDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceDamagingHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnSourceAfterEachBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnSourceAfterSetStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceAfterSubDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnSourceAfterSwitchInSelf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Item, Pokemon>? OnSourceAfterUseItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceAfterBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondarySelf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondary { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSelf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon>? OnSourceAttract { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnSourceAccuracy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceBasePower { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, IEffect>? OnSourceBeforeFaint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceBeforeMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnSourceBeforeSwitchIn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnSourceBeforeSwitchOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceChargeMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnSourceCriticalHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnSourceDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, int?>? OnSourceDeductPP { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnSourceDisableMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnSourceDragOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Item, Pokemon>? OnSourceEatItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnSourceEffectiveness { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnSourceFaint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, bool?>? OnSourceFlinch { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, string, Pokemon>? OnSourceImmunity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, string?>? OnSourceLockMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnSourceMaybeTrapPokemon { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyAccuracy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyAtk { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnSourceModifyBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyCritRatio { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyDef { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnSourceModifyMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnSourceModifySecondaries { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifySpA { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifySpD { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, int?>? OnSourceModifySpe { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifySTAB { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnSourceModifyType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnSourceModifyTarget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, int?>? OnSourceModifyWeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceMoveAborted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, string, bool?>? OnSourceNegateImmunity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnSourceOverrideAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourcePrepareHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnSourceRedirectTarget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, object, Pokemon, IEffect>? OnSourceResidual { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetAbility { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSourceSetWeather { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, bool?>? OnSourceStallMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnSourceSwitchOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnSourceTakeItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnSourceTerrain { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnSourceTrapPokemon { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceTryAddVolatile { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Item, Pokemon, bool?>? OnSourceTryEatItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, object, object?, object?, object?, object?>? OnSourceTryHeal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceTryHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitField { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitSide { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceInvulnerability { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnSourceTryPrimaryHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnSourceType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceWeatherModifyDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyDamagingHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAnyAfterEachBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAnyAfterSetStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyAfterSubDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAnyAfterSwitchInSelf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Item, Pokemon>? OnAnyAfterUseItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyAfterBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAnyAfterFaint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAnyAfterMega { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondarySelf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondary { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSelf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAnyAfterTerastallization { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon>? OnAnyAttract { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAnyAccuracy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyBasePower { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, IEffect>? OnAnyBeforeFaint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyBeforeMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAnyBeforeSwitchIn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAnyBeforeSwitchOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyChargeMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAnyCriticalHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAnyDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, int?>? OnAnyDeductPP { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAnyDisableMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAnyDragOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Item, Pokemon>? OnAnyEatItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAnyEffectiveness { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnAnyFaint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, bool?>? OnAnyFlinch { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, string, Pokemon>? OnAnyImmunity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, string?>? OnAnyLockMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyAccuracy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyAtk { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAnyModifyBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyCritRatio { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyDef { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAnyModifyMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAnyModifySecondaries { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifySpA { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifySpD { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, int?>? OnAnyModifySpe { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifySTAB { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAnyModifyType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAnyModifyTarget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, int?>? OnAnyModifyWeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyMoveAborted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, string, bool?>? OnAnyNegateImmunity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAnyOverrideAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyPrepareHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, Condition>? OnAnyPseudoWeatherChange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAnyRedirectTarget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, object, Pokemon, IEffect>? OnAnyResidual { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAnySetAbility { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnySetStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAnySetWeather { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, bool?>? OnAnyStallMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAnySwitchIn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAnySwitchOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAnyTakeItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAnyTerrain { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAnyTrapPokemon { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnyTryAddVolatile { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Item, Pokemon, bool?>? OnAnyTryEatItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, object, object?, object?, object?, object?>? OnAnyTryHeal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyTryHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitField { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitSide { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyInvulnerability { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAnyTryPrimaryHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAnyType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyWeatherModifyDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAccuracyPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnDamagingHitOrder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAfterMoveSecondaryPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAfterMoveSecondarySelfPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAfterMoveSelfPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAfterSetStatusPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAnyBasePowerPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAnyInvulnerabilityPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAnyModifyAccuracyPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAnyFaintPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAnyPrepareHitPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAnySwitchInPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAnySwitchInSubOrder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAllyBasePowerPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAllyModifyAtkPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAllyModifySpAPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAllyModifySpDPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnAttractPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnBasePowerPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnBeforeMovePriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnBeforeSwitchOutPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnChangeBoostPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnDamagePriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnDragOutPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnEffectivenessPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnFoeBasePowerPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnFoeBeforeMovePriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnFoeModifyDefPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnFoeModifySpDPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnFoeRedirectTargetPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnFoeTrapPokemonPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnFractionalPriorityPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnHitPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnInvulnerabilityPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnModifyAccuracyPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnModifyAtkPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnModifyCritRatioPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnModifyDefPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnModifyMovePriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnModifyPriorityPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnModifySpAPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnModifySpDPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnModifySpePriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnModifySTABPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnModifyTypePriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnModifyWeightPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnRedirectTargetPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnResidualOrder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnResidualPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnResidualSubOrder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnSourceBasePowerPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnSourceInvulnerabilityPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnSourceModifyAccuracyPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnSourceModifyAtkPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnSourceModifyDamagePriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnSourceModifySpAPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnSwitchInPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnSwitchInSubOrder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnTrapPokemonPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnTryBoostPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnTryEatItemPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnTryHealPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnTryHitPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnTryMovePriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnTryPrimaryHitPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnTypePriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldStart { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldRestart { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldResidual { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Field>? OnFieldEnd { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnFieldResidualOrder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnFieldResidualPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? OnFieldResidualSubOrder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyDamagingHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAllyAfterEachBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAllyAfterSetStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyAfterSubDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAllyAfterSwitchInSelf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Item, Pokemon>? OnAllyAfterUseItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyAfterBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAllyAfterFaint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondarySelf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondary { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSelf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon>? OnAllyAttract { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAllyAccuracy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyBasePower { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, IEffect>? OnAllyBeforeFaint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyBeforeMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAllyBeforeSwitchIn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAllyBeforeSwitchOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyTryBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyChargeMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAllyCriticalHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAllyDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, int?>? OnAllyDeductPP { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAllyDisableMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAllyDragOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Item, Pokemon>? OnAllyEatItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAllyEffectiveness { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnAllyFaint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, bool?>? OnAllyFlinch { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, string, Pokemon>? OnAllyImmunity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, string?>? OnAllyLockMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAllyMaybeTrapPokemon { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyAccuracy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyAtk { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAllyModifyBoost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyCritRatio { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyDef { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAllyModifyMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAllyModifySecondaries { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifySpA { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifySpD { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifySpe { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifySTAB { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAllyModifyType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAllyModifyTarget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifyWeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyMoveAborted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, string, bool?>? OnAllyNegateImmunity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAllyOverrideAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyPrepareHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAllyRedirectTarget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, object, Pokemon, IEffect>? OnAllyResidual { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAllySetAbility { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllySetStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAllySetWeather { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, bool?>? OnAllyStallMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAllySwitchOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAllyTakeItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAllyTerrain { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnAllyTrapPokemon { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllyTryAddVolatile { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Item, Pokemon, bool?>? OnAllyTryEatItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, object, object?, object?, object?, object?>? OnAllyTryHeal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyTryHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitField { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitSide { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyInvulnerability { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAllyTryPrimaryHit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAllyType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyWeatherModifyDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Condition(IAnyObject data) : base(data)
        {
            // If data contains "Weather", "Status", or "Terrain", set the corresponding properties
            // Else set to "Condition"
            if (data.ContainsKey("Weather"))
            {
                EffectType = "Weather";
            }
            else if (data.ContainsKey("Status"))
            {
                EffectType = "Status";
            }
            else if (data.ContainsKey("Terrain"))
            {
                EffectType = "Terrain";
            }
            else
            {
                EffectType = "Condition";
            }

            AssignMissingFields(data);
        }

        private void AssignMissingFields(IAnyObject data)
        {
            if (data.TryGetInt("effectOrder", out int effectOrderValue))
            {
                EffectOrder = effectOrderValue;
            }
            else if (data.TryGetInt("EffectOrder", out effectOrderValue))
            {
                EffectOrder = effectOrderValue;
            }

            if (data.TryGetInt("CounterMax", out int counterMaxValue))
            {
                CounterMax = counterMaxValue;
            }
            else if (data.TryGetInt("Countermax", out counterMaxValue))
            {
                CounterMax = counterMaxValue;
            }

            //if (data.TryGet("DurationCallback", out Func<Battle, Pokemon, Pokemon, IEffect?, int>? durationCallback))
            //{
            //    DurationCallback = durationCallback;
            //}
            //else if (data.TryGet("durationCallback", out durationCallback))
            //{
            //    DurationCallback = durationCallback;
            //}

            //if (data.TryGet("OnCopy", out Action<Battle, Pokemon>? onCopy))
            //{
            //    OnCopy = onCopy;
            //}
            //else if (data.TryGet("onCopy", out onCopy))
            //{
            //    OnCopy = onCopy;
            //}

            //if (data.TryGet("OnEnd", out Action<Battle, Pokemon>? onEnd))
            //{
            //    OnEnd = onEnd;
            //}
            //else if (data.TryGet("onEnd", out onEnd))
            //{
            //    OnEnd = onEnd;
            //}
        }
    }

    public class  ConditionConstants
    {
        public static readonly Condition EmptyCondition = new(new DefaultTextData
        {
            ["name"] = "",
            ["exists"] = false
        });
    }

    public class DexConditions(ModdedDex dex)
    {
        ModdedDex Dex { get; } = dex;
        private readonly Dictionary<Id, Condition> _conditionCache = [];

        public Condition Get(string? name = "")
        {
            throw new NotImplementedException("Get method is not implemented yet.");
        }

        public Condition Get(IEffect? effect)
        {
            throw new NotImplementedException("Get method is not implemented yet.");
        }

        public Condition GetById(Id id)
        {
            throw new NotImplementedException("GetById method is not implemented yet.");
        }
    }
}
