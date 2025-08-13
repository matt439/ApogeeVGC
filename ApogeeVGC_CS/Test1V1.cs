using System.Text.Json;
using ApogeeVGC_CS.sim;
using ApogeeVGC_CS.lib;

namespace ApogeeVGC_CS
{
    public class PlayerSpecification
    {
        public required string Name { get; init; }
        public required string Team { get; init; }
    }

    public class Test1V1
    {
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

            var battleSpec = new BattleSpecification
            {
                FormatId = "gen9vgc2024regh",
                Seed = [1, 2, 3, 4], // Use a simple array instead of Prng.GenerateSeed().ToArray()
                Rated = false,
                Debug = true
            };

            var p1 = new RandomPlayerAi(streams.Player1, new RandomPlayerAiOptions());
            var p2 = new RandomPlayerAi(streams.Player2, new RandomPlayerAiOptions());

            InitializeBattle(streams, battleSpec, p1Spec, p2Spec);

            p1.Start();
            p2.Start();

            var streamReadingTask = Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine("Starting to read from omniscient stream...");
                    
                    // Use a timeout to prevent infinite waiting
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                    
                    await foreach (string chunk in streams.Omniscient.ReadAllChunksAsync().WithCancellation(cts.Token))
                    {
                        Console.WriteLine($"Received chunk: {chunk}");
                    }
                    
                    Console.WriteLine("Finished reading from omniscient stream.");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Stream reading timed out after 30 seconds.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading omniscient stream: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            });

            // Wait a bit to let everything initialize
            Console.WriteLine("Waiting for battle to process...");
            Thread.Sleep(5000);
            
            Console.WriteLine("Test completed. Check output above for battle information.");

            Console.ReadLine();
        }

        private static void InitializeBattle(PlayerStreams streams, BattleSpecification battleSpec,
            PlayerSpecification p1Spec, PlayerSpecification p2Spec)
        {
            try
            {
                // Use cached JsonSerializerOptions to fix CA1869
                string battleJson = System.Text.Json.JsonSerializer.Serialize(battleSpec, JsonOptions);
                string p1Json = System.Text.Json.JsonSerializer.Serialize(p1Spec, JsonOptions);
                string p2Json = System.Text.Json.JsonSerializer.Serialize(p2Spec, JsonOptions);

                // Create the initialization command sequence
                var commands = new[]
                {
                    $"> start {battleJson}",
                    $"> player p1 {p1Json}",
                    $"> player p2 {p2Json}"
                };

                string fullCommand = string.Join("\n", commands) + "\n";

                // Print the fullCommand to console for debugging
                Console.WriteLine("Sending initialization commands:");
                Console.WriteLine(fullCommand);
                Console.WriteLine("=== End of initialization commands ===");

                // Use the WriteAsync method for the ObjectReadWriteStream
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await streams.Omniscient.WriteAsync(fullCommand);
                        Console.WriteLine("Commands sent to omniscient stream successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error writing to omniscient stream: {ex.Message}");
                    }
                });

                Console.WriteLine("Battle initialized successfully");
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
        public string FormatId { get; set; } = "gen9vgc2024regh";
        public int[] Seed { get; set; } = { 1, 2, 3, 4 };
        public bool Rated { get; set; } = false;
        public bool Debug { get; set; } = false;
    }
}
