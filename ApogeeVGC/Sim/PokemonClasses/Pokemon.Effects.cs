using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    private IEffect[] GetAllEffects()
    {
        List<IEffect> effects =
        [
            Specie,
            Ability,
        ];
        if (Item is not null)
        {
            effects.Add(Item);
        }
        effects.AddRange(Moves);
        effects.AddRange(Conditions);
        return effects.ToArray();
    }
}