using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ApogeeVGC_CS.lib;
using static ApogeeVGC_CS.sim.BattleStreamUtils;

namespace ApogeeVGC_CS.sim
{
    // Helper struct to hold player streams
    public struct PlayerStreams
    {
        public ObjectReadWriteStream<string> Omniscient { get; init; }
        public ObjectReadStream<string> Spectator { get; init; }
        public ObjectReadWriteStream<string> Player1 { get; init; }
        public ObjectReadWriteStream<string> Player2 { get; init; }
        public ObjectReadWriteStream<string> Player3 { get; init; } // Optional, for 3-player battles
        public ObjectReadWriteStream<string> Player4 { get; init; } // Optional, for 4-player battles
    }

    public static class BattleStreamUtils
    {
        public static string[] SplitFirst(string str, string delimiter, int limit = 1)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(delimiter))
                return [str];

            var parts = new List<string>();
            var remaining = str;

            for (int i = 0; i < limit && !string.IsNullOrEmpty(remaining); i++)
            {
                int index = remaining.IndexOf(delimiter, StringComparison.Ordinal);
                if (index == -1)
                {
                    parts.Add(remaining);
                    remaining = string.Empty;
                }
                else
                {
                    parts.Add(remaining[..index]);
                    remaining = remaining[(index + delimiter.Length)..];
                }
            }

            if (!string.IsNullOrEmpty(remaining))
                parts.Add(remaining);

            // Ensure we always return exactly limit + 1 elements
            while (parts.Count < limit + 1)
                parts.Add(string.Empty);

