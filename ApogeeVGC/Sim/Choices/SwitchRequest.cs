namespace ApogeeVGC.Sim.Choices;

public record SwitchRequest : IChoiceRequest
{
    public bool? Wait => null;
    public bool? TeamPreview => null;
    public required IReadOnlyList<bool> ForceSwitch { get; init; }
    public required SideRequestData Side { get; init; }
    public bool? NoCancel { get; init; }
    public bool? Update { get; set; }
}