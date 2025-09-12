namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleNew
{
    private static int RoundedDownAtHalf(double value)
    {
        return (int)(value + 0.5 - double.Epsilon);
    }

    private static int RoundedUpAtHald(double value)
    {
        return (int)(value + 0.5 + double.Epsilon);
    }
}