            return parts.ToArray();
        }

        /// <summary>
        /// Splits a BattleStream into omniscient, spectator, p1, p2, p3 and p4
        /// streams, for ease of consumption.
        /// </summary>
        public static PlayerStreams GetPlayerStreams(BattleStream stream)
        {
            var streams = new PlayerStreams
            {
                Omniscient = new ObjectReadWriteStream<string>(new ObjectReadWriteStreamOptions<string>
                {
                    Write = async (writeStream, data) => stream.Write(data),
                    WriteEnd = () => { 
                        // Note: WriteEnd method is not yet implemented in BattleStream
                        return Task.CompletedTask; 
                    }
                }),
                Spectator = new ObjectReadStream<string>([], new Queue<Exception>(), Task.CompletedTask),
                Player1 = new ObjectReadWriteStream<string>(new ObjectReadWriteStreamOptions<string>
                {
                    Write = async (writeStream, data) => stream.Write(Regex.Replace(data, @"(^|\n)", "$1>p1 "))
                }),
                Player2 = new ObjectReadWriteStream<string>(new ObjectReadWriteStreamOptions<string>
                {
                    Write = async (writeStream, data) => stream.Write(Regex.Replace(data, @"(^|\n)", "$1>p2 "))
                }),
                Player3 = new ObjectReadWriteStream<string>(new ObjectReadWriteStreamOptions<string>
                {
                    Write = async (writeStream, data) => stream.Write(Regex.Replace(data, @"(^|\n)", "$1>p3 "))
                }),
                Player4 = new ObjectReadWriteStream<string>(new ObjectReadWriteStreamOptions<string>
                {
                    Write = async (writeStream, data) => stream.Write(Regex.Replace(data, @"(^|\n)", "$1>p4 "))
                })
            };

            // Start async processing task
            _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var chunk in stream.ReadAllChunksAsync())
                    {
                        var parts = SplitFirst(chunk, "\n");
                        var type = parts[0];
                        var data = parts.Length > 1 ? parts[1] : string.Empty;

                        switch (type)
                        {
                            case "update":
                                var channelMessages = BattleUtils.ExtractChannelMessages(data, [-1, 0, 1, 2, 3, 4]);

                                if (channelMessages.TryGetValue(-1, out var omniscientMsgs))
                                    streams.Omniscient.Push(string.Join('\n', omniscientMsgs));

                                if (channelMessages.TryGetValue(0, out var spectatorMsgs))
                                    streams.Spectator.Push(string.Join('\n', spectatorMsgs));

                                if (channelMessages.TryGetValue(1, out var p1Msgs))
                                    streams.Player1.Push(string.Join('\n', p1Msgs));

                                if (channelMessages.TryGetValue(2, out var p2Msgs))
                                    streams.Player2.Push(string.Join('\n', p2Msgs));

                                if (channelMessages.TryGetValue(3, out var p3Msgs))
                                    streams.Player3.Push(string.Join('\n', p3Msgs));

                                if (channelMessages.TryGetValue(4, out var p4Msgs))
                                    streams.Player4.Push(string.Join('\n', p4Msgs));
                                break;

                            case "sideupdate":
                                var sideParts = SplitFirst(data, "\n");
                                var side = sideParts[0];
                                var sideData = sideParts.Length > 1 ? sideParts[1] : string.Empty;

                                var targetStream = side switch
                                {
                                    "p1" => streams.Player1,
                                    "p2" => streams.Player2,
                                    "p3" => streams.Player3,
                                    "p4" => streams.Player4,
                                    _ => null
                                };
                                targetStream?.Push(sideData);
                                break;

                            case "end":
                                // ignore
                                break;
                        }
                    }

                    // Signal end to all streams
                    streams.Omniscient.PushEnd();
                    streams.Spectator.PushEnd();
                    streams.Player1.PushEnd();
                    streams.Player2.PushEnd();
                    streams.Player3.PushEnd();
                    streams.Player4.PushEnd();
                }
                catch (Exception err)
                {
                    // Signal error to all streams
                    streams.Omniscient.PushError(err, true);
                    streams.Spectator.PushError(err, true);
                    streams.Player1.PushError(err, true);
                    streams.Player2.PushError(err, true);
                    streams.Player3.PushError(err, true);
                    streams.Player4.PushError(err, true);
                }
            });

            return streams;
        }
    }

    // Extension method for BattleStream to support async enumeration
    public static class BattleStreamExtensions
    {
        public static async IAsyncEnumerable<string> ReadAllChunksAsync(this BattleStream stream)
        {
            // Read from the base ObjectReadWriteStream implementation
            await foreach (var chunk in ((ObjectReadWriteStream<string>)stream))
            {
                yield return chunk;
            }
        }

        public static async IAsyncEnumerable<string> ReadAllChunksAsync(this ObjectReadWriteStream<string> stream)
        {
            // Read from the stream using the async enumerable interface
            await foreach (var chunk in stream)
            {
                yield return chunk;
            }
        }

        public static async IAsyncEnumerable<string> ReadAllChunksAsync(this ObjectReadStream<string> stream)
        {
            // Read from the stream using the async enumerable interface
            await foreach (string chunk in stream)
            {
                yield return chunk;
            }
        }
    }

    public class BattleStreamOptions
    {
        public bool? Debug { get; init; }
        public bool? NoCatch { get; init; }
        public bool? KeepAlive { get; init; }
        public BattleStreamReplay? Replay { get; init; }

    }

    public class BattleStream(BattleStreamOptions? options = null) : ObjectReadWriteStream<string>
    {
        public bool Debug { get; init; } = options?.Debug ?? false;
        public bool NoCatch { get; init; } = options?.NoCatch ?? false;
        public BattleStreamReplay Replay { get; init; } =
            options?.Replay ?? new BoolBattleStreamReplay(false);
        public bool KeepAlive { get; init; } = options?.KeepAlive ?? false;
        public Battle? Battle { get; set; } = null; // Made mutable

        public void Write(string chunk)
        {
            // Basic implementation: process the chunk by pushing it to the stream
            if (!string.IsNullOrEmpty(chunk))
            {
                // Split the chunk into lines and process each line
                var lines = chunk.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (line.StartsWith('>'))
                    {
                        // This is a command line, process it
                        var commandParts = SplitFirst(line[1..].Trim(), " ");
                        var command = commandParts[0];
                        var message = commandParts.Length > 1 ? commandParts[1] : string.Empty;
                        
                        WriteLine(command, message);
                    }
                    else if (!string.IsNullOrWhiteSpace(line))
                    {
                        // Regular data line, just push it
                        Push(line);
                    }
                }
            }
        }

        private void WriteLines(string chunk)
        {
            Write(chunk);
        }

        private void WriteLine(string type, string message)
        {
            switch (type)
            {
                case "start":
                    try
                    {
                        var options = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(message);
                        if (options != null)
                        {
                            // In a real implementation, you would create a Battle instance here
                            // For now, we'll just indicate that the battle would be created
                            
                            // Store battle creation info for potential future use
                            // Battle creation would happen here with proper BattleOptions
                            Push("info: Battle creation requested - implementation needed");
                        }
                    }
                    catch (Exception ex)
                    {
                        Push($"error: Failed to parse start options: {ex.Message}");
                    }
                    break;
                    
                case "player":
                    var parts = SplitFirst(message, " ");
                    var sideId = parts[0] switch
                    {
                        "p1" => SideId.P1, "p2" => SideId.P2, "p3" => SideId.P3, "p4" => SideId.P4,
                        _ => throw new ArgumentException($"Invalid slot: {parts[0]}")
                    };
                    try
                    {
                        var playerOptions = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(parts[1]);
                        // In a real implementation: Battle?.SetPlayer(sideId, playerOptions);
                        Push($"info: Player {sideId} setup requested - implementation needed");
                    }
                    catch (Exception ex)
                    {
                        Push($"error: Failed to parse player options: {ex.Message}");
                    }
                    break;
                    
                case "p1": case "p2": case "p3": case "p4":
                    var playerId = type switch
                    {
                        "p1" => SideId.P1, "p2" => SideId.P2, "p3" => SideId.P3, "p4" => SideId.P4,
                        _ => throw new ArgumentException($"Invalid type: {type}")
                    };
                    if (message == "undo")
                    {
                        // In a real implementation: Battle?.UndoChoice(playerId);
                        Push($"info: Undo choice for {playerId} - implementation needed");
                    }
                    else
                    {
                        // In a real implementation: Battle?.Choose(playerId, message);
                        Push($"info: Choice for {playerId}: {message} - implementation needed");
                    }
                    break;
                    
                case "forcewin": case "forcetie":
                    if (type == "forcewin")
                    {
                        var winSide = message switch
                        {
                            "p1" => SideId.P1, "p2" => SideId.P2, "p3" => SideId.P3, "p4" => SideId.P4,
                            _ => throw new ArgumentException($"Invalid side: {message}")
                        };
                        // In a real implementation: Battle?.Win(winSide);
                        Push($"info: Force win for {winSide} - implementation needed");
                    }
                    else
                    {
                        // In a real implementation: Battle?.Tie();
                        Push("info: Force tie - implementation needed");
                    }
                    
                    // Log the command
                    var logEntry = string.IsNullOrEmpty(message) ? "> forcetie" : $"> forcewin {message}";
                    Push($"log: {logEntry}");
                    break;
                    
                case "forcelose":
                    var loseSide = message switch
                    {
                        "p1" => SideId.P1, "p2" => SideId.P2, "p3" => SideId.P3, "p4" => SideId.P4,
                        _ => throw new ArgumentException($"Invalid side: {message}")
                    };
                    // In a real implementation: Battle?.Lose(loseSide);
                    Push($"info: Force lose for {loseSide} - implementation needed");
                    Push($"log: > forcelose {message}");
                    break;
                    
                case "reseed":
                    try
                    {
                        var seed = System.Text.Json.JsonSerializer.Deserialize<int[]>(message);
                        // In a real implementation: Battle?.ResetRng(seed);
                        Push("info: RNG reseed requested - implementation needed");
                        Push($"log: > reseed {message}");
                    }
                    catch (Exception ex)
                    {
                        Push($"error: Failed to parse seed: {ex.Message}");
                    }
                    break;
                    
                case "tiebreak":
                    // In a real implementation: Battle?.Tiebreak();
                    Push("info: Tiebreak - implementation needed");
                    break;
                    
                case "chat-inputlogonly":
                    Push($"log: > chat {message}");
                    break;
                    
                case "chat":
                    Push($"log: > chat {message}");
                    Push($"chat: {message}");
                    break;
                    
                case "eval":
                    // JavaScript eval equivalent - C# doesn't have direct eval
                    Push($"log: >eval {message}");
                    message = message.Replace("\f ", "\n");
                    Push(">>> " + message.Replace("\n ", "\n||"));
                    
                    try
                    {
                        // C# doesn't have JavaScript's eval() - this would require a scripting engine
                        // For now, just echo back that eval is not supported
                        string result = "C# eval not supported - would require scripting engine";
                        Push("<<< " + result);
                    }
                    catch (Exception e)
                    {
                        Push("<<< error: " + e.Message);
                    }
                    break;
                    
                case "requestlog":
                    // In a real implementation, this would return the actual input log
                    Push("requesteddata\n" + "# Input log would be returned here");
                    break;
                    
                case "requestexport":
                    // In a real implementation, this would return seed + input log
                    Push("requesteddata\n" + "# Export data would be returned here");
                    break;
                    
                case "requestteam":
                    if (int.TryParse(message[1..], out int slotNum))
                    {
                        slotNum -= 1; // Convert to 0-based
                        if (slotNum < 0 || slotNum >= 4) // Assuming max 4 players
                            throw new Exception($"Team requested for slot {message}, but that slot does not exist.");
                        
                        // In a real implementation: var team = Teams.Pack(side.Team);
                        Push("requesteddata\n" + $"# Team data for slot {slotNum} would be returned here");
                    }
                    break;
                    
                case "show-openteamsheets":
                    // In a real implementation: Battle?.ShowOpenTeamSheets();
                    Push("info: Show open team sheets - implementation needed");
                    break;
                    
                case "version": case "version-origin":
                    // These are ignored in the original implementation
                    break;
                    
                default:
                    throw new Exception($"Unrecognized command \">{type} {message}\"");
            }
        }
    }

    public abstract class BattlePlayer
    {
        public object Stream { get; } // Changed to object to accept any stream type
        public List<string> Log { get; } = [];
        public bool Debug { get; }
        private CancellationTokenSource? _cancellationTokenSource;

        public BattlePlayer(object playerStream, bool debug = false)
        {
            Stream = playerStream;
            Debug = debug;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                await foreach (var chunk in ReadStreamChunksAsync(_cancellationTokenSource.Token))
                {
                    Receive(chunk);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelled
            }
            catch (Exception ex)
            {
                ReceiveError(ex);
            }
        }

        private async IAsyncEnumerable<string> ReadStreamChunksAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            switch (Stream)
            {
                case ObjectReadWriteStream<string> readWriteStream:
                    await foreach (var chunk in readWriteStream.ReadAllChunksAsync()
                        .WithCancellation(cancellationToken))
                    {
                        yield return chunk;
                    }
                    break;

                case ObjectReadStream<string> readStream:
                    await foreach (var chunk in readStream.ReadAllChunksAsync()
                        .WithCancellation(cancellationToken))
                    {
                        yield return chunk;
                    }
                    break;

                case System.IO.Stream netStream:
                {
                    // Fallback for standard .NET streams
                    using var reader = new StreamReader(netStream, leaveOpen: true);
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        yield return line;
                    }
                    break;
                }
                
                default:
                    throw new NotSupportedException($"Unsupported stream type: {Stream.GetType()}");
            }
        }

        public void Receive(string chunk)
        {
            if (string.IsNullOrEmpty(chunk)) return;

            foreach (var line in chunk.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                ReceiveLine(line);
            }
        }

        public void ReceiveLine(string line)
        {
            if (Debug) Console.WriteLine(line);

            if (!line.StartsWith('|')) return;

            var parts = BattleStreamUtils.SplitFirst(line[1..], "|");
            var cmd = parts[0];
            var rest = parts.Length > 1 ? parts[1] : string.Empty;

            switch (cmd)
            {
                case "request":
                    HandleRequestCommand(rest);
                    return;

                case "error":
                    ReceiveError(new Exception(rest));
                    return;
            }

            Log.Add(line);
        }

        private void HandleRequestCommand(string jsonData)
        {
            try
            {
                // Use more sophisticated JSON parsing based on your IChoiceRequest implementation
                var request = ParseChoiceRequest(jsonData);
                if (request != null)
                {
                    ReceiveRequest(request);
                }
            }
            catch (Exception ex)
            {
                ReceiveError(new Exception($"Failed to parse request: {ex.Message}", ex));
            }
        }

        protected virtual IChoiceRequest? ParseChoiceRequest(string jsonData)
        {
            // This would need to be implemented based on your specific IChoiceRequest types
            // You might need a custom JsonConverter or factory method
            return System.Text.Json.JsonSerializer.Deserialize<IChoiceRequest>(jsonData);
        }

        public abstract void ReceiveRequest(IChoiceRequest request);

        public virtual void ReceiveError(Exception error)
        {
            throw error;
        }

        public async Task ChooseAsync(string choice)
        {
            var choiceBytes = System.Text.Encoding.UTF8.GetBytes(choice + "\n");
            
            switch (Stream)
            {
                case ObjectReadWriteStream<string> readWriteStream:
                    await readWriteStream.WriteAsync(choice);
                    break;
                case System.IO.Stream netStream:
                    await netStream.WriteAsync(choiceBytes, 0, choiceBytes.Length);
                    await netStream.FlushAsync();
                    break;
                default:
                    throw new NotSupportedException($"Unsupported stream type for writing: {Stream.GetType()}");
            }
        }

        public void Choose(string choice)
        {
            var choiceBytes = System.Text.Encoding.UTF8.GetBytes(choice + "\n");
            
            switch (Stream)
            {
                case ObjectReadWriteStream<string> readWriteStream:
                    // For synchronous operation, we need to block on the async method
                    Task.Run(async () => await readWriteStream.WriteAsync(choice)).Wait();
                    break;
                case System.IO.Stream netStream:
                    netStream.Write(choiceBytes, 0, choiceBytes.Length);
                    netStream.Flush();
                    break;
                default:
                    throw new NotSupportedException($"Unsupported stream type for writing: {Stream.GetType()}");
            }
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        // Keep the old synchronous Start method for backward compatibility
        public Delegate Start()
        {
            return new Func<Task>(() => StartAsync());
        }
    }

    public class BattleTextStream // : Stream or custom ReadWriteStream<string>
    {
        public BattleStream BattleStream { get; }
        public string CurrentMessage { get; set; }

        public BattleTextStream(BattleStreamOptions? options = null)
        {
            BattleStream = new BattleStream(options);
            CurrentMessage = string.Empty;
            Listen();
        }

        private Delegate Listen()
        {
            throw new NotImplementedException("Listen method is not implemented yet.");
        }
        
        private void Write(string message)
        {
            throw new NotImplementedException("Write method is not implemented yet.");
        }

        private Delegate WriteEnd()
        {
            throw new NotImplementedException("WriteEnd method is not implemented yet.");
        }
    }
}