﻿using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events;

public class Event
{
    public required EventId EventId { get; init; }
    public double Modifier { get; set; } = 1.0;
    public Pokemon? Target { get; set; }
}