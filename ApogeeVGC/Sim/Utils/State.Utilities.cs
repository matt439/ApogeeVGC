namespace ApogeeVGC.Sim.Utils;

public static partial class State
{
    public static List<string> NormalizeLog(List<string>? log = null)
    {
        if (log == null || log.Count == 0)
        {
            return [];
        }

        // Normalize each line by removing timestamp variations
        // Lines starting with "|t:|" are normalized to just "|t:|"
        return log.Select(line =>
            line.StartsWith("|t:|") ? "|t:|" : line
        ).ToList();
    }

    public static List<string> NormalizeLog(string log)
    {
        if (string.IsNullOrEmpty(log))
        {
            return [];
        }

        // Split the log string into lines
        string[] lines = log.Split('\n');

        // Normalize each line
        var normalized = lines.Select(line =>
            line.StartsWith("|t:|") ? "|t:|" : line
        ).ToList();

        return normalized;
    }

    private const string Positions = "abcdefghijklmnopqrstuvwx";

    private static readonly IReadOnlyList<string> ActiveMove = new List<string>
    {
        "move",
    };
}