namespace ApogeeVGC.Sim.Choices;

public record WaitRequest : IChoiceRequest
{
    public bool? Wait => true;
    public bool? TeamPreview => null;
    public IReadOnlyList<bool>? ForceSwitch => null;
    public required SideRequestData Side { get; init; }
    public bool? NoCancel { get; init; }
}