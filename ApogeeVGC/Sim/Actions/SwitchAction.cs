using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

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

    public ActionOrder Order
    {
        get;
        init
        {
            if (value is ActionOrder.S3 or ActionOrder.S6 or ActionOrder.S103)
            {
                field = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(Order), "Invalid ActionOrder for SwitchAction.");
            }
        }
    }
    public int Priority { get; init; }
    public int Speed { get; init; }
    public required Pokemon Pokemon { get; init; }
    public required Pokemon Target { get; init; }
    public IEffect? SourceEffect { get; init; }
}