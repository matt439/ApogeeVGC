#pragma once

enum class MoveTarget
{
    // Only relevant to Doubles or Triples, the move only targets an ally of the user.
    ADJACENT_ALLY,

    // The move can target the user or its ally.
    ADJACENT_ALLY_OR_SELF,

    // The move can target a foe, but not (in Triples) a distant foe.
    ADJACENT_FOE,

    // The move targets the field or all Pok�mon at once.
    ALL,

    // A spread move that also hits the user's ally.
    ALL_ADJACENT,

    // A spread move that hits all adjacent foes.
    ALL_ADJACENT_FOES,

    // The move affects all active Pok�mon on the user's team.
    ALLIES,

    // The move adds a side condition on the user's side.
    ALLY_SIDE,

    // The move affects all unfainted Pok�mon on the user's team.
    ALLY_TEAM,

    // The move can hit any other active Pok�mon, not just those adjacent.
    ANY,

    // The move adds a side condition on the foe's side.
    FOE_SIDE,

    // The move can hit one adjacent Pok�mon of your choice.
    NORMAL,

    // The move targets an adjacent foe at random.
    RANDOM_NORMAL,

    // The move targets the foe that damaged the user.
    SCRIPTED,

    // The move affects the user of the move.
    SELF,
};
