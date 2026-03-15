using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Strongly-typed context passed to all event handlers.
/// Provides type-safe access to event participants and state without reflection.
/// Instances are pooled by <see cref="Battle"/> to avoid per-invocation heap allocations.
/// </summary>
public sealed class EventContext
{
    // === Core References ===

    /// <summary>
    /// The battle instance (always available).
    /// </summary>
    public Battle Battle { get; set; } = null!;

  /// <summary>
  /// The event being triggered.
    /// </summary>
    public EventId EventId { get; set; }

    /// <summary>
    /// The effect whose handler is being invoked (e.g., the Item, Ability, Move, or Condition that owns this handler).
    /// </summary>
    public IEffect? Effect { get; set; }

    // === Event Targets ===

    /// <summary>
    /// Target Pokemon (if this is a Pokemon-targeted event).
 /// </summary>
    public Pokemon? TargetPokemon { get; set; }

    /// <summary>
    /// Target Side (if this is a side-targeted event).
    /// </summary>
    public Side? TargetSide { get; set; }

  /// <summary>
    /// Target Field (if this is a field-targeted event).
    /// </summary>
    public Field? TargetField { get; set; }

    // === Event Sources ===

    /// <summary>
    /// Source Pokemon (the Pokemon triggering this event).
  /// </summary>
    public Pokemon? SourcePokemon { get; set; }

    /// <summary>
    /// Source type (for type-specific events like Effectiveness).
    /// </summary>
    public PokemonType? SourceType { get; set; }

    /// <summary>
    /// Source effect (ability, item, move, condition, etc. that triggered this event).
    /// </summary>
    public IEffect? SourceEffect { get; set; }

    // === Event-Specific Data ===

    /// <summary>
    /// The active move being used (for move-related events).
    /// </summary>
 public ActiveMove? Move { get; set; }

    /// <summary>
    /// Relay variable for passing values through event chain.
    /// Can be modified by handlers to affect subsequent handlers or return values.
    /// </summary>
    public RelayVar? RelayVar { get; set; }

    /// <summary>
    /// Resets all fields so this instance can be returned to a pool.
    /// </summary>
    internal void ResetForPool()
    {
        Battle = null!;
        EventId = default;
        Effect = null;
        TargetPokemon = null;
        TargetSide = null;
        TargetField = null;
        SourcePokemon = null;
        SourceType = null;
        SourceEffect = null;
        Move = null;
        RelayVar = null;
    }

    // === Type-Safe Accessors ===

  /// <summary>
    /// Gets the target Pokemon, or null if not available.
    /// Returns null rather than throwing to match legacy adapter behavior
    /// where handlers receive null for missing participants.
    /// </summary>
    public Pokemon GetTargetPokemon() => TargetPokemon!;
    
    /// <summary>
  /// Gets the source Pokemon, or null if not available.
    /// </summary>
    public Pokemon GetSourcePokemon() => SourcePokemon!;

    /// <summary>
    /// Gets the move, or null if not available.
    /// </summary>
    public ActiveMove GetMove() => Move!;
    
    /// <summary>
    /// Gets the source effect cast to a specific type, or null if not available/wrong type.
    /// Returns null rather than throwing to match legacy adapter behavior.
    /// </summary>
    public TEffect GetSourceEffect<TEffect>() where TEffect : class, IEffect =>
        (SourceEffect as TEffect)!;
    
    /// <summary>
    /// Tries to get the source effect as a specific type.
    /// </summary>
    public TEffect? TryGetSourceEffect<TEffect>() where TEffect : class, IEffect =>
        SourceEffect as TEffect;
    
    /// <summary>
    /// Gets the target side, or null if not available.
    /// </summary>
    public Side GetTargetSide() =>
        (TargetSide ?? TargetPokemon?.Side)!;
    
 /// <summary>
    /// Gets the relay variable, throwing if not available.
    /// </summary>
    public RelayVar GetRelayVar() =>
 RelayVar ?? throw new InvalidOperationException(
        $"Event {EventId} does not have a relay variable");
    
    /// <summary>
    /// Gets the relay variable as a specific type, throwing if wrong type.
    /// </summary>
    public TRelayVar GetRelayVar<TRelayVar>() where TRelayVar : RelayVar =>
 RelayVar as TRelayVar ?? throw new InvalidOperationException(
            $"Event {EventId} relay variable is not {typeof(TRelayVar).Name} (got {RelayVar?.GetType().Name ?? "null"})");
    
    /// <summary>
    /// Tries to get the relay variable as a specific type.
    /// </summary>
    public TRelayVar? TryGetRelayVar<TRelayVar>() where TRelayVar : RelayVar =>
        RelayVar as TRelayVar;
    
    // === Relay Var Conversion Helpers ===
    // These mirror the type conversions in EventHandlerAdapter.TryUnwrapRelayVar
    // so that Create factories can handle the same relay var types as the legacy adapter.

    /// <summary>
    /// Gets the relay var value as an int, handling both IntRelayVar and DecimalRelayVar.
    /// Matches legacy adapter behavior where DecimalRelayVar can be unwrapped to int.
    /// </summary>
    public int GetIntRelayVar() =>
        RelayVar switch
        {
            IntRelayVar irv => irv.Value,
            DecimalRelayVar drv => (int)drv.Value,
            _ => throw new InvalidOperationException(
                $"Event {EventId} relay variable cannot be converted to int (got {RelayVar?.GetType().Name ?? "null"})")
        };

