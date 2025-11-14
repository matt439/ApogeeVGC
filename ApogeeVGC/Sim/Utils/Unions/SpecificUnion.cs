using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// CommonHandlers['ModifierSourceMove'] | -0.1
/// </summary>
public abstract record OnFractionalPriority : IUnionEventHandler
{
    public static implicit operator OnFractionalPriority(ModifierSourceMoveHandler function) =>
        new OnFractionalPriorityFunc(function);

    private static readonly decimal PriorityValue = new(-0.1);

    public static implicit operator OnFractionalPriority(decimal value) =>
        value == PriorityValue
            ? new OnFrationalPriorityNeg(value)
            : throw new ArgumentException("Must be -0.1 for OnFractionalPriorityNeg");

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnFractionalPriorityFunc(ModifierSourceMoveHandler Function) : OnFractionalPriority
{
    public override Delegate GetDelegate() => Function;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnFrationalPriorityNeg(decimal Value) : OnFractionalPriority
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object GetConstantValue() => Value;
}


/// <summary>
/// ((this: Battle, pokemon: Pokemon, source: null, move: ActiveMove) =&gt; boolean | void) | boolean
/// </summary>
public abstract record OnCriticalHit : IUnionEventHandler
{
    public static implicit operator OnCriticalHit(Func<Battle, Pokemon, object?, Move, BoolVoidUnion> function) =>
        new OnCriticalHitFunc(function);
    public static implicit operator OnCriticalHit(bool value) => new OnCriticalHitBool(value);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnCriticalHitFunc(Func<Battle, Pokemon, object?, Move, BoolVoidUnion> Function)
    : OnCriticalHit
{
    public override Delegate GetDelegate() => Function;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnCriticalHitBool(bool Value) : OnCriticalHit
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object GetConstantValue() => Value;
}

/// <summary>
/// ((this: Battle, relayVar: number, target: Pokemon, source: Pokemon, effect: Effect) => number | boolean | void)
/// | ((this: Battle, pokemon: Pokemon) => boolean | void)
/// | boolean
/// </summary>
public abstract record OnTryHeal : IUnionEventHandler
{
    public static implicit operator OnTryHeal(
        Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?> func) =>
        new OnTryHealFunc1(func);
    public static implicit operator OnTryHeal(Func<Battle, Pokemon, bool?> func) =>
        new OnTryHealFunc2(func);
    public static implicit operator OnTryHeal(bool value) => new OnTryHealBool(value);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnTryHealFunc1(Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?> Func) :
    OnTryHeal
{
    public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnTryHealFunc2(Func<Battle, Pokemon, bool?> Func) : OnTryHeal
{
    public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnTryHealBool(bool Value) : OnTryHeal
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object GetConstantValue() => Value;
}


/// <summary>
/// ((this: Battle, pokemon: Pokemon, source: null, move: ActiveMove) => boolean | void) | boolean
/// </summary>
public abstract record OnFlinch : IUnionEventHandler
{
    public static implicit operator OnFlinch(Func<Battle, Pokemon, object?, Move, BoolVoidUnion> func) =>
        new OnFlinchFunc(func);
    public static implicit operator OnFlinch(bool value) => new OnFlinchBool(value);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnFlinchFunc(Func<Battle, Pokemon, object?, Move, BoolVoidUnion> Func) : OnFlinch
{
    public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnFlinchBool(bool Value) : OnFlinch
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object GetConstantValue() => Value;
}



/// <summary>
/// string | ((this: Battle, pokemon: Pokemon) => void | string)    
/// </summary>
public abstract record OnLockMove : IUnionEventHandler
{
    public static implicit operator OnLockMove(MoveId moveId) => new OnLockMoveMoveId(moveId);
    public static implicit operator OnLockMove(Func<Battle, Pokemon, MoveIdVoidUnion> func) =>
        new OnLockMoveFunc(func);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnLockMoveMoveId(MoveId Id) : OnLockMove
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object GetConstantValue() => Id;
}

public record OnLockMoveFunc(Func<Battle, Pokemon, MoveIdVoidUnion> Func) : OnLockMove
{
    public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}


/// <summary>
/// SparseBoostsTable | void
/// </summary>
public abstract record SparseBoostsTableVoidUnion
{
    public static implicit operator SparseBoostsTableVoidUnion(SparseBoostsTable table) =>
        new SparseBoostsTableSparseBoostsTableVoidUnion(table);
    public static implicit operator SparseBoostsTableVoidUnion(VoidReturn value) =>
        new VoidSparseBoostsTableVoidUnion(value);
    public static SparseBoostsTableVoidUnion FromVoid() => new VoidSparseBoostsTableVoidUnion(new VoidReturn());
}
public record SparseBoostsTableSparseBoostsTableVoidUnion(SparseBoostsTable Table) :
    SparseBoostsTableVoidUnion;
public record VoidSparseBoostsTableVoidUnion(VoidReturn Value) : SparseBoostsTableVoidUnion;


/// <summary>
/// ((this: Battle, pokemon: Pokemon, type: string) => boolean | void) | boolean
/// </summary>
public abstract record OnNegateImmunity : IUnionEventHandler
{
    public static implicit operator OnNegateImmunity(Func<Battle, Pokemon, PokemonType, BoolVoidUnion> func) =>
        new OnNegateImmunityFunc(func);
    public static implicit operator OnNegateImmunity(bool value) => new OnNegateImmunityBool(value);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnNegateImmunityFunc(Func<Battle, Pokemon, PokemonType, BoolVoidUnion> Func)
    : OnNegateImmunity
{
    public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnNegateImmunityBool(bool Value) : OnNegateImmunity
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object GetConstantValue() => Value;
}




/// <summary>
/// (this: Battle, item: Item, pokemon: Pokemon, source: Pokemon, move?: ActiveMove) => boolean | void) | boolean
/// </summary>
public abstract record OnTakeItem : IUnionEventHandler
{
    public static implicit operator OnTakeItem(Func<Battle, Item, Pokemon, Pokemon, Move?, PokemonVoidUnion> func)
        => new OnTakeItemFunc(func);
    public static implicit operator OnTakeItem(bool value) => new OnTakeItemBool(value);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnTakeItemFunc(Func<Battle, Item, Pokemon, Pokemon, Move?, PokemonVoidUnion> Func)
    : OnTakeItem
{
    public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnTakeItemBool(bool Value) : OnTakeItem
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object GetConstantValue() => Value;
}



/// <summary>
/// Pokemon | Side | Field | Battle | Pokemon?
/// </summary>
public abstract record SingleEventTarget
{
    public static implicit operator SingleEventTarget(Pokemon pokemon) =>
        new PokemonSingleEventTarget(pokemon);

    public static implicit operator SingleEventTarget(Side side) => new SideSingleEventTarget(side);
    public static implicit operator SingleEventTarget(Field field) => new FieldSingleEventTarget(field);

    public static SingleEventTarget FromBattle(Battle battle) => new BattleSingleEventTarget(battle);
    public static SingleEventTarget? FromNullablePokemon(Pokemon? pokemon)
    {
        return pokemon is null ? null : new PokemonSingleEventTarget(pokemon);
    }
}
public record PokemonSingleEventTarget(Pokemon Pokemon) : SingleEventTarget;
public record SideSingleEventTarget(Side Side) : SingleEventTarget;
public record FieldSingleEventTarget(Field Field) : SingleEventTarget;
public record BattleSingleEventTarget(Battle Battle) : SingleEventTarget;



/// <summary>
/// Pokemon | Effect | false | PokemonType
/// </summary>
public abstract record SingleEventSource
{
    public static implicit operator SingleEventSource(Pokemon pokemon) =>
        new PokemonSingleEventSource(pokemon);

    public static SingleEventSource? FromNullablePokemon(Pokemon? pokemon)
    {
        return pokemon is null ? null : new PokemonSingleEventSource(pokemon);
    }

    public static implicit operator SingleEventSource(Ability ability) =>
        EffectUnionFactory.ToSingleEventSource(ability);

    public static implicit operator SingleEventSource(Item item) =>
        EffectUnionFactory.ToSingleEventSource(item);

    public static implicit operator SingleEventSource(ActiveMove activeMove) =>
        EffectUnionFactory.ToSingleEventSource(activeMove);

    public static implicit operator SingleEventSource(Species species) =>
        EffectUnionFactory.ToSingleEventSource(species);

    public static implicit operator SingleEventSource(Condition condition) =>
        EffectUnionFactory.ToSingleEventSource(condition);

    public static implicit operator SingleEventSource(Format format) =>
        EffectUnionFactory.ToSingleEventSource(format);

    public static SingleEventSource FromFalse() => new FalseSingleEventSource();
    public static implicit operator SingleEventSource(PokemonType type) => new PokemonTypeSingleEventSource(type);
}
public record PokemonSingleEventSource(Pokemon Pokemon) : SingleEventSource;
public record EffectSingleEventSource(IEffect Effect) : SingleEventSource;
public record FalseSingleEventSource : SingleEventSource;
public record PokemonTypeSingleEventSource(PokemonType Type) : SingleEventSource;


/// <summary>
/// Pokemon | Pokemon? | false | Value
/// </summary>
public abstract record RunEventSource
{
    public static implicit operator RunEventSource(Pokemon pokemon) => new PokemonRunEventSource(pokemon);
    public static RunEventSource? FromNullablePokemon(Pokemon? pokemon)
    {
        return pokemon is null ? null : new PokemonRunEventSource(pokemon);
    }
    public static RunEventSource FromFalse() => new FalseRunEventSource();
    public static implicit operator RunEventSource(PokemonType type) => new TypeRunEventSource(type);
}
public record PokemonRunEventSource(Pokemon Pokemon) : RunEventSource;
public record FalseRunEventSource : RunEventSource;
public record TypeRunEventSource(PokemonType Type) : RunEventSource;



/// <summary>
/// bool | int | IEffect | PokemonType | ConditionId? | BoostsTable | List<PokemonType/> | MoveType |
/// SparseBoostsTable | decimal | MoveId | string | RelayVar[] | Pokemon | Pokemon? | IntTrueUnion |
/// BoolIntUndefinedUnion | SecondaryEffect[] | Undefined | VoidReturn
/// </summary>
public abstract record RelayVar
{
    public static implicit operator RelayVar(bool value) => new BoolRelayVar(value);
    public static implicit operator RelayVar(int value) => new IntRelayVar(value);
    public static implicit operator RelayVar(Ability ability) => EffectUnionFactory.ToRelayVar(ability);
    public static implicit operator RelayVar(Item item) => EffectUnionFactory.ToRelayVar(item);
    public static implicit operator RelayVar(ActiveMove activeMove) => EffectUnionFactory.ToRelayVar(activeMove);
    public static implicit operator RelayVar(Species species) => EffectUnionFactory.ToRelayVar(species);
    public static implicit operator RelayVar(Condition condition) => EffectUnionFactory.ToRelayVar(condition);
    public static implicit operator RelayVar(Format format) => EffectUnionFactory.ToRelayVar(format);
    public static implicit operator RelayVar(PokemonType type) => new PokemonTypeRelayVar(type);
    public static implicit operator RelayVar(ConditionId? id) => new ConditionIdRelayVar(id);
    public static implicit operator RelayVar(BoostsTable table) => new BoostsTableRelayVar(table);
    public static implicit operator RelayVar(List<PokemonType> types) => new TypesRelayVar(types);
    public static implicit operator RelayVar(MoveType type) => new PokemonTypeRelayVar((PokemonType)type);
    public static implicit operator RelayVar(SparseBoostsTable table) => new SparseBoostsTableRelayVar(table);
    public static implicit operator RelayVar(decimal value) => new DecimalRelayVar(value);
    public static implicit operator RelayVar(MoveId moveId) => new MoveIdRelayVar(moveId);
    public static implicit operator RelayVar(string value) => new StringRelayVar(value);
 public static implicit operator RelayVar(RelayVar[] values) => new ArrayRelayVar([.. values]);
    public static implicit operator RelayVar(List<RelayVar> values) => new ArrayRelayVar(values);
    public static implicit operator RelayVar(Pokemon pokemon) => new PokemonRelayVar(pokemon);
    public static implicit operator RelayVar(VoidReturn value) => new VoidReturnRelayVar();
    public static RelayVar? FromNullablePokemon(Pokemon? pokemon) => pokemon is null ?
        null : new PokemonRelayVar(pokemon);

    public static RelayVar FromIntTrueUnion(IntTrueUnion union) => union switch
    {
   IntIntTrueUnion intValue => new IntRelayVar(intValue.Value),
        TrueIntTrueUnion => new BoolRelayVar(true),
    _ => throw new InvalidOperationException("Unknown IntTrueUnion type"),
    };
    public static implicit operator RelayVar(BoolIntUndefinedUnion union) => new BoolIntUndefinedUnionRelayVar(union);
    public static implicit operator RelayVar(SecondaryEffect[] effects) => new SecondaryEffectArrayRelayVar(effects);
    public static RelayVar FromUndefined() => new UndefinedRelayVar();
  public static RelayVar FromVoid() => new VoidReturnRelayVar();
}
public record BoolRelayVar(bool Value) : RelayVar;
public record IntRelayVar(int Value) : RelayVar;
public record EffectRelayVar(IEffect Effect) : RelayVar;
public record SpecieRelayVar(Species Species) : RelayVar;
public record PokemonTypeRelayVar(PokemonType Type) : RelayVar;
public record ConditionIdRelayVar(ConditionId? Id) : RelayVar;
public record BoostsTableRelayVar(BoostsTable Table) : RelayVar;
public record TypesRelayVar(List<PokemonType> Types) : RelayVar;
public record MoveTypeRelayVar(MoveType Type) : RelayVar;
public record SparseBoostsTableRelayVar(SparseBoostsTable Table) : RelayVar;
public record DecimalRelayVar(decimal Value) : RelayVar;
public record MoveIdRelayVar(MoveId MoveId) : RelayVar;
public record StringRelayVar(string Value) : RelayVar;
public record ArrayRelayVar(List<RelayVar> Values) : RelayVar;
public record PokemonRelayVar(Pokemon Pokemon) : RelayVar;
public record BoolIntUndefinedUnionRelayVar(BoolIntUndefinedUnion Value) : RelayVar;
public record SecondaryEffectArrayRelayVar(SecondaryEffect[] Effects) : RelayVar;
public record UndefinedRelayVar : RelayVar;
public record VoidReturnRelayVar : RelayVar;





/// <summary>
/// Pokemon | Pokemon[] | Side | Battle | PokemonSideBattleUnion? | PokemonSideBattleUnion|  Field | Pokemon?
/// </summary>
public abstract record RunEventTarget
{
    public static implicit operator RunEventTarget(Pokemon pokemon) =>
        new PokemonRunEventTarget(pokemon);
    public static RunEventTarget? FromNullablePokemon(Pokemon? pokemon)
    {
        return pokemon is null ? null : new PokemonRunEventTarget(pokemon);
    }

    public static implicit operator RunEventTarget(Pokemon[] pokemonList) =>
            new PokemonArrayRunEventTarget(pokemonList);

    public static implicit operator RunEventTarget(Side side) => new SideRunEventTarget(side);
    public static RunEventTarget FromBattle(Battle battle) => new BattleRunEventTarget(battle);
    public static RunEventTarget? FromNullablePokemonSideBattleUnion(PokemonSideBattleUnion? target)
    {
        return target switch
        {
            null => null,
            PokemonSideBattlePokemon pokemon => new PokemonRunEventTarget(pokemon.Pokemon),
            PokemonSideBattleSide side => new SideRunEventTarget(side.Side),
            PokemonSideBattleBattle battle => new BattleRunEventTarget(battle.Battle),
            _ => throw new InvalidOperationException("Cannot convert to RunEventTarget"),
        };
    }
    public static RunEventTarget FromPokemonSideBattleUnion(PokemonSideBattleUnion target)
    {
        return target switch
        {
            PokemonSideBattlePokemon pokemon => new PokemonRunEventTarget(pokemon.Pokemon),
            PokemonSideBattleSide side => new SideRunEventTarget(side.Side),
            PokemonSideBattleBattle battle => new BattleRunEventTarget(battle.Battle),
            _ => throw new InvalidOperationException("Cannot convert to RunEventTarget"),
        };
    }
    public static implicit operator RunEventTarget(Field field) => new FieldRunEventTarget(field);
}

public record PokemonRunEventTarget(Pokemon Pokemon) : RunEventTarget;
public record PokemonArrayRunEventTarget(Pokemon[] PokemonList) : RunEventTarget;
public record SideRunEventTarget(Side Side) : RunEventTarget;
public record BattleRunEventTarget(Battle Battle) : RunEventTarget;
public record FieldRunEventTarget(Field Field) : RunEventTarget;




/// <summary>
/// SpecieId | AbilityId | ItemId | ConditionId | MoveId | FormatId | Empty
/// </summary>
public abstract record EffectStateId
{
    public static implicit operator EffectStateId(SpecieId specieId) => new SpecieEffectStateId(specieId);
    public static implicit operator EffectStateId(AbilityId abilityId) => new AbilityEffectStateId(abilityId);
    public static implicit operator EffectStateId(ItemId itemId) => new ItemEffectStateId(itemId);
    public static implicit operator EffectStateId(ConditionId conditionId) => new ConditionEffectStateId(conditionId);
    public static implicit operator EffectStateId(MoveId moveId) => new MoveEffectStateId(moveId);
    public static implicit operator EffectStateId(FormatId formatId) => new FormatEffectStateId(formatId);
    public static EffectStateId FromEmpty() => new EmptyEffectStateId();
}
public record SpecieEffectStateId(SpecieId SpecieId) : EffectStateId;
public record AbilityEffectStateId(AbilityId AbilityId) : EffectStateId;
public record ItemEffectStateId(ItemId ItemId) : EffectStateId;
public record ConditionEffectStateId(ConditionId ConditionId) : EffectStateId;
public record MoveEffectStateId(MoveId MoveId) : EffectStateId;
public record FormatEffectStateId(FormatId FormatId) : EffectStateId;
public record EmptyEffectStateId : EffectStateId;



/// <summary>
/// Pokemon | Side | Field | Battle
/// Represents the resolved target parameter type for event callbacks.
/// </summary>
public abstract record EventTargetParameter
{
    public static EventTargetParameter? FromSingleEventTarget(SingleEventTarget? target, Type expectedType)
    {
        if (target == null) return null;

        return target switch
        {
            PokemonSingleEventTarget p when expectedType.IsAssignableFrom(typeof(Pokemon)) =>
                new PokemonEventTargetParameter(p.Pokemon),
            SideSingleEventTarget s when expectedType.IsAssignableFrom(typeof(Side)) =>
                new SideEventTargetParameter(s.Side),
            FieldSingleEventTarget f when expectedType.IsAssignableFrom(typeof(Field)) =>
                new FieldEventTargetParameter(f.Field),
            BattleSingleEventTarget b when expectedType.IsAssignableFrom(typeof(Battle)) =>
                new BattleEventTargetParameter(b.Battle),
            _ => null,
        };
    }

    public object? ToObject()
    {
        return this switch
        {
            PokemonEventTargetParameter p => p.Pokemon,
            SideEventTargetParameter s => s.Side,
            FieldEventTargetParameter f => f.Field,
            BattleEventTargetParameter b => b.Battle,
            _ => null,
        };
    }
}
public record PokemonEventTargetParameter(Pokemon Pokemon) : EventTargetParameter;
public record SideEventTargetParameter(Side Side) : EventTargetParameter;
public record FieldEventTargetParameter(Field Field) : EventTargetParameter;
public record BattleEventTargetParameter(Battle Battle) : EventTargetParameter;



/// <summary>
/// Pokemon | IEffect | PokemonType | bool (false)
/// Represents the resolved source parameter type for event callbacks.
/// </summary>
public abstract record EventSourceParameter
{
    public static EventSourceParameter? FromSingleEventSource(SingleEventSource? source, Type expectedType)
    {
        if (source == null) return null;

        return source switch
        {
            PokemonSingleEventSource p when expectedType.IsAssignableFrom(typeof(Pokemon)) =>
                new PokemonEventSourceParameter(p.Pokemon),
            EffectSingleEventSource e when expectedType.IsAssignableFrom(typeof(IEffect)) =>
                new EffectEventSourceParameter(e.Effect),
            PokemonTypeSingleEventSource t when expectedType.IsAssignableFrom(typeof(PokemonType)) =>
                new PokemonTypeEventSourceParameter(t.Type),
            FalseSingleEventSource when expectedType == typeof(bool) =>
                new BoolEventSourceParameter(false),
            _ => null,
        };
    }

    public object? ToObject()
    {
        return this switch
        {
            PokemonEventSourceParameter p => p.Pokemon,
            EffectEventSourceParameter e => e.Effect,
            PokemonTypeEventSourceParameter t => t.Type,
            BoolEventSourceParameter b => b.Value,
            _ => null,
        };
    }
}
public record PokemonEventSourceParameter(Pokemon Pokemon) : EventSourceParameter;
public record EffectEventSourceParameter(IEffect Effect) : EventSourceParameter;
public record PokemonTypeEventSourceParameter(PokemonType Type) : EventSourceParameter;
public record BoolEventSourceParameter(bool Value) : EventSourceParameter;



/// <summary>
/// SparseBoostsTable | false
/// </summary>
public abstract record ItemBoosts
{
    public static implicit operator ItemBoosts(SparseBoostsTable table) =>
        new SparseBoostsTableItemBoosts(table);

    public static ItemBoosts FromFalse() => new FalseItemBoosts();
}
public record SparseBoostsTableItemBoosts(SparseBoostsTable Table) : ItemBoosts;
public record FalseItemBoosts : ItemBoosts;



/// <summary>
/// (this: Battle, pokemon: Pokemon) => void) | false
/// </summary>
public abstract record OnItemEatUse : IUnionEventHandler
{
    public static implicit operator OnItemEatUse(Action<Battle, Pokemon> func) =>
        new OnItemEatUseFunc(func);
    public static OnItemEatUse FromFalse() => new OnItemEatUseFalse();

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnItemEatUseFunc(Action<Battle, Pokemon> Func) : OnItemEatUse
{
    public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnItemEatUseFalse : OnItemEatUse
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object? GetConstantValue() => null;
}


/// <summary>
/// bool | ((this: Battle, item: Item, pokemon: Pokemon) => boolean | void)
/// </summary>
public abstract record OnTryEatItem : IUnionEventHandler
{
    public static implicit operator OnTryEatItem(bool value) => new BoolOnTryEatItem(value);
    public static implicit operator OnTryEatItem(Func<Battle, Item, Pokemon, BoolVoidUnion> func) =>
        new FuncOnTryEatItem(func);

    public static OnTryEatItem FromFunc(Func<Battle, Item, Pokemon, BoolVoidUnion> func) =>
            new FuncOnTryEatItem(func);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record BoolOnTryEatItem(bool Value) : OnTryEatItem
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object GetConstantValue() => Value;
}

public record FuncOnTryEatItem(Func<Battle, Item, Pokemon, BoolVoidUnion> Func) : OnTryEatItem
{
    public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}



/// <summary>
/// int | 'level' | false
/// </summary>
public abstract record MoveDamage
{
    public static implicit operator MoveDamage(int value) => new IntMoveDamage(value);
    public static MoveDamage FromLevel() => new LevelMoveDamage();
    public static MoveDamage FromFalse() => new FalseMoveDamage();
}
public record IntMoveDamage(int Value) : MoveDamage;
public record LevelMoveDamage : MoveDamage;
public record FalseMoveDamage : MoveDamage;



/// <summary>
/// bool | 'ice'
/// </summary>
public abstract record MoveOhko
{
    public static implicit operator MoveOhko(bool value) => new BoolMoveOhko(value);
    public static MoveOhko FromIce() => new IceMoveOhko();
}
public record BoolMoveOhko(bool Value) : MoveOhko;
public record IceMoveOhko : MoveOhko;




/// <summary>
/// 'copyvolatile' | 'shedtail' | bool
/// </summary>
public abstract record MoveSelfSwitch
{
    public static MoveSelfSwitch FromCopyVolatile() => new CopyVolatileMoveSelfSwitch();
    public static MoveSelfSwitch FromShedTail() => new ShedTailMoveSelfSwitch();
    public static implicit operator MoveSelfSwitch(bool value) => new BoolMoveSelfSwitch(value);
}
public record CopyVolatileMoveSelfSwitch : MoveSelfSwitch;
public record ShedTailMoveSelfSwitch : MoveSelfSwitch;
public record BoolMoveSelfSwitch(bool Value) : MoveSelfSwitch;



/// <summary>
/// 'always' | 'ifHit' | bool
/// </summary>
public abstract record MoveSelfDestruct
{
    public static MoveSelfDestruct FromAlways() => new AlwaysMoveSelfDestruct();
    public static MoveSelfDestruct FromIfHit() => new IfHitMoveSelfDestruct();
    public static implicit operator MoveSelfDestruct(bool value) => new BoolMoveSelfDestruct(value);
}
public record AlwaysMoveSelfDestruct : MoveSelfDestruct;
public record IfHitMoveSelfDestruct : MoveSelfDestruct;
public record BoolMoveSelfDestruct(bool Value) : MoveSelfDestruct;




/// <summary>
/// boolean | { [PokemonType: string]: boolean }
/// </summary>
public abstract record MoveIgnoreImmunity
{
    public static implicit operator MoveIgnoreImmunity(bool value) =>
        new BoolMoveDataIgnoreImmunity(value);

    public static implicit operator MoveIgnoreImmunity(Dictionary<PokemonType, bool> typeImmunities) =>
        new TypeMoveDataIgnoreImmunity(typeImmunities);
}
public record BoolMoveDataIgnoreImmunity(bool Value) : MoveIgnoreImmunity;
public record TypeMoveDataIgnoreImmunity(Dictionary<PokemonType, bool> TypeImmunities) :
    MoveIgnoreImmunity;





/// <summary>
/// bool | 'past'
/// </summary>
public abstract record SpecieUnreleasedHidden
{
    public static implicit operator SpecieUnreleasedHidden(bool value) => new BoolSpecieUnreleasedHidden(value);
    public static SpecieUnreleasedHidden FromPast() => new PastSpecieUnreleasedHidden();
}
public record BoolSpecieUnreleasedHidden(bool Value) : SpecieUnreleasedHidden;
public record PastSpecieUnreleasedHidden : SpecieUnreleasedHidden;



/// <summary>
/// Effect | 'drain' | 'recoil'
/// </summary>
public abstract record BattleDamageEffect
{
    public static BattleDamageEffect FromIEffect(IEffect effect) => new EffectBattleDamageEffect(effect);
    public static BattleDamageEffect FromDrain() => new DrainBattleDamageEffect();
    public static BattleDamageEffect FromRecoil() => new RecoilBattleDamageEffect();
}
public record EffectBattleDamageEffect(IEffect Effect) : BattleDamageEffect;
public record DrainBattleDamageEffect : BattleDamageEffect;
public record RecoilBattleDamageEffect : BattleDamageEffect;


/// <summary>
/// Effect | 'drain'
/// </summary>
public abstract record BattleHealEffect
{
    public static BattleHealEffect FromIEffect(IEffect effect) => new EffectBattleHealEffect(effect);
    public static BattleHealEffect FromDrain() => new DrainBattleHealEffect();
}
public record EffectBattleHealEffect(IEffect Effect) : BattleHealEffect;
public record DrainBattleHealEffect : BattleHealEffect;





/// <summary>
/// Pokemon | Side | Field | Battle
/// </summary>
public abstract record EffectStateTarget
{
    public static implicit operator EffectStateTarget(Pokemon pokemon) => new PokemonEffectStateTarget(pokemon);
    public static implicit operator EffectStateTarget(Side side) => new SideEffectStateTarget(side);
    public static implicit operator EffectStateTarget(Field field) => new FieldEffectStateTarget(field);
    public static EffectStateTarget FromBattle(Battle battle) => new BattleEffectStateTarget(battle);
}
public record PokemonEffectStateTarget(Pokemon Pokemon) : EffectStateTarget;
public record SideEffectStateTarget(Side Side) : EffectStateTarget;
public record FieldEffectStateTarget(Field Field) : EffectStateTarget;
public record BattleEffectStateTarget(Battle Battle) : EffectStateTarget;



/// <summary>
/// Pokemon | Side | Field | Battle
/// </summary>
public abstract record EffectHolder
{
    public static implicit operator EffectHolder(Pokemon pokemon) => new PokemonEffectHolder(pokemon);
    public static implicit operator EffectHolder(Side side) => new SideEffectHolder(side);
    public static implicit operator EffectHolder(Field field) => new FieldEffectHolder(field);
    public static EffectHolder FromBattle(Battle battle) => new BattleEffectHolder(battle);
}
public record PokemonEffectHolder(Pokemon Pokemon) : EffectHolder;
public record SideEffectHolder(Side Side) : EffectHolder;
public record FieldEffectHolder(Field Field) : EffectHolder;
public record BattleEffectHolder(Battle Battle) : EffectHolder;



/// <summary>
/// Delegate? | OnFlinch | OnCriticalHit | OnFractionalPriority | OnTakeItem | OnTryHeal | OnTryEatItem |
/// OnNegateImmunity | OnLockMove
/// </summary>
public abstract record EffectDelegate
{
    public abstract Delegate? GetDelegate(int index = 0);

    public static implicit operator EffectDelegate(Delegate del) => new DelegateEffectDelegate(del);
    public static EffectDelegate? FromNullableDelegate(Delegate? del)
    {
        return del is null ? null : new DelegateEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnFlinch onFlinch) => new OnFlinchEffectDelegate(onFlinch);
    public static EffectDelegate? FromNullableOnFlinch(OnFlinch? del)
    {
        return del is null ? null : new OnFlinchEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnCriticalHit onCriticalHit) =>
        new OnCriticalHitEffectDelegate(onCriticalHit);

    public static EffectDelegate? FromNullableOnCriticalHit(OnCriticalHit? del)
    {
        return del is null ? null : new OnCriticalHitEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnFractionalPriority onFractionalPriority) =>
        new OnFractionalPriorityEffectDelegate(onFractionalPriority);
    public static EffectDelegate? FromNullableOnFractionalPriority(OnFractionalPriority? del)
    {
        return del is null ? null : new OnFractionalPriorityEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnTakeItem onTakeItem) =>
        new OnTakeItemEffectDelegate(onTakeItem);
    public static EffectDelegate? FromNullableOnTakeItem(OnTakeItem? del)
    {
        return del is null ? null : new OnTakeItemEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnTryHeal onTryHeal) => new OnTryHealEffectDelegate(onTryHeal);
    public static EffectDelegate? FromNullableOnTryHeal(OnTryHeal? del)
    {
        return del is null ? null : new OnTryHealEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnTryEatItem onTryEatItem) =>
        new OnTryEatItemEffectDelegate(onTryEatItem);
    public static EffectDelegate? FromNullableOnTryEatItem(OnTryEatItem? del)
    {
        return del is null ? null : new OnTryEatItemEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnNegateImmunity onNegateImmunity) =>
            new OnNegateImmunityEffectDelegate(onNegateImmunity);
    public static EffectDelegate? FromNullableOnNegateImmunity(OnNegateImmunity? del)
    {
        return del is null ? null : new OnNegateImmunityEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnLockMove onLockMove) =>
        new OnLockMoveEffectDelegate(onLockMove);
    public static EffectDelegate? FromNullableOnLockMove(OnLockMove? del)
    {
        return del is null ? null : new OnLockMoveEffectDelegate(del);
    }
}

public record DelegateEffectDelegate(Delegate Del) : EffectDelegate
{
    public override Delegate GetDelegate(int index = 0) => Del;
}

public record OnFlinchEffectDelegate(OnFlinch OnFlinch) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0) => OnFlinch switch
    {
        OnFlinchFunc f => f.Func,
        _ => null,
    };
}

public record OnCriticalHitEffectDelegate(OnCriticalHit OnCriticalHit) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0) => OnCriticalHit switch
    {
        OnCriticalHitFunc f => f.Function,
        _ => null,
    };
}

public record OnFractionalPriorityEffectDelegate(OnFractionalPriority OnFractionalPriority) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0) => OnFractionalPriority switch
    {
        OnFractionalPriorityFunc f => f.Function,
        _ => null,
    };
}

public record OnTakeItemEffectDelegate(OnTakeItem OnTakeItem) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0) => OnTakeItem switch
    {
        OnTakeItemFunc f => f.Func,
        _ => null,
    };
}

