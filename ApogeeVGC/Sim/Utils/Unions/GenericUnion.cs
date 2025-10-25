using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// int | bool
/// </summary>
public abstract record IntBoolUnion
{
    public abstract int ToInt();

    public static IntBoolUnion FromInt(int value) => new IntIntBoolUnion(value);
    public static IntBoolUnion FromBool(bool value) => new BoolIntBoolUnion(value);

    public static implicit operator IntBoolUnion(int value) => new IntIntBoolUnion(value);
    public static implicit operator IntBoolUnion(bool value) => new BoolIntBoolUnion(value);
}

public record IntIntBoolUnion(int Value) : IntBoolUnion
{
    public override int ToInt() => Value;
}

public record BoolIntBoolUnion(bool Value) : IntBoolUnion
{
    public override int ToInt() => Value ? 1 : 0;
}


/// <summary>
/// int | false
/// </summary>
public abstract record IntFalseUnion
{
    public abstract int ToInt();
    public abstract IntUndefinedFalseUnion ToIntUndefinedFalseUnion();

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

public record IntIntFalseUnion(int Value) : IntFalseUnion
{
    public override int ToInt() => Value;
    public override IntUndefinedFalseUnion ToIntUndefinedFalseUnion() => new IntIntUndefinedFalseUnion(Value);
}

public record FalseIntFalseUnion : IntFalseUnion
{
    public override int ToInt() => 0;
    public override IntUndefinedFalseUnion ToIntUndefinedFalseUnion() => new FalseIntUndefinedFalseUnion();
}



/// <summary>
/// int | false | undefined
/// </summary>
public abstract record IntFalseUndefinedUnion
{
    public static IntFalseUndefinedUnion FromInt(int value) => new IntIntFalseUndefined(value);
    public static IntFalseUndefinedUnion FromFalse() => new FalseIntFalseUndefined();
    public static IntFalseUndefinedUnion FromUndefined() => new UndefinedIntFalseUndefined(new Undefined());
    public static implicit operator IntFalseUndefinedUnion(int value) => new IntIntFalseUndefined(value);
    public static implicit operator IntFalseUndefinedUnion(Undefined value) =>
        new UndefinedIntFalseUndefined(value);
}
public record IntIntFalseUndefined(int Value) : IntFalseUndefinedUnion;
public record FalseIntFalseUndefined : IntFalseUndefinedUnion;
public record UndefinedIntFalseUndefined(Undefined Value) : IntFalseUndefinedUnion;



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
/// bool | empty | void
/// </summary>
public abstract record BoolEmptyVoidUnion
{
    public static BoolEmptyVoidUnion FromBool(bool value) => new BoolBoolEmptyVoidUnion(value);
    public static BoolEmptyVoidUnion FromEmpty() => new EmptyBoolEmptyVoidUnion(new Empty());
    public static BoolEmptyVoidUnion FromVoid() => new VoidUnionBoolEmptyVoidUnion(new VoidReturn());
    public static implicit operator BoolEmptyVoidUnion(bool value) => new BoolBoolEmptyVoidUnion(value);
    public static implicit operator BoolEmptyVoidUnion(VoidReturn value) => new VoidUnionBoolEmptyVoidUnion(value);
    public static implicit operator BoolEmptyVoidUnion(Empty value) => new EmptyBoolEmptyVoidUnion(value);
}
public record BoolBoolEmptyVoidUnion(bool Value) : BoolEmptyVoidUnion;
public record EmptyBoolEmptyVoidUnion(Empty Value) : BoolEmptyVoidUnion;
public record VoidUnionBoolEmptyVoidUnion(VoidReturn Value) : BoolEmptyVoidUnion;


/// <summary>
/// bool | int | empty | void
/// </summary>
public abstract record BoolIntEmptyVoidUnion
{
    public static BoolIntEmptyVoidUnion FromBool(bool value) => new BoolBoolIntEmptyVoidUnion(value);
    public static BoolIntEmptyVoidUnion FromInt(int value) => new IntBoolIntEmptyVoidUnion(value);
    public static BoolIntEmptyVoidUnion FromEmpty() => new EmptyBoolIntEmptyVoidUnion(new Empty());
    public static BoolIntEmptyVoidUnion FromVoid() => new VoidUnionBoolIntEmptyVoidUnion(new VoidReturn());
    public static implicit operator BoolIntEmptyVoidUnion(bool value) => new BoolBoolIntEmptyVoidUnion(value);
    public static implicit operator BoolIntEmptyVoidUnion(int value) => new IntBoolIntEmptyVoidUnion(value);
    public static implicit operator BoolIntEmptyVoidUnion(VoidReturn value) =>
        new VoidUnionBoolIntEmptyVoidUnion(value);
    public static implicit operator BoolIntEmptyVoidUnion(Empty value) =>
        new EmptyBoolIntEmptyVoidUnion(value);
}
public record BoolBoolIntEmptyVoidUnion(bool Value) : BoolIntEmptyVoidUnion;
public record IntBoolIntEmptyVoidUnion(int Value) : BoolIntEmptyVoidUnion;
public record EmptyBoolIntEmptyVoidUnion(Empty Value) : BoolIntEmptyVoidUnion;
public record VoidUnionBoolIntEmptyVoidUnion(VoidReturn Value) : BoolIntEmptyVoidUnion;


/// <summary>
/// bool | int | empty | undefined
/// </summary>
public abstract record BoolIntEmptyUndefinedUnion
{
    public abstract bool IsTruthy();
    public abstract bool IsZero();

