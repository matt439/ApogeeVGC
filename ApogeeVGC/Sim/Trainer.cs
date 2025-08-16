namespace ApogeeVGC.Sim;

public class Trainer
{
    public required string Name { get; init; }
}

public static class TrainerGenerator
{
    public static Trainer GenerateTestTrainer(string name)
    {
        return new Trainer
        {
            Name = name
        };
    }
}