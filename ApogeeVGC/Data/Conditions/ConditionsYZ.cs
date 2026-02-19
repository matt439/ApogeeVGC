using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsYz()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.Yawn] = new()
            {
                Id = ConditionId.Yawn,
                Name = "Yawn",
                AssociatedMove = MoveId.Yawn,
                EffectType = EffectType.Condition,
                NoCopy = true, // doesn't get copied by Baton Pass
                Duration = 2,
                OnStart = OnStartEventInfo.Create((battle, target, source, _) =>
                {
                    // TS always adds the message (no DisplayUi check)
                    battle.Add("-start", target, "move: Yawn", $"[of] {source}");
                    return null;
                }),
                OnResidual = OnResidualEventInfo.Create((_, _, _, _) =>
                {
                    // Yawn resolves at the end of the turn after it was applied
                }, order: 23),
                OnEnd = OnEndEventInfo.Create((battle, target) =>
                {
                    // TS always adds the message (no DisplayUi check)
                    battle.Add("-end", target, "move: Yawn", "[silent]");
                    // Try to set sleep status
                    target.TrySetStatus(ConditionId.Sleep, battle.EffectState.Source);
                }),
            },
            [ConditionId.ZenMode] = new()
            {
                Id = ConditionId.ZenMode,
                Name = "Zen Mode",
                EffectType = EffectType.Condition,
                AssociatedAbility = AbilityId.ZenMode,
                OnStart = OnStartEventInfo.Create((_, target, _, _) =>
                {
                    if (target is null) return null;

                    // Check if it's a Galar form
                    bool isGalar = target.Species.Forme is FormeId.Galar or FormeId.GalarZen;

                    if (!isGalar)
                    {
                        if (target.Species.Id != SpecieId.DarmanitanZen)
                        {
                            target.FormeChange(SpecieId.DarmanitanZen);
                        }
                    }
                    else
                    {
                        if (target.Species.Id != SpecieId.DarmanitanGalarZen)
                        {
                            target.FormeChange(SpecieId.DarmanitanGalarZen);
                        }
                    }

                    return null;
                }),
                OnEnd = OnEndEventInfo.Create((_, pokemon) =>
                {
                    if (pokemon == null) return;

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