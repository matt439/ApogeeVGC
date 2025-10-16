using System.ComponentModel.DataAnnotations;
using System.Xml;
using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Utils;

/// <summary>
/// The 'void' type, representing the absence of a value.
/// </summary>
public record VoidReturn;

public record Undefined;

/// <summary>
/// int | bool
/// </summary>
public abstract record IntBoolUnion
{
    public static IntBoolUnion FromInt(int value) => new IntIntBoolUnion(value);
    public static IntBoolUnion FromBool(bool value) => new BoolIntBoolUnion(value);

    public static implicit operator IntBoolUnion(int value) => new IntIntBoolUnion(value);
    public static implicit operator IntBoolUnion(bool value) => new BoolIntBoolUnion(value);
}
public record IntIntBoolUnion(int Value) : IntBoolUnion;
public record BoolIntBoolUnion(bool Value) : IntBoolUnion;


/// <summary>
/// int | false
/// </summary>
public abstract record IntFalseUnion
{
    public static IntFalseUnion FromInt(int value) => new IntIntFalseUnion(value);
    public static IntFalseUnion FromFalse() => new FalseIntFalseUnion();

    public static implicit operator IntFalseUnion(int value) => new IntIntFalseUnion(value);

    /// <summary>
    /// Compares this IntFalseUnion to another.
    /// False is treated as having the lowest priority (comes last when sorting ascending).
    /// When both are integers, standard integer comparison is used.
    /// </summary>
    public int CompareTo(IntFalseUnion? other)
    {
        if (other == null) return 1;

        return (this, other) switch
        {
            // False < any integer (false has lower priority)
            (FalseIntFalseUnion, IntIntFalseUnion) => 1,  // this is false, other is int -> this > other
            (IntIntFalseUnion, FalseIntFalseUnion) => -1, // this is int, other is false -> this < other

            // Both are false - they're equal
            (FalseIntFalseUnion, FalseIntFalseUnion) => 0,

            // Both are integers - compare the values
            (IntIntFalseUnion thisInt, IntIntFalseUnion otherInt) =>
                thisInt.Value.CompareTo(otherInt.Value),

            _ => 0,
        };
    }
}
public record IntIntFalseUnion(int Value) : IntFalseUnion;
public record FalseIntFalseUnion : IntFalseUnion;



/// <summary>
/// int | true
/// </summary>
public abstract record IntTrueUnion
{
    public static IntTrueUnion FromInt(int value) => new IntIntTrueUnion(value);
    public static IntTrueUnion FromTrue() => new TrueIntTrueUnion();
    public static implicit operator IntTrueUnion(int value) => new IntIntTrueUnion(value);
}
public record IntIntTrueUnion(int Value) : IntTrueUnion;
public record TrueIntTrueUnion : IntTrueUnion;


/// <summary>
/// double | void
/// </summary>
public abstract record DoubleVoidUnion
{
    public static DoubleVoidUnion FromDouble(double value) => new DoubleDoubleVoidUnion(value);
    public static DoubleVoidUnion FromVoid() => new VoidDoubleVoidUnion(new VoidReturn());
    public static implicit operator DoubleVoidUnion(double value) => new DoubleDoubleVoidUnion(value);
    public static implicit operator DoubleVoidUnion(VoidReturn value) => new VoidDoubleVoidUnion(value);
}
public record DoubleDoubleVoidUnion(double Value) : DoubleVoidUnion;
public record VoidDoubleVoidUnion(VoidReturn Value) : DoubleVoidUnion;


/// <summary>
/// int | void
/// </summary>
public abstract record IntVoidUnion
{
    public static IntVoidUnion FromInt(int value) => new IntIntVoidUnion(value);
    public static IntVoidUnion FromVoid() => new VoidIntVoidUnion(new VoidReturn());
    public static implicit operator IntVoidUnion(int value) => new IntIntVoidUnion(value);
    public static implicit operator IntVoidUnion(VoidReturn value) => new VoidIntVoidUnion(value);
}
public record IntIntVoidUnion(int Value) : IntVoidUnion;
public record VoidIntVoidUnion(VoidReturn Value) : IntVoidUnion;


