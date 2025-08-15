using ApogeeVGC.Sim;

namespace ApogeeVGC.Data;


public class Natures
{
    public static Dictionary<NatureType, NatureData> NatureData { get; } = new()
    {
        [NatureType.Adamant] = new NatureData
        {
            Plus = StatTypeExceptHp.Atk,
            Minus = StatTypeExceptHp.SpA,
        },
        [NatureType.Bashful] = new NatureData(),
        [NatureType.Bold] = new NatureData
        {
            Plus = StatTypeExceptHp.Def,
            Minus = StatTypeExceptHp.Atk,
        },
        [NatureType.Brave] = new NatureData
        {
            Plus = StatTypeExceptHp.Atk,
            Minus = StatTypeExceptHp.Spe,
        },
        [NatureType.Calm] = new NatureData
        {
            Plus = StatTypeExceptHp.SpD,
            Minus = StatTypeExceptHp.Atk,
        },
        [NatureType.Careful] = new NatureData
        {
            Plus = StatTypeExceptHp.SpD,
            Minus = StatTypeExceptHp.SpA,
        },
        [NatureType.Docile] = new NatureData(),
        [NatureType.Gentle] = new NatureData
        {
            Plus = StatTypeExceptHp.SpD,
            Minus = StatTypeExceptHp.Def,
        },
        [NatureType.Hardy] = new NatureData(),
        [NatureType.Hasty] = new NatureData
        {
            Plus = StatTypeExceptHp.Spe,
            Minus = StatTypeExceptHp.Def,
        },
        [NatureType.Impish] = new NatureData
        {
            Plus = StatTypeExceptHp.Def,
            Minus = StatTypeExceptHp.SpA,
        },
        [NatureType.Jolly] = new NatureData
        {
            Plus = StatTypeExceptHp.Spe,
            Minus = StatTypeExceptHp.SpA,
        },
        [NatureType.Lax] = new NatureData
        {
            Plus = StatTypeExceptHp.Def,
            Minus = StatTypeExceptHp.SpD,
        },
        [NatureType.Lonely] = new NatureData
        {
            Plus = StatTypeExceptHp.Atk,
            Minus = StatTypeExceptHp.Def,
        },
        [NatureType.Mild] = new NatureData
        {
            Plus = StatTypeExceptHp.SpA,
            Minus = StatTypeExceptHp.Def,
        },
        [NatureType.Modest] = new NatureData
        {
            Plus = StatTypeExceptHp.SpA,
            Minus = StatTypeExceptHp.Atk,
        },
        [NatureType.Naive] = new NatureData
        {
            Plus = StatTypeExceptHp.Spe,
            Minus = StatTypeExceptHp.SpD,
        },
        [NatureType.Naughty] = new NatureData
        {
            Plus = StatTypeExceptHp.Atk,
            Minus = StatTypeExceptHp.SpD,
        },
        [NatureType.Quiet] = new NatureData
        {
            Plus = StatTypeExceptHp.SpA,
            Minus = StatTypeExceptHp.Spe,
        },
        [NatureType.Quirky] = new NatureData(),
        [NatureType.Rash] = new NatureData
        {
            Plus = StatTypeExceptHp.SpA,
            Minus = StatTypeExceptHp.SpD,
        },
        [NatureType.Relaxed] = new NatureData
        {
            Plus = StatTypeExceptHp.Def,
            Minus = StatTypeExceptHp.Spe,
        },
        [NatureType.Sassy] = new NatureData
        {
            Plus = StatTypeExceptHp.SpD,
            Minus = StatTypeExceptHp.Spe,
        },
        [NatureType.Serious] = new NatureData(),
        [NatureType.Timid] = new NatureData
        {
            Plus = StatTypeExceptHp.Spe,
            Minus = StatTypeExceptHp.Atk,
        },
    };
}