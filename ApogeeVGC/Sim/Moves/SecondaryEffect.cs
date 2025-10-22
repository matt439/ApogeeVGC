using ApogeeVGC.Sim.GameObjects;

namespace ApogeeVGC.Sim.Moves;

public record SecondaryEffect : HitEffect
{
    public int? Chance { get; init; }
    public Ability? Ability { get; init; }
    // KingsRock is only for Gen 2
    public HitEffect? Self { get; init; }
}