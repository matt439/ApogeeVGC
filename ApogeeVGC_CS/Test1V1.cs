using ApogeeVGC_CS.lib;
using ApogeeVGC_CS.sim;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApogeeVGC_CS
{
    public class PlayerSpecification
    {
        public required string Name { get; init; }
        public required string Team { get; init; }
    }

    public class Test1V1
    {
        public Dex Dex { get; } = new();
        
        // Cache JsonSerializerOptions to fix CA1869
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

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

        public async Task RunTest()
        {
            PlayerStreams streams = BattleStreamUtils.GetPlayerStreams(new BattleStream(Dex));

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

            var battleSpec = new BattleSpecification
            {
                FormatId = "apogeevgc1v1",
                Seed = [1, 2, 3, 4], // Use a simple array instead of Prng.GenerateSeed().ToArray()
                Rated = false,
                Debug = true
            };

            var p1 = new RandomPlayerAi(streams.Player1, new RandomPlayerAiOptions
            {
                Move = 1,
                Mega = 0,
                Seed = new Prng(Prng.GenerateSeed())
            });

            var p2 = new RandomPlayerAi(streams.Player2, new RandomPlayerAiOptions
            {
                Move = 1,
                Mega = 0,
                Seed = new Prng(Prng.GenerateSeed())
            });

            // Wait for battle initialization to complete
            await InitializeBattleAsync(streams, battleSpec, p1Spec, p2Spec);

            Console.WriteLine("Battle initialized, starting players...");

            // Start both players asynchronously
            Task p1Task = p1.Start();
            Task p2Task = p2.Start();

            // Create a task that signals completion when battle ends
            var battleCompletionSource = new TaskCompletionSource<bool>();

            Task.Run(async () => {
                try
                {
                    await foreach (string chunk in streams.Omniscient.ReadAllChunksAsync())
                    {
                        Console.WriteLine($"Received: {chunk}");

                        // Signal battle completion when you see an end condition
                        if (!chunk.Contains("|win|") && !chunk.Contains("|tie|")) continue;
                        Console.WriteLine("Battle completed!");
                        battleCompletionSource.SetResult(true);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    battleCompletionSource.SetException(ex);
                }
            });

            // Wait for battle to complete with a generous timeout
            if (!battleCompletionSource.Task.Wait(TimeSpan.FromMinutes(2)))
            {
                Console.WriteLine("Battle timed out after 2 minutes");
            }

            // Wait for both players to finish
            await Task.WhenAll(p1Task, p2Task);

            Console.WriteLine("Test completed. Press Enter to exit.");
            //Console.ReadLine();
        }

        // Make initialization awaitable
        private static async Task InitializeBattleAsync(PlayerStreams streams,
            BattleSpecification battleSpec, PlayerSpecification p1Spec, PlayerSpecification p2Spec)
        {
            try
            {
                string battleJson = JsonSerializer.Serialize(battleSpec, JsonOptions);
                string p1Json = JsonSerializer.Serialize(p1Spec, JsonOptions);
                string p2Json = JsonSerializer.Serialize(p2Spec, JsonOptions);

                string[] commands =
                [
                    $">start {battleJson}",
                    $">player p1 {p1Json}",
                    $">player p2 {p2Json}"
                ];

                string fullCommand = string.Join("\n", commands) + "\n";
                Console.WriteLine("Sending initialization commands...");

                await streams.Omniscient.WriteAsync(fullCommand);
                Console.WriteLine("Commands sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing battle: {ex.Message}");
                throw;
            }
        }
    }
    public class BattleSpecification
    {
        [JsonPropertyName("formatid")]
        public string FormatId { get; set; } = "gen9vgc2024regh";

        [JsonPropertyName("seed")]
        public int[] Seed { get; set; } = [1, 2, 3, 4];

        [JsonPropertyName("rated")]
        public bool Rated { get; set; }

        [JsonPropertyName("debug")]
        public bool Debug { get; set; }
    }

}
