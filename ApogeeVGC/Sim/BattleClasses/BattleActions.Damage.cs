using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleActions
{
    /// <summary>
    /// Calculates damage for a move.
    /// 0 is a success dealing 0 damage, such as from False Swipe at 1 HP.
    /// 
    /// Normal PS return value rules apply:
    /// undefined = success, null = silent failure, false = loud failure
    /// </summary>
    public IntUndefinedFalseUnion GetDamage(Pokemon source, Pokemon target, ActiveMove move,
        bool suppressMessages = false)
    {
        // Check immunity
        if (!target.RunImmunity(move, !suppressMessages))
        {
            return IntUndefinedFalseUnion.FromFalse();
        }

        // OHKO moves deal max HP damage (Gen 9)
        if (move.Ohko != null)
        {
            return target.MaxHp;
        }

        // Damage callback (e.g., Seismic Toss, Night Shade)
        if (move.DamageCallback != null)
        {
            IntFalseUnion? damageResult = Battle.InvokeCallback<IntFalseUnion>(
                move.DamageCallback,
                Battle,
                source,
                target,
                move
            );
            return damageResult?.ToIntUndefinedFalseUnion() ?? IntUndefinedFalseUnion.FromFalse();
        }

        // Fixed damage moves
        if (move.Damage is LevelMoveDamage)
        {
            return source.Level;
        }
        else if (move.Damage is IntMoveDamage intDmg)
        {
            return intDmg.Value;
        }

        // Get move category (Physical/Special)
        MoveCategory category = Battle.GetCategory(move);

        // Calculate base power
        IntFalseUnion? basePower = move.BasePower;
        if (move.BasePowerCallback != null)
        {
            basePower = Battle.InvokeCallback<IntFalseUnion>(
                move.BasePowerCallback,
                Battle,
                source,
                target,
                move
            );
        }

        // Base power checks
        if (basePower == null || basePower == 0)
        {
            return basePower == 0 ? new Undefined() : IntUndefinedFalseUnion.FromFalse();
        }

        basePower = Battle.ClampIntRange(basePower.ToInt(), 1, null);

        // Calculate critical hit ratio (Gen 7+ logic, used in Gen 9)
        int critRatio = move.CritRatio ?? 0;
        RelayVar critRatioEvent = Battle.RunEvent(EventId.ModifyCritRatio, source,
            RunEventSource.FromNullablePokemon(target), move, critRatio);

        if (critRatioEvent is IntRelayVar irv)
        {
            critRatio = irv.Value;
        }

        critRatio = Battle.ClampIntRange(critRatio, 0, 4);

        // Gen 7+ critical hit multipliers (used in Gen 9)
        int[] critMult = [0, 24, 8, 2, 1];

        // Determine if move will critically hit
        MoveHitResult moveHit = target.GetMoveHitData(move);
        moveHit.Crit = move.WillCrit ?? false;

        if (move.WillCrit == null)
        {
            if (critRatio > 0)
            {
                moveHit.Crit = Battle.RandomChance(1, critMult[critRatio]);
            }
        }

        // Run CriticalHit event (can be cancelled)
        if (moveHit.Crit)
        {
            RelayVar critEvent = Battle.RunEvent(EventId.CriticalHit, target,
                RunEventSource.FromNullablePokemon(null), move);
            moveHit.Crit = critEvent is not BoolRelayVar { Value: false };
        }

        // Run BasePower event after crit calculation
        RelayVar basePowerEvent = Battle.RunEvent(EventId.BasePower, source,
            RunEventSource.FromNullablePokemon(target), move, basePower.ToInt(), true);

        if (basePowerEvent is IntRelayVar bpIrv)
        {
            basePower = bpIrv.Value;
        }

        if (basePower == 0)
        {
            return 0;
        }

        basePower = Battle.ClampIntRange(basePower.ToInt(), 1, null);

        // Terastallization 60 BP boost for low base power moves (Gen 9)
        Move dexMove = Library.Moves[move.Id];
        if (source.Terastallized != null &&
            (source.Terastallized == MoveType.Stellar
                ? !source.StellarBoostedTypes.Contains(move.Type)
                : source.HasType((PokemonType)move.Type)) &&
            basePower.ToInt() < 60 &&
            dexMove is { Priority: <= 0, MultiHit: null } &&
            // Exclude moves like Dragon Energy with variable BP
            !(move.BasePower is 0 or 150 && move.BasePowerCallback != null))
        {
            basePower = 60;
        }

        int level = source.Level;

        // Determine attacker and defender (some moves swap offensive/defensive Pokemon)
        Pokemon attacker = move.OverrideOffensivePokemon == MoveOverridePokemon.Target
            ? target
            : source;
        Pokemon defender = move.OverrideDefensivePokemon == MoveOverridePokemon.Source
            ? source
            : target;

        // Determine stats to use
        bool isPhysical = category == MoveCategory.Physical;
        StatIdExceptHp attackStat = move.OverrideOffensiveStat ??
                                    (isPhysical ? StatIdExceptHp.Atk : StatIdExceptHp.SpA);
        StatIdExceptHp defenseStat = move.OverrideDefensiveStat ??
                                     (isPhysical ? StatIdExceptHp.Def : StatIdExceptHp.SpD);

        // Get boost values
        int atkBoosts = attacker.Boosts.GetBoost(attackStat.ConvertToBoostId());
        int defBoosts = defender.Boosts.GetBoost(defenseStat.ConvertToBoostId());

        // Determine which boosts to ignore
        bool ignoreNegativeOffensive = move.IgnoreNegativeOffensive ?? false;
        bool ignorePositiveDefensive = move.IgnorePositiveDefensive ?? false;

        // Critical hits ignore negative offensive and positive defensive boosts
        if (moveHit.Crit)
        {
            ignoreNegativeOffensive = true;
            ignorePositiveDefensive = true;
        }

        bool ignoreOffensive =
            move.IgnoreOffensive == true || (ignoreNegativeOffensive && atkBoosts < 0);
        bool ignoreDefensive =
            move.IgnoreDefensive == true || (ignorePositiveDefensive && defBoosts > 0);

        if (ignoreOffensive)
        {
            Battle.Debug("Negating (sp)atk boost/penalty.");
            atkBoosts = 0;
        }

        if (ignoreDefensive)
        {
            Battle.Debug("Negating (sp)def boost/penalty.");
            defBoosts = 0;
        }

        // Calculate actual stat values with boosts
        int attack = attacker.CalculateStat(attackStat, atkBoosts, 1, source);
        int defense = defender.CalculateStat(defenseStat, defBoosts, 1, target);

        // Adjust attackStat for Modify event naming
        attackStat = category == MoveCategory.Physical ? StatIdExceptHp.Atk : StatIdExceptHp.SpA;

        // Run Modify stat events
        EventId modifyAtkEvent = attackStat switch
        {
            StatIdExceptHp.Atk => EventId.ModifyAtk,
            StatIdExceptHp.SpA => EventId.ModifySpA,
            _ => throw new InvalidOperationException("Invalid attack stat"),
        };

        EventId modifyDefEvent = defenseStat switch
        {
            StatIdExceptHp.Def => EventId.ModifyDef,
            StatIdExceptHp.SpD => EventId.ModifySpD,
            _ => throw new InvalidOperationException("Invalid defense stat"),
        };

        RelayVar modifyAtkResult = Battle.RunEvent(modifyAtkEvent, source,
            RunEventSource.FromNullablePokemon(target), move, attack);
        attack = modifyAtkResult is IntRelayVar atkIrv ? atkIrv.Value : attack;

        RelayVar modifyDefResult = Battle.RunEvent(modifyDefEvent, target,
            RunEventSource.FromNullablePokemon(source), move, defense);
        defense = modifyDefResult is IntRelayVar defIrv ? defIrv.Value : defense;

        // Calculate base damage using the standard Pokémon damage formula
        // int(int(int(2 * L / 5 + 2) * A * P / D) / 50)
        int baseDamage = Battle.Trunc(
            Battle.Trunc(
                Battle.Trunc(
                    Battle.Trunc(2 * level / 5 + 2) * basePower.ToInt() * attack
                ) / defense
            ) / 50
        );

        // Apply damage modifiers
        return ModifyDamage(baseDamage, source, target, move, suppressMessages);
    }

    public IntUndefinedFalseUnion GetDamage(Pokemon source, Pokemon target, MoveId moveId,
        bool suppressMessages = false)
    {
        Move move = Library.Moves[moveId];
        return GetDamage(source, target, move.ToActiveMove(), suppressMessages);
    }

    public IntUndefinedFalseUnion GetDamage(Pokemon source, Pokemon target, int basePower,
        bool suppressMessages = false)
    {
        // Create a temporary move with the specified base power
        var tempMove = new Move
        {
            Id = MoveId.None,
            Name = "Custom Move",
            Num = 0,
            BasePower = basePower,
            Type = MoveType.Unknown,
            Category = MoveCategory.Physical,
            Accuracy = new TrueIntTrueUnion(),
            BasePp = 1,
            Target = MoveTarget.Normal,
            WillCrit = false,
        }.ToActiveMove();

        tempMove.Hit = 0;

        return GetDamage(source, target, tempMove, suppressMessages);
    }

    public int ModifyDamage(int baseDamage, Pokemon pokemon, Pokemon target, ActiveMove move,
        bool suppressMessages = false)
    {
        MoveType type = move.Type;

        baseDamage += 2;

        // Spread move modifier (multi-target moves do 75% damage in doubles)
        if (move.SpreadHit == true)
        {
            if (Battle.DisplayUi)
            {
                Battle.Debug("Spread modifier: 0.75");
            }

            baseDamage = Battle.Modify(baseDamage, 0.75);
        }

        // Weather modifier
        RelayVar weatherModResult = Battle.RunEvent(EventId.WeatherModifyDamage, pokemon,
            RunEventSource.FromNullablePokemon(target), move, new IntRelayVar(baseDamage));

        if (weatherModResult is IntRelayVar weatherMod)
        {
            baseDamage = weatherMod.Value;
        }

        // Critical hit - not a modifier, direct multiplication
        MoveHitResult hitData = target.GetMoveHitData(move);
        bool isCrit = hitData.Crit;

        if (isCrit)
        {
            double critModifier = move.CritModifier ?? 1.5;
            baseDamage = Battle.Trunc((int)(baseDamage * critModifier));
        }

        // Random factor - also not a modifier
        baseDamage = Battle.Randomizer(baseDamage);

        // STAB (Same Type Attack Bonus)
        // The Typeless type never gets STAB
        if (type != MoveType.Unknown)
        {
            double stab = 1.0;

            bool isStab = move.ForceStab == true ||
                          pokemon.HasType((PokemonType)type) ||
                          pokemon.GetTypes(false, true).Contains((PokemonType)type);

            if (isStab)
            {
                stab = 1.5;
            }

            // Stellar tera type handling
            if (pokemon.Terastallized == MoveType.Stellar)
            {
                if (!pokemon.StellarBoostedTypes.Contains(type) || move.StellarBoosted == true)
                {
                    stab = isStab ? 2.0 : 4915.0 / 4096.0;
                    move.StellarBoosted = true;

                    // Terapagos-Stellar doesn't consume stellar boosts
                    if (pokemon.Species.Id != SpecieId.TerapagosStellar)
                    {
                        pokemon.StellarBoostedTypes.Add(type);
                    }
                }
            }
            else
            {
                // Regular tera type handling
                if (pokemon.Terastallized == type &&
                    pokemon.GetTypes(false, true).Contains((PokemonType)type))
                {
                    stab = 2.0;
                }

                // Run ModifySTAB event
                RelayVar modifyStabResult = Battle.RunEvent(EventId.ModifyStab, pokemon,
                    RunEventSource.FromNullablePokemon(target), move,
                    new DecimalRelayVar((decimal)stab));

                if (modifyStabResult is DecimalRelayVar stabMod)
                {
                    stab = (double)stabMod.Value;
                }
            }

            baseDamage = Battle.Modify(baseDamage, stab);
        }

        // Type effectiveness
        int typeMod = target.RunEffectiveness(move).ToModifier();
        typeMod = Battle.ClampIntRange(typeMod, -6, 6);
        hitData.TypeMod = typeMod;

        if (typeMod > 0)
        {
            if (!suppressMessages && Battle.DisplayUi)
            {
                Battle.Add("-supereffective", target);
            }

            for (int i = 0; i < typeMod; i++)
            {
                baseDamage *= 2;
            }
        }

        if (typeMod < 0)
        {
            if (!suppressMessages && Battle.DisplayUi)
            {
                Battle.Add("-resisted", target);
            }

            for (int i = 0; i > typeMod; i--)
            {
                baseDamage = Battle.Trunc((int)(baseDamage / 2.0));
            }
        }

        if (isCrit && !suppressMessages && Battle.DisplayUi)
        {
            Battle.Add("-crit", target);
        }

        // Burn modifier for physical moves (except Facade and Guts users)
        if (pokemon.Status == ConditionId.Burn &&
            move.Category == MoveCategory.Physical &&
            !pokemon.HasAbility(AbilityId.Guts))
        {
            // In Gen 6+, Facade ignores the burn modifier
            if (move.Id != MoveId.Facade)
            {
                baseDamage = Battle.Modify(baseDamage, 0.5);
            }
        }

        // Final modifier - Life Orb, etc.
        RelayVar finalModResult = Battle.RunEvent(EventId.ModifyDamage, pokemon,
            RunEventSource.FromNullablePokemon(target), move, new IntRelayVar(baseDamage));

        if (finalModResult is IntRelayVar finalMod)
        {
            baseDamage = finalMod.Value;
        }

        // Minimum 1 damage check (Gen 6+)
        if (baseDamage == 0)
        {
            return 1;
        }

        // 16-bit truncation (can truncate to 0)
        return Battle.Trunc(baseDamage, 16);
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
        int attack =
            pokemon.CalculateStat(StatIdExceptHp.Atk, pokemon.Boosts.GetBoost(BoostId.Atk));
        int defense =
            pokemon.CalculateStat(StatIdExceptHp.Def, pokemon.Boosts.GetBoost(BoostId.Def));
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

    public int CalcRecoilDamage(int damageDealt, Move move, Pokemon pokemon)
    {
        // Chloroblast is a special case - returns 50% of max HP as recoil
        if (move.Id == MoveId.Chloroblast)
        {
            return (int)Math.Round(pokemon.MaxHp / 2.0);
        }

        // Standard recoil calculation: damageDealt * recoil[0] / recoil[1]
        // Clamped to minimum of 1
        if (move.Recoil == null) return 0;

        int recoilDamage = (int)Math.Round(damageDealt * move.Recoil.Value.Item1 /
                                           (double)move.Recoil.Value.Item2);
        return Battle.ClampIntRange(recoilDamage, 1, null);
    }
}