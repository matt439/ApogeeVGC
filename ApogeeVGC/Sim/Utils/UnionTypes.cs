﻿using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils;

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
}
public record IntIntFalseUnion(int Value) : IntFalseUnion;
public record FalseIntFalseUnion : IntFalseUnion;


/// <summary>
/// CommonHandlers['ModifierSourceMove'] | -0.1
/// </summary>
public abstract record OnFractionalPriority
{
    public static implicit operator OnFractionalPriority(ModifierSourceMoveHandler function) =>
        new OnFractionalPriorityFunc(function);

    private const double Tolerance = 0.0001;

    public static implicit operator OnFractionalPriority(double value) =>
        Math.Abs(value - (-0.1)) < Tolerance
            ? new OnFrationalPriorityNeg(value)
            : throw new ArgumentException("Must be -0.1 for OnFractionalPriorityNeg");
}

public record OnFractionalPriorityFunc(ModifierSourceMoveHandler Function) : OnFractionalPriority;

public record OnFrationalPriorityNeg(double Value) : OnFractionalPriority;


/// <summary>
/// ((this: Battle, pokemon: Pokemon, source: null, move: ActiveMove) =&gt; boolean | void) | boolean
/// </summary>
public abstract record OnCriticalHit
{
    public static implicit operator OnCriticalHit(Func<IBattle, Pokemon, object?, Move, bool?> function) =>
        new OnCriticalHitFunc(function);
    public static implicit operator OnCriticalHit(bool value) => new OnCriticalHitBool(value);
}
public record OnCriticalHitFunc(Func<IBattle, Pokemon, object?, Move, bool?> Function) : OnCriticalHit;
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
/// Pokemon | Side | Battle
/// </summary>
public abstract record PokemonSideBattleUnion
{
    public static implicit operator PokemonSideBattleUnion(Pokemon pokemon) =>
        new PokemonSideBattlePokemon(pokemon);
    public static implicit operator PokemonSideBattleUnion(Side side) => new PokemonSideBattleSide(side);
    public static PokemonSideBattleUnion FromIBattle(IBattle battle) => new PokemonSideBattleBattle(battle);
}
public record PokemonSideBattlePokemon(Pokemon Pokemon) : PokemonSideBattleUnion;
public record PokemonSideBattleSide(Side Side) : PokemonSideBattleUnion;
public record PokemonSideBattleBattle(IBattle Battle) : PokemonSideBattleUnion;


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
    public static implicit operator OnFlinch(Func<IBattle, Pokemon, object?, Move, bool?> func) =>
        new OnFlinchFunc(func);
    public static implicit operator OnFlinch(bool value) => new OnFlinchBool(value);
}
public record OnFlinchFunc(Func<IBattle, Pokemon, object?, Move, bool?> Func) : OnFlinch;
public record OnFlinchBool(bool Value) : OnFlinch;


/// <summary>
/// ((this: Battle, pokemon: Pokemon, type: string) => boolean | void) | boolean
/// </summary>
public abstract record OnNegateImmunity
{
    public static implicit operator OnNegateImmunity(Func<IBattle, Pokemon, PokemonType, bool?> func) =>
        new OnNegateImmunityFunc(func);
    public static implicit operator OnNegateImmunity(bool value) => new OnNegateImmunityBool(value);
}
public record OnNegateImmunityFunc(Func<IBattle, Pokemon, PokemonType, bool?> Func) : OnNegateImmunity;
public record OnNegateImmunityBool(bool Value) : OnNegateImmunity;


