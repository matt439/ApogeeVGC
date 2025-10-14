using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC.Sim.BattleClasses;

public record FaintQueue
{
    public required Pokemon Target { get; init; }
    public Pokemon? Source { get; init; }
    public IEffect? Effect { get; init; }
}

public class BattleAsync : IBattle
{
    public BattleId Id { get; init; }
    public bool DebugMode { get; init; }
    public bool? ForceRandomChange { get; init; }
    public bool Deserialized { get; init; }
    public bool StrictChoices { get; init; }
    public Format Format { get; init; }
    public EffectState FormatData { get; init; }
    public GameType GameType { get; init; }
    public int ActivePerHalf
    {
        get;
        init
        {
            if (value is not 1)
            {
                throw new ArgumentOutOfRangeException(nameof(ActivePerHalf), "ActivePerHalf must be 1.");
            }
            field = value;
        }
    }
    public Field Field { get; init; }

    public List<Side> Sides
    {
        get;
        init
        {
            if (value.Count != 2)
            {
                throw new ArgumentException("There must be exactly 2 sides in a battle.", nameof(Sides));
            }
            field = value;
        }
    }
    public PrngSeed PrngSeed { get; init; }
    public ModdedDex Dex { get; set; }
    public int Gen => 9;
    public RuleTable RuleTable { get; set; }

    public Prng Prng { get; set; }
    public bool Rated { get; set; }
    public bool ReportExactHp { get; set; } = false;
    public bool ReportPercentages { get; set; } = false;
    public bool SupportCancel { get; set; } = false;

    public BattleActions Actions { get; set; }
    public BattleQueue Queue { get; set; }
    public List<FaintQueue> FaintQueue { get; init; } = [];

    public List<string> Log { get; set; } = [];
    public List<string> InputLog { get; set; } = [];
    public List<string> MessageLog { get; set; } = [];
    public int SentLogPos { get; set; } = 0;
    public bool SentEnd { get; set; } = false;
    public static bool SentRequests => true;

    public RequestState RequestState { get; set; } = RequestState.None;
    public int Turn { get; set; } = 0;
    public bool MidTurn { get; set; } = false;
    public bool Started { get; set; } = false;
    public bool Ended { get; set; } = false;
    public PlayerId? Winnder { get; set; }

    public IEffect Effect { get; set; }
    public EffectState EffectState { get; set; }

    public Event Event { get; set; } = new();
    public Event? Events { get; set; } = null;
    public int EventDepth { get; set; }

    public ActiveMove? ActiveMove { get; set; } = null;
    public Pokemon? ActivePokemon { get; set; } = null;
    public Pokemon? ActiveTarget { get; set; } = null;

    public ActiveMove? LastMove { get; set; } = null;
    public MoveId? LastSuccessfulMoveThisTurn { get; set; } = null;
    public int LastMoveLine { get; set; } = -1;
    public int LastDamage { get; set; } = 0;
    public int EffectOrder { get; set; }
    public bool QuickClawRoll { get; set; } = false;
    public List<int> SpeedOrder { get; set; } = [];

    // TeamGenerator
    // Hints

    public static Undefined NotFail => new();
    public static int HitSubstiture => 0;
    public static bool Fail => false;
    public const object? SilentFail = null;

    public Action<SendType, IReadOnlyList<string>> Send { get; init; }

    //public Func<int, int?, int> Trunc { get; init; }
    //public Func<int, int?, int?, int> ClampIntRange { get; init; }


    public Library Library { get; init; }
    public bool PrintDebug { get; init; }

    public BattleAsync(BattleOptions options, Library library)
    {
        Library = library;
        Dex = new ModdedDex(Library);
        RuleTable = new RuleTable();
        Field = new Field(this);

        Format = options.Format ?? Library.Formats[options.Id];
        // RuleTable
        Id = BattleId.Default;
        DebugMode = options.Debug;
        ForceRandomChange = options.ForceRandomChance;
        Deserialized = options.Deserialized;
        StrictChoices = options.StrictChoices;
        FormatData = InitEffectState(Format.FormatId);
        GameType = Format.GameType;
        Sides = new List<Side>(2)
        {
            [0] = new Side(this),
            [1] = new Side(this),
        };
        ActivePerHalf = 1;
        Prng = options.Prng ?? new Prng(options.Seed);
        PrngSeed = Prng.StartingSeed;

        Rated = options.Rated ?? false;

        Queue = new BattleQueue(this);
        Actions = new BattleActions(this);

        Effect = null!; // TODO: Fix nullability
        EffectState = InitEffectState();

        for (int i = 0; i < ActivePerHalf * 2; i++)
        {
            SpeedOrder.Add(i);
        }

        // TeamGenerator
        // Hints

        Send = options.Send ?? ((_, _) => { });

        // InputOptions

        if (options.P1 is not null)
        {
            SetPlayer(SideId.P1, options.P1);
        }
        if (options.P2 is not null)
        {
            SetPlayer(SideId.P2, options.P2);
        }
    }