    public abstract BoolIntUndefinedUnion ToBoolIntUndefinedUnion();

    public static BoolIntEmptyUndefinedUnion FromBool(bool value) => new BoolBoolIntEmptyUndefinedUnion(value);
    public static BoolIntEmptyUndefinedUnion FromInt(int value) => new IntBoolIntEmptyUndefinedUnion(value);
    public static BoolIntEmptyUndefinedUnion FromEmpty() => new EmptyBoolIntEmptyUndefinedUnion(new Empty());
    public static BoolIntEmptyUndefinedUnion FromUndefined() =>
        new UndefinedBoolIntEmptyUndefinedUnion(new Undefined());
    public static implicit operator BoolIntEmptyUndefinedUnion(bool value) =>
        new BoolBoolIntEmptyUndefinedUnion(value);
    public static implicit operator BoolIntEmptyUndefinedUnion(int value) =>
        new IntBoolIntEmptyUndefinedUnion(value);
    public static implicit operator BoolIntEmptyUndefinedUnion(Empty value) =>
        new EmptyBoolIntEmptyUndefinedUnion(value);
    public static implicit operator BoolIntEmptyUndefinedUnion(Undefined value) =>
        new UndefinedBoolIntEmptyUndefinedUnion(value);

    public static BoolIntEmptyUndefinedUnion? FromNullableBoolIntUndefinedUnion(BoolIntUndefinedUnion? value)
    {
        return value switch
        {
            BoolBoolIntUndefinedUnion b => b.Value,
            IntBoolIntUndefinedUnion i => i.Value,
            UndefinedBoolIntUndefinedUnion => new Undefined(),
            null => null,
            _ => throw new InvalidOperationException(),
        };
    }
}

public record BoolBoolIntEmptyUndefinedUnion(bool Value) : BoolIntEmptyUndefinedUnion
{
    public override bool IsTruthy() => Value;
    public override bool IsZero() => !Value;

    public override BoolIntUndefinedUnion ToBoolIntUndefinedUnion() => Value;
}

public record IntBoolIntEmptyUndefinedUnion(int Value) : BoolIntEmptyUndefinedUnion
{
    public override bool IsTruthy() => Value != 0;
    public override bool IsZero() => Value == 0;
    public override BoolIntUndefinedUnion ToBoolIntUndefinedUnion() => Value;

}

public record EmptyBoolIntEmptyUndefinedUnion(Empty Value) : BoolIntEmptyUndefinedUnion
{
    public override bool IsTruthy() => false;
    public override bool IsZero() => true;

    public override BoolIntUndefinedUnion ToBoolIntUndefinedUnion() =>
        throw new InvalidOperationException("BoolIntUndefinedUnion cannot hold the Undefined type.");
}
public record UndefinedBoolIntEmptyUndefinedUnion(Undefined Value) : BoolIntEmptyUndefinedUnion
{
    public override bool IsTruthy() => false;
    public override bool IsZero() => false;
    public override BoolIntUndefinedUnion ToBoolIntUndefinedUnion() => new Undefined();
}



/// <summary>
/// bool | undefined
/// </summary>
public abstract record BoolUndefinedUnion
{
    public abstract bool IsTruthy();
    public static BoolUndefinedUnion FromBool(bool value) => new BoolBoolUndefinedUnion(value);
    public static BoolUndefinedUnion FromUndefined() => new UndefinedBoolUndefinedUnion(new Undefined());
    public static implicit operator BoolUndefinedUnion(bool value) => new BoolBoolUndefinedUnion(value);
    public static implicit operator BoolUndefinedUnion(Undefined value) =>
        new UndefinedBoolUndefinedUnion(value);
}
public record BoolBoolUndefinedUnion(bool Value) : BoolUndefinedUnion
{
    public override bool IsTruthy() => Value;
}
public record UndefinedBoolUndefinedUnion(Undefined Value) : BoolUndefinedUnion
{
    public override bool IsTruthy() => false;
}


/// <summary>
/// bool | int | undefined
/// </summary>
public abstract record BoolIntUndefinedUnion
{
    public abstract bool IsTruthy();
    public abstract bool IsZero();
    public abstract int ToInt();

