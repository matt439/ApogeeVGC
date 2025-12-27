using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesJkl()
    {
        return new Dictionary<MoveId, Move>
        {
            [MoveId.JawLock] = new()
            {
                Id = MoveId.JawLock,
                Num = 746,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Jaw Lock",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Bite = true,
                },
                // TODO: onHit - add 'trapped' volatile to both source and target
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.JetPunch] = new()
            {
                Id = MoveId.JetPunch,
                Num = 857,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Jet Punch",
                BasePp = 15,
                Priority = 1,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Punch = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.Judgment] = new()
            {
                Id = MoveId.Judgment,
                Num = 449,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Judgment",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                // TODO: onModifyType - change type based on plate held (item.onPlate)
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.JungleHealing] = new()
            {
                Id = MoveId.JungleHealing,
                Num = 816,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Jungle Healing",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Heal = true,
                    BypassSub = true,
                    AllyAnim = true,
                },
                // TODO: onHit - heal 25% of target's max HP and cure status
                Secondary = null,
                Target = MoveTarget.Allies,
                Type = MoveType.Grass,
            },
            [MoveId.LeechSeed] = new()
            {
                Id = MoveId.LeechSeed,
                Num = 73,
                Accuracy = 90,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Leech Seed",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.LeechSeed,
                Condition = _library.Conditions[ConditionId.LeechSeed],
                OnTryImmunity = new OnTryImmunityEventInfo((_, target, _, _) =>
                    !target.HasType(PokemonType.Grass)),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.LightScreen] = new()
            {
                Id = MoveId.LightScreen,
                Num = 113,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Light Screen",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                SideCondition = ConditionId.LightScreen,
                Condition = _library.Conditions[ConditionId.LightScreen],
                Secondary = null,
                Target = MoveTarget.AllySide,
                Type = MoveType.Psychic,
            },
            [MoveId.LowKick] = new()
            {
                Id = MoveId.LowKick,
                Num = 67,
                Accuracy = 100,
                BasePower = 0,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, _, target, _) =>
                {
                    int targetWeight = target.GetWeight();
                    int bp = targetWeight switch
                    {
                        >= 2000 => 120,
                        >= 1000 => 100,
                        >= 500 => 80,
                        >= 250 => 60,
                        >= 100 => 40,
                        _ => 20,
                    };
                    if (battle.DisplayUi)
                    {
                        battle.Debug($"BP: {bp}");
                    }

                    return bp;
                }),
                Category = MoveCategory.Physical,
                Name = "Low Kick",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                // OnTryHit only applies to dynamax
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
        };
    }
}
