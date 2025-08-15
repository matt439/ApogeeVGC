using ApogeeVGC.Data;

namespace ApogeeVGC.Sim;

public class Battle
{
    public required Field Field { get; init; }
    public required List<Side> Sides { get; init; }

    private int Damage(Pokemon attacker, Pokemon defender, Move move, bool crit = false)
    {
        // Placeholder for damage calculation logic
        // This should include type effectiveness, stats, and other factors
        return 0; // Return a dummy value for now
    }
}

public static class BattleGenerator
{
    public static Battle GenerateTestBattle(Library library)
    {
        return new Battle
        {
            Field = new Field(),
            Sides =
            [
                SideGenerator.GenerateTestSide(library),
                SideGenerator.GenerateTestSide(library)
            ]
        };
    }
}