    public RelayVar? SingleEvent(EventId eventId, IEffect effect, EffectState? state = null,
        SingleEventTarget? target = null, SingleEventSource? source = null, IEffect? sourceEffect = null,
        RelayVar? relayVar = null, EffectDelegate? customCallback = null)
    {
        // Check for stack overflow
        if (EventDepth >= 8)
        {
            UiGenerator.PrintMessage("STACK LIMIT EXCEEDED");
            UiGenerator.PrintMessage($"Event: {eventId}");
            UiGenerator.PrintMessage($"Parent event: {Event.Id}");
            throw new InvalidOperationException("Stack overflow");
        }

        // Check for infinite loop
        if (Log.Count - SentLogPos > 1000)
        {
            UiGenerator.PrintMessage("STACK LIMIT EXCEEDED");
            UiGenerator.PrintMessage($"Event: {eventId}");
            UiGenerator.PrintMessage($"Parent event: {Event.Id}");
            throw new InvalidOperationException("Infinite loop");
        }

        // Track if relayVar was explicitly provided
        bool hasRelayVar = relayVar != null;
        relayVar ??= new BoolRelayVar(true);

        // Check if status effect has changed
        if (effect.EffectType ==  EffectType.Status &&
            target is PokemonSingleEventTarget pokemonTarget)
        {
            Pokemon targetPokemon = pokemonTarget.Pokemon;
            if (effect is Condition condition && targetPokemon.Status != condition.Id)
            {
                // Status has changed; abort the event
                return relayVar;
            }
        }

        // Check if ability is suppressed by Mold Breaker
        if (eventId == EventId.SwitchIn && 
            effect.EffectType == EffectType.Ability && 
            effect is Ability { Flags.Breakable: true } &&
            target is PokemonSingleEventTarget moldbreakerTarget &&
            SuppressingAbility(moldbreakerTarget.Pokemon))
        {
            return relayVar;
        }

        // Check if item is suppressed
        if (eventId != EventId.Start && 
            eventId != EventId.TakeItem && 
            effect.EffectType == EffectType.Item &&
            target is PokemonSingleEventTarget itemTarget &&
            itemTarget.Pokemon.IgnoringItem())
        {
            return relayVar;
        }

        // Check if ability is suppressed by Gastro Acid/Neutralizing Gas
        if (eventId != EventId.End && 
            effect.EffectType == EffectType.Ability &&
            target is PokemonSingleEventTarget abilityTarget &&
            abilityTarget.Pokemon.IgnoringAbility())
        {
            return relayVar;
        }

        // Check if weather is suppressed
        if (effect.EffectType == EffectType.Weather &&
            eventId != EventId.FieldStart &&
            eventId != EventId.FieldResidual &&
            eventId != EventId.FieldEnd &&
            Field.SuppressingWeather())
        {
            return relayVar;
        }

        // Get the callback - either custom or from the effect
        EffectDelegate? callback = customCallback ?? effect.GetDelegate(eventId);
        if (callback == null) return relayVar;

        // Save parent context
        IEffect parentEffect = Effect;
        EffectState parentEffectState = EffectState;
        Event parentEvent = Event;

        // Set up new event context
        Effect = effect;
        EffectState = state ?? InitEffectState();
        Event = new Event
        {
            Id = eventId,
            Target = target,
            Source = source,
            Effect = sourceEffect,
        };
        EventDepth++;

        // Invoke the callback with appropriate parameters
        RelayVar? returnVal;
        try
        {
            returnVal = InvokeEventCallback(callback, hasRelayVar, relayVar, target, source, sourceEffect);
        }
        finally
        {
            // Restore parent context
            EventDepth--;
            Effect = parentEffect;
            EffectState = parentEffectState;
            Event = parentEvent;
        }

        return returnVal ?? relayVar;
    }

    /// <summary>
    /// Invokes an event callback with the appropriate parameters based on its signature.
    /// </summary>
    private RelayVar? InvokeEventCallback(EffectDelegate callback, bool hasRelayVar, RelayVar relayVar, 
        SingleEventTarget? target, SingleEventSource? source, IEffect? sourceEffect)
    {
        // Handle non-function callbacks (constants)
        switch (callback)
        {
            case OnFlinchEffectDelegate { OnFlinch: OnFlinchBool ofb }:
                return new BoolRelayVar(ofb.Value);
            case OnCriticalHitEffectDelegate { OnCriticalHit: OnCriticalHitBool ocb }:
                return new BoolRelayVar(ocb.Value);
            case OnFractionalPriorityEffectDelegate { OnFractionalPriority: OnFrationalPriorityNeg ofpn }:
                return new DecimalRelayVar(ofpn.Value);
            case OnTakeItemEffectDelegate { OnTakeItem: OnTakeItemBool otib }:
                return new BoolRelayVar(otib.Value);
            case OnTryHealEffectDelegate { OnTryHeal: OnTryHealBool othb }:
                return new BoolRelayVar(othb.Value);
            case OnTryEatItemEffectDelegate { OnTryEatItem: BoolOnTryEatItem botei }:
                return new BoolRelayVar(botei.Value);
            case OnNegateImmunityEffectDelegate { OnNegateImmunity: OnNegateImmunityBool onib }:
                return new BoolRelayVar(onib.Value);
            case OnLockMoveEffectDelegate { OnLockMove: OnLockMoveMoveId olmmi }:
                return new MoveIdRelayVar(olmmi.Id);
        }

        // Extract the actual delegate
        Delegate del = callback switch
        {
            DelegateEffectDelegate ded => ded.Del,
            OnFlinchEffectDelegate { OnFlinch: OnFlinchFunc off } => off.Func,
            OnCriticalHitEffectDelegate { OnCriticalHit: OnCriticalHitFunc ocf } => ocf.Function,
            OnFractionalPriorityEffectDelegate { OnFractionalPriority: OnFractionalPriorityFunc ofpf } => ofpf.Function,
            OnTakeItemEffectDelegate { OnTakeItem: OnTakeItemFunc otif } => otif.Func,
            OnTryHealEffectDelegate { OnTryHeal: OnTryHealFunc1 othf1 } => othf1.Func,
            OnTryHealEffectDelegate { OnTryHeal: OnTryHealFunc2 othf2 } => othf2.Func,
            OnTryEatItemEffectDelegate { OnTryEatItem: FuncOnTryEatItem fotei } => fotei.Func,
            OnNegateImmunityEffectDelegate { OnNegateImmunity: OnNegateImmunityFunc onif } => onif.Func,
            OnLockMoveEffectDelegate { OnLockMove: OnLockMoveFunc olmf } => olmf.Func,
            _ => throw new InvalidOperationException($"Unknown EffectDelegate type: {callback.GetType().Name}"),
        };
        
        // Build parameter list - most delegates expect (IBattle, ...) as first parameter(s)
        var args = new List<object?>();
        var parameters = del.Method.GetParameters();
        
        // First parameter is typically IBattle (this)
        if (parameters.Length > 0 && parameters[0].ParameterType.IsAssignableFrom(typeof(IBattle)))
        {
            args.Add(this);
        }

        // Add relayVar if it was explicitly provided and if the delegate expects it
        // This is typically the second parameter after IBattle
        if (hasRelayVar && args.Count < parameters.Length)
        {
            args.Add(relayVar);
        }

        // Add remaining standard parameters: target, source, sourceEffect
        // Convert union types to the expected parameter types
        while (args.Count < parameters.Length)
        {
            Type paramType = parameters[args.Count].ParameterType;
            
            // Try to match target parameter
            if (args.Count < parameters.Length && target != null)
            {
                EventTargetParameter? targetParam = EventTargetParameter.FromSingleEventTarget(target, paramType);
                if (targetParam != null)
                {
                    args.Add(targetParam.ToObject());
                    continue;
                }
            }
            
            // Try to match source parameter
            if (args.Count < parameters.Length && source != null)
            {
                EventSourceParameter? sourceParam = EventSourceParameter.FromSingleEventSource(source, paramType);
                if (sourceParam != null)
                {
                    args.Add(sourceParam.ToObject());
                    continue;
                }
            }
            
            // Try to match sourceEffect parameter
            if (args.Count < parameters.Length && sourceEffect != null)
            {
                if (paramType.IsInstanceOfType(sourceEffect))
                {
                    args.Add(sourceEffect);
                    continue;
                }
            }
            
            // If we couldn't match, add null
            args.Add(null);
        }

        // Invoke the callback
        object? result = del.DynamicInvoke([.. args]);

        return (RelayVar?)result;
    }