/// <summary>
/// bool | undefined | void
/// </summary>
public abstract record BoolUndefinedVoidUnion
{
    public static BoolUndefinedVoidUnion FromBool(bool value) => new BoolBoolUndefinedVoidUnion(value);
    public static BoolUndefinedVoidUnion FromUndefined() => new UndefinedBoolUndefinedVoidUnion(new Undefined());
    public static BoolUndefinedVoidUnion FromVoid() => new VoidUnionBoolUndefinedVoidUnion(new VoidReturn());
    public static implicit operator BoolUndefinedVoidUnion(bool value) => new BoolBoolUndefinedVoidUnion(value);
    public static implicit operator BoolUndefinedVoidUnion(VoidReturn value) =>
        new VoidUnionBoolUndefinedVoidUnion(value);

    public static implicit operator BoolUndefinedVoidUnion(Undefined value) =>
        new UndefinedBoolUndefinedVoidUnion(value);
}
public record BoolBoolUndefinedVoidUnion(bool Value) : BoolUndefinedVoidUnion;
public record UndefinedBoolUndefinedVoidUnion(Undefined Value) : BoolUndefinedVoidUnion;
public record VoidUnionBoolUndefinedVoidUnion(VoidReturn Value) : BoolUndefinedVoidUnion;


/// <summary>
/// bool | int | undefined | void
/// </summary>
public abstract record BoolIntUndefinedVoidUnion
{
    public static BoolIntUndefinedVoidUnion FromBool(bool value) => new BoolBoolIntUndefinedVoidUnion(value);
    public static BoolIntUndefinedVoidUnion FromInt(int value) => new IntBoolIntUndefinedVoidUnion(value);
    public static BoolIntUndefinedVoidUnion FromUndefined() =>
        new UndefinedBoolIntUndefinedVoidUnion(new Undefined());
    public static BoolIntUndefinedVoidUnion FromVoid() => new VoidUnionBoolIntUndefinedVoidUnion(new VoidReturn());
    public static implicit operator BoolIntUndefinedVoidUnion(bool value) => new BoolBoolIntUndefinedVoidUnion(value);
    public static implicit operator BoolIntUndefinedVoidUnion(int value) => new IntBoolIntUndefinedVoidUnion(value);
    public static implicit operator BoolIntUndefinedVoidUnion(VoidReturn value) =>
        new VoidUnionBoolIntUndefinedVoidUnion(value);
    public static implicit operator BoolIntUndefinedVoidUnion(Undefined value) =>
        new UndefinedBoolIntUndefinedVoidUnion(value);
}
public record BoolBoolIntUndefinedVoidUnion(bool Value) : BoolIntUndefinedVoidUnion;
public record IntBoolIntUndefinedVoidUnion(int Value) : BoolIntUndefinedVoidUnion;
public record UndefinedBoolIntUndefinedVoidUnion(Undefined Value) : BoolIntUndefinedVoidUnion;
public record VoidUnionBoolIntUndefinedVoidUnion(VoidReturn Value) : BoolIntUndefinedVoidUnion;


/// <summary>
/// bool | void
/// </summary>
public abstract record BoolVoidUnion
{
    public static BoolVoidUnion FromBool(bool value) => new BoolBoolVoidUnion(value);
    public static BoolVoidUnion FromVoid() => new VoidBoolVoidUnion(new VoidReturn());
    public static implicit operator BoolVoidUnion(bool value) => new BoolBoolVoidUnion(value);
    public static implicit operator BoolVoidUnion(VoidReturn value) => new VoidBoolVoidUnion(value);
}
public record BoolBoolVoidUnion(bool Value) : BoolVoidUnion;
public record VoidBoolVoidUnion(VoidReturn Value) : BoolVoidUnion;


/// <summary>
/// int | bool | void
/// </summary>
public abstract record IntBoolVoidUnion
{
    public static IntBoolVoidUnion FromInt(int value) => new IntIntBoolVoidUnion(value);
    public static IntBoolVoidUnion FromBool(bool value) => new BoolIntBoolVoidUnion(value);
    public static IntBoolVoidUnion FromVoid() => new VoidIntBoolVoidUnion(new VoidReturn());
    public static implicit operator IntBoolVoidUnion(int value) => new IntIntBoolVoidUnion(value);
    public static implicit operator IntBoolVoidUnion(bool value) => new BoolIntBoolVoidUnion(value);
    public static implicit operator IntBoolVoidUnion(VoidReturn value) => new VoidIntBoolVoidUnion(value);
}
public record IntIntBoolVoidUnion(int Value) : IntBoolVoidUnion;
public record BoolIntBoolVoidUnion(bool Value) : IntBoolVoidUnion;
public record VoidIntBoolVoidUnion(VoidReturn Value) : IntBoolVoidUnion;




