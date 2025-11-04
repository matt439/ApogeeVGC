using System.Text.Json;
using System.Text.Json.Serialization;
using ApogeeVGC.Sim.FormatClasses;

namespace ApogeeVGC.Sim.Utils;

/// <summary>
/// JSON converter for FormatId that handles Showdown format strings.
/// Converts between strings like "gen9customgame" and enum values like FormatId.Gen9CustomGame.
/// </summary>
public class FormatIdJsonConverter : JsonConverter<FormatId>
{
    public override FormatId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        if (string.IsNullOrEmpty(value))
        {
            return FormatId.Gen9Ou; // Default
 }

   // Convert showdown format strings to FormatId enum
        return value.ToLowerInvariant() switch
        {
            "gen9ou" => FormatId.Gen9Ou,
        "gen9customgame" => FormatId.Gen9CustomGame,
     "gen9doublescustomgame" => FormatId.Gen9CustomGame, // Doubles format maps to Gen9CustomGame
            "emptyeffect" => FormatId.EmptyEffect,
            _ => FormatId.Gen9Ou, // Default fallback
        };
    }

    public override void Write(Utf8JsonWriter writer, FormatId value, JsonSerializerOptions options)
    {
      // Convert FormatId enum to showdown format string
    string formatString = value switch
   {
            FormatId.Gen9Ou => "gen9ou",
     FormatId.Gen9CustomGame => "gen9customgame",
      FormatId.EmptyEffect => "emptyeffect",
            _ => "gen9ou",
        };

        writer.WriteStringValue(formatString);
    }
}
