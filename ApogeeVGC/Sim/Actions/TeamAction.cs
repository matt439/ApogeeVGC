using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Actions;

public record TeamAction : IAction
{
    public ActionId Choice
    {
        get;
        init
        {
            if (value is ActionId.Team)
            {
                field = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(Choice), "Invalid ActionId for TeamAction.");
            }
        }
    }

    public int Priority { get; init; }
    public int Speed => 1;
    public required Pokemon Pokemon { get; init; }
    public int Index { get; init; }


    public IntFalseUnion Order => int.MaxValue;
    public int SubOrder => 0;
    public int EffectOrder => 0;
}