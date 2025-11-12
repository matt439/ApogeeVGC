using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.FormatClasses;


public enum FormatId
{
    Gen9Ou,
    CustomSingles,
    CustomSinglesBlind,
    CustomDoubles,
}

public enum FormatEffectType
{
    Format,
    Ruleset,
    Rule,
    ValidatorRule,
}

public enum RuleId
{
    Standard,
    OverflowStatMod,
    EndlessBattleClause,
}

public record Format : IEffect, IBasicEffect, IEventMethodsV2, ICopyable<Format>
{
    public FormatId FormatId { get; init; }
    public EffectStateId EffectStateId => FormatId;
    public EffectType EffectType => EffectType.Format;
    public required string Name { get; init; }
    public string FullName => $"format: {Name}";
    public string? Desc { get; init; }
    public bool AffectsFainted { get; init; }
    public FormatEffectType FormatEffectType { get; init; }
    public GameType GameType { get; init; }
    public static int PlayerCount => 2;

    public IReadOnlyList<RuleId> Ruleset { get; init; } = [];
    public IReadOnlyList<RuleId> BaseRuleset { get; init; } = [];
    public IReadOnlyList<RuleId> Banlist { get; init; } = [];
    public IReadOnlyList<RuleId> Restricted { get; init; } = [];
    public IReadOnlyList<RuleId> Unbanlist { get; init; } = [];
    public IReadOnlyList<RuleId>? CustomRules { get; init; }
    public RuleTable? RuleTable { get; set; }
    public Action<Battle>? OnBegin { get; init; }
    public bool NoLog { get; init; }

    public FormatHasValue? HasValue { get; init; }
    // OnValidateRule TODO: Implement this event method
    public RuleId? MutuallyExclusiveWith { get; init; }
    // ModdedDex fields here. Possible unnecessary in this context.
    public bool? ChallengeShow { get; init; }
    public bool? SearchShow { get; init; }
    public bool? BestOfDefault { get; init; }
    public bool? TeraPreviewDefault { get; init; }
    // Threads // TODO: Implement thread handling
    public bool? TournamentShow { get; init; }


    public Func<TeamValidator, Move, Species, PokemonSources, PokemonSet, string?>? CheckCanLearn { get; init; }
    public Func<Format, SpecieId, SpecieId>? GetEvoFamily { get; init; }
    public Func<Format, Pokemon, HashSet<string>>? GetSharedPower { get; init; }
    public Func<Format, Pokemon, HashSet<string>>? GetSharedItems { get; init; }
    public Func<TeamValidator, PokemonSet, Format, object?, object?, string[]?>? OnChangeSet { get; init; }
    public int? OnModifySpeciesPriority { get; init; }
    public Func<Battle, Species, Pokemon?, Pokemon?, IEffect?, Species?>? OnModifySpecies { get; init; }
    public Action<Battle>? OnBattleStart { get; init; }
    public Action<Battle>? OnTeamPreview { get; init; }
    public Func<TeamValidator, PokemonSet, Format, object, object, string[]?>? OnValidateSet { get; init; }
    public Func<TeamValidator, PokemonSet[], Format, object, string[]?>? OnValidateTeam { get; init; }
    public Func<TeamValidator, PokemonSet, object, string[]?>? ValidateSet { get; init; }
    public Func<TeamValidator, PokemonSet[], ValidateTeamOptions?, string[]?>? ValidateTeam { get; init; }

    public class ValidateTeamOptions
    {
        public bool RemoveNicknames { get; init; }
        public Dictionary<string, Dictionary<string, bool>>? SkipSets { get; init; }
    }
    public string? Section { get; init; }
    public int? Column { get; init; }


    #region IEventMethods Implementation

