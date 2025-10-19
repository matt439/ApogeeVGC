using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Stats;
using System.Collections.ObjectModel;

namespace ApogeeVGC.Data;


public record Natures
{
    public IReadOnlyDictionary<NatureId, Nature> NatureData { get; }

    public Natures()
    {
        NatureData = new ReadOnlyDictionary<NatureId, Nature>(_natureData);
    }

    private readonly Dictionary<NatureId, Nature> _natureData = new()
    {
        [NatureId.Adamant] = new Nature
        {
            Id = NatureId.Adamant,
            Plus = StatIdExceptHp.Atk,
            Minus = StatIdExceptHp.SpA,
        },
        [NatureId.Bashful] = new Nature
        {
            Id = NatureId.Bashful,
        },
        [NatureId.Bold] = new Nature
        {
            Id = NatureId.Bold,
            Plus = StatIdExceptHp.Def,
            Minus = StatIdExceptHp.Atk,
        },
        [NatureId.Brave] = new Nature
        {
            Id = NatureId.Brave,
            Plus = StatIdExceptHp.Atk,
            Minus = StatIdExceptHp.Spe,
        },
        [NatureId.Calm] = new Nature
        {
            Id = NatureId.Calm,
            Plus = StatIdExceptHp.SpD,
            Minus = StatIdExceptHp.Atk,
        },
        [NatureId.Careful] = new Nature
        {
            Id = NatureId.Careful,
            Plus = StatIdExceptHp.SpD,
            Minus = StatIdExceptHp.SpA,
        },
        [NatureId.Docile] = new Nature
        {
            Id = NatureId.Docile,
        },
        [NatureId.Gentle] = new Nature
        {
            Id = NatureId.Gentle,
            Plus = StatIdExceptHp.SpD,
            Minus = StatIdExceptHp.Def,
        },
        [NatureId.Hardy] = new Nature
        {
            Id = NatureId.Hardy,
        },
        [NatureId.Hasty] = new Nature
        {
            Id = NatureId.Hasty,
            Plus = StatIdExceptHp.Spe,
            Minus = StatIdExceptHp.Def,
        },
        [NatureId.Impish] = new Nature
        {
            Id = NatureId.Impish,
            Plus = StatIdExceptHp.Def,
            Minus = StatIdExceptHp.SpA,
        },
        [NatureId.Jolly] = new Nature
        {
            Id = NatureId.Jolly,
            Plus = StatIdExceptHp.Spe,
            Minus = StatIdExceptHp.SpA,
        },
        [NatureId.Lax] = new Nature
        {
            Id = NatureId.Lax,
            Plus = StatIdExceptHp.Def,
            Minus = StatIdExceptHp.SpD,
        },
        [NatureId.Lonely] = new Nature
        {
            Id = NatureId.Lonely,
            Plus = StatIdExceptHp.Atk,
            Minus = StatIdExceptHp.Def,
        },
        [NatureId.Mild] = new Nature
        {
            Id = NatureId.Mild,
            Plus = StatIdExceptHp.SpA,
            Minus = StatIdExceptHp.Def,
        },
        [NatureId.Modest] = new Nature
        {
            Id = NatureId.Modest,
            Plus = StatIdExceptHp.SpA,
            Minus = StatIdExceptHp.Atk,
        },
        [NatureId.Naive] = new Nature
        {
            Id = NatureId.Naive,
            Plus = StatIdExceptHp.Spe,
            Minus = StatIdExceptHp.SpD,
        },
        [NatureId.Naughty] = new Nature
        {
            Id = NatureId.Naughty,
            Plus = StatIdExceptHp.Atk,
            Minus = StatIdExceptHp.SpD,
        },
        [NatureId.Quiet] = new Nature
        {
            Id = NatureId.Quiet,
            Plus = StatIdExceptHp.SpA,
            Minus = StatIdExceptHp.Spe,
        },
        [NatureId.Quirky] = new Nature
        {
            Id = NatureId.Quirky,
        },
        [NatureId.Rash] = new Nature
        {
            Id = NatureId.Rash,
            Plus = StatIdExceptHp.SpA,
            Minus = StatIdExceptHp.SpD,
        },
        [NatureId.Relaxed] = new Nature
        {
            Id = NatureId.Relaxed,
            Plus = StatIdExceptHp.Def,
            Minus = StatIdExceptHp.Spe,
        },
        [NatureId.Sassy] = new Nature
        {
            Id = NatureId.Sassy,
            Plus = StatIdExceptHp.SpD,
            Minus = StatIdExceptHp.Spe,
        },
        [NatureId.Serious] = new Nature
        {
            Id = NatureId.Serious,
        },
        [NatureId.Timid] = new Nature
        {
            Id = NatureId.Timid,
            Plus = StatIdExceptHp.Spe,
            Minus = StatIdExceptHp.Atk,
        },
    };
}