public record OnTryHealEffectDelegate(OnTryHeal OnTryHeal) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0)
    {
        if (OnTryHeal is OnTryHealBool) return null;
        return index switch
        {
            0 when OnTryHeal is OnTryHealFunc1 f1 => f1.Func,
            1 when OnTryHeal is OnTryHealFunc2 f2 => f2.Func,
            _ => null,
        };
    }
}
public record OnTryEatItemEffectDelegate(OnTryEatItem OnTryEatItem) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0) => OnTryEatItem switch
    {
        FuncOnTryEatItem f => f.Func,
        _ => null,
    };
}

public record OnNegateImmunityEffectDelegate(OnNegateImmunity OnNegateImmunity) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0) => OnNegateImmunity switch
    {
        OnNegateImmunityFunc f => f.Func,
        _ => null,
    };
}

public record OnLockMoveEffectDelegate(OnLockMove OnLockMove) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0) => OnLockMove switch
    {
        OnLockMoveFunc f => f.Func,
        _ => null,
    };
}


/// <summary>
/// bool | 'integer' | 'positive-integer'
/// </summary>
public abstract record FormatHasValue
{
    public static implicit operator FormatHasValue(bool value) => new BoolFormatHasValue(value);
    public static FormatHasValue FromInteger() => new IntegerFormatHasValue();
    public static FormatHasValue FromPositiveInteger() => new PositiveIntegerFormatHasValue();
}
public record BoolFormatHasValue(bool Value) : FormatHasValue;
public record IntegerFormatHasValue : FormatHasValue;
public record PositiveIntegerFormatHasValue : FormatHasValue;



