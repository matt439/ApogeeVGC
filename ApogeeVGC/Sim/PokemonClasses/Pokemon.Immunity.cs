using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    /// <summary>
    /// Checks if the Pokemon is immune to a specific move type.
    /// </summary>
    /// <param name="source">The move or type string to check immunity against</param>
    /// <param name="message">If true/non-empty, display immunity messages</param>
    /// <returns>True if not immune (move can hit), false if immune</returns>
    public bool RunImmunity(ActiveMove source, bool message = false)
    {
        return RunImmunity(source.Type, message);
    }

    /// <summary>
    /// Checks if the Pokemon is immune to a specific type.
    /// </summary>
    /// <param name="source">The type to check immunity against</param>
    /// <param name="message">If true, display immunity messages</param>
    /// <returns>True if not immune, false if immune</returns>
    public bool RunImmunity(MoveType source, bool message = false)
    {
        // Null or unknown type is never immune
        if (source == MoveType.Unknown)
        {
            return true;
        }

        // Run NegateImmunity event
        RelayVar? negateEvent = Battle.RunEvent(EventId.NegateImmunity, this, null, null,
            source);
        bool negateImmunity = negateEvent is not BoolRelayVar { Value: true };

        // Special handling for Ground-type
        bool? notImmune;
        if (source == MoveType.Ground)
        {
            notImmune = IsGrounded(negateImmunity);
        }
        else
        {
            // Check type immunity using the dex
            notImmune = negateImmunity || Battle.Dex.GetImmunity(source, this);
        }

        // If not immune, return true
        if (notImmune == true)
        {
            return true;
        }

        // If no message requested, just return false
        if (!message)
        {
            return false;
        }

        // Display appropriate immunity message only if DisplayUi is true
        if (Battle.DisplayUi)
        {
            if (notImmune == null)
            {
                // Levitate ability immunity
                Battle.Add("-immune", this, "[from] ability: Levitate");
            }
            else
            {
                // General immunity
                Battle.Add("-immune", this);
            }
        }

        return false;
    }

    /// <summary>
    /// Checks status immunity based on Pokemon type (e.g., Fire-types immune to Burn)
    /// </summary>
    public bool RunStatusImmunity(PokemonType? type, string? message = null)
    {
        // Check if Pokemon is fainted
        if (Fainted) return false;

        // Equivalent to TypeScript: if (!type) return true;
        if (type == null) return true;

        // Convert PokemonType to MoveType for immunity check
        MoveType moveType = type.Value.ConvertToMoveType();

        // Check natural type immunity using battle's dex
        if (!Battle.Dex.GetImmunity(moveType, this))
        {
            if (Battle.DisplayUi)
            {
                Battle.Debug("natural status immunity");
                if (!string.IsNullOrEmpty(message))
                {
                    Battle.Add("-immune", this);
                }
            }
            return false;
        }

        // Check artificial immunity (abilities, items, etc.)
        RelayVar? immunity = Battle.RunEvent(EventId.Immunity, this, null, null, type);

        // TypeScript logic: if (!immunity) - means if immunity is falsy/false
        if (immunity is BoolRelayVar { Value: false } or null)
        {
            if (Battle.DisplayUi)
            {
                Battle.Debug("artificial status immunity");
                // TypeScript: if (message && immunity !== null)
                if (!string.IsNullOrEmpty(message) && immunity is not null)
                {
                    Battle.Add("-immune", this);
                }
            }
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks immunity for specific conditions (e.g., Sleep immunity for certain Pokemon types)
    /// </summary>
    public bool RunStatusImmunity(ConditionId? conditionId, string? message = null)
    {
        // Check if Pokemon is fainted
        if (Fainted) return false;

        // Equivalent to TypeScript: if (!type) return true;
        if (conditionId == null) return true;

        // Get the condition from the library
        Condition condition = Battle.Library.Conditions[conditionId.Value];

        // Check natural condition immunity using ModdedDex static method
        if (!ModdedDex.GetImmunity(condition, Types))
        {
            if (Battle.DisplayUi)
            {
                Battle.Debug("natural condition immunity");
                if (!string.IsNullOrEmpty(message))
                {
                    Battle.Add("-immune", this);
                }
            }
            return false;
        }

        // Check artificial immunity (abilities, items, etc.) for conditions
        RelayVar? immunity = Battle.RunEvent(EventId.Immunity, this, null, null,
            conditionId);

        // TypeScript logic: if (!immunity) - means if immunity is falsy/false
        if (immunity is BoolRelayVar { Value: false } or null)
        {
            if (Battle.DisplayUi)
            {
                Battle.Debug("artificial condition immunity");
                // TypeScript: if (message && immunity !== null)
                if (!string.IsNullOrEmpty(message) && immunity is not null)
                {
                    Battle.Add("-immune", this);
                }
            }
            return false;
        }

        return true;
    }

    /// <summary>
    /// Attempts to trap this Pokemon, preventing it from switching out.
    /// </summary>
    /// <param name="isHidden">If true, the trap is hidden (e.g., Shadow Tag ability)</param>
    /// <returns>True if successfully trapped, false if immune</returns>
    public bool TryTrap(bool isHidden = false)
    {
        // Check immunity to trapped status
        if (!RunStatusImmunity(ConditionId.Trapped))
        {
            return false;
        }

        // If already trapped and this is a hidden trap attempt, return true
        if (Trapped != PokemonTrapped.False && isHidden)
        {
            return true;
        }

        // Set trapped state (hidden or regular)
        Trapped = isHidden ? PokemonTrapped.Hidden : PokemonTrapped.True;

        return true;
    }

    /// <summary>
    /// Checks if the Pokemon is grounded (affected by Ground-type moves and terrain).
    /// Returns true if grounded, false if not grounded, null if Levitate provides immunity.
    /// </summary>
    /// <param name="negateImmunity">If true, ignore type-based immunity (for moves like Thousand Arrows)</param>
    /// <returns>True if grounded, false if not grounded, null if Levitate ability</returns>
    public bool? IsGrounded(bool negateImmunity = false)
    {
        // Gravity forces all Pokemon to be grounded
        if (Battle.Field.PseudoWeather.ContainsKey(ConditionId.Gravity))
        {
            return true;
        }

        // Ingrain grounds the Pokemon (Gen 4+)
        if (Volatiles.ContainsKey(ConditionId.Ingrain) && Battle.Gen >= 4)
        {
            return true;
        }

        // Smackdown grounds the Pokemon
        if (Volatiles.ContainsKey(ConditionId.SmackDown))
        {
            return true;
        }

        // Get effective item (empty if ignoring item)
        ItemId effectiveItem = IgnoringItem() ? ItemId.None : Item;

        // Iron Ball grounds the Pokemon
        if (effectiveItem == ItemId.IronBall)
        {
            return true;
        }

        // Flying-type immunity check (unless negated)
        // Special case: Fire/Flying using Burn Up + Roost becomes ???/Flying but is still grounded
        if (!negateImmunity && HasType(PokemonType.Flying))
        {
            // Exception: ???-type + Roost active means it's still grounded
            bool roosting = HasType(PokemonType.Unknown) && Volatiles.ContainsKey(ConditionId.Roost);
            if (!roosting)
            {
                return false;
            }
        }

        // Levitate ability provides immunity (unless ability is being suppressed)
        if (HasAbility(AbilityId.Levitate))
        {
            return null; // Special return value indicating Levitate immunity
        }

        // Magnet Rise makes Pokemon airborne
        if (Volatiles.ContainsKey(ConditionId.MagnetRise))
        {
            return false;
        }

        // Telekinesis makes Pokemon airborne
        if (Volatiles.ContainsKey(ConditionId.Telekinesis))
        {
            return false;
        }

        // Air Balloon makes Pokemon airborne (unless popped)
        return effectiveItem != ItemId.AirBalloon;
    }

    /// <summary>
    /// Checks if the Pokemon is semi-invulnerable (untargetable due to two-turn moves).
    /// </summary>
    public bool IsSemiInvulnerable()
    {
        // List of all semi-invulnerable conditions
        ConditionId[] semiInvulnerableConditions =
        [
            ConditionId.Fly,
            ConditionId.Bounce,
            ConditionId.Dive,
            ConditionId.Dig,
            ConditionId.PhantomForce,
            ConditionId.ShadowForce,
        ];

        return semiInvulnerableConditions.Any(Volatiles.ContainsKey) || IsSkyDropped();
    }

    /// <summary>
    /// Checks if this Pokemon is affected by Sky Drop (either as target or source).
    /// </summary>
    public bool IsSkyDropped()
    {
        // Check if this Pokemon is the target of Sky Drop
        if (Volatiles.ContainsKey(ConditionId.SkyDrop))
        {
            return true;
        }

        // Check if this Pokemon is the source of Sky Drop on any opponent
        return Side.Foe.Active.Any(foeActive =>
            foeActive != null && foeActive.Volatiles.TryGetValue(ConditionId.SkyDrop, out EffectState? state) &&
            state.Source == this);
    }

    /// <summary>
    /// Checks if the Pokemon is protected against a single-target damaging move.
    /// Returns true if the Pokemon has any protection volatile status (Protect, Detect, King's Shield, etc.)
    /// </summary>
    /// <returns>True if protected, false otherwise</returns>
    public bool IsProtected()
    {
        return Volatiles.ContainsKey(ConditionId.Protect) ||
               Volatiles.ContainsKey(ConditionId.Detect) ||
               Volatiles.ContainsKey(ConditionId.MaxGuard) ||
               Volatiles.ContainsKey(ConditionId.KingsShield) ||
               Volatiles.ContainsKey(ConditionId.SpikyShield) ||
               Volatiles.ContainsKey(ConditionId.BanefulBunker) ||
               Volatiles.ContainsKey(ConditionId.Obstruct) ||
               Volatiles.ContainsKey(ConditionId.SilkTrap) ||
               Volatiles.ContainsKey(ConditionId.BurningBulwark);
    }
}