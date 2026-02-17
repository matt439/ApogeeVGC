using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

public delegate DoubleVoidUnion ModifierEffectHandler(Battle battle, int relayVar, Pokemon target,
    Pokemon source, IEffect effect);

public delegate DoubleVoidUnion ModifierMoveHandler(Battle battle, int relayVar, Pokemon target,
    Pokemon source, ActiveMove move);

public delegate BoolEmptyVoidUnion? ResultMoveHandler(Battle battle, Pokemon target, Pokemon source,
    ActiveMove move);

public delegate BoolIntEmptyVoidUnion? ExtResultMoveHandler(Battle battle, Pokemon target, Pokemon source,
    ActiveMove move);

public delegate void VoidEffectHandler(Battle battle, Pokemon target, Pokemon source, IEffect effect);
public delegate void VoidMoveHandler(Battle battle, Pokemon target, Pokemon source, ActiveMove move);

// Source-based variants (source comes first in parameters)
public delegate DoubleVoidUnion ModifierSourceEffectHandler(Battle battle, int relayVar, Pokemon source,
    Pokemon target, IEffect effect);

public delegate DoubleVoidUnion ModifierSourceMoveHandler(Battle battle, int relayVar, Pokemon source,
    Pokemon target, ActiveMove move);

public delegate BoolEmptyVoidUnion? ResultSourceMoveHandler(Battle battle, Pokemon source, Pokemon target,
    ActiveMove move);

public delegate BoolIntEmptyVoidUnion? ExtResultSourceMoveHandler(Battle battle, Pokemon source,
    Pokemon target, ActiveMove move);

public delegate void VoidSourceEffectHandler(Battle battle, Pokemon source, Pokemon target, IEffect effect);

// Note: The name "VoidSourceMoveHandler" might be a bit misleading since it returns a BoolVoidUnion.
// This was changed due to Paralysis' OnBeforeMove event needing to return a bool value.
public delegate BoolVoidUnion VoidSourceMoveHandler(Battle battle, Pokemon source, Pokemon target, ActiveMove move);

// Handler for FractionalPriority event - modifies move priority (no target parameter)
public delegate DoubleVoidUnion FractionalPriorityHandler(Battle battle, int priority, Pokemon pokemon, ActiveMove move);