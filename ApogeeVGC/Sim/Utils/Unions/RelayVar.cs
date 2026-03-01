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
    public static implicit operator RelayVar(bool value) => value ? BoolRelayVar.True : BoolRelayVar.False;
    public static implicit operator RelayVar(int value) => IntRelayVar.Get(value);
    public static implicit operator RelayVar(Ability ability) => EffectUnionFactory.ToRelayVar(ability);
    public static implicit operator RelayVar(Item item) => EffectUnionFactory.ToRelayVar(item);
    public static implicit operator RelayVar(ActiveMove activeMove) => EffectUnionFactory.ToRelayVar(activeMove);
    public static implicit operator RelayVar(Species species) => EffectUnionFactory.ToRelayVar(species);
    public static implicit operator RelayVar(Condition condition) => EffectUnionFactory.ToRelayVar(condition);
    public static implicit operator RelayVar(Format format) => EffectUnionFactory.ToRelayVar(format);
    public static implicit operator RelayVar(PokemonType type) => new PokemonTypeRelayVar(type);
    public static implicit operator RelayVar(ConditionId? id) => new ConditionIdRelayVar(id);
    public static implicit operator RelayVar(BoostsTable table) => new BoostsTableRelayVar(table);
    public static implicit operator RelayVar(PokemonType[] types) => new TypesRelayVar(types);
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
 IntIntTrueUnion intValue => IntRelayVar.Get(intValue.Value),
        TrueIntTrueUnion => BoolRelayVar.True,
  _ => throw new InvalidOperationException("Unknown IntTrueUnion type"),
    };
    public static implicit operator RelayVar(BoolIntUndefinedUnion union) => new BoolIntUndefinedUnionRelayVar(union);
    public static implicit operator RelayVar(SecondaryEffect[] effects) => new SecondaryEffectArrayRelayVar(effects);
    public static RelayVar FromUndefined() => new UndefinedRelayVar();
  public static RelayVar FromVoid() => new VoidReturnRelayVar();
}

public record BoolRelayVar(bool Value) : RelayVar
{
    public static readonly BoolRelayVar True = new(true);
    public static readonly BoolRelayVar False = new(false);
}
public record IntRelayVar(int Value) : RelayVar
{
    private const int CacheMin = -16;
    private const int CacheMax = 1024;
    private static readonly IntRelayVar[] Cache = InitCache();

    private static IntRelayVar[] InitCache()
    {
        var cache = new IntRelayVar[CacheMax - CacheMin];
        for (int i = 0; i < cache.Length; i++)
        {
            cache[i] = new IntRelayVar(i + CacheMin);
        }
        return cache;
    }

    public static IntRelayVar Get(int value)
    {
        int index = value - CacheMin;
        if ((uint)index < (uint)Cache.Length)
        {
            return Cache[index];
        }
        return new IntRelayVar(value);
    }
}
public record EffectRelayVar(IEffect Effect) : RelayVar;
public record SpecieRelayVar(Species Species) : RelayVar;
public record PokemonTypeRelayVar(PokemonType Type) : RelayVar;
public record ConditionIdRelayVar(ConditionId? Id) : RelayVar;
public record BoostsTableRelayVar(BoostsTable Table) : RelayVar;
public record TypesRelayVar(PokemonType[] Types) : RelayVar;
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
/// Represents TS null semantics in event handlers: "this action failed silently;
/// suppress any 'But it failed!' messages". Distinct from C# null (= TS undefined
/// = passthrough/no opinion) and BoolRelayVar(false) (= TS false = "this action failed").
/// </summary>
public record NullRelayVar : RelayVar;
