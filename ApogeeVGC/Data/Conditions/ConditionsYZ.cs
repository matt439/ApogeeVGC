using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsYz()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.ZenMode] = new()
            {
                Id = ConditionId.ZenMode,
                Name = "Zen Mode",
                EffectType = EffectType.Condition,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (target is not Pokemon pokemon) return new VoidReturn();

                    // Check if it's a Galar form
                    bool isGalar = pokemon.Species.Forme == FormeId.Galar ||
                                   pokemon.Species.Forme == FormeId.GalarZen;

                    if (!isGalar)
                    {
                        if (pokemon.Species.Id != SpecieId.DarmanitanZen)
                        {
                            pokemon.FormeChange(SpecieId.DarmanitanZen);
                        }
                    }
                    else
                    {
                        if (pokemon.Species.Id != SpecieId.DarmanitanGalarZen)
                        {
                            pokemon.FormeChange(SpecieId.DarmanitanGalarZen);
                        }
                    }

                    return new VoidReturn();
                }),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    // Check if in Zen forme
                    if (pokemon.Species.Forme is FormeId.Zen or FormeId.GalarZen)
                    {
                        // Revert to base forme - Darmanitan or Darmanitan-Galar
                        SpecieId baseForme = pokemon.Species.Forme == FormeId.GalarZen
                            ? SpecieId.DarmanitanGalar
                            : SpecieId.Darmanitan;
                        pokemon.FormeChange(baseForme);
                    }
                }),
            },
        };
    }
}
