using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApogeeVGC_CS.sim;

namespace ApogeeVGC_CS.sim
{
    public interface IAbilityEventMethods
    {
        Action<Battle, Pokemon>? OnCheckShow { get; set; }
        Action<Battle, Pokemon>? OnEnd { get; set; }
        Action<Battle, Pokemon>? OnStart { get; set; }
    }

    public class AbilityFlags
    {
        public bool Breakable { get; set; } // Can be suppressed by Mold Breaker and related effects
        public bool CantSuppress { get; set; } // Ability can't be suppressed by e.g. Gastro Acid or Neutralizing Gas
        public bool FailRolePlay { get; set; } // Role Play fails if target has this Ability
        public bool FailSkillSwap { get; set; } // Skill Swap fails if either the user or target has this Ability
        public bool NoEntrain { get; set; } // Entrainment fails if user has this Ability
        public bool NoReceiver { get; set; } // Receiver and Power of Alchemy will not activate if an ally faints with this Ability
        public bool NoTrace { get; set; } // Trace cannot copy this Ability
        public bool NoTransform { get; set; } // Disables the Ability if the user is Transformed
    }

    //public interface IAbilityData : IAbilityEventMethods, IPokemonEventMethods
    //{
    //    string Name { get; set; }
    //    // Additional properties from Ability can be added here
    //}

    public class AbilityData : IAbility, IAbilityEventMethods, IPokemonEventMethods
    {
        public string Name { get; set; } = string.Empty;
        public Action<Battle, Pokemon>? OnCheckShow { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnEnd { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<Battle, Pokemon>? OnStart { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
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
        public Id Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Fullname { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public EffectType EffectType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Exists { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Num { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Gen { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string? ShortDesc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string? Desc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Nonstandard? IsNonstandard { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? Duration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool NoCopy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AffectsFainted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Id? Status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Id? Weather { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SourceEffect { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string? EffectTypeString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool? Infiltrates { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string? RealMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Rating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool SuppressWeather { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AbilityFlags Flags { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IConditionData? Condition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    // Modded ability data with inheritance support
    public class ModdedAbilityData : AbilityData
    {
        public bool Inherit { get; set; } = true;
    }

    // Type aliases for data tables
    public class AbilityDataTable : Dictionary<IdEntry, AbilityData> { }
    public class ModdedAbilityDataTable : Dictionary<IdEntry, ModdedAbilityData> { }

    /// <summary>
    /// Represents an ability effect.
    /// </summary>
    public interface IAbility : IBasicEffect, IEffect 
    {
        int Rating { get; set; }
        bool SuppressWeather { get; set; }
        AbilityFlags Flags { get; set; }
        IConditionData? Condition { get; set; }
    }

    // Main Ability class extending BasicEffect
    public class Ability : BasicEffect, IAbility
    {
        // Rating from -1 Detrimental to +5 Essential
        public int Rating { get; set; } = 0;
        public bool SuppressWeather { get; set; } = false;
        public AbilityFlags Flags { get; set; } = new();
        public IConditionData? Condition { get; set; } = null;

        public Ability(IAbility other) : base(other)
        {
            Rating = other.Rating;
            SuppressWeather = other.SuppressWeather;
            Flags = other.Flags;
            Condition = other.Condition;

            Init();
        }

        private void Init()
        {

            Fullname = $"ability: {Name}";
            EffectType = EffectType.Ability;

            if (Gen == 0)
            {
                if (Num >= 268)
                    Gen = 9;
                else if (Num >= 234)
                    Gen = 8;
                else if (Num >= 192)
                    Gen = 7;
                else if (Num >= 165)
                    Gen = 6;
                else if (Num >= 124)
                    Gen = 5;
                else if (Num >= 77)
                    Gen = 4;
                else if (Num >= 1)
                    Gen = 3;
            }
        }
    }

    public static class AbilityConstants
    {
        public static readonly Ability EmptyAbility = new(new AbilityData
        {
            Id = new Id(string.Empty),
            Name = "",
            Exists = false
        });
    }

    public class DexAbilities(ModdedDex dex)
    {
        public ModdedDex Dex { get; } = dex;
        private readonly Dictionary<Id, Ability> _abilityCache = [];
        private List<Ability>? _allCache = null;

        public Ability Get(string name = "")
        {
            throw new NotImplementedException("Get method is not implemented yet.");
        }

        public Ability Get(Ability ability)
        {
            throw new NotImplementedException();
        }

        public Ability GetById(Id id)
        {
            throw new NotImplementedException();
        }

        public List<Ability> All()
        {
            throw new NotImplementedException("All method is not implemented yet.");
        }
    }
}