/// <summary>
/// string | number | boolean | Pokemon | Side | Effect | Move | undefined
/// </summary>
public abstract record Part
{
    public static implicit operator Part(string value) => new StringPart(value);
    public static implicit operator Part(int value) => new IntPart(value);
    public static implicit operator Part(double value) => new DoublePart(value);
    public static implicit operator Part(bool value) => new BoolPart(value);
    public static implicit operator Part(Pokemon? pokemon) =>
  pokemon == null ? new StringPart(string.Empty) : new PokemonPart(pokemon);
    public static implicit operator Part(Side side) => new SidePart(side);
    public static implicit operator Part(ActiveMove move) => new MovePart(move);

    public static implicit operator Part(Item item) => EffectUnionFactory.ToPart(item);
    public static implicit operator Part(Ability ability) => EffectUnionFactory.ToPart(ability);
    public static implicit operator Part(Species species) => EffectUnionFactory.ToPart(species);
    public static implicit operator Part(Condition condition) => EffectUnionFactory.ToPart(condition);
    public static implicit operator Part(Format format) => EffectUnionFactory.ToPart(format);

    public static Part FromIEffect(IEffect effect)
    {
        return effect switch
        {
            Item item => EffectUnionFactory.ToPart(item),
            Ability ability => EffectUnionFactory.ToPart(ability),
            Species species => EffectUnionFactory.ToPart(species),
            Condition condition => EffectUnionFactory.ToPart(condition),
            Format format => EffectUnionFactory.ToPart(format),
            ActiveMove activeMove => EffectUnionFactory.ToPart(activeMove),
            _ => throw new InvalidOperationException("Invalid IEffect type."),
        };
    }

