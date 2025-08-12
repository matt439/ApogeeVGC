using ApogeeVGC_CS.sim;
using ApogeeVGC_CS.data;

namespace ApogeeVGC_CS
{
    public class PlayerSpecification
    {
        public required string Name { get; init; }
        public required string Team { get; init; }
    }

    public class Test1V1
    {
        private static List<PokemonSet> Team =>
        [
            new()
            {
                Name = "calyrexice",
                Species = "calyrexice",
                Item = "leftovers",
                Ability = "asoneice",
                Moves = ["glaciallance", "leechseed", "trickroom", "protect"],
                Nature = "adamant",
                Gender = GenderName.N,
                Evs = new StatsTable()
                {
                    Hp = 236,
                    Atk = 36,
                    Spd = 236,
                },
                Level = 50,
            },
            new()
            {
                Name = "miraidon",
                Species = "miraidon",
                Item = "choicespecs",
                Ability = "hadronengine",
                Moves = ["volt-switch", "dazzlinggleam", "electrodrift", "dracometeor"],
                Nature = "modest",
                Gender = GenderName.N,
                Evs = new StatsTable()
                {
                    Hp = 236,
                    Def = 52,
                    Spa = 124,
                    Spd = 68,
                    Spe = 28,
                },
                Level = 50,
            },
            new()
            {
                Name = "ursaluna",
                Species = "ursaluna",
                Item = "flameorb",
                Ability = "guts",
                Moves = ["facade", "crunch", "headlongrush", "protect"],
                Nature = "adamant",
                Gender = GenderName.M,
                Evs = new StatsTable()
                {
                    Hp = 108,
                    Atk = 156,
                    Def = 4,
                    Spd = 116,
                    Spe = 124,
                },
                Level = 50,
            },
            new()
            {
                Name = "volcarona",
                Species = "volcarona",
                Item = "rockyhelmet",
                Ability = "flamebody",
                Moves = ["strugglebug", "overheat", "ragepowder", "tailwind"],
                Nature = "bold",
                Gender = GenderName.M,
                Evs = new StatsTable()
                {
                    Hp = 252,
                    Def = 196,
                    Spd = 60,
                },
                Level = 50,
            },
            new()
            {
                Name = "grimmsnarl",
                Species = "grimmsnarl",
                Item = "lightclay",
                Ability = "prankster",
                Moves = ["spiritbreak", "thunderwave", "reflect", "lightscreen"],
                Nature = "careful",
                Gender = GenderName.M,
                Evs = new StatsTable()
                {
                    Hp = 252,
                    Atk = 4,
                    Def = 140,
                    Spd = 116,
                    Spe = 12,
                },
                Level = 50,
            },
            new()
            {
                Name = "ironhands",
                Species = "ironhands",
                Item = "assaultvest",
                Ability = "quarkdrive",
                Moves = ["fakeout", "heavyslam", "lowkick", "wildcharge"],
                Nature = "adamant",
                Gender = GenderName.N,
                Evs = new StatsTable()
                {
                    Atk = 236,
                    Spd = 236,
                    Spe = 36,
                },
                Level = 50,
            }
        ];
        
        public void RunTest()
        {
            var streams = BattleStreamUtils.GetPlayerStreams(new BattleStream());

            var p1Spec = new PlayerSpecification()
            {
                Name = "Player1",
                Team = Teams.Pack(Team),
            };

            var p2Spec = new PlayerSpecification()
            {
                Name = "Player2",
                Team = Teams.Pack(Team),
            };
        }
    }
}
