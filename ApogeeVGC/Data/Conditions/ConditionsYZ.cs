using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
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
            [ConditionId.Wish] = new()
            {
                Id = ConditionId.Wish,
                Name = "Wish",
                EffectType = EffectType.Condition,
                // This is a slot condition
                OnStart = new OnStartEventInfo((battle, pokemon, source, _) =>
                {
                    battle.EffectState.Hp = source != null ? source.MaxHp / 2 : pokemon.MaxHp / 2;
                    battle.EffectState.StartingTurn = battle.GetOverflowedTurnCount();
                    if (battle.EffectState.StartingTurn == 255)
                    {
                        battle.Hint($"In Gen 8+, Wish will never resolve when used on the {battle.Turn}th turn.");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, target, _, _) =>
                {
                    // Use GetOverflowedTurnCount for proper Gen 8+ turn overflow handling
                    if (battle.GetOverflowedTurnCount() <= battle.EffectState.StartingTurn) return;
                    Pokemon? slotTarget = battle.GetAtSlot(battle.EffectState.SourceSlot);
                    if (slotTarget != null)
                    {
                        target.Side.RemoveSlotCondition(slotTarget,
                            _library.Conditions[ConditionId.Wish]);
                    }
                }, 4),
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (target is { Fainted: false })
                    {
                        int healAmount = battle.EffectState.Hp ?? 0;
                        IntFalseUnion damage = battle.Heal(healAmount, target, target);
                        if (damage is IntIntFalseUnion { Value: > 0 })
                        {
                            string wisherName = battle.EffectState.Source?.Name ?? "unknown";
                            battle.Add("-heal", target, target.GetHealth,
                                "[from] move: Wish", $"[wisher] {wisherName}");
                        }
                    }
                }),
            },
            [ConditionId.Yawn] = new()
            {
                Id = ConditionId.Yawn,
                Name = "Yawn",
                AssociatedMove = MoveId.Yawn,
                EffectType = EffectType.Condition,
                NoCopy = true, // doesn't get copied by Baton Pass
                Duration = 2,
                OnStart = new OnStartEventInfo((battle, target, source, _) =>
                {
                    // TS always adds the message (no DisplayUi check)
                    battle.Add("-start", target, "move: Yawn", $"[of] {source}");
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((_, _, _, _) =>
                {
                    // Yawn resolves at the end of the turn after it was applied
                }, 23),
                OnEnd = new OnEndEventInfo((battle, target) =>
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
                OnStart = new OnStartEventInfo((_, target, _, _) =>
                {
                    if (target is null) return BoolVoidUnion.FromVoid();

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

                    return BoolVoidUnion.FromVoid();
                }),
                OnEnd = new OnEndEventInfo((_, pokemon) =>
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