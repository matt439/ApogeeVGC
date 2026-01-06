using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Actions;

public record FieldAction : IAction
{
    public ActionId Choice
    {
        get;
        init
        {
            if (value is ActionId.Start or ActionId.Residual or ActionId.Pass or ActionId.BeforeTurn)
            {
                field = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(Choice), "Invalid ActionId for FieldAction.");
            }
        }
    }
    public int Priority { get; init; }
    public int Speed => 1;
    public Pokemon? Pokemon => null;

    // Properties to satisfy IPriorityComparison
    public IntFalseUnion Order => int.MaxValue;
    public int SubOrder => 0;
    public int EffectOrder => 0;
}