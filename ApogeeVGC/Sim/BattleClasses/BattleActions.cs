using ApogeeVGC.Data;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.BattleClasses;

public class BattleActions(IBattle battle)
{
    public IBattle Battle { get; init; } = battle;
    public Library Library => Battle.Library;

    public MoveTypeFalseUnion CanTerastallize(IBattle battle, Pokemon pokemon)
    {
        throw new NotImplementedException();
    }
}