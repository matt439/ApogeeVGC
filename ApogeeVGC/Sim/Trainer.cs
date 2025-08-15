namespace ApogeeVGC.Sim;

public class Trainer
{
    public required string Name { get; init; }
}

public static class TrainerGenerator
{
    public static Trainer GenerateTestTrainer()
    {
        return new Trainer
        {
            Name = "Test Trainer"
        };
    }
}