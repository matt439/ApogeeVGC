using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Gui.State;

/// <summary>
/// Maintains the GUI's view of the battle state
/// Updated incrementally based on messages from the battle engine
/// </summary>
public class GuiBattleState
{
    // Active Pokemon by slot
    private readonly Dictionary<int, PokemonState> _playerActive = new();
    private readonly Dictionary<int, PokemonState> _opponentActive = new();
    
    // Team rosters (for switch messages)
    private readonly List<PokemonState> _playerTeam = [];
    private readonly List<PokemonState> _opponentTeam = [];
    
    // Events for UI updates
    public event Action<PokemonState, int, int>? PokemonDamaged; // pokemon, oldHp, newHp
    public event Action<PokemonState>? PokemonFainted;
    public event Action<PokemonState, int>? PokemonSwitchedIn; // pokemon, slot
    public event Action<PokemonState, int>? PokemonSwitchedOut; // pokemon, slot
    public event Action<int>? TurnStarted;
    
    /// <summary>
    /// Initialize state from initial perspective
    /// </summary>
    public void Initialize(BattlePerspective perspective)
    {
        _playerActive.Clear();
        _opponentActive.Clear();
        _playerTeam.Clear();
        _opponentTeam.Clear();
        
        // Initialize player team
        for (int i = 0; i < perspective.PlayerSide.Pokemon.Count; i++)
        {
            var pokemon = PokemonState.FromPerspective(
                perspective.PlayerSide.Pokemon[i], 
                i, 
                SideId.P1);
            _playerTeam.Add(pokemon);
        }
        
        // Initialize opponent team
        for (int i = 0; i < perspective.OpponentSide.Pokemon.Count; i++)
        {
            var pokemon = PokemonState.FromPerspective(
                perspective.OpponentSide.Pokemon[i], 
                i, 
                SideId.P2);
            _opponentTeam.Add(pokemon);
        }
        
        // Set active Pokemon
        // Important: perspective.PlayerSide.Active is a list where:
        // - The INDEX represents the battle field position (0 or 1 for doubles)
        // - The active.Position represents the team slot (0-5)
        for (int battlePosition = 0; battlePosition < perspective.PlayerSide.Active.Count; battlePosition++)
        {
            var active = perspective.PlayerSide.Active[battlePosition];
            if (active != null)
            {
                var teamSlot = active.Position;
                if (teamSlot >= 0 && teamSlot < _playerTeam.Count)
                {
                    var pokemon = _playerTeam[teamSlot];
                    pokemon.IsActive = true;
                    pokemon.Position = battlePosition;
                    _playerActive[battlePosition] = pokemon;
                }
            }
        }
        
        for (int battlePosition = 0; battlePosition < perspective.OpponentSide.Active.Count; battlePosition++)
        {
            var active = perspective.OpponentSide.Active[battlePosition];
            if (active != null)
            {
                var teamSlot = active.Position;
                if (teamSlot >= 0 && teamSlot < _opponentTeam.Count)
                {
                    var pokemon = _opponentTeam[teamSlot];
                    pokemon.IsActive = true;
                    pokemon.Position = battlePosition;
                    _opponentActive[battlePosition] = pokemon;
                }
            }
        }
    }
    
    /// <summary>
    /// Process a battle message and update state
    /// </summary>
    public void ProcessMessage(BattleMessage message)
    {
        switch (message)
        {
            case TurnStartMessage turnStart:
                HandleTurnStart(turnStart);
                break;
                
            case DamageMessage damage:
                HandleDamage(damage);
                break;
                
            case FaintMessage faint:
                HandleFaint(faint);
                break;
                
            case SwitchMessage switchMsg:
                HandleSwitch(switchMsg);
                break;
                
            case HealMessage heal:
                HandleHeal(heal);
                break;
                
            case StatusMessage status:
                HandleStatus(status);
                break;
                
            // Add other message types as needed
        }
    }
    
    private void HandleTurnStart(TurnStartMessage message)
    {
        TurnStarted?.Invoke(message.TurnNumber);
    }
    
    private void HandleDamage(DamageMessage message)
    {
        var pokemon = GetPokemon(message.PokemonName, message.SideId);
        if (pokemon == null) return;
        
        // Use the current state HP as the "old HP" because damage messages
        // from the battle engine may have stale HP values when multiple attacks
        // hit the same Pokemon in quick succession
        int oldHp = pokemon.Hp;
        
        // Calculate the new HP by subtracting the damage amount from current HP
        // Don't trust message.RemainingHp as it may be based on stale initial HP
        int newHp = Math.Max(0, oldHp - message.DamageAmount);
        pokemon.Hp = newHp;
        
        Console.WriteLine($"[GuiBattleState.HandleDamage] {pokemon.Name}: {oldHp} ? {newHp} (damage: {message.DamageAmount}, message claimed: {message.RemainingHp})");
        
        PokemonDamaged?.Invoke(pokemon, oldHp, newHp);
    }
    
    private void HandleFaint(FaintMessage message)
    {
        var pokemon = GetPokemon(message.PokemonName, message.SideId);
        if (pokemon == null) return;
        
        Console.WriteLine($"[GuiBattleState.HandleFaint] {pokemon.Name} fainted (HP was: {pokemon.Hp}, IsActive was: {pokemon.IsActive})");
        
        pokemon.IsFainted = true;
        pokemon.IsActive = false; // Clear active flag when fainting
        // Don't set Hp to 0 here - it should already be 0 from HandleDamage
        
        PokemonFainted?.Invoke(pokemon);
    }
    
