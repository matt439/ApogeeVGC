using System.Text.Json.Nodes;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Utils;

public static class State
{
    private const string Positions = "abcdefghijklmnopqrstuvwx";

    private static IReadOnlyList<string> _battle = new List<string>
    {
        "dex",
        "gen",
        "ruleTable",
        "id",
        "log",
        "inherit",
        "format",
        "teamGenerator",
        "HIT_SUBSTITUTE",
        "NOT_FAIL",
        "FAIL",
        "SILENT_FAIL",
        "field",
        "sides",
        "prng",
        "hints",
        "deserialized",
        "queue",
        "actions",
    };

    private static IReadOnlyList<string> _side = new List<string>
    {
        "battle",
        "team",
        "pokemon",
        "choice",
        "activeRequest",
    };

    private static IReadOnlyList<string> _pokemon = new List<string>
    {
        "side",
        "battle",
        "set",
        "name",
        "fullname",
        "id",
        "happiness",
        "level",
        "pokeball",
        "baseMoveSlots",
    };

    private static IReadOnlyList<string> _choice = new List<string>
    {
        "switchIns",
    };

    private static IReadOnlyList<string> _activeMove = new List<string>
    {
        "move",
    };

    public static JsonObject SerializeBattle(IBattle battle)
    {
        throw new NotImplementedException();
    }

    public static IBattle DeserializeBattle(JsonObject serialized)
    {
        throw new NotImplementedException();
    }

    public static IBattle DeserializeBattle(string serialized)
    {
        throw new NotImplementedException();
    }

    public static JsonObject Normalize(JsonObject state)
    {
        throw new NotImplementedException();
    }

    public static List<string> NormalizeLog(List<string>? log = null)
    {
        throw new NotImplementedException();
    }

    public static List<string> NormalizeLog(string log)
    {
        throw new NotImplementedException();
    }

    public static JsonObject SerializeField(Field field)
    {
        throw new NotImplementedException();
    }

    public static void DeserializeField(JsonObject state, out Field field)
    {
        throw new NotImplementedException();
    }

    public static JsonObject SerializeSide(Side side)
    {
        throw new NotImplementedException();
    }

    public static void DeserializeSide(JsonObject state, out Side side)
    {
        throw new NotImplementedException();
    }

    public static JsonObject SerializePokemon(Pokemon pokemon)
    {
        throw new NotImplementedException();
    }

    public static void DeserializePokemon(JsonObject state, out Pokemon pokemon)
    {
        throw new NotImplementedException();
    }

    public static bool IsActiveMove(JsonObject obj)
    {
        throw new NotImplementedException();
    }

    public static JsonObject SerializeActiveMove(ActiveMove activeMove, IBattle battle)
    {
        throw new NotImplementedException();
    }

    public static ActiveMove DeserializeActiveMove(JsonObject state, IBattle battle)
    {
        throw new NotImplementedException();
    }

    public static object? SerializeWithRefs(object? obj, IBattle battle)
    {
        throw new NotImplementedException();
    }

    public static object? DeserializeWithRefs(object? obj, IBattle battle)
    {
        throw new NotImplementedException();
    }

    public static bool IsReferable(object obj)
    {
        throw new NotImplementedException();
    }

    public static string ToRef(Referable obj)
    {
        throw new NotImplementedException();
    }

    public static ReferableUndefinedUnion FromRef(string reference, IBattle battle)
    {
        throw new NotImplementedException();
    }

    public static JsonObject Serialize(object obj, List<string> skip, IBattle battle)
    {
        throw new NotImplementedException();
    }

    public static void Deserialize(JsonObject state, out object obj, List<string> skip, IBattle battle)
    {
        throw new NotImplementedException();
    }
}