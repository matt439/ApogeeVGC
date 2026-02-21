using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Utils.Unions;

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
/// Represents a TS 'null' return value from an event handler, meaning "fail silently".
/// Distinct from C# null (which means "no handler opinion / use default") and
/// BoolRelayVar(false) (which means "fail with message").
/// </summary>
public record NullRelayVar : RelayVar;
