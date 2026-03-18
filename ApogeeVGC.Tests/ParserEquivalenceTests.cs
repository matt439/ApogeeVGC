using System.Collections.Concurrent;
using System.Text.Json;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using PlayerType = ApogeeVGC.Sim.Player.PlayerType;
using ApogeeVGC.Sim.Utils;
using Xunit.Abstractions;

namespace ApogeeVGC.Tests;

/// <summary>
/// Generates battle fixtures for validating the Python parser.
/// Runs C# battles, captures protocol log + BattlePerspective at each turn,
/// and writes them as JSON fixtures. A companion Python test
/// (Tools/ReplayScraper/test_parser_equivalence.py) parses the log and
/// compares against the C# ground truth.
/// </summary>
[Collection(LibraryCollection.Name)]
public class ParserEquivalenceTests(LibraryFixture fixture, ITestOutputHelper output)
{
    private const int NumFixtures = 1000;
    private const int BaseSeed = 77777;
    private static readonly string FixtureDir = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..",
        "Tools", "ReplayScraper", "test_fixtures");

    private readonly Library _library = fixture.Library;

    [Fact]
    public void GenerateParserFixtures()
    {
        Directory.CreateDirectory(FixtureDir);

        var fixtures = new ConcurrentBag<object>();
        var errors = new ConcurrentBag<(int, Exception)>();

        Parallel.For(0, NumFixtures, i =>
        {
            try
            {
                object? fixtureData = RunBattleAndCapture(i);
                if (fixtureData != null)
                    fixtures.Add(fixtureData);
            }
            catch (Exception ex)
            {
                errors.Add((i, ex));
            }
        });

        output.WriteLine($"Generated {fixtures.Count}/{NumFixtures} fixtures, {errors.Count} errors");

        foreach (var (idx, ex) in errors.OrderBy(e => e.Item1).Take(5))
            output.WriteLine($"  Battle {idx}: {ex.GetType().Name}: {ex.Message}");

        // Write all fixtures to a single JSON file
        string json = JsonSerializer.Serialize(fixtures.ToArray(), new JsonSerializerOptions
        {
            WriteIndented = true,
        });
        string path = Path.Combine(FixtureDir, "parser_fixtures.json");
        File.WriteAllText(path, json);
        output.WriteLine($"Wrote fixtures to {path}");

        Assert.True(fixtures.Count > 0, "Should generate at least one fixture");
    }

    private object? RunBattleAndCapture(int index)
    {
        int baseOffset = index * 5;
        int team1Seed = BaseSeed + baseOffset;
        int team2Seed = BaseSeed + baseOffset + 1;
        int player1Seed = BaseSeed + baseOffset + 2;
        int player2Seed = BaseSeed + baseOffset + 3;
        int battleSeed = BaseSeed + baseOffset + 4;

        FormatId formatId = FormatId.Gen9VgcRegulationI;
        var team1 = new RandomTeamGenerator(_library, formatId, team1Seed).GenerateTeam();
        var team2 = new RandomTeamGenerator(_library, formatId, team2Seed).GenerateTeam();

        var battleOptions = new BattleOptions
        {
            Id = formatId,
            Player1Options = new PlayerOptions
            {
                Type = PlayerType.Random,
                Name = "Player 1",
                Team = team1,
                Seed = new PrngSeed(player1Seed),
                PrintDebug = false,
            },
            Player2Options = new PlayerOptions
            {
                Type = PlayerType.Random,
                Name = "Player 2",
                Team = team2,
                Seed = new PrngSeed(player2Seed),
                PrintDebug = false,
            },
            Debug = false,
            Sync = true,
            DisplayUi = true,
            Seed = new PrngSeed(battleSeed),
            MaxTurns = 200,
        };

        // Run battle via SimulatorSync, capturing perspectives at each turn
        var turnSnapshots = new List<object>();
        int lastCapturedTurn = -1;

        var battle = new Battle(battleOptions, _library);
        var p1Player = new ApogeeVGC.Sim.Player.PlayerRandom(SideId.P1, battleOptions.Player1Options,
            new SimulatorSync());
        var p2Player = new ApogeeVGC.Sim.Player.PlayerRandom(SideId.P2, battleOptions.Player2Options,
            new SimulatorSync());

        battle.ChoiceRequested += (_, args) =>
        {
            // Capture perspective at each new turn (move request only)
            if (args.RequestType == BattleRequestType.TurnStart)
            {
                int turn = battle.Turn;
                if (turn != lastCapturedTurn)
                {
                    lastCapturedTurn = turn;
                    var p1Persp = battle.GetPerspectiveForSide(SideId.P1);
                    var p2Persp = battle.GetPerspectiveForSide(SideId.P2);
                    turnSnapshots.Add(new
                    {
                        turn,
                        p1 = SerializeSidePerspective(p1Persp),
                        p2 = SerializeSidePerspective(p2Persp),
                    });
                }
            }

            // Let the random player make a choice
            ApogeeVGC.Sim.Player.IPlayer player = args.SideId == SideId.P1 ? p1Player : p2Player;
            Side side = battle.Sides.First(s => s.Id == args.SideId);
            var choice = player.GetChoiceSync(args.Request, args.RequestType,
                () => args.Perspective);

            if (choice.Actions.Count == 0)
            {
                side.AutoChoose();
                if (battle.AllChoicesDone()) battle.CommitChoices();
                return;
            }

            if (!battle.Choose(args.SideId, choice))
            {
                side.AutoChoose();
                if (battle.AllChoicesDone()) battle.CommitChoices();
            }
        };

        battle.UpdateRequested += (_, _) => { };
        battle.BattleEnded += (_, _) => { };

        // Validate teams
        var validator = new TeamValidator(_library, battle.Format);
        validator.ValidateTeam(battleOptions.Player1Options.Team);
        validator.ValidateTeam(battleOptions.Player2Options.Team);

        battle.Start();
        while (!battle.Ended)
        {
            if (battle.RequestState != RequestState.None)
                battle.RequestPlayerChoices();
            else
                break;
        }

        if (turnSnapshots.Count == 0)
            return null;

        string? winner = null;
        if (!string.IsNullOrEmpty(battle.Winner))
        {
            winner = battle.Winner == battleOptions.Player1Options.Name ? "p1" : "p2";
        }

        return new
        {
            index,
            seeds = new
            {
                team1 = team1Seed, team2 = team2Seed,
                player1 = player1Seed, player2 = player2Seed,
                battle = battleSeed,
            },
            winner,
            log = battle.Log.ToArray(),
            turns = turnSnapshots,
        };
    }

    private object SerializeSidePerspective(BattlePerspective perspective)
    {
        return new
        {
            active = SerializeActivePokemon(perspective.PlayerSide.Active),
            opponent_active = SerializeActivePokemon(perspective.OpponentSide.Active),
            field = SerializeField(
                perspective.Field,
                perspective.PlayerSide.SideConditionsWithDuration,
                perspective.OpponentSide.SideConditionsWithDuration),
        };
    }

    private object?[] SerializeActivePokemon(IReadOnlyList<PokemonPerspective?> actives)
    {
        return actives.Select(p =>
        {
            if (p == null) return null;
            return (object)new
            {
                species = _library.Species[p.Species].Name,
                hp = p.MaxHp > 0 ? (int)Math.Round(100.0 * p.Hp / p.MaxHp) : 0,
                fainted = p.Fainted || p.Hp == 0,
                status = SerializeStatus(p.Status),
                boosts = new
                {
                    atk = p.Boosts.Atk,
                    def_ = p.Boosts.Def,
                    spa = p.Boosts.SpA,
                    spd = p.Boosts.SpD,
                    spe = p.Boosts.Spe,
                },
                tera = p.Terastallized?.ToString(),
                volatiles = p.Volatiles
                    .Select(v => v.ToString())
                    .OrderBy(v => v)
                    .ToArray(),
            };
        }).ToArray();
    }

    private static string? SerializeStatus(ConditionId status) => status switch
    {
        ConditionId.Paralysis => "par",
        ConditionId.Burn => "brn",
        ConditionId.Sleep => "slp",
        ConditionId.Poison => "psn",
        ConditionId.Toxic => "tox",
        ConditionId.Freeze => "frz",
        _ => null,
    };

    private static object SerializeField(
        Sim.FieldClasses.FieldPerspective field,
        IReadOnlyDictionary<ConditionId, int?> mySideConditions,
        IReadOnlyDictionary<ConditionId, int?> oppSideConditions)
    {
        return new
        {
            weather = SerializeWeather(field.Weather),
            terrain = SerializeTerrain(field.Terrain),
            trick_room = field.PseudoWeather.Contains(ConditionId.TrickRoom),
            my_tailwind = mySideConditions.ContainsKey(ConditionId.Tailwind),
            opp_tailwind = oppSideConditions.ContainsKey(ConditionId.Tailwind),
            my_reflect = mySideConditions.ContainsKey(ConditionId.Reflect),
            opp_reflect = oppSideConditions.ContainsKey(ConditionId.Reflect),
            my_light_screen = mySideConditions.ContainsKey(ConditionId.LightScreen),
            opp_light_screen = oppSideConditions.ContainsKey(ConditionId.LightScreen),
            my_aurora_veil = mySideConditions.ContainsKey(ConditionId.AuroraVeil),
            opp_aurora_veil = oppSideConditions.ContainsKey(ConditionId.AuroraVeil),
        };
    }

    private static string? SerializeWeather(ConditionId weather) => weather switch
    {
        ConditionId.SunnyDay or ConditionId.DesolateLand => "SunnyDay",
        ConditionId.RainDance or ConditionId.PrimordialSea => "RainDance",
        ConditionId.Sandstorm => "Sandstorm",
        ConditionId.Snowscape => "Snowscape",
        _ => null,
    };

    private static string? SerializeTerrain(ConditionId terrain) => terrain switch
    {
        ConditionId.ElectricTerrain => "Electric Terrain",
        ConditionId.GrassyTerrain => "Grassy Terrain",
        ConditionId.PsychicTerrain => "Psychic Terrain",
        ConditionId.MistyTerrain => "Misty Terrain",
        _ => null,
    };
}
