using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.SideClasses;

public partial class Side
{
    public Pokemon? RandomFoe()
    {
        var actives = Foes();
        if (actives.Count == 0) return null;
        int index = Battle.Prng.Random(actives.Count);
        return actives[index];
    }

    public List<Side> FoeSidesWithConditions()
    {
        return [Foe];
    }

    public int FoePokemonLeft()
    {
        return Foe.PokemonLeft;
    }

    public PokemonBuffer Allies(bool all = false)
    {
        var allies = new PokemonBuffer();
        foreach (var p in Active)
        {
            if (p != null && (all || p.Hp > 0))
                allies.Add(p);
        }
        return allies;
    }

    public PokemonBuffer Foes(bool all = false)
    {
        return Foe.Allies(all);
    }

    public PokemonBuffer ActiveTeam()
    {
        var team = new PokemonBuffer();
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