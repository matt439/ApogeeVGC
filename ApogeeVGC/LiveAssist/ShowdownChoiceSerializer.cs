using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Converts AI decision outputs (LegalAction) into Showdown /choose command strings.
/// Reference: SIM-PROTOCOL.md "Sending decisions" section.
/// </summary>
public static partial class ShowdownChoiceSerializer
{
    /// <summary>
    /// Serialize a team preview selection.
    /// Input: 0-based indices of Pokemon to bring, leads first.
    /// VGC brings 4 of 6: e.g. indices [2,0,4,1] → "team 3152"
    /// </summary>
    public static string SerializeTeamPreview(int[] orderedIndices)
    {
        var sb = new StringBuilder("team ");
        foreach (int idx in orderedIndices)
            sb.Append(idx + 1); // Convert 0-based to 1-based
        return sb.ToString();
    }

    /// <summary>
    /// Serialize a battle choice from one or two LegalActions.
    /// Uses the ShowdownState's last request JSON to find 1-based move slot numbers.
    /// </summary>
    public static string SerializeBattleChoice(
        LegalAction slotA, LegalAction? slotB, ShowdownState state)
    {
        JsonElement root = state.LastRequest!.RootElement;

        string choiceA = SerializeSlotChoice(slotA, root, slotIndex: 0);

        if (slotB == null)
            return choiceA;

        // Prevent both slots switching to the same Pokemon
        LegalAction finalB = slotB.Value;
        if (slotA.ChoiceType == ChoiceType.Switch &&
            finalB.ChoiceType == ChoiceType.Switch &&
            slotA.SwitchIndex == finalB.SwitchIndex)
        {
            Console.WriteLine($"  [Serializer] WARNING: Both slots switching to slot {slotA.SwitchIndex + 1} — sending pass for slot B");
            return $"{choiceA}, pass";
        }

        string choiceB = SerializeSlotChoice(finalB, root, slotIndex: 1);
        return $"{choiceA}, {choiceB}";
    }

    private static string SerializeSlotChoice(LegalAction action, JsonElement root, int slotIndex)
    {
        return action.ChoiceType switch
        {
            ChoiceType.Move => SerializeMoveChoice(action, root, slotIndex),
            ChoiceType.Switch => $"switch {action.SwitchIndex + 1}",
            _ => "pass",
        };
    }

    private static string SerializeMoveChoice(LegalAction action, JsonElement root, int slotIndex)
    {
        // Find the 1-based move slot number from the request JSON
        int moveSlot = FindMoveSlot(action.MoveId, root, slotIndex);

        var sb = new StringBuilder();
        sb.Append($"move {moveSlot}");

        // Target (only for moves that need targeting in doubles)
        if (action.TargetLoc != 0)
        {
            if (action.TargetLoc > 0)
                sb.Append($" +{action.TargetLoc}");
            else
                sb.Append($" {action.TargetLoc}");
        }

        // Modifiers — only include terastallize if Showdown's request actually allows it
        if (action.Terastallize.HasValue && CanTerastallize(root, slotIndex))
            sb.Append(" terastallize");
        if (action.Mega.HasValue)
            sb.Append(" mega");

        return sb.ToString();
    }

    /// <summary>
    /// Find the 1-based move slot in the request JSON's active[slotIndex].moves[] array
    /// matching the given MoveId. Falls back to slot 1 if not found.
    /// </summary>
    private static int FindMoveSlot(MoveId moveId, JsonElement root, int slotIndex)
    {
        if (!root.TryGetProperty("active", out JsonElement active))
            return 1;
        if (slotIndex >= active.GetArrayLength())
            return 1;

        JsonElement slotData = active[slotIndex];
        if (!slotData.TryGetProperty("moves", out JsonElement moves))
            return 1;

        // Convert our MoveId to a showdown-style ID for matching
        string targetId = ToShowdownId(moveId.ToString());

        int slot = 1;
        foreach (JsonElement move in moves.EnumerateArray())
        {
            string id = move.TryGetProperty("id", out JsonElement idElem)
                ? idElem.GetString() ?? "" : "";

            if (string.Equals(id, targetId, StringComparison.OrdinalIgnoreCase))
                return slot;

            slot++;
        }

        // Fallback: try matching by move display name
        slot = 1;
        foreach (JsonElement move in moves.EnumerateArray())
        {
            string moveName = move.TryGetProperty("move", out JsonElement nameElem)
                ? nameElem.GetString() ?? "" : "";

            if (ToShowdownId(moveName) == targetId)
                return slot;

            slot++;
        }

        return 1; // Last resort fallback
    }

    /// <summary>
    /// Convert a name to Showdown ID format: lowercase, no non-alphanumeric chars.
    /// </summary>
    private static string ToShowdownId(string name)
    {
        return ShowdownIdRegex().Replace(name.ToLowerInvariant(), "");
    }

    /// <summary>
    /// Check if the Showdown request allows terastallization for the given slot.
    /// </summary>
    private static bool CanTerastallize(JsonElement root, int slotIndex)
    {
        if (!root.TryGetProperty("active", out JsonElement active))
            return false;
        if (slotIndex >= active.GetArrayLength())
            return false;
        JsonElement slotData = active[slotIndex];
        return slotData.TryGetProperty("canTerastallize", out JsonElement canTera)
               && canTera.ValueKind == JsonValueKind.String
               && !string.IsNullOrEmpty(canTera.GetString());
    }

    [GeneratedRegex(@"[^a-z0-9]")]
    private static partial Regex ShowdownIdRegex();
}
