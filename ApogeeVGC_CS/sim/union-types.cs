using System.ComponentModel.DataAnnotations;

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

        public static EffectEffectStringUnion ToEffectStringUnion(IEffect effect) => effect switch
        {
            Ability ability => new EffectEffectStringUnion(ability),
            ItemData itemData => new EffectEffectStringUnion(itemData),
            ActiveMove activeMove => new EffectEffectStringUnion(activeMove),
            Species species => new EffectEffectStringUnion(species),
            Condition condition => new EffectEffectStringUnion(condition),
            Format format => new EffectEffectStringUnion(format),
            _ => throw new InvalidOperationException($"Cannot convert {effect.GetType()} to EffectEffectStringUnion")
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
            value ? new NullSingleEventSource() : new FalseSingleEventSource();
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
            value ? new NullRunEventSource() : new FalseRunEventSource();
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
            value ? new NullSpreadDamageTarget() : new FalseSpreadDamageTarget();

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

    /// <summary>
    /// int | false
    /// </summary>
    public abstract record IntFalseUnion
    {
        public static implicit operator IntFalseUnion(int value) => new IntIntFalseUnion(value);

        public static implicit operator IntFalseUnion(bool value) =>
            value ? throw new ArgumentException("must be 'false'") : new FalseIntFalseUnion();
    }

    public record IntIntFalseUnion(int Value) : IntFalseUnion;

    public record FalseIntFalseUnion : IntFalseUnion;

    /// <summary>
    /// bool | string
    /// </summary>
    public abstract record BoolStringUnion
    {
        public static implicit operator BoolStringUnion(bool value) => new BoolBoolStringUnion(value);
        public static implicit operator BoolStringUnion(string value) => new StringBoolStringUnion(value);
    }

    public record BoolBoolStringUnion(bool Value) : BoolStringUnion;

    public record StringBoolStringUnion(string Value) : BoolStringUnion;

    /// <summary>
    /// int | bool
    /// </summary>
    public abstract record IntBoolUnion
    {
        public static implicit operator IntBoolUnion(int value) => new IntIntBoolUnion(value);
        public static implicit operator IntBoolUnion(bool value) => new BoolIntBoolUnion(value);
    }

    public record IntIntBoolUnion(int Value) : IntBoolUnion;

    public record BoolIntBoolUnion(bool Value) : IntBoolUnion;


    /// <summary>
    /// Pokemon | Side | Field | Battle
    /// </summary>
    public abstract record EventEffectHolder
    {
        public static implicit operator EventEffectHolder(Pokemon pokemon) =>
            new PokemonEventEffectHolder(pokemon);

        public static implicit operator EventEffectHolder(Side side) =>
            new SideEventEffectHolder(side);

        public static implicit operator EventEffectHolder(Field field) =>
            new FieldEventEffectHolder(field);

        public static implicit operator EventEffectHolder(Battle battle) =>
            new BattleEventEffectHolder(battle);
    }

    public record PokemonEventEffectHolder(Pokemon Pokemon) : EventEffectHolder;

    public record SideEventEffectHolder(Side Side) : EventEffectHolder;

    public record FieldEventEffectHolder(Field Field) : EventEffectHolder;

    public record BattleEventEffectHolder(Battle Battle) : EventEffectHolder;

    /// <summary>
    /// boolean | 'done'
    /// </summary>
    public abstract record MoveActionMega
    {
        public static implicit operator MoveActionMega(bool value) => new BoolMoveActionMega(value);

        public static implicit operator MoveActionMega(string value) =>
            value == "done" ? new DoneMoveActionMega() : throw new ArgumentException("Must be 'done'");
    }

    public record BoolMoveActionMega(bool Value) : MoveActionMega;

    public record DoneMoveActionMega : MoveActionMega;


    /// <summary>
    /// boolean | 'spectator'
    /// </summary>
    public abstract record BattleStreamReplay
    {
        public static implicit operator BattleStreamReplay(bool value) => new BoolBattleStreamReplay(value);

        public static implicit operator BattleStreamReplay(string value) =>
            value == "spectator"
                ? new SpectatorBattleStreamReplay()
                : throw new ArgumentException("Must be 'spectator'");
    }

    public record BoolBattleStreamReplay(bool Value) : BattleStreamReplay;

    public record SpectatorBattleStreamReplay : BattleStreamReplay;

    /// <summary>
    /// Pokemon & Side
    /// </summary>
    public class OnAllyResidualTarget
    {
        public required Pokemon Pokemon { get; init; }
        public required Side Side { get; init; }
    }


    /// <summary>
    /// bool | ''
    /// </summary>
    public abstract record BoolEmptyStringUnion
    {
        public static implicit operator BoolEmptyStringUnion(bool value) =>
            new BoolEmptyStringBoolUnion(value);

        public static implicit operator BoolEmptyStringUnion(string value) =>
            value == string.Empty
                ? new EmptyStringBoolUnion(value)
                : throw new ArgumentException("Must be an empty string");
    }

    public record BoolEmptyStringBoolUnion(bool Value) : BoolEmptyStringUnion;

    public record EmptyStringBoolUnion(string Value) : BoolEmptyStringUnion;

    /// <summary>
    /// CommonHandlers['ModifierSourceMove'] | -0.1
    /// </summary>
    public abstract record OnFractionalPriority
    {
        public static implicit operator OnFractionalPriority(
            Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?> function) =>
            new OnFractionalPriorityFunc(function);

        private const double Tolerance = 0.0001;

        public static implicit operator OnFractionalPriority(double value) =>
            Math.Abs(value - (-0.1)) < Tolerance
                ? new OnFrationalPriorityNeg(value)
                : throw new ArgumentException("Must be -0.1 for OnFractionalPriorityNeg");
    }

    public record OnFractionalPriorityFunc(
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?> Function) : OnFractionalPriority;

    public record OnFrationalPriorityNeg(double Value) : OnFractionalPriority;

    /// <summary>
    /// string | List<string>
    /// </summary>
    public abstract record StrListStrUnion
    {
        public static implicit operator StrListStrUnion(string value) => new StrStrListStrUnion(value);
        public static implicit operator StrListStrUnion(List<string> value) => new StrListStrListUnion(value);
    }

    public record StrStrListStrUnion(string Value) : StrListStrUnion;

    public record StrListStrListUnion(List<string> Value) : StrListStrUnion;

    /// <summary>
    /// string | int | ActiveMove
    /// </summary>
    public abstract record GetDamageMove
    {
        public static implicit operator GetDamageMove(string value) => new StrGetDamageMove(value);
        public static implicit operator GetDamageMove(int value) => new IntGetDamageMove(value);

        public static implicit operator GetDamageMove(ActiveMove activeMove) =>
            new ActiveMoveGetDamageMove(activeMove);
    }

    public record StrGetDamageMove(string Value) : GetDamageMove;

    public record IntGetDamageMove(int Value) : GetDamageMove;

    public record ActiveMoveGetDamageMove(ActiveMove ActiveMove) : GetDamageMove;

    /// <summary>
    /// bool | HasValueOption.Integer | HasValueOption.PositiveInteger
    /// </summary>
    public abstract record DexFormatsHasValue
    {
        public static implicit operator DexFormatsHasValue(bool value) => new BoolDexFormatsHasValue(value);

        public static implicit operator DexFormatsHasValue(HasValueOption value) =>
            new HasValueOptionUnion(value);
    }

    public record BoolDexFormatsHasValue(bool Value) : DexFormatsHasValue;

    public record HasValueOptionUnion(HasValueOption Value) : DexFormatsHasValue;

    /// <summary>
    /// string | int | List<string>
    /// </summary>
    public abstract record StrIntListStrUnion
    {
        public static implicit operator StrIntListStrUnion(string value) => new StrStrIntListStrUnion(value);
        public static implicit operator StrIntListStrUnion(int value) => new IntStrIntListStrUnion(value);

        public static implicit operator StrIntListStrUnion(List<string> value) =>
            new ListStrStrIntListStrUnion(value);
    }

    public record StrStrIntListStrUnion(string Value) : StrIntListStrUnion;

    public record IntStrIntListStrUnion(int Value) : StrIntListStrUnion;

    public record ListStrStrIntListStrUnion(List<string> Value) : StrIntListStrUnion;


    /// <summary>
    /// string | (string | number | string[])[]
    /// </summary>
    public abstract record ValidateRuleReturn
    {
        public static implicit operator ValidateRuleReturn(string value) => new StringValidateRuleReturn(value);

        public static implicit operator ValidateRuleReturn(List<StrIntListStrUnion> value) =>
            new ListValidateRuleReturn(value);
    }

    public record StringValidateRuleReturn(string Value) : ValidateRuleReturn;

    public record ListValidateRuleReturn(List<StrIntListStrUnion> Value) : ValidateRuleReturn;


    /// <summary>
    /// string | true
    /// </summary>
    public abstract record StringTrueUnion
    {
        public static implicit operator StringTrueUnion(string value) => new StringTrue(value);

        public static implicit operator StringTrueUnion(bool value) =>
            value
                ? new TrueStringTrueUnion(value)
                : throw new ArgumentException("Must be 'true' for TrueStringTrueUnion");
    }

    public record StringTrue(string Value) : StringTrueUnion;

    public record TrueStringTrueUnion(bool Value) : StringTrueUnion;


    /// <summary>
    /// SparseBoostsTable | false
    /// </summary>
    public abstract record ItemBoosts
    {
        public static implicit operator ItemBoosts(SparseBoostsTable table) =>
            new SparseBoostsTableItemBoosts(table);

        public static implicit operator ItemBoosts(bool value) =>
            value ? throw new ArgumentException("Must be 'false' for FlaseItemBoosts") : new FlaseItemBoosts(value);
    }

    public record SparseBoostsTableItemBoosts(SparseBoostsTable Table) : ItemBoosts;

    public record FlaseItemBoosts(bool Value) : ItemBoosts;


    /// <summary>
    /// int | double | true
    /// </summary>
    public abstract record MoveDataAccuracy
    {
        public static implicit operator MoveDataAccuracy(int value) => new IntMoveDataAccuracy(value);
        public static implicit operator MoveDataAccuracy(double value) => new DoubleMoveDataAccuracy(value);

        public static implicit operator MoveDataAccuracy(bool value) =>
            value
                ? new TrueMoveDataAccuracy(value)
                : throw new ArgumentException("Must be 'true' for TrueMoveDataAccuracy");
    }

    public record IntMoveDataAccuracy(int Value) : MoveDataAccuracy;

    public record DoubleMoveDataAccuracy(double Value) : MoveDataAccuracy;

    public record TrueMoveDataAccuracy(bool Value) : MoveDataAccuracy;

    /// <summary>
    /// int | 'level' | false
    /// </summary>
    public abstract record MoveDataDamage
    {
        public static implicit operator MoveDataDamage(int value) => new MoveDataDamageInt(value);

        public static implicit operator MoveDataDamage(string value) =>
            value == "level" ? new MoveDataDamageLevel(value) : throw new ArgumentException("Must be 'level'");

        public static implicit operator MoveDataDamage(bool value) =>
            value
                ? throw new ArgumentException("Must be 'false' for MoveDataDamageFalse")
                : new MoveDataDamageFalse(value);
    }
    public record MoveDataDamageInt(int Value) : MoveDataDamage;
    public record MoveDataDamageLevel(string Value) : MoveDataDamage;
    public record MoveDataDamageFalse(bool Value) : MoveDataDamage;

    /// <summary>
    /// boolean | IDEntry
    /// </summary>
    public abstract record MoveDataIsZ
    {
        public static implicit operator MoveDataIsZ(bool value) => new BoolMoveDataIsZ(value);
        public static implicit operator MoveDataIsZ(IdEntry value) => new IdEntryMoveDataIsZ(value);
    }
    public record BoolMoveDataIsZ(bool Value) : MoveDataIsZ;
    public record IdEntryMoveDataIsZ(IdEntry Value) : MoveDataIsZ;

    /// <summary>
    ///  boolean | 'Ice'
    /// </summary>
    public abstract record MoveDataOhko
    {
        public static implicit operator MoveDataOhko(bool value) => new BoolMoveDataOhko(value);

        public static implicit operator MoveDataOhko(string value) =>
            value == "Ice"
                ? new IceMoveDataOhko(value)
                : throw new ArgumentException("Must be 'Ice' for IceMoveDataOhko");
    }
    public record BoolMoveDataOhko(bool Value) : MoveDataOhko;
    public record IceMoveDataOhko(string Value) : MoveDataOhko;


    /// <summary>
    /// 'copyvolatile' | 'shedtail' | boolean
    /// </summary>
    public abstract record MoveDataSelfSwitch
    {
        public static implicit operator MoveDataSelfSwitch(string value) =>
            value switch
            {
                "copyvolatile" => new CopyVolatileMoveDataSelfSwitch(),
                "shedtail" => new ShedTailMoveDataSelfSwitch(),
                _ => new BoolMoveDataSelfSwitch(bool.Parse(value))
            };

        public static implicit operator MoveDataSelfSwitch(bool value) => new BoolMoveDataSelfSwitch(value);
    }
    public record CopyVolatileMoveDataSelfSwitch : MoveDataSelfSwitch;
    public record ShedTailMoveDataSelfSwitch : MoveDataSelfSwitch;
    public record BoolMoveDataSelfSwitch(bool Value) : MoveDataSelfSwitch;

    /// <summary>
    /// 'always' | 'ifHit' | boolean
    /// </summary>
    public abstract record MoveDataSelfdestruct
    {
        public static implicit operator MoveDataSelfdestruct(string value) =>
            value switch
            {
                "always" => new AlwaysMoveDataSelfdestruct(),
                "ifHit" => new IfHitMoveDataSelfdestruct(),
                _ => new BoolMoveDataSelfdestruct(bool.Parse(value))
            };

        public static implicit operator MoveDataSelfdestruct(bool value) => new BoolMoveDataSelfdestruct(value);
    }
    public record AlwaysMoveDataSelfdestruct : MoveDataSelfdestruct;
    public record IfHitMoveDataSelfdestruct : MoveDataSelfdestruct;
    public record BoolMoveDataSelfdestruct(bool Value) : MoveDataSelfdestruct;


    /// <summary>
    /// boolean | { [PokemonType: string]: boolean }
    /// </summary>
    public abstract record MoveDataIgnoreImmunity
    {
        public static implicit operator MoveDataIgnoreImmunity(bool value) =>
            new BoolMoveDataIgnoreImmunity(value);

        public static implicit operator MoveDataIgnoreImmunity(Dictionary<PokemonType, bool> typeImmunities) =>
            new TypeMoveDataIgnoreImmunity(typeImmunities);
    }
    public record BoolMoveDataIgnoreImmunity(bool Value) : MoveDataIgnoreImmunity;
    public record TypeMoveDataIgnoreImmunity(Dictionary<PokemonType, bool> TypeImmunities) :
        MoveDataIgnoreImmunity;


    /// <summary>
    /// int | true
    /// </summary>
    public abstract record IntTrueUnion
    {
        public static implicit operator IntTrueUnion(int value) => new IntIntTrueUnion(value);

        public static implicit operator IntTrueUnion(bool value) =>
            value ? new TrueIntTrueUnion(value) : throw new ArgumentException("Must be 'true' for TrueIntTrueUnion");
    }

    public record IntIntTrueUnion(int Value) : IntTrueUnion;
    public record TrueIntTrueUnion(bool Value) : IntTrueUnion;


    /// <summary>
    /// boolean | 'Past'
    /// </summary>
    public abstract record DexSpeciesUnreleasedHidden
    {
        public static implicit operator DexSpeciesUnreleasedHidden(bool value) =>
            new BoolDexSpeciesUnreleasedHidden(value);

        public static implicit operator DexSpeciesUnreleasedHidden(string value) =>
            value == "Past"
                ? new PastDexSpeciesUnreleasedHidden(value)
                : throw new ArgumentException("Must be 'Past' for PastDexSpeciesUnreleasedHidden");
    }
    public record BoolDexSpeciesUnreleasedHidden(bool Value) : DexSpeciesUnreleasedHidden;
    public record PastDexSpeciesUnreleasedHidden(string Value) : DexSpeciesUnreleasedHidden;


    /// <summary>
    /// Move | string
    /// </summary>
    public abstract record MoveStringUnion
    {
        public static implicit operator MoveStringUnion(Move move) => new MoveMoveStringUnion(move);
        public static implicit operator MoveStringUnion(string value) => new StringMoveStringUnion(value);
    }

    public record MoveMoveStringUnion(Move Move) : MoveStringUnion;
    public record StringMoveStringUnion(string Value) : MoveStringUnion;


    /// <summary>
    /// boolean | "pursuitfaint"
    /// </summary>
    public abstract record SwitchInReturnBase
    {
        public static implicit operator SwitchInReturnBase(bool value) => new BoolSwitchInReturn(value);

        public static implicit operator SwitchInReturnBase(string value) =>
            value == "pursuitfaint"
                ? new PursuitFaintSwitchInReturn(value)
                : throw new ArgumentException("Must be 'pursuitfaint' for PursuitFaintSwitchInReturn");
    }

    public record BoolSwitchInReturn(bool Value) : SwitchInReturnBase;
    public record PursuitFaintSwitchInReturn(string Value) : SwitchInReturnBase;


    /// <summary>
    /// int | false | ''
    /// </summary>
    public abstract record TryMoveHitReturn
    {
        public static implicit operator TryMoveHitReturn(int value) => new IntTryMoveHitReturn(value);

        public static implicit operator TryMoveHitReturn(bool value) =>
            value
                ? throw new ArgumentException("Must be 'false' for FalseTryMoveHitReturn")
                : new FalseTryMoveHitReturn(value);

        public static implicit operator TryMoveHitReturn(string value) =>
            string.IsNullOrEmpty(value)
                ? new StringTryMoveHitReturn(value)
                : throw new ArgumentException("Must be an empty string for StringTryMoveHitReturn");
    }
    public record IntTryMoveHitReturn(int Value) : TryMoveHitReturn;
    public record FalseTryMoveHitReturn(bool Value) : TryMoveHitReturn;
    public record StringTryMoveHitReturn(string Value) : TryMoveHitReturn;

    /// <summary>
    /// Condition | string
    /// </summary>
    public abstract record ConditionStrUnion
    {
        public static implicit operator ConditionStrUnion(Condition condition) =>
            new ConditionConStrUnion(condition);

        public static implicit operator ConditionStrUnion(string value) => new StringConStrUnion(value);
    }
    public record ConditionConStrUnion(Condition Condition) : ConditionStrUnion;
    public record StringConStrUnion(string Value) : ConditionStrUnion;


    /// <summary>
    /// Pokemon | 'debug'
    /// </summary>
    public abstract record AddSideConditionSource
    {
        public static implicit operator AddSideConditionSource(Pokemon pokemon) =>
            new PokemonAddSideConditionSource(pokemon);
        public static implicit operator AddSideConditionSource(string value) =>
            value == "debug" ? new DebugAddSideConditionSource(value) :
                throw new ArgumentException("Must be 'debug'");
    }
    public record PokemonAddSideConditionSource(Pokemon Pokemon) : AddSideConditionSource;
    public record DebugAddSideConditionSource(string Value) : AddSideConditionSource;


    ///// <summary>
    ///// Move | string
    ///// </summary>
    //public abstract record MoveStrUnion
    //{
    //    public static implicit operator MoveStrUnion(Move move) => new MoveMoveStrUnion(move);
    //    public static implicit operator MoveStrUnion(string value) => new StringMoveStrUnion(value);
    //}
    //public record MoveMoveStrUnion(Move Move) : MoveStrUnion;
    //public record StringMoveStrUnion(string Value) : MoveStrUnion;


    /// <summary>
    /// Species | string
    /// </summary>
    public abstract record SpeciesStrUnion
    {
        public static implicit operator SpeciesStrUnion(Species species) => new SpeciesSpeciesStrUnion(species);
        public static implicit operator SpeciesStrUnion(string value) => new StringSpeciesStrUnion(value);
    }
    public record SpeciesSpeciesStrUnion(Species Species) : SpeciesStrUnion;
    public record StringSpeciesStrUnion(string Value) : SpeciesStrUnion;


    /// <summary>
    /// ActiveMove | string
    /// </summary>
    public abstract record ActiveMoveStringUnion
    {
        public static implicit operator ActiveMoveStringUnion(ActiveMove activeMove) =>
            new ActiveMoveActiveMoveStringUnion(activeMove);
        public static implicit operator ActiveMoveStringUnion(string value) =>
            new StringActiveMoveStringUnion(value);
    }
    public record ActiveMoveActiveMoveStringUnion(ActiveMove ActiveMove) : ActiveMoveStringUnion;
    public record StringActiveMoveStringUnion(string Value) : ActiveMoveStringUnion;


    /// <summary>
    /// Ability | string
    /// </summary>
    public abstract record AbilityStringUnion
    {
        public static implicit operator AbilityStringUnion(Ability ability) =>
            new AbilityAbilityStringUnion(ability);
        public static implicit operator AbilityStringUnion(string value) => new StringAbilityStringUnion(value);
    }
    public record AbilityAbilityStringUnion(Ability Ability) : AbilityStringUnion;
    public record StringAbilityStringUnion(string Value) : AbilityStringUnion;


    /// <summary>
    /// string | false
    /// </summary>
    public abstract record StringFalseUnion
    {
        public static implicit operator StringFalseUnion(string value) => new StringStringFalseUnion(value);
        public static implicit operator StringFalseUnion(bool value) =>
            value ? throw new ArgumentException("Must be 'false'") : new FalseStringFalseUnion();
    }
    public record StringStringFalseUnion(string Value) : StringFalseUnion;
    public record FalseStringFalseUnion : StringFalseUnion;


    /// <summary>
    /// Item | string
    /// </summary>
    public abstract record ItemStringUnion
    {
        public static implicit operator ItemStringUnion(Item item) => new ItemItemStringUnion(item);
        public static implicit operator ItemStringUnion(string value) => new StringItemStringUnion(value);
    }
    public record ItemItemStringUnion(Item Item) : ItemStringUnion;
    public record StringItemStringUnion(string Value) : ItemStringUnion;


    /// <summary>
    /// Item | boolean
    /// </summary>
    public abstract record ItemBoolUnion
    {
        public static implicit operator ItemBoolUnion(Item item) => new ItemItemBoolUnion(item);
        public static implicit operator ItemBoolUnion(bool value) =>
            value ? throw new ArgumentException("Must be 'false'") : new FalseItemBoolUnion();
    }
    public record ItemItemBoolUnion(Item Item) : ItemBoolUnion;
    public record FalseItemBoolUnion : ItemBoolUnion;


    /// <summary>
    /// IEffect | string
    /// </summary>
    public abstract record EffectStringUnion
    {
        public static implicit operator EffectStringUnion(Ability ability) =>
            EffectUnionFactory.ToEffectStringUnion(ability);
        public static implicit operator EffectStringUnion(ItemData itemData) =>
            EffectUnionFactory.ToEffectStringUnion(itemData);
        public static implicit operator EffectStringUnion(ActiveMove activeMove) =>
            EffectUnionFactory.ToEffectStringUnion(activeMove);
        public static implicit operator EffectStringUnion(Species species) =>
            EffectUnionFactory.ToEffectStringUnion(species);
        public static implicit operator EffectStringUnion(Condition condition) =>
            EffectUnionFactory.ToEffectStringUnion(condition);
        public static implicit operator EffectStringUnion(Format format) =>
            EffectUnionFactory.ToEffectStringUnion(format);

        public static implicit operator EffectStringUnion(string value) => new StringEffectStringUnion(value);
    }
    public record EffectEffectStringUnion(IEffect Effect) : EffectStringUnion;
    public record StringEffectStringUnion(string Value) : EffectStringUnion;


    /// <summary>
    /// Bool | 0
    /// </summary>
    public abstract record BoolZeroUnion
    {
        public static implicit operator BoolZeroUnion(bool value) => new BoolBoolZeroUnion(value);
        public static implicit operator BoolZeroUnion(int value) =>
            value == 0 ? new ZeroBoolZeroUnion(value) :
                throw new ArgumentException("Must be 0 for ZeroBoolZeroUnion");
    }
    public record BoolBoolZeroUnion(bool Value) : BoolZeroUnion;
    public record ZeroBoolZeroUnion(int Value) : BoolZeroUnion;


    /// <summary>
    /// SideId | '' | Side
    /// </summary>
    public abstract record WinSideUnion
    {
        public static implicit operator WinSideUnion(SideId sideId) => new SideIdWinSideUnion(sideId);
        public static implicit operator WinSideUnion(string value) =>
            string.IsNullOrEmpty(value) ? new EmptyStringWinSideUnion(value) :
                throw new ArgumentException("Must be an empty string for EmptyStringWinSideUnion");
        public static implicit operator WinSideUnion(Side side) => new SideWinSideUnion(side);
    }
    public record SideIdWinSideUnion(SideId SideId) : WinSideUnion;
    public record EmptyStringWinSideUnion(string Value) : WinSideUnion;
    public record SideWinSideUnion(Side Side) : WinSideUnion;


    /// <summary>
    /// bool | 'hidden'
    /// </summary>
    public abstract record PokemonTrapped
    {
        public static implicit operator PokemonTrapped(bool value) => new BoolPokemonTrapped(value);
        public static implicit operator PokemonTrapped(string value) =>
            value == "hidden" ? new HiddenPokemonTrapped(value) :
                throw new ArgumentException("Must be 'hidden' for HiddenPokemonTrapped");
    }
    public record BoolPokemonTrapped(bool Value) : PokemonTrapped;
    public record HiddenPokemonTrapped(string Value) : PokemonTrapped;


    /// <summary>
    /// Id | boolean
    /// </summary>
    public abstract record IdBoolUnion
    {
        public static implicit operator IdBoolUnion(Id id) => new IdBoolUnionId(id);
        public static implicit operator IdBoolUnion(bool value) => new IdBoolUnionBool(value);
    }
    public record IdBoolUnionId(Id Id) : IdBoolUnion;
    public record IdBoolUnionBool(bool Value) : IdBoolUnion;













}