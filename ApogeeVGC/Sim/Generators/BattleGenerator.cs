using System.Net.Sockets;
using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.FieldClasses;

namespace ApogeeVGC.Sim.Generators;

public static class BattleGenerator
{
    //public static BattleAsync GenerateTestBattleNew(Library library, IPlayerNew player1,
    //    IPlayerNew player2, string trainerName1, string trainerName2, GameType format,
    //    bool printDebug = false, int? seed = null, CancellationTokenSource? player1TokenSource = null,
    //    CancellationTokenSource? player2TokenSource = null)
    //{
    //    return new BattleAsync
    //    {
    //        Library = library,
    //        Field = new Field(),
    //        Side1 = SideGenerator.GenerateTestSide(library, trainerName1, PlayerId.Player1, Side.Side1, format,
    //            printDebug),
    //        Side2 = SideGenerator.GenerateTestSide(library, trainerName2, PlayerId.Player2, Side.Side2, format,
    //            printDebug),
    //        DisplayUi = printDebug,
    //        BattleSeed = seed,
    //        Format = format,
    //        Player1 = player1,
    //        Player2 = player2,
    //        Player1CancellationTokenSource = player1TokenSource ?? new CancellationTokenSource(),
    //        Player2CancellationTokenSource = player2TokenSource ?? new CancellationTokenSource(),
    //    };
    //}

    public static IBattle GenerateTestBattleAsync()
    {
        throw new NotImplementedException();
    }
}

