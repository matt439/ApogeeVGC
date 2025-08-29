using ApogeeVGC.Data;
using ApogeeVGC.Sim;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApogeeVGC.Data;


public class SideConditions
{
    public IReadOnlyDictionary<SideConditionId, SideCondition> SideConditionData { get; }
    private readonly Library _library;

    public SideConditions(Library library)
    {
        _library = library;
        SideConditionData = new ReadOnlyDictionary<SideConditionId, SideCondition>(CreateSideConditions());
    }

    private Dictionary<SideConditionId, SideCondition> CreateSideConditions()
    {
        return new Dictionary<SideConditionId, SideCondition>
        {
            [SideConditionId.Tailwind] = new()
            {
                Id = SideConditionId.Tailwind,
                Name = "Tailwind",
                BaseDuration = 4,
                DurationExtension = 0,
                OnSideResidualOrder = 26,
                OnSideResidualSubOrder = 5,
                OnSideStart = (side, context) =>
                {
                    var pokemon = side.Team.AllActivePokemon;
                    
                    // For each pokemon on the side, add the tailwind condition
                    foreach (Pokemon p in pokemon)
                    {
                        p.AddCondition(_library.Conditions[ConditionId.Tailwind].Copy(), context);
                    }

                    if (context.PrintDebug)
                    {
                        UiGenerator.PrintTailwindStart(side.Team.Trainer.Name);
                    }
                },
                OnSideEnd = (side, context) =>
                {
                    var pokemon = side.Team.AllActivePokemon;

                    // For each pokemon on the side, remove the tailwind condition
                    foreach (Pokemon p in pokemon)
                    {
                        if (!p.RemoveCondition(ConditionId.Tailwind))
                        {
                            throw new InvalidOperationException($"Failed to remove tailwind condition from {p.Specie.Name}");
                        }
                    }
                    if (context.PrintDebug)
                    {
                        UiGenerator.PrintTailwindEnd(side.Team.Trainer.Name);
                    }
                },
                //OnIncrementTurnCounter = (_, element, context) =>
                //{
                //    if (context.PrintDebug)
                //    {
                //        UiGenerator.PrintFieldElementCounter(element);
                //    }
                //},
                OnPokemonSwitchIn = (pokemon, context) =>
                {
                    pokemon.AddCondition(_library.Conditions[ConditionId.Tailwind].Copy(), context);
                },
            },
        };
    }
}