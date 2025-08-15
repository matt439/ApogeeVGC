using ApogeeVGC.Data;

namespace ApogeeVGC.Sim;

public class Team
{
    public required Trainer Trainer { get; init; }
    public required PokemonSet PokemonSet { get; init; }
}

public static class TeamGenerator
{
    public static Team GenerateTestTeam(Library library)
    {
        return new Team
        {
            Trainer = TrainerGenerator.GenerateTestTrainer(),
            PokemonSet = PokemonBuilder.BuildTestSet(library)
        };
    }
}