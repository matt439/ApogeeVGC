namespace ApogeeVGC_CS.sim
{
    /// <summary>
    /// EffectState | Record<string, never> | null
    /// </summary>
    public abstract record SingleEventState
    {
        public static implicit operator SingleEventState(EffectState effectState) =>
            new EffectStateSingleEventState(effectState);
    }
    public record EffectStateSingleEventState(EffectState EffectState) : SingleEventState;
    public record EmptySingleEventState : SingleEventState;
    public record NullSingleEventState : SingleEventState;

    /// <summary>
    /// string | Pokemon | Side | Field | Battle | null
    /// </summary>
    public abstract record SingleEventTarget
    {
        public static implicit operator SingleEventTarget(string value) =>
            new StringSingleEventTarget(value);
        public static implicit operator SingleEventTarget(Pokemon pokemon) =>
            new PokemonSingleEventTarget(pokemon);
        public static implicit operator SingleEventTarget(Side side) => new SideSingleEventTarget(side);
        public static implicit operator SingleEventTarget(Field field) => new FieldSingleEventTarget(field);
        public static implicit operator SingleEventTarget(Battle battle) =>
            new BattleSingleEventTarget(battle);
    }
    public record StringSingleEventTarget(string Value) : SingleEventTarget;
    public record PokemonSingleEventTarget(Pokemon Pokemon) : SingleEventTarget;
    public record SideSingleEventTarget(Side Side) : SingleEventTarget;
    public record FieldSingleEventTarget(Field Field) : SingleEventTarget;
    public record BattleSingleEventTarget(Battle Battle) : SingleEventTarget;
    public record NullSingleEventTarget : SingleEventTarget;

    public static class EffectUnionFactory
    {
        public static AddPart ToAddPart(IEffect effect) => effect switch
        {
            Ability ability => new PartAddPart(new EffectPart(ability)),
            ItemData itemData => new PartAddPart(new EffectPart(itemData)),
            ActiveMove activeMove => new PartAddPart(new EffectPart(activeMove)),
            Species species => new PartAddPart(new EffectPart(species)),
            Condition condition => new PartAddPart(new EffectPart(condition)),
            Format format => new PartAddPart(new EffectPart(format)),
            _ => throw new InvalidOperationException($"Cannot convert {effect.GetType()} to AddPart")
        };

        public static SingleEventSource ToSingleEventSource(IEffect effect) => effect switch
        {
            Ability ability => new EffectSingleEventSource(ability),
            ItemData itemData => new EffectSingleEventSource(itemData),
            ActiveMove activeMove => new EffectSingleEventSource(activeMove),
            Species species => new EffectSingleEventSource(species),
            Condition condition => new EffectSingleEventSource(condition),
            Format format => new EffectSingleEventSource(format),
            _ => throw new InvalidOperationException($"Cannot convert {effect.GetType()} to SingleEventSource")
        };

        public static SpreadDamageEffect ToSpreadDamageEffect(IEffect effect) => effect switch
        {
            Ability ability => new EffectSpreadDamageEffect(ability),
            ItemData itemData => new EffectSpreadDamageEffect(itemData),
            ActiveMove activeMove => new EffectSpreadDamageEffect(activeMove),
            Species species => new EffectSpreadDamageEffect(species),
            Condition condition => new EffectSpreadDamageEffect(condition),
            Format format => new EffectSpreadDamageEffect(format),
            _ => throw new InvalidOperationException($"Cannot convert {effect.GetType()} to SpreadDamageEffect")
        };

        public static DamageEffect ToDamageEffect(IEffect effect) => effect switch
        {
            Ability ability => new EffectDamageEffect(ability),
            ItemData itemData => new EffectDamageEffect(itemData),
            ActiveMove activeMove => new EffectDamageEffect(activeMove),
            Species species => new EffectDamageEffect(species),
            Condition condition => new EffectDamageEffect(condition),
            Format format => new EffectDamageEffect(format),
            _ => throw new InvalidOperationException($"Cannot convert {effect.GetType()} to DamageEffect")
        };

        public static Part ToPart(IEffect effect) => effect switch
        {
            Ability ability => new EffectPart(ability),
            ItemData itemData => new EffectPart(itemData),
            ActiveMove activeMove => new EffectPart(activeMove),
            Species species => new EffectPart(species),
            Condition condition => new EffectPart(condition),
            Format format => new EffectPart(format),
            _ => throw new InvalidOperationException($"Cannot convert {effect.GetType()} to Part")
        };

        public static HealEffect ToHealEffect(IEffect effect) => effect switch
        {
            Ability ability => new EffectHealEffect(ability),
            ItemData itemData => new EffectHealEffect(itemData),
            ActiveMove activeMove => new EffectHealEffect(activeMove),
            Species species => new EffectHealEffect(species),
            Condition condition => new EffectHealEffect(condition),
            Format format => new EffectHealEffect(format),
            _ => throw new InvalidOperationException($"Cannot convert {effect.GetType()} to HealEffect")
        };
    }

    public static class AnyObjectUnionFactory
    {
        public static AddMoveArg ToAddMoveArg(IAnyObject anyObject) => anyObject switch
        {
            Pokemon pokemon => new AnyObjectAddMoveArg(pokemon),
            _ => throw new InvalidOperationException($"Cannot convert {anyObject.GetType()} to AddMoveArg")
        };
    }

    /// <summary>
    /// string | Pokemon | Effect | false | null
    /// </summary>
    public abstract record SingleEventSource
    {
        public static implicit operator SingleEventSource(string value) =>
            new StringSingleEventSource(value);
        public static implicit operator SingleEventSource(Pokemon pokemon) =>
            new PokemonSingleEventSource(pokemon);

        public static implicit operator SingleEventSource(Ability ability) =>
            EffectUnionFactory.ToSingleEventSource(ability);
        public static implicit operator SingleEventSource(ItemData itemData) =>
            EffectUnionFactory.ToSingleEventSource(itemData);
        public static implicit operator SingleEventSource(ActiveMove activeMove) =>
            EffectUnionFactory.ToSingleEventSource(activeMove);
        public static implicit operator SingleEventSource(Species species) =>
            EffectUnionFactory.ToSingleEventSource(species);
        public static implicit operator SingleEventSource(Condition condition) =>
            EffectUnionFactory.ToSingleEventSource(condition);
        public static implicit operator SingleEventSource(Format format) =>
            EffectUnionFactory.ToSingleEventSource(format);

        public static implicit operator SingleEventSource(bool value) =>
            value ? new FalseSingleEventSource() : new NullSingleEventSource();
    }
    public record StringSingleEventSource(string Value) : SingleEventSource;
    public record PokemonSingleEventSource(Pokemon Pokemon) : SingleEventSource;
    public record EffectSingleEventSource(IEffect Effect) : SingleEventSource;
    public record FalseSingleEventSource : SingleEventSource;
    public record NullSingleEventSource : SingleEventSource;

    /// <summary>
    /// Pokemon | Pokemon[] | Side | Battle | null
    /// </summary>
    public abstract record RunEventTarget
    {
        public static implicit operator RunEventTarget(Pokemon pokemon) =>
            new PokemonRunEventTarget(pokemon);
        public static implicit operator RunEventTarget(List<Pokemon> pokemonList) =>
            new PokemonListRunEventTarget(pokemonList);
        public static implicit operator RunEventTarget(Side side) => new SideRunEventTarget(side);
        public static implicit operator RunEventTarget(Battle battle) => new BattleRunEventTarget(battle);
    }
    public record PokemonRunEventTarget(Pokemon Pokemon) : RunEventTarget;
    public record PokemonListRunEventTarget(List<Pokemon> PokemonList) : RunEventTarget;
    public record SideRunEventTarget(Side Side) : RunEventTarget;
    public record BattleRunEventTarget(Battle Battle) : RunEventTarget;
    public record NullRunEventTarget : RunEventTarget;

    /// <summary>
    /// string | Pokemon | false | null
    /// </summary>
    public abstract record RunEventSource
    {
        public static implicit operator RunEventSource(string value) => new StringRunEventSource(value);
        public static implicit operator RunEventSource(Pokemon pokemon) => new PokemonRunEventSource(pokemon);
        public static implicit operator RunEventSource(bool value) =>
            value ? new FalseRunEventSource() : new NullRunEventSource();
    }
    public record StringRunEventSource(string Value) : RunEventSource;
    public record PokemonRunEventSource(Pokemon Pokemon) : RunEventSource;
    public record FalseRunEventSource : RunEventSource;
    public record NullRunEventSource : RunEventSource;

    /// <summary>
    /// Pokemon | Side | Battle
    /// </summary>
    public abstract record PriorityEventTarget
    {
        public static implicit operator PriorityEventTarget(Pokemon pokemon) =>
            new PokemonPriorityEventTarget(pokemon);
        public static implicit operator PriorityEventTarget(Side side) =>
            new SidePriorityEventTarget(side);
        public static implicit operator PriorityEventTarget(Battle battle) =>
            new BattlePriorityEventTarget(battle);
    }
    public record PokemonPriorityEventTarget(Pokemon Pokemon) : PriorityEventTarget;
    public record SidePriorityEventTarget(Side Side) : PriorityEventTarget;
    public record BattlePriorityEventTarget(Battle Battle) : PriorityEventTarget;

    /// <summary>
    /// Pokemon | Side | Field | Battle
    /// </summary>
    public abstract record GetCallbackTarget
    {
        public static implicit operator GetCallbackTarget(Pokemon pokemon) =>
            new PokemonGetCallbackTarget(pokemon);
        public static implicit operator GetCallbackTarget(Side side) => new SideGetCallbackTarget(side);
        public static implicit operator GetCallbackTarget(Field field) => new FieldGetCallbackTarget(field);
        public static implicit operator GetCallbackTarget(Battle battle) =>
            new BattleGetCallbackTarget(battle);
    }
    public record PokemonGetCallbackTarget(Pokemon Pokemon) : GetCallbackTarget;
    public record SideGetCallbackTarget(Side Side) : GetCallbackTarget;
    public record FieldGetCallbackTarget(Field Field) : GetCallbackTarget;
    public record BattleGetCallbackTarget(Battle Battle) : GetCallbackTarget;

    /// <summary>
    /// Pokemon | Pokemon[] | Side | Battle
    /// </summary>
    public abstract record FindEventHandlersTarget
    {
        public static implicit operator FindEventHandlersTarget(Pokemon pokemon) =>
            new PokemonFindEventHandlersTarget(pokemon);
        public static implicit operator FindEventHandlersTarget(List<Pokemon> pokemonList) =>
            new PokemonListFindEventHandlersTarget(pokemonList);
        public static implicit operator FindEventHandlersTarget(Side side) =>
            new SideFindEventHandlersTarget(side);
        public static implicit operator FindEventHandlersTarget(Battle battle) =>
            new BattleFindEventHandlersTarget(battle);
    }
    public record PokemonFindEventHandlersTarget(Pokemon Pokemon) : FindEventHandlersTarget;
    public record PokemonListFindEventHandlersTarget(List<Pokemon> PokemonList) : FindEventHandlersTarget;
    public record SideFindEventHandlersTarget(Side Side) : FindEventHandlersTarget;
    public record BattleFindEventHandlersTarget(Battle Battle) : FindEventHandlersTarget;

    /// <summary>
    /// false | Pokemon | null
    /// </summary>
    public abstract record SpreadDamageTarget
    {
        public static implicit operator SpreadDamageTarget(bool value) =>
            value ? new FalseSpreadDamageTarget() : new NullSpreadDamageTarget();
        public static implicit operator SpreadDamageTarget(Pokemon pokemon) =>
            new PokemonSpreadDamageTarget(pokemon);
    }
    public record FalseSpreadDamageTarget : SpreadDamageTarget;
    public record PokemonSpreadDamageTarget(Pokemon Pokemon) : SpreadDamageTarget;
    public record NullSpreadDamageTarget : SpreadDamageTarget;

    /// <summary>
    /// 'drain' | 'recoil' | Effect | null
    /// </summary>
    public abstract record SpreadDamageEffect
    {
        public static implicit operator SpreadDamageEffect(string value) =>
            value switch
            {
                "drain" => new DrainSpreadDamageEffect(),
                "recoil" => new RecoilSpreadDamageEffect(),
                _ => new NullSpreadDamageEffect()
            };

        public static implicit operator SpreadDamageEffect(Ability ability) =>
            EffectUnionFactory.ToSpreadDamageEffect(ability);
        public static implicit operator SpreadDamageEffect(ItemData itemData) =>
            EffectUnionFactory.ToSpreadDamageEffect(itemData);
        public static implicit operator SpreadDamageEffect(ActiveMove activeMove) =>
            EffectUnionFactory.ToSpreadDamageEffect(activeMove);
        public static implicit operator SpreadDamageEffect(Species species) =>
            EffectUnionFactory.ToSpreadDamageEffect(species);
        public static implicit operator SpreadDamageEffect(Condition condition) =>
            EffectUnionFactory.ToSpreadDamageEffect(condition);
        public static implicit operator SpreadDamageEffect(Format format) =>
            EffectUnionFactory.ToSpreadDamageEffect(format);
    }
    public record DrainSpreadDamageEffect : SpreadDamageEffect;
    public record RecoilSpreadDamageEffect : SpreadDamageEffect;
    public record EffectSpreadDamageEffect(IEffect Effect) : SpreadDamageEffect;
    public record NullSpreadDamageEffect : SpreadDamageEffect;

    /// <summary>
    /// 'drain' | 'recoil' | Effect | null
    /// </summary>
    public abstract record DamageEffect
    {
        public static implicit operator DamageEffect(string value) =>
            value switch
            {
                "drain" => new DrainDamageEffect(),
                "recoil" => new RecoilDamageEffect(),
                _ => new NullDamageEffect()
            };

        public static implicit operator DamageEffect(Ability ability) =>
            EffectUnionFactory.ToDamageEffect(ability);
        public static implicit operator DamageEffect(ItemData itemData) =>
            EffectUnionFactory.ToDamageEffect(itemData);
        public static implicit operator DamageEffect(ActiveMove activeMove) =>
            EffectUnionFactory.ToDamageEffect(activeMove);
        public static implicit operator DamageEffect(Species species) =>
            EffectUnionFactory.ToDamageEffect(species);
        public static implicit operator DamageEffect(Condition condition) =>
            EffectUnionFactory.ToDamageEffect(condition);
        public static implicit operator DamageEffect(Format format) =>
            EffectUnionFactory.ToDamageEffect(format);
    }
    public record DrainDamageEffect : DamageEffect;
    public record RecoilDamageEffect : DamageEffect;
    public record EffectDamageEffect(IEffect Effect) : DamageEffect;
    public record NullDamageEffect : DamageEffect;

    /// <summary>
    /// string | number | boolean | Pokemon | Side | Effect | Move | null | undefined
    /// </summary>
    public abstract record Part
    {
        public static implicit operator Part(string value) => new StringPart(value);
        public static implicit operator Part(int value) => new IntPart(value);
        public static implicit operator Part(bool value) => new BoolPart(value);
        public static implicit operator Part(Pokemon pokemon) => new PokemonPart(pokemon);
        public static implicit operator Part(Side side) => new SidePart(side);

        public static implicit operator Part(Ability ability) =>
            EffectUnionFactory.ToPart(ability);
        public static implicit operator Part(ItemData itemData) =>
            EffectUnionFactory.ToPart(itemData);
        public static implicit operator Part(ActiveMove activeMove) =>
            EffectUnionFactory.ToPart(activeMove);
        public static implicit operator Part(Species species) =>
            EffectUnionFactory.ToPart(species);
        public static implicit operator Part(Condition condition) =>
            EffectUnionFactory.ToPart(condition);
        public static implicit operator Part(Format format) =>
            EffectUnionFactory.ToPart(format);

        public static implicit operator Part(Move move) => new MovePart(move);
    }
    public record StringPart(string Value) : Part;
    public record IntPart(int Value) : Part;
    public record BoolPart(bool Value) : Part;
    public record PokemonPart(Pokemon Pokemon) : Part;
    public record SidePart(Side Side) : Part;
    public record EffectPart(IEffect Effect) : Part;
    public record MovePart(Move Move) : Part;
    public record NullPart : Part;

    /// <summary>
    /// Part {string | number | boolean | Pokemon | Side | Effect | Move | null | undefined} | 
    /// Func<SideSecretShared> | GameType
    /// </summary>
    public abstract record AddPart
    {
        public static implicit operator AddPart(string value) => new PartAddPart(new StringPart(value));
        public static implicit operator AddPart(int value) => new PartAddPart(new IntPart(value));
        public static implicit operator AddPart(bool value) => new PartAddPart(new BoolPart(value));
        public static implicit operator AddPart(Pokemon value) => new PartAddPart(new PokemonPart(value));
        public static implicit operator AddPart(Side value) => new PartAddPart(new SidePart(value));

        public static implicit operator AddPart(Ability ability) => EffectUnionFactory.ToAddPart(ability);
        public static implicit operator AddPart(ItemData itemData) => EffectUnionFactory.ToAddPart(itemData);
        public static implicit operator AddPart(ActiveMove activeMove) => EffectUnionFactory.ToAddPart(activeMove);
        public static implicit operator AddPart(Species species) => EffectUnionFactory.ToAddPart(species);
        public static implicit operator AddPart(Condition condition) => EffectUnionFactory.ToAddPart(condition);
        public static implicit operator AddPart(Format format) => EffectUnionFactory.ToAddPart(format);

        public static implicit operator AddPart(Move value) => new PartAddPart(new MovePart(value));
        public static implicit operator AddPart(GameType value) => new GameTypeAddPart(value);
        public static implicit operator AddPart(Func<SideSecretShared> func) => new FuncAddPart(func);
    }
    public record PartAddPart(Part Part) : AddPart;
    public record FuncAddPart(Func<SideSecretShared> Func) : AddPart;
    public record GameTypeAddPart(GameType GameType) : AddPart;

    /// <summary>
    /// string | number | Function | AnyObject
    /// </summary>
    public abstract record AddMoveArg
    {
        public static implicit operator AddMoveArg(string value) => new StringAddMoveArg(value);
        public static implicit operator AddMoveArg(int value) => new IntAddMoveArg(value);
        public static implicit operator AddMoveArg(Delegate @delegate) => new FuncAddMoveArg(@delegate);
        public static implicit operator AddMoveArg(Pokemon pokemon) =>
            AnyObjectUnionFactory.ToAddMoveArg(pokemon);
    }
    public record StringAddMoveArg(string Value) : AddMoveArg;
    public record IntAddMoveArg(int Value) : AddMoveArg;
    public record FuncAddMoveArg(Delegate Delegate) : AddMoveArg;
    public record AnyObjectAddMoveArg(IAnyObject AnyObject) : AddMoveArg;

    /// <summary>
    /// int | false
    /// </summary>
    public abstract record IntFalseUnion
    {
        public static implicit operator IntFalseUnion(int value) => new IntIntFalseUnion(value);
        public static implicit operator IntFalseUnion(bool value) =>
            value ? new FalseIntFalseUnion() : throw new ArgumentException("Must be false");
    }
    public record IntIntFalseUnion(int Value) : IntFalseUnion;
    public record FalseIntFalseUnion : IntFalseUnion;

    /// <summary>
    /// 'drain' | Effect | null
    /// </summary>
    public abstract record HealEffect
    {
        public static implicit operator HealEffect(string value) =>
            value switch
            {
                "drain" => new DrainHealEffect(),
                _ => new NullHealEffect()
            };

        public static implicit operator HealEffect(Ability ability) =>
            EffectUnionFactory.ToHealEffect(ability);
        public static implicit operator HealEffect(ItemData itemData) =>
            EffectUnionFactory.ToHealEffect(itemData);
        public static implicit operator HealEffect(ActiveMove activeMove) =>
            EffectUnionFactory.ToHealEffect(activeMove);
        public static implicit operator HealEffect(Species species) =>
            EffectUnionFactory.ToHealEffect(species);
        public static implicit operator HealEffect(Condition condition) =>
            EffectUnionFactory.ToHealEffect(condition);
        public static implicit operator HealEffect(Format format) =>
            EffectUnionFactory.ToHealEffect(format);
    }
    public record DrainHealEffect : HealEffect;
    public record EffectHealEffect(IEffect Effect) : HealEffect;
    public record NullHealEffect : HealEffect;



















    ///// <summary>
    ///// StringObject | Pokemon | Side | Field | Battle
    ///// </summary>
    //public interface ISingleEventTarget;

    ///// <summary>
    ///// StringObject | Pokemon |
    ///// IEffect {Ability | Item | ActiveMove | Species | Condition | Format} | FalseObject
    ///// </summary>
    //public interface ISingleEventSource;

    ///// <summary>
    ///// Pokemon | PokemonList | Side | Battle
    ///// </summary>
    //public interface IRunEventTarget;

    ///// <summary>
    ///// StringObject | Pokemon | FalseObject
    ///// </summary>
    //public interface IRunEventSource;

    ///// <summary>
    ///// Pokemon | Side | Battle
    ///// </summary>
    //public interface IPriorityEventTarget;

    ///// <summary>
    ///// Pokemon | Side | Field | Battle
    ///// </summary>
    //public interface IGetCallbackTarget;

    ///// <summary>
    ///// Pokemon | PokemonList | Side | Battle
    ///// </summary>
    //public interface  IFindEventHandlersTarget;

    ///// <summary>
    ///// false | Pokemon
    ///// </summary>
    //public interface ISpreadDamageTarget;

    ///// <summary>
    ///// 'drain' | 'recoil' | Effect
    ///// </summary>
    //public interface ISpreadDamageEffect;

    ///// <summary>
    ///// 'drain' | 'recoil' | Effect
    ///// </summary>
    //public interface IDamageEffect;

    ///// <summary>
    ///// number | false
    ///// </summary>
    //public interface IIntFalseUnion;

    ///// <summary>
    ///// 'drain' | Effect
    ///// </summary>
    //public interface IHealEffect;

    ///// <summary>
    ///// int | List<int>
    ///// </summary>
    //public interface IIntListIntUnion;

    /////// <summary>
    /////// string | number | boolean | Pokemon | Side | Effect | Move | null | undefined
    /////// </summary>
    ////public interface IPart : IAddPart;

    /////// <summary>
    /////// Part {string | number | boolean | Pokemon | Side | Effect | Move | null | undefined} | 
    /////// Func<SideSecretShared> | GameType
    /////// </summary>
    ////public interface IAddPart;

    ///// <summary>
    ///// string | number | Function | AnyObject
    ///// </summary>
    //public interface IAddMoveArg;

    ///// <summary>
    ///// Pokemon |
    ///// </summary>
    //public interface IAnyObject : IAddMoveArg;

    //public class GameTypeObject(GameType gameType) : IAddPart
    //{
    //    public GameType GameType { get; set; } = gameType;
    //}

    //public class AddPartFunc(Func<SideSecretShared> func) : IAddPart
    //{
    //    public required Func<SideSecretShared> Func { get; set; } = func;
    //}

    //public class DelegateObject(Delegate @delegate) : IAddMoveArg
    //{
    //    public required Delegate Delegate { get; set; } = @delegate;
    //}


    ////public class ActionObject : IAction;

    ////public class FunctionObject : IFunction;

    ////public class AnyObject : IAddMoveArg;

    //public class PokemonList : List<Pokemon>, IRunEventTarget, IFindEventHandlersTarget;

    //public class StringObject(string s) : IRunEventSource, ISingleEventTarget, ISpreadDamageEffect,
    //    IDamageEffect, IHealEffect, IPart, IAddMoveArg
    //{
    //    public string String { get; set; } = s;
    //}

    //public class BoolObject(bool b) : IPart
    //{
    //    public bool Bool { get; set; } = b;
    //}

    //public class FalseObject : IRunEventSource, ISingleEventSource, ISpreadDamageTarget,
    //    IIntFalseUnion
    //{
    //    public static bool False => false;
    //}

    //public class IntObject(int i) : IIntFalseUnion, IIntListIntUnion, IPart, IAddMoveArg
    //{
    //    public int Int { get; set; } = i;
    //}

    //public class IntList : List<int>, IIntListIntUnion;






    ///// <summary>
    ///// Pokemon | PokemonList | Side | Battle
    ///// </summary>
    //public interface IRunEventTarget : IPokemonSideBattlePokemonListUnion;

    ///// <summary>
    ///// StringObject | Pokemon | FalseObject
    ///// </summary>
    //public interface IRunEventSource : IStringObjectPokemonFalseObjectUnion;

    ///// <summary>
    ///// StringObject | Pokemon | Side | Field | Battle
    ///// </summary>
    //public interface ISingleEventTarget : IPokemonSideFieldBattleUnionStringObject;

    ///// <summary>
    ///// StringObject | Pokemon |
    ///// IEffect {Ability | Item | ActiveMove | Species | Condition | Format} | FalseObject
    ///// </summary>
    //public interface ISingleEventSource : IStringObjectPokemonFalseObjectEffectUnion;

    ///// <summary>
    ///// Pokemon | Side | Battle
    ///// </summary>
    //public interface IPokemonSideBattleUnion;

    ///// <summary>
    ///// Pokemon | Side | Field | Battle
    ///// </summary>
    //public interface IPokemonSideFieldBattleUnion : IPokemonSideBattleUnion;

    ///// <summary>
    ///// Pokemon | Side | Battle | PokemonList
    ///// </summary>
    //public interface IPokemonSideBattlePokemonListUnion : IPokemonSideFieldBattleUnion;

    ///// <summary>
    ///// Pokemon | Side | Field | Battle | StringObject
    ///// </summary>
    //public interface IPokemonSideFieldBattleUnionStringObject : IPokemonSideFieldBattleUnion;

    ///// <summary>
    ///// StringObject | Pokemon | FalseObject
    ///// </summary>
    //public interface IStringObjectPokemonFalseObjectUnion;

    ///// <summary>
    ///// StringObject | Pokemon | FalseObject | Effect
    ///// </summary>
    //public interface IStringObjectPokemonFalseObjectEffectUnion :
    //    IStringObjectPokemonFalseObjectUnion;

    //public class PokemonList : IPokemonSideBattlePokemonListUnion
    //{
    //    public required List<Pokemon> Value {  get; set; }
    //}

    //public class StringObject : IStringObjectPokemonFalseObjectUnion,
    //    IPokemonSideFieldBattleUnionStringObject
    //{
    //    public required string Value { get; set; }
    //}

    //public class FalseObject : IStringObjectPokemonFalseObjectUnion
    //{
    //    public static bool Value => false;
    //}
}
