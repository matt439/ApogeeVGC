//using ApogeeVGC.Sim.Core;
//using ApogeeVGC.Sim.Effects;

//namespace ApogeeVGC.Sim.PokemonClasses;

///// <summary>
///// Represents the health status information of a Pokemon.
///// Contains both secret (exact) and shared (observable by opponent) health data.
///// </summary>
//public sealed record PokemonHealth
//{
//    public required SideId Side { get; init; }
//    public required HealthData Secret { get; init; }
//    public required HealthData Shared { get; init; }
//}

///// <summary>
///// Base class for health data display formats.
///// Different formats include exact HP, percentage, and pixel-based display.
///// </summary>
//public abstract record HealthData
//{
//    /// <summary>
//    /// The status condition (if any) affecting the Pokemon.
//    /// </summary>
//    public ConditionId? StatusCondition { get; init; }
//}

///// <summary>
///// Represents a fainted Pokemon (0 HP).
///// </summary>
//public sealed record FaintedHealthData : HealthData
//{
//    public static readonly FaintedHealthData Instance = new();

//    private FaintedHealthData() { }

//    public override string ToString() => "0 fnt";
//}

///// <summary>
///// Represents exact HP values (HP/MaxHP format).
///// Used when battle.reportExactHP is true.
///// </summary>
//public sealed record ExactHealthData : HealthData
//{
//    public required int CurrentHp { get; init; }
//    public required int MaxHp { get; init; }

//    public override string ToString()
//    {
//        string result = $"{CurrentHp}/{MaxHp}";
//        if (StatusCondition.HasValue)
//        {
//            result += $" {StatusCondition.Value}";
//        }
//        return result;
//    }
//}

///// <summary>
///// Represents HP as a percentage (used in Gen 7+ or when reportPercentages is true).
///// </summary>
//public sealed record PercentageHealthData : HealthData
//{
//    public required int Percentage { get; init; }

//    public override string ToString()
//    {
//        string result = $"{Percentage}/100";
//        if (StatusCondition.HasValue)
//        {
//            result += $" {StatusCondition.Value}";
//        }
//        return result;
//    }
//}

///// <summary>
///// Represents HP using pixel-based display (used in Gen 3-6).
///// Includes color indicators for certain thresholds in Gen 5+.
///// </summary>
//public sealed record PixelHealthData : HealthData
//{
//    public required int Pixels { get; init; }
//    public PixelColorIndicator? ColorIndicator { get; init; }

//    public override string ToString()
//    {
//        string result = $"{Pixels}/48";

//        if (ColorIndicator.HasValue)
//        {
//            result += ColorIndicator.Value switch
//            {
//                PixelColorIndicator.Yellow => "y",
//                PixelColorIndicator.Red => "r",
//                PixelColorIndicator.Green => "g",
//                _ => string.Empty,
//            };
//        }

//        if (StatusCondition.HasValue)
//        {
//            result += $" {StatusCondition.Value}";
//        }

//        return result;
//    }
//}

///// <summary>
///// Color indicators for pixel-based health display (Gen 5+).
///// Used to indicate HP thresholds at pixel boundaries.
///// </summary>
//public enum PixelColorIndicator
//{
//    /// <summary>Yellow indicator (low HP warning)</summary>
//    Yellow,

//    /// <summary>Red indicator (critical HP)</summary>
//    Red,

//    /// <summary>Green indicator (healthy HP)</summary>
//    Green,
//}
