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
            switch (value)
            {
                case IntIntFalseUnion iifu:
                    int i = iifu.Value;
                    if (i is 3 or 6 or 103)
                    {
                        field = i;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(value),
                            "Order must be one of the predefined values.");
                    }
                    break;
                case FalseIntFalseUnion:
                    throw new ArgumentException("Order cannot be of type FalseIntFalseUnion.", nameof(value));
                default:
                    throw new ArgumentException("Order must be of type FalseIntFalseUnion or IntIntFalseUnion.",
                        nameof(value));
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