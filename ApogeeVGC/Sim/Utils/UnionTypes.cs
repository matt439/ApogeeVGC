using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;

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


[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
public readonly struct EffectIdUnion : IEquatable<EffectIdUnion>
{
    [System.Runtime.InteropServices.FieldOffset(0)]
    private readonly int _value;
    [System.Runtime.InteropServices.FieldOffset(4)]
    private readonly EffectType _type;

    public EffectIdUnion(MoveId moveId)
    {
        _value = (int)moveId;
        _type = EffectType.Move;
    }

    public EffectIdUnion(AbilityId abilityId)
    {
        _value = (int)abilityId;
        _type = EffectType.Ability;
    }

    public EffectIdUnion(ItemId itemId)
    {
        _value = (int)itemId;
        _type = EffectType.Item;
    }

    public EffectIdUnion(ConditionId conditionId)
    {
        _value = (int)conditionId;
        _type = EffectType.Condition;
    }

    public EffectIdUnion(SpecieId specieId)
    {
        _value = (int)specieId;
        _type = EffectType.Specie;
    }

    public override string ToString()
    {
        return _type switch
        {
            EffectType.Move => ((MoveId)_value).ToString(),
            EffectType.Ability => ((AbilityId)_value).ToString(),
            EffectType.Item => ((ItemId)_value).ToString(),
            EffectType.Condition => ((ConditionId)_value).ToString(),
            EffectType.Specie => ((SpecieId)_value).ToString(),
            _ => _value.ToString(),
        };
    }

    // Implicit conversions for ease of use
    public static implicit operator EffectIdUnion(MoveId moveId) => new(moveId);
    public static implicit operator EffectIdUnion(AbilityId abilityId) => new(abilityId);
    public static implicit operator EffectIdUnion(ItemId itemId) => new(itemId);
    public static implicit operator EffectIdUnion(ConditionId conditionId) => new(conditionId);
    public static implicit operator EffectIdUnion(SpecieId specieId) => new(specieId);

    public bool Equals(EffectIdUnion other) => _value == other._value && _type == other._type;
    public override bool Equals(object? obj) => obj is EffectIdUnion other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(_value, _type);
    public static bool operator ==(EffectIdUnion left, EffectIdUnion right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EffectIdUnion left, EffectIdUnion right)
    {
        return !(left == right);
    }
}