/// <summary>
/// CommonHandlers['ModifierSourceMove'] | -0.1
/// </summary>
public abstract record OnFractionalPriority
{
    public static implicit operator OnFractionalPriority(ModifierSourceMoveHandler function) =>
        new OnFractionalPriorityFunc(function);

    private static readonly decimal PriorityValue = new(-0.1);

    public static implicit operator OnFractionalPriority(decimal value) =>
        value == PriorityValue
            ? new OnFrationalPriorityNeg(value)
            : throw new ArgumentException("Must be -0.1 for OnFractionalPriorityNeg");
}
public record OnFractionalPriorityFunc(ModifierSourceMoveHandler Function) : OnFractionalPriority;
public record OnFrationalPriorityNeg(decimal Value) : OnFractionalPriority;


/// <summary>
/// ((this: Battle, pokemon: Pokemon, source: null, move: ActiveMove) =&gt; boolean | void) | boolean
/// </summary>
public abstract record OnCriticalHit
{
    public static implicit operator OnCriticalHit(Func<IBattle, Pokemon, object?, Move, BoolVoidUnion> function) =>
        new OnCriticalHitFunc(function);
    public static implicit operator OnCriticalHit(bool value) => new OnCriticalHitBool(value);
}
public record OnCriticalHitFunc(Func<IBattle, Pokemon, object?, Move, BoolVoidUnion> Function) : OnCriticalHit;
public record OnCriticalHitBool(bool Value) : OnCriticalHit;


/// <summary>
/// Pokemon | Side
/// </summary>
public abstract record PokemonSideUnion
{
    public static implicit operator PokemonSideUnion(Pokemon pokemon) => new PokemonSidePokemon(pokemon);
    public static implicit operator PokemonSideUnion(Side side) => new PokemonSideSide(side);
}
public record PokemonSidePokemon(Pokemon Pokemon) : PokemonSideUnion;
public record PokemonSideSide(Side Side) : PokemonSideUnion;

/// <summary>
/// Pokemon | Side | Field
/// </summary>
public abstract record PokemonSideFieldUnion
{
    public static implicit operator PokemonSideFieldUnion(Pokemon pokemon) => new PokemonSideFieldPokemon(pokemon);
    public static implicit operator PokemonSideFieldUnion(Side side) => new PokemonSideFieldSide(side);
    public static implicit operator PokemonSideFieldUnion(Field field) => new PokemonSideFieldField(field);
}
public record PokemonSideFieldPokemon(Pokemon Pokemon) : PokemonSideFieldUnion;
public record PokemonSideFieldSide(Side Side) : PokemonSideFieldUnion;
public record PokemonSideFieldField(Field Field) : PokemonSideFieldUnion;


/// <summary>
/// Pokemon | Side | Field | Battle
/// </summary>
public abstract record PokemonSideFieldBattleUnion
{
    public static implicit operator PokemonSideFieldBattleUnion(Pokemon pokemon) =>
        new PokemonSideFieldBattlePokemon(pokemon);
    public static implicit operator PokemonSideFieldBattleUnion(Side side) =>
        new PokemonSideFieldBattleSide(side);
    public static implicit operator PokemonSideFieldBattleUnion(Field field) =>
        new PokemonSideFieldBattleField(field);
    public static PokemonSideFieldBattleUnion FromIBattle(IBattle battle) =>
        new PokemonSideFieldBattleBattle(battle);
}
public record PokemonSideFieldBattlePokemon(Pokemon Pokemon) : PokemonSideFieldBattleUnion;
public record PokemonSideFieldBattleSide(Side Side) : PokemonSideFieldBattleUnion;
public record PokemonSideFieldBattleField(Field Field) : PokemonSideFieldBattleUnion;
public record PokemonSideFieldBattleBattle(IBattle Battle) : PokemonSideFieldBattleUnion;


