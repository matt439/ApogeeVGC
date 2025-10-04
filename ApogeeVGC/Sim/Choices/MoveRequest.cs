namespace ApogeeVGC.Sim.Choices;

public record MoveRequest : IChoiceRequest
{
    public bool? Wait => null;
    public bool? TeamPreview => null;
    public IReadOnlyList<bool>? ForceSwitch => null;
    public required IReadOnlyList<PokemonMoveRequestData> Active { get; init; }
    public required SideRequestData Side { get; init; }
    public SideRequestData? Ally { get; init; }
    public bool? NoCancel { get; init; }
    public bool? Update { get; set; }
}