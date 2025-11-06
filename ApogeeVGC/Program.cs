using ApogeeVGC.Sim.Core;
using ApogeeVGC.Gui;

namespace ApogeeVGC;

public class Program
{
    public static void Main(string[] args)
    {
        var driver = new Driver();
        driver.Start(DriverMode.GuiVsRandomSingles);
    }
}