/// <summary>
/// Pokemon | Side | Battle | Pokemon?
/// </summary>
public abstract record PokemonSideBattleUnion
{
    public static implicit operator PokemonSideBattleUnion(Pokemon pokemon) =>
        new PokemonSideBattlePokemon(pokemon);
    public static implicit operator PokemonSideBattleUnion(Side side) => new PokemonSideBattleSide(side);
    public static PokemonSideBattleUnion FromIBattle(IBattle battle) => new PokemonSideBattleBattle(battle);
    public static PokemonSideBattleUnion? FromNullablePokemon(Pokemon? pokemon)
    {
        return pokemon is null ? null : new PokemonSideBattleNullablePokemon(pokemon);
    }

    public static PokemonSideBattleUnion? FromNullableSingleEventTarget(SingleEventTarget? target)
    {
        return target switch
        {
            null => null,
            PokemonSingleEventTarget pokemon => new PokemonSideBattlePokemon(pokemon.Pokemon),
            SideSingleEventTarget side => new PokemonSideBattleSide(side.Side),
            BattleSingleEventTarget battle => new PokemonSideBattleBattle(battle.Battle),
            _ => throw new InvalidOperationException("Cannot convert to PokemonSideBattleUnion"),
        };
    }
}
public record PokemonSideBattlePokemon(Pokemon Pokemon) : PokemonSideBattleUnion;
public record PokemonSideBattleSide(Side Side) : PokemonSideBattleUnion;
public record PokemonSideBattleBattle(IBattle Battle) : PokemonSideBattleUnion;
public record PokemonSideBattleNullablePokemon(Pokemon? Pokemon) : PokemonSideBattleUnion;


/// <summary>
/// ((this: Battle, relayVar: number, target: Pokemon, source: Pokemon, effect: Effect) => number | boolean | void)
/// | ((this: Battle, pokemon: Pokemon) => boolean | void)
/// | boolean
/// </summary>
public abstract record OnTryHeal
{
    public static implicit operator OnTryHeal(
        Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?> func) =>
        new OnTryHealFunc1(func);
    public static implicit operator OnTryHeal(Func<IBattle, Pokemon, bool?> func) =>
        new OnTryHealFunc2(func);
    public static implicit operator OnTryHeal(bool value) => new OnTryHealBool(value);
}
public record OnTryHealFunc1(Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?> Func) :
    OnTryHeal;
public record OnTryHealFunc2(Func<IBattle, Pokemon, bool?> Func) : OnTryHeal;
public record OnTryHealBool(bool Value) : OnTryHeal;


/// <summary>
/// ((this: Battle, pokemon: Pokemon, source: null, move: ActiveMove) => boolean | void) | boolean
/// </summary>
public abstract record OnFlinch
{
    public static implicit operator OnFlinch(Func<IBattle, Pokemon, object?, Move, BoolVoidUnion> func) =>
        new OnFlinchFunc(func);
    public static implicit operator OnFlinch(bool value) => new OnFlinchBool(value);
}
public record OnFlinchFunc(Func<IBattle, Pokemon, object?, Move, BoolVoidUnion> Func) : OnFlinch;
public record OnFlinchBool(bool Value) : OnFlinch;



/// <summary>
/// string | ((this: Battle, pokemon: Pokemon) => void | string)    
/// </summary>
public abstract record OnLockMove
{
    public static implicit operator OnLockMove(MoveId moveId) => new OnLockMoveMoveId(moveId);
    public static implicit operator OnLockMove(Func<IBattle, Pokemon, MoveIdVoidUnion> func) =>
        new OnLockMoveFunc(func);
}
public record OnLockMoveMoveId(MoveId Id) : OnLockMove;
public record OnLockMoveFunc(Func<IBattle, Pokemon, MoveIdVoidUnion> Func) : OnLockMove;


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
/// MoveId | void
/// </summary>
public abstract record MoveIdVoidUnion
{
    public static implicit operator MoveIdVoidUnion(MoveId moveId) => new MoveIdMoveIdVoidUnion(moveId);
    public static implicit operator MoveIdVoidUnion(VoidReturn value) => new VoidMoveIdVoidUnion(value);
    public static MoveIdVoidUnion FromVoid() => new VoidMoveIdVoidUnion(new VoidReturn());
}
public record MoveIdMoveIdVoidUnion(MoveId MoveId) : MoveIdVoidUnion;
public record VoidMoveIdVoidUnion(VoidReturn Value) : MoveIdVoidUnion;