    public static Part FromUndefined() => new UndefinedPart(new Undefined());

    public static implicit operator Part(Undefined value) => new UndefinedPart(value);

    public static Part? FromNullable<T>(T? value) where T : class
    {
        return value switch
        {
            null => null,
            string s => new StringPart(s),
            Pokemon p => new PokemonPart(p),
            Side s => new SidePart(s),
            ActiveMove m => new MovePart(m),
            IEffect e => new EffectPart(e),
            _ => throw new InvalidOperationException($"Unsupported type: {typeof(T)}"),
        };
    }
}
public record StringPart(string Value) : Part;
public record IntPart(int Value) : Part;
public record DoublePart(double Value) : Part;
public record BoolPart(bool Value) : Part;
public record PokemonPart(Pokemon Pokemon) : Part;
public record SidePart(Side Side) : Part;
public record MovePart(ActiveMove Move) : Part;
public record EffectPart(IEffect Effect) : Part;
public record UndefinedPart(Undefined Value) : Part;




/// <summary>
/// Part | (() => { side: SideID, secret: string, shared: string })
/// Represents a battle log part that can be either a direct value or a function that generates side-specific content.
/// </summary>
public abstract record PartFuncUnion
{
    public static implicit operator PartFuncUnion(Part part) => new PartPartFuncUnion(part);
    public static implicit operator PartFuncUnion(Func<SideSecretSharedResult> func) => new FuncPartFuncUnion(func);