    public static BoolIntUndefinedUnion FromBool(bool value) => new BoolBoolIntUndefinedUnion(value);
    public static BoolIntUndefinedUnion FromInt(int value) => new IntBoolIntUndefinedUnion(value);
    public static BoolIntUndefinedUnion FromUndefined() =>
        new UndefinedBoolIntUndefinedUnion(new Undefined());
    public static implicit operator BoolIntUndefinedUnion(bool value) => new BoolBoolIntUndefinedUnion(value);
    public static implicit operator BoolIntUndefinedUnion(int value) => new IntBoolIntUndefinedUnion(value);
    public static implicit operator BoolIntUndefinedUnion(Undefined value) =>
        new UndefinedBoolIntUndefinedUnion(value);

    public IntFalseUndefinedUnion ToIntFalseUndefinedUnion()
    {
        return this switch
        {
            BoolBoolIntUndefinedUnion boolCase => boolCase.Value
                ? throw new InvalidOperationException("Cannot convert true to IntFalseUndefinedUnion")
                : new FalseIntFalseUndefined(),
            IntBoolIntUndefinedUnion intCase => new IntIntFalseUndefined(intCase.Value),
            UndefinedBoolIntUndefinedUnion => new UndefinedIntFalseUndefined(new Undefined()),
            _ => throw new InvalidOperationException("Unknown BoolIntUndefinedUnion type"),
        };
    }
}

public record BoolBoolIntUndefinedUnion(bool Value) : BoolIntUndefinedUnion
{
    public override bool IsTruthy() => Value;
    public override bool IsZero() => !Value;
    public override int ToInt() => Value ? 1 : 0;
}

public record IntBoolIntUndefinedUnion(int Value) : BoolIntUndefinedUnion
{
    public override bool IsTruthy() => Value != 0;
    public override bool IsZero() => Value == 0;
    public override int ToInt() => Value;
}

public record UndefinedBoolIntUndefinedUnion(Undefined Value) : BoolIntUndefinedUnion
{
    public override bool IsTruthy() => false;
    public override bool IsZero() => false;
    public override int ToInt() => 0;
}




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
    public override bool IsTrue() => MoveId != MoveId.None;
}
public record BoolMoveIdBoolUnion(bool Value) : MoveIdBoolUnion
{
    public override bool IsTrue() => Value;
}


/// <summary>
/// int | Undefined | false
/// </summary>
public abstract record IntUndefinedFalseUnion
{
    public static implicit operator IntUndefinedFalseUnion(int value) =>
        new IntIntUndefinedFalseUnion(value);
    public static implicit operator IntUndefinedFalseUnion(Undefined value) =>
        new UndefinedIntUndefinedFalseUnion(value);
    public static IntUndefinedFalseUnion FromFalse() => new FalseIntUndefinedFalseUnion();
}
public record IntIntUndefinedFalseUnion(int Value) : IntUndefinedFalseUnion;
public record UndefinedIntUndefinedFalseUnion(Undefined Value) : IntUndefinedFalseUnion;
public record FalseIntUndefinedFalseUnion : IntUndefinedFalseUnion;


/// <summary>
/// int | undefined | false | empty
/// </summary>
public abstract record IntUndefinedFalseEmptyUnion
{
    public static implicit operator IntUndefinedFalseEmptyUnion(int value) =>
        new IntIntUndefinedFalseEmptyUnion(value);
    public static implicit operator IntUndefinedFalseEmptyUnion(Undefined value) =>
        new UndefinedIntUndefinedFalseEmptyUnion(value);
    public static IntUndefinedFalseEmptyUnion FromFalse() => new FalseIntUndefinedFalseEmptyUnion();
    public static IntUndefinedFalseEmptyUnion FromEmpty() => new EmptyIntUndefinedFalseEmptyUnion(new Empty());

    public static IntUndefinedFalseEmptyUnion FromIntUndefinedFalseUnion(IntUndefinedFalseUnion value)
    {
        return value switch
        {
            IntIntUndefinedFalseUnion intCase => new IntIntUndefinedFalseEmptyUnion(intCase.Value),
            UndefinedIntUndefinedFalseUnion undefinedCase =>
                new UndefinedIntUndefinedFalseEmptyUnion(undefinedCase.Value),
            FalseIntUndefinedFalseUnion => new FalseIntUndefinedFalseEmptyUnion(),
            _ => throw new InvalidOperationException("Unknown IntUndefinedFalseUnion type"),
        };
    }
}
public record IntIntUndefinedFalseEmptyUnion(int Value) : IntUndefinedFalseEmptyUnion;
public record UndefinedIntUndefinedFalseEmptyUnion(Undefined Value) : IntUndefinedFalseEmptyUnion;
public record FalseIntUndefinedFalseEmptyUnion : IntUndefinedFalseEmptyUnion;
public record EmptyIntUndefinedFalseEmptyUnion(Empty Value) : IntUndefinedFalseEmptyUnion;





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
/// ConditionId | bool
/// </summary>
public abstract record ConditionIdBoolUnion
{
    public static implicit operator ConditionIdBoolUnion(ConditionId conditionId) =>
        new ConditionIdConditionIdBoolUnion(conditionId);
    public static implicit operator ConditionIdBoolUnion(bool value) => new BoolConditionIdBoolUnion(value);
}
public record ConditionIdConditionIdBoolUnion(ConditionId ConditionId) : ConditionIdBoolUnion;
public record BoolConditionIdBoolUnion(bool Value) : ConditionIdBoolUnion;


