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
        
        int oldHp = pokemon.Hp;
        pokemon.Hp = message.RemainingHp;
        
        PokemonDamaged?.Invoke(pokemon, oldHp, pokemon.Hp);
    }
    
    private void HandleFaint(FaintMessage message)
    {
        var pokemon = GetPokemon(message.PokemonName, message.SideId);
        if (pokemon == null) return;
        
        pokemon.IsFainted = true;
        pokemon.Hp = 0;
        
        PokemonFainted?.Invoke(pokemon);
    }
    
    private void HandleSwitch(SwitchMessage message)
    {
        // Determine which side and slot
        var (side, slot, pokemon) = FindPokemonForSwitch(message.PokemonName, message.TrainerName);
        if (pokemon == null || slot < 0) return;
        
        // Get current active Pokemon in that slot (if any)
        var currentActive = side == SideId.P1 
            ? _playerActive.GetValueOrDefault(slot)
            : _opponentActive.GetValueOrDefault(slot);
        
        if (currentActive != null)
        {
            currentActive.IsActive = false;
            PokemonSwitchedOut?.Invoke(currentActive, slot);
        }
        
        // Set new active Pokemon
        pokemon.IsActive = true;
        pokemon.Position = slot;
        
        if (side == SideId.P1)
        {
            _playerActive[slot] = pokemon;
        }
        else
        {
            _opponentActive[slot] = pokemon;
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
        // Try player side first
        var pokemon = _playerTeam.FirstOrDefault(p => p.Name == pokemonName);
        if (pokemon != null)
        {
            // Check if this Pokemon is already active - ignore initial switch-in messages
            if (pokemon.IsActive)
            {
                return (SideId.P1, -1, null);
            }
            
            // Find empty slot or slot with fainted Pokemon
            var emptySlot = _playerActive.FirstOrDefault(kvp => 
                kvp.Value.IsFainted || !kvp.Value.IsActive);
            
            int slot = emptySlot.Value != null ? emptySlot.Key : 0;
            
            return (SideId.P1, slot, pokemon);
        }
        
        // Try opponent side
        pokemon = _opponentTeam.FirstOrDefault(p => p.Name == pokemonName);
        if (pokemon != null)
        {
            // Check if this Pokemon is already active - ignore initial switch-in messages
            if (pokemon.IsActive)
            {
                return (SideId.P2, -1, null);
            }
            
            var emptySlot = _opponentActive.FirstOrDefault(kvp => 
                kvp.Value.IsFainted || !kvp.Value.IsActive);
            
            int slot = emptySlot.Value != null ? emptySlot.Key : 0;
            
            return (SideId.P2, slot, pokemon);
        }
        
        return (SideId.P1, -1, null);
    }
    
    // Public accessors
    public IReadOnlyDictionary<int, PokemonState> PlayerActive => _playerActive;
    public IReadOnlyDictionary<int, PokemonState> OpponentActive => _opponentActive;
    public IReadOnlyList<PokemonState> PlayerTeam => _playerTeam;
    public IReadOnlyList<PokemonState> OpponentTeam => _opponentTeam;
}
