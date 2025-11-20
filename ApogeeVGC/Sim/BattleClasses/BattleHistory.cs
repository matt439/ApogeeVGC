using ApogeeVGC.Sim.Core;
using System.Text;

namespace ApogeeVGC.Sim.BattleClasses;

/// <summary>
/// Tracks battle history for debugging and analysis.
/// Records key events, choices, and state changes throughout the battle.
/// </summary>
public class BattleHistory
{
    private readonly List<HistoryEntry> _entries = new();
    private int _currentTurn = 0;

    /// <summary>
    /// Tracks damage dealt by Player 1
    /// </summary>
    public List<int> P1DamageDealt { get; } = new();

    /// <summary>
    /// Tracks damage dealt by Player 2
    /// </summary>
    public List<int> P2DamageDealt { get; } = new();

    /// <summary>
    /// Tracks damage received by Player 1
    /// </summary>
    public List<int> P1DamageReceived { get; } = new();

    /// <summary>
    /// Tracks damage received by Player 2
    /// </summary>
    public List<int> P2DamageReceived { get; } = new();

    /// <summary>
    /// Tracks moves used by Player 1
    /// </summary>
    public List<string> P1MovesUsed { get; } = new();

    /// <summary>
    /// Tracks moves used by Player 2
    /// </summary>
    public List<string> P2MovesUsed { get; } = new();

    /// <summary>
    /// Records a new turn starting.
    /// </summary>
    public void RecordTurnStart(int turnNumber)
    {
     _currentTurn = turnNumber;
  AddEntry($"=== Turn {turnNumber} Started ===", HistoryEntryType.TurnStart);
    }

    /// <summary>
    /// Records a choice made by a player.
    /// </summary>
    public void RecordChoice(SideId sideId, string choiceDescription)
    {
        AddEntry($"[{sideId}] Choice: {choiceDescription}", HistoryEntryType.Choice);
    }

    /// <summary>
    /// Records a move being used.
    /// </summary>
    public void RecordMove(string pokemonName, string moveName, string? target = null)
    {
   string message = target != null
        ? $"{pokemonName} used {moveName} on {target}"
            : $"{pokemonName} used {moveName}";
        AddEntry(message, HistoryEntryType.Move);
    }

    /// <summary>
    /// Records a move being used with side information for bias analysis.
    /// </summary>
    public void RecordMoveWithSide(string pokemonName, SideId sideId, string moveName, string? target = null)
    {
        // Track moves by side
        if (sideId == SideId.P1)
        {
            P1MovesUsed.Add(moveName);
        }
        else
        {
            P2MovesUsed.Add(moveName);
        }

        string message = target != null
            ? $"{pokemonName} ({sideId}) used {moveName} on {target}"
            : $"{pokemonName} ({sideId}) used {moveName}";
        AddEntry(message, HistoryEntryType.Move);
    }

    /// <summary>
    /// Records damage dealt.
    /// </summary>
    public void RecordDamage(string pokemonName, int damage, int remainingHp, int maxHp)
    {
        AddEntry($"{pokemonName} took {damage} damage ({remainingHp}/{maxHp} HP remaining)", 
            HistoryEntryType.Damage);
    }

    /// <summary>
    /// Records damage dealt with attacker and defender information for bias analysis.
    /// </summary>
    public void RecordDamageWithSides(string attackerName, SideId attackerSide, string defenderName, SideId defenderSide, int damage, int remainingHp, int maxHp)
    {
        // Track damage by side
        if (attackerSide == SideId.P1)
        {
            P1DamageDealt.Add(damage);
            P2DamageReceived.Add(damage);
        }
        else
        {
            P2DamageDealt.Add(damage);
            P1DamageReceived.Add(damage);
        }

        AddEntry($"{attackerName} ({attackerSide}) dealt {damage} damage to {defenderName} ({defenderSide}) - {remainingHp}/{maxHp} HP remaining", 
            HistoryEntryType.Damage);
    }

    /// <summary>
    /// Records healing.
    /// </summary>
    public void RecordHeal(string pokemonName, int healing, int newHp, int maxHp)
    {
        AddEntry($"{pokemonName} healed {healing} HP ({newHp}/{maxHp})", 
            HistoryEntryType.Heal);
    }

    /// <summary>
    /// Records a Pokemon fainting.
    /// </summary>
    public void RecordFaint(string pokemonName)
    {
        AddEntry($"{pokemonName} fainted!", HistoryEntryType.Faint);
    }

    /// <summary>
    /// Records a Pokemon switching.
    /// </summary>
    public void RecordSwitch(SideId sideId, string switchingOut, string switchingIn)
    {
        AddEntry($"[{sideId}] {switchingOut} switched out, {switchingIn} switched in", 
  HistoryEntryType.Switch);
    }

    /// <summary>
    /// Records a status condition being applied.
    /// </summary>
    public void RecordStatus(string pokemonName, string status)
    {
        AddEntry($"{pokemonName} was inflicted with {status}", HistoryEntryType.Status);
    }

    /// <summary>
    /// Records a stat change.
    /// </summary>
    public void RecordStatChange(string pokemonName, string stat, int stages)
    {
        string direction = stages > 0 ? "rose" : "fell";
        string amount = Math.Abs(stages) == 1 ? "" : $" {Math.Abs(stages)} stages";
        AddEntry($"{pokemonName}'s {stat} {direction}{amount}", HistoryEntryType.StatChange);
    }

