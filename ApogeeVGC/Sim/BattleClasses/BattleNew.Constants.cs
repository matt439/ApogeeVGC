namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleNew
{
    private const int TeamPreviewLimitSeconds = 900; // TODO: Change back to 90 seconds
    private const int StandardTurnLimitSeconds = 4500; // TODO: Change back to 45 seconds
    private const int PlayerTotalTimeLimitMinutes = 70; // TODO: Change back to 7 minutes
    private const int GameTotalTimeLimitMinutes = 200; // TODO: Change back to 20 minutes
    private const int TimeoutWarningThresholdSeconds = 10;
    private const int TurnLimit = 1000;
    private const double Epsilon = 1e-10;
}