using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    /// <summary>
    /// Applies damage to this Pokemon.
    /// If damage reduces HP to 0 or below, queues the Pokemon to faint.
    /// </summary>
    /// <param name="d">Amount of damage to deal</param>
    /// <param name="source">The Pokemon dealing the damage</param>
    /// <param name="effect">The effect causing the damage</param>
    /// <returns>Actual damage dealt (adjusted for overkill)</returns>
    public int Damage(int d, Pokemon? source = null, IEffect? effect = null)
    {
        // Early exit if Pokemon has no HP, damage is invalid, or damage is non-positive
        if (Hp <= 0 || d <= 0) return 0;

        // Truncate decimal values
        d = Battle.Trunc(d);

        // Apply damage
        Hp -= d;

        // Check if Pokemon should faint
        if (Hp > 0) return d;
        // Adjust damage for overkill (Hp is negative, so this reduces d)
        d += Hp;

        // Queue faint
        Faint(source, effect);

        return d;
    }

    /// <summary>
    /// Heals the Pokemon by a specified amount.
    /// Returns the actual amount of HP healed (capped at max HP).
    /// Returns false if healing is not possible.
    /// </summary>
    /// <param name="d">Amount to heal</param>
    /// <param name="source">The Pokemon causing the heal (optional)</param>
    /// <param name="effect">The effect causing the heal (optional)</param>
    /// <returns>Actual amount healed, or false if healing failed</returns>
    public IntFalseUnion Heal(int d, Pokemon? source = null, IEffect? effect = null)
    {
        // Early exit if Pokemon is fainted
        if (Hp <= 0) return IntFalseUnion.FromFalse();

        // Truncate decimal values (Battle.Trunc handles this)
        d = Battle.Trunc(d);

        // Validate heal amount is positive
        if (d <= 0) return IntFalseUnion.FromFalse();

        // Early exit if already at max HP
        if (Hp >= MaxHp) return IntFalseUnion.FromFalse();

        // Apply healing
        Hp += d;

        // Cap at max HP and adjust heal amount if overhealed
        if (Hp <= MaxHp) return IntFalseUnion.FromInt(d);
        d -= Hp - MaxHp; // Reduce d by the overheal amount
        Hp = MaxHp;

        // Return actual amount healed
        return IntFalseUnion.FromInt(d);
    }

    /// <summary>
    /// Sets the Pokemon's HP to a specific value.
    /// Returns the delta (change in HP).
    /// Minimum HP is 1 (cannot set to 0 via this method).
    /// </summary>
    /// <param name="d">Target HP value</param>
    /// <returns>The actual change in HP (delta), or null if Pokemon is fainted</returns>
    public int? SetHp(int d)
    {
        // Early exit if Pokemon is fainted
        if (Hp <= 0) return 0;

        // Truncate decimal values
        d = Battle.Trunc(d);

        // Ensure minimum HP of 1
        if (d < 1) d = 1;

        // Calculate delta (difference between target and current HP)
        d -= Hp;

        // Apply the change
        Hp += d;

        // Cap at max HP and adjust delta if exceeded
        if (Hp <= MaxHp) return d;
        d -= Hp - MaxHp; // Reduce d by the overheal amount
        Hp = MaxHp;

        // Return the actual change in HP
        return d;
    }

    /// <summary>
    /// This function only puts the Pokemon in the faint queue;
    /// actually setting Fainted comes later when the faint queue is resolved.
    /// Returns the amount of HP the Pokemon had (damage dealt).
    /// </summary>
    /// <param name="source">The Pokemon that caused the fainting</param>
    /// <param name="effect">The effect that caused the fainting</param>
    /// <returns>The amount of HP the Pokemon had before fainting</returns>
    public int Faint(Pokemon? source = null, IEffect? effect = null)
    {
        // Early exit if already fainted or queued to faint
        if (Fainted || FaintQueued) return 0;

        // Store current HP (the damage dealt)
        int damage = Hp;

        // Set HP to 0
        Hp = 0;

        // Clear switch flag
        SwitchFlag = false;

        // Mark as queued for fainting
        FaintQueued = true;

        // Add to battle's faint queue
        Battle.FaintQueue.Add(new FaintQueue
        {
            Target = this,
            Source = source,
            Effect = effect,
        });

        return damage;
    }

    /// <summary>
    /// Records that this Pokemon was attacked by another Pokemon's move.
    /// Used for moves like Counter, Mirror Coat, Revenge, Bide, etc.
    /// </summary>
    /// <param name="moveId">The move that hit this Pokemon</param>
    /// <param name="damage">The damage dealt (can be false/null if no damage)</param>
    /// <param name="source">The Pokemon that attacked</param>
    public void GotAttacked(MoveId moveId, IntFalseUnion? damage, Pokemon source)
    {
        Move move = Battle.Library.Moves[moveId];
        GotAttacked(move, damage, source);
    }

    /// <summary>
    /// Records that this Pokemon was attacked by another Pokemon's move.
    /// Used for moves like Counter, Mirror Coat, Revenge, Bide, etc.
    /// </summary>
    /// <param name="move">The move that hit this Pokemon</param>
    /// <param name="damage">The damage dealt (can be false/null if no damage)</param>
    /// <param name="source">The Pokemon that attacked</param>
    public void GotAttacked(Move move, IntFalseUnion? damage, Pokemon source)
    {
        // Convert damage to numeric value (0 if false/null)
        int damageNumber = damage switch
        {
            IntIntFalseUnion intDamage => intDamage.Value,
            _ => 0,
        };

        // Add attack record to the list
        AttackedBy.Add(new Attacker
        {
            Source = source,
            Damage = damageNumber,
            Move = move.Id,
            ThisTurn = true,
            PokemonSlot = source.GetSlot(),
            DamageValue = damage,
        });
    }

    public Attacker? GetLastAttackedBy()
    {
        return AttackedBy.Count == 0 ? null : AttackedBy[^1];
    }

    /// <summary>
    /// Gets the most recent attacker that actually dealt numeric damage to this Pokemon.
    /// Used for moves like Metal Burst, Revenge, and Avalanche.
    /// </summary>
    /// <param name="filterOutSameSide">If true, exclude attacks from allies</param>
    /// <returns>The last damaging attacker, or null if none found</returns>
    public Attacker? GetLastDamagedBy(bool filterOutSameSide = false)
    {
        // Filter attackers that dealt actual numeric damage
        var damagedBy = AttackedBy.Where(attacker =>
        {
            // Check if damageValue is a numeric value (not false/null)
            bool hasNumericDamage = attacker.DamageValue is IntIntFalseUnion;

            // If no same-side filtering, just check damage
            if (!filterOutSameSide)
                return hasNumericDamage;

            // With same-side filtering, also check if attacker is not an ally
            return hasNumericDamage && !IsAlly(attacker.Source);
        }).ToList();

        // Return the last (most recent) damaging attacker, or null if list is empty
        return damagedBy.Count == 0 ? null : damagedBy[^1];
    }
}