/// <summary>
/// Item | false
/// </summary>
public abstract record ItemFalseUnion
{
    public static implicit operator ItemFalseUnion(Item item) => new ItemItemFalseUnion(item);
    public static ItemFalseUnion FromFalse() => new FalseItemFalseUnion();
}
public record ItemItemFalseUnion(Item Item) : ItemFalseUnion;
public record FalseItemFalseUnion : ItemFalseUnion;



/// <summary>
/// Pokemon | int
/// </summary>
public abstract record PokemonIntUnion
{
    public static implicit operator PokemonIntUnion(Pokemon pokemon) => new PokemonPokemonIntUnion(pokemon);
    public static implicit operator PokemonIntUnion(int value) => new IntPokemonIntUnion(value);
}
public record PokemonPokemonIntUnion(Pokemon Pokemon) : PokemonIntUnion;
public record IntPokemonIntUnion(int Value) : PokemonIntUnion;



/// <summary>
/// Side | bool
/// </summary>
public abstract record SideBoolUnion
{
    public abstract bool IsTrue();
    public static implicit operator SideBoolUnion(Side side) => new SideSideBoolUnion(side);
    public static implicit operator SideBoolUnion(bool value) => new BoolSideBoolUnion(value);
}

public record SideSideBoolUnion(Side Side) : SideBoolUnion
{
    public override bool IsTrue() => true;
}

public record BoolSideBoolUnion(bool Value) : SideBoolUnion
{
    public override bool IsTrue() => Value;
}




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
/// MoveId | int
/// </summary>
public abstract record MoveIdIntUnion
{
    public static implicit operator MoveIdIntUnion(MoveId moveId) => new MoveIdMoveIdIntUnion(moveId);
    public static implicit operator MoveIdIntUnion(int value) => new IntMoveIdIntUnion(value);
}
public record MoveIdMoveIdIntUnion(MoveId MoveId) : MoveIdIntUnion;
public record IntMoveIdIntUnion(int Value) : MoveIdIntUnion;




/// <summary>
/// string | number | Delegate | object
/// Represents arguments for the addMove method that can be strings, numbers, functions, or arbitrary objects.
/// Maps to TypeScript: (string | number | Function | AnyObject)
/// </summary>
public abstract record StringNumberDelegateObjectUnion
{
    public static implicit operator StringNumberDelegateObjectUnion(string value) =>
        new StringStringNumberDelegateObjectUnion(value);
    public static implicit operator StringNumberDelegateObjectUnion(int value) =>
        new IntStringNumberDelegateObjectUnion(value);
    public static implicit operator StringNumberDelegateObjectUnion(double value) =>
        new DoubleStringNumberDelegateObjectUnion(value);
    public static implicit operator StringNumberDelegateObjectUnion(Delegate del) =>
        new DelegateStringNumberDelegateObjectUnion(del);

    // Factory method for explicit object creation
    public static StringNumberDelegateObjectUnion FromObject(object obj) =>
        new ObjectStringNumberDelegateObjectUnion(obj);
}

public record StringStringNumberDelegateObjectUnion(string Value) : StringNumberDelegateObjectUnion;
public record IntStringNumberDelegateObjectUnion(int Value) : StringNumberDelegateObjectUnion;
public record DoubleStringNumberDelegateObjectUnion(double Value) : StringNumberDelegateObjectUnion;
public record DelegateStringNumberDelegateObjectUnion(Delegate Delegate) : StringNumberDelegateObjectUnion;
public record ObjectStringNumberDelegateObjectUnion(object Object) : StringNumberDelegateObjectUnion;




/// <summary>
/// string | undefined
/// </summary>
public abstract record StringUndefinedUnion
{
    public static implicit operator StringUndefinedUnion(string value) =>
        new StringStringUndefinedUnion(value);
    public static implicit operator StringUndefinedUnion(Undefined value) =>
        new UndefinedStringUndefinedUnion(value);
}
public record StringStringUndefinedUnion(string Value) : StringUndefinedUnion;
public record UndefinedStringUndefinedUnion(Undefined Value) : StringUndefinedUnion;