    public RelayVar? RunEvent(EventId eventId, RunEventTarget? target = null, RunEventSource? source = null,
        IEffect? sourceEffect = null, RelayVar? relayVar = null, bool? onEffect = null, bool? fastExit = null)
    {
        throw new NotImplementedException();
    }

    //findEventHandlers(target: Pokemon | Pokemon[] | Side | Battle, eventName: string, source?: Pokemon | null)
    //{
    //    let handlers: EventListener[] = [];
    //    if (Array.isArray(target))
    //    {
    //        for (const [i, pokemon] of target.entries()) {
    //            // console.log(`Event: ${eventName}, Target: ${pokemon}, ${i}`);
    //            const curHandlers = this.findEventHandlers(pokemon, eventName, source);
    //            for (const handler of curHandlers) {
    //                handler.target = pokemon; // Original "effectHolder"
    //                handler.index = i;
    //            }
    //            handlers = handlers.concat(curHandlers);
    //        }
    //        return handlers;
    //    }
    //    // events that target a Pokemon normally bubble up to the Side
    //    const shouldBubbleDown = target instanceof Side;
    //    // events usually run through EachEvent should never have any handlers besides `on${eventName}` so don't check for them
    //    const prefixedHandlers = ! ['BeforeTurn', 'Update', 'Weather', 'WeatherChange', 'TerrainChange'].includes(eventName);
    //    if (target instanceof Pokemon && (target.isActive || source?.isActive)) {
    //        handlers = this.findPokemonEventHandlers(target, `on${ eventName}`);
    //        if (prefixedHandlers)
    //        {
    //            for (const allyActive of target.alliesAndSelf()) {
    //                handlers.push(...this.findPokemonEventHandlers(allyActive, `onAlly${ eventName}`));
    //                handlers.push(...this.findPokemonEventHandlers(allyActive, `onAny${ eventName}`));
    //            }
    //            for (const foeActive of target.foes()) {
    //                handlers.push(...this.findPokemonEventHandlers(foeActive, `onFoe${ eventName}`));
    //                handlers.push(...this.findPokemonEventHandlers(foeActive, `onAny${ eventName}`));
    //            }
    //        }
    //        target = target.side;
    //    }
    //    if (source && prefixedHandlers)
    //    {
    //        handlers.push(...this.findPokemonEventHandlers(source, `onSource${ eventName}`));
    //    }
    //    if (target instanceof Side) {
    //        for (const side of this.sides) {
    //            if (shouldBubbleDown)
    //            {
    //                for (const active of side.active) {
    //                    if (side === target || side === target.allySide)
    //                    {
    //                        handlers = handlers.concat(this.findPokemonEventHandlers(active, `on${ eventName}`));
    //                    }
    //                    else if (prefixedHandlers)
    //                    {
    //                        handlers = handlers.concat(this.findPokemonEventHandlers(active, `onFoe${ eventName}`));
    //                    }
    //                    if (prefixedHandlers) handlers = handlers.concat(this.findPokemonEventHandlers(active, `onAny${ eventName}`));
    //                }
    //            }
    //            if (side.n < 2 || !side.allySide)
    //            {
    //                if (side === target || side === target.allySide)
    //                {
    //                    handlers.push(...this.findSideEventHandlers(side, `on${ eventName}`));
    //                }
    //                else if (prefixedHandlers)
    //                {
    //                    handlers.push(...this.findSideEventHandlers(side, `onFoe${ eventName}`));
    //                }
    //                if (prefixedHandlers) handlers.push(...this.findSideEventHandlers(side, `onAny${ eventName}`));
    //            }
    //        }
    //    }
    //    handlers.push(...this.findFieldEventHandlers(this.field, `on${ eventName}`));
    //    handlers.push(...this.findBattleEventHandlers(`on${ eventName}`));
    //    return handlers;
    //}

    private static List<EventListener> FindEventHandlers(RunEventTarget target, EventId eventName,
        Pokemon? source = null)
    {
        throw new NotImplementedException();
    }

