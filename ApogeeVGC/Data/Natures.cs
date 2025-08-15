using ApogeeVGC.Sim;
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
            Plus = StatIdExceptHp.Atk,
            Minus = StatIdExceptHp.SpA,
        },
        [NatureType.Bashful] = new Nature(),
        [NatureType.Bold] = new Nature
        {
            Plus = StatIdExceptHp.Def,
            Minus = StatIdExceptHp.Atk,
        },
        [NatureType.Brave] = new Nature
        {
            Plus = StatIdExceptHp.Atk,
            Minus = StatIdExceptHp.Spe,
        },
        [NatureType.Calm] = new Nature
        {
            Plus = StatIdExceptHp.SpD,
            Minus = StatIdExceptHp.Atk,
        },
        [NatureType.Careful] = new Nature
        {
            Plus = StatIdExceptHp.SpD,
            Minus = StatIdExceptHp.SpA,
        },
        [NatureType.Docile] = new Nature(),
        [NatureType.Gentle] = new Nature
        {
            Plus = StatIdExceptHp.SpD,
            Minus = StatIdExceptHp.Def,
        },
        [NatureType.Hardy] = new Nature(),
        [NatureType.Hasty] = new Nature
        {
            Plus = StatIdExceptHp.Spe,
            Minus = StatIdExceptHp.Def,
        },
        [NatureType.Impish] = new Nature
        {
            Plus = StatIdExceptHp.Def,
            Minus = StatIdExceptHp.SpA,
        },
        [NatureType.Jolly] = new Nature
        {
            Plus = StatIdExceptHp.Spe,
            Minus = StatIdExceptHp.SpA,
        },
        [NatureType.Lax] = new Nature
        {
            Plus = StatIdExceptHp.Def,
            Minus = StatIdExceptHp.SpD,
        },
        [NatureType.Lonely] = new Nature
        {
            Plus = StatIdExceptHp.Atk,
            Minus = StatIdExceptHp.Def,
        },
        [NatureType.Mild] = new Nature
        {
            Plus = StatIdExceptHp.SpA,
            Minus = StatIdExceptHp.Def,
        },
        [NatureType.Modest] = new Nature
        {
            Plus = StatIdExceptHp.SpA,
            Minus = StatIdExceptHp.Atk,
        },
        [NatureType.Naive] = new Nature
        {
            Plus = StatIdExceptHp.Spe,
            Minus = StatIdExceptHp.SpD,
        },
        [NatureType.Naughty] = new Nature
        {
            Plus = StatIdExceptHp.Atk,
            Minus = StatIdExceptHp.SpD,
        },
        [NatureType.Quiet] = new Nature
        {
            Plus = StatIdExceptHp.SpA,
            Minus = StatIdExceptHp.Spe,
        },
        [NatureType.Quirky] = new Nature(),
        [NatureType.Rash] = new Nature
        {
            Plus = StatIdExceptHp.SpA,
            Minus = StatIdExceptHp.SpD,
        },
        [NatureType.Relaxed] = new Nature
        {
            Plus = StatIdExceptHp.Def,
            Minus = StatIdExceptHp.Spe,
        },
        [NatureType.Sassy] = new Nature
        {
            Plus = StatIdExceptHp.SpD,
            Minus = StatIdExceptHp.Spe,
        },
        [NatureType.Serious] = new Nature(),
        [NatureType.Timid] = new Nature
        {
            Plus = StatIdExceptHp.Spe,
            Minus = StatIdExceptHp.Atk,
        },
    };
}