    /// <summary>
    /// Records an ability activation.
    /// </summary>
    public void RecordAbility(string pokemonName, string abilityName, string effect)
    {
        AddEntry($"{pokemonName}'s {abilityName}: {effect}", HistoryEntryType.Ability);
    }

    /// <summary>
    /// Records an item usage.
    /// </summary>
 public void RecordItem(string pokemonName, string itemName, string effect)
    {
      AddEntry($"{pokemonName}'s {itemName}: {effect}", HistoryEntryType.Item);
    }

    /// <summary>
    /// Records weather/terrain/field effects.
    /// </summary>
    public void RecordField(string effect)
    {
        AddEntry($"Field: {effect}", HistoryEntryType.Field);
    }

    /// <summary>
    /// Records an error or unexpected state.
    /// </summary>
    public void RecordError(string errorMessage)
    {
        AddEntry($"ERROR: {errorMessage}", HistoryEntryType.Error);
    }

    /// <summary>
    /// Records battle end.
    /// </summary>
 public void RecordBattleEnd(string? winner)
  {
string message = winner != null 
    ? $"=== Battle Ended - Winner: {winner} ===" 
            : "=== Battle Ended - Tie ===";
        AddEntry(message, HistoryEntryType.BattleEnd);
    }

    /// <summary>
    /// Records a custom debug message.
    /// </summary>
  public void RecordDebug(string message)
    {
        AddEntry($"DEBUG: {message}", HistoryEntryType.Debug);
    }

    private void AddEntry(string message, HistoryEntryType type)
    {
        _entries.Add(new HistoryEntry
        {
            Turn = _currentTurn,
         Timestamp = DateTime.UtcNow,
            Message = message,
      Type = type
      });
    }

    /// <summary>
 /// Gets all history entries.
    /// </summary>
    public IReadOnlyList<HistoryEntry> GetAllEntries()
    {
        return _entries.AsReadOnly();
    }

    /// <summary>
    /// Gets entries for a specific turn.
    /// </summary>
    public IEnumerable<HistoryEntry> GetEntriesForTurn(int turn)
    {
  return _entries.Where(e => e.Turn == turn);
    }

    /// <summary>
    /// Gets entries of a specific type.
    /// </summary>
    public IEnumerable<HistoryEntry> GetEntriesByType(HistoryEntryType type)
    {
 return _entries.Where(e => e.Type == type);
    }

    /// <summary>
    /// Formats the entire history as a readable string for debugging.
    /// </summary>
    public string FormatForDebug()
    {
        var sb = new StringBuilder();
     sb.AppendLine("=== BATTLE HISTORY ===");
    sb.AppendLine($"Total Entries: {_entries.Count}");
        sb.AppendLine($"Turns: {_currentTurn}");
        sb.AppendLine();

        foreach (var entry in _entries)
        {
       sb.AppendLine($"[T{entry.Turn}] {entry.Timestamp:HH:mm:ss.fff} - {entry.Message}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Formats history for a specific turn.
    /// </summary>
    public string FormatTurn(int turn)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"=== TURN {turn} ===");

        var turnEntries = GetEntriesForTurn(turn).ToList();
        if (turnEntries.Count == 0)
        {
      sb.AppendLine("No entries for this turn.");
       return sb.ToString();
        }

     foreach (var entry in turnEntries)
   {
  sb.AppendLine($"{entry.Timestamp:HH:mm:ss.fff} - {entry.Message}");
    }

   return sb.ToString();
    }

    /// <summary>
    /// Formats a summary of key events.
    /// </summary>
    public string FormatSummary()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== BATTLE SUMMARY ===");
        sb.AppendLine($"Total Turns: {_currentTurn}");
        sb.AppendLine($"Total Moves: {_entries.Count(e => e.Type == HistoryEntryType.Move)}");
        sb.AppendLine($"Total Damage Events: {_entries.Count(e => e.Type == HistoryEntryType.Damage)}");
        sb.AppendLine($"Total Faints: {_entries.Count(e => e.Type == HistoryEntryType.Faint)}");
  sb.AppendLine($"Total Switches: {_entries.Count(e => e.Type == HistoryEntryType.Switch)}");
      sb.AppendLine($"Errors: {_entries.Count(e => e.Type == HistoryEntryType.Error)}");

        var errors = _entries.Where(e => e.Type == HistoryEntryType.Error).ToList();
        if (errors.Any())
        {
    sb.AppendLine();
            sb.AppendLine("ERRORS:");
      foreach (var error in errors)
    {
  sb.AppendLine($"  Turn {error.Turn}: {error.Message}");
            }
    }

        return sb.ToString();
    }

  /// <summary>
    /// Exports history to a file for sharing with Copilot or debugging.
    /// </summary>
    public async Task ExportToFileAsync(string filePath)
    {
        var content = FormatForDebug();
        await File.WriteAllTextAsync(filePath, content);
    }

    /// <summary>
    /// Gets the total number of entries.
    /// </summary>
    public int Count => _entries.Count;
}

/// <summary>
/// A single entry in the battle history.
/// </summary>
public class HistoryEntry
{
    public int Turn { get; init; }
 public DateTime Timestamp { get; init; }
    public string Message { get; init; } = string.Empty;
    public HistoryEntryType Type { get; init; }
}

/// <summary>
/// Types of history entries for filtering and analysis.
/// </summary>
public enum HistoryEntryType
{
    TurnStart,
    Choice,
    Move,
    Damage,
    Heal,
    Faint,
    Switch,
  Status,
 StatChange,
    Ability,
    Item,
    Field,
    BattleEnd,
    Error,
    Debug
}