    public OnDamagingHitEventInfo? OnDamagingHit { get; init; }
    public OnEmergencyExitEventInfo? OnEmergencyExit { get; init; }
    public OnAfterEachBoostEventInfo? OnAfterEachBoost { get; init; }
    public OnAfterHitEventInfo? OnAfterHit { get; init; }
    public OnAfterMegaEventInfo? OnAfterMega { get; init; }
    public OnAfterSetStatusEventInfo? OnAfterSetStatus { get; init; }
    public OnAfterSubDamageEventInfo? OnAfterSubDamage { get; init; }
    public OnAfterSwitchInSelfEventInfo? OnAfterSwitchInSelf { get; init; }
    public OnAfterTerastallizationEventInfo? OnAfterTerastallization { get; init; }
    public OnAfterUseItemEventInfo? OnAfterUseItem { get; init; }
    public OnAfterTakeItemEventInfo? OnAfterTakeItem { get; init; }
    public OnAfterBoostEventInfo? OnAfterBoost { get; init; }
    public OnAfterFaintEventInfo? OnAfterFaint { get; init; }
    public OnAfterMoveSecondarySelfEventInfo? OnAfterMoveSecondarySelf { get; init; }
    public OnAfterMoveSecondaryEventInfo? OnAfterMoveSecondary { get; init; }
    public OnAfterMoveEventInfo? OnAfterMove { get; init; }
    public OnAfterMoveSelfEventInfo? OnAfterMoveSelf { get; init; }
    public OnAttractEventInfo? OnAttract { get; init; }
    public OnAccuracyEventInfo? OnAccuracy { get; init; }
    public OnBasePowerEventInfo? OnBasePower { get; init; }
    public OnBeforeFaintEventInfo? OnBeforeFaint { get; init; }
    public OnBeforeMoveEventInfo? OnBeforeMove { get; init; }
    public OnBeforeSwitchInEventInfo? OnBeforeSwitchIn { get; init; }
    public OnBeforeSwitchOutEventInfo? OnBeforeSwitchOut { get; init; }
    public OnBeforeTurnEventInfo? OnBeforeTurn { get; init; }
    public OnChangeBoostEventInfo? OnChangeBoost { get; init; }
    public OnTryBoostEventInfo? OnTryBoost { get; init; }
    public OnChargeMoveEventInfo? OnChargeMove { get; init; }
    public OnCriticalHitEventInfo? OnCriticalHit { get; init; }
    public OnDamageEventInfo? OnDamage { get; init; }
    public OnDeductPpEventInfo? OnDeductPp { get; init; }
    public OnDisableMoveEventInfo? OnDisableMove { get; init; }
    public OnDragOutEventInfo? OnDragOut { get; init; }
    public OnEatItemEventInfo? OnEatItem { get; init; }
    public OnEffectivenessEventInfo? OnEffectiveness { get; init; }
    public OnEntryHazardEventInfo? OnEntryHazard { get; init; }
    public OnFaintEventInfo? OnFaint { get; init; }
    public OnFlinchEventInfo? OnFlinch { get; init; }
    public OnFractionalPriorityEventInfo? OnFractionalPriority { get; init; }
    public OnHitEventInfo? OnHit { get; init; }
    public OnImmunityEventInfo? OnImmunity { get; init; }
    public OnLockMoveEventInfo? OnLockMove { get; init; }
    public OnMaybeTrapPokemonEventInfo? OnMaybeTrapPokemon { get; init; }
    public OnModifyAccuracyEventInfo? OnModifyAccuracy { get; init; }
    public OnModifyAtkEventInfo? OnModifyAtk { get; init; }
    public OnModifyBoostEventInfo? OnModifyBoost { get; init; }
    public OnModifyCritRatioEventInfo? OnModifyCritRatio { get; init; }
    public OnModifyDamageEventInfo? OnModifyDamage { get; init; }
    public OnModifyDefEventInfo? OnModifyDef { get; init; }
    public OnModifyMoveEventInfo? OnModifyMove { get; init; }
    public OnModifyPriorityEventInfo? OnModifyPriority { get; init; }
    public OnModifySecondariesEventInfo? OnModifySecondaries { get; init; }
    public OnModifyTypeEventInfo? OnModifyType { get; init; }
    public OnModifyTargetEventInfo? OnModifyTarget { get; init; }
    public OnModifySpAEventInfo? OnModifySpA { get; init; }
    public OnModifySpDEventInfo? OnModifySpD { get; init; }
    public OnModifySpeEventInfo? OnModifySpe { get; init; }
    public OnModifyStabEventInfo? OnModifyStab { get; init; }
    public OnModifyWeightEventInfo? OnModifyWeight { get; init; }
    public OnMoveAbortedEventInfo? OnMoveAborted { get; init; }
    public OnNegateImmunityEventInfo? OnNegateImmunity { get; init; }
    public OnOverrideActionEventInfo? OnOverrideAction { get; init; }
    public OnPrepareHitEventInfo? OnPrepareHit { get; init; }
    public OnPseudoWeatherChangeEventInfo? OnPseudoWeatherChange { get; init; }
    public OnRedirectTargetEventInfo? OnRedirectTarget { get; init; }
    public OnResidualEventInfo? OnResidual { get; init; }
    public OnSetAbilityEventInfo? OnSetAbility { get; init; }
    public OnSetStatusEventInfo? OnSetStatus { get; init; }
    public OnSetWeatherEventInfo? OnSetWeather { get; init; }
    public OnSideConditionStartEventInfo? OnSideConditionStart { get; init; }
    public OnStallMoveEventInfo? OnStallMove { get; init; }
    public OnSwitchInEventInfo? OnSwitchIn { get; init; }
    public OnSwitchOutEventInfo? OnSwitchOut { get; init; }
    public OnSwapEventInfo? OnSwap { get; init; }
    public OnTakeItemEventInfo? OnTakeItem { get; init; }
    public OnWeatherChangeEventInfo? OnWeatherChange { get; init; }
    public OnTerrainChangeEventInfo? OnTerrainChange { get; init; }
    public OnTrapPokemonEventInfo? OnTrapPokemon { get; init; }
    public OnTryAddVolatileEventInfo? OnTryAddVolatile { get; init; }
    public OnTryEatItemEventInfo? OnTryEatItem { get; init; }
    public OnTryHealEventInfo? OnTryHeal { get; init; }
    public OnTryHitEventInfo? OnTryHit { get; init; }
    public OnTryHitFieldEventInfo? OnTryHitField { get; init; }
    public OnTryHitSideEventInfo? OnTryHitSide { get; init; }
    public OnInvulnerabilityEventInfo? OnInvulnerability { get; init; }
    public OnTryMoveEventInfo? OnTryMove { get; init; }
    public OnTryPrimaryHitEventInfo? OnTryPrimaryHit { get; init; }
    public OnTypeEventInfo? OnType { get; init; }
    public OnUseItemEventInfo? OnUseItem { get; init; }
    public OnUpdateEventInfo? OnUpdate { get; init; }
    public OnWeatherEventInfo? OnWeather { get; init; }
    public OnWeatherModifyDamageEventInfo? OnWeatherModifyDamage { get; init; }
    public OnModifyDamagePhase1EventInfo? OnModifyDamagePhase1 { get; init; }
    public OnModifyDamagePhase2EventInfo? OnModifyDamagePhase2 { get; init; }
    public OnFoeDamagingHitEventInfo? OnFoeDamagingHit { get; init; }
    public OnFoeAfterEachBoostEventInfo? OnFoeAfterEachBoost { get; init; }
    public OnFoeAfterHitEventInfo? OnFoeAfterHit { get; init; }
    public OnFoeAfterSetStatusEventInfo? OnFoeAfterSetStatus { get; init; }
    public OnFoeAfterSubDamageEventInfo? OnFoeAfterSubDamage { get; init; }
    public OnFoeAfterSwitchInSelfEventInfo? OnFoeAfterSwitchInSelf { get; init; }
    public OnFoeAfterUseItemEventInfo? OnFoeAfterUseItem { get; init; }
    public OnFoeAfterBoostEventInfo? OnFoeAfterBoost { get; init; }
    public OnFoeAfterFaintEventInfo? OnFoeAfterFaint { get; init; }
    public OnFoeAfterMoveSecondarySelfEventInfo? OnFoeAfterMoveSecondarySelf { get; init; }
    public OnFoeAfterMoveSecondaryEventInfo? OnFoeAfterMoveSecondary { get; init; }
    public OnFoeAfterMoveEventInfo? OnFoeAfterMove { get; init; }
    public OnFoeAfterMoveSelfEventInfo? OnFoeAfterMoveSelf { get; init; }
    public OnFoeAttractEventInfo? OnFoeAttract { get; init; }
    public OnFoeAccuracyEventInfo? OnFoeAccuracy { get; init; }
    public OnFoeBasePowerEventInfo? OnFoeBasePower { get; init; }
    public OnFoeBeforeFaintEventInfo? OnFoeBeforeFaint { get; init; }
    public OnFoeBeforeMoveEventInfo? OnFoeBeforeMove { get; init; }
    public OnFoeBeforeSwitchInEventInfo? OnFoeBeforeSwitchIn { get; init; }
    public OnFoeBeforeSwitchOutEventInfo? OnFoeBeforeSwitchOut { get; init; }
    public OnFoeTryBoostEventInfo? OnFoeTryBoost { get; init; }
    public OnFoeChargeMoveEventInfo? OnFoeChargeMove { get; init; }
    public OnFoeCriticalHitEventInfo? OnFoeCriticalHit { get; init; }
    public OnFoeDamageEventInfo? OnFoeDamage { get; init; }
    public OnFoeDeductPpEventInfo? OnFoeDeductPp { get; init; }
    public OnFoeDisableMoveEventInfo? OnFoeDisableMove { get; init; }
    public OnFoeDragOutEventInfo? OnFoeDragOut { get; init; }
    public OnFoeEatItemEventInfo? OnFoeEatItem { get; init; }
    public OnFoeEffectivenessEventInfo? OnFoeEffectiveness { get; init; }
    public OnFoeFaintEventInfo? OnFoeFaint { get; init; }
    public OnFoeFlinchEventInfo? OnFoeFlinch { get; init; }
    public OnFoeHitEventInfo? OnFoeHit { get; init; }
    public OnFoeImmunityEventInfo? OnFoeImmunity { get; init; }
    public OnFoeLockMoveEventInfo? OnFoeLockMove { get; init; }
    public OnFoeMaybeTrapPokemonEventInfo? OnFoeMaybeTrapPokemon { get; init; }
    public OnFoeModifyAccuracyEventInfo? OnFoeModifyAccuracy { get; init; }
    public OnFoeModifyAtkEventInfo? OnFoeModifyAtk { get; init; }
    public OnFoeModifyBoostEventInfo? OnFoeModifyBoost { get; init; }
    public OnFoeModifyCritRatioEventInfo? OnFoeModifyCritRatio { get; init; }
    public OnFoeModifyDamageEventInfo? OnFoeModifyDamage { get; init; }
    public OnFoeModifyDefEventInfo? OnFoeModifyDef { get; init; }
    public OnFoeModifyMoveEventInfo? OnFoeModifyMove { get; init; }
    public OnFoeModifyPriorityEventInfo? OnFoeModifyPriority { get; init; }
    public OnFoeModifySecondariesEventInfo? OnFoeModifySecondaries { get; init; }
    public OnFoeModifySpAEventInfo? OnFoeModifySpA { get; init; }
    public OnFoeModifySpDEventInfo? OnFoeModifySpD { get; init; }
    public OnFoeModifySpeEventInfo? OnFoeModifySpe { get; init; }
    public OnFoeModifyStabEventInfo? OnFoeModifyStab { get; init; }
    public OnFoeModifyTypeEventInfo? OnFoeModifyType { get; init; }
    public OnFoeModifyTargetEventInfo? OnFoeModifyTarget { get; init; }
    public OnFoeModifyWeightEventInfo? OnFoeModifyWeight { get; init; }
    public OnFoeMoveAbortedEventInfo? OnFoeMoveAborted { get; init; }
    public OnFoeNegateImmunityEventInfo? OnFoeNegateImmunity { get; init; }
    public OnFoeOverrideActionEventInfo? OnFoeOverrideAction { get; init; }
    public OnFoePrepareHitEventInfo? OnFoePrepareHit { get; init; }
    public OnFoeRedirectTargetEventInfo? OnFoeRedirectTarget { get; init; }
    public OnFoeResidualEventInfo? OnFoeResidual { get; init; }
    public OnFoeSetAbilityEventInfo? OnFoeSetAbility { get; init; }
    public OnFoeSetStatusEventInfo? OnFoeSetStatus { get; init; }
    public OnFoeSetWeatherEventInfo? OnFoeSetWeather { get; init; }
    public OnFoeStallMoveEventInfo? OnFoeStallMove { get; init; }
    public OnFoeSwitchOutEventInfo? OnFoeSwitchOut { get; init; }
    public OnFoeTakeItemEventInfo? OnFoeTakeItem { get; init; }
    public OnFoeTerrainEventInfo? OnFoeTerrain { get; init; }
    public OnFoeTrapPokemonEventInfo? OnFoeTrapPokemon { get; init; }
    public OnFoeTryAddVolatileEventInfo? OnFoeTryAddVolatile { get; init; }
    public OnFoeTryEatItemEventInfo? OnFoeTryEatItem { get; init; }
    public OnFoeTryHealEventInfo? OnFoeTryHeal { get; init; }
    public OnFoeTryHitEventInfo? OnFoeTryHit { get; init; }
    public OnFoeTryHitFieldEventInfo? OnFoeTryHitField { get; init; }
    public OnFoeTryHitSideEventInfo? OnFoeTryHitSide { get; init; }
    public OnFoeInvulnerabilityEventInfo? OnFoeInvulnerability { get; init; }
    public OnFoeTryMoveEventInfo? OnFoeTryMove { get; init; }
    public OnFoeTryPrimaryHitEventInfo? OnFoeTryPrimaryHit { get; init; }
    public OnFoeTypeEventInfo? OnFoeType { get; init; }
    public OnFoeWeatherModifyDamageEventInfo? OnFoeWeatherModifyDamage { get; init; }
    public OnFoeModifyDamagePhase1EventInfo? OnFoeModifyDamagePhase1 { get; init; }
    public OnFoeModifyDamagePhase2EventInfo? OnFoeModifyDamagePhase2 { get; init; }
    public OnSourceDamagingHitEventInfo? OnSourceDamagingHit { get; init; }
    public OnSourceAfterEachBoostEventInfo? OnSourceAfterEachBoost { get; init; }
    public OnSourceAfterHitEventInfo? OnSourceAfterHit { get; init; }
    public OnSourceAfterSetStatusEventInfo? OnSourceAfterSetStatus { get; init; }
    public OnSourceAfterSubDamageEventInfo? OnSourceAfterSubDamage { get; init; }
    public OnSourceAfterSwitchInSelfEventInfo? OnSourceAfterSwitchInSelf { get; init; }
    public OnSourceAfterUseItemEventInfo? OnSourceAfterUseItem { get; init; }
    public OnSourceAfterBoostEventInfo? OnSourceAfterBoost { get; init; }
    public OnSourceAfterFaintEventInfo? OnSourceAfterFaint { get; init; }
    public OnSourceAfterMoveSecondarySelfEventInfo? OnSourceAfterMoveSecondarySelf { get; init; }
    public OnSourceAfterMoveSecondaryEventInfo? OnSourceAfterMoveSecondary { get; init; }
    public OnSourceAfterMoveEventInfo? OnSourceAfterMove { get; init; }
    public OnSourceAfterMoveSelfEventInfo? OnSourceAfterMoveSelf { get; init; }
    public OnSourceAttractEventInfo? OnSourceAttract { get; init; }
    public OnSourceAccuracyEventInfo? OnSourceAccuracy { get; init; }
    public OnSourceBasePowerEventInfo? OnSourceBasePower { get; init; }
    public OnSourceBeforeFaintEventInfo? OnSourceBeforeFaint { get; init; }
    public OnSourceBeforeMoveEventInfo? OnSourceBeforeMove { get; init; }
    public OnSourceBeforeSwitchInEventInfo? OnSourceBeforeSwitchIn { get; init; }
    public OnSourceBeforeSwitchOutEventInfo? OnSourceBeforeSwitchOut { get; init; }
    public OnSourceTryBoostEventInfo? OnSourceTryBoost { get; init; }
    public OnSourceChargeMoveEventInfo? OnSourceChargeMove { get; init; }
    public OnSourceCriticalHitEventInfo? OnSourceCriticalHit { get; init; }
    public OnSourceDamageEventInfo? OnSourceDamage { get; init; }
    public OnSourceDeductPpEventInfo? OnSourceDeductPp { get; init; }
    public OnSourceDisableMoveEventInfo? OnSourceDisableMove { get; init; }
    public OnSourceDragOutEventInfo? OnSourceDragOut { get; init; }
    public OnSourceEatItemEventInfo? OnSourceEatItem { get; init; }
    public OnSourceEffectivenessEventInfo? OnSourceEffectiveness { get; init; }
    public OnSourceFaintEventInfo? OnSourceFaint { get; init; }
    public OnSourceFlinchEventInfo? OnSourceFlinch { get; init; }
    public OnSourceHitEventInfo? OnSourceHit { get; init; }
    public OnSourceImmunityEventInfo? OnSourceImmunity { get; init; }
    public OnSourceLockMoveEventInfo? OnSourceLockMove { get; init; }
    public OnSourceMaybeTrapPokemonEventInfo? OnSourceMaybeTrapPokemon { get; init; }
    public OnSourceModifyAccuracyEventInfo? OnSourceModifyAccuracy { get; init; }
    public OnSourceModifyAtkEventInfo? OnSourceModifyAtk { get; init; }
    public OnSourceModifyBoostEventInfo? OnSourceModifyBoost { get; init; }
    public OnSourceModifyCritRatioEventInfo? OnSourceModifyCritRatio { get; init; }
    public OnSourceModifyDamageEventInfo? OnSourceModifyDamage { get; init; }
    public OnSourceModifyDefEventInfo? OnSourceModifyDef { get; init; }
    public OnSourceModifyMoveEventInfo? OnSourceModifyMove { get; init; }
    public OnSourceModifyPriorityEventInfo? OnSourceModifyPriority { get; init; }
    public OnSourceModifySecondariesEventInfo? OnSourceModifySecondaries { get; init; }
    public OnSourceModifySpAEventInfo? OnSourceModifySpA { get; init; }
    public OnSourceModifySpDEventInfo? OnSourceModifySpD { get; init; }
    public OnSourceModifySpeEventInfo? OnSourceModifySpe { get; init; }
    public OnSourceModifyStabEventInfo? OnSourceModifyStab { get; init; }
    public OnSourceModifyTypeEventInfo? OnSourceModifyType { get; init; }
    public OnSourceModifyTargetEventInfo? OnSourceModifyTarget { get; init; }
    public OnSourceModifyWeightEventInfo? OnSourceModifyWeight { get; init; }
    public OnSourceMoveAbortedEventInfo? OnSourceMoveAborted { get; init; }
    public OnSourceNegateImmunityEventInfo? OnSourceNegateImmunity { get; init; }
    public OnSourceOverrideActionEventInfo? OnSourceOverrideAction { get; init; }
    public OnSourcePrepareHitEventInfo? OnSourcePrepareHit { get; init; }
    public OnSourceRedirectTargetEventInfo? OnSourceRedirectTarget { get; init; }
    public OnSourceResidualEventInfo? OnSourceResidual { get; init; }
    public OnSourceSetAbilityEventInfo? OnSourceSetAbility { get; init; }
    public OnSourceSetStatusEventInfo? OnSourceSetStatus { get; init; }
    public OnSourceSetWeatherEventInfo? OnSourceSetWeather { get; init; }
    public OnSourceStallMoveEventInfo? OnSourceStallMove { get; init; }
    public OnSourceSwitchOutEventInfo? OnSourceSwitchOut { get; init; }
    public OnSourceTakeItemEventInfo? OnSourceTakeItem { get; init; }
    public OnSourceTerrainEventInfo? OnSourceTerrain { get; init; }
    public OnSourceTrapPokemonEventInfo? OnSourceTrapPokemon { get; init; }
    public OnSourceTryAddVolatileEventInfo? OnSourceTryAddVolatile { get; init; }
    public OnSourceTryEatItemEventInfo? OnSourceTryEatItem { get; init; }
    public OnSourceTryHealEventInfo? OnSourceTryHeal { get; init; }
    public OnSourceTryHitEventInfo? OnSourceTryHit { get; init; }
    public OnSourceTryHitFieldEventInfo? OnSourceTryHitField { get; init; }
    public OnSourceTryHitSideEventInfo? OnSourceTryHitSide { get; init; }
    public OnSourceInvulnerabilityEventInfo? OnSourceInvulnerability { get; init; }
    public OnSourceTryMoveEventInfo? OnSourceTryMove { get; init; }
    public OnSourceTryPrimaryHitEventInfo? OnSourceTryPrimaryHit { get; init; }
    public OnSourceTypeEventInfo? OnSourceType { get; init; }
    public OnSourceWeatherModifyDamageEventInfo? OnSourceWeatherModifyDamage { get; init; }
    public OnSourceModifyDamagePhase1EventInfo? OnSourceModifyDamagePhase1 { get; init; }
    public OnSourceModifyDamagePhase2EventInfo? OnSourceModifyDamagePhase2 { get; init; }
    public OnAnyDamagingHitEventInfo? OnAnyDamagingHit { get; init; }
    public OnAnyAfterEachBoostEventInfo? OnAnyAfterEachBoost { get; init; }
    public OnAnyAfterHitEventInfo? OnAnyAfterHit { get; init; }
    public OnAnyAfterSetStatusEventInfo? OnAnyAfterSetStatus { get; init; }
    public OnAnyAfterSubDamageEventInfo? OnAnyAfterSubDamage { get; init; }
    public OnAnyAfterSwitchInSelfEventInfo? OnAnyAfterSwitchInSelf { get; init; }
    public OnAnyAfterUseItemEventInfo? OnAnyAfterUseItem { get; init; }
    public OnAnyAfterBoostEventInfo? OnAnyAfterBoost { get; init; }
    public OnAnyAfterFaintEventInfo? OnAnyAfterFaint { get; init; }
    public OnAnyAfterMegaEventInfo? OnAnyAfterMega { get; init; }
    public OnAnyAfterMoveSecondarySelfEventInfo? OnAnyAfterMoveSecondarySelf { get; init; }
    public OnAnyAfterMoveSecondaryEventInfo? OnAnyAfterMoveSecondary { get; init; }
    public OnAnyAfterMoveEventInfo? OnAnyAfterMove { get; init; }
    public OnAnyAfterMoveSelfEventInfo? OnAnyAfterMoveSelf { get; init; }
    public OnAnyAfterTerastallizationEventInfo? OnAnyAfterTerastallization { get; init; }
    public OnAnyAttractEventInfo? OnAnyAttract { get; init; }
    public OnAnyAccuracyEventInfo? OnAnyAccuracy { get; init; }
    public OnAnyBasePowerEventInfo? OnAnyBasePower { get; init; }
    public OnAnyBeforeFaintEventInfo? OnAnyBeforeFaint { get; init; }
    public OnAnyBeforeMoveEventInfo? OnAnyBeforeMove { get; init; }
    public OnAnyBeforeSwitchInEventInfo? OnAnyBeforeSwitchIn { get; init; }
    public OnAnyBeforeSwitchOutEventInfo? OnAnyBeforeSwitchOut { get; init; }
    public OnAnyTryBoostEventInfo? OnAnyTryBoost { get; init; }
    public OnAnyChargeMoveEventInfo? OnAnyChargeMove { get; init; }
    public OnAnyCriticalHitEventInfo? OnAnyCriticalHit { get; init; }
    public OnAnyDamageEventInfo? OnAnyDamage { get; init; }
    public OnAnyDeductPpEventInfo? OnAnyDeductPp { get; init; }
    public OnAnyDisableMoveEventInfo? OnAnyDisableMove { get; init; }
    public OnAnyDragOutEventInfo? OnAnyDragOut { get; init; }
    public OnAnyEatItemEventInfo? OnAnyEatItem { get; init; }
    public OnAnyEffectivenessEventInfo? OnAnyEffectiveness { get; init; }
    public OnAnyFaintEventInfo? OnAnyFaint { get; init; }
    public OnAnyFlinchEventInfo? OnAnyFlinch { get; init; }
    public OnAnyHitEventInfo? OnAnyHit { get; init; }
    public OnAnyImmunityEventInfo? OnAnyImmunity { get; init; }
    public OnAnyLockMoveEventInfo? OnAnyLockMove { get; init; }
    public OnAnyMaybeTrapPokemonEventInfo? OnAnyMaybeTrapPokemon { get; init; }
    public OnAnyModifyAccuracyEventInfo? OnAnyModifyAccuracy { get; init; }
    public OnAnyModifyAtkEventInfo? OnAnyModifyAtk { get; init; }
    public OnAnyModifyBoostEventInfo? OnAnyModifyBoost { get; init; }
    public OnAnyModifyCritRatioEventInfo? OnAnyModifyCritRatio { get; init; }
    public OnAnyModifyDamageEventInfo? OnAnyModifyDamage { get; init; }
    public OnAnyModifyDefEventInfo? OnAnyModifyDef { get; init; }
    public OnAnyModifyMoveEventInfo? OnAnyModifyMove { get; init; }
    public OnAnyModifyPriorityEventInfo? OnAnyModifyPriority { get; init; }
    public OnAnyModifySecondariesEventInfo? OnAnyModifySecondaries { get; init; }
    public OnAnyModifySpAEventInfo? OnAnyModifySpA { get; init; }
    public OnAnyModifySpDEventInfo? OnAnyModifySpD { get; init; }
    public OnAnyModifySpeEventInfo? OnAnyModifySpe { get; init; }
    public OnAnyModifyStabEventInfo? OnAnyModifyStab { get; init; }
    public OnAnyModifyTypeEventInfo? OnAnyModifyType { get; init; }
    public OnAnyModifyTargetEventInfo? OnAnyModifyTarget { get; init; }
    public OnAnyModifyWeightEventInfo? OnAnyModifyWeight { get; init; }
    public OnAnyMoveAbortedEventInfo? OnAnyMoveAborted { get; init; }
    public OnAnyNegateImmunityEventInfo? OnAnyNegateImmunity { get; init; }
    public OnAnyOverrideActionEventInfo? OnAnyOverrideAction { get; init; }
    public OnAnyPrepareHitEventInfo? OnAnyPrepareHit { get; init; }
    public OnAnyPseudoWeatherChangeEventInfo? OnAnyPseudoWeatherChange { get; init; }
    public OnAnyRedirectTargetEventInfo? OnAnyRedirectTarget { get; init; }
    public OnAnyResidualEventInfo? OnAnyResidual { get; init; }
    public OnAnySetAbilityEventInfo? OnAnySetAbility { get; init; }
    public OnAnySetStatusEventInfo? OnAnySetStatus { get; init; }
    public OnAnySetWeatherEventInfo? OnAnySetWeather { get; init; }
    public OnAnyStallMoveEventInfo? OnAnyStallMove { get; init; }
    public OnAnySwitchInEventInfo? OnAnySwitchIn { get; init; }
    public OnAnySwitchOutEventInfo? OnAnySwitchOut { get; init; }
    public OnAnyTakeItemEventInfo? OnAnyTakeItem { get; init; }
    public OnAnyTerrainEventInfo? OnAnyTerrain { get; init; }
    public OnAnyTrapPokemonEventInfo? OnAnyTrapPokemon { get; init; }
    public OnAnyTryAddVolatileEventInfo? OnAnyTryAddVolatile { get; init; }
    public OnAnyTryEatItemEventInfo? OnAnyTryEatItem { get; init; }
    public OnAnyTryHealEventInfo? OnAnyTryHeal { get; init; }
    public OnAnyTryHitEventInfo? OnAnyTryHit { get; init; }
    public OnAnyTryHitFieldEventInfo? OnAnyTryHitField { get; init; }
    public OnAnyTryHitSideEventInfo? OnAnyTryHitSide { get; init; }
    public OnAnyInvulnerabilityEventInfo? OnAnyInvulnerability { get; init; }
    public OnAnyTryMoveEventInfo? OnAnyTryMove { get; init; }
    public OnAnyTryPrimaryHitEventInfo? OnAnyTryPrimaryHit { get; init; }
    public OnAnyTypeEventInfo? OnAnyType { get; init; }
    public OnAnyWeatherModifyDamageEventInfo? OnAnyWeatherModifyDamage { get; init; }
    public OnAnyModifyDamagePhase1EventInfo? OnAnyModifyDamagePhase1 { get; init; }
    public OnAnyModifyDamagePhase2EventInfo? OnAnyModifyDamagePhase2 { get; init; }

