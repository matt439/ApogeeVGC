using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Player;

namespace ApogeeVGC.Sim.Utils.Extensions;

/// <summary>
/// Splits a BattleStream into omniscient, spectator, p1 and p2 streams,
/// for ease of consumption.
/// </summary>
public static class BattleStreamExtensions
{
    /// <summary>
    /// Creates player-specific streams from a BattleStream.
    /// </summary>
    public static PlayerStreams GetPlayerStreams(BattleStream stream)
    {
        var omniscient = new PlayerReadWriteStream(stream);
        var spectator = new PlayerReadStream();
        var p1 = new PlayerReadWriteStream(stream, "p1");
        var p2 = new PlayerReadWriteStream(stream, "p2");

        var streams = new PlayerStreams(omniscient, spectator, p1, p2);

        // Start background task to distribute messages
        _ = Task.Run(async () =>
        {
            try
            {
                await foreach (string chunk in stream.ReadAllAsync())
                {
                    (string type, string data) = SplitFirst(chunk, '\n');

                    switch (type)
                    {
                        case "update":
                            {
                                // Extract channel messages for different perspectives
                                var channelMessages = ExtractChannelMessages(data, [-1, 0, 1, 2]);

                                await omniscient.PushAsync(string.Join('\n', channelMessages[-1]));
                                await spectator.PushAsync(string.Join('\n', channelMessages[0]));
                                await p1.PushAsync(string.Join('\n', channelMessages[1]));
                                await p2.PushAsync(string.Join('\n', channelMessages[2]));
                                break;
                            }
                        case "sideupdate":
                            {
                                (string side, string sideData) = SplitFirst(data, '\n');
                                Console.WriteLine($"[BattleStreamExtensions] sideupdate for {side}: {sideData.Substring(0, Math.Min(100, sideData.Length))}...");

                                PlayerReadStream? targetStream = side.ToLowerInvariant() switch
                                {
                                    "p1" => p1,
                                    "p2" => p2,
                                    _ => null
                                };

                                if (targetStream != null)
                                {
                                    await targetStream.PushAsync(sideData);
                                    Console.WriteLine($"[BattleStreamExtensions] Pushed sideupdate to {side} stream");
                                }
                                else
                                {
                                    Console.WriteLine($"[BattleStreamExtensions] Unknown side: {side}");
                                }
                                break;
                            }
                        case "end":
                            // Ignore end messages
                            break;
                    }
                }

                // Complete all streams
                omniscient.PushEnd();
                spectator.PushEnd();
                p1.PushEnd();
                p2.PushEnd();
            }
            catch (Exception err)
            {
                omniscient.PushError(err, true);
                spectator.PushError(err, true);
                p1.PushError(err, true);
                p2.PushError(err, true);
            }
        });

        return streams;
    }

    /// <summary>
    /// Extracts channel-specific messages from battle update data.
    /// </summary>
    private static Dictionary<int, List<string>> ExtractChannelMessages(string data, int[] channels)
    {
        var result = new Dictionary<int, List<string>>();
        foreach (int channel in channels)
        {
            result[channel] = new List<string>();
        }

        // Split data into lines
        var lines = data.Split('\n');

        foreach (string line in lines)
        {
            // Parse channel indicators (|split|channel)
            // For now, simplified implementation that adds to all channels
            // TODO: Implement full channel extraction logic based on message privacy

            // Channel -1 = omniscient (all messages)
            // Channel 0 = spectator (public messages only)
            // Channel 1-4 = player-specific (public + player's private messages)

            bool isPrivate = line.StartsWith("|") &&
                           (line.Contains("|split|") || line.Contains("|request|") ||
                            line.Contains("|teampreview"));

            foreach (int channel in channels)
            {
                if (channel == -1)
                {
                    // Omniscient sees everything
                    result[channel].Add(line);
                }
                else if (channel == 0)
                {
                    // Spectator only sees public messages
                    if (!isPrivate)
                    {
                        result[channel].Add(line);
                    }
                }
                else
                {
                    // Players see public messages + their own private messages
                    // TODO: Implement proper player-specific message filtering
                    result[channel].Add(line);
                }
            }
        }

        return result;
    }

    private static (string first, string rest) SplitFirst(string str, char delimiter, int limit = 1)
    {
        var parts = new List<string>();

        for (int i = 0; i < limit; i++)
        {
            int index = str.IndexOf(delimiter);
            if (index >= 0)
            {
                parts.Add(str[..index]);
                str = str[(index + 1)..];
            }
            else
            {
                parts.Add(str);
                str = string.Empty;
            }
        }

        parts.Add(str);

        return parts.Count >= 2 ? (parts[0], parts[1]) : (parts[0], string.Empty);
    }
}