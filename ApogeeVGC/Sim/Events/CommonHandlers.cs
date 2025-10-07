using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

public delegate double? ModifierEffectHandler(IBattle battle, int relayVar, Pokemon target,
    Pokemon source, IEffect effect);

public delegate double? ModifierMoveHandler(IBattle battle, int relayVar, Pokemon target,
    Pokemon source, ActiveMove move);

public delegate bool? ResultMoveHandler(IBattle battle, Pokemon target, Pokemon source, ActiveMove move);
public delegate IntBoolUnion? ExtResultMoveHandler(IBattle battle, Pokemon target, Pokemon source, ActiveMove move);
public delegate void VoidEffectHandler(IBattle battle, Pokemon target, Pokemon source, IEffect effect);
public delegate void VoidMoveHandler(IBattle battle, Pokemon target, Pokemon source, ActiveMove move);

// Source-based variants (source comes first in parameters)
public delegate double? ModifierSourceEffectHandler(IBattle battle, int relayVar, Pokemon source,
    Pokemon target, IEffect effect);

public delegate double? ModifierSourceMoveHandler(IBattle battle, int relayVar, Pokemon source,
    Pokemon target, ActiveMove move);

public delegate bool? ResultSourceMoveHandler(IBattle battle, Pokemon source, Pokemon target, ActiveMove move);
public delegate IntBoolUnion? ExtResultSourceMoveHandler(IBattle battle, Pokemon source,
    Pokemon target, ActiveMove move);

public delegate void VoidSourceEffectHandler(IBattle battle, Pokemon source, Pokemon target, IEffect effect);

// Note: The name "VoidSourceMoveHandler" might be a bit misleading since it returns a bool?.
// This was changed due to Paralysis' OnBeforeMove event needing to return a bool value.
public delegate bool? VoidSourceMoveHandler(IBattle battle, Pokemon source, Pokemon target, ActiveMove move);