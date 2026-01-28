using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesYz()
    {
        return new Dictionary<AbilityId, Ability>
        {
            // ==================== 'Y' Abilities ====================
            // No standard abilities start with 'Y' in Generation 9

            // ==================== 'Z' Abilities ====================
            [AbilityId.ZenMode] = new()
            {
                Id = AbilityId.ZenMode,
                Name = "Zen Mode",
                Num = 161,
                Rating = 0.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    CantSuppress = true,
                },
                Condition = ConditionId.ZenMode,
                // OnResidualOrder = 29
                OnResidual = new OnResidualEventInfo((_, pokemon, _, _) =>
                {
                    // Only works for Darmanitan that hasn't transformed
                    if (pokemon.BaseSpecies.BaseSpecies != SpecieId.Darmanitan ||
                        pokemon.Transformed)
                    {
                        return;
                    }

                    bool isInZenForme = pokemon.Species.Forme is FormeId.Zen or FormeId.GalarZen;

                    // Check if HP is at or below 50%
                    if (pokemon.Hp <= pokemon.MaxHp / 2 && !isInZenForme)
                    {
                        // Transform to Zen Mode
                        pokemon.AddVolatile(ConditionId.ZenMode);
                    }
                    else if (pokemon.Hp > pokemon.MaxHp / 2 && isInZenForme)
                    {
                        // Add volatile in case of base Darmanitan-Zen, then remove to trigger reversion
                        pokemon.AddVolatile(ConditionId.ZenMode);
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.ZenMode]);
                    }
                }, order: 29),
                OnEnd = new OnEndEventInfo((battle, pokemonUnion) =>
                {
                    if (pokemonUnion is not PokemonSideFieldPokemon psfp) return;
                    Pokemon pokemon = psfp.Pokemon;

                    // Add null check for Volatiles dictionary
                    if (pokemon.Volatiles == null ||
                        !pokemon.Volatiles.ContainsKey(ConditionId.ZenMode) ||
                        pokemon.Hp == 0) return;

                    pokemon.Transformed = false;
                    pokemon.Volatiles.Remove(ConditionId.ZenMode);

                    // If in a battle-only forme, revert
                    if (pokemon.Species is
                        {
                            BaseSpecies: SpecieId.Darmanitan, Forme: FormeId.Zen or FormeId.GalarZen
                        })
                    {
                        SpecieId baseForme = pokemon.Species.Forme == FormeId.GalarZen
                            ? SpecieId.DarmanitanGalar
                            : SpecieId.Darmanitan;
                        pokemon.FormeChange(baseForme, battle.Effect, message: "[silent]");
                    }
                }),
            },
            [AbilityId.ZeroToHero] = new()
            {
                Id = AbilityId.ZeroToHero,
                Name = "Zero to Hero",
                Num = 278,
                Rating = 5.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    CantSuppress = true,
                    NoTransform = true,
                },
                OnSwitchOut = new OnSwitchOutEventInfo((battle, pokemon) =>
                {
                    if (pokemon.BaseSpecies.BaseSpecies != SpecieId.Palafin) return;

                    // If not already in Hero forme, transform into it
                    if (pokemon.Species.Forme != FormeId.Hero)
                    {
                        pokemon.FormeChange(SpecieId.PalafinHero, battle.Effect, true);
                        pokemon.HeroMessageDisplayed = false;
                    }
                }),
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    if (pokemon.BaseSpecies.BaseSpecies != SpecieId.Palafin) return;

                    // Display the Hero message if in Hero forme and not already displayed
                    if (pokemon is { HeroMessageDisplayed: false, Species.Forme: FormeId.Hero })
                    {
                        battle.Add("-activate", pokemon, "ability: Zero to Hero");
                        pokemon.HeroMessageDisplayed = true;
                    }
                }),
            },
        };
    }
}