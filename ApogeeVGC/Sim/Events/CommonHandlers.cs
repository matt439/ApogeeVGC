using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

public delegate DoubleVoidUnion ModifierEffectHandler(IBattle battle, int relayVar, Pokemon target,
    Pokemon source, IEffect effect);

public delegate DoubleVoidUnion ModifierMoveHandler(IBattle battle, int relayVar, Pokemon target,
    Pokemon source, ActiveMove move);

public delegate BoolUndefinedVoidUnion? ResultMoveHandler(IBattle battle, Pokemon target, Pokemon source,
    ActiveMove move);

public delegate BoolIntUndefinedVoidUnion? ExtResultMoveHandler(IBattle battle, Pokemon target, Pokemon source,
    ActiveMove move);

public delegate void VoidEffectHandler(IBattle battle, Pokemon target, Pokemon source, IEffect effect);
public delegate void VoidMoveHandler(IBattle battle, Pokemon target, Pokemon source, ActiveMove move);

// Source-based variants (source comes first in parameters)
public delegate DoubleVoidUnion ModifierSourceEffectHandler(IBattle battle, int relayVar, Pokemon source,
    Pokemon target, IEffect effect);

public delegate DoubleVoidUnion ModifierSourceMoveHandler(IBattle battle, int relayVar, Pokemon source,
    Pokemon target, ActiveMove move);

public delegate BoolUndefinedVoidUnion? ResultSourceMoveHandler(IBattle battle, Pokemon source, Pokemon target,
    ActiveMove move);

public delegate BoolIntUndefinedVoidUnion? ExtResultSourceMoveHandler(IBattle battle, Pokemon source,
    Pokemon target, ActiveMove move);

public delegate void VoidSourceEffectHandler(IBattle battle, Pokemon source, Pokemon target, IEffect effect);

// Note: The name "VoidSourceMoveHandler" might be a bit misleading since it returns a bool?.
// This was changed due to Paralysis' OnBeforeMove event needing to return a bool value.
public delegate BoolVoidUnion VoidSourceMoveHandler(IBattle battle, Pokemon source, Pokemon target, ActiveMove move);