/// <summary>
/// (this: Battle, item: Item, pokemon: Pokemon, source: Pokemon, move?: ActiveMove) => boolean | void) | boolean
/// </summary>
public abstract record OnTakeItem
{
    public static implicit operator OnTakeItem(Func<IBattle, Item, Pokemon, Pokemon, Move?, bool?> func) =>
        new OnTakeItemFunc(func);
    public static implicit operator OnTakeItem(bool value) => new OnTakeItemBool(value);
}
public record OnTakeItemFunc(Func<IBattle, Item, Pokemon, Pokemon, Move?, bool?> Func) : OnTakeItem;
public record OnTakeItemBool(bool Value) : OnTakeItem;




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
/// Pokemon | Effect | false
/// </summary>
public abstract record SingleEventSource
{
    public static implicit operator SingleEventSource(Pokemon pokemon) =>
        new PokemonSingleEventSource(pokemon);

    public static implicit operator SingleEventSource(Ability ability) =>
        EffectUnionFactory.ToSingleEventSource(ability);

    public static implicit operator SingleEventSource(Item item) =>
        EffectUnionFactory.ToSingleEventSource(item);

    public static implicit operator SingleEventSource(Move activeMove) =>
        EffectUnionFactory.ToSingleEventSource(activeMove);

    public static implicit operator SingleEventSource(Specie specie) =>
        EffectUnionFactory.ToSingleEventSource(specie);

    public static implicit operator SingleEventSource(Condition condition) =>
        EffectUnionFactory.ToSingleEventSource(condition);

    public static implicit operator SingleEventSource(Format format) =>
        EffectUnionFactory.ToSingleEventSource(format);

    public static SingleEventSource FromFalse() => new FalseSingleEventSource();
}
public record PokemonSingleEventSource(Pokemon Pokemon) : SingleEventSource;
public record EffectSingleEventSource(IEffect Effect) : SingleEventSource;
public record FalseSingleEventSource : SingleEventSource;


/// <summary>
/// bool | int
/// </summary>
public abstract record RelayVar
{
    public static implicit operator RelayVar(bool value) => new BoolRelayVar(value);
    public static implicit operator RelayVar(int value) => new IntRelayVar(value);
}
public record BoolRelayVar(bool Value) : RelayVar;
public record IntRelayVar(int Value) : RelayVar;



public static class EffectUnionFactory
{
    public static SingleEventSource ToSingleEventSource(IEffect effect) => effect switch
    {
        Ability ability => new EffectSingleEventSource(ability),
        Item item => new EffectSingleEventSource(item),
        Move activeMove => new EffectSingleEventSource(activeMove),
        Specie specie => new EffectSingleEventSource(specie),
        Condition condition => new EffectSingleEventSource(condition),
        Format format => new EffectSingleEventSource(format),
        _ => throw new InvalidOperationException($"Cannot convert {effect.GetType()} to SingleEventSource"),
    };
}



/// <summary>
/// Pokemon | Pokemon[] | Side | Battle
/// </summary>
public abstract record RunEventTarget
{
    public static implicit operator RunEventTarget(Pokemon pokemon) =>
        new PokemonRunEventTarget(pokemon);

    public static implicit operator RunEventTarget(Pokemon[] pokemonList) =>
        new PokemonArrayRunEventTarget(pokemonList);

    public static implicit operator RunEventTarget(Side side) => new SideRunEventTarget(side);
    public static RunEventTarget FromIBattle(IBattle battle) => new BattleRunEventTarget(battle);
}

public record PokemonRunEventTarget(Pokemon Pokemon) : RunEventTarget;
public record PokemonArrayRunEventTarget(Pokemon[] PokemonList) : RunEventTarget;
public record SideRunEventTarget(Side Side) : RunEventTarget;
public record BattleRunEventTarget(IBattle Battle) : RunEventTarget;






/// <summary>
/// pokemon | false
/// </summary>
public abstract record PokemonFalseUnion
{
    public static implicit operator PokemonFalseUnion(Pokemon pokemon) => new PokemonPokemonUnion(pokemon);

    public static PokemonFalseUnion FromFalse() => new FalsePokemonUnion();
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
    public static implicit operator MoveIdBoolUnion(MoveId moveId) => new MoveIdMoveIdBoolUnion(moveId);
    public static implicit operator MoveIdBoolUnion(bool value) => new BoolMoveIdBoolUnion(value);
}
public record MoveIdMoveIdBoolUnion(MoveId MoveId) : MoveIdBoolUnion;
public record BoolMoveIdBoolUnion(bool Value) : MoveIdBoolUnion;


