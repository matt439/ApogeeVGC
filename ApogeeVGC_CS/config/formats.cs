using ApogeeVGC_CS.sim;

namespace ApogeeVGC_CS.config
{
    public static class Formats
    {
        public static FormatList FormatsList { get; } =
        [
            new FormatData
            {
                Name = "ApogeeVGC 1v1",
                Mod = "gen9",

                Fullname = null,
                EffectType = EffectType.Condition,
                Exists = false,
                Num = 0,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = null,
                FormatEffectType = FormatEffectType.Format,
                Debug = false,
                Rated = null,
                GameType = GameType.Singles,
                Ruleset = null,
                BaseRuleset = null,
                Banlist = null,
                Restricted = null,
                Unbanlist = null,
                NoLog = false,
            }
        ];
    }
}
