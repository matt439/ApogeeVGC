namespace ApogeeVGC.Sim;

public record Trainer
{
    public required string Name { get; init; }
    public bool PrintDebug { get; init; }
}