/// <summary>
/// MoveType | false
/// </summary>
public abstract record MoveTypeFalseUnion
{
    public static implicit operator MoveTypeFalseUnion(PokemonType moveType) =>
        new MoveTypeMoveTypeFalseUnion(moveType);
    public static MoveTypeFalseUnion FromFalse() => new FalseMoveTypeFalseUnion();
}
public record MoveTypeMoveTypeFalseUnion(PokemonType MoveType) : MoveTypeFalseUnion;
public record FalseMoveTypeFalseUnion : MoveTypeFalseUnion;

///// <summary>
///// ((this: Battle, pokemon: Pokemon, source: null, move: ActiveMove) => boolean | void) | boolean
///// </summary>
//public abstract record OnCriticalHit
//{
//    public static implicit operator OnCriticalHit(Func<BattleContext, Pokemon, object?, Move, bool?> func) =>
//        new OnSourceCriticalHitFunc(func);
//    public static implicit operator OnCriticalHit(bool value) => new OnSourceCriticalHitBool(value);
//}
//public record OnSourceCriticalHitFunc(Func<BattleContext, Pokemon, object?, Move, bool?> Func) : OnCriticalHit;
//public record OnSourceCriticalHitBool(bool Value) : OnCriticalHit;




//[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
//public readonly struct EffectIdUnion : IEquatable<EffectIdUnion>
//{
//    [System.Runtime.InteropServices.FieldOffset(0)]
//    private readonly int _value;
//    [System.Runtime.InteropServices.FieldOffset(4)]
//    private readonly EffectType _type;

//    public EffectIdUnion(MoveId moveId)
//    {
//        _value = (int)moveId;
//        _type = EffectType.Move;
//    }

//    public EffectIdUnion(AbilityId abilityId)
//    {
//        _value = (int)abilityId;
//        _type = EffectType.Ability;
//    }

//    public EffectIdUnion(ItemId itemId)
//    {
//        _value = (int)itemId;
//        _type = EffectType.Item;
//    }

//    public EffectIdUnion(ConditionId conditionId)
//    {
//        _value = (int)conditionId;
//        _type = EffectType.Condition;
//    }

//    public EffectIdUnion(SpecieId specieId)
//    {
//        _value = (int)specieId;
//        _type = EffectType.Specie;
//    }

//    public override string ToString()
//    {
//        return _type switch
//        {
//            EffectType.Move => ((MoveId)_value).ToString(),
//            EffectType.Ability => ((AbilityId)_value).ToString(),
//            EffectType.Item => ((ItemId)_value).ToString(),
//            EffectType.Condition => ((ConditionId)_value).ToString(),
//            EffectType.Specie => ((SpecieId)_value).ToString(),
//            _ => _value.ToString(),
//        };
//    }

//    // Implicit conversions for ease of use
//    public static implicit operator EffectIdUnion(MoveId moveId) => new(moveId);
//    public static implicit operator EffectIdUnion(AbilityId abilityId) => new(abilityId);
//    public static implicit operator EffectIdUnion(ItemId itemId) => new(itemId);
//    public static implicit operator EffectIdUnion(ConditionId conditionId) => new(conditionId);
//    public static implicit operator EffectIdUnion(SpecieId specieId) => new(specieId);

//    public bool Equals(EffectIdUnion other) => _value == other._value && _type == other._type;
//    public override bool Equals(object? obj) => obj is EffectIdUnion other && Equals(other);
//    public override int GetHashCode() => HashCode.Combine(_value, _type);
//    public static bool operator ==(EffectIdUnion left, EffectIdUnion right)
//    {
//        return left.Equals(right);
//    }

//    public static bool operator !=(EffectIdUnion left, EffectIdUnion right)
//    {
//        return !(left == right);
//    }
//}