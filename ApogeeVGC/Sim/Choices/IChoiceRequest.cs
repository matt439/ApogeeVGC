namespace ApogeeVGC.Sim.Choices;

public interface IChoiceRequest
{
    public bool? Wait { get; }
    public SideRequestData Side { get; }
    public bool? NoCancel { get; }
    public bool? TeamPreview { get; }
    public IReadOnlyList<bool>? ForceSwitch { get; }
}