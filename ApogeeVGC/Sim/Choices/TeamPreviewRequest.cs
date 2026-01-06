namespace ApogeeVGC.Sim.Choices;

public record TeamPreviewRequest : IChoiceRequest
{
    public bool? Wait => null;
    public bool? TeamPreview => true;
    public IReadOnlyList<bool>? ForceSwitch => null;
    public int? MaxChosenTeamSize { get; init; }
    public required SideRequestData Side { get; init; }
    public bool? NoCancel { get; init; }
}