/// <summary>
/// ((this: Battle, pokemon: Pokemon, type: string) => boolean | void) | boolean
/// </summary>
public abstract record OnNegateImmunity
{
    public static implicit operator OnNegateImmunity(Func<IBattle, Pokemon, PokemonType, BoolVoidUnion> func) =>
        new OnNegateImmunityFunc(func);
    public static implicit operator OnNegateImmunity(bool value) => new OnNegateImmunityBool(value);
}
public record OnNegateImmunityFunc(Func<IBattle, Pokemon, PokemonType, BoolVoidUnion> Func) : OnNegateImmunity;
public record OnNegateImmunityBool(bool Value) : OnNegateImmunity;


/// <summary>
/// Delegate | void
/// </summary>
public abstract record DelegateVoidUnion
{
    public static implicit operator DelegateVoidUnion(Delegate del) => new DelegateDelegateVoidUnion(del);
    public static implicit operator DelegateVoidUnion(VoidReturn value) => new VoidDelegateVoidUnion(value);
    public static DelegateVoidUnion FromVoid() => new VoidDelegateVoidUnion(new VoidReturn());
}
public record DelegateDelegateVoidUnion(Delegate Del) : DelegateVoidUnion;
public record VoidDelegateVoidUnion(VoidReturn Value) : DelegateVoidUnion;


/// <summary>
/// Pokemon | void
/// </summary>
public abstract record PokemonVoidUnion
{
    public static implicit operator PokemonVoidUnion(Pokemon pokemon) => new PokemonPokemonVoidUnion(pokemon);
    public static implicit operator PokemonVoidUnion(VoidReturn value) => new VoidPokemonVoidUnion(value);
    public static PokemonVoidUnion FromVoid() => new VoidPokemonVoidUnion(new VoidReturn());
}
public record PokemonPokemonVoidUnion(Pokemon Pokemon) : PokemonVoidUnion;
public record VoidPokemonVoidUnion(VoidReturn Value) : PokemonVoidUnion;


/// <summary>
/// (this: Battle, item: Item, pokemon: Pokemon, source: Pokemon, move?: ActiveMove) => boolean | void) | boolean
/// </summary>
public abstract record OnTakeItem
{
    public static implicit operator OnTakeItem(Func<IBattle, Item, Pokemon, Pokemon, Move?, PokemonVoidUnion> func)
        => new OnTakeItemFunc(func);
    public static implicit operator OnTakeItem(bool value) => new OnTakeItemBool(value);
}
public record OnTakeItemFunc(Func<IBattle, Item, Pokemon, Pokemon, Move?, PokemonVoidUnion> Func) : OnTakeItem;
public record OnTakeItemBool(bool Value) : OnTakeItem;


/// <summary>
/// PokemonType[] | void
/// </summary>
public abstract record TypesVoidUnion
{
    public static implicit operator TypesVoidUnion(PokemonType[] types) => new TypesTypesVoidUnion(types);
    public static implicit operator TypesVoidUnion(VoidReturn value) => new VoidTypesVoidUnion(value);
    public static TypesVoidUnion FromVoid() => new VoidTypesVoidUnion(new VoidReturn());
}
public record TypesTypesVoidUnion(PokemonType[] Types) : TypesVoidUnion;
public record VoidTypesVoidUnion(VoidReturn Value) : TypesVoidUnion;



/// <summary>
/// Pokemon | Side | Field | Battle
/// </summary>
public abstract record SingleEventTarget
{
    public static implicit operator SingleEventTarget(Pokemon pokemon) =>
        new PokemonSingleEventTarget(pokemon);

    public static implicit operator SingleEventTarget(Side side) => new SideSingleEventTarget(side);
    public static implicit operator SingleEventTarget(Field field) => new FieldSingleEventTarget(field);

