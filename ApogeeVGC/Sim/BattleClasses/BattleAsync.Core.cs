using ApogeeVGC.Data;
using ApogeeVGC.Sim.Actions;
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
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Extensions;
using static ApogeeVGC.Sim.PokemonClasses.Pokemon;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync : IBattle, IDisposable
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
    public int SentLogPos { get; set; }
    public bool SentEnd { get; set; }
    public static bool SentRequests => true;

    public RequestState RequestState { get; set; } = RequestState.None;
    public int Turn { get; set; }
    public bool MidTurn { get; set; }
    public bool Started { get; set; }
    public bool Ended { get; set; }
    public string? Winner { get; set; }

    public IEffect Effect { get; set; }
    public EffectState EffectState { get; set; }

    public Event Event { get; set; } = new();
    public Event? Events { get; set; } = null;
    public int EventDepth { get; set; }

    public ActiveMove? ActiveMove { get; set; }
    public Pokemon? ActivePokemon { get; set; }
    public Pokemon? ActiveTarget { get; set; }

    public ActiveMove? LastMove { get; set; }
    public MoveId? LastSuccessfulMoveThisTurn { get; set; }
    public int LastMoveLine { get; set; } = -1;
    public int LastDamage { get; set; }
    public int EffectOrder { get; set; }
    public bool QuickClawRoll { get; set; }
    public List<int> SpeedOrder { get; set; } = [];

    // TeamGenerator
    // Hints

    public static Undefined NotFail => new();
    public int HitSubstitute => 0;
    public static bool Fail => false;
    public const object? SilentFail = null;

    public Action<SendType, IEnumerable<string>> Send { get; init; }

    public Library Library { get; init; }
    public bool DisplayUi { get; init; }
    public Side P1 => Sides[0];
    public Side P2 => Sides[1];
    public static Side P3 => throw new Exception("3v3 battles are not implemented.");
    public static Side P4 => throw new Exception("4v4 battles are not implemented.");
    private HashSet<string> Hints { get; set; } = [];

    private bool _disposed;

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
            new Side(this),
            new Side(this),
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

        Send = options.Send ?? ((type, messages) => { });

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

    public bool RandomChance(int numerator, int denominator)
    {
        return ForceRandomChange ?? Prng.RandomChance(numerator, denominator);
    }

    public T Sample<T>(IReadOnlyList<T> items)
    {
        return Prng.Sample(items);
    }

    public void ResetRng(PrngSeed? seed = null)
    {
        Prng = new Prng(seed);
    }

    

    

    

    

    

    

    

    

    

    

    

    

    

    

    

    

    

    

    public static IReadOnlyList<PokemonSet> GetTeam(PlayerOptions options)
    {
        return options.Team ?? throw new InvalidOperationException();
    }

    

    

    

    
    

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

    public EffectState InitEffectState(EffectStateId id, Side target, Pokemon source, PokemonSlot sourceSlot,
        bool isSlotCondition, int? duration)
    {
        // Use the first overload to handle basic initialization and effect ordering
        // Side conditions are considered inactive for effect ordering purposes
        EffectState state = InitEffectState(id, effectOrder: null, target: null);
        // Add the additional properties specific to this overload
        state.Target = target;
        state.Source = source;
        state.SourceSlot = sourceSlot;
        state.IsSlotCondition = isSlotCondition;
        state.Duration = duration;
        return state;
    }

    public void ClearEffectState(ref EffectState state)
    {
        EffectStateTarget? prevTarget = state.Target;
        state = new EffectState
        {
            Id = EffectStateId.FromEmpty(),
            Target = prevTarget,
            EffectOrder = 0,
        };
    }

    /// <summary>
    /// Disposes of battle resources and breaks circular references.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed resources
            Field.Destroy();

            foreach (Side side in Sides)
            {
                side.Destroy();
            }

            Queue.Clear();
            Log.Clear();
            InputLog.Clear();
            MessageLog.Clear();
            FaintQueue.Clear();
        }

        // Mark as disposed
        _disposed = true;
    }

    /// <summary>
    /// Public method for backwards compatibility with original API
    /// </summary>
    public void Destroy()
    {
        Dispose();
    }

    public IBattle Copy()
    {
        throw new NotImplementedException();
    }

    

    #region Helpers

    


    /// <summary>
    /// Invokes a DelegateEffectDelegate by attempting common delegate signatures.
    /// Falls back to reflection only when necessary.
    /// Optimized to minimize allocations by reusing a fixed-size array.
    /// </summary>
    private RelayVar? InvokeDelegateEffectDelegate(Delegate del, bool hasRelayVar, RelayVar relayVar,
        SingleEventTarget? target, SingleEventSource? source, IEffect? sourceEffect)
    {
        // Cache parameter info to avoid repeated reflection calls
        var parameters = del.Method.GetParameters();
        int paramCount = parameters.Length;

        // Most common signature: (IBattle battle, ...)
        if (paramCount == 0)
        {
            return (RelayVar?)del.DynamicInvoke(null);
        }

        // Optimize for the most common cases (1-5 parameters)
        // This avoids array allocation for the majority of callbacks
        object? result;
        switch (paramCount)
        {
            case 1:
                result = del.DynamicInvoke(BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect));
                return (RelayVar?)result;
            case 2:
                result = del.DynamicInvoke(
                    BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect),
                    BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source, sourceEffect, 1)
                );
                return (RelayVar?)result;
            case 3:
                result = del.DynamicInvoke(
                    BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect),
                    BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source, sourceEffect, 1),
                    BuildSingleArg(parameters[2], hasRelayVar, relayVar, target, source, sourceEffect, 2)
                );
                return (RelayVar?)result;
            case 4:
                result = del.DynamicInvoke(
                    BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect),
                    BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source, sourceEffect, 1),
                    BuildSingleArg(parameters[2], hasRelayVar, relayVar, target, source, sourceEffect, 2),
                    BuildSingleArg(parameters[3], hasRelayVar, relayVar, target, source, sourceEffect, 3)
                );
                return (RelayVar?)result;
        }

        // Fallback for 5+ parameters (rare)
        // Use array allocation for these cases
        object?[] args = new object?[paramCount];
        int argIndex = 0;

        // First parameter is typically IBattle (this)
        if (parameters[0].ParameterType.IsAssignableFrom(typeof(IBattle)))
        {
            args[argIndex++] = this;
        }

        // Add relayVar if it was explicitly provided and if the delegate expects it
        if (hasRelayVar)
        {
            args[argIndex++] = relayVar;
        }

        // Add remaining standard parameters: target, source, sourceEffect
        while (argIndex < paramCount)
        {
            Type paramType = parameters[argIndex].ParameterType;

            // Try to match target parameter
            if (target != null)
            {
                EventTargetParameter? targetParam = EventTargetParameter.FromSingleEventTarget(target, paramType);
                if (targetParam != null)
                {
                    args[argIndex++] = targetParam.ToObject();
                    continue;
                }
            }

            // Try to match source parameter
            if (source != null)
            {
                EventSourceParameter? sourceParam = EventSourceParameter.FromSingleEventSource(source, paramType);
                if (sourceParam != null)
                {
                    args[argIndex++] = sourceParam.ToObject();
                    continue;
                }
            }

            // Try to match sourceEffect parameter
            if (sourceEffect != null && paramType.IsInstanceOfType(sourceEffect))
            {
                args[argIndex++] = sourceEffect;
                continue;
            }

            // If we couldn't match, add null
            args[argIndex++] = null;
        }

        result = del.DynamicInvoke(args);
        return (RelayVar?)result;
    }

    /// <summary>
    /// Builds a single argument for delegate invocation.
    /// Used by the optimized fast-path for common parameter counts.
    /// </summary>
    private object? BuildSingleArg(System.Reflection.ParameterInfo param, bool hasRelayVar, RelayVar relayVar,
        SingleEventTarget? target, SingleEventSource? source, IEffect? sourceEffect, int position = 0)
    {
        Type paramType = param.ParameterType;

        // First parameter is typically IBattle
        if (position == 0 && paramType.IsAssignableFrom(typeof(IBattle)))
        {
            return this;
        }

        // Second parameter might be relayVar if explicitly provided
        if (position == 1 && hasRelayVar)
        {
            return relayVar;
        }

        // Adjust position if IBattle was first
        int adjustedPos = position;
        if (position > 0 && paramType.IsAssignableFrom(typeof(IBattle)))
        {
            adjustedPos--;
        }
        if (hasRelayVar && adjustedPos > 0)
        {
            adjustedPos--;
        }

        // Try to match standard parameters in order: target, source, sourceEffect
        switch (adjustedPos)
        {
            case 0:
                // Try target first
                if (target != null)
                {
                    EventTargetParameter? targetParam = EventTargetParameter.FromSingleEventTarget(target, paramType);
                    if (targetParam != null) return targetParam.ToObject();
                }
                break;
            case 1:
                // Try source second
                if (source != null)
                {
                    EventSourceParameter? sourceParam = EventSourceParameter.FromSingleEventSource(source, paramType);
                    if (sourceParam != null) return sourceParam.ToObject();
                }
                break;
            case 2:
                // Try sourceEffect third
                if (sourceEffect != null && paramType.IsInstanceOfType(sourceEffect))
                {
                    return sourceEffect;
                }
                break;
        }

        return null;
    }

    

    

    #endregion
}