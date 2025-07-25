﻿using ApogeeVGC_CS.sim;
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

    public interface IPokemonConditionData : IPokemonEventMethods, ICondition, IConditionData { }

    public class PokemonConditionData : IPokemonConditionData
    {
        public int? CounterMax { get; set; }
        public int? EffectOrder { get; set; }
        public Action<Battle, Pokemon>? OnCopy { get; set; }
        public Action<Battle, Pokemon>? OnEnd { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, bool?>? OnRestart { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, bool?>? OnStart { get; set; }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideStart { get; set; }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideRestart { get; set; }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideResidual { get; set; }
        public Action<Battle, Side>? OnSideEnd { get; set; }
        public int? OnSideResidualOrder { get; set; }
        public int? OnSideResidualPriority { get; set; }
        public int? OnSideResidualSubOrder { get; set; }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldStart { get; set; }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldRestart { get; set; }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldResidual { get; set; }
        public Action<Battle, Field>? OnFieldEnd { get; set; }
        public int? OnFieldResidualOrder { get; set; }
        public int? OnFieldResidualPriority { get; set; }
        public int? OnFieldResidualSubOrder { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAllyAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAllyAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnAllyAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnAllyAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAllyAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnAllyAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAllyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnAllyBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnAllyBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnAllyBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAllyCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAllyDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnAllyDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnAllyDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAllyDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnAllyEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAllyEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnAllyFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAllyFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyHit { get; set; }
        public Action<Battle, string, Pokemon>? OnAllyImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnAllyLockMove { get; set; }
        public Action<Battle, Pokemon>? OnAllyMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAllyModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAllyModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAllyModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAllyModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAllyModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnAllyNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAllyOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyPrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAllyRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnAllyResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAllySetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllySetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAllySetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAllyStallMove { get; set; }
        public Action<Battle, Pokemon>? OnAllySwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAllyTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnAllyTerrain { get; set; }
        public Action<Battle, Pokemon>? OnAllyTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllyTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnAllyTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnAllyTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAllyTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAllyType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnDamagingHit { get; set; }
        public Action<Battle, Pokemon>? OnEmergencyExit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; set; }
        public Action<Battle, Pokemon>? OnAfterMega { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnAfterSwitchInSelf { get; set; }
        public Action<Battle, Pokemon>? OnAfterTerastallization { get; set; }
        public Action<Battle, Item, Pokemon>? OnAfterUseItem { get; set; }
        public Action<Battle, Item, Pokemon>? OnAfterTakeItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnBeforeSwitchOut { get; set; }
        public Action<Battle, Pokemon>? OnBeforeTurn { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnChangeBoost { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; set; }
        public Action<Battle, Pokemon>? OnEntryHazard { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnFlinch { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, double?>? OnFractionalPriority { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; set; }
        public Action<Battle, string, Pokemon>? OnImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnLockMove { get; set; }
        public Action<Battle, Pokemon>? OnMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnModifySecondaries { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifySTAB { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; set; }
        public Action<Battle, Pokemon, Pokemon, Condition>? OnPseudoWeatherChange { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnRedirectTarget { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, object?>? OnSetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSetWeather { get; set; }
        public Action<Battle, Side, Pokemon, Condition>? OnSideConditionStart { get; set; }
        public Func<Battle, Pokemon, bool?>? OnStallMove { get; set; }
        public Action<Battle, Pokemon>? OnSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnSwitchOut { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnSwap { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnTakeItem { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnWeatherChange { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; set; }
        public Action<Battle, Pokemon>? OnTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnTryEatItem { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnType { get; set; }
        public Action<Battle, Item, Pokemon>? OnUseItem { get; set; }
        public Action<Battle, Pokemon>? OnUpdate { get; set; }
        public Action<Battle, Pokemon, object?, Condition>? OnWeather { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnFoeAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnFoeAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnFoeAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnFoeAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnFoeAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnFoeAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnFoeAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnFoeBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnFoeBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnFoeBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnFoeCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnFoeDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnFoeDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnFoeDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnFoeDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnFoeEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnFoeEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnFoeFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnFoeFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeHit { get; set; }
        public Action<Battle, string, Pokemon>? OnFoeImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnFoeLockMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnFoeModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnFoeModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnFoeModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnFoeModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnFoeModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnFoeModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnFoeModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnFoeNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnFoeOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoePrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnFoeRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnFoeResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnFoeSetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnFoeStallMove { get; set; }
        public Action<Battle, Pokemon>? OnFoeSwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnFoeTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnFoeTerrain { get; set; }
        public Action<Battle, Pokemon>? OnFoeTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnFoeTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnFoeTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnFoeTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnFoeType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnSourceAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnSourceAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnSourceAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnSourceAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnSourceAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnSourceAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnSourceBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnSourceBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnSourceBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnSourceCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnSourceDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnSourceDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnSourceDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnSourceDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnSourceEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnSourceEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnSourceFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnSourceFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceHit { get; set; }
        public Action<Battle, string, Pokemon>? OnSourceImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnSourceLockMove { get; set; }
        public Action<Battle, Pokemon>? OnSourceMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnSourceModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnSourceModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnSourceModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnSourceModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnSourceModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnSourceModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnSourceModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnSourceNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnSourceOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourcePrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnSourceRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnSourceResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSourceSetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnSourceStallMove { get; set; }
        public Action<Battle, Pokemon>? OnSourceSwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnSourceTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnSourceTerrain { get; set; }
        public Action<Battle, Pokemon>? OnSourceTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnSourceTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnSourceTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnSourceTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnSourceType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAnyAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAnyAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnAnyAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnAnyAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAnyAfterFaint { get; set; }
        public Action<Battle, Pokemon>? OnAnyAfterMega { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon>? OnAnyAfterTerastallization { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnAnyAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAnyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnAnyBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnAnyBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnAnyBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAnyCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAnyDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnAnyDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnAnyDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAnyDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnAnyEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAnyEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnAnyFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAnyFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyHit { get; set; }
        public Action<Battle, string, Pokemon>? OnAnyImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnAnyLockMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAnyModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAnyModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAnyModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAnyModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAnyModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAnyModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAnyModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnAnyNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAnyOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyPrepareHit { get; set; }
        public Action<Battle, Pokemon, Pokemon, Condition>? OnAnyPseudoWeatherChange { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAnyRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnAnyResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAnySetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnySetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAnySetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAnyStallMove { get; set; }
        public Action<Battle, Pokemon>? OnAnySwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnAnySwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAnyTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnAnyTerrain { get; set; }
        public Action<Battle, Pokemon>? OnAnyTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnyTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnAnyTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnAnyTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAnyTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAnyType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase2 { get; set; }
        public int? OnAccuracyPriority { get; set; }
        public int? OnDamagingHitOrder { get; set; }
        public int? OnAfterMoveSecondaryPriority { get; set; }
        public int? OnAfterMoveSecondarySelfPriority { get; set; }
        public int? OnAfterMoveSelfPriority { get; set; }
        public int? OnAfterSetStatusPriority { get; set; }
        public int? OnAnyBasePowerPriority { get; set; }
        public int? OnAnyInvulnerabilityPriority { get; set; }
        public int? OnAnyModifyAccuracyPriority { get; set; }
        public int? OnAnyFaintPriority { get; set; }
        public int? OnAnyPrepareHitPriority { get; set; }
        public int? OnAnySwitchInPriority { get; set; }
        public int? OnAnySwitchInSubOrder { get; set; }
        public int? OnAllyBasePowerPriority { get; set; }
        public int? OnAllyModifyAtkPriority { get; set; }
        public int? OnAllyModifySpAPriority { get; set; }
        public int? OnAllyModifySpDPriority { get; set; }
        public int? OnAttractPriority { get; set; }
        public int? OnBasePowerPriority { get; set; }
        public int? OnBeforeMovePriority { get; set; }
        public int? OnBeforeSwitchOutPriority { get; set; }
        public int? OnChangeBoostPriority { get; set; }
        public int? OnDamagePriority { get; set; }
        public int? OnDragOutPriority { get; set; }
        public int? OnEffectivenessPriority { get; set; }
        public int? OnFoeBasePowerPriority { get; set; }
        public int? OnFoeBeforeMovePriority { get; set; }
        public int? OnFoeModifyDefPriority { get; set; }
        public int? OnFoeModifySpDPriority { get; set; }
        public int? OnFoeRedirectTargetPriority { get; set; }
        public int? OnFoeTrapPokemonPriority { get; set; }
        public int? OnFractionalPriorityPriority { get; set; }
        public int? OnHitPriority { get; set; }
        public int? OnInvulnerabilityPriority { get; set; }
        public int? OnModifyAccuracyPriority { get; set; }
        public int? OnModifyAtkPriority { get; set; }
        public int? OnModifyCritRatioPriority { get; set; }
        public int? OnModifyDefPriority { get; set; }
        public int? OnModifyMovePriority { get; set; }
        public int? OnModifyPriorityPriority { get; set; }
        public int? OnModifySpAPriority { get; set; }
        public int? OnModifySpDPriority { get; set; }
        public int? OnModifySpePriority { get; set; }
        public int? OnModifySTABPriority { get; set; }
        public int? OnModifyTypePriority { get; set; }
        public int? OnModifyWeightPriority { get; set; }
        public int? OnRedirectTargetPriority { get; set; }
        public int? OnResidualOrder { get; set; }
        public int? OnResidualPriority { get; set; }
        public int? OnResidualSubOrder { get; set; }
        public int? OnSourceBasePowerPriority { get; set; }
        public int? OnSourceInvulnerabilityPriority { get; set; }
        public int? OnSourceModifyAccuracyPriority { get; set; }
        public int? OnSourceModifyAtkPriority { get; set; }
        public int? OnSourceModifyDamagePriority { get; set; }
        public int? OnSourceModifySpAPriority { get; set; }
        public int? OnSwitchInPriority { get; set; }
        public int? OnSwitchInSubOrder { get; set; }
        public int? OnTrapPokemonPriority { get; set; }
        public int? OnTryBoostPriority { get; set; }
        public int? OnTryEatItemPriority { get; set; }
        public int? OnTryHealPriority { get; set; }
        public int? OnTryHitPriority { get; set; }
        public int? OnTryMovePriority { get; set; }
        public int? OnTryPrimaryHitPriority { get; set; }
        public int? OnTypePriority { get; set; }
        public Id Id { get; set; }
        public string Name { get; set; }
        public string Fullname { get; set; }
        public EffectType EffectType { get; set; }
        public bool Exists { get; set; }
        public int Num { get; set; }
        public int Gen { get; set; }
        public string? ShortDesc { get; set; }
        public string? Desc { get; set; }
        public Nonstandard? IsNonstandard { get; set; }
        public int? Duration { get; set; }
        public bool NoCopy { get; set; }
        public bool AffectsFainted { get; set; }
        public Id? Status { get; set; }
        public Id? Weather { get; set; }
        public string SourceEffect { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; set; }
        public string? EffectTypeString { get; set; }
        public bool? Infiltrates { get; set; }
        public string? RealMove { get; set; }
    }

    public interface ISideConditionData : ISideEventMethods, ICondition, IConditionData { }

    public class SideConditionData : ISideConditionData
    {
        public int? CounterMax { get; set; }
        public int? EffectOrder { get; set; }
        public Action<Battle, Pokemon>? OnCopy { get; set; }
        public Action<Battle, Pokemon>? OnEnd { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, bool?>? OnRestart { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, bool?>? OnStart { get; set; }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideStart { get; set; }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideRestart { get; set; }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideResidual { get; set; }
        public Action<Battle, Side>? OnSideEnd { get; set; }
        public int? OnSideResidualOrder { get; set; }
        public int? OnSideResidualPriority { get; set; }
        public int? OnSideResidualSubOrder { get; set; }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldStart { get; set; }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldRestart { get; set; }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldResidual { get; set; }
        public Action<Battle, Field>? OnFieldEnd { get; set; }
        public int? OnFieldResidualOrder { get; set; }
        public int? OnFieldResidualPriority { get; set; }
        public int? OnFieldResidualSubOrder { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAllyAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAllyAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnAllyAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnAllyAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAllyAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnAllyAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAllyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnAllyBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnAllyBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnAllyBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAllyCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAllyDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnAllyDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnAllyDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAllyDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnAllyEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAllyEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnAllyFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAllyFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyHit { get; set; }
        public Action<Battle, string, Pokemon>? OnAllyImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnAllyLockMove { get; set; }
        public Action<Battle, Pokemon>? OnAllyMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAllyModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAllyModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAllyModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAllyModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAllyModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnAllyNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAllyOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyPrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAllyRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnAllyResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAllySetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllySetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAllySetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAllyStallMove { get; set; }
        public Action<Battle, Pokemon>? OnAllySwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAllyTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnAllyTerrain { get; set; }
        public Action<Battle, Pokemon>? OnAllyTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllyTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnAllyTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnAllyTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAllyTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAllyType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnDamagingHit { get; set; }
        public Action<Battle, Pokemon>? OnEmergencyExit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; set; }
        public Action<Battle, Pokemon>? OnAfterMega { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnAfterSwitchInSelf { get; set; }
        public Action<Battle, Pokemon>? OnAfterTerastallization { get; set; }
        public Action<Battle, Item, Pokemon>? OnAfterUseItem { get; set; }
        public Action<Battle, Item, Pokemon>? OnAfterTakeItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnBeforeSwitchOut { get; set; }
        public Action<Battle, Pokemon>? OnBeforeTurn { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnChangeBoost { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; set; }
        public Action<Battle, Pokemon>? OnEntryHazard { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnFlinch { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, double?>? OnFractionalPriority { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; set; }
        public Action<Battle, string, Pokemon>? OnImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnLockMove { get; set; }
        public Action<Battle, Pokemon>? OnMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnModifySecondaries { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifySTAB { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; set; }
        public Action<Battle, Pokemon, Pokemon, Condition>? OnPseudoWeatherChange { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnRedirectTarget { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, object?>? OnSetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSetWeather { get; set; }
        public Action<Battle, Side, Pokemon, Condition>? OnSideConditionStart { get; set; }
        public Func<Battle, Pokemon, bool?>? OnStallMove { get; set; }
        public Action<Battle, Pokemon>? OnSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnSwitchOut { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnSwap { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnTakeItem { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnWeatherChange { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; set; }
        public Action<Battle, Pokemon>? OnTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnTryEatItem { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnType { get; set; }
        public Action<Battle, Item, Pokemon>? OnUseItem { get; set; }
        public Action<Battle, Pokemon>? OnUpdate { get; set; }
        public Action<Battle, Pokemon, object?, Condition>? OnWeather { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnFoeAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnFoeAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnFoeAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnFoeAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnFoeAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnFoeAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnFoeAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnFoeBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnFoeBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnFoeBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnFoeCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnFoeDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnFoeDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnFoeDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnFoeDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnFoeEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnFoeEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnFoeFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnFoeFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeHit { get; set; }
        public Action<Battle, string, Pokemon>? OnFoeImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnFoeLockMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnFoeModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnFoeModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnFoeModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnFoeModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnFoeModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnFoeModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnFoeModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnFoeNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnFoeOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoePrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnFoeRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnFoeResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnFoeSetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnFoeStallMove { get; set; }
        public Action<Battle, Pokemon>? OnFoeSwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnFoeTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnFoeTerrain { get; set; }
        public Action<Battle, Pokemon>? OnFoeTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnFoeTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnFoeTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnFoeTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnFoeType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnSourceAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnSourceAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnSourceAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnSourceAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnSourceAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnSourceAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnSourceBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnSourceBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnSourceBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnSourceCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnSourceDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnSourceDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnSourceDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnSourceDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnSourceEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnSourceEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnSourceFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnSourceFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceHit { get; set; }
        public Action<Battle, string, Pokemon>? OnSourceImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnSourceLockMove { get; set; }
        public Action<Battle, Pokemon>? OnSourceMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnSourceModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnSourceModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnSourceModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnSourceModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnSourceModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnSourceModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnSourceModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnSourceNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnSourceOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourcePrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnSourceRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnSourceResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSourceSetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnSourceStallMove { get; set; }
        public Action<Battle, Pokemon>? OnSourceSwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnSourceTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnSourceTerrain { get; set; }
        public Action<Battle, Pokemon>? OnSourceTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnSourceTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnSourceTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnSourceTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnSourceType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAnyAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAnyAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnAnyAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnAnyAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAnyAfterFaint { get; set; }
        public Action<Battle, Pokemon>? OnAnyAfterMega { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon>? OnAnyAfterTerastallization { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnAnyAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAnyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnAnyBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnAnyBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnAnyBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAnyCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAnyDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnAnyDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnAnyDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAnyDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnAnyEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAnyEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnAnyFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAnyFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyHit { get; set; }
        public Action<Battle, string, Pokemon>? OnAnyImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnAnyLockMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAnyModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAnyModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAnyModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAnyModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAnyModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAnyModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAnyModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnAnyNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAnyOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyPrepareHit { get; set; }
        public Action<Battle, Pokemon, Pokemon, Condition>? OnAnyPseudoWeatherChange { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAnyRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnAnyResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAnySetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnySetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAnySetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAnyStallMove { get; set; }
        public Action<Battle, Pokemon>? OnAnySwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnAnySwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAnyTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnAnyTerrain { get; set; }
        public Action<Battle, Pokemon>? OnAnyTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnyTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnAnyTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnAnyTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAnyTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAnyType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase2 { get; set; }
        public int? OnAccuracyPriority { get; set; }
        public int? OnDamagingHitOrder { get; set; }
        public int? OnAfterMoveSecondaryPriority { get; set; }
        public int? OnAfterMoveSecondarySelfPriority { get; set; }
        public int? OnAfterMoveSelfPriority { get; set; }
        public int? OnAfterSetStatusPriority { get; set; }
        public int? OnAnyBasePowerPriority { get; set; }
        public int? OnAnyInvulnerabilityPriority { get; set; }
        public int? OnAnyModifyAccuracyPriority { get; set; }
        public int? OnAnyFaintPriority { get; set; }
        public int? OnAnyPrepareHitPriority { get; set; }
        public int? OnAnySwitchInPriority { get; set; }
        public int? OnAnySwitchInSubOrder { get; set; }
        public int? OnAllyBasePowerPriority { get; set; }
        public int? OnAllyModifyAtkPriority { get; set; }
        public int? OnAllyModifySpAPriority { get; set; }
        public int? OnAllyModifySpDPriority { get; set; }
        public int? OnAttractPriority { get; set; }
        public int? OnBasePowerPriority { get; set; }
        public int? OnBeforeMovePriority { get; set; }
        public int? OnBeforeSwitchOutPriority { get; set; }
        public int? OnChangeBoostPriority { get; set; }
        public int? OnDamagePriority { get; set; }
        public int? OnDragOutPriority { get; set; }
        public int? OnEffectivenessPriority { get; set; }
        public int? OnFoeBasePowerPriority { get; set; }
        public int? OnFoeBeforeMovePriority { get; set; }
        public int? OnFoeModifyDefPriority { get; set; }
        public int? OnFoeModifySpDPriority { get; set; }
        public int? OnFoeRedirectTargetPriority { get; set; }
        public int? OnFoeTrapPokemonPriority { get; set; }
        public int? OnFractionalPriorityPriority { get; set; }
        public int? OnHitPriority { get; set; }
        public int? OnInvulnerabilityPriority { get; set; }
        public int? OnModifyAccuracyPriority { get; set; }
        public int? OnModifyAtkPriority { get; set; }
        public int? OnModifyCritRatioPriority { get; set; }
        public int? OnModifyDefPriority { get; set; }
        public int? OnModifyMovePriority { get; set; }
        public int? OnModifyPriorityPriority { get; set; }
        public int? OnModifySpAPriority { get; set; }
        public int? OnModifySpDPriority { get; set; }
        public int? OnModifySpePriority { get; set; }
        public int? OnModifySTABPriority { get; set; }
        public int? OnModifyTypePriority { get; set; }
        public int? OnModifyWeightPriority { get; set; }
        public int? OnRedirectTargetPriority { get; set; }
        public int? OnResidualOrder { get; set; }
        public int? OnResidualPriority { get; set; }
        public int? OnResidualSubOrder { get; set; }
        public int? OnSourceBasePowerPriority { get; set; }
        public int? OnSourceInvulnerabilityPriority { get; set; }
        public int? OnSourceModifyAccuracyPriority { get; set; }
        public int? OnSourceModifyAtkPriority { get; set; }
        public int? OnSourceModifyDamagePriority { get; set; }
        public int? OnSourceModifySpAPriority { get; set; }
        public int? OnSwitchInPriority { get; set; }
        public int? OnSwitchInSubOrder { get; set; }
        public int? OnTrapPokemonPriority { get; set; }
        public int? OnTryBoostPriority { get; set; }
        public int? OnTryEatItemPriority { get; set; }
        public int? OnTryHealPriority { get; set; }
        public int? OnTryHitPriority { get; set; }
        public int? OnTryMovePriority { get; set; }
        public int? OnTryPrimaryHitPriority { get; set; }
        public int? OnTypePriority { get; set; }
        public Id Id { get; set; }
        public string Name { get; set; }
        public string Fullname { get; set; }
        public EffectType EffectType { get; set; }
        public bool Exists { get; set; }
        public int Num { get; set; }
        public int Gen { get; set; }
        public string? ShortDesc { get; set; }
        public string? Desc { get; set; }
        public Nonstandard? IsNonstandard { get; set; }
        public int? Duration { get; set; }
        public bool NoCopy { get; set; }
        public bool AffectsFainted { get; set; }
        public Id? Status { get; set; }
        public Id? Weather { get; set; }
        public string SourceEffect { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; set; }
        public string? EffectTypeString { get; set; }
        public bool? Infiltrates { get; set; }
        public string? RealMove { get; set; }
    }

    public interface IFieldConditionData : IFieldEventMethods, ICondition, IConditionData { }

    public class FieldConditionData : IFieldConditionData
    {
        public int? CounterMax { get; set; }
        public int? EffectOrder { get; set; }
        public Action<Battle, Pokemon>? OnCopy { get; set; }
        public Action<Battle, Pokemon>? OnEnd { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, bool?>? OnRestart { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, bool?>? OnStart { get; set; }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideStart { get; set; }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideRestart { get; set; }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideResidual { get; set; }
        public Action<Battle, Side>? OnSideEnd { get; set; }
        public int? OnSideResidualOrder { get; set; }
        public int? OnSideResidualPriority { get; set; }
        public int? OnSideResidualSubOrder { get; set; }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldStart { get; set; }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldRestart { get; set; }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldResidual { get; set; }
        public Action<Battle, Field>? OnFieldEnd { get; set; }
        public int? OnFieldResidualOrder { get; set; }
        public int? OnFieldResidualPriority { get; set; }
        public int? OnFieldResidualSubOrder { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAllyAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAllyAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnAllyAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnAllyAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAllyAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnAllyAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAllyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnAllyBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnAllyBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnAllyBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAllyCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAllyDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnAllyDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnAllyDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAllyDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnAllyEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAllyEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnAllyFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAllyFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyHit { get; set; }
        public Action<Battle, string, Pokemon>? OnAllyImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnAllyLockMove { get; set; }
        public Action<Battle, Pokemon>? OnAllyMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAllyModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAllyModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAllyModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAllyModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAllyModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnAllyNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAllyOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyPrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAllyRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnAllyResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAllySetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllySetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAllySetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAllyStallMove { get; set; }
        public Action<Battle, Pokemon>? OnAllySwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAllyTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnAllyTerrain { get; set; }
        public Action<Battle, Pokemon>? OnAllyTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllyTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnAllyTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnAllyTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAllyTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAllyType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnDamagingHit { get; set; }
        public Action<Battle, Pokemon>? OnEmergencyExit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; set; }
        public Action<Battle, Pokemon>? OnAfterMega { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnAfterSwitchInSelf { get; set; }
        public Action<Battle, Pokemon>? OnAfterTerastallization { get; set; }
        public Action<Battle, Item, Pokemon>? OnAfterUseItem { get; set; }
        public Action<Battle, Item, Pokemon>? OnAfterTakeItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnBeforeSwitchOut { get; set; }
        public Action<Battle, Pokemon>? OnBeforeTurn { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnChangeBoost { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; set; }
        public Action<Battle, Pokemon>? OnEntryHazard { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnFlinch { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, double?>? OnFractionalPriority { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; set; }
        public Action<Battle, string, Pokemon>? OnImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnLockMove { get; set; }
        public Action<Battle, Pokemon>? OnMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnModifySecondaries { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifySTAB { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; set; }
        public Action<Battle, Pokemon, Pokemon, Condition>? OnPseudoWeatherChange { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnRedirectTarget { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, object?>? OnSetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSetWeather { get; set; }
        public Action<Battle, Side, Pokemon, Condition>? OnSideConditionStart { get; set; }
        public Func<Battle, Pokemon, bool?>? OnStallMove { get; set; }
        public Action<Battle, Pokemon>? OnSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnSwitchOut { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnSwap { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnTakeItem { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnWeatherChange { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; set; }
        public Action<Battle, Pokemon>? OnTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnTryEatItem { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnType { get; set; }
        public Action<Battle, Item, Pokemon>? OnUseItem { get; set; }
        public Action<Battle, Pokemon>? OnUpdate { get; set; }
        public Action<Battle, Pokemon, object?, Condition>? OnWeather { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnFoeAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnFoeAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnFoeAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnFoeAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnFoeAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnFoeAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnFoeAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnFoeBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnFoeBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnFoeBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnFoeCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnFoeDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnFoeDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnFoeDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnFoeDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnFoeEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnFoeEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnFoeFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnFoeFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeHit { get; set; }
        public Action<Battle, string, Pokemon>? OnFoeImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnFoeLockMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnFoeModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnFoeModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnFoeModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnFoeModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnFoeModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnFoeModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnFoeModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnFoeNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnFoeOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoePrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnFoeRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnFoeResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnFoeSetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnFoeStallMove { get; set; }
        public Action<Battle, Pokemon>? OnFoeSwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnFoeTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnFoeTerrain { get; set; }
        public Action<Battle, Pokemon>? OnFoeTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnFoeTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnFoeTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnFoeTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnFoeType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnSourceAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnSourceAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnSourceAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnSourceAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnSourceAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnSourceAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnSourceBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnSourceBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnSourceBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnSourceCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnSourceDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnSourceDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnSourceDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnSourceDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnSourceEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnSourceEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnSourceFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnSourceFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceHit { get; set; }
        public Action<Battle, string, Pokemon>? OnSourceImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnSourceLockMove { get; set; }
        public Action<Battle, Pokemon>? OnSourceMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnSourceModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnSourceModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnSourceModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnSourceModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnSourceModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnSourceModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnSourceModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnSourceNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnSourceOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourcePrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnSourceRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnSourceResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSourceSetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnSourceStallMove { get; set; }
        public Action<Battle, Pokemon>? OnSourceSwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnSourceTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnSourceTerrain { get; set; }
        public Action<Battle, Pokemon>? OnSourceTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnSourceTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnSourceTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnSourceTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnSourceType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAnyAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAnyAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnAnyAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnAnyAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAnyAfterFaint { get; set; }
        public Action<Battle, Pokemon>? OnAnyAfterMega { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon>? OnAnyAfterTerastallization { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnAnyAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAnyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnAnyBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnAnyBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnAnyBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAnyCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAnyDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnAnyDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnAnyDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAnyDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnAnyEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAnyEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnAnyFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAnyFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyHit { get; set; }
        public Action<Battle, string, Pokemon>? OnAnyImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnAnyLockMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAnyModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAnyModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAnyModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAnyModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAnyModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAnyModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAnyModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnAnyNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAnyOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyPrepareHit { get; set; }
        public Action<Battle, Pokemon, Pokemon, Condition>? OnAnyPseudoWeatherChange { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAnyRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnAnyResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAnySetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnySetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAnySetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAnyStallMove { get; set; }
        public Action<Battle, Pokemon>? OnAnySwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnAnySwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAnyTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnAnyTerrain { get; set; }
        public Action<Battle, Pokemon>? OnAnyTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnyTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnAnyTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnAnyTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAnyTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAnyType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase2 { get; set; }
        public int? OnAccuracyPriority { get; set; }
        public int? OnDamagingHitOrder { get; set; }
        public int? OnAfterMoveSecondaryPriority { get; set; }
        public int? OnAfterMoveSecondarySelfPriority { get; set; }
        public int? OnAfterMoveSelfPriority { get; set; }
        public int? OnAfterSetStatusPriority { get; set; }
        public int? OnAnyBasePowerPriority { get; set; }
        public int? OnAnyInvulnerabilityPriority { get; set; }
        public int? OnAnyModifyAccuracyPriority { get; set; }
        public int? OnAnyFaintPriority { get; set; }
        public int? OnAnyPrepareHitPriority { get; set; }
        public int? OnAnySwitchInPriority { get; set; }
        public int? OnAnySwitchInSubOrder { get; set; }
        public int? OnAllyBasePowerPriority { get; set; }
        public int? OnAllyModifyAtkPriority { get; set; }
        public int? OnAllyModifySpAPriority { get; set; }
        public int? OnAllyModifySpDPriority { get; set; }
        public int? OnAttractPriority { get; set; }
        public int? OnBasePowerPriority { get; set; }
        public int? OnBeforeMovePriority { get; set; }
        public int? OnBeforeSwitchOutPriority { get; set; }
        public int? OnChangeBoostPriority { get; set; }
        public int? OnDamagePriority { get; set; }
        public int? OnDragOutPriority { get; set; }
        public int? OnEffectivenessPriority { get; set; }
        public int? OnFoeBasePowerPriority { get; set; }
        public int? OnFoeBeforeMovePriority { get; set; }
        public int? OnFoeModifyDefPriority { get; set; }
        public int? OnFoeModifySpDPriority { get; set; }
        public int? OnFoeRedirectTargetPriority { get; set; }
        public int? OnFoeTrapPokemonPriority { get; set; }
        public int? OnFractionalPriorityPriority { get; set; }
        public int? OnHitPriority { get; set; }
        public int? OnInvulnerabilityPriority { get; set; }
        public int? OnModifyAccuracyPriority { get; set; }
        public int? OnModifyAtkPriority { get; set; }
        public int? OnModifyCritRatioPriority { get; set; }
        public int? OnModifyDefPriority { get; set; }
        public int? OnModifyMovePriority { get; set; }
        public int? OnModifyPriorityPriority { get; set; }
        public int? OnModifySpAPriority { get; set; }
        public int? OnModifySpDPriority { get; set; }
        public int? OnModifySpePriority { get; set; }
        public int? OnModifySTABPriority { get; set; }
        public int? OnModifyTypePriority { get; set; }
        public int? OnModifyWeightPriority { get; set; }
        public int? OnRedirectTargetPriority { get; set; }
        public int? OnResidualOrder { get; set; }
        public int? OnResidualPriority { get; set; }
        public int? OnResidualSubOrder { get; set; }
        public int? OnSourceBasePowerPriority { get; set; }
        public int? OnSourceInvulnerabilityPriority { get; set; }
        public int? OnSourceModifyAccuracyPriority { get; set; }
        public int? OnSourceModifyAtkPriority { get; set; }
        public int? OnSourceModifyDamagePriority { get; set; }
        public int? OnSourceModifySpAPriority { get; set; }
        public int? OnSwitchInPriority { get; set; }
        public int? OnSwitchInSubOrder { get; set; }
        public int? OnTrapPokemonPriority { get; set; }
        public int? OnTryBoostPriority { get; set; }
        public int? OnTryEatItemPriority { get; set; }
        public int? OnTryHealPriority { get; set; }
        public int? OnTryHitPriority { get; set; }
        public int? OnTryMovePriority { get; set; }
        public int? OnTryPrimaryHitPriority { get; set; }
        public int? OnTypePriority { get; set; }
        public Id Id { get; set; }
        public string Name { get; set; }
        public string Fullname { get; set; }
        public EffectType EffectType { get; set; }
        public bool Exists { get; set; }
        public int Num { get; set; }
        public int Gen { get; set; }
        public string? ShortDesc { get; set; }
        public string? Desc { get; set; }
        public Nonstandard? IsNonstandard { get; set; }
        public int? Duration { get; set; }
        public bool NoCopy { get; set; }
        public bool AffectsFainted { get; set; }
        public Id? Status { get; set; }
        public Id? Weather { get; set; }
        public string SourceEffect { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; set; }
        public string? EffectTypeString { get; set; }
        public bool? Infiltrates { get; set; }
        public string? RealMove { get; set; }
    }

    public interface IModdedConditionData : IConditionData
    {
        public bool Inherit { get; set; }
    }
    public class ConditionDataTable : Dictionary<IdEntry, IConditionData> { }
    public class ModdedConditionDataTable : Dictionary<IdEntry, IModdedConditionData> { }


    /// <summary>
    /// Represents a condition effect.
    /// </summary>
    public interface ICondition : IBasicEffect
    {
        public int? CounterMax { get; set; }
        public int? EffectOrder { get; set; }
        public Action<Battle, Pokemon>? OnCopy { get; set; }
        public Action<Battle, Pokemon>? OnEnd { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, bool?>? OnRestart { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, bool?>? OnStart { get; set; }
    }

    public class Condition : BasicEffect, ICondition, ISideEventMethods,
        IFieldEventMethods, IPokemonEventMethods, IEffect
    {
        public int? CounterMax { get; set; }
        public int? EffectOrder { get; set; }
        public Action<Battle, Pokemon>? OnCopy { get; set; }
        public Action<Battle, Pokemon>? OnEnd { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, bool?>? OnRestart { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, bool?>? OnStart { get; set; }

        public Action<Battle, Side, Pokemon, IEffect>? OnSideStart { get; set; }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideRestart { get; set; }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideResidual { get; set; }
        public Action<Battle, Side>? OnSideEnd { get; set; }
        public int? OnSideResidualOrder { get; set; }
        public int? OnSideResidualPriority { get; set; }
        public int? OnSideResidualSubOrder { get; set; }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldStart { get; set; }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldRestart { get; set; }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldResidual { get; set; }
        public Action<Battle, Field>? OnFieldEnd { get; set; }
        public int? OnFieldResidualOrder { get; set; }
        public int? OnFieldResidualPriority { get; set; }
        public int? OnFieldResidualSubOrder { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAllyAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAllyAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnAllyAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnAllyAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAllyAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnAllyAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAllyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnAllyBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnAllyBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnAllyBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAllyCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAllyDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnAllyDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnAllyDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAllyDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnAllyEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAllyEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnAllyFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAllyFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyHit { get; set; }
        public Action<Battle, string, Pokemon>? OnAllyImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnAllyLockMove { get; set; }
        public Action<Battle, Pokemon>? OnAllyMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAllyModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAllyModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAllyModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAllyModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAllyModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnAllyNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAllyOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyPrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAllyRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnAllyResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAllySetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllySetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAllySetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAllyStallMove { get; set; }
        public Action<Battle, Pokemon>? OnAllySwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAllyTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnAllyTerrain { get; set; }
        public Action<Battle, Pokemon>? OnAllyTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllyTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnAllyTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnAllyTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAllyTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAllyType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnDamagingHit { get; set; }
        public Action<Battle, Pokemon>? OnEmergencyExit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; set; }
        public Action<Battle, Pokemon>? OnAfterMega { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnAfterSwitchInSelf { get; set; }
        public Action<Battle, Pokemon>? OnAfterTerastallization { get; set; }
        public Action<Battle, Item, Pokemon>? OnAfterUseItem { get; set; }
        public Action<Battle, Item, Pokemon>? OnAfterTakeItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnBeforeSwitchOut { get; set; }
        public Action<Battle, Pokemon>? OnBeforeTurn { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnChangeBoost { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; set; }
        public Action<Battle, Pokemon>? OnEntryHazard { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnFlinch { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, double?>? OnFractionalPriority { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; set; }
        public Action<Battle, string, Pokemon>? OnImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnLockMove { get; set; }
        public Action<Battle, Pokemon>? OnMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnModifySecondaries { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifySTAB { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; set; }
        public Action<Battle, Pokemon, Pokemon, Condition>? OnPseudoWeatherChange { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnRedirectTarget { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, object?>? OnSetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSetWeather { get; set; }
        public Action<Battle, Side, Pokemon, Condition>? OnSideConditionStart { get; set; }
        public Func<Battle, Pokemon, bool?>? OnStallMove { get; set; }
        public Action<Battle, Pokemon>? OnSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnSwitchOut { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnSwap { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnTakeItem { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnWeatherChange { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; set; }
        public Action<Battle, Pokemon>? OnTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnTryEatItem { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnType { get; set; }
        public Action<Battle, Item, Pokemon>? OnUseItem { get; set; }
        public Action<Battle, Pokemon>? OnUpdate { get; set; }
        public Action<Battle, Pokemon, object?, Condition>? OnWeather { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnFoeAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnFoeAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnFoeAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnFoeAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnFoeAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnFoeAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnFoeAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnFoeBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnFoeBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnFoeBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnFoeCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnFoeDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnFoeDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnFoeDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnFoeDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnFoeEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnFoeEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnFoeFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnFoeFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeHit { get; set; }
        public Action<Battle, string, Pokemon>? OnFoeImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnFoeLockMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnFoeModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnFoeModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnFoeModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnFoeModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnFoeModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnFoeModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnFoeModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnFoeNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnFoeOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoePrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnFoeRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnFoeResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnFoeSetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnFoeStallMove { get; set; }
        public Action<Battle, Pokemon>? OnFoeSwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnFoeTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnFoeTerrain { get; set; }
        public Action<Battle, Pokemon>? OnFoeTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnFoeTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnFoeTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnFoeTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnFoeType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnSourceAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnSourceAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnSourceAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnSourceAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnSourceAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnSourceAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnSourceBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnSourceBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnSourceBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnSourceCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnSourceDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnSourceDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnSourceDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnSourceDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnSourceEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnSourceEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnSourceFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnSourceFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceHit { get; set; }
        public Action<Battle, string, Pokemon>? OnSourceImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnSourceLockMove { get; set; }
        public Action<Battle, Pokemon>? OnSourceMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnSourceModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnSourceModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnSourceModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnSourceModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnSourceModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnSourceModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnSourceModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnSourceNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnSourceOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourcePrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnSourceRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnSourceResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSourceSetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnSourceStallMove { get; set; }
        public Action<Battle, Pokemon>? OnSourceSwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnSourceTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnSourceTerrain { get; set; }
        public Action<Battle, Pokemon>? OnSourceTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnSourceTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnSourceTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnSourceTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnSourceType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAnyAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAnyAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnAnyAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnAnyAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAnyAfterFaint { get; set; }
        public Action<Battle, Pokemon>? OnAnyAfterMega { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon>? OnAnyAfterTerastallization { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnAnyAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAnyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnAnyBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnAnyBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnAnyBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAnyCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAnyDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnAnyDeductPP { get; set; }
        public Action<Battle, Pokemon>? OnAnyDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAnyDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnAnyEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAnyEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnAnyFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAnyFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyHit { get; set; }
        public Action<Battle, string, Pokemon>? OnAnyImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnAnyLockMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAnyModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAnyModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAnyModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAnyModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifySTAB { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAnyModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAnyModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAnyModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnAnyNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAnyOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyPrepareHit { get; set; }
        public Action<Battle, Pokemon, Pokemon, Condition>? OnAnyPseudoWeatherChange { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAnyRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnAnyResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAnySetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnySetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAnySetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAnyStallMove { get; set; }
        public Action<Battle, Pokemon>? OnAnySwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnAnySwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAnyTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnAnyTerrain { get; set; }
        public Action<Battle, Pokemon>? OnAnyTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnyTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnAnyTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnAnyTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAnyTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAnyType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase2 { get; set; }
        public int? OnAccuracyPriority { get; set; }
        public int? OnDamagingHitOrder { get; set; }
        public int? OnAfterMoveSecondaryPriority { get; set; }
        public int? OnAfterMoveSecondarySelfPriority { get; set; }
        public int? OnAfterMoveSelfPriority { get; set; }
        public int? OnAfterSetStatusPriority { get; set; }
        public int? OnAnyBasePowerPriority { get; set; }
        public int? OnAnyInvulnerabilityPriority { get; set; }
        public int? OnAnyModifyAccuracyPriority { get; set; }
        public int? OnAnyFaintPriority { get; set; }
        public int? OnAnyPrepareHitPriority { get; set; }
        public int? OnAnySwitchInPriority { get; set; }
        public int? OnAnySwitchInSubOrder { get; set; }
        public int? OnAllyBasePowerPriority { get; set; }
        public int? OnAllyModifyAtkPriority { get; set; }
        public int? OnAllyModifySpAPriority { get; set; }
        public int? OnAllyModifySpDPriority { get; set; }
        public int? OnAttractPriority { get; set; }
        public int? OnBasePowerPriority { get; set; }
        public int? OnBeforeMovePriority { get; set; }
        public int? OnBeforeSwitchOutPriority { get; set; }
        public int? OnChangeBoostPriority { get; set; }
        public int? OnDamagePriority { get; set; }
        public int? OnDragOutPriority { get; set; }
        public int? OnEffectivenessPriority { get; set; }
        public int? OnFoeBasePowerPriority { get; set; }
        public int? OnFoeBeforeMovePriority { get; set; }
        public int? OnFoeModifyDefPriority { get; set; }
        public int? OnFoeModifySpDPriority { get; set; }
        public int? OnFoeRedirectTargetPriority { get; set; }
        public int? OnFoeTrapPokemonPriority { get; set; }
        public int? OnFractionalPriorityPriority { get; set; }
        public int? OnHitPriority { get; set; }
        public int? OnInvulnerabilityPriority { get; set; }
        public int? OnModifyAccuracyPriority { get; set; }
        public int? OnModifyAtkPriority { get; set; }
        public int? OnModifyCritRatioPriority { get; set; }
        public int? OnModifyDefPriority { get; set; }
        public int? OnModifyMovePriority { get; set; }
        public int? OnModifyPriorityPriority { get; set; }
        public int? OnModifySpAPriority { get; set; }
        public int? OnModifySpDPriority { get; set; }
        public int? OnModifySpePriority { get; set; }
        public int? OnModifySTABPriority { get; set; }
        public int? OnModifyTypePriority { get; set; }
        public int? OnModifyWeightPriority { get; set; }
        public int? OnRedirectTargetPriority { get; set; }
        public int? OnResidualOrder { get; set; }
        public int? OnResidualPriority { get; set; }
        public int? OnResidualSubOrder { get; set; }
        public int? OnSourceBasePowerPriority { get; set; }
        public int? OnSourceInvulnerabilityPriority { get; set; }
        public int? OnSourceModifyAccuracyPriority { get; set; }
        public int? OnSourceModifyAtkPriority { get; set; }
        public int? OnSourceModifyDamagePriority { get; set; }
        public int? OnSourceModifySpAPriority { get; set; }
        public int? OnSwitchInPriority { get; set; }
        public int? OnSwitchInSubOrder { get; set; }
        public int? OnTrapPokemonPriority { get; set; }
        public int? OnTryBoostPriority { get; set; }
        public int? OnTryEatItemPriority { get; set; }
        public int? OnTryHealPriority { get; set; }
        public int? OnTryHitPriority { get; set; }
        public int? OnTryMovePriority { get; set; }
        public int? OnTryPrimaryHitPriority { get; set; }
        public int? OnTypePriority { get; set; }

        public Condition(PokemonConditionData other) : base(other)
        {
            CopyConditionData(other);
            CopyEventMethods(other);
            CopyPokemonEventMethods(other);
        }

        public Condition(SideConditionData other) : base(other)
        {
            CopyConditionData(other);
            CopyEventMethods(other);
            CopySideEventMethods(other);
        }

        public Condition(FieldConditionData other) : base(other)
        {
            CopyConditionData(other);
            CopyEventMethods(other);
            CopyFieldEventMethods(other);
        }

        private void CopyConditionData(ICondition other)
        {
            CounterMax = other.CounterMax;
            EffectOrder = other.EffectOrder;
            OnCopy = other.OnCopy;
            OnEnd = other.OnEnd;
            OnRestart = other.OnRestart;
            OnStart = other.OnStart;
        }

        private void CopyEventMethods(IEventMethods eventMethods)
        {
            // Basic event methods
            OnDamagingHit = eventMethods.OnDamagingHit;
            OnEmergencyExit = eventMethods.OnEmergencyExit;
            OnAfterEachBoost = eventMethods.OnAfterEachBoost;
            OnAfterHit = eventMethods.OnAfterHit;
            OnAfterMega = eventMethods.OnAfterMega;
            OnAfterSetStatus = eventMethods.OnAfterSetStatus;
            OnAfterSubDamage = eventMethods.OnAfterSubDamage;
            OnAfterSwitchInSelf = eventMethods.OnAfterSwitchInSelf;
            OnAfterTerastallization = eventMethods.OnAfterTerastallization;
            OnAfterUseItem = eventMethods.OnAfterUseItem;
            OnAfterTakeItem = eventMethods.OnAfterTakeItem;
            OnAfterBoost = eventMethods.OnAfterBoost;
            OnAfterFaint = eventMethods.OnAfterFaint;
            OnAfterMoveSecondarySelf = eventMethods.OnAfterMoveSecondarySelf;
            OnAfterMoveSecondary = eventMethods.OnAfterMoveSecondary;
            OnAfterMove = eventMethods.OnAfterMove;
            OnAfterMoveSelf = eventMethods.OnAfterMoveSelf;
            OnAttract = eventMethods.OnAttract;
            OnAccuracy = eventMethods.OnAccuracy;
            OnBasePower = eventMethods.OnBasePower;
            OnBeforeFaint = eventMethods.OnBeforeFaint;
            OnBeforeMove = eventMethods.OnBeforeMove;
            OnBeforeSwitchIn = eventMethods.OnBeforeSwitchIn;
            OnBeforeSwitchOut = eventMethods.OnBeforeSwitchOut;
            OnBeforeTurn = eventMethods.OnBeforeTurn;
            OnChangeBoost = eventMethods.OnChangeBoost;
            OnTryBoost = eventMethods.OnTryBoost;
            OnChargeMove = eventMethods.OnChargeMove;
            OnCriticalHit = eventMethods.OnCriticalHit;
            OnDamage = eventMethods.OnDamage;
            OnDeductPP = eventMethods.OnDeductPP;
            OnDisableMove = eventMethods.OnDisableMove;
            OnDragOut = eventMethods.OnDragOut;
            OnEatItem = eventMethods.OnEatItem;
            OnEffectiveness = eventMethods.OnEffectiveness;
            OnEntryHazard = eventMethods.OnEntryHazard;
            OnFaint = eventMethods.OnFaint;
            OnFlinch = eventMethods.OnFlinch;
            OnFractionalPriority = eventMethods.OnFractionalPriority;
            OnHit = eventMethods.OnHit;
            OnImmunity = eventMethods.OnImmunity;
            OnLockMove = eventMethods.OnLockMove;
            OnMaybeTrapPokemon = eventMethods.OnMaybeTrapPokemon;
            OnModifyAccuracy = eventMethods.OnModifyAccuracy;
            OnModifyAtk = eventMethods.OnModifyAtk;
            OnModifyBoost = eventMethods.OnModifyBoost;
            OnModifyCritRatio = eventMethods.OnModifyCritRatio;
            OnModifyDamage = eventMethods.OnModifyDamage;
            OnModifyDef = eventMethods.OnModifyDef;
            OnModifyMove = eventMethods.OnModifyMove;
            OnModifyPriority = eventMethods.OnModifyPriority;
            OnModifySecondaries = eventMethods.OnModifySecondaries;
            OnModifyType = eventMethods.OnModifyType;
            OnModifyTarget = eventMethods.OnModifyTarget;
            OnModifySpA = eventMethods.OnModifySpA;
            OnModifySpD = eventMethods.OnModifySpD;
            OnModifySpe = eventMethods.OnModifySpe;
            OnModifySTAB = eventMethods.OnModifySTAB;
            OnModifyWeight = eventMethods.OnModifyWeight;
            OnMoveAborted = eventMethods.OnMoveAborted;
            OnNegateImmunity = eventMethods.OnNegateImmunity;
            OnOverrideAction = eventMethods.OnOverrideAction;
            OnPrepareHit = eventMethods.OnPrepareHit;
            OnPseudoWeatherChange = eventMethods.OnPseudoWeatherChange;
            OnRedirectTarget = eventMethods.OnRedirectTarget;
            OnResidual = eventMethods.OnResidual;
            OnSetAbility = eventMethods.OnSetAbility;
            OnSetStatus = eventMethods.OnSetStatus;
            OnSetWeather = eventMethods.OnSetWeather;
            OnSideConditionStart = eventMethods.OnSideConditionStart;
            OnStallMove = eventMethods.OnStallMove;
            OnSwitchIn = eventMethods.OnSwitchIn;
            OnSwitchOut = eventMethods.OnSwitchOut;
            OnSwap = eventMethods.OnSwap;
            OnTakeItem = eventMethods.OnTakeItem;
            OnWeatherChange = eventMethods.OnWeatherChange;
            OnTerrainChange = eventMethods.OnTerrainChange;
            OnTrapPokemon = eventMethods.OnTrapPokemon;
            OnTryAddVolatile = eventMethods.OnTryAddVolatile;
            OnTryEatItem = eventMethods.OnTryEatItem;
            OnTryHeal = eventMethods.OnTryHeal;
            OnTryHit = eventMethods.OnTryHit;
            OnTryHitField = eventMethods.OnTryHitField;
            OnTryHitSide = eventMethods.OnTryHitSide;
            OnInvulnerability = eventMethods.OnInvulnerability;
            OnTryMove = eventMethods.OnTryMove;
            OnTryPrimaryHit = eventMethods.OnTryPrimaryHit;
            OnType = eventMethods.OnType;
            OnUseItem = eventMethods.OnUseItem;
            OnUpdate = eventMethods.OnUpdate;
            OnWeather = eventMethods.OnWeather;
            OnWeatherModifyDamage = eventMethods.OnWeatherModifyDamage;
            OnModifyDamagePhase1 = eventMethods.OnModifyDamagePhase1;
            OnModifyDamagePhase2 = eventMethods.OnModifyDamagePhase2;

            // Foe event handlers
            OnFoeDamagingHit = eventMethods.OnFoeDamagingHit;
            OnFoeAfterEachBoost = eventMethods.OnFoeAfterEachBoost;
            OnFoeAfterHit = eventMethods.OnFoeAfterHit;
            OnFoeAfterSetStatus = eventMethods.OnFoeAfterSetStatus;
            OnFoeAfterSubDamage = eventMethods.OnFoeAfterSubDamage;
            OnFoeAfterSwitchInSelf = eventMethods.OnFoeAfterSwitchInSelf;
            OnFoeAfterUseItem = eventMethods.OnFoeAfterUseItem;
            OnFoeAfterBoost = eventMethods.OnFoeAfterBoost;
            OnFoeAfterFaint = eventMethods.OnFoeAfterFaint;
            OnFoeAfterMoveSecondarySelf = eventMethods.OnFoeAfterMoveSecondarySelf;
            OnFoeAfterMoveSecondary = eventMethods.OnFoeAfterMoveSecondary;
            OnFoeAfterMove = eventMethods.OnFoeAfterMove;
            OnFoeAfterMoveSelf = eventMethods.OnFoeAfterMoveSelf;
            OnFoeAttract = eventMethods.OnFoeAttract;
            OnFoeAccuracy = eventMethods.OnFoeAccuracy;
            OnFoeBasePower = eventMethods.OnFoeBasePower;
            OnFoeBeforeFaint = eventMethods.OnFoeBeforeFaint;
            OnFoeBeforeMove = eventMethods.OnFoeBeforeMove;
            OnFoeBeforeSwitchIn = eventMethods.OnFoeBeforeSwitchIn;
            OnFoeBeforeSwitchOut = eventMethods.OnFoeBeforeSwitchOut;
            OnFoeTryBoost = eventMethods.OnFoeTryBoost;
            OnFoeChargeMove = eventMethods.OnFoeChargeMove;
            OnFoeCriticalHit = eventMethods.OnFoeCriticalHit;
            OnFoeDamage = eventMethods.OnFoeDamage;
            OnFoeDeductPP = eventMethods.OnFoeDeductPP;
            OnFoeDisableMove = eventMethods.OnFoeDisableMove;
            OnFoeDragOut = eventMethods.OnFoeDragOut;
            OnFoeEatItem = eventMethods.OnFoeEatItem;
            OnFoeEffectiveness = eventMethods.OnFoeEffectiveness;
            OnFoeFaint = eventMethods.OnFoeFaint;
            OnFoeFlinch = eventMethods.OnFoeFlinch;
            OnFoeHit = eventMethods.OnFoeHit;
            OnFoeImmunity = eventMethods.OnFoeImmunity;
            OnFoeLockMove = eventMethods.OnFoeLockMove;
            OnFoeMaybeTrapPokemon = eventMethods.OnFoeMaybeTrapPokemon;
            OnFoeModifyAccuracy = eventMethods.OnFoeModifyAccuracy;
            OnFoeModifyAtk = eventMethods.OnFoeModifyAtk;
            OnFoeModifyBoost = eventMethods.OnFoeModifyBoost;
            OnFoeModifyCritRatio = eventMethods.OnFoeModifyCritRatio;
            OnFoeModifyDamage = eventMethods.OnFoeModifyDamage;
            OnFoeModifyDef = eventMethods.OnFoeModifyDef;
            OnFoeModifyMove = eventMethods.OnFoeModifyMove;
            OnFoeModifyPriority = eventMethods.OnFoeModifyPriority;
            OnFoeModifySecondaries = eventMethods.OnFoeModifySecondaries;
            OnFoeModifySpA = eventMethods.OnFoeModifySpA;
            OnFoeModifySpD = eventMethods.OnFoeModifySpD;
            OnFoeModifySpe = eventMethods.OnFoeModifySpe;
            OnFoeModifySTAB = eventMethods.OnFoeModifySTAB;
            OnFoeModifyType = eventMethods.OnFoeModifyType;
            OnFoeModifyTarget = eventMethods.OnFoeModifyTarget;
            OnFoeModifyWeight = eventMethods.OnFoeModifyWeight;
            OnFoeMoveAborted = eventMethods.OnFoeMoveAborted;
            OnFoeNegateImmunity = eventMethods.OnFoeNegateImmunity;
            OnFoeOverrideAction = eventMethods.OnFoeOverrideAction;
            OnFoePrepareHit = eventMethods.OnFoePrepareHit;
            OnFoeRedirectTarget = eventMethods.OnFoeRedirectTarget;
            OnFoeResidual = eventMethods.OnFoeResidual;
            OnFoeSetAbility = eventMethods.OnFoeSetAbility;
            OnFoeSetStatus = eventMethods.OnFoeSetStatus;
            OnFoeSetWeather = eventMethods.OnFoeSetWeather;
            OnFoeStallMove = eventMethods.OnFoeStallMove;
            OnFoeSwitchOut = eventMethods.OnFoeSwitchOut;
            OnFoeTakeItem = eventMethods.OnFoeTakeItem;
            OnFoeTerrain = eventMethods.OnFoeTerrain;
            OnFoeTrapPokemon = eventMethods.OnFoeTrapPokemon;
            OnFoeTryAddVolatile = eventMethods.OnFoeTryAddVolatile;
            OnFoeTryEatItem = eventMethods.OnFoeTryEatItem;
            OnFoeTryHeal = eventMethods.OnFoeTryHeal;
            OnFoeTryHit = eventMethods.OnFoeTryHit;
            OnFoeTryHitField = eventMethods.OnFoeTryHitField;
            OnFoeTryHitSide = eventMethods.OnFoeTryHitSide;
            OnFoeInvulnerability = eventMethods.OnFoeInvulnerability;
            OnFoeTryMove = eventMethods.OnFoeTryMove;
            OnFoeTryPrimaryHit = eventMethods.OnFoeTryPrimaryHit;
            OnFoeType = eventMethods.OnFoeType;
            OnFoeWeatherModifyDamage = eventMethods.OnFoeWeatherModifyDamage;
            OnFoeModifyDamagePhase1 = eventMethods.OnFoeModifyDamagePhase1;
            OnFoeModifyDamagePhase2 = eventMethods.OnFoeModifyDamagePhase2;

            // Source event handlers  
            OnSourceDamagingHit = eventMethods.OnSourceDamagingHit;
            OnSourceAfterEachBoost = eventMethods.OnSourceAfterEachBoost;
            OnSourceAfterHit = eventMethods.OnSourceAfterHit;
            OnSourceAfterSetStatus = eventMethods.OnSourceAfterSetStatus;
            OnSourceAfterSubDamage = eventMethods.OnSourceAfterSubDamage;
            OnSourceAfterSwitchInSelf = eventMethods.OnSourceAfterSwitchInSelf;
            OnSourceAfterUseItem = eventMethods.OnSourceAfterUseItem;
            OnSourceAfterBoost = eventMethods.OnSourceAfterBoost;
            OnSourceAfterFaint = eventMethods.OnSourceAfterFaint;
            OnSourceAfterMoveSecondarySelf = eventMethods.OnSourceAfterMoveSecondarySelf;
            OnSourceAfterMoveSecondary = eventMethods.OnSourceAfterMoveSecondary;
            OnSourceAfterMove = eventMethods.OnSourceAfterMove;
            OnSourceAfterMoveSelf = eventMethods.OnSourceAfterMoveSelf;
            OnSourceAttract = eventMethods.OnSourceAttract;
            OnSourceAccuracy = eventMethods.OnSourceAccuracy;
            OnSourceBasePower = eventMethods.OnSourceBasePower;
            OnSourceBeforeFaint = eventMethods.OnSourceBeforeFaint;
            OnSourceBeforeMove = eventMethods.OnSourceBeforeMove;
            OnSourceBeforeSwitchIn = eventMethods.OnSourceBeforeSwitchIn;
            OnSourceBeforeSwitchOut = eventMethods.OnSourceBeforeSwitchOut;
            OnSourceTryBoost = eventMethods.OnSourceTryBoost;
            OnSourceChargeMove = eventMethods.OnSourceChargeMove;
            OnSourceCriticalHit = eventMethods.OnSourceCriticalHit;
            OnSourceDamage = eventMethods.OnSourceDamage;
            OnSourceDeductPP = eventMethods.OnSourceDeductPP;
            OnSourceDisableMove = eventMethods.OnSourceDisableMove;
            OnSourceDragOut = eventMethods.OnSourceDragOut;
            OnSourceEatItem = eventMethods.OnSourceEatItem;
            OnSourceEffectiveness = eventMethods.OnSourceEffectiveness;
            OnSourceFaint = eventMethods.OnSourceFaint;
            OnSourceFlinch = eventMethods.OnSourceFlinch;
            OnSourceHit = eventMethods.OnSourceHit;
            OnSourceImmunity = eventMethods.OnSourceImmunity;
            OnSourceLockMove = eventMethods.OnSourceLockMove;
            OnSourceMaybeTrapPokemon = eventMethods.OnSourceMaybeTrapPokemon;
            OnSourceModifyAccuracy = eventMethods.OnSourceModifyAccuracy;
            OnSourceModifyAtk = eventMethods.OnSourceModifyAtk;
            OnSourceModifyBoost = eventMethods.OnSourceModifyBoost;
            OnSourceModifyCritRatio = eventMethods.OnSourceModifyCritRatio;
            OnSourceModifyDamage = eventMethods.OnSourceModifyDamage;
            OnSourceModifyDef = eventMethods.OnSourceModifyDef;
            OnSourceModifyMove = eventMethods.OnSourceModifyMove;
            OnSourceModifyPriority = eventMethods.OnSourceModifyPriority;
            OnSourceModifySecondaries = eventMethods.OnSourceModifySecondaries;
            OnSourceModifySpA = eventMethods.OnSourceModifySpA;
            OnSourceModifySpD = eventMethods.OnSourceModifySpD;
            OnSourceModifySpe = eventMethods.OnSourceModifySpe;
            OnSourceModifySTAB = eventMethods.OnSourceModifySTAB;
            OnSourceModifyType = eventMethods.OnSourceModifyType;
            OnSourceModifyTarget = eventMethods.OnSourceModifyTarget;
            OnSourceModifyWeight = eventMethods.OnSourceModifyWeight;
            OnSourceMoveAborted = eventMethods.OnSourceMoveAborted;
            OnSourceNegateImmunity = eventMethods.OnSourceNegateImmunity;
            OnSourceOverrideAction = eventMethods.OnSourceOverrideAction;
            OnSourcePrepareHit = eventMethods.OnSourcePrepareHit;
            OnSourceRedirectTarget = eventMethods.OnSourceRedirectTarget;
            OnSourceResidual = eventMethods.OnSourceResidual;
            OnSourceSetAbility = eventMethods.OnSourceSetAbility;
            OnSourceSetStatus = eventMethods.OnSourceSetStatus;
            OnSourceSetWeather = eventMethods.OnSourceSetWeather;
            OnSourceStallMove = eventMethods.OnSourceStallMove;
            OnSourceSwitchOut = eventMethods.OnSourceSwitchOut;
            OnSourceTakeItem = eventMethods.OnSourceTakeItem;
            OnSourceTerrain = eventMethods.OnSourceTerrain;
            OnSourceTrapPokemon = eventMethods.OnSourceTrapPokemon;
            OnSourceTryAddVolatile = eventMethods.OnSourceTryAddVolatile;
            OnSourceTryEatItem = eventMethods.OnSourceTryEatItem;
            OnSourceTryHeal = eventMethods.OnSourceTryHeal;
            OnSourceTryHit = eventMethods.OnSourceTryHit;
            OnSourceTryHitField = eventMethods.OnSourceTryHitField;
            OnSourceTryHitSide = eventMethods.OnSourceTryHitSide;
            OnSourceInvulnerability = eventMethods.OnSourceInvulnerability;
            OnSourceTryMove = eventMethods.OnSourceTryMove;
            OnSourceTryPrimaryHit = eventMethods.OnSourceTryPrimaryHit;
            OnSourceType = eventMethods.OnSourceType;
            OnSourceWeatherModifyDamage = eventMethods.OnSourceWeatherModifyDamage;
            OnSourceModifyDamagePhase1 = eventMethods.OnSourceModifyDamagePhase1;
            OnSourceModifyDamagePhase2 = eventMethods.OnSourceModifyDamagePhase2;

            // Any event handlers
            OnAnyDamagingHit = eventMethods.OnAnyDamagingHit;
            OnAnyAfterEachBoost = eventMethods.OnAnyAfterEachBoost;
            OnAnyAfterHit = eventMethods.OnAnyAfterHit;
            OnAnyAfterSetStatus = eventMethods.OnAnyAfterSetStatus;
            OnAnyAfterSubDamage = eventMethods.OnAnyAfterSubDamage;
            OnAnyAfterSwitchInSelf = eventMethods.OnAnyAfterSwitchInSelf;
            OnAnyAfterUseItem = eventMethods.OnAnyAfterUseItem;
            OnAnyAfterBoost = eventMethods.OnAnyAfterBoost;
            OnAnyAfterFaint = eventMethods.OnAnyAfterFaint;
            OnAnyAfterMega = eventMethods.OnAnyAfterMega;
            OnAnyAfterMoveSecondarySelf = eventMethods.OnAnyAfterMoveSecondarySelf;
            OnAnyAfterMoveSecondary = eventMethods.OnAnyAfterMoveSecondary;
            OnAnyAfterMove = eventMethods.OnAnyAfterMove;
            OnAnyAfterMoveSelf = eventMethods.OnAnyAfterMoveSelf;
            OnAnyAfterTerastallization = eventMethods.OnAnyAfterTerastallization;
            OnAnyAttract = eventMethods.OnAnyAttract;
            OnAnyAccuracy = eventMethods.OnAnyAccuracy;
            OnAnyBasePower = eventMethods.OnAnyBasePower;
            OnAnyBeforeFaint = eventMethods.OnAnyBeforeFaint;
            OnAnyBeforeMove = eventMethods.OnAnyBeforeMove;
            OnAnyBeforeSwitchIn = eventMethods.OnAnyBeforeSwitchIn;
            OnAnyBeforeSwitchOut = eventMethods.OnAnyBeforeSwitchOut;
            OnAnyTryBoost = eventMethods.OnAnyTryBoost;
            OnAnyChargeMove = eventMethods.OnAnyChargeMove;
            OnAnyCriticalHit = eventMethods.OnAnyCriticalHit;
            OnAnyDamage = eventMethods.OnAnyDamage;
            OnAnyDeductPP = eventMethods.OnAnyDeductPP;
            OnAnyDisableMove = eventMethods.OnAnyDisableMove;
            OnAnyDragOut = eventMethods.OnAnyDragOut;
            OnAnyEatItem = eventMethods.OnAnyEatItem;
            OnAnyEffectiveness = eventMethods.OnAnyEffectiveness;
            OnAnyFaint = eventMethods.OnAnyFaint;
            OnAnyFlinch = eventMethods.OnAnyFlinch;
            OnAnyHit = eventMethods.OnAnyHit;
            OnAnyImmunity = eventMethods.OnAnyImmunity;
            OnAnyLockMove = eventMethods.OnAnyLockMove;
            OnAnyMaybeTrapPokemon = eventMethods.OnAnyMaybeTrapPokemon;
            OnAnyModifyAccuracy = eventMethods.OnAnyModifyAccuracy;
            OnAnyModifyAtk = eventMethods.OnAnyModifyAtk;
            OnAnyModifyBoost = eventMethods.OnAnyModifyBoost;
            OnAnyModifyCritRatio = eventMethods.OnAnyModifyCritRatio;
            OnAnyModifyDamage = eventMethods.OnAnyModifyDamage;
            OnAnyModifyDef = eventMethods.OnAnyModifyDef;
            OnAnyModifyMove = eventMethods.OnAnyModifyMove;
            OnAnyModifyPriority = eventMethods.OnAnyModifyPriority;
            OnAnyModifySecondaries = eventMethods.OnAnyModifySecondaries;
            OnAnyModifySpA = eventMethods.OnAnyModifySpA;
            OnAnyModifySpD = eventMethods.OnAnyModifySpD;
            OnAnyModifySpe = eventMethods.OnAnyModifySpe;
            OnAnyModifySTAB = eventMethods.OnAnyModifySTAB;
            OnAnyModifyType = eventMethods.OnAnyModifyType;
            OnAnyModifyTarget = eventMethods.OnAnyModifyTarget;
            OnAnyModifyWeight = eventMethods.OnAnyModifyWeight;
            OnAnyMoveAborted = eventMethods.OnAnyMoveAborted;
            OnAnyNegateImmunity = eventMethods.OnAnyNegateImmunity;
            OnAnyOverrideAction = eventMethods.OnAnyOverrideAction;
            OnAnyPrepareHit = eventMethods.OnAnyPrepareHit;
            OnAnyPseudoWeatherChange = eventMethods.OnAnyPseudoWeatherChange;
            OnAnyRedirectTarget = eventMethods.OnAnyRedirectTarget;
            OnAnyResidual = eventMethods.OnAnyResidual;
            OnAnySetAbility = eventMethods.OnAnySetAbility;
            OnAnySetStatus = eventMethods.OnAnySetStatus;
            OnAnySetWeather = eventMethods.OnAnySetWeather;
            OnAnyStallMove = eventMethods.OnAnyStallMove;
            OnAnySwitchIn = eventMethods.OnAnySwitchIn;
            OnAnySwitchOut = eventMethods.OnAnySwitchOut;
            OnAnyTakeItem = eventMethods.OnAnyTakeItem;
            OnAnyTerrain = eventMethods.OnAnyTerrain;
            OnAnyTrapPokemon = eventMethods.OnAnyTrapPokemon;
            OnAnyTryAddVolatile = eventMethods.OnAnyTryAddVolatile;
            OnAnyTryEatItem = eventMethods.OnAnyTryEatItem;
            OnAnyTryHeal = eventMethods.OnAnyTryHeal;
            OnAnyTryHit = eventMethods.OnAnyTryHit;
            OnAnyTryHitField = eventMethods.OnAnyTryHitField;
            OnAnyTryHitSide = eventMethods.OnAnyTryHitSide;
            OnAnyInvulnerability = eventMethods.OnAnyInvulnerability;
            OnAnyTryMove = eventMethods.OnAnyTryMove;
            OnAnyTryPrimaryHit = eventMethods.OnAnyTryPrimaryHit;
            OnAnyType = eventMethods.OnAnyType;
            OnAnyWeatherModifyDamage = eventMethods.OnAnyWeatherModifyDamage;
            OnAnyModifyDamagePhase1 = eventMethods.OnAnyModifyDamagePhase1;
            OnAnyModifyDamagePhase2 = eventMethods.OnAnyModifyDamagePhase2;

            // Priority fields
            OnAccuracyPriority = eventMethods.OnAccuracyPriority;
            OnDamagingHitOrder = eventMethods.OnDamagingHitOrder;
            OnAfterMoveSecondaryPriority = eventMethods.OnAfterMoveSecondaryPriority;
            OnAfterMoveSecondarySelfPriority = eventMethods.OnAfterMoveSecondarySelfPriority;
            OnAfterMoveSelfPriority = eventMethods.OnAfterMoveSelfPriority;
            OnAfterSetStatusPriority = eventMethods.OnAfterSetStatusPriority;
            OnAnyBasePowerPriority = eventMethods.OnAnyBasePowerPriority;
            OnAnyInvulnerabilityPriority = eventMethods.OnAnyInvulnerabilityPriority;
            OnAnyModifyAccuracyPriority = eventMethods.OnAnyModifyAccuracyPriority;
            OnAnyFaintPriority = eventMethods.OnAnyFaintPriority;
            OnAnyPrepareHitPriority = eventMethods.OnAnyPrepareHitPriority;
            OnAnySwitchInPriority = eventMethods.OnAnySwitchInPriority;
            OnAnySwitchInSubOrder = eventMethods.OnAnySwitchInSubOrder;
            OnAllyBasePowerPriority = eventMethods.OnAllyBasePowerPriority;
            OnAllyModifyAtkPriority = eventMethods.OnAllyModifyAtkPriority;
            OnAllyModifySpAPriority = eventMethods.OnAllyModifySpAPriority;
            OnAllyModifySpDPriority = eventMethods.OnAllyModifySpDPriority;
            OnAttractPriority = eventMethods.OnAttractPriority;
            OnBasePowerPriority = eventMethods.OnBasePowerPriority;
            OnBeforeMovePriority = eventMethods.OnBeforeMovePriority;
            OnBeforeSwitchOutPriority = eventMethods.OnBeforeSwitchOutPriority;
            OnChangeBoostPriority = eventMethods.OnChangeBoostPriority;
            OnDamagePriority = eventMethods.OnDamagePriority;
            OnDragOutPriority = eventMethods.OnDragOutPriority;
            OnEffectivenessPriority = eventMethods.OnEffectivenessPriority;
            OnFoeBasePowerPriority = eventMethods.OnFoeBasePowerPriority;
            OnFoeBeforeMovePriority = eventMethods.OnFoeBeforeMovePriority;
            OnFoeModifyDefPriority = eventMethods.OnFoeModifyDefPriority;
            OnFoeModifySpDPriority = eventMethods.OnFoeModifySpDPriority;
            OnFoeRedirectTargetPriority = eventMethods.OnFoeRedirectTargetPriority;
            OnFoeTrapPokemonPriority = eventMethods.OnFoeTrapPokemonPriority;
            OnFractionalPriorityPriority = eventMethods.OnFractionalPriorityPriority;
            OnHitPriority = eventMethods.OnHitPriority;
            OnInvulnerabilityPriority = eventMethods.OnInvulnerabilityPriority;
            OnModifyAccuracyPriority = eventMethods.OnModifyAccuracyPriority;
            OnModifyAtkPriority = eventMethods.OnModifyAtkPriority;
            OnModifyCritRatioPriority = eventMethods.OnModifyCritRatioPriority;
            OnModifyDefPriority = eventMethods.OnModifyDefPriority;
            OnModifyMovePriority = eventMethods.OnModifyMovePriority;
            OnModifyPriorityPriority = eventMethods.OnModifyPriorityPriority;
            OnModifySpAPriority = eventMethods.OnModifySpAPriority;
            OnModifySpDPriority = eventMethods.OnModifySpDPriority;
            OnModifySpePriority = eventMethods.OnModifySpePriority;
            OnModifySTABPriority = eventMethods.OnModifySTABPriority;
            OnModifyTypePriority = eventMethods.OnModifyTypePriority;
            OnModifyWeightPriority = eventMethods.OnModifyWeightPriority;
            OnRedirectTargetPriority = eventMethods.OnRedirectTargetPriority;
            OnResidualOrder = eventMethods.OnResidualOrder;
            OnResidualPriority = eventMethods.OnResidualPriority;
            OnResidualSubOrder = eventMethods.OnResidualSubOrder;
            OnSourceBasePowerPriority = eventMethods.OnSourceBasePowerPriority;
            OnSourceInvulnerabilityPriority = eventMethods.OnSourceInvulnerabilityPriority;
            OnSourceModifyAccuracyPriority = eventMethods.OnSourceModifyAccuracyPriority;
            OnSourceModifyAtkPriority = eventMethods.OnSourceModifyAtkPriority;
            OnSourceModifyDamagePriority = eventMethods.OnSourceModifyDamagePriority;
            OnSourceModifySpAPriority = eventMethods.OnSourceModifySpAPriority;
            OnSwitchInPriority = eventMethods.OnSwitchInPriority;
            OnSwitchInSubOrder = eventMethods.OnSwitchInSubOrder;
            OnTrapPokemonPriority = eventMethods.OnTrapPokemonPriority;
            OnTryBoostPriority = eventMethods.OnTryBoostPriority;
            OnTryEatItemPriority = eventMethods.OnTryEatItemPriority;
            OnTryHealPriority = eventMethods.OnTryHealPriority;
            OnTryHitPriority = eventMethods.OnTryHitPriority;
            OnTryMovePriority = eventMethods.OnTryMovePriority;
            OnTryPrimaryHitPriority = eventMethods.OnTryPrimaryHitPriority;
            OnTypePriority = eventMethods.OnTypePriority;
        }

        private void CopyPokemonEventMethods(IPokemonEventMethods eventMethods)
        {
            // Ally event handlers (triggered by ally Pokémon actions)
            OnAllyDamagingHit = eventMethods.OnAllyDamagingHit;
            OnAllyAfterEachBoost = eventMethods.OnAllyAfterEachBoost;
            OnAllyAfterHit = eventMethods.OnAllyAfterHit;
            OnAllyAfterSetStatus = eventMethods.OnAllyAfterSetStatus;
            OnAllyAfterSubDamage = eventMethods.OnAllyAfterSubDamage;
            OnAllyAfterSwitchInSelf = eventMethods.OnAllyAfterSwitchInSelf;
            OnAllyAfterUseItem = eventMethods.OnAllyAfterUseItem;
            OnAllyAfterBoost = eventMethods.OnAllyAfterBoost;
            OnAllyAfterFaint = eventMethods.OnAllyAfterFaint;
            OnAllyAfterMoveSecondarySelf = eventMethods.OnAllyAfterMoveSecondarySelf;
            OnAllyAfterMoveSecondary = eventMethods.OnAllyAfterMoveSecondary;
            OnAllyAfterMove = eventMethods.OnAllyAfterMove;
            OnAllyAfterMoveSelf = eventMethods.OnAllyAfterMoveSelf;
            OnAllyAttract = eventMethods.OnAllyAttract;
            OnAllyAccuracy = eventMethods.OnAllyAccuracy;
            OnAllyBasePower = eventMethods.OnAllyBasePower;
            OnAllyBeforeFaint = eventMethods.OnAllyBeforeFaint;
            OnAllyBeforeMove = eventMethods.OnAllyBeforeMove;
            OnAllyBeforeSwitchIn = eventMethods.OnAllyBeforeSwitchIn;
            OnAllyBeforeSwitchOut = eventMethods.OnAllyBeforeSwitchOut;
            OnAllyTryBoost = eventMethods.OnAllyTryBoost;
            OnAllyChargeMove = eventMethods.OnAllyChargeMove;
            OnAllyCriticalHit = eventMethods.OnAllyCriticalHit;
            OnAllyDamage = eventMethods.OnAllyDamage;
            OnAllyDeductPP = eventMethods.OnAllyDeductPP;
            OnAllyDisableMove = eventMethods.OnAllyDisableMove;
            OnAllyDragOut = eventMethods.OnAllyDragOut;
            OnAllyEatItem = eventMethods.OnAllyEatItem;
            OnAllyEffectiveness = eventMethods.OnAllyEffectiveness;
            OnAllyFaint = eventMethods.OnAllyFaint;
            OnAllyFlinch = eventMethods.OnAllyFlinch;
            OnAllyHit = eventMethods.OnAllyHit;
            OnAllyImmunity = eventMethods.OnAllyImmunity;
            OnAllyLockMove = eventMethods.OnAllyLockMove;
            OnAllyMaybeTrapPokemon = eventMethods.OnAllyMaybeTrapPokemon;
            OnAllyModifyAccuracy = eventMethods.OnAllyModifyAccuracy;
            OnAllyModifyAtk = eventMethods.OnAllyModifyAtk;
            OnAllyModifyBoost = eventMethods.OnAllyModifyBoost;
            OnAllyModifyCritRatio = eventMethods.OnAllyModifyCritRatio;
            OnAllyModifyDamage = eventMethods.OnAllyModifyDamage;
            OnAllyModifyDef = eventMethods.OnAllyModifyDef;
            OnAllyModifyMove = eventMethods.OnAllyModifyMove;
            OnAllyModifyPriority = eventMethods.OnAllyModifyPriority;
            OnAllyModifySecondaries = eventMethods.OnAllyModifySecondaries;
            OnAllyModifySpA = eventMethods.OnAllyModifySpA;
            OnAllyModifySpD = eventMethods.OnAllyModifySpD;
            OnAllyModifySpe = eventMethods.OnAllyModifySpe;
            OnAllyModifySTAB = eventMethods.OnAllyModifySTAB;
            OnAllyModifyType = eventMethods.OnAllyModifyType;
            OnAllyModifyTarget = eventMethods.OnAllyModifyTarget;
            OnAllyModifyWeight = eventMethods.OnAllyModifyWeight;
            OnAllyMoveAborted = eventMethods.OnAllyMoveAborted;
            OnAllyNegateImmunity = eventMethods.OnAllyNegateImmunity;
            OnAllyOverrideAction = eventMethods.OnAllyOverrideAction;
            OnAllyPrepareHit = eventMethods.OnAllyPrepareHit;
            OnAllyRedirectTarget = eventMethods.OnAllyRedirectTarget;
            OnAllyResidual = eventMethods.OnAllyResidual;
            OnAllySetAbility = eventMethods.OnAllySetAbility;
            OnAllySetStatus = eventMethods.OnAllySetStatus;
            OnAllySetWeather = eventMethods.OnAllySetWeather;
            OnAllyStallMove = eventMethods.OnAllyStallMove;
            OnAllySwitchOut = eventMethods.OnAllySwitchOut;
            OnAllyTakeItem = eventMethods.OnAllyTakeItem;
            OnAllyTerrain = eventMethods.OnAllyTerrain;
            OnAllyTrapPokemon = eventMethods.OnAllyTrapPokemon;
            OnAllyTryAddVolatile = eventMethods.OnAllyTryAddVolatile;
            OnAllyTryEatItem = eventMethods.OnAllyTryEatItem;
            OnAllyTryHeal = eventMethods.OnAllyTryHeal;
            OnAllyTryHit = eventMethods.OnAllyTryHit;
            OnAllyTryHitField = eventMethods.OnAllyTryHitField;
            OnAllyTryHitSide = eventMethods.OnAllyTryHitSide;
            OnAllyInvulnerability = eventMethods.OnAllyInvulnerability;
            OnAllyTryMove = eventMethods.OnAllyTryMove;
            OnAllyTryPrimaryHit = eventMethods.OnAllyTryPrimaryHit;
            OnAllyType = eventMethods.OnAllyType;
            OnAllyWeatherModifyDamage = eventMethods.OnAllyWeatherModifyDamage;
            OnAllyModifyDamagePhase1 = eventMethods.OnAllyModifyDamagePhase1;
            OnAllyModifyDamagePhase2 = eventMethods.OnAllyModifyDamagePhase2;
        }

        private void CopySideEventMethods(ISideEventMethods eventMethods)
        {
            // Side condition lifecycle events
            OnSideStart = eventMethods.OnSideStart;
            OnSideRestart = eventMethods.OnSideRestart;
            OnSideResidual = eventMethods.OnSideResidual;
            OnSideEnd = eventMethods.OnSideEnd;

            // Side event priorities and ordering
            OnSideResidualOrder = eventMethods.OnSideResidualOrder;
            OnSideResidualPriority = eventMethods.OnSideResidualPriority;
            OnSideResidualSubOrder = eventMethods.OnSideResidualSubOrder;
        }

        private void CopyFieldEventMethods(IFieldEventMethods eventMethods)
        {
            // Field condition lifecycle events
            OnFieldStart = eventMethods.OnFieldStart;
            OnFieldRestart = eventMethods.OnFieldRestart;
            OnFieldResidual = eventMethods.OnFieldResidual;
            OnFieldEnd = eventMethods.OnFieldEnd;

            // Field event priorities and ordering
            OnFieldResidualOrder = eventMethods.OnFieldResidualOrder;
            OnFieldResidualPriority = eventMethods.OnFieldResidualPriority;
            OnFieldResidualSubOrder = eventMethods.OnFieldResidualSubOrder;
        }

        public void Init()
        {
            InitBasicEffect();

            if (EffectType != EffectType.Condition ||
                EffectType != EffectType.Weather ||
                EffectType != EffectType.Status ||
                EffectType != EffectType.Terrain)
            {
                throw new ArgumentException();
            }
        }
    }

    public static class  ConditionConstants
    {
        public static readonly Condition EmptyCondition = new(new PokemonConditionData
        {
            Name = "",
            Exists = false,
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