    //public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnDamagingHit { get; init; }
    //public Action<Battle, Pokemon>? OnEmergencyExit { get; init; }
    //public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterEachBoost { get; init; }
    //public VoidSourceMoveHandler? OnAfterHit { get; init; }
    //public Action<Battle, Pokemon>? OnAfterMega { get; init; }
    //public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAfterSetStatus { get; init; }
    //public OnAfterSubDamageHandler? OnAfterSubDamage { get; init; }
    //public Action<Battle, Pokemon>? OnAfterSwitchInSelf { get; init; }
    //public Action<Battle, Pokemon>? OnAfterTerastallization { get; init; }
    //public Action<Battle, Item, Pokemon>? OnAfterUseItem { get; init; }
    //public Action<Battle, Item, Pokemon>? OnAfterTakeItem { get; init; }
    //public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterBoost { get; init; }
    //public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAfterFaint { get; init; }
    //public VoidSourceMoveHandler? OnAfterMoveSecondarySelf { get; init; }
    //public VoidMoveHandler? OnAfterMoveSecondary { get; init; }
    //public VoidSourceMoveHandler? OnAfterMove { get; init; }
    //public VoidSourceMoveHandler? OnAfterMoveSelf { get; init; }
    //public Action<Battle, Pokemon, Pokemon>? OnAttract { get; init; }
    //public Func<Battle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnAccuracy { get; init; }
    //public ModifierSourceMoveHandler? OnBasePower { get; init; }
    //public Action<Battle, Pokemon, IEffect>? OnBeforeFaint { get; init; }
    //public VoidSourceMoveHandler? OnBeforeMove { get; init; }
    //public Action<Battle, Pokemon>? OnBeforeSwitchIn { get; init; }
    //public Action<Battle, Pokemon>? OnBeforeSwitchOut { get; init; }
    //public Action<Battle, Pokemon>? OnBeforeTurn { get; init; }
    //public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnChangeBoost { get; init; }
    //public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnTryBoost { get; init; }
    //public VoidSourceMoveHandler? OnChargeMove { get; init; }
    //public OnCriticalHit? OnCriticalHit { get; init; }
    //public Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? OnDamage { get; init; }
    //public Func<Battle, Pokemon, Pokemon, IntVoidUnion>? OnDeductPp { get; init; }
    //public Action<Battle, Pokemon>? OnDisableMove { get; init; }
    //public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnDragOut { get; init; }
    //public Action<Battle, Item, Pokemon>? OnEatItem { get; init; }
    //public OnEffectivenessHandler? OnEffectiveness { get; init; }
    //public Action<Battle, Pokemon>? OnEntryHazard { get; init; }
    //public VoidEffectHandler? OnFaint { get; init; }
    //public OnFlinch? OnFlinch { get; init; }
    //public OnFractionalPriority? OnFractionalPriority { get; init; }
    //public ResultMoveHandler? OnHit { get; init; }
    //public Action<Battle, PokemonType, Pokemon>? OnImmunity { get; init; }
    //public OnLockMove? OnLockMove { get; init; }
    //public Action<Battle, Pokemon>? OnMaybeTrapPokemon { get; init; }
    //public ModifierMoveHandler? OnModifyAccuracy { get; init; }
    //public ModifierSourceMoveHandler? OnModifyAtk { get; init; }
    //public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>? OnModifyBoost { get; init; }
    //public ModifierSourceMoveHandler? OnModifyCritRatio { get; init; }
    //public ModifierSourceMoveHandler? OnModifyDamage { get; init; }
    //public ModifierMoveHandler? OnModifyDef { get; init; }
    //public OnModifyMoveHandler? OnModifyMove { get; init; }
    //public ModifierSourceMoveHandler? OnModifyPriority { get; init; }
    //public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnModifySecondaries { get; init; }
    //public OnModifyTypeHandler? OnModifyType { get; init; }
    //public OnModifyTargetHandler? OnModifyTarget { get; init; }
    //public ModifierSourceMoveHandler? OnModifySpA { get; init; }
    //public ModifierMoveHandler? OnModifySpD { get; init; }
    //public Func<Battle, int, Pokemon, IntVoidUnion>? OnModifySpe { get; init; }
    //public ModifierSourceMoveHandler? OnModifyStab { get; init; }
    //public Func<Battle, int, Pokemon, IntVoidUnion>? OnModifyWeight { get; init; }
    //public VoidMoveHandler? OnMoveAborted { get; init; }
    //public OnNegateImmunity? OnNegateImmunity { get; init; }
    //public Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>? OnOverrideAction { get; init; }
    //public ResultSourceMoveHandler? OnPrepareHit { get; init; }
    //public Action<Battle, Pokemon, Pokemon, Condition>? OnPseudoWeatherChange { get; init; }
    //public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>? OnRedirectTarget { get; init; }
    //public Action<Battle, Pokemon, Pokemon, IEffect>? OnResidual { get; init; }
    //public Action<Battle, Ability, Pokemon, Pokemon, IEffect>? OnSetAbility { get; init; }
    //public Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnSetStatus { get; init; }
    //public Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion>? OnSetWeather { get; init; }
    //public Action<Battle, Side, Pokemon, Condition>? OnSideConditionStart { get; init; }
    //public Func<Battle, Pokemon, BoolVoidUnion>? OnStallMove { get; init; }
    //public Action<Battle, Pokemon>? OnSwitchIn { get; init; }
    //public Action<Battle, Pokemon>? OnSwitchOut { get; init; }
    //public Action<Battle, Pokemon, Pokemon>? OnSwap { get; init; }
    //public OnTakeItem? OnTakeItem { get; init; }
    //public Action<Battle, Pokemon, Pokemon, IEffect>? OnWeatherChange { get; init; }
    //public Action<Battle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; init; }
    //public Action<Battle, Pokemon>? OnTrapPokemon { get; init; }
    //public Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnTryAddVolatile { get; init; }
    //public OnTryEatItem? OnTryEatItem { get; init; }
    //public OnTryHeal? OnTryHeal { get; init; }
    //public ExtResultSourceMoveHandler? OnTryHit { get; init; }
    //public ResultMoveHandler? OnTryHitField { get; init; }
    //public ResultMoveHandler? OnTryHitSide { get; init; }
    //public ExtResultMoveHandler? OnInvulnerability { get; init; }
    //public ResultSourceMoveHandler? OnTryMove { get; init; }
    //public Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnTryPrimaryHit { get; init; }
    //public Func<Battle, PokemonType[], Pokemon, TypesVoidUnion>? OnType { get; init; }
    //public Action<Battle, Item, Pokemon>? OnUseItem { get; init; }
    //public Action<Battle, Pokemon>? OnUpdate { get; init; }
    //public Action<Battle, Pokemon, object?, Condition>? OnWeather { get; init; }
    //public ModifierSourceMoveHandler? OnWeatherModifyDamage { get; init; }
    //public ModifierSourceMoveHandler? OnModifyDamagePhase1 { get; init; }
    //public ModifierSourceMoveHandler? OnModifyDamagePhase2 { get; init; }
    //public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeDamagingHit { get; init; }
    //public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnFoeAfterEachBoost { get; init; }
    //public VoidSourceMoveHandler? OnFoeAfterHit { get; init; }
    //public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnFoeAfterSetStatus { get; init; }
    //public OnAfterSubDamageHandler? OnFoeAfterSubDamage { get; init; }
    //public Action<Battle, Pokemon>? OnFoeAfterSwitchInSelf { get; init; }
    //public Action<Battle, Item, Pokemon>? OnFoeAfterUseItem { get; init; }
    //public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeAfterBoost { get; init; }
    //public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnFoeAfterFaint { get; init; }
    //public VoidSourceMoveHandler? OnFoeAfterMoveSecondarySelf { get; init; }
    //public VoidMoveHandler? OnFoeAfterMoveSecondary { get; init; }
    //public VoidSourceMoveHandler? OnFoeAfterMove { get; init; }
    //public VoidSourceMoveHandler? OnFoeAfterMoveSelf { get; init; }
    //public Action<Battle, Pokemon, Pokemon>? OnFoeAttract { get; init; }
    //public Func<Battle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnFoeAccuracy { get; init; }
    //public ModifierSourceMoveHandler? OnFoeBasePower { get; init; }
    //public Action<Battle, Pokemon, IEffect>? OnFoeBeforeFaint { get; init; }
    //public VoidSourceMoveHandler? OnFoeBeforeMove { get; init; }
    //public Action<Battle, Pokemon>? OnFoeBeforeSwitchIn { get; init; }
    //public Action<Battle, Pokemon>? OnFoeBeforeSwitchOut { get; init; }
    //public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get; init; }
    //public VoidSourceMoveHandler? OnFoeChargeMove { get; init; }
    //public OnCriticalHit? OnFoeCriticalHit { get; init; }
    //public Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? OnFoeDamage { get; init; }
    //public Func<Battle, Pokemon, Pokemon, IntVoidUnion>? OnFoeDeductPp { get; init; }
    //public Action<Battle, Pokemon>? OnFoeDisableMove { get; init; }
    //public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnFoeDragOut { get; init; }
    //public Action<Battle, Item, Pokemon>? OnFoeEatItem { get; init; }
    //public OnEffectivenessHandler? OnFoeEffectiveness { get; init; }
    //public VoidEffectHandler? OnFoeFaint { get; init; }
    //public OnFlinch? OnFoeFlinch { get; init; }
    //public ResultMoveHandler? OnFoeHit { get; init; }
    //public Action<Battle, PokemonType, Pokemon>? OnFoeImmunity { get; init; }
    //public OnLockMove? OnFoeLockMove { get; init; }
    //public Action<Battle, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get; init; }
    //public ModifierMoveHandler? OnFoeModifyAccuracy { get; init; }
    //public ModifierSourceMoveHandler? OnFoeModifyAtk { get; init; }
    //public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>? OnFoeModifyBoost { get; init; }
    //public ModifierSourceMoveHandler? OnFoeModifyCritRatio { get; init; }
    //public ModifierSourceMoveHandler? OnFoeModifyDamage { get; init; }
    //public ModifierMoveHandler? OnFoeModifyDef { get; init; }
    //public OnModifyMoveHandler? OnFoeModifyMove { get; init; }
    //public ModifierSourceMoveHandler? OnFoeModifyPriority { get; init; }
    //public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnFoeModifySecondaries { get; init; }
    //public ModifierSourceMoveHandler? OnFoeModifySpA { get; init; }
    //public ModifierMoveHandler? OnFoeModifySpD { get; init; }
    //public Func<Battle, int, Pokemon, IntVoidUnion>? OnFoeModifySpe { get; init; }
    //public ModifierSourceMoveHandler? OnFoeModifyStab { get; init; }
    //public OnModifyTypeHandler? OnFoeModifyType { get; init; }
    //public OnModifyTargetHandler? OnFoeModifyTarget { get; init; }
    //public Func<Battle, int, Pokemon, IntVoidUnion>? OnFoeModifyWeight { get; init; }
    //public VoidMoveHandler? OnFoeMoveAborted { get; init; }
    //public OnNegateImmunity? OnFoeNegateImmunity { get; init; }
    //public Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>? OnFoeOverrideAction { get; init; }
    //public ResultSourceMoveHandler? OnFoePrepareHit { get; init; }
    //public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>? OnFoeRedirectTarget { get; init; }
    //public Action<Battle, PokemonSideUnion, Pokemon, IEffect>? OnFoeResidual { get; init; }
    //public Func<Battle, Ability, Pokemon, Pokemon, IEffect, BoolVoidUnion>? OnFoeSetAbility { get; init; }
    //public Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnFoeSetStatus { get; init; }
    //public Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion>? OnFoeSetWeather { get; init; }
    //public Func<Battle, Pokemon, BoolVoidUnion>? OnFoeStallMove { get; init; }
    //public Action<Battle, Pokemon>? OnFoeSwitchOut { get; init; }
    //public OnTakeItem? OnFoeTakeItem { get; init; }
    //public Action<Battle, Pokemon>? OnFoeTerrain { get; init; }
    //public Action<Battle, Pokemon>? OnFoeTrapPokemon { get; init; }
    //public Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnFoeTryAddVolatile { get; init; }
    //public OnTryEatItem? OnFoeTryEatItem { get; init; }
    //public OnTryHeal? OnFoeTryHeal { get; init; }
    //public ExtResultSourceMoveHandler? OnFoeTryHit { get; init; }
    //public ResultMoveHandler? OnFoeTryHitField { get; init; }
    //public ResultMoveHandler? OnFoeTryHitSide { get; init; }
    //public ExtResultMoveHandler? OnFoeInvulnerability { get; init; }
    //public ResultSourceMoveHandler? OnFoeTryMove { get; init; }
    //public Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnFoeTryPrimaryHit { get; init; }
    //public Func<Battle, PokemonType[], Pokemon, TypesVoidUnion>? OnFoeType { get; init; }
    //public ModifierSourceMoveHandler? OnFoeWeatherModifyDamage { get; init; }
    //public ModifierSourceMoveHandler? OnFoeModifyDamagePhase1 { get; init; }
    //public ModifierSourceMoveHandler? OnFoeModifyDamagePhase2 { get; init; }
    //public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceDamagingHit { get; init; }
    //public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnSourceAfterEachBoost { get; init; }
    //public VoidSourceMoveHandler? OnSourceAfterHit { get; init; }
    //public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnSourceAfterSetStatus { get; init; }
    //public OnAfterSubDamageHandler? OnSourceAfterSubDamage { get; init; }
    //public Action<Battle, Pokemon>? OnSourceAfterSwitchInSelf { get; init; }
    //public Action<Battle, Item, Pokemon>? OnSourceAfterUseItem { get; init; }
    //public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceAfterBoost { get; init; }
    //public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; init; }
    //public VoidSourceMoveHandler? OnSourceAfterMoveSecondarySelf { get; init; }
    //public VoidMoveHandler? OnSourceAfterMoveSecondary { get; init; }
    //public VoidSourceMoveHandler? OnSourceAfterMove { get; init; }
    //public VoidSourceMoveHandler? OnSourceAfterMoveSelf { get; init; }
    //public Action<Battle, Pokemon, Pokemon>? OnSourceAttract { get; init; }
    //public Func<Battle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnSourceAccuracy { get; init; }
    //public ModifierSourceMoveHandler? OnSourceBasePower { get; init; }
    //public Action<Battle, Pokemon, IEffect>? OnSourceBeforeFaint { get; init; }
    //public VoidSourceMoveHandler? OnSourceBeforeMove { get; init; }
    //public Action<Battle, Pokemon>? OnSourceBeforeSwitchIn { get; init; }
    //public Action<Battle, Pokemon>? OnSourceBeforeSwitchOut { get; init; }
    //public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get; init; }
    //public VoidSourceMoveHandler? OnSourceChargeMove { get; init; }
    //public OnCriticalHit? OnSourceCriticalHit { get; init; }
    //public Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? OnSourceDamage { get; init; }
    //public Func<Battle, Pokemon, Pokemon, IntVoidUnion>? OnSourceDeductPp { get; init; }
    //public Action<Battle, Pokemon>? OnSourceDisableMove { get; init; }
    //public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnSourceDragOut { get; init; }
    //public Action<Battle, Item, Pokemon>? OnSourceEatItem { get; init; }
    //public OnEffectivenessHandler? OnSourceEffectiveness { get; init; }
    //public VoidEffectHandler? OnSourceFaint { get; init; }
    //public OnFlinch? OnSourceFlinch { get; init; }
    //public ResultMoveHandler? OnSourceHit { get; init; }
    //public Action<Battle, PokemonType, Pokemon>? OnSourceImmunity { get; init; }
    //public OnLockMove? OnSourceLockMove { get; init; }
    //public Action<Battle, Pokemon>? OnSourceMaybeTrapPokemon { get; init; }
    //public ModifierMoveHandler? OnSourceModifyAccuracy { get; init; }
    //public ModifierSourceMoveHandler? OnSourceModifyAtk { get; init; }
    //public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>? OnSourceModifyBoost { get; init; }
    //public ModifierSourceMoveHandler? OnSourceModifyCritRatio { get; init; }
    //public ModifierSourceMoveHandler? OnSourceModifyDamage { get; init; }
    //public ModifierMoveHandler? OnSourceModifyDef { get; init; }
    //public OnModifyMoveHandler? OnSourceModifyMove { get; init; }
    //public ModifierSourceMoveHandler? OnSourceModifyPriority { get; init; }
    //public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnSourceModifySecondaries { get; init; }
    //public ModifierSourceMoveHandler? OnSourceModifySpA { get; init; }
    //public ModifierMoveHandler? OnSourceModifySpD { get; init; }
    //public Func<Battle, int, Pokemon, IntVoidUnion>? OnSourceModifySpe { get; init; }
    //public ModifierSourceMoveHandler? OnSourceModifyStab { get; init; }
    //public OnModifyTypeHandler? OnSourceModifyType { get; init; }
    //public OnModifyTargetHandler? OnSourceModifyTarget { get; init; }
    //public Func<Battle, int, Pokemon, IntVoidUnion>? OnSourceModifyWeight { get; init; }
    //public VoidMoveHandler? OnSourceMoveAborted { get; init; }
    //public OnNegateImmunity? OnSourceNegateImmunity { get; init; }
    //public Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>? OnSourceOverrideAction { get; init; }
    //public ResultSourceMoveHandler? OnSourcePrepareHit { get; init; }
    //public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>? OnSourceRedirectTarget { get; init; }
    //public Action<Battle, PokemonSideUnion, Pokemon, IEffect>? OnSourceResidual { get; init; }
    //public Func<Battle, Ability, Pokemon, Pokemon, IEffect, BoolVoidUnion>? OnSourceSetAbility { get; init; }
    //public Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnSourceSetStatus { get; init; }
    //public Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion>? OnSourceSetWeather { get; init; }
    //public Func<Battle, Pokemon, BoolVoidUnion>? OnSourceStallMove { get; init; }
    //public Action<Battle, Pokemon>? OnSourceSwitchOut { get; init; }
    //public OnTakeItem? OnSourceTakeItem { get; init; }
    //public Action<Battle, Pokemon>? OnSourceTerrain { get; init; }
    //public Action<Battle, Pokemon>? OnSourceTrapPokemon { get; init; }
    //public Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnSourceTryAddVolatile { get; init; }
    //public OnTryEatItem? OnSourceTryEatItem { get; init; }
    //public OnTryHeal? OnSourceTryHeal { get; init; }
    //public ExtResultSourceMoveHandler? OnSourceTryHit { get; init; }
    //public ResultMoveHandler? OnSourceTryHitField { get; init; }
    //public ResultMoveHandler? OnSourceTryHitSide { get; init; }
    //public ExtResultMoveHandler? OnSourceInvulnerability { get; init; }
    //public ResultSourceMoveHandler? OnSourceTryMove { get; init; }
    //public Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnSourceTryPrimaryHit { get; init; }
    //public Func<Battle, PokemonType[], Pokemon, TypesVoidUnion>? OnSourceType { get; init; }
    //public ModifierSourceMoveHandler? OnSourceWeatherModifyDamage { get; init; }
    //public ModifierSourceMoveHandler? OnSourceModifyDamagePhase1 { get; init; }
    //public ModifierSourceMoveHandler? OnSourceModifyDamagePhase2 { get; init; }
    //public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyDamagingHit { get; init; }
    //public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAnyAfterEachBoost { get; init; }
    //public VoidSourceMoveHandler? OnAnyAfterHit { get; init; }
    //public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAnyAfterSetStatus { get; init; }
    //public OnAfterSubDamageHandler? OnAnyAfterSubDamage { get; init; }
    //public Action<Battle, Pokemon>? OnAnyAfterSwitchInSelf { get; init; }
    //public Action<Battle, Item, Pokemon>? OnAnyAfterUseItem { get; init; }
    //public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyAfterBoost { get; init; }
    //public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAnyAfterFaint { get; init; }
    //public Action<Battle, Pokemon>? OnAnyAfterMega { get; init; }
    //public VoidSourceMoveHandler? OnAnyAfterMoveSecondarySelf { get; init; }
    //public VoidMoveHandler? OnAnyAfterMoveSecondary { get; init; }
    //public VoidSourceMoveHandler? OnAnyAfterMove { get; init; }
    //public VoidSourceMoveHandler? OnAnyAfterMoveSelf { get; init; }
    //public Action<Battle, Pokemon>? OnAnyAfterTerastallization { get; init; }
    //public Action<Battle, Pokemon, Pokemon>? OnAnyAttract { get; init; }
    //public Func<Battle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnAnyAccuracy { get; init; }
    //public ModifierSourceMoveHandler? OnAnyBasePower { get; init; }
    //public Action<Battle, Pokemon, IEffect>? OnAnyBeforeFaint { get; init; }
    //public VoidSourceMoveHandler? OnAnyBeforeMove { get; init; }
    //public Action<Battle, Pokemon>? OnAnyBeforeSwitchIn { get; init; }
    //public Action<Battle, Pokemon>? OnAnyBeforeSwitchOut { get; init; }
    //public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get; init; }
    //public VoidSourceMoveHandler? OnAnyChargeMove { get; init; }
    //public OnCriticalHit? OnAnyCriticalHit { get; init; }
    //public Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? OnAnyDamage { get; init; }
    //public Func<Battle, Pokemon, Pokemon, IntVoidUnion>? OnAnyDeductPp { get; init; }
    //public Action<Battle, Pokemon>? OnAnyDisableMove { get; init; }
    //public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAnyDragOut { get; init; }
    //public Action<Battle, Item, Pokemon>? OnAnyEatItem { get; init; }
    //public OnEffectivenessHandler? OnAnyEffectiveness { get; init; }
    //public VoidEffectHandler? OnAnyFaint { get; init; }
    //public OnFlinch? OnAnyFlinch { get; init; }
    //public ResultMoveHandler? OnAnyHit { get; init; }
    //public Action<Battle, PokemonType, Pokemon>? OnAnyImmunity { get; init; }
    //public OnLockMove? OnAnyLockMove { get; init; }
    //public Action<Battle, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get; init; }
    //public ModifierMoveHandler? OnAnyModifyAccuracy { get; init; }
    //public ModifierSourceMoveHandler? OnAnyModifyAtk { get; init; }
    //public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>? OnAnyModifyBoost { get; init; }
    //public ModifierSourceMoveHandler? OnAnyModifyCritRatio { get; init; }
    //public ModifierSourceMoveHandler? OnAnyModifyDamage { get; init; }
    //public ModifierMoveHandler? OnAnyModifyDef { get; init; }
    //public OnModifyMoveHandler? OnAnyModifyMove { get; init; }
    //public ModifierSourceMoveHandler? OnAnyModifyPriority { get; init; }
    //public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAnyModifySecondaries { get; init; }
    //public ModifierSourceMoveHandler? OnAnyModifySpA { get; init; }
    //public ModifierMoveHandler? OnAnyModifySpD { get; init; }
    //public Func<Battle, int, Pokemon, IntVoidUnion>? OnAnyModifySpe { get; init; }
    //public ModifierSourceMoveHandler? OnAnyModifyStab { get; init; }
    //public OnModifyTypeHandler? OnAnyModifyType { get; init; }
    //public OnModifyTargetHandler? OnAnyModifyTarget { get; init; }
    //public Func<Battle, int, Pokemon, IntVoidUnion>? OnAnyModifyWeight { get; init; }
    //public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyMoveAborted { get; init; }
    //public OnNegateImmunity? OnAnyNegateImmunity { get; init; }
    //public Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>? OnAnyOverrideAction { get; init; }
    //public ResultSourceMoveHandler? OnAnyPrepareHit { get; init; }
    //public Action<Battle, Pokemon, Pokemon, Condition>? OnAnyPseudoWeatherChange { get; init; }
    //public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>? OnAnyRedirectTarget { get; init; }
    //public Action<Battle, PokemonSideUnion, Pokemon, IEffect>? OnAnyResidual { get; init; }
    //public Func<Battle, Ability, Pokemon, Pokemon, IEffect, bool?>? OnAnySetAbility { get; init; }
    //public Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion>? OnAnySetStatus { get; init; }
    //public Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion>? OnAnySetWeather { get; init; }
    //public Func<Battle, Pokemon, BoolVoidUnion>? OnAnyStallMove { get; init; }
    //public Action<Battle, Pokemon>? OnAnySwitchIn { get; init; }
    //public Action<Battle, Pokemon>? OnAnySwitchOut { get; init; }
    //public OnTakeItem? OnAnyTakeItem { get; init; }
    //public Action<Battle, Pokemon>? OnAnyTerrain { get; init; }
    //public Action<Battle, Pokemon>? OnAnyTrapPokemon { get; init; }
    //public Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnAnyTryAddVolatile { get; init; }
    //public OnTryEatItem? OnAnyTryEatItem { get; init; }
    //public OnTryHeal? OnAnyTryHeal { get; init; }
    //public ExtResultSourceMoveHandler? OnAnyTryHit { get; init; }
    //public ResultMoveHandler? OnAnyTryHitField { get; init; }
    //public ResultMoveHandler? OnAnyTryHitSide { get; init; }
    //public ExtResultMoveHandler? OnAnyInvulnerability { get; init; }
    //public ResultSourceMoveHandler? OnAnyTryMove { get; init; }
    //public Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnAnyTryPrimaryHit { get; init; }
    //public Func<Battle, PokemonType[], Pokemon, TypesVoidUnion>? OnAnyType { get; init; }
    //public ModifierSourceMoveHandler? OnAnyWeatherModifyDamage { get; init; }
    //public ModifierSourceMoveHandler? OnAnyModifyDamagePhase1 { get; init; }
    //public ModifierSourceMoveHandler? OnAnyModifyDamagePhase2 { get; init; }
    //public int? OnAccuracyPriority { get; init; }
    //public int? OnDamagingHitOrder { get; init; }
    //public int? OnAfterMoveSecondaryPriority { get; init; }
    //public int? OnAfterMoveSecondarySelfPriority { get; init; }
    //public int? OnAfterMoveSelfPriority { get; init; }
    //public int? OnAfterSetStatusPriority { get; init; }
    //public int? OnAnyBasePowerPriority { get; init; }
    //public int? OnAnyInvulnerabilityPriority { get; init; }
    //public int? OnAnyModifyAccuracyPriority { get; init; }
    //public int? OnAnyFaintPriority { get; init; }
    //public int? OnAnyPrepareHitPriority { get; init; }
    //public int? OnAnySwitchInPriority { get; init; }
    //public int? OnAnySwitchInSubOrder { get; init; }
    //public int? OnAllyBasePowerPriority { get; init; }
    //public int? OnAllyModifyAtkPriority { get; init; }
    //public int? OnAllyModifySpAPriority { get; init; }
    //public int? OnAllyModifySpDPriority { get; init; }
    //public int? OnAttractPriority { get; init; }
    //public int? OnBasePowerPriority { get; init; }
    //public int? OnBeforeMovePriority { get; init; }
    //public int? OnBeforeSwitchOutPriority { get; init; }
    //public int? OnChangeBoostPriority { get; init; }
    //public int? OnDamagePriority { get; init; }
    //public int? OnDragOutPriority { get; init; }
    //public int? OnEffectivenessPriority { get; init; }
    //public int? OnFoeBasePowerPriority { get; init; }
    //public int? OnFoeBeforeMovePriority { get; init; }
    //public int? OnFoeModifyDefPriority { get; init; }
    //public int? OnFoeModifySpDPriority { get; init; }
    //public int? OnFoeRedirectTargetPriority { get; init; }
    //public int? OnFoeTrapPokemonPriority { get; init; }
    //public int? OnFractionalPriorityPriority { get; init; }
    //public int? OnHitPriority { get; init; }
    //public int? OnInvulnerabilityPriority { get; init; }
    //public int? OnModifyAccuracyPriority { get; init; }
    //public int? OnModifyAtkPriority { get; init; }
    //public int? OnModifyCritRatioPriority { get; init; }
    //public int? OnModifyDefPriority { get; init; }
    //public int? OnModifyMovePriority { get; init; }
    //public int? OnModifyPriorityPriority { get; init; }
    //public int? OnModifySpAPriority { get; init; }
    //public int? OnModifySpDPriority { get; init; }
    //public int? OnModifySpePriority { get; init; }
    //public int? OnModifyStabPriority { get; init; }
    //public int? OnModifyTypePriority { get; init; }
    //public int? OnModifyWeightPriority { get; init; }
    //public int? OnRedirectTargetPriority { get; init; }
    //public int? OnResidualOrder { get; init; }
    //public int? OnResidualPriority { get; init; }
    //public int? OnResidualSubOrder { get; init; }
    //public int? OnSourceBasePowerPriority { get; init; }
    //public int? OnSourceInvulnerabilityPriority { get; init; }
    //public int? OnSourceModifyAccuracyPriority { get; init; }
    //public int? OnSourceModifyAtkPriority { get; init; }
    //public int? OnSourceModifyDamagePriority { get; init; }
    //public int? OnSourceModifySpAPriority { get; init; }
    //public int? OnSwitchInPriority { get; init; }
    //public int? OnSwitchInSubOrder { get; init; }
    //public int? OnTrapPokemonPriority { get; init; }
    //public int? OnTryBoostPriority { get; init; }
    //public int? OnTryEatItemPriority { get; init; }
    //public int? OnTryHealPriority { get; init; }
    //public int? OnTryHitPriority { get; init; }
    //public int? OnTryMovePriority { get; init; }
    //public int? OnTryPrimaryHitPriority { get; init; }
    //public int? OnTypePriority { get; init; }

