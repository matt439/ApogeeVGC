using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.SideClasses;

public partial class Side
{
    public Pokemon? RandomFoe()
    {
        var actives = Foes();
        return actives.Count == 0 ? null : Battle.Sample(actives);
    }

    public List<Side> FoeSidesWithConditions()
    {
        return [Foe];
    }

    public int FoePokemonLeft()
    {
        return Foe.PokemonLeft;
    }

    public List<Pokemon> Allies(bool all = false)
    {
        var allies = Active.Where(p => p != null).Select(p => p!).ToList();
        if (!all) allies = allies.Where(ally => ally.Hp > 0).ToList();
        return allies;
    }

    public List<Pokemon> Foes(bool all = false)
    {
        return Foe.Allies(all);
    }

    public List<Pokemon> ActiveTeam()
    {
        return Battle.Sides[N % 2].Active.Concat(Battle.Sides[N % 2 + 2].Active)
            .Where(p => p != null)
            .Select(p => p!)
            .ToList();
    }

    public bool HasAlly(Pokemon pokemon)
    {
        return pokemon.Side == this;
    }
}