    private static List<EventListener> FindPokemonEventHandlers(Pokemon pokemon, EventId callbackName,
        EffectStateKey? getKey = null)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Finds event handlers registered at the battle level.
    /// This includes handlers from the format (ruleset) and custom event handlers.
    /// </summary>
    /// <param name="callbackName">The event callback name to search for</param>
    /// <param name="getKey">Optional property key to check in effect state (e.g., "duration")</param>
    /// <param name="customHolder">Optional custom effect holder (defaults to this battle)</param>
    /// <returns>List of event listeners found at the battle level</returns>
    private List<EventListener> FindBattleEventHandlers(EventId callbackName, EffectStateKey? getKey = null,
        Pokemon? customHolder = null)
    {
        List<EventListener> handlers = [];

        // Check format (ruleset) for handlers
        EffectDelegate? callback = GetCallback(RunEventTarget.FromIBattle(this), Format, callbackName);
        
        if (callback != null || (getKey != null && FormatData.GetProperty(getKey) != null))
        {
            handlers.Add(ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = Format,
                Callback = callback,
                State = FormatData,
                End = null,
                EffectHolder = customHolder ?? EffectHolder.FromIBattle(this),
            }, callbackName));
        }

        // Check custom event handlers registered in this.Events

        EffectDelegate? effectDelegate = Events?.GetDelegate(callbackName);
        if (Events != null && effectDelegate is not null)
        {


        }

        return handlers;
    }

    private List<EventListener> FindFieldEventHandlers(Field field, EventId callbackName,
        EffectStateKey? getKey = null, Pokemon? customHolder = null)
    {
        return FindFieldEventHandlersInternal(field, callbackName, getKey, customHolder).ToList();
    }

    private IEnumerable<EventListener> FindFieldEventHandlersInternal(Field field, EventId callbackName,
        EffectStateKey? getKey = null, Pokemon? customHolder = null)
    {
        // Check pseudo-weather effects (Trick Room, Gravity, etc.)
        foreach (ConditionId id in field.PseudoWeather.Keys)
        {
            EffectState pseudoWeatherState = field.PseudoWeather[id];
            Condition pseudoWeather = Library.Conditions[id];
            EffectDelegate? callback = GetCallback(field, pseudoWeather, callbackName);
            
            if (callback != null || (getKey != null && pseudoWeatherState.GetProperty(getKey) != null))
            {
                yield return ResolvePriority(new EventListenerWithoutPriority
                {
                    Effect = pseudoWeather,
                    Callback = callback,
                    State = pseudoWeatherState,
                    End = customHolder == null
                        ? EffectDelegate.FromNullableDelegate((Func<ConditionId, bool>)field.RemovePseudoWeather)
                        : null,
                    EffectHolder = customHolder is null ? field : customHolder,
                }, callbackName);
            }
        }

        // Check weather effect
        Condition weather = field.GetWeather();
        EffectDelegate? weatherCallback = GetCallback(field, weather, callbackName);
        if (weatherCallback != null || (getKey != null && Field.WeatherState.GetProperty(getKey) != null))
        {
            yield return ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = weather,
                Callback = weatherCallback,
                State = Field.WeatherState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(field.ClearWeather)
                    : null,
                EffectHolder = customHolder is null ? field : customHolder,
            }, callbackName);
        }

        // Check terrain effect
        Condition terrain = field.GetTerrain();
        EffectDelegate? terrainCallback = GetCallback(field, terrain, callbackName);
        if (terrainCallback != null || (getKey != null && field.TerrainState.GetProperty(getKey) != null))
        {
            yield return ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = terrain,
                Callback = terrainCallback,
                State = field.TerrainState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(field.ClearTerrain)
                    : null,
                EffectHolder = customHolder is null ? field : customHolder,
            }, callbackName);
        }
    }

    private List<EventListener> FindSideEventHandlers(Side side, EventId callbackName,
        EffectStateKey? getKey = null, Pokemon? customHolder = null)
    {
        return FindSideEventHandlersInternal(side, callbackName, getKey, customHolder).ToList();
    }

    private IEnumerable<EventListener> FindSideEventHandlersInternal(Side side, EventId callbackName,
        EffectStateKey? getKey = null, Pokemon? customHolder = null)
    {
        foreach (ConditionId id in side.SideConditions.Keys)
        {
            EffectState sideConditionData = side.SideConditions[id];
            Condition sideCondition = Library.Conditions[id];
            EffectDelegate? callback = GetCallback(side, sideCondition, callbackName);
            if (callback != null || (getKey != null && sideConditionData.GetProperty(getKey) != null))
            {
                yield return ResolvePriority(new EventListenerWithoutPriority
                {
                    Effect = sideCondition,
                    Callback = callback,
                    State = sideConditionData,
                    End = customHolder == null
                        ? EffectDelegate.FromNullableDelegate((Func<ConditionId, bool>)side.RemoveSideCondition)
                        : null,
                    EffectHolder = customHolder is null ? side : customHolder,
                }, callbackName);
            }
        }
    }

    private EffectDelegate? GetCallback(RunEventTarget target, IEffect effect, EventId callbackName)
    {
        EffectDelegate? del = effect.GetDelegate(callbackName);
        Delegate? callback = del?.GetDelegate();

        // Special case: In Gen 5+, abilities and items trigger onStart during SwitchIn
        // instead of having a separate Start event
        if (callback is null &&
            target is PokemonRunEventTarget &&
            Gen >= 5 &&
            callbackName == EventId.SwitchIn &&
            effect.GetDelegate(EventId.AnySwitchIn) == null && // Check onAnySwitchIn doesn't exist
            (IsAbilityOrItem(effect) || IsInnateAbilityOrItem(effect)))
        {
            del = effect.GetDelegate(EventId.Start);
            callback = del?.GetDelegate();
        }

        return EffectDelegate.FromNullableDelegate(callback);
    }

    // Helper method to check if effect is Ability or Item
    private static bool IsAbilityOrItem(IEffect effect)
    {
        return effect.EffectType is EffectType.Ability or EffectType.Item;
    }

    // Helper method to check if effect is an innate ability/item
    private static bool IsInnateAbilityOrItem(IEffect effect)
    {
        if (effect.EffectType != EffectType.Status)
        {
            return false;
        }
        var condition = (Condition)effect;
        return condition.AssociatedItem is not null || condition.AssociatedAbility is not null;
    }

    private EventListener ResolvePriority(EventListenerWithoutPriority h, EventId callbackName)
    {
        // Get event metadata from Library
        EventIdInfo eventInfo = Library.Events[callbackName];

        // Look up order/priority/subOrder from the effect using the delegate system
        // These would need to be added to IEffect interface or accessed via reflection once
        IntFalseUnion? order = h.Effect.GetOrder(callbackName);
        int? priority = h.Effect.GetPriority(callbackName);
        int? subOrder = h.Effect.GetSubOrder(callbackName);

        // Calculate default subOrder if not set
        if (subOrder == 0)
        {
            subOrder = CalculateDefaultSubOrder(h);
        }

        // Determine effectOrder based on event type
        int effectOrder = eventInfo.UsesEffectOrder
            ? (h.State?.EffectOrder ?? 0)
            : 0;

        // Calculate speed if needed
        int speed = 0;
        if (eventInfo.UsesSpeed && h.EffectHolder is PokemonEffectHolder pokemonEffectHolder)
        {
            Pokemon pokemon = pokemonEffectHolder.Pokemon;
            speed = pokemon.Speed;

            // Special case for Magic Bounce
            if (h.Effect.EffectType == EffectType.Ability &&
                h.Effect is Ability { Id: AbilityId.MagicBounce } &&
                callbackName == EventId.AllyTryHitSide)
            {
                speed = pokemon.GetStat(StatIdExceptHp.Spe, unmodified: true, unboosted: true);
            }

            // Apply fractional speed adjustment for switch-in events
            if (eventInfo.UsesFractionalSpeed)
            {
                int fieldPositionValue = pokemon.Side.N * Sides.Count + pokemon.Position;
                speed -= SpeedOrder.IndexOf(fieldPositionValue) / (ActivePerHalf * 2);
            }
        }

        return new EventListener
        {
            Effect = h.Effect,
            Target = h.Target,
            Index = h.Index,
            Callback = h.Callback,
            State = h.State,
            End = h.End,
            EndCallArgs = h.EndCallArgs,
            EffectHolder = h.EffectHolder,
            Order = order ?? IntFalseUnion.FromFalse(),
            Priority = priority ?? 0,
            Speed = speed,
            SubOrder = subOrder ?? 0,
            EffectOrder = effectOrder,
        };
    }

    private int CalculateDefaultSubOrder(EventListenerWithoutPriority listener)
    {
        // Effect type hierarchy for subOrder
        int subOrder = listener.Effect.EffectType switch
        {
            EffectType.Condition => 2,
            EffectType.Weather => 5,
            EffectType.Format => 5,
            EffectType.Rule => 5,
            EffectType.Ruleset => 5,
            EffectType.Ability => 7,
            EffectType.Item => 8,
            _ => 0,
        };

        // Refine for conditions
        if (listener.Effect.EffectType == EffectType.Condition && listener.State?.Target != null)
        {
            subOrder = listener.State.Target switch
            {
                SideEffectStateTarget when listener.State.IsSlotCondition == true => 3,  // Slot condition
                SideEffectStateTarget => 4,                                       // Side condition
                FieldEffectStateTarget => 5,                                      // Field condition
                _ => subOrder,
            };
        }

        // Special abilities
        if (listener.Effect is Ability ability)
        {
            subOrder = ability.Id switch
            {
                AbilityId.PoisonTouch => 6,
                AbilityId.PerishBody => 6,
                AbilityId.Stall => 9,
                _ => subOrder,
            };
        }

        return subOrder;
    }

    public void EachEvent(EventId eventId, IEffect? effect, bool? relayVar)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Modifies a Pokémon's stat stages (boosts) during battle.
    /// 
    /// Process:
    /// 1. Validates the target has HP and is active
    /// 2. Runs ChangeBoost and TryBoost events for modification
    /// 3. Applies boosts via target.BoostBy() for each stat
    /// 4. Logs boost messages using UiGenerator methods
    /// 5. Triggers AfterEachBoost and AfterBoost events
    /// 6. Updates statsRaisedThisTurn/statsLoweredThisTurn flags
    /// 
    /// Returns:
    /// - null if boost succeeded
    /// - 0 if target has no HP
    /// - false if target is inactive or no foes remain (Gen 6+)
    /// </summary>
    public BoolZeroUnion? Boost(SparseBoostsTable boost, Pokemon? target = null, Pokemon? source = null,
        IEffect? effect = null, bool isSecondary = false, bool isSelf = false)
    {
        if (target is null && Event.Target is PokemonSingleEventTarget eventTarget)
        {
            target = eventTarget.Pokemon;
        }
        if (source is null && Event.Source is PokemonSingleEventSource eventSource)
        {
            source = eventSource.Pokemon;
        }
        effect ??= Event.Effect;

        // Validate target has HP
        if (target?.Hp <= 0) return new ZeroBoolZeroUnion();

        // Validate target is active
        if (!(target?.IsActive ?? false)) return new BoolBoolZeroUnion(false);

        // Gen 6+: Check if any foes remain
        if (Gen > 5 && target.Side.FoePokemonLeft() <= 0) return new BoolBoolZeroUnion(false);

        // Run ChangeBoost event to allow modifications
        RelayVar modifiedBoost = RunEvent(EventId.ChangeBoost, target, 
            RunEventSource.FromNullablePokemon(source), effect, boost) ?? boost;

        if (modifiedBoost is not SparseBoostsTableRelayVar modifiedBoostTable)
        {
            throw new InvalidOperationException("ChangeBoost event did not return a valid SparseBoostsTable.");
        }

        // Cap the boosts to valid ranges (-6 to +6)
        SparseBoostsTable cappedBoost = target.GetCappedBoost(modifiedBoostTable.Table);

        // Run TryBoost event to allow prevention
        RelayVar finalBoost = RunEvent(EventId.TryBoost, target, RunEventSource.FromNullablePokemon(source),
                                   effect, cappedBoost) ?? cappedBoost;

        if (finalBoost is not SparseBoostsTableRelayVar finalBoostTable)
        {
            throw new InvalidOperationException("ChangeBoost event did not return a valid SparseBoostsTable.");
        }

        bool? success = null;

        // Apply each boost
        foreach (BoostId boostId in Enum.GetValues<BoostId>())
        {
            int? boostValue = finalBoostTable.Table.GetBoost(boostId);
            if (!boostValue.HasValue) continue;

            // Create a sparse table for just this stat
            var currentBoost = new SparseBoostsTable();
            currentBoost.SetBoost(boostId, boostValue.Value);

            // Apply the boost and get the actual change
            int boostBy = target.BoostBy(currentBoost);

            // Determine if this is a boost or unboost for messaging
            bool isUnboost = boostValue.Value < 0 || target.Boosts.GetBoost(boostId) == -6;

            if (boostBy != 0)
            {
                success = true;

                // Handle special cases
                EffectStateId effectId = effect?.EffectStateId ?? EffectStateId.FromEmpty();
                if (effectId is MoveEffectStateId { MoveId: MoveId.BellyDrum } or
                    AbilityEffectStateId { AbilityId: AbilityId.AngerPoint })
                {
                    // Use PrintSetBoostEvent for moves that set boosts to maximum
                    UiGenerator.PrintSetBoostEvent(target, boostId, boostBy, effect ??
                        throw new ArgumentNullException(nameof(effect)));
                }
                else if (effect is not null)
                {
                    switch (effect.EffectType)
                    {
                        case EffectType.Move:
                            // Regular move boost/unboost
                            break;

                        case EffectType.Item:
                            // Item-triggered boost/unboost (messages handled by UI)
                            break;

                        default:
                            // Ability or other effect type
                            if (effect.EffectType == EffectType.Ability && !isSecondary)
                            {
                                if (effect is not Ability ability)
                                {
                                    throw new InvalidOperationException("Effect is not an Ability.");
                                }
                                UiGenerator.PrintAbilityEvent(target, ability);
                                //boosted = true;
                            }
                            break;
                    }

                    if (isUnboost)
                    {
                        UiGenerator.PrintUnboostEvent(target, boostId, -boostBy, effect);
                    }
                    else
                    {
                        UiGenerator.PrintBoostEvent(target, boostId, boostBy, effect);
                    }

                    break;
                }
                // Trigger AfterEachBoost event
                RunEvent(EventId.AfterEachBoost, target, RunEventSource.FromNullablePokemon(source), effect,
                    currentBoost);
            }
            else if (effect?.EffectType == EffectType.Ability)
            {
                // Ability boost that failed
                if (!isSecondary && !isSelf) continue;
                if (isUnboost)
                {
                    UiGenerator.PrintUnboostEvent(target, boostId, 0, effect);
                }
                else
                {
                    UiGenerator.PrintBoostEvent(target, boostId, 0, effect);
                }
            }
            else if (!isSecondary && !isSelf)
            {
                // Failed boost that should be announced
                if (isUnboost)
                {
                    UiGenerator.PrintUnboostEvent(target, boostId, 0, effect ?? throw new ArgumentNullException(nameof(effect)));
                }
                else
                {
                    UiGenerator.PrintBoostEvent(target, boostId, 0, effect ?? throw new ArgumentNullException(nameof(effect)));
                }
            }
        }

        // Trigger AfterBoost event
        RunEvent(EventId.AfterBoost, target, RunEventSource.FromNullablePokemon(source), effect, finalBoost);

        // Update turn flags
        if (success == true)
        {
            // Check if any boosts were positive
            bool hasPositiveBoost = false;
            bool hasNegativeBoost = false;

            foreach (BoostId boostId in Enum.GetValues<BoostId>())
            {
                int? boostValue = finalBoostTable.Table.GetBoost(boostId);
                switch (boostValue)
                {
                    case null:
                        continue;
                    case > 0:
                        hasPositiveBoost = true;
                        break;
                    case < 0:
                        hasNegativeBoost = true;
                        break;
                }
            }

            if (hasPositiveBoost) target.StatsRaisedThisTurn = true;
            if (hasNegativeBoost) target.StatsLoweredThisTurn = true;
        }

        return success.HasValue ? new BoolBoolZeroUnion(success.Value) : null;
    }

    public IntFalseUnion? Damage(int damage, Pokemon? target = null, Pokemon? source = null,
        BattleDamageEffect? effect = null, bool instafaint = false)
    {
        return null;
    }

    public IntFalseUnion? Heal(int damage, Pokemon? target = null, Pokemon? source = null,
        BattleHealEffect? effect = null)
    {
        return null;
    }

    public StatsTable SpreadModify(StatsTable baseStats, PokemonSet set)
    {
        StatsTable modStats = new();

        // iterate through all stats in baseStats
        foreach (StatId statName in baseStats.Keys)
        {
            modStats[statName] = StatModify(baseStats, set, statName);
        }
        return modStats;
    }

    public int FinalModify(int relayVar)
    {
        relayVar = Modify(relayVar, Event.Modifier ?? 1.0);
        Event.Modifier = 1.0;
        return relayVar;
    }

    public double ChainModify(int numerator, int denominator = 1)
    {
        // Get the current modifier from the event state as fixed-point
        // Default to 1.0 (4096 in fixed-point) if null
        int previousMod = Trunc((int)((Event.Modifier ?? 1.0) * 4096));

        // Convert the new modifier to fixed-point format
        int nextMod = Trunc(numerator * 4096 / denominator);

        // Chain the modifiers together and store back in the event
        // The >> 12 is a right shift by 12 bits (equivalent to dividing by 4096)
        // Add 2048 for proper rounding before the shift
        Event.Modifier = ((previousMod * nextMod + 2048) >> 12) / 4096.0;
        return nextMod;
    }

    public double ChainModify(int[] fraction)
    {
        if (fraction.Length != 2)
        {
            throw new ArgumentException("Fraction array must have exactly 2 elements [numerator, denominator]", nameof(fraction));
        }

        return ChainModify(fraction[0], fraction[1]);
    }

    public double ChainModify(double fraction)
    {
        if (fraction <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(fraction), "Fraction must be greater than 0.");
        }
        // Convert the double fraction to a fixed-point representation
        int fixedPointFraction = Trunc((int)(fraction * 4096));
        // Chain the fixed-point modification
        int previousMod = Trunc((int)((Event.Modifier ?? 1.0) * 4096));
        Event.Modifier = ((previousMod * fixedPointFraction + 2048) >> 12) / 4096.0;
        return fixedPointFraction;
    }

    public int Modify(int value, int numerator, int denominator = 1)
    {
        // Calculate the 4096-based fixed-point modifier
        int modifier = Trunc(numerator * 4096 / denominator);
        
        // Apply the modifier with proper rounding
        return Trunc((Trunc(value * modifier) + 2048 - 1) / 4096);
    }

    public int Modify(int value, int[] fraction)
    {
        if (fraction.Length != 2)
        {
            throw new ArgumentException("Fraction array must have exactly 2 elements [numerator, denominator]", nameof(fraction));
        }

        return Modify(value, fraction[0], fraction[1]);
    }

    public int Modify(int value, double fraction)
    {
        if (fraction <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(fraction), "Fraction must be greater than 0.");
        }
        // Convert the double fraction to a fixed-point representation
        int fixedPointFraction = Trunc((int)(fraction * 4096));
        // Apply the fixed-point modification
        return Trunc((Trunc(value * fixedPointFraction) + 2048 - 1) / 4096);
    }

    public bool CheckMoveMakesContact(Move move, Pokemon attacker, Pokemon defender, bool announcePads = false)
    {
        if (move.Flags.Contact is not true || !attacker.HasItem(ItemId.ProtectivePads))
        {
            return move.Flags.Contact is true;
        }
        if (!announcePads) return false;
        UiGenerator.PrintActivateEvent(defender, Effect);
        UiGenerator.PrintActivateEvent(attacker, Library.Items[ItemId.ProtectivePads]);
        return false;
    }

    public bool RandomChance(int numerator, int denominator)
    {
        return ForceRandomChange ?? Prng.RandomChance(numerator, denominator);
    }

    public int Random(int m, int n)
    {
        return Prng.Random(m, n);
    }

    public int Random(int n)
    { 
        return Prng.Random(n);
    }

    public double Random()
    {
        return Prng.Random();
    }

    public IBattle Copy()
    {
        throw new NotImplementedException();
    }

    public int ClampIntRange(int num, int? min, int? max)
    {
        if (num < min)
        {
            return min.Value;
        }
        return num > max ? max.Value : num;
    }

    public Pokemon? GetAtSlot(PokemonSlot? slot)
    {
        if (slot is null) return null;
        Side side = GetSide(slot.SideId);
        int position = (int)slot.PositionLetter;
        int positionOffset = (int)Math.Floor(side.N / 2.0) * side.Active.Count;
        return side.Active[position - positionOffset];
    }

    private Side GetSide(SideId id)
    {
        return id switch
        {
            SideId.P1 => Sides[0],
            SideId.P2 => Sides[1],
            _ => throw new ArgumentOutOfRangeException(nameof(id), $"Invalid SideId: {id}"),
        };
    }

    //initEffectState(obj: Partial<EffectState>, effectOrder?: number) : EffectState {
    //    if (!obj.id) obj.id = '';
    //    if (effectOrder !== undefined) {
    //        obj.effectOrder = effectOrder;
    //    } else if (obj.id && obj.target && (!(obj.target instanceof Pokemon) || obj.target.isActive)) {
    //        obj.effectOrder = this.effectOrder++;
    //    } else {
    //        obj.effectOrder = 0;
    //    }
    //    return obj as EffectState;
    //}

    /// <summary>
    /// Initializes an EffectState object with proper effect ordering.
    /// Effect order is used to determine priority when multiple effects trigger.
    /// - Effects with explicit effectOrder use that value
    /// - Effects on active Pokemon/entities get auto-incremented order
    /// - Effects on inactive targets get order 0
    /// </summary>
    public EffectState InitEffectState(EffectStateId? id = null, int? effectOrder = null, Pokemon? target = null)
    {
        // Create new EffectState with the provided or default ID
        EffectStateId effectId = id ?? EffectStateId.FromEmpty();

        int finalEffectOrder;

        if (effectOrder.HasValue)
        {
            // If an effect order is explicitly provided, use it
            finalEffectOrder = effectOrder.Value;
        }
        else if (effectId != EffectStateId.FromEmpty() && target != null)
        {
            // Auto-assign effect order for effects on targets
            // Only increment for active Pokemon, otherwise use 0
            // Use the battle's master counter for active effects
            finalEffectOrder = target.IsActive ? EffectOrder++ : 0;
        }
        else
        {
            // Effects with no ID or no target get a default order of 0
            finalEffectOrder = 0;
        }

        // Create and return the EffectState
        return new EffectState
        {
            Id = effectId,
            EffectOrder = finalEffectOrder,
            Duration = null,

            // TODO: Initialize other properties as needed
        };
    }

    public EffectState InitEffectState(EffectStateId id, Pokemon? source, PokemonSlot? sourceSlot, int? duration)
    {
        // Use the first overload to handle basic initialization and effect ordering
        // Pass the source Pokemon as the target for effect ordering purposes
        EffectState state = InitEffectState(id, effectOrder: null, target: source);

        // Add the additional properties specific to this overload
        state.Source = source;
        state.SourceSlot = sourceSlot;
        state.Duration = duration;

        return state;
    }

    public MoveCategory GetCategory(ActiveMove move)
    {
        return move.Category;
    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public List<Pokemon> GetAllActive(bool includeFainted = false)
    {
        List<Pokemon> pokemnoList = [];
        foreach (Side side in Sides)
        {
            pokemnoList.AddRange(side.Active.Where(pokemon => includeFainted || !pokemon.Fainted));
        }
        return pokemnoList;
    }

    /// <summary>
    /// Truncate a number to an unsigned 32-bit integer.
    /// If bits is specified, the number is scaled, truncated, then unscaled.
    /// This is used for precise damage calculations in Pokemon battles.
    /// </summary>
    public int Trunc(int num, int bits = 0)
    {
        if (bits == 0)
        {
            // Simple case: just return the integer as-is
            return num;
        }

        // For 16-bit truncation (used in nature calculations):
        // Truncate to 16 bits by masking with 0xFFFF (65535)
        // This matches the game's behavior for overflow prevention
        if (bits == 16)
        {
            return num & 0xFFFF;
        }

        // For other bit counts, scale up by 2^bits, truncate, then scale back down
        // This effectively performs: Math.Floor(num / (2^bits)) * (2^bits)
        int divisor = 1 << bits; // 2^bits
        return (num / divisor) * divisor;
    }

    /// <summary>
    /// Calculate a single stat value using Pokemon's official stat calculation formula.
    /// HP uses: floor(floor(2 * base + IV + floor(EV/4) + 100) * level / 100 + 10)
    /// Other stats use: floor(floor(2 * base + IV + floor(EV/4)) * level / 100 + 5)
    /// Then nature modifiers are applied with 16-bit truncation.
    /// </summary>
    public int StatModify(StatsTable baseStats, PokemonSet set, StatId statName)
    {
        int stat = baseStats.GetStat(statName);
        int iv = set.Ivs.GetStat(statName);
        int ev = set.Evs.GetStat(statName);

        // HP calculation uses a different formula
        if (statName == StatId.Hp)
        {
            // HP = floor(floor(2 * base + IV + floor(EV/4) + 100) * level / 100 + 10)
            return Trunc(Trunc(2 * stat + iv + Trunc(ev / 4) + 100) * set.Level / 100 + 10);
        }

        // Other stats: floor(floor(2 * base + IV + floor(EV/4)) * level / 100 + 5)
        stat = Trunc(Trunc(2 * stat + iv + Trunc(ev / 4)) * set.Level / 100 + 5);

        // Apply nature modifiers
        Nature nature = set.Nature;

        // Natures are calculated with 16-bit truncation
        // This only affects Eternatus-Eternamax in Pure Hackmons
        if (nature.Plus == statName.ConvertToStatIdExceptId())
        {
            // Positive nature: multiply by 1.1 (110/100)
            // Overflow protection: cap at 595 if rule is enabled
            stat = RuleTable.Has(RuleId.OverflowStatMod) ? Math.Min(stat, 595) : stat;
            stat = Trunc(Trunc(stat * 110, 16) / 100);
        }
        else if (nature.Minus == statName.ConvertToStatIdExceptId())
        {
            // Negative nature: multiply by 0.9 (90/100)
            // Overflow protection: cap at 728 if rule is enabled
            stat = RuleTable.Has(RuleId.OverflowStatMod) ? Math.Min(stat, 728) : stat;
            stat = Trunc(Trunc(stat * 90, 16) / 100);
        }
        return stat;
    }

    /// <summary>
    /// Determines if the current active move is suppressing abilities.
    /// Returns true if:
    /// - There's an active Pokemon that is active (not fainted)
    /// - The active Pokemon is not the target (or Gen &lt; 8)
    /// - There's an active move that ignores abilities
    /// - The target doesn't have an Ability Shield
    /// Used for abilities like Mold Breaker, Teravolt, Turboblaze and moves like
    /// Sunsteel Strike, Moongeist Beam that ignore target abilities.
    /// </summary>
    public bool SuppressingAbility(Pokemon? target = null)
    {
        // Check if there's an active Pokemon and it's currently active
        if (ActivePokemon is not { IsActive: true })
        {
            return false;
        }

        // In Gen 8+, moves can't suppress their user's own ability
        // In earlier gens, they could
        if (ActivePokemon == target && Gen >= 8)
        {
            return false;
        }

        // Check if there's an active move that ignores abilities
        if (ActiveMove is not { IgnoreAbility: true })
        {
            return false;
        }

        // Ability Shield protects against ability suppression
        return target?.HasItem(ItemId.AbilityShield) != true;
    }

    private void SetPlayer(SideId slot, PlayerOptions options)
    {
        Side? side;
        bool didSomething = true;

        // Convert SideId enum to array index (P1=0, P2=1)
        int slotNum = slot == SideId.P1 ? 0 : 1;

        if (!Sides[slotNum].Initialised)
        {
            // Create new player
            var team = GetTeam(options);
            string playerName = options.Name ?? $"Player {slotNum + 1}";
            side = new Side(playerName, this, slot, [.. team])
            {
                Name = playerName,
                Avatar = options.Avatar ?? string.Empty,
                Team = [.. team],
                Pokemon = [],
                Active = [],
                SideConditions = [],
                SlotConditions = [],
                Choice = new Choice
                {
                    CantUndo = false,
                    Actions = [],
                    ForcedSwitchesLeft = 0,
                    ForcedPassesLeft = 0,
                    SwitchIns = [],
                    Terastallize = false,
                },
            };

            Sides[slotNum] = side;
        }
        else
        {
            // Edit existing player
            side = Sides[slotNum];
            didSomething = false;

            // Update name if different
            if (!string.IsNullOrEmpty(options.Name) && side.Name != options.Name)
            {
                side.Name = options.Name;
                didSomething = true;
            }

            // Update avatar if different
            if (!string.IsNullOrEmpty(options.Avatar) && side.Avatar != options.Avatar)
            {
                side.Avatar = options.Avatar;
                didSomething = true;
            }

            // Prevent team changes for existing players
            if (options.Team != null)
            {
                throw new InvalidOperationException($"Player {slot} already has a team!");
            }
        }

        // Exit early if no changes were made
        if (!didSomething) return;

        // Log the player setup
        string optionsJson = System.Text.Json.JsonSerializer.Serialize(options);
        InputLog.Add($"> player {slot} {optionsJson}");

        // Add player info to battle log
        string rating = options.Rating?.ToString() ?? string.Empty;
        Log.Add($"|player|{side.Id}|{side.Name}|{side.Avatar}|{rating}");

        // Start battle if all sides are ready and battle hasn't started
        if (Sides.All(playerSide => !playerSide.Initialised) && !Started)
        {
            Start();
        }
    }

    private static IReadOnlyList<PokemonSet> GetTeam(PlayerOptions options)
    {
        return options.Team ?? throw new InvalidOperationException();
    }

    public int Randomizer(int baseDamage)
    {
        return Trunc(Trunc(baseDamage * (100 - Random(16))) / 100);
    }

    /// <summary>
    /// The default sort order for actions, but also event listeners.
    /// 
    /// 1. Order, low to high (default last)
    /// 2. Priority, high to low (default 0)
    /// 3. Speed, high to low (default 0)
    /// 4. SubOrder, low to high (default 0)
    /// 5. EffectOrder, low to high (default 0)
    /// 
    /// This is a static comparison function that doesn't reference battle state.
    /// </summary>
    public static int ComparePriority(IPriorityComparison a, IPriorityComparison b)
    {
        // 1. Order comparison (lower values first, Max = last)
        // ActionOrder.Max represents items without explicit order
        int orderResult = a.Order.CompareTo(b.Order);
        if (orderResult != 0) return orderResult;

        // 2. Priority comparison (higher values first)
        int priorityResult = b.Priority.CompareTo(a.Priority); // Reversed for descending
        if (priorityResult != 0) return priorityResult;

        // 3. Speed comparison (higher values first)
        int speedResult = b.Speed.CompareTo(a.Speed); // Reversed for descending
        if (speedResult != 0) return speedResult;

        // 4. SubOrder comparison (lower values first)
        int subOrderResult = a.SubOrder.CompareTo(b.SubOrder);
        if (subOrderResult != 0) return subOrderResult;

        // 5. EffectOrder comparison (lower values first)
        return a.EffectOrder.CompareTo(b.EffectOrder);
    }


    /// <summary>
    /// Sort a list, resolving speed ties the way the games do.
    /// 
    /// This uses a Selection Sort algorithm - not the fastest sort in general, but
    /// actually faster than QuickSort for small arrays like the ones SpeedSort is used for.
    /// More importantly, it makes it easiest to resolve speed ties properly through
    /// randomization via Prng.Shuffle().
    /// </summary>
    /// <typeparam name="T">Type that implements IPriorityComparison for sorting</typeparam>
    /// <param name="list">List to sort in-place</param>
    /// <param name="comparator">Comparison function (defaults to ComparePriority)</param>
    public void SpeedSort<T>(List<T> list, Func<T, T, int>? comparator = null) 
        where T : IPriorityComparison
    {
        // Default to ComparePriority if no comparator provided
        comparator ??= (a, b) => ComparePriority(a, b);

        // Nothing to sort for lists with less than 2 elements
        if (list.Count < 2) return;

        int sorted = 0;

        // Selection Sort with speed tie resolution
        while (sorted + 1 < list.Count)
        {
            // Start with the first unsorted element
            List<int> nextIndexes = [sorted];

            // Find all elements that should come next (including ties)
            for (int i = sorted + 1; i < list.Count; i++)
            {
                int delta = comparator(list[nextIndexes[0]], list[i]);

                switch (delta)
                {
                    case < 0:
                        // Current element is already better, skip
                        continue;
                    case > 0:
                        // Found a better element, start new list
                        nextIndexes = [i];
                        break;
                    // delta == 0
                    default:
                        // Speed tie - add to list of tied elements
                        nextIndexes.Add(i);
                        break;
                }
            }

            // Place the next elements in their sorted positions
            for (int i = 0; i < nextIndexes.Count; i++)
            {
                int index = nextIndexes[i];
                if (index != sorted + i)
                {
                    // Swap elements into place
                    // nextIndexes is guaranteed to be in order, so it will never have
                    // been disturbed by an earlier swap
                    (list[sorted + i], list[index]) = (list[index], list[sorted + i]);
                }
            }

            // If there are multiple elements with the same priority (speed ties),
            // shuffle them randomly to fairly resolve the tie
            if (nextIndexes.Count > 1)
            {
                Prng.Shuffle(list, sorted, sorted + nextIndexes.Count);
            }

            sorted += nextIndexes.Count;
        }
    }

    public void ClearEffectState(EffectState state)
    {
        EffectStateTarget? prevTarget = state.Target;
        state = new EffectState()
        {
            Id = EffectStateId.FromEmpty(),
            Target = prevTarget,
            EffectOrder = 0,
        };
    }
}