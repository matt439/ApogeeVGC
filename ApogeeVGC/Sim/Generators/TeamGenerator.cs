//using ApogeeVGC.Data;
//using ApogeeVGC.Sim.Core;
//using ApogeeVGC.Sim.GameObjects;
//using ApogeeVGC.Sim.PokemonClasses;

//namespace ApogeeVGC.Sim.Generators;

//public static class TeamGenerator
//{
//    public static Team GenerateTestTeam(Library library, string trainerName, SideId sideId, bool printDebug = false)
//    {
//        Trainer trainer = TrainerGenerator.GenerateTestTrainer(trainerName, printDebug);

//        return new Team
//        {
//            Trainer = trainer,
//            PokemonSet = PokemonBuilder.BuildTestSet(library, trainer, sideId, printDebug),
//            PrintDebug = printDebug,
//            SideId = sideId,
//        };
//    }
//}