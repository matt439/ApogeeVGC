using System.Text.Json;
using System.Text.Json.Serialization;
using ApogeeVGC.Sim.Choices;

namespace ApogeeVGC.Sim.Utils;

/// <summary>
/// Custom JSON converter for IChoiceRequest that determines the concrete type based on the JSON properties.
/// </summary>
public class ChoiceRequestConverter : JsonConverter<IChoiceRequest>
{
    public override IChoiceRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
 var root = doc.RootElement;

        // Debug: print the JSON (first 200 chars)
  string json = root.GetRawText();
Console.WriteLine($"[ChoiceRequestConverter] JSON properties: {string.Join(", ", root.EnumerateObject().Select(p => p.Name))}");

        // Determine the type based on which properties are present
        // Check for Active property (MoveRequest) - it will be an array
        bool hasActive = root.TryGetProperty("Active", out var active) && active.ValueKind == JsonValueKind.Array;
      bool hasForceSwitch = root.TryGetProperty("ForceSwitch", out var forceSwitch) && forceSwitch.ValueKind == JsonValueKind.Array;
        bool hasTeamPreview = root.TryGetProperty("TeamPreview", out var teamPreview) && teamPreview.ValueKind != JsonValueKind.Null && teamPreview.GetBoolean();
   bool hasWait = root.TryGetProperty("Wait", out var wait) && wait.ValueKind != JsonValueKind.Null && wait.GetBoolean();

        Console.WriteLine($"[ChoiceRequestConverter] hasActive={hasActive}, hasForceSwitch={hasForceSwitch}, hasTeamPreview={hasTeamPreview}, hasWait={hasWait}");

        if (hasActive)
        {
    Console.WriteLine($"[ChoiceRequestConverter] Deserializing as MoveRequest");
return JsonSerializer.Deserialize<MoveRequest>(json, options);
      }
        else if (hasForceSwitch)
        {
 Console.WriteLine($"[ChoiceRequestConverter] Deserializing as SwitchRequest");
     return JsonSerializer.Deserialize<SwitchRequest>(json, options);
      }
 else if (hasTeamPreview)
     {
 Console.WriteLine($"[ChoiceRequestConverter] Deserializing as TeamPreviewRequest");
      return JsonSerializer.Deserialize<TeamPreviewRequest>(json, options);
 }
   else if (hasWait)
        {
   Console.WriteLine($"[ChoiceRequestConverter] Deserializing as WaitRequest");
    return JsonSerializer.Deserialize<WaitRequest>(json, options);
        }

        // Default to WaitRequest if we can't determine the type
  Console.WriteLine($"[ChoiceRequestConverter] Defaulting to WaitRequest");
        return JsonSerializer.Deserialize<WaitRequest>(json, options);
    }

    public override void Write(Utf8JsonWriter writer, IChoiceRequest value, JsonSerializerOptions options)
    {
        // Serialize the actual concrete type
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
