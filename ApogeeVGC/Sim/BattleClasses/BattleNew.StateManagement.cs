//using ApogeeVGC.Player;
//using ApogeeVGC.Sim.PokemonClasses;
//using ApogeeVGC.Sim.FieldClasses;

//namespace ApogeeVGC.Sim.BattleClasses;

//public partial class BattleNew
//{
//    /// <summary>
//    /// Create a deep copy of the current battle state
//    /// </summary>
//    public BattleStateSnapshot CreateStateSnapshot()
//    {
//        return new BattleStateSnapshot
//        {
//            TurnCounter = TurnCounter,
//            Side1 = Side1.Copy(),
//            Side2 = Side2.Copy(),
//            Field = Field.Copy(),
//            Player1TotalTime = _player1TotalTime,
//            Player2TotalTime = _player2TotalTime,
//            GameStartTime = _gameStartTime,
//            SnapshotTime = DateTime.UtcNow,
//        };
//    }

//    /// <summary>
//    /// Restore battle state from a snapshot (for rollback scenarios)
//    /// </summary>
//    public void RestoreFromSnapshot(BattleStateSnapshot snapshot)
//    {
//        if (PrintDebug)
//            Console.WriteLine($"Restoring battle state from snapshot at turn {snapshot.TurnCounter}");

//        // Note: This is a dangerous operation and should only be used in specific scenarios
//        // It breaks the immutable history model, so use with caution

//        TurnCounter = snapshot.TurnCounter;
        
//        // Copy the side states
//        Side1.SetSlotsWithCopies(snapshot.Side1.AllSlots.ToList());
//        Side2.SetSlotsWithCopies(snapshot.Side2.AllSlots.ToList());
        
//        // Restore field state
//        // Field = snapshot.Field.Copy(); // This would require a way to set field state
        
//        // Restore timing
//        _player1TotalTime = snapshot.Player1TotalTime;
//        _player2TotalTime = snapshot.Player2TotalTime;
//        _gameStartTime = snapshot.GameStartTime;

//        if (PrintDebug)
//            Console.WriteLine("Battle state restored successfully");
//    }

//    /// <summary>
//    /// Apply a set of changes to the battle state
//    /// </summary>
//    public void ApplyStateChanges(List<StateChange> changes)
//    {
//        foreach (var change in changes)
//        {
//            ApplyStateChange(change);
//        }
//    }

//    /// <summary>
//    /// Apply a single state change to the battle
//    /// </summary>
//    private void ApplyStateChange(StateChange change)
//    {
//        switch (change.Type)
//        {
//            case StateChangeType.PokemonDamage:
//                ApplyPokemonDamage(change);
//                break;
                
//            case StateChangeType.PokemonHeal:
//                ApplyPokemonHeal(change);
//                break;
                
//            case StateChangeType.PokemonFaint:
//                ApplyPokemonFaint(change);
//                break;
                
//            case StateChangeType.StatusChange:
//                ApplyStatusChange(change);
//                break;
                
//            case StateChangeType.StatChange:
//                ApplyStatChange(change);
//                break;
                
//            case StateChangeType.FieldChange:
//                ApplyFieldChange(change);
//                break;
                
//            default:
//                if (PrintDebug)
//                    Console.WriteLine($"Unknown state change type: {change.Type}");
//                break;
//        }
//    }

//    /// <summary>
//    /// Apply damage to a Pokemon
//    /// </summary>
//    private void ApplyPokemonDamage(StateChange change)
//    {
//        var pokemon = GetPokemonFromReference(change.TargetReference);
//        if (pokemon != null && change.Amount.HasValue)
//        {
//            int damageAmount = change.Amount.Value;
//            int actualDamage = pokemon.Damage(damageAmount);
            
//            if (PrintDebug)
//                Console.WriteLine($"{pokemon.Specie.Name} takes {actualDamage} damage ({pokemon.CurrentHp}/{pokemon.UnmodifiedHp} HP)");
//        }
//    }

//    /// <summary>
//    /// Apply healing to a Pokemon
//    /// </summary>
//    private void ApplyPokemonHeal(StateChange change)
//    {
//        var pokemon = GetPokemonFromReference(change.TargetReference);
//        if (pokemon != null && change.Amount.HasValue)
//        {
//            int healAmount = change.Amount.Value;
//            int actualHeal = pokemon.Heal(healAmount);
            
//            if (PrintDebug)
//                Console.WriteLine($"{pokemon.Specie.Name} heals {actualHeal} HP ({pokemon.CurrentHp}/{pokemon.UnmodifiedHp} HP)");
//        }
//    }

