using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Actions;

public record PokemonAction : IAction
{
    public ActionId Choice
    {
        get;
        init
        {
            if (value is ActionId.Shift or ActionId.RunSwitch or ActionId.Event or ActionId.Terastallize)
            {
                field = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(Choice), "Invalid ActionId for PokemonAction.");
            }
        }
    }

    public int Priority { get; init; }
    public int Speed { get; set; }
    
    public required Pokemon Pokemon { get; init; }
    public Pokemon? Dragger { get; init; }
    public EventId? Event { get; init; }

    public int SubOrder => 0;
    public int EffectOrder => 0;
    public IntFalseUnion Order => int.MaxValue;
}