    // Convenience implicit conversions for Part types
    public static implicit operator PartFuncUnion(string value) => new PartPartFuncUnion(value);
    public static implicit operator PartFuncUnion(int value) => new PartPartFuncUnion(value);
    public static implicit operator PartFuncUnion(double value) => new PartPartFuncUnion(value);
    public static implicit operator PartFuncUnion(bool value) => new PartPartFuncUnion(value);
    public static implicit operator PartFuncUnion(Pokemon pokemon) => new PartPartFuncUnion(pokemon);
    public static implicit operator PartFuncUnion(Side side) => new PartPartFuncUnion(side);
    public static implicit operator PartFuncUnion(ActiveMove move) => new PartPartFuncUnion(move);
    public static implicit operator PartFuncUnion(Undefined value) => new PartPartFuncUnion(value);

    public static implicit operator PartFuncUnion(Item item) => EffectUnionFactory.ToPart(item);
    public static implicit operator PartFuncUnion(Ability ability) => EffectUnionFactory.ToPart(ability);
    public static implicit operator PartFuncUnion(Species species) => EffectUnionFactory.ToPart(species);
    public static implicit operator PartFuncUnion(Condition condition) => EffectUnionFactory.ToPart(condition);
    public static implicit operator PartFuncUnion(Format format) => EffectUnionFactory.ToPart(format);

