using ApogeeVGC.Data;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.BattleClasses;

public class BattleActions(IBattle battle)
{
    public IBattle Battle { get; init; } = battle;
    public Library Library => Battle.Library;

    public MoveTypeFalseUnion CanTerastallize(IBattle battle, Pokemon pokemon)
    {
        throw new NotImplementedException();
    }

    ///**
    // * Confusion damage is unique - most typical modifiers that get run when calculating
    // * damage (e.g. Huge Power, Life Orb, critical hits) don't apply. It also uses a 16-bit
    // * context for its damage, unlike the regular damage formula (though this only comes up
    // * for base damage).
    // */
    //getConfusionDamage(pokemon: Pokemon, basePower: number)
    //{
    //    const tr = this.battle.trunc;

    //    const attack = pokemon.calculateStat('atk', pokemon.boosts['atk']);
    //    const defense = pokemon.calculateStat('def', pokemon.boosts['def']);
    //    const level = pokemon.level;
    //    const baseDamage = tr(tr(tr(tr(2 * level / 5 + 2) * basePower * attack) / defense) / 50) + 2;

    //    // Damage is 16-bit context in self-hit confusion damage
    //    let damage = tr(baseDamage, 16);
    //    damage = this.battle.randomizer(damage);
    //    return Math.max(1, damage);
    //}

    public int GetConfusionDamage(Pokemon pokemon, int basePower)
    {
        throw new NotImplementedException();
    }
}