namespace ApogeeVGC.Sim;

public record Trainer
{
    public required string Name { get; init; }
    public bool PrintDebug { get; init; }
}

public static class TrainerGenerator
{
    public static Trainer GenerateTestTrainer(string name, bool printDebug = false)
    {
        return new Trainer
        {
            Name = name,
            PrintDebug = printDebug,
        };
    }
}