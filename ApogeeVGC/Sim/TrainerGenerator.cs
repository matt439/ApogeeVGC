namespace ApogeeVGC.Sim;

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