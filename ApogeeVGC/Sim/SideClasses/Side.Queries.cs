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
        var allies = new List<Pokemon>(Active.Count);
        foreach (var p in Active)
        {
            if (p != null && (all || p.Hp > 0))
                allies.Add(p);
        }
        return allies;
    }

    public List<Pokemon> Foes(bool all = false)
    {
        return Foe.Allies(all);
    }

    public List<Pokemon> ActiveTeam()
    {
        var team = new List<Pokemon>(Active.Count);
        foreach (var p in Active)
        {
            if (p != null) team.Add(p);
        }
        return team;
    }

    public bool HasAlly(Pokemon pokemon)
    {
        return pokemon.Side == this;
    }
}