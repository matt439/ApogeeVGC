using ApogeeVGC.Data;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.BattleClasses;

public class BattleActions(IBattle battle)
{
    public IBattle Battle { get; init; } = battle;
    public Library Library => Battle.Library;

    public MoveType? CanTerastallize(IBattle battle, Pokemon pokemon)
    {
        if (Battle.Gen != 9)
        {
            return null;
        }
        return pokemon.TeraType;
    }

    /// <summary>
    /// Calculates confusion self-hit damage.
    /// 
    /// Confusion damage is unique - most typical modifiers that get run when calculating
    /// damage (e.g. Huge Power, Life Orb, critical hits) don't apply. It also uses a 16-bit
    /// context for its damage, unlike the regular damage formula (though this only comes up
    /// for base damage).
    /// </summary>
    /// <param name="pokemon">The confused Pokémon hitting itself</param>
    /// <param name="basePower">Base power of the confusion damage (typically 40)</param>
    /// <returns>The calculated damage amount (minimum 1)</returns>
    public int GetConfusionDamage(Pokemon pokemon, int basePower)
    {
        // Get the Pokémon's attack and defense stats with current boosts applied
        int attack = pokemon.CalculateStat(StatIdExceptHp.Atk, pokemon.Boosts.GetBoost(BoostId.Atk));
        int defense = pokemon.CalculateStat(StatIdExceptHp.Def, pokemon.Boosts.GetBoost(BoostId.Def));
        int level = pokemon.Level;

        // Calculate base damage using the standard Pokémon damage formula
        // Formula: ((2 * level / 5 + 2) * basePower * attack / defense) / 50 + 2
        // Each step is truncated to match game behavior
        int baseDamage = Battle.Trunc(
            Battle.Trunc(
                Battle.Trunc(
                    Battle.Trunc(2 * level / 5 + 2) * basePower * attack
                ) / defense
            ) / 50
        ) + 2;

        // Apply 16-bit truncation for confusion damage
        // This only matters for extremely high damage values (Eternatus-Eternamax level stats)
        int damage = Battle.Trunc(baseDamage, 16);

        // Apply random damage variance (85-100% of calculated damage)
        damage = Battle.Randomizer(damage);

        // Ensure at least 1 damage is dealt
        return Math.Max(1, damage);
    }
}