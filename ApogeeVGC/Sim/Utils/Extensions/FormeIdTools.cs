using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class FormeIdTools
{
    public static bool IsGen8Forme(this FormeId forme)
    {
        string formeName = forme.ToString();
        return formeName.Contains("Gmax") ||
               formeName.Contains("Galar") ||
               formeName.Contains("Galar-Zen") ||
               formeName.Contains("Hisui");
    }

    public static bool IsGen7Forme(this FormeId forme)
    {
        string formeName = forme.ToString();
        return formeName.StartsWith("Alola") || formeName == "Starter";
    }

    public static bool IsPrimalForme(this FormeId forme)
    {
        return forme.ToString() == "Primal";
    }

    public static bool IsMegaForme(this FormeId forme)
    {
        string formeName = forme.ToString();
        return formeName.Contains("Mega");
    }
}