    public static SingleEventTarget FromIBattle(IBattle battle) => new BattleSingleEventTarget(battle);
}
public record PokemonSingleEventTarget(Pokemon Pokemon) : SingleEventTarget;
public record SideSingleEventTarget(Side Side) : SingleEventTarget;
public record FieldSingleEventTarget(Field Field) : SingleEventTarget;
public record BattleSingleEventTarget(IBattle Battle) : SingleEventTarget;



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
/// Pokemon | Pokemon? | false | Type
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
/// SparseBoostsTable | decimal | MoveId | string | RelayVar[]
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



/// <summary>
/// AbilityId | false
/// </summary>
public abstract record AbilityIdFalseUnion
{
    public static implicit operator AbilityIdFalseUnion(AbilityId abilityId) =>
        new AbilityIdAbilityIdFalseUnion(abilityId);
    public static AbilityIdFalseUnion FromFalse() => new FalseAbilityIdFalseUnion();
}
public record AbilityIdAbilityIdFalseUnion(AbilityId AbilityId) : AbilityIdFalseUnion;
public record FalseAbilityIdFalseUnion : AbilityIdFalseUnion;


/// <summary>
/// MoveAction | SwitchAction
/// </summary>
public abstract record MoveSwitchActionUnion
{
    public static implicit operator MoveSwitchActionUnion(MoveAction moveAction) =>
        new MoveActionMoveSwitchActionUnion(moveAction);
    public static implicit operator MoveSwitchActionUnion(SwitchAction switchAction) =>
        new SwitchActionMoveSwitchActionUnion(switchAction);
}
public record MoveActionMoveSwitchActionUnion(MoveAction MoveAction) : MoveSwitchActionUnion;
public record SwitchActionMoveSwitchActionUnion(SwitchAction SwitchAction) : MoveSwitchActionUnion;


/// <summary>
/// MoveAction | SwitchAction | TeamAction | PokemonAction
/// </summary>
public abstract record MoveSwitchTeamPokemonActionUnion
{
    public static implicit operator MoveSwitchTeamPokemonActionUnion(MoveAction moveAction) =>
        new MoveActionMoveSwitchTeamPokemonActionUnion(moveAction);
    public static implicit operator MoveSwitchTeamPokemonActionUnion(SwitchAction switchAction) =>
        new SwitchActionMoveSwitchTeamPokemonActionUnion(switchAction);
    public static implicit operator MoveSwitchTeamPokemonActionUnion(TeamAction teamAction) =>
        new TeamActionMoveSwitchTeamPokemonActionUnion(teamAction);
    public static implicit operator MoveSwitchTeamPokemonActionUnion(PokemonAction pokemonAction) =>
        new PokemonActionMoveSwitchTeamPokemonActionUnion(pokemonAction);
}
public record MoveActionMoveSwitchTeamPokemonActionUnion(MoveAction MoveAction) :
    MoveSwitchTeamPokemonActionUnion;
public record SwitchActionMoveSwitchTeamPokemonActionUnion(SwitchAction SwitchAction) :
    MoveSwitchTeamPokemonActionUnion;
public record TeamActionMoveSwitchTeamPokemonActionUnion(TeamAction TeamAction) :
    MoveSwitchTeamPokemonActionUnion;
public record PokemonActionMoveSwitchTeamPokemonActionUnion(PokemonAction PokemonAction) :
    MoveSwitchTeamPokemonActionUnion;


public static class EffectUnionFactory
{
    public static SingleEventSource ToSingleEventSource(IEffect effect) => effect switch
    {
        Ability ability => new EffectSingleEventSource(ability),
        Item item => new EffectSingleEventSource(item),
        ActiveMove activeMove => new EffectSingleEventSource(activeMove),
        Species specie => new EffectSingleEventSource(specie),
        Condition condition => new EffectSingleEventSource(condition),
        Format format => new EffectSingleEventSource(format),
        _ => throw new InvalidOperationException($"Cannot convert {effect.GetType()} to SingleEventSource"),
    };

