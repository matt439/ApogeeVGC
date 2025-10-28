using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Unions;
using System.Text.Json.Serialization;

namespace ApogeeVGC.Sim.Conditions;

public record Condition : ISideEventMethods, IFieldEventMethods, IPokemonEventMethods, IEffect, IBasicEffect,
    ICopyable<Condition>, IIdentifiable
{
    public required ConditionId Id { get; init; }
    public EffectStateId EffectStateId => Id;
    public EffectType EffectType
    {
        get;
        init
        {
            if (value is not (EffectType.Condition or EffectType.Weather or EffectType.Status or EffectType.Terrain))
            {
                throw new ArgumentException("Condition EffectType must be Condition, Weather, Status, or Terrain.");
            }
            field = value;
        }
    }
    public string Name { get; init; } = string.Empty;
    public string FullName => Name;
    public string ShowdownId => Name;

    //public ConditionEffectType ConditionEffectType { get; init; }

    ///// <summary>
    ///// Many conditions are defined by a source effect, such as a move or ability.
    ///// This property tracks that source effect, if any.
    ///// </summary>
    //public IEffect? Source { get; init; }

    public AbilityId? AssociatedAbility { get; init; }
    public ItemId? AssociatedItem { get; init; }
    public MoveId? AssociatedMove { get; init; }
    public SpecieId? AssociatedSpecies { get; init; }

    public int? Duration { get; set; }
    public int? CounterMax { get; init; }
    public int? Counter { get; set; }

    public bool NoCopy { get; init; }

    public IReadOnlyList<PokemonType>? ImmuneTypes { get; init; }

    public bool AffectsFainted { get; init; }

    /// <summary>
    /// battle, target, source, effect -> number
    /// </summary>
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; init; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnCopy { get; init; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnEnd { get; init; }

    /// <summary>
    /// battle, target, source, sourceEffect -> boolean | null
    /// </summary>
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnRestart { get; init; }

    /// <summary>
    /// battle, target, source, sourceEffect -> boolean | null
    /// </summary>
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnStart { get; init; }


    [JsonIgnore]
 public Action<IBattle, int, Pokemon, Pokemon, ActiveMove>? OnDamagingHit { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnEmergencyExit { get; init; }
    [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterEachBoost { get; init; }
    [JsonIgnore]
public VoidSourceMoveHandler? OnAfterHit { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAfterMega { get; init; }
    [JsonIgnore]
    public Action<IBattle, Condition, Pokemon, Pokemon, IEffect>? OnAfterSetStatus { get; init; }
    [JsonIgnore]
    public OnAfterSubDamageHandler? OnAfterSubDamage { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAfterSwitchInSelf { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAfterTerastallization { get; init; }
    [JsonIgnore]
    public Action<IBattle, Item, Pokemon>? OnAfterUseItem { get; init; }
    [JsonIgnore]
    public Action<IBattle, Item, Pokemon>? OnAfterTakeItem { get; init; }
    [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterBoost { get; init; }
    [JsonIgnore]
    public Action<IBattle, int, Pokemon, Pokemon, IEffect>? OnAfterFaint { get; init; }
    [JsonIgnore]
  public VoidSourceMoveHandler? OnAfterMoveSecondarySelf { get; init; }
    [JsonIgnore]
    public VoidMoveHandler? OnAfterMoveSecondary { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnAfterMove { get; init; }
    [JsonIgnore]
 public VoidSourceMoveHandler? OnAfterMoveSelf { get; init; }
    [JsonIgnore]
  public Action<IBattle, Pokemon, Pokemon>? OnAttract { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnAccuracy { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnBasePower { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, IEffect>? OnBeforeFaint { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnBeforeMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnBeforeSwitchIn { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnBeforeSwitchOut { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnBeforeTurn { get; init; }
    [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnChangeBoost { get; init; }
    [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnTryBoost { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnChargeMove { get; init; }
    [JsonIgnore]
    public OnCriticalHit? OnCriticalHit { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? OnDamage { get; init; }
  [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, IntVoidUnion>? OnDeductPp { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnDisableMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon?, ActiveMove?>? OnDragOut { get; init; }
    [JsonIgnore]
    public Action<IBattle, Item, Pokemon>? OnEatItem { get; init; }
    [JsonIgnore]
    public OnEffectivenessHandler? OnEffectiveness { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnEntryHazard { get; init; }
    [JsonIgnore]
    public VoidEffectHandler? OnFaint { get; init; }
    [JsonIgnore]
    public OnFlinch? OnFlinch { get; init; }
    [JsonIgnore]
    public OnFractionalPriority? OnFractionalPriority { get; init; }
    [JsonIgnore]
    public ResultMoveHandler? OnHit { get; init; }
    [JsonIgnore]
  public Action<IBattle, PokemonType, Pokemon>? OnImmunity { get; init; }
    [JsonIgnore]
    public OnLockMove? OnLockMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnMaybeTrapPokemon { get; init; }
    [JsonIgnore]
    public ModifierMoveHandler? OnModifyAccuracy { get; init; }
    [JsonIgnore]
  public ModifierSourceMoveHandler? OnModifyAtk { get; init; }
    [JsonIgnore]
    public Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>? OnModifyBoost { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnModifyCritRatio { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnModifyDamage { get; init; }
    [JsonIgnore]
    public ModifierMoveHandler? OnModifyDef { get; init; }
    [JsonIgnore]
    public OnModifyMoveHandler? OnModifyMove { get; init; }
    [JsonIgnore]
public ModifierSourceMoveHandler? OnModifyPriority { get; init; }
    [JsonIgnore]
    public Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnModifySecondaries { get; init; }
    [JsonIgnore]
    public OnModifyTypeHandler? OnModifyType { get; init; }
    [JsonIgnore]
    public OnModifyTargetHandler? OnModifyTarget { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnModifySpA { get; init; }
    [JsonIgnore]
    public ModifierMoveHandler? OnModifySpD { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnModifySpe { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnModifyStab { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnModifyWeight { get; init; }
    [JsonIgnore]
    public VoidMoveHandler? OnMoveAborted { get; init; }
 [JsonIgnore]
    public OnNegateImmunity? OnNegateImmunity { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>? OnOverrideAction { get; init; }
    [JsonIgnore]
    public ResultSourceMoveHandler? OnPrepareHit { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon, Condition>? OnPseudoWeatherChange { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>? OnRedirectTarget { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon, IEffect>? OnResidual { get; init; }
    [JsonIgnore]
    public Action<IBattle, Ability, Pokemon, Pokemon, IEffect>? OnSetAbility { get; init; }
    [JsonIgnore]
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnSetStatus { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, Condition, BoolVoidUnion>? OnSetWeather { get; init; }
    [JsonIgnore]
    public Action<IBattle, Side, Pokemon, Condition>? OnSideConditionStart { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, BoolVoidUnion>? OnStallMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnSwitchIn { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnSwitchOut { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon>? OnSwap { get; init; }
    [JsonIgnore]
    public OnTakeItem? OnTakeItem { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon, IEffect>? OnWeatherChange { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnTrapPokemon { get; init; }
    [JsonIgnore]
  public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnTryAddVolatile { get; init; }
  [JsonIgnore]
    public OnTryEatItem? OnTryEatItem { get; init; }
    [JsonIgnore]
    public OnTryHeal? OnTryHeal { get; init; }
    [JsonIgnore]
  public ExtResultSourceMoveHandler? OnTryHit { get; init; }
    [JsonIgnore]
 public ResultMoveHandler? OnTryHitField { get; init; }
 [JsonIgnore]
    public ResultMoveHandler? OnTryHitSide { get; init; }
    [JsonIgnore]
    public ExtResultMoveHandler? OnInvulnerability { get; init; }
    [JsonIgnore]
    public ResultSourceMoveHandler? OnTryMove { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnTryPrimaryHit { get; init; }
    [JsonIgnore]
    public Func<IBattle, PokemonType[], Pokemon, TypesVoidUnion>? OnType { get; init; }
    [JsonIgnore]
    public Action<IBattle, Item, Pokemon>? OnUseItem { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnUpdate { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, object?, Condition>? OnWeather { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnWeatherModifyDamage { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnModifyDamagePhase1 { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnModifyDamagePhase2 { get; init; }
    [JsonIgnore]
    public Action<IBattle, int, Pokemon, Pokemon, ActiveMove>? OnFoeDamagingHit { get; init; }
    [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon>? OnFoeAfterEachBoost { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnFoeAfterHit { get; init; }
    [JsonIgnore]
    public Action<IBattle, Condition, Pokemon, Pokemon, IEffect>? OnFoeAfterSetStatus { get; init; }
[JsonIgnore]
  public OnAfterSubDamageHandler? OnFoeAfterSubDamage { get; init; }
 [JsonIgnore]
    public Action<IBattle, Pokemon>? OnFoeAfterSwitchInSelf { get; init; }
    [JsonIgnore]
    public Action<IBattle, Item, Pokemon>? OnFoeAfterUseItem { get; init; }
 [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeAfterBoost { get; init; }
    [JsonIgnore]
    public Action<IBattle, int, Pokemon, Pokemon, IEffect>? OnFoeAfterFaint { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnFoeAfterMoveSecondarySelf { get; init; }
    [JsonIgnore]
    public VoidMoveHandler? OnFoeAfterMoveSecondary { get; init; }
 [JsonIgnore]
    public VoidSourceMoveHandler? OnFoeAfterMove { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnFoeAfterMoveSelf { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon>? OnFoeAttract { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnFoeAccuracy { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnFoeBasePower { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, IEffect>? OnFoeBeforeFaint { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnFoeBeforeMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnFoeBeforeSwitchIn { get; init; }
    [JsonIgnore]
 public Action<IBattle, Pokemon>? OnFoeBeforeSwitchOut { get; init; }
    [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnFoeChargeMove { get; init; }
    [JsonIgnore]
    public OnCriticalHit? OnFoeCriticalHit { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? OnFoeDamage { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, IntVoidUnion>? OnFoeDeductPp { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnFoeDisableMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon?, ActiveMove?>? OnFoeDragOut { get; init; }
    [JsonIgnore]
 public Action<IBattle, Item, Pokemon>? OnFoeEatItem { get; init; }
    [JsonIgnore]
    public OnEffectivenessHandler? OnFoeEffectiveness { get; init; }
    [JsonIgnore]
    public VoidEffectHandler? OnFoeFaint { get; init; }
    [JsonIgnore]
    public OnFlinch? OnFoeFlinch { get; init; }
 [JsonIgnore]
    public ResultMoveHandler? OnFoeHit { get; init; }
    [JsonIgnore]
  public Action<IBattle, PokemonType, Pokemon>? OnFoeImmunity { get; init; }
    [JsonIgnore]
public OnLockMove? OnFoeLockMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get; init; }
    [JsonIgnore]
    public ModifierMoveHandler? OnFoeModifyAccuracy { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnFoeModifyAtk { get; init; }
    [JsonIgnore]
    public Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>? OnFoeModifyBoost { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnFoeModifyCritRatio { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnFoeModifyDamage { get; init; }
 [JsonIgnore]
    public ModifierMoveHandler? OnFoeModifyDef { get; init; }
    [JsonIgnore]
    public OnModifyMoveHandler? OnFoeModifyMove { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnFoeModifyPriority { get; init; }
    [JsonIgnore]
    public Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnFoeModifySecondaries { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnFoeModifySpA { get; init; }
    [JsonIgnore]
    public ModifierMoveHandler? OnFoeModifySpD { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnFoeModifySpe { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnFoeModifyStab { get; init; }
    [JsonIgnore]
    public OnModifyTypeHandler? OnFoeModifyType { get; init; }
    [JsonIgnore]
    public OnModifyTargetHandler? OnFoeModifyTarget { get; init; }
 [JsonIgnore]
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnFoeModifyWeight { get; init; }
    [JsonIgnore]
    public VoidMoveHandler? OnFoeMoveAborted { get; init; }
    [JsonIgnore]
    public OnNegateImmunity? OnFoeNegateImmunity { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>? OnFoeOverrideAction { get; init; }
    [JsonIgnore]
    public ResultSourceMoveHandler? OnFoePrepareHit { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>? OnFoeRedirectTarget { get; init; }
    [JsonIgnore]
    public Action<IBattle, PokemonSideUnion, Pokemon, IEffect>? OnFoeResidual { get; init; }
    [JsonIgnore]
    public Func<IBattle, Ability, Pokemon, Pokemon, IEffect, BoolVoidUnion>? OnFoeSetAbility { get; init; }
    [JsonIgnore]
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnFoeSetStatus { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, Condition, BoolVoidUnion>? OnFoeSetWeather { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, BoolVoidUnion>? OnFoeStallMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnFoeSwitchOut { get; init; }
    [JsonIgnore]
    public OnTakeItem? OnFoeTakeItem { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnFoeTerrain { get; init; }
    [JsonIgnore]
 public Action<IBattle, Pokemon>? OnFoeTrapPokemon { get; init; }
    [JsonIgnore]
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnFoeTryAddVolatile { get; init; }
    [JsonIgnore]
    public OnTryEatItem? OnFoeTryEatItem { get; init; }
    [JsonIgnore]
    public OnTryHeal? OnFoeTryHeal { get; init; }
    [JsonIgnore]
    public ExtResultSourceMoveHandler? OnFoeTryHit { get; init; }
    [JsonIgnore]
    public ResultMoveHandler? OnFoeTryHitField { get; init; }
    [JsonIgnore]
    public ResultMoveHandler? OnFoeTryHitSide { get; init; }
    [JsonIgnore]
  public ExtResultMoveHandler? OnFoeInvulnerability { get; init; }
 [JsonIgnore]
    public ResultSourceMoveHandler? OnFoeTryMove { get; init; }
    [JsonIgnore]
 public Func<IBattle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnFoeTryPrimaryHit { get; init; }
    [JsonIgnore]
    public Func<IBattle, PokemonType[], Pokemon, TypesVoidUnion>? OnFoeType { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnFoeWeatherModifyDamage { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnFoeModifyDamagePhase1 { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnFoeModifyDamagePhase2 { get; init; }
    [JsonIgnore]
    public Action<IBattle, int, Pokemon, Pokemon, ActiveMove>? OnSourceDamagingHit { get; init; }
    [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon>? OnSourceAfterEachBoost { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnSourceAfterHit { get; init; }
    [JsonIgnore]
    public Action<IBattle, Condition, Pokemon, Pokemon, IEffect>? OnSourceAfterSetStatus { get; init; }
    [JsonIgnore]
    public OnAfterSubDamageHandler? OnSourceAfterSubDamage { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnSourceAfterSwitchInSelf { get; init; }
    [JsonIgnore]
    public Action<IBattle, Item, Pokemon>? OnSourceAfterUseItem { get; init; }
    [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceAfterBoost { get; init; }
    [JsonIgnore]
    public Action<IBattle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnSourceAfterMoveSecondarySelf { get; init; }
    [JsonIgnore]
    public VoidMoveHandler? OnSourceAfterMoveSecondary { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnSourceAfterMove { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnSourceAfterMoveSelf { get; init; }
    [JsonIgnore]
  public Action<IBattle, Pokemon, Pokemon>? OnSourceAttract { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnSourceAccuracy { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnSourceBasePower { get; init; }
    [JsonIgnore]
  public Action<IBattle, Pokemon, IEffect>? OnSourceBeforeFaint { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnSourceBeforeMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnSourceBeforeSwitchIn { get; init; }
 [JsonIgnore]
    public Action<IBattle, Pokemon>? OnSourceBeforeSwitchOut { get; init; }
    [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnSourceChargeMove { get; init; }
    [JsonIgnore]
    public OnCriticalHit? OnSourceCriticalHit { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? OnSourceDamage { get; init; }
    [JsonIgnore]
  public Func<IBattle, Pokemon, Pokemon, IntVoidUnion>? OnSourceDeductPp { get; init; }
    [JsonIgnore]
  public Action<IBattle, Pokemon>? OnSourceDisableMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon?, ActiveMove?>? OnSourceDragOut { get; init; }
    [JsonIgnore]
    public Action<IBattle, Item, Pokemon>? OnSourceEatItem { get; init; }
    [JsonIgnore]
    public OnEffectivenessHandler? OnSourceEffectiveness { get; init; }
 [JsonIgnore]
    public VoidEffectHandler? OnSourceFaint { get; init; }
    [JsonIgnore]
    public OnFlinch? OnSourceFlinch { get; init; }
    [JsonIgnore]
    public ResultMoveHandler? OnSourceHit { get; init; }
 [JsonIgnore]
    public Action<IBattle, PokemonType, Pokemon>? OnSourceImmunity { get; init; }
    [JsonIgnore]
    public OnLockMove? OnSourceLockMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnSourceMaybeTrapPokemon { get; init; }
    [JsonIgnore]
    public ModifierMoveHandler? OnSourceModifyAccuracy { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnSourceModifyAtk { get; init; }
    [JsonIgnore]
    public Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>? OnSourceModifyBoost { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnSourceModifyCritRatio { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnSourceModifyDamage { get; init; }
    [JsonIgnore]
    public ModifierMoveHandler? OnSourceModifyDef { get; init; }
    [JsonIgnore]
    public OnModifyMoveHandler? OnSourceModifyMove { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnSourceModifyPriority { get; init; }
    [JsonIgnore]
    public Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnSourceModifySecondaries { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnSourceModifySpA { get; init; }
    [JsonIgnore]
    public ModifierMoveHandler? OnSourceModifySpD { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnSourceModifySpe { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnSourceModifyStab { get; init; }
    [JsonIgnore]
    public OnModifyTypeHandler? OnSourceModifyType { get; init; }
    [JsonIgnore]
    public OnModifyTargetHandler? OnSourceModifyTarget { get; init; }
    [JsonIgnore]
  public Func<IBattle, int, Pokemon, IntVoidUnion>? OnSourceModifyWeight { get; init; }
    [JsonIgnore]
    public VoidMoveHandler? OnSourceMoveAborted { get; init; }
    [JsonIgnore]
    public OnNegateImmunity? OnSourceNegateImmunity { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>? OnSourceOverrideAction { get; init; }
    [JsonIgnore]
    public ResultSourceMoveHandler? OnSourcePrepareHit { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>? OnSourceRedirectTarget { get; init; }
    [JsonIgnore]
    public Action<IBattle, PokemonSideUnion, Pokemon, IEffect>? OnSourceResidual { get; init; }
    [JsonIgnore]
    public Func<IBattle, Ability, Pokemon, Pokemon, IEffect, BoolVoidUnion>? OnSourceSetAbility { get; init; }
    [JsonIgnore]
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnSourceSetStatus { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, Condition, BoolVoidUnion>? OnSourceSetWeather { get; init; }
    [JsonIgnore]
 public Func<IBattle, Pokemon, BoolVoidUnion>? OnSourceStallMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnSourceSwitchOut { get; init; }
    [JsonIgnore]
    public OnTakeItem? OnSourceTakeItem { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnSourceTerrain { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnSourceTrapPokemon { get; init; }
    [JsonIgnore]
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnSourceTryAddVolatile { get; init; }
    [JsonIgnore]
public OnTryEatItem? OnSourceTryEatItem { get; init; }
    [JsonIgnore]
    public OnTryHeal? OnSourceTryHeal { get; init; }
    [JsonIgnore]
    public ExtResultSourceMoveHandler? OnSourceTryHit { get; init; }
    [JsonIgnore]
    public ResultMoveHandler? OnSourceTryHitField { get; init; }
    [JsonIgnore]
    public ResultMoveHandler? OnSourceTryHitSide { get; init; }
    [JsonIgnore]
    public ExtResultMoveHandler? OnSourceInvulnerability { get; init; }
    [JsonIgnore]
    public ResultSourceMoveHandler? OnSourceTryMove { get; init; }
    [JsonIgnore]
 public Func<IBattle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnSourceTryPrimaryHit { get; init; }
[JsonIgnore]
    public Func<IBattle, PokemonType[], Pokemon, TypesVoidUnion>? OnSourceType { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnSourceWeatherModifyDamage { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnSourceModifyDamagePhase1 { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnSourceModifyDamagePhase2 { get; init; }
  [JsonIgnore]
    public Action<IBattle, int, Pokemon, Pokemon, ActiveMove>? OnAnyDamagingHit { get; init; }
    [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon>? OnAnyAfterEachBoost { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnAnyAfterHit { get; init; }
    [JsonIgnore]
    public Action<IBattle, Condition, Pokemon, Pokemon, IEffect>? OnAnyAfterSetStatus { get; init; }
    [JsonIgnore]
    public OnAfterSubDamageHandler? OnAnyAfterSubDamage { get; init; }
    [JsonIgnore]
  public Action<IBattle, Pokemon>? OnAnyAfterSwitchInSelf { get; init; }
    [JsonIgnore]
    public Action<IBattle, Item, Pokemon>? OnAnyAfterUseItem { get; init; }
    [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyAfterBoost { get; init; }
    [JsonIgnore]
    public Action<IBattle, int, Pokemon, Pokemon, IEffect>? OnAnyAfterFaint { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAnyAfterMega { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnAnyAfterMoveSecondarySelf { get; init; }
    [JsonIgnore]
    public VoidMoveHandler? OnAnyAfterMoveSecondary { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnAnyAfterMove { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnAnyAfterMoveSelf { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAnyAfterTerastallization { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon>? OnAnyAttract { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnAnyAccuracy { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAnyBasePower { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, IEffect>? OnAnyBeforeFaint { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnAnyBeforeMove { get; init; }
  [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAnyBeforeSwitchIn { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAnyBeforeSwitchOut { get; init; }
    [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnAnyChargeMove { get; init; }
    [JsonIgnore]
    public OnCriticalHit? OnAnyCriticalHit { get; init; }
    [JsonIgnore]
 public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? OnAnyDamage { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, IntVoidUnion>? OnAnyDeductPp { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAnyDisableMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon?, ActiveMove?>? OnAnyDragOut { get; init; }
    [JsonIgnore]
    public Action<IBattle, Item, Pokemon>? OnAnyEatItem { get; init; }
    [JsonIgnore]
    public OnEffectivenessHandler? OnAnyEffectiveness { get; init; }
    [JsonIgnore]
    public VoidEffectHandler? OnAnyFaint { get; init; }
    [JsonIgnore]
    public OnFlinch? OnAnyFlinch { get; init; }
    [JsonIgnore]
    public ResultMoveHandler? OnAnyHit { get; init; }
    [JsonIgnore]
  public Action<IBattle, PokemonType, Pokemon>? OnAnyImmunity { get; init; }
    [JsonIgnore]
    public OnLockMove? OnAnyLockMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get; init; }
    [JsonIgnore]
    public ModifierMoveHandler? OnAnyModifyAccuracy { get; init; }
 [JsonIgnore]
    public ModifierSourceMoveHandler? OnAnyModifyAtk { get; init; }
    [JsonIgnore]
    public Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>? OnAnyModifyBoost { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAnyModifyCritRatio { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAnyModifyDamage { get; init; }
    [JsonIgnore]
    public ModifierMoveHandler? OnAnyModifyDef { get; init; }
    [JsonIgnore]
    public OnModifyMoveHandler? OnAnyModifyMove { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAnyModifyPriority { get; init; }
[JsonIgnore]
    public Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAnyModifySecondaries { get; init; }
  [JsonIgnore]
    public ModifierSourceMoveHandler? OnAnyModifySpA { get; init; }
    [JsonIgnore]
 public ModifierMoveHandler? OnAnyModifySpD { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnAnyModifySpe { get; init; }
    [JsonIgnore]
  public ModifierSourceMoveHandler? OnAnyModifyStab { get; init; }
    [JsonIgnore]
    public OnModifyTypeHandler? OnAnyModifyType { get; init; }
    [JsonIgnore]
    public OnModifyTargetHandler? OnAnyModifyTarget { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnAnyModifyWeight { get; init; }
 [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon, ActiveMove>? OnAnyMoveAborted { get; init; }
    [JsonIgnore]
    public OnNegateImmunity? OnAnyNegateImmunity { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>? OnAnyOverrideAction { get; init; }
    [JsonIgnore]
    public ResultSourceMoveHandler? OnAnyPrepareHit { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon, Condition>? OnAnyPseudoWeatherChange { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>? OnAnyRedirectTarget { get; init; }
    [JsonIgnore]
    public Action<IBattle, PokemonSideUnion, Pokemon, IEffect>? OnAnyResidual { get; init; }
    [JsonIgnore]
    public Func<IBattle, Ability, Pokemon, Pokemon, IEffect, bool?>? OnAnySetAbility { get; init; }
    [JsonIgnore]
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion>? OnAnySetStatus { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, Condition, BoolVoidUnion>? OnAnySetWeather { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, BoolVoidUnion>? OnAnyStallMove { get; init; }
 [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAnySwitchIn { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAnySwitchOut { get; init; }
    [JsonIgnore]
  public OnTakeItem? OnAnyTakeItem { get; init; }
    [JsonIgnore]
  public Action<IBattle, Pokemon>? OnAnyTerrain { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAnyTrapPokemon { get; init; }
    [JsonIgnore]
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnAnyTryAddVolatile { get; init; }
    [JsonIgnore]
    public OnTryEatItem? OnAnyTryEatItem { get; init; }
    [JsonIgnore]
    public OnTryHeal? OnAnyTryHeal { get; init; }
  [JsonIgnore]
    public ExtResultSourceMoveHandler? OnAnyTryHit { get; init; }
    [JsonIgnore]
    public ResultMoveHandler? OnAnyTryHitField { get; init; }
    [JsonIgnore]
    public ResultMoveHandler? OnAnyTryHitSide { get; init; }
    [JsonIgnore]
    public ExtResultMoveHandler? OnAnyInvulnerability { get; init; }
    [JsonIgnore]
    public ResultSourceMoveHandler? OnAnyTryMove { get; init; }
[JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnAnyTryPrimaryHit { get; init; }
    [JsonIgnore]
    public Func<IBattle, PokemonType[], Pokemon, TypesVoidUnion>? OnAnyType { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAnyWeatherModifyDamage { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAnyModifyDamagePhase1 { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAnyModifyDamagePhase2 { get; init; }
    [JsonIgnore]
    public int? OnAccuracyPriority { get; init; }
    public int? OnDamagingHitOrder { get; init; }
    public int? OnAfterMoveSecondaryPriority { get; init; }
    public int? OnAfterMoveSecondarySelfPriority { get; init; }
    public int? OnAfterMoveSelfPriority { get; init; }
    public int? OnAfterSetStatusPriority { get; init; }
    public int? OnAnyBasePowerPriority { get; init; }
    public int? OnAnyInvulnerabilityPriority { get; init; }
    public int? OnAnyModifyAccuracyPriority { get; init; }
    public int? OnAnyFaintPriority { get; init; }
    public int? OnAnyPrepareHitPriority { get; init; }
    public int? OnAnySwitchInPriority { get; init; }
    public int? OnAnySwitchInSubOrder { get; init; }
    public int? OnAllyBasePowerPriority { get; init; }
    public int? OnAllyModifyAtkPriority { get; init; }
    public int? OnAllyModifySpAPriority { get; init; }
    public int? OnAllyModifySpDPriority { get; init; }
    public int? OnAttractPriority { get; init; }
    public int? OnBasePowerPriority { get; init; }
    public int? OnBeforeMovePriority { get; init; }
    public int? OnBeforeSwitchOutPriority { get; init; }
    public int? OnChangeBoostPriority { get; init; }
    public int? OnDamagePriority { get; init; }
    public int? OnDragOutPriority { get; init; }
    public int? OnEffectivenessPriority { get; init; }
    public int? OnFoeBasePowerPriority { get; init; }
    public int? OnFoeBeforeMovePriority { get; init; }
    public int? OnFoeModifyDefPriority { get; init; }
    public int? OnFoeModifySpDPriority { get; init; }
    public int? OnFoeRedirectTargetPriority { get; init; }
    public int? OnFoeTrapPokemonPriority { get; init; }
    public int? OnFractionalPriorityPriority { get; init; }
    public int? OnHitPriority { get; init; }
    public int? OnInvulnerabilityPriority { get; init; }
    public int? OnModifyAccuracyPriority { get; init; }
    public int? OnModifyAtkPriority { get; init; }
    public int? OnModifyCritRatioPriority { get; init; }
    public int? OnModifyDefPriority { get; init; }
    public int? OnModifyMovePriority { get; init; }
    public int? OnModifyPriorityPriority { get; init; }
    public int? OnModifySpAPriority { get; init; }
    public int? OnModifySpDPriority { get; init; }
    public int? OnModifySpePriority { get; init; }
    public int? OnModifyStabPriority { get; init; }
    public int? OnModifyTypePriority { get; init; }
    public int? OnModifyWeightPriority { get; init; }
    public int? OnRedirectTargetPriority { get; init; }
    public int? OnResidualOrder { get; init; }
    public int? OnResidualPriority { get; init; }
    public int? OnResidualSubOrder { get; init; }
    public int? OnSourceBasePowerPriority { get; init; }
    public int? OnSourceInvulnerabilityPriority { get; init; }
    public int? OnSourceModifyAccuracyPriority { get; init; }
    public int? OnSourceModifyAtkPriority { get; init; }
    public int? OnSourceModifyDamagePriority { get; init; }
    public int? OnSourceModifySpAPriority { get; init; }
    public int? OnSwitchInPriority { get; init; }
    public int? OnSwitchInSubOrder { get; init; }
    public int? OnTrapPokemonPriority { get; init; }
    public int? OnTryBoostPriority { get; init; }
    public int? OnTryEatItemPriority { get; init; }
    public int? OnTryHealPriority { get; init; }
    public int? OnTryHitPriority { get; init; }
    public int? OnTryMovePriority { get; init; }
    public int? OnTryPrimaryHitPriority { get; init; }
    public int? OnTypePriority { get; init; }
    [JsonIgnore]
    public Action<IBattle, Side, Pokemon, IEffect>? OnSideStart { get; init; }
    [JsonIgnore]
    public Action<IBattle, Side, Pokemon, IEffect>? OnSideRestart { get; init; }
    [JsonIgnore]
    public Action<IBattle, Side, Pokemon, IEffect>? OnSideResidual { get; init; }
    [JsonIgnore]
    public Action<IBattle, Side>? OnSideEnd { get; init; }
    public int? OnSideResidualOrder { get; init; }
    public int? OnSideResidualPriority { get; init; }
    public int? OnSideResidualSubOrder { get; init; }
    [JsonIgnore]
    public Action<IBattle, Field, Pokemon, IEffect>? OnFieldStart { get; init; }
    [JsonIgnore]
    public Action<IBattle, Field, Pokemon, IEffect>? OnFieldRestart { get; init; }
    [JsonIgnore]
    public Action<IBattle, Field, Pokemon, IEffect>? OnFieldResidual { get; init; }
    [JsonIgnore]
    public Action<IBattle, Field>? OnFieldEnd { get; init; }
    public int? OnFieldResidualOrder { get; init; }
    public int? OnFieldResidualPriority { get; init; }
    public int? OnFieldResidualSubOrder { get; init; }
    [JsonIgnore]
    public Action<IBattle, int, Pokemon, Pokemon, ActiveMove>? OnAllyDamagingHit { get; init; }
    [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon>? OnAllyAfterEachBoost { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnAllyAfterHit { get; init; }
    [JsonIgnore]
    public Action<IBattle, Condition, Pokemon, Pokemon, IEffect>? OnAllyAfterSetStatus { get; init; }
    [JsonIgnore]
    public OnAfterSubDamageHandler? OnAllyAfterSubDamage { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAllyAfterSwitchInSelf { get; init; }
    [JsonIgnore]
    public Action<IBattle, Item, Pokemon>? OnAllyAfterUseItem { get; init; }
    [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyAfterBoost { get; init; }
    [JsonIgnore]
    public Action<IBattle, int, Pokemon, Pokemon, IEffect>? OnAllyAfterFaint { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnAllyAfterMoveSecondarySelf { get; init; }
    [JsonIgnore]
    public VoidMoveHandler? OnAllyAfterMoveSecondary { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnAllyAfterMove { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnAllyAfterMoveSelf { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon>? OnAllyAttract { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnAllyAccuracy { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAllyBasePower { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, IEffect>? OnAllyBeforeFaint { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnAllyBeforeMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAllyBeforeSwitchIn { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAllyBeforeSwitchOut { get; init; }
    [JsonIgnore]
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyTryBoost { get; init; }
    [JsonIgnore]
    public VoidSourceMoveHandler? OnAllyChargeMove { get; init; }
    [JsonIgnore]
    public OnCriticalHit? OnAllyCriticalHit { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? OnAllyDamage { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, IntVoidUnion>? OnAllyDeductPp { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAllyDisableMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon, Pokemon?, ActiveMove?>? OnAllyDragOut { get; init; }
    [JsonIgnore]
    public Action<IBattle, Item, Pokemon>? OnAllyEatItem { get; init; }
    [JsonIgnore]
    public OnEffectivenessHandler? OnAllyEffectiveness { get; init; }
    [JsonIgnore]
    public VoidEffectHandler? OnAllyFaint { get; init; }
    [JsonIgnore]
    public OnFlinch? OnAllyFlinch { get; init; }
    [JsonIgnore]
    public ResultMoveHandler? OnAllyHit { get; init; }
    [JsonIgnore]
    public Action<IBattle, PokemonType, Pokemon>? OnAllyImmunity { get; init; }
    [JsonIgnore]
    public OnLockMove? OnAllyLockMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAllyMaybeTrapPokemon { get; init; }
    [JsonIgnore]
    public ModifierMoveHandler? OnAllyModifyAccuracy { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAllyModifyAtk { get; init; }
    [JsonIgnore]
    public Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>? OnAllyModifyBoost { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAllyModifyCritRatio { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAllyModifyDamage { get; init; }
    [JsonIgnore]
    public ModifierMoveHandler? OnAllyModifyDef { get; init; }
    [JsonIgnore]
    public OnModifyMoveHandler? OnAllyModifyMove { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAllyModifyPriority { get; init; }
    [JsonIgnore]
    public Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAllyModifySecondaries { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAllyModifySpA { get; init; }
    [JsonIgnore]
    public ModifierMoveHandler? OnAllyModifySpD { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnAllyModifySpe { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAllyModifyStab { get; init; }
    [JsonIgnore]
    public OnModifyTypeHandler? OnAllyModifyType { get; init; }
    [JsonIgnore]
    public OnModifyTargetHandler? OnAllyModifyTarget { get; init; }
    [JsonIgnore]
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnAllyModifyWeight { get; init; }
    [JsonIgnore]
    public VoidMoveHandler? OnAllyMoveAborted { get; init; }
    [JsonIgnore]
    public OnNegateImmunity? OnAllyNegateImmunity { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>? OnAllyOverrideAction { get; init; }
    [JsonIgnore]
    public ResultSourceMoveHandler? OnAllyPrepareHit { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>? OnAllyRedirectTarget { get; init; }
    [JsonIgnore]
    public Action<IBattle, PokemonSideUnion, Pokemon, IEffect>? OnAllyResidual { get; init; }
    [JsonIgnore]
    public Func<IBattle, Ability, Pokemon, Pokemon, IEffect, BoolVoidUnion>? OnAllySetAbility { get; init; }
    [JsonIgnore]
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, PokemonVoidUnion?>? OnAllySetStatus { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, Condition, PokemonVoidUnion>? OnAllySetWeather { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, PokemonVoidUnion>? OnAllyStallMove { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAllySwitchOut { get; init; }
    [JsonIgnore]
    public OnTakeItem? OnAllyTakeItem { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAllyTerrain { get; init; }
    [JsonIgnore]
    public Action<IBattle, Pokemon>? OnAllyTrapPokemon { get; init; }
    [JsonIgnore]
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnAllyTryAddVolatile { get; init; }
    [JsonIgnore]
    public Func<IBattle, Item, Pokemon, BoolVoidUnion>? OnAllyTryEatItem { get; init; }
    [JsonIgnore]
    public OnTryHeal? OnAllyTryHeal { get; init; }
    [JsonIgnore]
    public ExtResultSourceMoveHandler? OnAllyTryHit { get; init; }
    [JsonIgnore]
    public ExtResultSourceMoveHandler? OnAllyTryHitField { get; init; }
    [JsonIgnore]
    public ResultMoveHandler? OnAllyTryHitSide { get; init; }
    [JsonIgnore]
    public ExtResultMoveHandler? OnAllyInvulnerability { get; init; }
    [JsonIgnore]
    public ResultSourceMoveHandler? OnAllyTryMove { get; init; }
    [JsonIgnore]
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnAllyTryPrimaryHit { get; init; }
    [JsonIgnore]
    public Func<IBattle, PokemonType[], Pokemon, TypesVoidUnion>? OnAllyType { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAllyWeatherModifyDamage { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAllyModifyDamagePhase1 { get; init; }
    [JsonIgnore]
    public ModifierSourceMoveHandler? OnAllyModifyDamagePhase2 { get; init; }

    public EffectDelegate? GetDelegate(EventId id)
    {
        return id switch
        {
            EventId.End => EffectDelegate.FromNullableDelegate(OnEnd),
            EventId.Start => EffectDelegate.FromNullableDelegate(OnStart),
            EventId.Restart => EffectDelegate.FromNullableDelegate(OnRestart),
            EventId.DamagingHit => EffectDelegate.FromNullableDelegate(OnDamagingHit),
            EventId.EmergencyExit => EffectDelegate.FromNullableDelegate(OnEmergencyExit),
            EventId.AfterEachBoost => EffectDelegate.FromNullableDelegate(OnAfterEachBoost),
            EventId.AfterHit => EffectDelegate.FromNullableDelegate(OnAfterHit),
            EventId.AfterMega => EffectDelegate.FromNullableDelegate(OnAfterMega),
            EventId.AfterSetStatus => EffectDelegate.FromNullableDelegate(OnAfterSetStatus),
            EventId.AfterSubDamage => EffectDelegate.FromNullableDelegate(OnAfterSubDamage),
            EventId.AfterSwitchInSelf => EffectDelegate.FromNullableDelegate(OnAfterSwitchInSelf),
            EventId.AfterTerastallization => EffectDelegate.FromNullableDelegate(OnAfterTerastallization),
            EventId.AfterUseItem => EffectDelegate.FromNullableDelegate(OnAfterUseItem),
            EventId.AfterTakeItem => EffectDelegate.FromNullableDelegate(OnAfterTakeItem),
            EventId.AfterBoost => EffectDelegate.FromNullableDelegate(OnAfterBoost),
            EventId.AfterFaint => EffectDelegate.FromNullableDelegate(OnAfterFaint),
            EventId.AfterMoveSecondarySelf => EffectDelegate.FromNullableDelegate(OnAfterMoveSecondarySelf),
            EventId.AfterMoveSecondary => EffectDelegate.FromNullableDelegate(OnAfterMoveSecondary),
            EventId.AfterMove => EffectDelegate.FromNullableDelegate(OnAfterMove),
            EventId.AfterMoveSelf => EffectDelegate.FromNullableDelegate(OnAfterMoveSelf),
            EventId.Attract => EffectDelegate.FromNullableDelegate(OnAttract),
            EventId.Accuracy => EffectDelegate.FromNullableDelegate(OnAccuracy),
            EventId.BasePower => EffectDelegate.FromNullableDelegate(OnBasePower),
            EventId.BeforeFaint => EffectDelegate.FromNullableDelegate(OnBeforeFaint),
            EventId.BeforeMove => EffectDelegate.FromNullableDelegate(OnBeforeMove),
            EventId.BeforeSwitchIn => EffectDelegate.FromNullableDelegate(OnBeforeSwitchIn),
            EventId.BeforeSwitchOut => EffectDelegate.FromNullableDelegate(OnBeforeSwitchOut),
            EventId.BeforeTurn => EffectDelegate.FromNullableDelegate(OnBeforeTurn),
            EventId.ChangeBoost => EffectDelegate.FromNullableDelegate(OnChangeBoost),
            EventId.TryBoost => EffectDelegate.FromNullableDelegate(OnTryBoost),
            EventId.ChargeMove => EffectDelegate.FromNullableDelegate(OnChargeMove),
            EventId.CriticalHit => EffectDelegate.FromNullableOnCriticalHit(OnCriticalHit),
            EventId.Damage => EffectDelegate.FromNullableDelegate(OnDamage),
            EventId.DeductPp => EffectDelegate.FromNullableDelegate(OnDeductPp),
            EventId.DisableMove => EffectDelegate.FromNullableDelegate(OnDisableMove),
            EventId.DragOut => EffectDelegate.FromNullableDelegate(OnDragOut),
            EventId.EatItem => EffectDelegate.FromNullableDelegate(OnEatItem),
            EventId.Effectiveness => EffectDelegate.FromNullableDelegate(OnEffectiveness),
            EventId.EntryHazard => EffectDelegate.FromNullableDelegate(OnEntryHazard),
            EventId.Faint => EffectDelegate.FromNullableDelegate(OnFaint),
            EventId.Flinch => EffectDelegate.FromNullableOnFlinch(OnFlinch),
            EventId.FractionalPriority => EffectDelegate.FromNullableOnFractionalPriority(OnFractionalPriority),
            EventId.Hit => EffectDelegate.FromNullableDelegate(OnHit),
            EventId.Immunity => EffectDelegate.FromNullableDelegate(OnImmunity),
            EventId.LockMove => EffectDelegate.FromNullableOnLockMove(OnLockMove),
            EventId.MaybeTrapPokemon => EffectDelegate.FromNullableDelegate(OnMaybeTrapPokemon),
            EventId.ModifyAccuracy => EffectDelegate.FromNullableDelegate(OnModifyAccuracy),
            EventId.ModifyAtk => EffectDelegate.FromNullableDelegate(OnModifyAtk),
            EventId.ModifyBoost => EffectDelegate.FromNullableDelegate(OnModifyBoost),
            EventId.ModifyCritRatio => EffectDelegate.FromNullableDelegate(OnModifyCritRatio),
            EventId.ModifyDamage => EffectDelegate.FromNullableDelegate(OnModifyDamage),
            EventId.ModifyDef => EffectDelegate.FromNullableDelegate(OnModifyDef),
            EventId.ModifyMove => EffectDelegate.FromNullableDelegate(OnModifyMove),
            EventId.ModifyPriority => EffectDelegate.FromNullableDelegate(OnModifyPriority),
            EventId.ModifySecondaries => EffectDelegate.FromNullableDelegate(OnModifySecondaries),
            EventId.ModifyType => EffectDelegate.FromNullableDelegate(OnModifyType),
            EventId.ModifyTarget => EffectDelegate.FromNullableDelegate(OnModifyTarget),
            EventId.ModifySpA => EffectDelegate.FromNullableDelegate(OnModifySpA),
            EventId.ModifySpD => EffectDelegate.FromNullableDelegate(OnModifySpD),
            EventId.ModifySpe => EffectDelegate.FromNullableDelegate(OnModifySpe),
            EventId.ModifyStab => EffectDelegate.FromNullableDelegate(OnModifyStab),
            EventId.ModifyWeight => EffectDelegate.FromNullableDelegate(OnModifyWeight),
            EventId.MoveAborted => EffectDelegate.FromNullableDelegate(OnMoveAborted),
            EventId.NegateImmunity => EffectDelegate.FromNullableOnNegateImmunity(OnNegateImmunity),
            EventId.OverrideAction => EffectDelegate.FromNullableDelegate(OnOverrideAction),
            EventId.PrepareHit => EffectDelegate.FromNullableDelegate(OnPrepareHit),
            EventId.PseudoWeatherChange => EffectDelegate.FromNullableDelegate(OnPseudoWeatherChange),
            EventId.RedirectTarget => EffectDelegate.FromNullableDelegate(OnRedirectTarget),
            EventId.Residual => EffectDelegate.FromNullableDelegate(OnResidual),
            EventId.SetAbility => EffectDelegate.FromNullableDelegate(OnSetAbility),
            EventId.SetStatus => EffectDelegate.FromNullableDelegate(OnSetStatus),
            EventId.SetWeather => EffectDelegate.FromNullableDelegate(OnSetWeather),
            EventId.SideConditionStart => EffectDelegate.FromNullableDelegate(OnSideConditionStart),
            EventId.StallMove => EffectDelegate.FromNullableDelegate(OnStallMove),
            EventId.SwitchIn => EffectDelegate.FromNullableDelegate(OnSwitchIn),
            EventId.SwitchOut => EffectDelegate.FromNullableDelegate(OnSwitchOut),
            EventId.Swap => EffectDelegate.FromNullableDelegate(OnSwap),
            EventId.TakeItem => EffectDelegate.FromNullableOnTakeItem(OnTakeItem),
            EventId.WeatherChange => EffectDelegate.FromNullableDelegate(OnWeatherChange),
            EventId.TerrainChange => EffectDelegate.FromNullableDelegate(OnTerrainChange),
            EventId.TrapPokemon => EffectDelegate.FromNullableDelegate(OnTrapPokemon),
            EventId.TryAddVolatile => EffectDelegate.FromNullableDelegate(OnTryAddVolatile),
            EventId.TryEatItem => EffectDelegate.FromNullableOnTryEatItem(OnTryEatItem),
            EventId.TryHeal => EffectDelegate.FromNullableOnTryHeal(OnTryHeal),
            EventId.TryHit => EffectDelegate.FromNullableDelegate(OnTryHit),
            EventId.TryHitField => EffectDelegate.FromNullableDelegate(OnTryHitField),
            EventId.TryHitSide => EffectDelegate.FromNullableDelegate(OnTryHitSide),
            EventId.Invulnerability => EffectDelegate.FromNullableDelegate(OnInvulnerability),
            EventId.TryMove => EffectDelegate.FromNullableDelegate(OnTryMove),
            EventId.TryPrimaryHit => EffectDelegate.FromNullableDelegate(OnTryPrimaryHit),
            EventId.Type => EffectDelegate.FromNullableDelegate(OnType),
            EventId.UseItem => EffectDelegate.FromNullableDelegate(OnUseItem),
            EventId.Update => EffectDelegate.FromNullableDelegate(OnUpdate),
            EventId.Weather => EffectDelegate.FromNullableDelegate(OnWeather),
            EventId.WeatherModifyDamage => EffectDelegate.FromNullableDelegate(OnWeatherModifyDamage),
            EventId.ModifyDamagePhase1 => EffectDelegate.FromNullableDelegate(OnModifyDamagePhase1),
            EventId.ModifyDamagePhase2 => EffectDelegate.FromNullableDelegate(OnModifyDamagePhase2),
            EventId.SideStart => EffectDelegate.FromNullableDelegate(OnSideStart),
            EventId.SideRestart => EffectDelegate.FromNullableDelegate(OnSideRestart),
            EventId.SideResidual => EffectDelegate.FromNullableDelegate(OnSideResidual),
            EventId.SideEnd => EffectDelegate.FromNullableDelegate(OnSideEnd),
            EventId.FieldStart => EffectDelegate.FromNullableDelegate(OnFieldStart),
            EventId.FieldRestart => EffectDelegate.FromNullableDelegate(OnFieldRestart),
            EventId.FieldResidual => EffectDelegate.FromNullableDelegate(OnFieldResidual),
            EventId.FieldEnd => EffectDelegate.FromNullableDelegate(OnFieldEnd),
            _ => null,
        };
    }

    public int? GetPriority(EventId id)
    {
        return id switch
        {
            EventId.Accuracy => OnAccuracyPriority,
            EventId.DamagingHit => OnDamagingHitOrder,
            EventId.AfterMoveSecondary => OnAfterMoveSecondaryPriority,
            EventId.AfterMoveSecondarySelf => OnAfterMoveSecondarySelfPriority,
            EventId.AfterMoveSelf => OnAfterMoveSelfPriority,
            EventId.AfterSetStatus => OnAfterSetStatusPriority,
            EventId.AnyBasePower => OnAnyBasePowerPriority,
            EventId.AnyInvulnerability => OnAnyInvulnerabilityPriority,
            EventId.AnyModifyAccuracy => OnAnyModifyAccuracyPriority,
            EventId.AnyFaint => OnAnyFaintPriority,
            EventId.AnyPrepareHit => OnAnyPrepareHitPriority,
            EventId.AnySwitchIn => OnAnySwitchInPriority,
            EventId.AllyBasePower => OnAllyBasePowerPriority,
            EventId.AllyModifyAtk => OnAllyModifyAtkPriority,
            EventId.AllyModifySpA => OnAllyModifySpAPriority,
            EventId.AllyModifySpD => OnAllyModifySpDPriority,
            EventId.Attract => OnAttractPriority,
            EventId.BasePower => OnBasePowerPriority,
            EventId.BeforeMove => OnBeforeMovePriority,
            EventId.BeforeSwitchOut => OnBeforeSwitchOutPriority,
            EventId.ChangeBoost => OnChangeBoostPriority,
            EventId.Damage => OnDamagePriority,
            EventId.DragOut => OnDragOutPriority,
            EventId.Effectiveness => OnEffectivenessPriority,
            EventId.FoeBasePower => OnFoeBasePowerPriority,
            EventId.FoeBeforeMove => OnFoeBeforeMovePriority,
            EventId.FoeModifyDef => OnFoeModifyDefPriority,
            EventId.FoeModifySpD => OnFoeModifySpDPriority,
            EventId.FoeRedirectTarget => OnFoeRedirectTargetPriority,
            EventId.FoeTrapPokemon => OnFoeTrapPokemonPriority,
            EventId.FractionalPriority => OnFractionalPriorityPriority,
            EventId.Hit => OnHitPriority,
            EventId.Invulnerability => OnInvulnerabilityPriority,
            EventId.ModifyAccuracy => OnModifyAccuracyPriority,
            EventId.ModifyAtk => OnModifyAtkPriority,
            EventId.ModifyCritRatio => OnModifyCritRatioPriority,
            EventId.ModifyDef => OnModifyDefPriority,
            EventId.ModifyMove => OnModifyMovePriority,
            EventId.ModifyPriority => OnModifyPriorityPriority,
            EventId.ModifySpA => OnModifySpAPriority,
            EventId.ModifySpD => OnModifySpDPriority,
            EventId.ModifySpe => OnModifySpePriority,
            EventId.ModifyStab => OnModifyStabPriority,
            EventId.ModifyType => OnModifyTypePriority,
            EventId.ModifyWeight => OnModifyWeightPriority,
            EventId.RedirectTarget => OnRedirectTargetPriority,
            EventId.Residual => OnResidualPriority,
            EventId.SourceBasePower => OnSourceBasePowerPriority,
            EventId.SourceInvulnerability => OnSourceInvulnerabilityPriority,
            EventId.SourceModifyAccuracy => OnSourceModifyAccuracyPriority,
            EventId.SourceModifyAtk => OnSourceModifyAtkPriority,
            EventId.SourceModifyDamage => OnSourceModifyDamagePriority,
            EventId.SourceModifySpA => OnSourceModifySpAPriority,
            EventId.SwitchIn => OnSwitchInPriority,
            EventId.TrapPokemon => OnTrapPokemonPriority,
            EventId.TryBoost => OnTryBoostPriority,
            EventId.TryEatItem => OnTryEatItemPriority,
            EventId.TryHeal => OnTryHealPriority,
            EventId.TryHit => OnTryHitPriority,
            EventId.TryMove => OnTryMovePriority,
            EventId.TryPrimaryHit => OnTryPrimaryHitPriority,
            EventId.Type => OnTypePriority,
            EventId.SideResidual => OnSideResidualPriority,
            EventId.FieldResidual => OnFieldResidualPriority,
            _ => null,
     };
    }

    public IntFalseUnion? GetOrder(EventId id)
    {
        return id switch
        {
            EventId.DamagingHit => OnDamagingHitOrder.HasValue ? IntFalseUnion.FromInt(OnDamagingHitOrder.Value) :
                null,
            EventId.Residual => OnResidualOrder.HasValue ? IntFalseUnion.FromInt(OnResidualOrder.Value) : null,
            EventId.SideResidual => OnSideResidualOrder.HasValue ? IntFalseUnion.FromInt(OnSideResidualOrder.Value) :
                null,

            EventId.FieldResidual => OnFieldResidualOrder.HasValue ?
                IntFalseUnion.FromInt(OnFieldResidualOrder.Value) : null,

            _ => null,
        };
    }

    public int? GetSubOrder(EventId id)
    {
        return id switch
        {
            EventId.AnySwitchIn => OnAnySwitchInSubOrder,
            EventId.Residual => OnResidualSubOrder,
            EventId.SwitchIn => OnSwitchInSubOrder,
            EventId.SideResidual => OnSideResidualSubOrder,
            EventId.FieldResidual => OnFieldResidualSubOrder,
            _ => null,
        };
    }

    public Condition Copy()
    {
        return this with
        {
            // Records have built-in copy semantics with 'with' expression
            // This creates a shallow copy which is appropriate since most properties
            // are either value types, immutable references (strings), or function delegates
            // The only mutable properties (Duration, Counter) are copied correctly
        };
    }

    public override string ToString()
    {
        return Name;
    }
}