    #endregion

    //public EffectDelegate? GetDelegate(EventId id)
    //{
    //    return id switch
    //    {
    //        EventId.DamagingHit => EffectDelegate.FromNullableDelegate(OnDamagingHit),
    //        EventId.EmergencyExit => EffectDelegate.FromNullableDelegate(OnEmergencyExit),
    //        EventId.AfterEachBoost => EffectDelegate.FromNullableDelegate(OnAfterEachBoost),
    //        EventId.AfterHit => EffectDelegate.FromNullableDelegate(OnAfterHit),
    //        EventId.AfterMega => EffectDelegate.FromNullableDelegate(OnAfterMega),
    //        EventId.AfterSetStatus => EffectDelegate.FromNullableDelegate(OnAfterSetStatus),
    //        EventId.AfterSubDamage => EffectDelegate.FromNullableDelegate(OnAfterSubDamage),
    //        EventId.AfterSwitchInSelf => EffectDelegate.FromNullableDelegate(OnAfterSwitchInSelf),
    //        EventId.AfterTerastallization => EffectDelegate.FromNullableDelegate(OnAfterTerastallization),
    //        EventId.AfterUseItem => EffectDelegate.FromNullableDelegate(OnAfterUseItem),
    //        EventId.AfterTakeItem => EffectDelegate.FromNullableDelegate(OnAfterTakeItem),
    //        EventId.AfterBoost => EffectDelegate.FromNullableDelegate(OnAfterBoost),
    //        EventId.AfterFaint => EffectDelegate.FromNullableDelegate(OnAfterFaint),
    //        EventId.AfterMoveSecondarySelf => EffectDelegate.FromNullableDelegate(OnAfterMoveSecondarySelf),
    //        EventId.AfterMoveSecondary => EffectDelegate.FromNullableDelegate(OnAfterMoveSecondary),
    //        EventId.AfterMove => EffectDelegate.FromNullableDelegate(OnAfterMove),
    //        EventId.AfterMoveSelf => EffectDelegate.FromNullableDelegate(OnAfterMoveSelf),
    //        EventId.Attract => EffectDelegate.FromNullableDelegate(OnAttract),
    //        EventId.Accuracy => EffectDelegate.FromNullableDelegate(OnAccuracy),
    //        EventId.BasePower => EffectDelegate.FromNullableDelegate(OnBasePower),
    //        EventId.BeforeFaint => EffectDelegate.FromNullableDelegate(OnBeforeFaint),
    //        EventId.BeforeMove => EffectDelegate.FromNullableDelegate(OnBeforeMove),
    //        EventId.BeforeSwitchIn => EffectDelegate.FromNullableDelegate(OnBeforeSwitchIn),
    //        EventId.BeforeSwitchOut => EffectDelegate.FromNullableDelegate(OnBeforeSwitchOut),
    //        EventId.BeforeTurn => EffectDelegate.FromNullableDelegate(OnBeforeTurn),
    //        EventId.ChangeBoost => EffectDelegate.FromNullableDelegate(OnChangeBoost),
    //        EventId.TryBoost => EffectDelegate.FromNullableDelegate(OnTryBoost),
    //        EventId.ChargeMove => EffectDelegate.FromNullableDelegate(OnChargeMove),
    //        EventId.CriticalHit => EffectDelegate.FromNullableOnCriticalHit(OnCriticalHit),
    //        EventId.Damage => EffectDelegate.FromNullableDelegate(OnDamage),
    //        EventId.DeductPp => EffectDelegate.FromNullableDelegate(OnDeductPp),
    //        EventId.DisableMove => EffectDelegate.FromNullableDelegate(OnDisableMove),
    //        EventId.DragOut => EffectDelegate.FromNullableDelegate(OnDragOut),
    //        EventId.EatItem => EffectDelegate.FromNullableDelegate(OnEatItem),
    //        EventId.Effectiveness => EffectDelegate.FromNullableDelegate(OnEffectiveness),
    //        EventId.EntryHazard => EffectDelegate.FromNullableDelegate(OnEntryHazard),
    //        EventId.Faint => EffectDelegate.FromNullableDelegate(OnFaint),
    //        EventId.Flinch => EffectDelegate.FromNullableOnFlinch(OnFlinch),
    //        EventId.FractionalPriority => EffectDelegate.FromNullableOnFractionalPriority(OnFractionalPriority),
    //        EventId.Hit => EffectDelegate.FromNullableDelegate(OnHit),
    //        EventId.Immunity => EffectDelegate.FromNullableDelegate(OnImmunity),
    //        EventId.LockMove => EffectDelegate.FromNullableOnLockMove(OnLockMove),
    //        EventId.MaybeTrapPokemon => EffectDelegate.FromNullableDelegate(OnMaybeTrapPokemon),
    //        EventId.ModifyAccuracy => EffectDelegate.FromNullableDelegate(OnModifyAccuracy),
    //        EventId.ModifyAtk => EffectDelegate.FromNullableDelegate(OnModifyAtk),
    //        EventId.ModifyBoost => EffectDelegate.FromNullableDelegate(OnModifyBoost),
    //        EventId.ModifyCritRatio => EffectDelegate.FromNullableDelegate(OnModifyCritRatio),
    //        EventId.ModifyDamage => EffectDelegate.FromNullableDelegate(OnModifyDamage),
    //        EventId.ModifyDef => EffectDelegate.FromNullableDelegate(OnModifyDef),
    //        EventId.ModifyMove => EffectDelegate.FromNullableDelegate(OnModifyMove),
    //        EventId.ModifyPriority => EffectDelegate.FromNullableDelegate(OnModifyPriority),
    //        EventId.ModifySecondaries => EffectDelegate.FromNullableDelegate(OnModifySecondaries),
    //        EventId.ModifyType => EffectDelegate.FromNullableDelegate(OnModifyType),
    //        EventId.ModifyTarget => EffectDelegate.FromNullableDelegate(OnModifyTarget),
    //        EventId.ModifySpA => EffectDelegate.FromNullableDelegate(OnModifySpA),
    //        EventId.ModifySpD => EffectDelegate.FromNullableDelegate(OnModifySpD),
    //        EventId.ModifySpe => EffectDelegate.FromNullableDelegate(OnModifySpe),
    //        EventId.ModifyStab => EffectDelegate.FromNullableDelegate(OnModifyStab),
    //        EventId.ModifyWeight => EffectDelegate.FromNullableDelegate(OnModifyWeight),
    //        EventId.MoveAborted => EffectDelegate.FromNullableDelegate(OnMoveAborted),
    //        EventId.NegateImmunity => EffectDelegate.FromNullableOnNegateImmunity(OnNegateImmunity),
    //        EventId.OverrideAction => EffectDelegate.FromNullableDelegate(OnOverrideAction),
    //        EventId.PrepareHit => EffectDelegate.FromNullableDelegate(OnPrepareHit),
    //        EventId.PseudoWeatherChange => EffectDelegate.FromNullableDelegate(OnPseudoWeatherChange),
    //        EventId.RedirectTarget => EffectDelegate.FromNullableDelegate(OnRedirectTarget),
    //        EventId.Residual => EffectDelegate.FromNullableDelegate(OnResidual),
    //        EventId.SetAbility => EffectDelegate.FromNullableDelegate(OnSetAbility),
    //        EventId.SetStatus => EffectDelegate.FromNullableDelegate(OnSetStatus),
    //        EventId.SetWeather => EffectDelegate.FromNullableDelegate(OnSetWeather),
    //        EventId.SideConditionStart => EffectDelegate.FromNullableDelegate(OnSideConditionStart),
    //        EventId.StallMove => EffectDelegate.FromNullableDelegate(OnStallMove),
    //        EventId.SwitchIn => EffectDelegate.FromNullableDelegate(OnSwitchIn),
    //        EventId.SwitchOut => EffectDelegate.FromNullableDelegate(OnSwitchOut),
    //        EventId.Swap => EffectDelegate.FromNullableDelegate(OnSwap),
    //        EventId.TakeItem => EffectDelegate.FromNullableOnTakeItem(OnTakeItem),
    //        EventId.TerrainChange => EffectDelegate.FromNullableDelegate(OnTerrainChange),
    //        EventId.TrapPokemon => EffectDelegate.FromNullableDelegate(OnTrapPokemon),
    //        EventId.TryAddVolatile => EffectDelegate.FromNullableDelegate(OnTryAddVolatile),
    //        EventId.TryEatItem => EffectDelegate.FromNullableOnTryEatItem(OnTryEatItem),
    //        EventId.TryHeal => EffectDelegate.FromNullableOnTryHeal(OnTryHeal),
    //        EventId.TryHit => EffectDelegate.FromNullableDelegate(OnTryHit),
    //        EventId.TryHitField => EffectDelegate.FromNullableDelegate(OnTryHitField),
    //        EventId.TryHitSide => EffectDelegate.FromNullableDelegate(OnTryHitSide),
    //        EventId.Invulnerability => EffectDelegate.FromNullableDelegate(OnInvulnerability),
    //        EventId.TryMove => EffectDelegate.FromNullableDelegate(OnTryMove),
    //        EventId.TryPrimaryHit => EffectDelegate.FromNullableDelegate(OnTryPrimaryHit),
    //        EventId.Type => EffectDelegate.FromNullableDelegate(OnType),
    //        EventId.UseItem => EffectDelegate.FromNullableDelegate(OnUseItem),
    //        EventId.Update => EffectDelegate.FromNullableDelegate(OnUpdate),
    //        EventId.Weather => EffectDelegate.FromNullableDelegate(OnWeather),
    //        EventId.WeatherModifyDamage => EffectDelegate.FromNullableDelegate(OnWeatherModifyDamage),
    //        EventId.ModifyDamagePhase1 => EffectDelegate.FromNullableDelegate(OnModifyDamagePhase1),
    //        EventId.ModifyDamagePhase2 => EffectDelegate.FromNullableDelegate(OnModifyDamagePhase2),
    //        _ => null,
    //    };
    //}

