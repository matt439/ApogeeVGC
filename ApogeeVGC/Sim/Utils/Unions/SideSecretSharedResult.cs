using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Represents the result of a side-specific content generation function.
/// Maps to TypeScript: { side: SideID, secret: string, shared: string }
/// </summary>
public record SideSecretSharedResult(SideId Side, Secret Secret, Shared Shared)
{
 /// <summary>
    /// Optional HP color indicator (used for opponent Pokemon HP display)
    /// </summary>
    public HpColor? HpColor { get; init; }
}
