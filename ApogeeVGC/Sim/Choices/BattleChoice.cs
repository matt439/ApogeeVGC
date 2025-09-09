using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.Choices;

/// <summary>
/// Base class for all battle choices, whether single-slot or multi-slot.
/// Represents any action a trainer can take during a battle turn.
/// </summary>
public abstract record BattleChoice
{
    /// <summary>
    /// Gets the trainer making this choice.
    /// </summary>
    public abstract Trainer Trainer { get; }

    /// <summary>
    /// Gets the side ID for this choice.
    /// </summary>
    public abstract SideId SideId { get; }

    /// <summary>
    /// Gets whether this choice affects a single slot.
    /// </summary>
    public virtual bool IsSingleSlotChoice => this is SlotChoice;

    /// <summary>
    /// Gets whether this choice affects multiple slots.
    /// </summary>
    public virtual bool IsMultiSlotChoice => this is DoubleSlotChoice;

    public virtual bool IsTeamPreviewChoice => this is TeamPreviewChoice;
}