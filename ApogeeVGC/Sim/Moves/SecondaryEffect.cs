using ApogeeVGC.Sim.Abilities;

namespace ApogeeVGC.Sim.Moves;

public record SecondaryEffect : HitEffect
{
    public int? Chance { get; set; }
    public Ability? Ability { get; init; }
    // KingsRock is only for Gen 2
    public HitEffect? Self { get; init; }
}