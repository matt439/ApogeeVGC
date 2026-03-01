using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Actions;

public record SwitchAction : IAction
{
    public ActionId Choice
    {
        get;
        init
        {
            if (value is ActionId.Switch or ActionId.InstaSwitch or ActionId.RevivalBlessing)
            {
                field = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(Choice), "Invalid ActionId for SwitchAction.");
            }
        }
    }
    public required IntFalseUnion Order
    {
        get;
        init
        {
            if (value.IsFalse)
                throw new ArgumentException("Order cannot be false.", nameof(value));

            int i = value.Value;
            if (i is 3 or 6 or 103)
            {
                field = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    "Order must be one of the predefined values.");
            }
        }
    }
    public int Priority { get; init; }
    public int Speed { get; set; }
    public required Pokemon Pokemon { get; init; }
    public required Pokemon Target { get; init; }
    public IEffect? SourceEffect { get; init; }

    public int SubOrder => 0;
    public int EffectOrder => 0;
}