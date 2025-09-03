using ApogeeVGC.Data;

namespace ApogeeVGC.Sim;

public static class TeamGenerator
{
    public static Team GenerateTestTeam(Library library, string trainerName, bool printDebug = false)
    {
        return new Team
        {
            Trainer = TrainerGenerator.GenerateTestTrainer(trainerName, printDebug),
            PokemonSet = PokemonBuilder.BuildTestSet(library, printDebug),
            PrintDebug = printDebug,
        };
    }
}