    public static RelayVar ToRelayVar(IEffect effect) => effect switch
    {
        Ability ability => new EffectRelayVar(ability),
        Item item => new EffectRelayVar(item),
        ActiveMove activeMove => new EffectRelayVar(activeMove),
        Species specie => new SpecieRelayVar(specie),
        Condition condition => new EffectRelayVar(condition),
        Format format => new EffectRelayVar(format),
        _ => throw new InvalidOperationException($"Cannot convert {effect.GetType()} to RelayVar"),
    };
}



/// <summary>
/// Pokemon | Pokemon[] | Side | Battle | PokemonSideBattleUnion? | Field
/// </summary>
public abstract record RunEventTarget
{
    public static implicit operator RunEventTarget(Pokemon pokemon) =>
        new PokemonRunEventTarget(pokemon);

    public static implicit operator RunEventTarget(Pokemon[] pokemonList) =>
        new PokemonArrayRunEventTarget(pokemonList);

    public static implicit operator RunEventTarget(Side side) => new SideRunEventTarget(side);
    public static RunEventTarget FromIBattle(IBattle battle) => new BattleRunEventTarget(battle);
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
    public static implicit operator RunEventTarget(Field field) => new FieldRunEventTarget(field);
}

public record PokemonRunEventTarget(Pokemon Pokemon) : RunEventTarget;
public record PokemonArrayRunEventTarget(Pokemon[] PokemonList) : RunEventTarget;
public record SideRunEventTarget(Side Side) : RunEventTarget;
public record BattleRunEventTarget(IBattle Battle) : RunEventTarget;
public record FieldRunEventTarget(Field Field) : RunEventTarget;







/// <summary>
/// pokemon | false
/// </summary>
public abstract record PokemonFalseUnion
{
    public static implicit operator PokemonFalseUnion(Pokemon pokemon) => new PokemonPokemonUnion(pokemon);
    public static PokemonFalseUnion FromFalse() => new FalsePokemonUnion();

    public static PokemonFalseUnion? FromNullablePokemon(Pokemon? pokemon)
    {
        return pokemon is null ? null : new PokemonPokemonUnion(pokemon);
    }
}
public record PokemonPokemonUnion(Pokemon Pokemon) : PokemonFalseUnion;
public record FalsePokemonUnion : PokemonFalseUnion;



/// <summary>
/// bool| 0
/// </summary>
public abstract record BoolZeroUnion
{
    public static implicit operator BoolZeroUnion(bool value) => new BoolBoolZeroUnion(value);
    public static BoolZeroUnion FromZero() => new ZeroBoolZeroUnion();
}
public record BoolBoolZeroUnion(bool Value) : BoolZeroUnion;
public record ZeroBoolZeroUnion : BoolZeroUnion;


/// <summary>
/// MoveId | bool
/// </summary>
public abstract record MoveIdBoolUnion
{
    public abstract bool IsTrue();
    public static implicit operator MoveIdBoolUnion(MoveId moveId) => new MoveIdMoveIdBoolUnion(moveId);
    public static implicit operator MoveIdBoolUnion(bool value) => new BoolMoveIdBoolUnion(value);
}
public record MoveIdMoveIdBoolUnion(MoveId MoveId) : MoveIdBoolUnion
{
    public override bool IsTrue() => true;
}
public record BoolMoveIdBoolUnion(bool Value) : MoveIdBoolUnion
{
    public override bool IsTrue() => Value;
}



/// <summary>
/// (int | bool | undefined)[]
/// </summary>
public class SpreadMoveDamage : List<IntBoolUnion?>
{
}


/// <summary>
/// MoveType | false
/// </summary>
public abstract record MoveTypeFalseUnion
{
    public static implicit operator MoveTypeFalseUnion(MoveType moveType) =>
        new MoveTypeMoveTypeFalseUnion(moveType);
    public static MoveTypeFalseUnion FromFalse() => new FalseMoveTypeFalseUnion();
}
public record MoveTypeMoveTypeFalseUnion(MoveType MoveType) : MoveTypeFalseUnion;
public record FalseMoveTypeFalseUnion : MoveTypeFalseUnion;


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
/// Pokemon | Side | Field | IBattle
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
            BattleSingleEventTarget b when expectedType.IsAssignableFrom(typeof(IBattle)) => 
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
public record BattleEventTargetParameter(IBattle Battle) : EventTargetParameter;



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
public abstract record OnItemEatUse
{
    public static implicit operator OnItemEatUse(Action<IBattle, Pokemon> func) =>
        new OnItemEatUseFunc(func);
    public static OnItemEatUse FromFalse() => new OnItemEatUseFalse();
}
public record OnItemEatUseFunc(Action<IBattle, Pokemon> Func) : OnItemEatUse;
public record OnItemEatUseFalse : OnItemEatUse;


