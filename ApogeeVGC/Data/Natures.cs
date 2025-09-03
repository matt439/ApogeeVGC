using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Stats;
using System.Collections.ObjectModel;

namespace ApogeeVGC.Data;


public record Natures
{
    public IReadOnlyDictionary<NatureType, Nature> NatureData { get; }

    public Natures()
    {
        NatureData = new ReadOnlyDictionary<NatureType, Nature>(_natureData);
    }

    private readonly Dictionary<NatureType, Nature> _natureData = new()
    {
        [NatureType.Adamant] = new Nature
        {
            Type = NatureType.Adamant,
            Plus = StatIdExceptHp.Atk,
            Minus = StatIdExceptHp.SpA,
        },
        [NatureType.Bashful] = new Nature
        {
            Type = NatureType.Bashful,
        },
        [NatureType.Bold] = new Nature
        {
            Type = NatureType.Bold,
            Plus = StatIdExceptHp.Def,
            Minus = StatIdExceptHp.Atk,
        },
        [NatureType.Brave] = new Nature
        {
            Type = NatureType.Brave,
            Plus = StatIdExceptHp.Atk,
            Minus = StatIdExceptHp.Spe,
        },
        [NatureType.Calm] = new Nature
        {
            Type = NatureType.Calm,
            Plus = StatIdExceptHp.SpD,
            Minus = StatIdExceptHp.Atk,
        },
        [NatureType.Careful] = new Nature
        {
            Type = NatureType.Careful,
            Plus = StatIdExceptHp.SpD,
            Minus = StatIdExceptHp.SpA,
        },
        [NatureType.Docile] = new Nature
        {
            Type = NatureType.Docile,
        },
        [NatureType.Gentle] = new Nature
        {
            Type = NatureType.Gentle,
            Plus = StatIdExceptHp.SpD,
            Minus = StatIdExceptHp.Def,
        },
        [NatureType.Hardy] = new Nature
        {
            Type = NatureType.Hardy,
        },
        [NatureType.Hasty] = new Nature
        {
            Type = NatureType.Hasty,
            Plus = StatIdExceptHp.Spe,
            Minus = StatIdExceptHp.Def,
        },
        [NatureType.Impish] = new Nature
        {
            Type = NatureType.Impish,
            Plus = StatIdExceptHp.Def,
            Minus = StatIdExceptHp.SpA,
        },
        [NatureType.Jolly] = new Nature
        {
            Type = NatureType.Jolly,
            Plus = StatIdExceptHp.Spe,
            Minus = StatIdExceptHp.SpA,
        },
        [NatureType.Lax] = new Nature
        {
            Type = NatureType.Lax,
            Plus = StatIdExceptHp.Def,
            Minus = StatIdExceptHp.SpD,
        },
        [NatureType.Lonely] = new Nature
        {
            Type = NatureType.Lonely,
            Plus = StatIdExceptHp.Atk,
            Minus = StatIdExceptHp.Def,
        },
        [NatureType.Mild] = new Nature
        {
            Type = NatureType.Mild,
            Plus = StatIdExceptHp.SpA,
            Minus = StatIdExceptHp.Def,
        },
        [NatureType.Modest] = new Nature
        {
            Type = NatureType.Modest,
            Plus = StatIdExceptHp.SpA,
            Minus = StatIdExceptHp.Atk,
        },
        [NatureType.Naive] = new Nature
        {
            Type = NatureType.Naive,
            Plus = StatIdExceptHp.Spe,
            Minus = StatIdExceptHp.SpD,
        },
        [NatureType.Naughty] = new Nature
        {
            Type = NatureType.Naughty,
            Plus = StatIdExceptHp.Atk,
            Minus = StatIdExceptHp.SpD,
        },
        [NatureType.Quiet] = new Nature
        {
            Type = NatureType.Quiet,
            Plus = StatIdExceptHp.SpA,
            Minus = StatIdExceptHp.Spe,
        },
        [NatureType.Quirky] = new Nature
        {
            Type = NatureType.Quirky,
        },
        [NatureType.Rash] = new Nature
        {
            Type = NatureType.Rash,
            Plus = StatIdExceptHp.SpA,
            Minus = StatIdExceptHp.SpD,
        },
        [NatureType.Relaxed] = new Nature
        {
            Type = NatureType.Relaxed,
            Plus = StatIdExceptHp.Def,
            Minus = StatIdExceptHp.Spe,
        },
        [NatureType.Sassy] = new Nature
        {
            Type = NatureType.Sassy,
            Plus = StatIdExceptHp.SpD,
            Minus = StatIdExceptHp.Spe,
        },
        [NatureType.Serious] = new Nature
        {
            Type = NatureType.Serious,
        },
        [NatureType.Timid] = new Nature
        {
            Type = NatureType.Timid,
            Plus = StatIdExceptHp.Spe,
            Minus = StatIdExceptHp.Atk,
        },
    };
}