    private void HandleSwitch(SwitchMessage message)
    {
        Console.WriteLine($"[GuiBattleState.HandleSwitch] Processing switch for {message.PokemonName} from trainer {message.TrainerName}");
        
        // Determine which side and slot
        var (side, slot, pokemon) = FindPokemonForSwitch(message.PokemonName, message.TrainerName);
        
        Console.WriteLine($"[GuiBattleState.HandleSwitch] FindPokemonForSwitch returned: side={side}, slot={slot}, pokemon={(pokemon?.Name ?? "null")}");
        
        if (pokemon == null || slot < 0)
        {
            Console.WriteLine($"[GuiBattleState.HandleSwitch] Aborting switch - invalid pokemon or slot");
            return;
        }
        
        // Get current active Pokemon in that slot (if any)
        var currentActive = side == SideId.P1 
            ? _playerActive.GetValueOrDefault(slot)
            : _opponentActive.GetValueOrDefault(slot);
        
        if (currentActive != null)
        {
            Console.WriteLine($"[GuiBattleState.HandleSwitch] Switching out {currentActive.Name} from slot {slot}");
            currentActive.IsActive = false;
            PokemonSwitchedOut?.Invoke(currentActive, slot);
        }
        
        // Set new active Pokemon
        pokemon.IsActive = true;
        pokemon.Position = slot;
        
        if (side == SideId.P1)
        {
            _playerActive[slot] = pokemon;
            Console.WriteLine($"[GuiBattleState.HandleSwitch] Set player slot {slot} to {pokemon.Name}");
        }
        else
        {
            _opponentActive[slot] = pokemon;
            Console.WriteLine($"[GuiBattleState.HandleSwitch] Set opponent slot {slot} to {pokemon.Name}");
        }
        
        PokemonSwitchedIn?.Invoke(pokemon, slot);
    }
    
    private void HandleHeal(HealMessage message)
    {
        var pokemon = GetPokemon(message.PokemonName, message.SideId);
        if (pokemon == null) return;
        
        int oldHp = pokemon.Hp;
        pokemon.Hp = message.CurrentHp;
        
        // Could add a PokemonHealed event if needed
        // For now, treat it as a negative damage
        if (oldHp != pokemon.Hp)
        {
            PokemonDamaged?.Invoke(pokemon, oldHp, pokemon.Hp);
        }
    }
    
    private void HandleStatus(StatusMessage message)
    {
        var pokemon = GetPokemon(message.PokemonName, message.SideId);
        if (pokemon == null) return;
        
        // Update status based on status name
        // This is a simplified version - you may need to map status names to ConditionId
        // For now, we'll keep the existing status
        // TODO: Add proper status mapping if needed
    }
    
    private PokemonState? GetPokemon(string name, SideId? side)
    {
        if (!side.HasValue) return null;
        
        var team = side.Value == SideId.P1 ? _playerTeam : _opponentTeam;
        return team.FirstOrDefault(p => p.Name == name);
    }
    
    private (SideId side, int slot, PokemonState? pokemon) FindPokemonForSwitch(
        string pokemonName, 
        string trainerName)
    {
        Console.WriteLine($"[FindPokemonForSwitch] Looking for '{pokemonName}' from trainer '{trainerName}'");
        
        // Determine which side based on trainer name
        // In a GUI battle, the player is always "Matt" (or the local player name)
        // The opponent is the other trainer (e.g., "Random")
        bool isPlayerSwitch = trainerName != "Random"; // Simple heuristic - improve if needed
        
        // Search the correct team first based on trainer name
        if (isPlayerSwitch)
        {
            // Search player team first
            var pokemon = _playerTeam.FirstOrDefault(p => p.Name == pokemonName);
            if (pokemon != null && !pokemon.IsActive)
            {
                var emptySlot = _playerActive.FirstOrDefault(kvp => 
                    kvp.Value.IsFainted || !kvp.Value.IsActive);
                int slot = emptySlot.Value != null ? emptySlot.Key : 0;
                Console.WriteLine($"[FindPokemonForSwitch] Found {pokemonName} in player team, switching to slot {slot}");
                return (SideId.P1, slot, pokemon);
            }
            
            // If already active, ignore
            if (pokemon?.IsActive == true)
            {
                Console.WriteLine($"[FindPokemonForSwitch] Player {pokemonName} is already active, ignoring");
                return (SideId.P1, -1, null);
            }
        }
        else
        {
            // Search opponent team first
            var pokemon = _opponentTeam.FirstOrDefault(p => p.Name == pokemonName);
            if (pokemon != null && !pokemon.IsActive)
            {
                var emptySlot = _opponentActive.FirstOrDefault(kvp => 
                    kvp.Value.IsFainted || !kvp.Value.IsActive);
                int slot = emptySlot.Value != null ? emptySlot.Key : 0;
                Console.WriteLine($"[FindPokemonForSwitch] Found {pokemonName} in opponent team, switching to slot {slot}");
                return (SideId.P2, slot, pokemon);
            }
            
            // If already active, ignore
            if (pokemon?.IsActive == true)
            {
                Console.WriteLine($"[FindPokemonForSwitch] Opponent {pokemonName} is already active, ignoring");
                return (SideId.P2, -1, null);
            }
        }
        
        Console.WriteLine($"[FindPokemonForSwitch] Pokemon '{pokemonName}' from trainer '{trainerName}' NOT FOUND or already active!");
        return (SideId.P1, -1, null);
    }
    
    // Public accessors
    public IReadOnlyDictionary<int, PokemonState> PlayerActive => _playerActive;
    public IReadOnlyDictionary<int, PokemonState> OpponentActive => _opponentActive;
    public IReadOnlyList<PokemonState> PlayerTeam => _playerTeam;
    public IReadOnlyList<PokemonState> OpponentTeam => _opponentTeam;
}
