namespace ApogeeVGC.Sim.Moves;

/// <summary>
/// Describes the acceptable target(s) of a move.
/// </summary>
public enum MoveTarget
{
    /// <summary>
    /// Only relevant to Doubles or Triples, the move only targets an ally of the user.
    /// </summary>
    AdjacentAlly,

    /// <summary>
    /// The move can target the user or its ally.
    /// </summary>
    AdjacentAllyOrSelf,

    /// <summary>
    /// The move can target a foe, but not (in Triples) a distant foe.
    /// </summary>
    AdjacentFoe,

    /// <summary>
    /// The move targets the field or all Pokémon at once.
    /// </summary>
    All,

    /// <summary>
    /// The move is a spread move that also hits the user's ally.
    /// </summary>
    AllAdjacent,

    /// <summary>
    /// The move is a spread move.
    /// </summary>
    AllAdjacentFoes,

    /// <summary>
    /// The move affects all active Pokémon on the user's side.
    /// </summary>
    Allies,

    /// <summary>
    /// The move adds a side condition on the user's side.
    /// </summary>
    AllySide,

    /// <summary>
    /// The move affects all unfainted Pokémon on the user's side.
    /// </summary>
    AllyTeam,

    /// <summary>
    /// The move can hit any other active Pokémon, not just those adjacent.
    /// </summary>
    Any,

    /// <summary>
    /// The move adds a side condition on the foe's side.
    /// </summary>
    FoeSide,

    /// <summary>
    /// The move can hit one adjacent Pokémon of your choice.
    /// </summary>
    Normal,

    /// <summary>
    /// The move targets an adjacent foe at random.
    /// </summary>
    RandomNormal,

    /// <summary>
    /// The move targets the foe that damaged the user.
    /// </summary>
    Scripted,

    /// <summary>
    /// The move affects the user of the move.
    /// </summary>
    Self,

    /// <summary>
    /// Added for DataMove.SelfSwitch - indicates no specific target.
    /// </summary>
    None,

    /// <summary>
    /// Added for Trick Room - the move targets the entire battlefield.
    /// </summary>
    Field,
}