    //public int? GetPriority(EventId id)
    //{
    //    return id switch
    //    {
    //        EventId.Accuracy => OnAccuracyPriority,
    //        EventId.DamagingHit => OnDamagingHitOrder,
    //        EventId.AfterMoveSecondary => OnAfterMoveSecondaryPriority,
    //        EventId.AfterMoveSecondarySelf => OnAfterMoveSecondarySelfPriority,
    //        EventId.AfterMoveSelf => OnAfterMoveSelfPriority,
    //        EventId.AfterSetStatus => OnAfterSetStatusPriority,
    //        EventId.AnyBasePower => OnAnyBasePowerPriority,
    //        EventId.AnyInvulnerability => OnAnyInvulnerabilityPriority,
    //        EventId.AnyModifyAccuracy => OnAnyModifyAccuracyPriority,
    //        EventId.AnyFaint => OnAnyFaintPriority,
    //        EventId.AnyPrepareHit => OnAnyPrepareHitPriority,
    //        EventId.AnySwitchIn => OnAnySwitchInPriority,
    //        EventId.AllyBasePower => OnAllyBasePowerPriority,
    //        EventId.AllyModifyAtk => OnAllyModifyAtkPriority,
    //        EventId.AllyModifySpA => OnAllyModifySpAPriority,
    //        EventId.AllyModifySpD => OnAllyModifySpDPriority,
    //        EventId.Attract => OnAttractPriority,
    //        EventId.BasePower => OnBasePowerPriority,
    //        EventId.BeforeMove => OnBeforeMovePriority,
    //        EventId.BeforeSwitchOut => OnBeforeSwitchOutPriority,
    //        EventId.ChangeBoost => OnChangeBoostPriority,
    //        EventId.Damage => OnDamagePriority,
    //        EventId.DragOut => OnDragOutPriority,
    //        EventId.Effectiveness => OnEffectivenessPriority,
    //        EventId.FoeBasePower => OnFoeBasePowerPriority,
    //        EventId.FoeBeforeMove => OnFoeBeforeMovePriority,
    //        EventId.FoeModifyDef => OnFoeModifyDefPriority,
    //        EventId.FoeModifySpD => OnFoeModifySpDPriority,
    //        EventId.FoeRedirectTarget => OnFoeRedirectTargetPriority,
    //        EventId.FoeTrapPokemon => OnFoeTrapPokemonPriority,
    //        EventId.FractionalPriority => OnFractionalPriorityPriority,
    //        EventId.Hit => OnHitPriority,
    //        EventId.Invulnerability => OnInvulnerabilityPriority,
    //        EventId.ModifyAccuracy => OnModifyAccuracyPriority,
    //        EventId.ModifyAtk => OnModifyAtkPriority,
    //        EventId.ModifyCritRatio => OnModifyCritRatioPriority,
    //        EventId.ModifyDef => OnModifyDefPriority,
    //        EventId.ModifyMove => OnModifyMovePriority,
    //        EventId.ModifyPriority => OnModifyPriorityPriority,
    //        EventId.ModifySpA => OnModifySpAPriority,
    //        EventId.ModifySpD => OnModifySpDPriority,
    //        EventId.ModifySpe => OnModifySpePriority,
    //        EventId.ModifyStab => OnModifyStabPriority,
    //        EventId.ModifyType => OnModifyTypePriority,
    //        EventId.ModifyWeight => OnModifyWeightPriority,
    //        EventId.RedirectTarget => OnRedirectTargetPriority,
    //        EventId.Residual => OnResidualPriority,
    //        EventId.SourceBasePower => OnSourceBasePowerPriority,
    //        EventId.SourceInvulnerability => OnSourceInvulnerabilityPriority,
    //        EventId.SourceModifyAccuracy => OnSourceModifyAccuracyPriority,
    //        EventId.SourceModifyAtk => OnSourceModifyAtkPriority,
    //        EventId.SourceModifyDamage => OnSourceModifyDamagePriority,
    //        EventId.SourceModifySpA => OnSourceModifySpAPriority,
    //        EventId.SwitchIn => OnSwitchInPriority,
    //        EventId.TrapPokemon => OnTrapPokemonPriority,
    //        EventId.TryBoost => OnTryBoostPriority,
    //        EventId.TryEatItem => OnTryEatItemPriority,
    //        EventId.TryHeal => OnTryHealPriority,
    //        EventId.TryHit => OnTryHitPriority,
    //        EventId.TryMove => OnTryMovePriority,
    //        EventId.TryPrimaryHit => OnTryPrimaryHitPriority,
    //        EventId.Type => OnTypePriority,
    //        _ => null,
    //    };
    //}

