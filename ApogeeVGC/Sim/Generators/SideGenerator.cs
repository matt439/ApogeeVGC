//using ApogeeVGC.Data;
//using ApogeeVGC.Player;
//using ApogeeVGC.Sim.Core;
//using ApogeeVGC.Sim.GameObjects;
//using ApogeeVGC.Sim.PokemonClasses;

//namespace ApogeeVGC.Sim.Generators;

//public static class SideGenerator
//{
//    public static Side GenerateTestSide(Library library, string trainerName, PlayerId playerId, SideId sideId,
//        BattleFormat format, bool printDebug = false)
//    {
//        Trainer defaulTrainer = TrainerGenerator.GenerateTestTrainer("Default", printDebug);

//        return new Side
//        {
//            PlayerId = playerId,
//            Team = TeamGenerator.GenerateTestTeam(library, trainerName, sideId, printDebug),
//            PrintDebug = printDebug,
//            SideId = sideId,
//            Slot1 = PokemonBuilder.BuildDefaultPokemon(library, defaulTrainer, sideId),
//            Slot2 = PokemonBuilder.BuildDefaultPokemon(library, defaulTrainer, sideId),
//            Slot3 = PokemonBuilder.BuildDefaultPokemon(library, defaulTrainer, sideId),
//            Slot4 = PokemonBuilder.BuildDefaultPokemon(library, defaulTrainer, sideId),
//            Slot5 = PokemonBuilder.BuildDefaultPokemon(library, defaulTrainer, sideId),
//            Slot6 = PokemonBuilder.BuildDefaultPokemon(library, defaulTrainer, sideId),
//            BattleFormat = format,

//        };
//    }
//}