    public static PartFuncUnion FromIEffect(IEffect effect) => Part.FromIEffect(effect);
}

public record PartPartFuncUnion(Part Part) : PartFuncUnion;
public record FuncPartFuncUnion(Func<SideSecretSharedResult> Func) : PartFuncUnion;

/// <summary>
/// Represents HP color indicators for Pokemon health display
/// </summary>
public enum HpColor
{
    Green,
    Yellow,
 Red
}

/// <summary>
/// string | ConditionId
/// </summary>
public abstract record Secret
{
    public static implicit operator Secret(string value) => new SecretString(value);
    public static implicit operator Secret(ConditionId id) => new SecretConditionId(id);
}
public record SecretString(string Value) : Secret
{
    public override string ToString() => Value;
}
public record SecretConditionId(ConditionId Value) : Secret
{
    public override string ToString() => Value.ToString();
}

/// <summary>
/// string
/// </summary>
public abstract record Shared
{
    public static implicit operator Shared(string value) => new SharedString(value);
}
public record SharedString(string Value) : Shared
{
    public override string ToString() => Value;
}

/// <summary>
/// Represents the result of a side-specific content generation function.
/// Maps to TypeScript: { side: SideID, secret: string, shared: string }
/// </summary>
public record SideSecretSharedResult(SideId Side, Secret Secret, Shared Shared)
{
 /// <summary>
    /// Optional HP color indicator (used for opponent Pokemon HP display)
    /// </summary>
    public HpColor? HpColor { get; init; }
}
