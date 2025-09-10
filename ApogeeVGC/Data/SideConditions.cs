using ApogeeVGC.Sim;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Ui;
using System.Collections.ObjectModel;
using ApogeeVGC.Sim.PokemonClasses;

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
                    Pokemon[] pokemon = [side.Slot1];
                    
                    // For each pokemon on the side, add the tailwind condition
                    foreach (Pokemon p in pokemon)
                    {
                        p.AddCondition(context.Library.Conditions[ConditionId.Tailwind], context);
                    }

                    if (context.PrintDebug)
                    {
                        UiGenerator.PrintTailwindStart(side.Team.Trainer.Name);
                    }
                },
                OnSideEnd = (side, context) =>
                {
                    Pokemon[] pokemon = [side.Slot1];

                    // For each pokemon on the side, remove the tailwind condition
                    foreach (Pokemon p in pokemon)
                    {
                        if (!p.RemoveCondition(ConditionId.Tailwind))
                        {
                            throw new InvalidOperationException($"Failed to remove tailwind condition from" +
                                                                $"{p.Specie.Name}");
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
                    pokemon.AddCondition(context.Library.Conditions[ConditionId.Tailwind], context);
                },
            },
            [SideConditionId.Reflect] = new()
            {
                Id = SideConditionId.Reflect,
                Name = "Reflect",
                BaseDuration = 5,
                DurationExtension = 3,
                DurationCallback = (_, source, _) => source.HasItem(ItemId.LightClay),
                OnSideStart = (side, context) =>
                {
                    Pokemon[] pokemon = [side.Slot1];

                    // For each pokemon on the side, add the reflect condition
                    foreach (Pokemon p in pokemon)
                    {
                        p.AddCondition(context.Library.Conditions[ConditionId.Reflect], context);
                    }

                    if (context.PrintDebug)
                    {
                        UiGenerator.PrintReflectStart(side.Team.Trainer.Name);
                    }
                },
                OnSideResidualOrder = 26,
                OnSideResidualSubOrder = 1,
                OnSideEnd = (side, context) =>
                 {
                     Pokemon[] pokemon = [side.Slot1];

                     // For each pokemon on the side, remove the reflect condition
                     foreach (Pokemon p in pokemon)
                     {
                         if (!p.RemoveCondition(ConditionId.Reflect))
                         {
                             throw new InvalidOperationException($"Failed to remove the reflect condition from" +
                                                                 $"{p.Specie.Name}");
                         }
                     }
                     if (context.PrintDebug)
                     {
                         UiGenerator.PrintReflectEnd(side.Team.Trainer.Name);
                     }
                 },
                OnPokemonSwitchIn = (pokemon, context) =>
                {
                    pokemon.AddCondition(context.Library.Conditions[ConditionId.Reflect], context);
                },
            },
            [SideConditionId.LightScreen] = new()
            {
                Id = SideConditionId.LightScreen,
                Name = "Light Screen",
                BaseDuration = 5,
                DurationExtension = 3,
                DurationCallback = (_, source, _) => source.HasItem(ItemId.LightClay),
                OnSideStart = (side, context) =>
                {
                    Pokemon[] pokemon = [side.Slot1];

                    // For each pokemon on the side, add the light screen condition
                    foreach (Pokemon p in pokemon)
                    {
                        p.AddCondition(context.Library.Conditions[ConditionId.LightScreen], context);
                    }

                    if (context.PrintDebug)
                    {
                        UiGenerator.PrintLightScreenStart(side.Team.Trainer.Name);
                    }
                },
                OnSideResidualOrder = 26,
                OnSideResidualSubOrder = 2,
                OnSideEnd = (side, context) =>
                {
                    Pokemon[] pokemon = [side.Slot1];

                    // For each pokemon on the side, remove the light screen condition
                    foreach (Pokemon p in pokemon)
                    {
                        if (!p.RemoveCondition(ConditionId.LightScreen))
                        {
                            throw new InvalidOperationException("Failed to remove the light screen condition" +
                                                                $"from {p.Specie.Name}");
                        }
                    }
                    if (context.PrintDebug)
                    {
                        UiGenerator.PrintLightScreenEnd(side.Team.Trainer.Name);
                    }
                },
                OnPokemonSwitchIn = (pokemon, context) =>
                {
                    pokemon.AddCondition(context.Library.Conditions[ConditionId.LightScreen], context);
                },
            },
        };
    }
}