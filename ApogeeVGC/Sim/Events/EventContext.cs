using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Strongly-typed context passed to all event handlers.
/// Provides type-safe access to event participants and state without reflection.
/// </summary>
public sealed class EventContext
{
    // === Core References ===
    
    /// <summary>
    /// The battle instance (always available).
    /// </summary>
    public required Battle Battle { get; init; }
    
  /// <summary>
  /// The event being triggered.
    /// </summary>
    public required EventId EventId { get; init; }
    
    // === Event Targets ===
    
    /// <summary>
    /// Target Pokemon (if this is a Pokemon-targeted event).
 /// </summary>
    public Pokemon? TargetPokemon { get; init; }
    
    /// <summary>
    /// Target Side (if this is a side-targeted event).
    /// </summary>
    public Side? TargetSide { get; init; }
    
  /// <summary>
    /// Target Field (if this is a field-targeted event).
    /// </summary>
    public Field? TargetField { get; init; }
    
    // === Event Sources ===
    
    /// <summary>
    /// Source Pokemon (the Pokemon triggering this event).
  /// </summary>
    public Pokemon? SourcePokemon { get; init; }
  
    /// <summary>
    /// Source effect (ability, item, move, condition, etc. that triggered this event).
    /// </summary>
    public IEffect? SourceEffect { get; init; }
    
    // === Event-Specific Data ===
    
    /// <summary>
    /// The active move being used (for move-related events).
    /// </summary>
 public ActiveMove? Move { get; init; }
    
    /// <summary>
    /// Relay variable for passing values through event chain.
    /// Can be modified by handlers to affect subsequent handlers or return values.
    /// </summary>
    public RelayVar? RelayVar { get; init; }

    // === Type-Safe Accessors ===
    
  /// <summary>
    /// Gets the target Pokemon, throwing if not available.
    /// </summary>
    public Pokemon GetTargetPokemon() =>
        TargetPokemon ?? throw new InvalidOperationException(
            $"Event {EventId} does not have a target Pokemon");
    
    /// <summary>
  /// Gets the source Pokemon, throwing if not available.
    /// </summary>
    public Pokemon GetSourcePokemon() =>
        SourcePokemon ?? throw new InvalidOperationException(
  $"Event {EventId} does not have a source Pokemon");

    /// <summary>
    /// Gets the move, throwing if not available.
    /// </summary>
    public ActiveMove GetMove() =>
     Move ?? throw new InvalidOperationException(
     $"Event {EventId} does not have a move");
    
    /// <summary>
    /// Gets the source effect cast to a specific type, throwing if wrong type.
    /// </summary>
    public TEffect GetSourceEffect<TEffect>() where TEffect : class, IEffect =>
        SourceEffect as TEffect ?? throw new InvalidOperationException(
      $"Event {EventId} source effect is not {typeof(TEffect).Name} (got {SourceEffect?.GetType().Name ?? "null"})");
    
    /// <summary>
    /// Tries to get the source effect as a specific type.
    /// </summary>
    public TEffect? TryGetSourceEffect<TEffect>() where TEffect : class, IEffect =>
        SourceEffect as TEffect;
    
    /// <summary>
    /// Gets the target side, throwing if not available.
    /// </summary>
    public Side GetTargetSide() =>
        TargetSide ?? TargetPokemon?.Side ?? throw new InvalidOperationException(
     $"Event {EventId} does not have a target side");
    
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
