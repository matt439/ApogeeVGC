namespace ApogeeVGC.Sim.Choices;

public record MoveRequest : IChoiceRequest
{
    public bool? Wait => null;
    public bool? TeamPreview => null;
    public IReadOnlyList<bool>? ForceSwitch => null;
    /// <summary>
    /// Move request data for each active Pokemon slot.
    /// Elements can be null for fainted Pokemon or empty slots.
    /// The index matches the Side.Active index to preserve slot positions.
    /// </summary>
    public required IReadOnlyList<PokemonMoveRequestData?> Active { get; init; }
    public required SideRequestData Side { get; init; }
    public SideRequestData? Ally { get; init; }
    public bool? NoCancel { get; init; }
    public bool? Update { get; set; }
}