namespace ApogeeVGC.Sim.BattleClasses;

/// <summary>
/// Base class for all battle messages that are sent to the GUI.
/// These messages represent battle events that should be displayed to the player.
/// In the future, some of these may trigger animations.
/// </summary>
public abstract class BattleMessage
{
    /// <summary>
    /// Timestamp when the message was created
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Convert the message to a human-readable string
    /// </summary>
    public abstract string ToDisplayText();
}

/// <summary>
/// Message for when a Pokémon uses a move
/// </summary>
public class MoveUsedMessage : BattleMessage
{
    public required string PokemonName { get; init; }
    public required string MoveName { get; init; }

    public override string ToDisplayText()
    {
        return $"{PokemonName} used {MoveName}!";
    }
}

/// <summary>
/// Message for move effectiveness (super effective, not very effective, etc.)
/// </summary>
public class EffectivenessMessage : BattleMessage
{
    public enum EffectivenessType
    {
        SuperEffective,
        NotVeryEffective,
        NoEffect,
    }

    public required EffectivenessType Effectiveness { get; init; }

    public override string ToDisplayText()
    {
        return Effectiveness switch
        {
            EffectivenessType.SuperEffective => "It's super effective!",
            EffectivenessType.NotVeryEffective => "It's not very effective...",
            EffectivenessType.NoEffect => "It doesn't affect the target...",
            _ => string.Empty,
        };
    }
}

/// <summary>
/// Message for when a Pokémon takes damage
/// </summary>
public class DamageMessage : BattleMessage
{
    public required string PokemonName { get; init; }
    public required int DamageAmount { get; init; }
    public required int RemainingHp { get; init; }
    public required int MaxHp { get; init; }

    /// <summary>
    /// The effect that caused the damage (e.g., "psn", "brn", "confusion", move name)
    /// </summary>
    public string? EffectName { get; init; }

    /// <summary>
    /// The source Pokemon that caused the damage (if applicable)
    /// </summary>
    public string? SourcePokemonName { get; init; }

    /// <summary>
    /// Special damage tags like "[partiallytrapped]" or "[silent]"
    /// </summary>
    public string? SpecialTag { get; init; }

    public override string ToDisplayText()
    {
        double percentRemaining = (double)RemainingHp / MaxHp * 100;

        string baseMessage =
            $"{PokemonName} took {DamageAmount} damage! ({percentRemaining:F1}% HP remaining)";

        if (!string.IsNullOrEmpty(EffectName))
        {
            baseMessage += $" from {EffectName}";
        }

        if (!string.IsNullOrEmpty(SourcePokemonName))
        {
            baseMessage += $" (source: {SourcePokemonName})";
        }

        return baseMessage;
    }
}

/// <summary>
/// Message for when a Pokémon faints
/// </summary>
public class FaintMessage : BattleMessage
{
    public required string PokemonName { get; init; }

    public override string ToDisplayText()
    {
        return $"{PokemonName} fainted!";
    }
}

/// <summary>
/// Message for when a Pokémon is switched in
/// </summary>
public class SwitchMessage : BattleMessage
{
    public required string TrainerName { get; init; }
    public required string PokemonName { get; init; }

    public override string ToDisplayText()
    {
        return $"{TrainerName} sent out {PokemonName}!";
    }
}

/// <summary>
/// Message for a critical hit
/// </summary>
public class CriticalHitMessage : BattleMessage
{
    public override string ToDisplayText()
    {
        return "A critical hit!";
    }
}

/// <summary>
/// Message for when a move misses
/// </summary>
public class MissMessage : BattleMessage
{
    public required string PokemonName { get; init; }

    public override string ToDisplayText()
    {
        return $"{PokemonName}'s attack missed!";
    }
}

/// <summary>
/// Message for status conditions being applied
/// </summary>
public class StatusMessage : BattleMessage
{
    public required string PokemonName { get; init; }
    public required string StatusName { get; init; }

    public override string ToDisplayText()
    {
        return $"{PokemonName} was {StatusName}!";
    }
}

/// <summary>
/// Message for weather changes
/// </summary>
public class WeatherMessage : BattleMessage
{
    public required string WeatherName { get; init; }
    public bool IsEnding { get; init; }

    public override string ToDisplayText()
    {
        return IsEnding
            ? $"The {WeatherName} stopped!"
            : $"{WeatherName} started!";
    }
}

/// <summary>
/// Message for stat changes
/// </summary>
public class StatChangeMessage : BattleMessage
{
    public required string PokemonName { get; init; }
    public required string StatName { get; init; }
    public required int Stages { get; init; }

    public override string ToDisplayText()
    {
        string direction = Stages > 0 ? "rose" : "fell";
        string amount = Math.Abs(Stages) switch
        {
            1 => "",
            2 => " sharply",
            >= 3 => " drastically",
            _ => ""
        };

        return $"{PokemonName}'s {StatName} {direction}{amount}!";
    }
}

/// <summary>
/// Message for when a move fails
/// </summary>
public class MoveFailMessage : BattleMessage
{
    public required string Reason { get; init; }

    public override string ToDisplayText()
    {
        return $"But it failed! {Reason}";
    }
}

/// <summary>
/// Generic text message for events not covered by other message types
/// </summary>
public class GenericMessage : BattleMessage
{
    public required string Text { get; init; }

    public override string ToDisplayText()
    {
        return Text;
    }
}

/// <summary>
/// Message for turn start
/// </summary>
public class TurnStartMessage : BattleMessage
{
    public required int TurnNumber { get; init; }

    public override string ToDisplayText()
    {
        return $"--- Turn {TurnNumber} ---";
    }
}

/// <summary>
/// Message for healing
/// </summary>
public class HealMessage : BattleMessage
{
    public required string PokemonName { get; init; }
    public required int HealAmount { get; init; }
    public required int CurrentHp { get; init; }
    public required int MaxHp { get; init; }

    public override string ToDisplayText()
    {
        return $"{PokemonName} healed to {CurrentHp} HP!";
    }
}

/// <summary>
/// Message for item usage
/// </summary>
public class ItemMessage : BattleMessage
{
    public required string PokemonName { get; init; }
    public required string ItemName { get; init; }

    public override string ToDisplayText()
    {
        return $"{PokemonName} used its {ItemName}!";
    }
}

/// <summary>
/// Message for ability activation
/// </summary>
public class AbilityMessage : BattleMessage
{
    public required string PokemonName { get; init; }
    public required string AbilityName { get; init; }
    public string? AdditionalInfo { get; init; }

    public override string ToDisplayText()
    {
        string baseText = $"{PokemonName}'s {AbilityName}!";
        return AdditionalInfo != null ? $"{baseText} {AdditionalInfo}" : baseText;
    }
}