    //public IntFalseUnion? GetOrder(EventId id)
    //{
    //    int? order = id switch
    //    {
    //        EventId.DamagingHit => OnDamagingHitOrder,
    //        EventId.Residual => OnResidualOrder,
    //        _ => null,
    //    };
    //    return order.HasValue ? IntFalseUnion.FromInt(order.Value) : null;
    //}

    //public int? GetSubOrder(EventId id)
    //{
    //    return id switch
    //    {
    //        EventId.AnySwitchIn => OnAnySwitchInSubOrder,
    //        EventId.Residual => OnResidualSubOrder,
    //        EventId.SwitchIn => OnSwitchInSubOrder,
    //        _ => null,
    //    };
    //}

    /// <summary>
    /// Gets event handler information for the specified event (TODO: implement fully).
    /// </summary>
    public EventHandlerInfo? GetEventHandlerInfo(EventId id)
    {
        // TODO: Implement using EventHandlerInfoBuilder similar to Ability class
        // For now, return null as this hasn't been migrated yet
        return null;
    }

    /// <summary>
    /// Creates a copy of this Format for simulation purposes.
    /// This method creates an independent copy with the same state while sharing immutable references.
    /// </summary>
    /// <returns>A new Format instance with copied state</returns>
    public Format Copy()
    {
        // Since Format is a record, we can use the with expression to create a shallow copy
        // All delegate properties (event handlers) are immutable references and safe to share
        return this with { };
    }
}