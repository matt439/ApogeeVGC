using ApogeeVGC.Sim.Core;
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
    public static implicit operator OnCriticalHit(Func<BattleContext, Pokemon, object?, Move, bool?> function) =>
        new OnCriticalHitFunc(function);
    public static implicit operator OnCriticalHit(bool value) => new OnCriticalHitBool(value);
}
public record OnCriticalHitFunc(Func<BattleContext, Pokemon, object?, Move, bool?> Function) : OnCriticalHit;
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
/// ((this: Battle, relayVar: number, target: Pokemon, source: Pokemon, effect: Effect) => number | boolean | void)
/// | ((this: Battle, pokemon: Pokemon) => boolean | void)
/// | boolean
/// </summary>
public abstract record OnTryHeal
{
    public static implicit operator OnTryHeal(
        Func<BattleContext, int, Pokemon, Pokemon, IEffect, IntBoolUnion?> func) =>
        new OnTryHealFunc1(func);
    public static implicit operator OnTryHeal(Func<BattleContext, Pokemon, bool?> func) =>
        new OnTryHealFunc2(func);
    public static implicit operator OnTryHeal(bool value) => new OnTryHealBool(value);
}
public record OnTryHealFunc1(Func<BattleContext, int, Pokemon, Pokemon, IEffect, IntBoolUnion?> Func) :
    OnTryHeal;
public record OnTryHealFunc2(Func<BattleContext, Pokemon, bool?> Func) : OnTryHeal;
public record OnTryHealBool(bool Value) : OnTryHeal;


/// <summary>
/// ((this: Battle, pokemon: Pokemon, source: null, move: ActiveMove) => boolean | void) | boolean
/// </summary>
public abstract record OnFlinch
{
    public static implicit operator OnFlinch(Func<BattleContext, Pokemon, object?, Move, bool?> func) =>
        new OnFlinchFunc(func);
    public static implicit operator OnFlinch(bool value) => new OnFlinchBool(value);
}
public record OnFlinchFunc(Func<BattleContext, Pokemon, object?, Move, bool?> Func) : OnFlinch;
public record OnFlinchBool(bool Value) : OnFlinch;


/// <summary>
/// ((this: Battle, pokemon: Pokemon, type: string) => boolean | void) | boolean
/// </summary>
public abstract record OnNegateImmunity
{
    public static implicit operator OnNegateImmunity(Func<BattleContext, Pokemon, PokemonType, bool?> func) =>
        new OnNegateImmunityFunc(func);
    public static implicit operator OnNegateImmunity(bool value) => new OnNegateImmunityBool(value);
}
public record OnNegateImmunityFunc(Func<BattleContext, Pokemon, PokemonType, bool?> Func) : OnNegateImmunity;
public record OnNegateImmunityBool(bool Value) : OnNegateImmunity;


/// <summary>
/// (this: Battle, item: Item, pokemon: Pokemon, source: Pokemon, move?: ActiveMove) => boolean | void) | boolean
/// </summary>
public abstract record OnTakeItem
{
    public static implicit operator OnTakeItem(Func<BattleContext, Item, Pokemon, Pokemon, Move?, bool?> func) =>
        new OnTakeItemFunc(func);
    public static implicit operator OnTakeItem(bool value) => new OnTakeItemBool(value);
}
public record OnTakeItemFunc(Func<BattleContext, Item, Pokemon, Pokemon, Move?, bool?> Func) : OnTakeItem;
public record OnTakeItemBool(bool Value) : OnTakeItem;







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