/// <summary>
/// bool | ((this: Battle, item: Item, pokemon: Pokemon) => boolean | void)
/// </summary>
public abstract record OnTryEatItem
{
    public static implicit operator OnTryEatItem(bool value) => new BoolOnTryEatItem(value);
    public static implicit operator OnTryEatItem(Func<IBattle, Item, Pokemon, BoolVoidUnion> func) =>
        new FuncOnTryEatItem(func);
}
public record BoolOnTryEatItem(bool Value) : OnTryEatItem;
public record FuncOnTryEatItem(Func<IBattle, Item, Pokemon, BoolVoidUnion> Func) : OnTryEatItem;



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
/// int | int[]
/// </summary>
public abstract record IntIntArrayUnion
{
    public static implicit operator IntIntArrayUnion(int value) => new IntIntIntArrayUnion(value);
    public static implicit operator IntIntArrayUnion(int[] values) => new IntArrayIntIntArrayUnion(values);
}
public record IntIntIntArrayUnion(int Value) : IntIntArrayUnion;
public record IntArrayIntIntArrayUnion(int[] Values) : IntIntArrayUnion;



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
/// bool | 'hidden'
/// </summary>
public abstract record BoolHiddenUnion
{
    public static implicit operator BoolHiddenUnion(bool value) => new BoolBoolHiddenUnion(value);
    public static BoolHiddenUnion FromHidden() => new HiddenBoolHiddenUnion();
    
    /// <summary>
    /// Returns true if this union represents a truthy value (true or 'hidden').
    /// Used to check if a move is effectively disabled.
    /// </summary>
    public bool IsTruthy() => this switch
    {
        BoolBoolHiddenUnion { Value: true } => true,
        HiddenBoolHiddenUnion => true,
        _ => false,
    };
    
    /// <summary>
    /// Returns true if this union is explicitly the boolean value true.
    /// </summary>
    public bool IsTrue() => this is BoolBoolHiddenUnion { Value: true };
}
public record BoolBoolHiddenUnion(bool Value) : BoolHiddenUnion;
public record HiddenBoolHiddenUnion : BoolHiddenUnion;


/// <summary>
/// Pokemon | Side | Field | Battle
/// </summary>
public abstract record EffectStateTarget
{
    public static implicit operator EffectStateTarget(Pokemon pokemon) => new PokemonEffectStateTarget(pokemon);
    public static implicit operator EffectStateTarget(Side side) => new SideEffectStateTarget(side);
    public static implicit operator EffectStateTarget(Field field) => new FieldEffectStateTarget(field);
    public static EffectStateTarget FromIBattle(IBattle battle) => new BattleEffectStateTarget(battle);
}
public record PokemonEffectStateTarget(Pokemon Pokemon) : EffectStateTarget;
public record SideEffectStateTarget(Side Side) : EffectStateTarget;
public record FieldEffectStateTarget(Field Field) : EffectStateTarget;
public record BattleEffectStateTarget(IBattle Battle) : EffectStateTarget;



/// <summary>
/// Pokemon | Side | Field | Battle
/// </summary>
public abstract record EffectHolder
{
    public static implicit operator EffectHolder(Pokemon pokemon) => new PokemonEffectHolder(pokemon);
    public static implicit operator EffectHolder(Side side) => new SideEffectHolder(side);
    public static implicit operator EffectHolder(Field field) => new FieldEffectHolder(field);
    public static EffectHolder FromIBattle(IBattle battle) => new BattleEffectHolder(battle);
}
public record PokemonEffectHolder(Pokemon Pokemon) : EffectHolder;
public record SideEffectHolder(Side Side) : EffectHolder;
public record FieldEffectHolder(Field Field) : EffectHolder;
public record BattleEffectHolder(IBattle Battle) : EffectHolder;



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
    public override Delegate? GetDelegate(int index = 0) => Del;
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



