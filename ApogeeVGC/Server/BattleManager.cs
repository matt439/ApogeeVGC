using System.Collections.Concurrent;
using ApogeeVGC.Data;

namespace ApogeeVGC.Server;

/// <summary>
/// Manages active battles and assigns unique battle IDs.
/// </summary>
public class BattleManager
{
    private readonly ConcurrentDictionary<string, BattleRoom> _activeBattles = new();
    private readonly Library _library;
    private int _battleCounter;

    public BattleManager(Library library)
    {
        _library = library;
    }

    /// <summary>
    /// Creates a new battle and returns the battle room.
  /// </summary>
    public BattleRoom CreateBattle(string player1Username, string format = "gen9doublescustomgame")
    {
   string battleId = $"battle-gen9doublescustomgame-{Interlocked.Increment(ref _battleCounter)}";
        
        var battleRoom = new BattleRoom(battleId, player1Username, format, _library);
        
 if (_activeBattles.TryAdd(battleId, battleRoom))
     {
            Console.WriteLine($"Created battle: {battleId} for player {player1Username}");
       return battleRoom;
        }
        
        throw new InvalidOperationException($"Failed to create battle with ID {battleId}");
    }

    /// <summary>
/// Gets a battle by its ID.
    /// </summary>
 public BattleRoom? GetBattle(string battleId)
    {
   _activeBattles.TryGetValue(battleId, out var battle);
        return battle;
    }

    /// <summary>
    /// Removes a battle from the active battles list.
 /// </summary>
    public void RemoveBattle(string battleId)
  {
        if (_activeBattles.TryRemove(battleId, out var battle))
        {
Console.WriteLine($"Removed battle: {battleId}");
    battle.Dispose();
        }
    }

    /// <summary>
    /// Gets the number of active battles.
    /// </summary>
    public int ActiveBattleCount => _activeBattles.Count;
}
