using ApogeeVGC.Sim.Actions;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// MoveAction | SwitchAction | TeamAction | PokemonAction
/// </summary>
public abstract record MoveSwitchTeamPokemonActionUnion
{
    public static implicit operator MoveSwitchTeamPokemonActionUnion(MoveAction moveAction) =>
        new MoveActionMoveSwitchTeamPokemonActionUnion(moveAction);
    public static implicit operator MoveSwitchTeamPokemonActionUnion(SwitchAction switchAction) =>
  new SwitchActionMoveSwitchTeamPokemonActionUnion(switchAction);
    public static implicit operator MoveSwitchTeamPokemonActionUnion(TeamAction teamAction) =>
        new TeamActionMoveSwitchTeamPokemonActionUnion(teamAction);
    public static implicit operator MoveSwitchTeamPokemonActionUnion(PokemonAction pokemonAction) =>
        new PokemonActionMoveSwitchTeamPokemonActionUnion(pokemonAction);
}

public record MoveActionMoveSwitchTeamPokemonActionUnion(MoveAction MoveAction) :
    MoveSwitchTeamPokemonActionUnion;
public record SwitchActionMoveSwitchTeamPokemonActionUnion(SwitchAction SwitchAction) :
    MoveSwitchTeamPokemonActionUnion;
public record TeamActionMoveSwitchTeamPokemonActionUnion(TeamAction TeamAction) :
    MoveSwitchTeamPokemonActionUnion;
public record PokemonActionMoveSwitchTeamPokemonActionUnion(PokemonAction PokemonAction) :
    MoveSwitchTeamPokemonActionUnion;
