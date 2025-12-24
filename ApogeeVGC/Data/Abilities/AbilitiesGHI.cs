using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesGhi()
    {
        return new Dictionary<AbilityId, Ability>
        {
            [AbilityId.Guts] = new()
            {
                Id = AbilityId.Guts,
                Name = "Guts",
                Num = 62,
                Rating = 3.5,
                //OnModifyAtkPriority = 5,
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, pokemon, _, _) =>
                    {
                        if (pokemon.Status is not ConditionId.None)
                        {
                            battle.ChainModify(1.5);
                            return battle.FinalModify(atk);
                        }

                        return atk;
                    },
                    5),
            },
            [AbilityId.HadronEngine] = new()
            {
                Id = AbilityId.HadronEngine,
                Name = "Hadron Engine",
                Num = 289,
                Rating = 4.5,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    battle.Debug($"[HadronEngine.OnStart] HANDLER EXECUTING for {pokemon.Name}");

                    bool terrainSet = battle.Field.SetTerrain(
                        _library.Conditions[ConditionId.ElectricTerrain]);

                    battle.Debug($"[HadronEngine.OnStart] SetTerrain returned: {terrainSet}");
                    battle.Debug($"[HadronEngine.OnStart] Current terrain: {battle.Field.Terrain}");

                    if (!terrainSet &&
                        battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                    {
                        battle.Debug(
                            $"[HadronEngine.OnStart] Terrain already Electric, showing activate");
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", pokemon, "ability: Hadron Engine");
                        }
                    }
                }),
                //OnModifySpAPriority = 5,
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, _, _, _) =>
                    {
                        if (!battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                            return spa;
                        battle.Debug("Hadron Engine boost");
                        battle.ChainModify([5461, 4096]);
                        return battle.FinalModify(spa);
                    },
                    5),
            },
        };
    }
}
