using ApogeeVGC_CS.sim;

namespace ApogeeVGC_CS.data
{
    public static class Natures
    {
        public static NatureDataTable NatureData { get; } = new()
        {
            [new IdEntry("adamant")] = new NatureData
            {
                Name = "Adamant",
                Plus = StatIdExceptHp.Atk,
                Minus = StatIdExceptHp.Spa,
            },
            [new IdEntry("bashful")] = new NatureData
            {
                Name = "Bashful"
            },
            [new IdEntry("bold")] = new NatureData
            {
                Name = "Bold",
                Plus = StatIdExceptHp.Def,
                Minus = StatIdExceptHp.Atk
            },
            [new IdEntry("brave")] = new NatureData
            {
                Name = "Brave",
                Plus = StatIdExceptHp.Atk,
                Minus = StatIdExceptHp.Spe
            },
            [new IdEntry("calm")] = new NatureData
            {
                Name = "Calm",
                Plus = StatIdExceptHp.Spd,
                Minus = StatIdExceptHp.Atk
            },
            [new IdEntry("careful")] = new NatureData
            {
                Name = "Careful",
                Plus = StatIdExceptHp.Spd,
                Minus = StatIdExceptHp.Spa
            },
            [new IdEntry("docile")] = new NatureData
            {
                Name = "Docile"
            },
            [new IdEntry("gentle")] = new NatureData
            {
                Name = "Gentle",
                Plus = StatIdExceptHp.Spd,
                Minus = StatIdExceptHp.Def
            },
            [new IdEntry("hardy")] = new NatureData
            {
                Name = "Hardy"
            },
            [new IdEntry("hasty")] = new NatureData
            {
                Name = "Hasty",
                Plus = StatIdExceptHp.Spe,
                Minus = StatIdExceptHp.Def
            },
            [new IdEntry("impish")] = new NatureData
            {
                Name = "Impish",
                Plus = StatIdExceptHp.Def,
                Minus = StatIdExceptHp.Spa
            },
            [new IdEntry("jolly")] = new NatureData
            {
                Name = "Jolly",
                Plus = StatIdExceptHp.Spe,
                Minus = StatIdExceptHp.Spa
            },
            [new IdEntry("lax")] = new NatureData
            {
                Name = "Lax",
                Plus = StatIdExceptHp.Def,
                Minus = StatIdExceptHp.Spd
            },
            [new IdEntry("lonely")] = new NatureData
            {
                Name = "Lonely",
                Plus = StatIdExceptHp.Atk,
                Minus = StatIdExceptHp.Def
            },
            [new IdEntry("mild")] = new NatureData
            {
                Name = "Mild",
                Plus = StatIdExceptHp.Spa,
                Minus = StatIdExceptHp.Def
            },
            [new IdEntry("modest")] = new NatureData
            {
                Name = "Modest",
                Plus = StatIdExceptHp.Spa,
                Minus = StatIdExceptHp.Atk
            },
            [new IdEntry("naive")] = new NatureData
            {
                Name = "Naive",
                Plus = StatIdExceptHp.Spe,
                Minus = StatIdExceptHp.Spd
            },
            [new IdEntry("naughty")] = new NatureData
            {
                Name = "Naughty",
                Plus = StatIdExceptHp.Atk,
                Minus = StatIdExceptHp.Spd
            },
            [new IdEntry("quiet")] = new NatureData
            {
                Name = "Quiet",
                Plus = StatIdExceptHp.Spa,
                Minus = StatIdExceptHp.Spe
            },
            [new IdEntry("quirky")] = new NatureData
            {
                Name = "Quirky"
            },
            [new IdEntry("rash")] = new NatureData
            {
                Name = "Rash",
                Plus = StatIdExceptHp.Spa,
                Minus = StatIdExceptHp.Spd
            },
            [new IdEntry("relaxed")] = new NatureData
            {
                Name = "Relaxed",
                Plus = StatIdExceptHp.Def,
                Minus = StatIdExceptHp.Spe
            },
            [new IdEntry("sassy")] = new NatureData
            {
                Name = "Sassy",
                Plus = StatIdExceptHp.Spd,
                Minus = StatIdExceptHp.Spe
            },
            [new IdEntry("serious")] = new NatureData
            {
                Name = "Serious"
            },
            [new IdEntry("timid")] = new NatureData
            {
                Name = "Timid",
                Plus = StatIdExceptHp.Spe,
                Minus = StatIdExceptHp.Atk
            },
        };
    }
}
