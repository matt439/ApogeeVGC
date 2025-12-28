using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
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
            [ConditionId.Yawn] = new()
            {
                Id = ConditionId.Yawn,
                Name = "Yawn",
                EffectType = EffectType.Condition,
                NoCopy = true, // doesn't get copied by Baton Pass
                Duration = 2,
                OnStart = new OnStartEventInfo((battle, target, source, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "move: Yawn", $"[of] {source}");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, target, _, _) =>
                {
                    // Yawn resolves at the end of the turn after it was applied
                }, 23),
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "move: Yawn", "[silent]");
                    }
                    // Try to set sleep status
                    target.TrySetStatus(ConditionId.Sleep, battle.EffectState.Source);
                }),
            },
            [ConditionId.Wish] = new()
            {
                Id = ConditionId.Wish,
                Name = "Wish",
                EffectType = EffectType.Condition,
                // This is a slot condition
                OnStart = new OnStartEventInfo((battle, pokemon, source, _) =>
                {
                    battle.EffectState.Hp = source != null ? source.MaxHp / 2 : pokemon.MaxHp / 2;
                    battle.EffectState.StartingTurn = battle.Turn; // TODO: Use GetOverflowedTurnCount() if available
                    if (battle.EffectState.StartingTurn >= 255)
                    {
                        // In Gen 8+, Wish will never resolve when used on the 255th turn
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, target, _, _) =>
                {
                    // TODO: Use GetOverflowedTurnCount() if available
                    if (battle.Turn <= battle.EffectState.StartingTurn) return;
                    var slotTarget = battle.GetAtSlot(battle.EffectState.SourceSlot);
                    if (slotTarget != null)
                    {
                        target.Side.RemoveSlotCondition(slotTarget, _library.Conditions[ConditionId.Wish]);
                    }
                }, 4),
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (target != null && !target.Fainted)
                    {
                        var healAmount = battle.EffectState.Hp ?? 0;
                        var damage = battle.Heal(healAmount, target, target);
                        if (damage is IntIntFalseUnion { Value: > 0 } && battle.DisplayUi)
                        {
                            var wisherName = battle.EffectState.Source?.Name ?? "unknown";
                            battle.Add("-heal", target, target.GetHealth,
                                "[from] move: Wish", $"[wisher] {wisherName}");
                        }
                    }
                }),
            },
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