    /// <summary>
    /// Tries to get the relay var value as a nullable int, handling IntRelayVar, DecimalRelayVar,
    /// and BoolRelayVar(true) → null (for accuracy events).
    /// </summary>
    public int? GetNullableIntRelayVar() =>
        RelayVar switch
        {
            IntRelayVar irv => irv.Value,
            DecimalRelayVar drv => (int)drv.Value,
            BoolRelayVar { Value: true } => null,
            null => null,
            _ => throw new InvalidOperationException(
                $"Event {EventId} relay variable cannot be converted to int? (got {RelayVar?.GetType().Name})")
        };

    /// <summary>
    /// Gets an effect from the relay var (for events like SetStatus where Condition is passed as RelayVar).
    /// Matches legacy adapter behavior for EffectRelayVar unwrapping.
    /// </summary>
    public TEffect GetRelayVarEffect<TEffect>() where TEffect : class, IEffect =>
        RelayVar is EffectRelayVar effectRelayVar && effectRelayVar.Effect is TEffect typed
            ? typed
            : throw new InvalidOperationException(
                $"Event {EventId} relay variable does not contain {typeof(TEffect).Name} (got {RelayVar?.GetType().Name ?? "null"})");

    /// <summary>
    /// Tries to get an effect from the relay var.
    /// </summary>
    public TEffect? TryGetRelayVarEffect<TEffect>() where TEffect : class, IEffect =>
        RelayVar is EffectRelayVar effectRelayVar ? effectRelayVar.Effect as TEffect : null;

    /// <summary>
    /// Gets an effect parameter matching the legacy adapter's resolution order:
    /// 1. Relay var (if it contains an EffectRelayVar wrapping the correct type)
    /// 2. Effect (the handler's owning effect)
    /// 3. SourceEffect
    /// This handles the case where earlier handlers in a RunEvent chain replace the
    /// relay var (e.g., from EffectRelayVar to BoolRelayVar), and the effect must be
    /// resolved from the handler's owning effect instead.
    /// </summary>
    public TEffect GetEffectParam<TEffect>() where TEffect : class, IEffect
    {
        if (RelayVar is EffectRelayVar erv && erv.Effect is TEffect fromRelay)
            return fromRelay;
        if (Effect is TEffect fromEffect)
            return fromEffect;
        if (SourceEffect is TEffect fromSource)
            return fromSource;
        throw new InvalidOperationException(
            $"Event {EventId} cannot resolve {typeof(TEffect).Name} from relay var, effect, or source effect " +
            $"(RelayVar={RelayVar?.GetType().Name ?? "null"}, Effect={Effect?.GetType().Name ?? "null"}, SourceEffect={SourceEffect?.GetType().Name ?? "null"})");
    }

    /// <summary>
    /// Tries to get an effect parameter, returning null if not found.
    /// Uses the same resolution order as <see cref="GetEffectParam{TEffect}"/>.
    /// In RunEvent handler chains, earlier handlers may replace the relay var
    /// (e.g., from EffectRelayVar to BoolRelayVar), making the original effect
    /// unresolvable. This mirrors TypeScript's dynamic typing where handlers
    /// receive the current relay var regardless of its type.
    /// </summary>
    public TEffect? TryGetEffectParam<TEffect>() where TEffect : class, IEffect
    {
        if (RelayVar is EffectRelayVar erv && erv.Effect is TEffect fromRelay)
            return fromRelay;
        if (Effect is TEffect fromEffect)
            return fromEffect;
        if (SourceEffect is TEffect fromSource)
            return fromSource;
        return null;
    }

    /// <summary>
    /// Gets a SparseBoostsTable from the relay var, handling both SparseBoostsTableRelayVar
    /// and BoostsTableRelayVar (with conversion). Matches legacy adapter TryUnwrapRelayVar behavior.
    /// </summary>
    public SparseBoostsTable GetSparseBoostsTableRelayVar() =>
        RelayVar switch
        {
            SparseBoostsTableRelayVar srv => srv.Table,
            BoostsTableRelayVar brv => SparseBoostsTable.FromBoostsTable(brv.Table),
            _ => throw new InvalidOperationException(
                $"Event {EventId} relay variable cannot be converted to SparseBoostsTable (got {RelayVar?.GetType().Name ?? "null"})")
        };

    // === Fallback Pokemon Accessors ===
    // These match the legacy adapter behavior where unnamed Pokemon params resolve to
    // TargetPokemon ?? SourcePokemon. Use when either participant may be null.

    /// <summary>
    /// Gets the Pokemon at the handler's first Pokemon param position.
    /// In Showdown's runEvent callback: (relayVar, target, source, effect)
    /// "target" = runEvent's first arg = Event.Target (attacker for damage events).
    /// </summary>
    public Pokemon GetSourceOrTargetPokemon() => (SourcePokemon ?? TargetPokemon)!;

    /// <summary>
    /// Gets the Pokemon at the handler's second Pokemon param position.
    /// In Showdown's runEvent callback: (relayVar, target, source, effect)
    /// "source" = runEvent's second arg = Event.Source (defender for damage events).
    /// </summary>
    public Pokemon GetTargetOrSourcePokemon() => (TargetPokemon ?? SourcePokemon)!;

    // === Convenience Properties ===

    /// <summary>
    /// True if this event has a target Pokemon.
    /// </summary>
  public bool HasTargetPokemon => TargetPokemon != null;
    
    /// <summary>
    /// True if this event has a source Pokemon.
    /// </summary>
  public bool HasSourcePokemon => SourcePokemon != null;
    
    /// <summary>
    /// True if this event has a move.
    /// </summary>
    public bool HasMove => Move != null;
    
    /// <summary>
    /// True if this event has a relay variable.
    /// </summary>
  public bool HasRelayVar => RelayVar != null;
    
    /// <summary>
    /// True if this event has a source effect.
    /// </summary>
    public bool HasSourceEffect => SourceEffect != null;
}
