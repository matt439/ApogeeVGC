using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesPqr()
    {
        return new Dictionary<AbilityId, Ability>
        {
            [AbilityId.Prankster] = new()
            {
                Id = AbilityId.Prankster,
                Name = "Prankster",
                Num = 158,
                Rating = 4.0,
                OnModifyPriority = new OnModifyPriorityEventInfo((_, priority, _, _, move) =>
                {
                    if (move.Category != MoveCategory.Status) return new VoidReturn();

                    move.PranksterBooster = true;
                    return priority + 1;
                }),
            },
            [AbilityId.QuarkDrive] = new()
            {
                Id = AbilityId.QuarkDrive,
                Name = "Quark Drive",
                Num = 282,
                Rating = 3.0,
                Condition = ConditionId.QuarkDrive,
                //OnSwitchInPriority = -2,
                OnSwitchIn = new OnSwitchInEventInfo((_, _) => { }, -2),

                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    battle.SingleEvent(EventId.TerrainChange, battle.Effect, battle.EffectState,
                        pokemon);
                }),
                OnTerrainChange = new OnTerrainChangeEventInfo((battle, pokemon, _, _) =>
                {
                    Condition quarkDrive = _library.Conditions[ConditionId.QuarkDrive];

                    if (battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                    {
                        pokemon.AddVolatile(quarkDrive.Id);
                    }
                    else if (!(pokemon.GetVolatile(ConditionId.QuarkDrive)?.FromBooster ?? false))
                    {
                        pokemon.RemoveVolatile(quarkDrive);
                    }
                }),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (pokemon is not PokemonSideFieldPokemon pok)
                    {
                        throw new ArgumentException("Expecting a Pokemon here.");
                    }

                    pok.Pokemon.DeleteVolatile(ConditionId.QuarkDrive);

                    if (battle.DisplayUi)
                    {
                        //UiGenerator.PrintEndEvent(pok.Pokemon, _library.Abilities[pok.Pokemon.Ability]);

                        battle.Add("-end", pok.Pokemon, "Quark Drive", "[silent]");
                    }
                }),
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    NoTransform = true,
                },
            },
        };
    }
}
