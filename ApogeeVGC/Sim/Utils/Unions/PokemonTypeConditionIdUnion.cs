using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Union type that can represent either a PokemonType or a ConditionId.
/// Used for immunity checks that can apply to both type immunity and weather/condition immunity.
/// </summary>
public readonly struct PokemonTypeConditionIdUnion
{
    private readonly object? _value;
    private readonly bool _isPokemonType;

    public PokemonTypeConditionIdUnion(PokemonType pokemonType)
    {
        _value = pokemonType;
        _isPokemonType = true;
    }

    public PokemonTypeConditionIdUnion(ConditionId conditionId)
    {
        _value = conditionId;
        _isPokemonType = false;
    }

    public bool IsPokemonType => _isPokemonType;
    public bool IsConditionId => !_isPokemonType;

    public PokemonType? AsPokemonType => _isPokemonType ? (PokemonType)_value! : null;
    public ConditionId? AsConditionId => !_isPokemonType ? (ConditionId)_value! : null;

    public static implicit operator PokemonTypeConditionIdUnion(PokemonType pokemonType) 
        => new(pokemonType);

    public static implicit operator PokemonTypeConditionIdUnion(ConditionId conditionId) 
        => new(conditionId);

    public override string ToString()
    {
        return _value?.ToString() ?? "null";
    }

    public override bool Equals(object? obj)
    {
        if (obj is PokemonTypeConditionIdUnion other)
        {
            return _isPokemonType == other._isPokemonType && 
                   Equals(_value, other._value);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_value, _isPokemonType);
    }

    public static bool operator ==(PokemonTypeConditionIdUnion left, PokemonTypeConditionIdUnion right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PokemonTypeConditionIdUnion left, PokemonTypeConditionIdUnion right)
    {
        return !left.Equals(right);
    }
}