//    /// <summary>
//    /// Apply fainting to a Pokemon
//    /// </summary>
//    private void ApplyPokemonFaint(StateChange change)
//    {
//        var pokemon = GetPokemonFromReference(change.TargetReference);
//        if (pokemon != null)
//        {
//            // Force Pokemon to 0 HP which will set IsFainted to true
//            pokemon.Damage(pokemon.CurrentHp);
            
//            if (PrintDebug)
//                Console.WriteLine($"{pokemon.Specie.Name} fainted!");
//        }
//    }

//    /// <summary>
//    /// Apply status change to a Pokemon
//    /// </summary>
//    private void ApplyStatusChange(StateChange change)
//    {
//        var pokemon = GetPokemonFromReference(change.TargetReference);
//        if (pokemon != null && !string.IsNullOrEmpty(change.StatusId))
//        {
//            // TODO: Apply status change based on your status system
//            if (PrintDebug)
//                Console.WriteLine($"{pokemon.Specie.Name} status changed to {change.StatusId}");
//        }
//    }

//    /// <summary>
//    /// Apply stat change to a Pokemon
//    /// </summary>
//    private void ApplyStatChange(StateChange change)
//    {
//        var pokemon = GetPokemonFromReference(change.TargetReference);
//        if (pokemon != null && !string.IsNullOrEmpty(change.StatId) && change.Amount.HasValue)
//        {
//            // TODO: Apply stat change based on your stat system
//            if (PrintDebug)
//                Console.WriteLine($"{pokemon.Specie.Name} {change.StatId} changed by {change.Amount}");
//        }
//    }

//    /// <summary>
//    /// Apply field change (weather, terrain, etc.)
//    /// </summary>
//    private void ApplyFieldChange(StateChange change)
//    {
//        // TODO: Apply field changes based on your field system
//        if (PrintDebug)
//            Console.WriteLine($"Field change: {change.FieldEffectId}");
//    }

//    /// <summary>
//    /// Get a Pokemon from a reference
//    /// </summary>
//    private Pokemon? GetPokemonFromReference(PokemonReference reference)
//    {
//        var side = reference.PlayerId == PlayerId.Player1 ? Side1 : Side2;
//        var pokemonList = side.AllSlots.ToList();
        
//        if (reference.SlotIndex >= 0 && reference.SlotIndex < pokemonList.Count)
//        {
//            return pokemonList[reference.SlotIndex];
//        }
        
//        return null;
//    }
//}

///// <summary>
///// Snapshot of battle state for rollback scenarios
///// </summary>
//public class BattleStateSnapshot
//{
//    public required int TurnCounter { get; init; }
//    public required Side Side1 { get; init; }
//    public required Side Side2 { get; init; }
//    public required Field Field { get; init; }
//    public required TimeSpan Player1TotalTime { get; init; }
//    public required TimeSpan Player2TotalTime { get; init; }
//    public required DateTime GameStartTime { get; init; }
//    public required DateTime SnapshotTime { get; init; }
//}

///// <summary>
///// Represents a change to battle state
///// </summary>
//public class StateChange
//{
//    public required StateChangeType Type { get; init; }
//    public required PokemonReference TargetReference { get; init; }
//    public int? Amount { get; init; }
//    public string? StatusId { get; init; }
//    public string? StatId { get; init; }
//    public string? FieldEffectId { get; init; }
//}

///// <summary>
///// Reference to a specific Pokemon
///// </summary>
//public class PokemonReference
//{
//    public required PlayerId PlayerId { get; init; }
//    public required int SlotIndex { get; init; }
//}

///// <summary>
///// Types of state changes
///// </summary>
//public enum StateChangeType
//{
//    PokemonDamage,
//    PokemonHeal,
//    PokemonFaint,
//    StatusChange,
//    StatChange,
//    FieldChange
//}

///// <summary>
///// Current battle state for external systems
///// </summary>
//public class CurrentBattleState
//{
//    public required int TurnCounter { get; init; }
//    public required bool IsGameComplete { get; init; }
//    public required string CurrentTurnType { get; init; }
//    public required List<PokemonState> Side1Pokemon { get; init; }
//    public required List<PokemonState> Side2Pokemon { get; init; }
//    public required List<string> FieldConditions { get; init; }
//    public required TimeSpan Player1TimeRemaining { get; init; }
//    public required TimeSpan Player2TimeRemaining { get; init; }
//    public required TimeSpan GameTimeRemaining { get; init; }
//}

///// <summary>
///// State of a single Pokemon
///// </summary>
//public class PokemonState
//{
//    public required string Species { get; init; }
//    public required int CurrentHp { get; init; }
//    public required int MaxHp { get; init; }
//    public required bool Fainted { get; init; }
//    public required bool IsActive { get; init; }
//    public required string StatusCondition { get; init; }
//}