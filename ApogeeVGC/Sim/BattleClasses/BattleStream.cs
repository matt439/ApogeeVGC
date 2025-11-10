using ApogeeVGC.Data;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils;
using System.Text.Json;
using System.Threading.Channels;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.BattleClasses;

/// <summary>
/// Supports interacting with a Pokemon battle in Stream format.
/// Provides asynchronous message-based communication with the battle engine.
/// </summary>
public class BattleStream
{
    private readonly Channel<string> _inputChannel;
    private readonly Channel<string> _outputChannel;
    private readonly Task? _processingTask;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public bool Debug { get; }
    public bool NoCatch { get; }
    public bool KeepAlive { get; }
    public BattleReplayMode Replay { get; }
    private Battle Battle { get; set; }
    private Library Library { get; }

    public BattleStream(Library lib, BattleOptions battleOptions, BattleStreamOptions? options = null)
    {
        Library = lib;
        Battle = new Battle(battleOptions, Library);
        BattleStreamOptions options1 = options ?? new BattleStreamOptions();
        Debug = options1.Debug;
        NoCatch = options1.NoCatch;
        KeepAlive = options1.KeepAlive;
        Replay = options1.Replay;

        // Create unbounded channels for input/output
        _inputChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
        });

        _outputChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = true,
        });

        _cancellationTokenSource = new CancellationTokenSource();

        // Start processing input messages
        _processingTask = ProcessInputAsync(_cancellationTokenSource.Token);
    }

    /// <summary>
    /// Writes a message to the battle stream for processing.
    /// </summary>
    public async Task WriteAsync(string message, CancellationToken cancellationToken = default)
    {
        await _inputChannel.Writer.WriteAsync(message, cancellationToken);
    }

    /// <summary>
    /// Reads the next output message from the battle stream.
    /// </summary>
    public async Task<string?> ReadAsync(CancellationToken cancellationToken = default)
    {
        if (!await _outputChannel.Reader.WaitToReadAsync(cancellationToken)) return null;
        return _outputChannel.Reader.TryRead(out string? message) ? message : null;

    }

    /// <summary>
    /// Gets an async enumerable to read all output messages.
    /// </summary>
    public IAsyncEnumerable<string> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _outputChannel.Reader.ReadAllAsync(cancellationToken);
    }

    /// <summary>
    /// Signals that no more input will be written.
    /// </summary>
    public void CompleteWriting()
    {
        _inputChannel.Writer.Complete();
    }

    private async Task ProcessInputAsync(CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("[BattleStream] Starting ProcessInputAsync");
            await foreach (string chunk in _inputChannel.Reader.ReadAllAsync(cancellationToken))
            {
                Console.WriteLine($"[BattleStream] Processing chunk: {chunk.Substring(0, Math.Min(100, chunk.Length))}...");
                if (NoCatch)
                {
                    ProcessLines(chunk);
                }
                else
                {
                    try
                    {
                        ProcessLines(chunk);
                    }
                    catch (Exception ex)
                    {
                        await PushErrorAsync(ex);
                        return;
                    }
                }

                // Send battle updates after processing
                Console.WriteLine($"[BattleStream] Calling SendUpdates, Battle is {(Battle == null ? "null" : "not null")}");
                Battle?.SendUpdates();
            }

            Console.WriteLine("[BattleStream] Input completed");
            // Input completed, signal end if needed
            if (!KeepAlive)
            {
                await PushEndAsync();
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("[BattleStream] ProcessInputAsync cancelled");
            // Expected when cancellation is requested
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BattleStream] ProcessInputAsync error: {ex.Message}");
            await PushErrorAsync(ex);
        }
        finally
        {
            Console.WriteLine("[BattleStream] ProcessInputAsync completing output channel");
            _outputChannel.Writer.Complete();
        }
    }

    private void ProcessLines(string chunk)
    {
        foreach (string line in chunk.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            if (!line.StartsWith('>')) continue;

            (string type, string message) = SplitFirst(line[1..], ' ');
            ProcessLine(type, message);
        }
    }

    private async Task PushMessageAsync(string type, string data)
    {
        Console.WriteLine($"[BattleStream] PushMessage: type={type}, data length={data.Length}");
        if (Replay != BattleReplayMode.None)
        {
            if (type != "update") return;
            // Extract channel messages for replay
            string channelMessages = ExtractChannelMessages(data, Replay);
            if (!string.IsNullOrEmpty(channelMessages))
            {
                await _outputChannel.Writer.WriteAsync(channelMessages);
            }
            return;
        }

        // Normal mode: send type\ndata format
        string message = $"{type}\n{data}";
        Console.WriteLine($"[BattleStream] Writing to output channel: {message.Substring(0, Math.Min(100, message.Length))}...");
        await _outputChannel.Writer.WriteAsync(message);
    }

    private void ProcessLine(string type, string message)
    {
        switch (type)
        {
            case "start":
                {
                    BattleOptions options = JsonSerializer.Deserialize<BattleOptions>(message)
                                            ?? throw new InvalidOperationException("Invalid battle options");

                    // Set up the send callback to push messages to output
                    options.Send = (sendType, data) =>
                    {
                        string dataStr = string.Join("\n", data);

                        _ = PushMessageAsync(sendType.ToString().ToLowerInvariant(), dataStr);

                        if (sendType == SendType.End && !KeepAlive)
                        {
                            _ = PushEndAsync();
                        }
                    };

                    if (Debug)
                    {
                        options.Debug = true;
                    }

                    Battle = new Battle(options, Library);
                    break;
                }

            case "player":
                {
                    (string slotStr, string optionsText) = SplitFirst(message, ' ');
                    var slot = Enum.Parse<SideId>(slotStr, ignoreCase: true);
                    PlayerOptions playerOptions = JsonSerializer.Deserialize<PlayerOptions>(optionsText)
                                                  ?? throw new InvalidOperationException("Invalid player options");

                    if (Battle is null)
                    {
                        throw new InvalidOperationException("Battle not started");
                    }
                    Battle.SetPlayer(slot, playerOptions);
                    break;
                }

            case "p1":
            case "p2":
                {
                    if (Battle == null)
                        throw new InvalidOperationException("Battle not started");

                    var sideId = Enum.Parse<SideId>(type, ignoreCase: true);

                    if (message == "undo")
                    {
                        Battle.UndoChoice(sideId);
                    }
                    else
                    {
                        // Parse choice from message
                        Choice choice = ParseChoice(message);
                        Battle.Choose(sideId, choice);
                    }
                    break;
                }

            case "forcewin":
                {
                    if (Battle == null) break;
                    SideId? winnerId = string.IsNullOrEmpty(message)
                        ? null
                        : Enum.Parse<SideId>(message, ignoreCase: true);
                    Battle.ForceWin(winnerId);
                    Battle.InputLog.Add(string.IsNullOrEmpty(message)
                        ? ">forcetie"
                        : $">forcewin {message}");
                    break;
                }

            case "forcetie":
                {
                    Battle?.Tie();
                    Battle?.InputLog.Add(">forcetie");
                    break;
                }

            case "forcelose":
                {
                    if (Battle == null || string.IsNullOrEmpty(message)) break;
                    var loserId = Enum.Parse<SideId>(message, ignoreCase: true);
                    Battle.Lose(loserId);
                    Battle.InputLog.Add($">forcelose {message}");
                    break;
                }

            case "reseed":
                {
                    if (Battle == null) break;

                    // Validate that message contains only digits
                    if (!string.IsNullOrEmpty(message) && !ulong.TryParse(message, out _))
                    {
                        throw new InvalidOperationException($"Invalid seed for reseed: \"{message}\"");
                    }

                    // Convert seed string to int (or null if empty)
                    int? seed = string.IsNullOrEmpty(message)
                        ? null
                        : int.Parse(message);

                    PrngSeed? prngSeed = seed.HasValue ? new PrngSeed(seed.Value) : null;
                    Battle.ResetRng(prngSeed);
                    Battle.InputLog.Add($">reseed {Battle.Prng.GetSeed()}");
                    break;
                }

            case "tiebreak":
                {
                    Battle?.Tiebreak();
                    break;
                }

            case "chat-inputlogonly":
                {
                    Battle?.InputLog.Add($">chat {message}");
                    break;
                }

            case "chat":
                {
                    Battle?.InputLog.Add($">chat {message}");
                    if (Battle?.DisplayUi == true)
                    {
                        Battle.Add("chat", message);
                    }
                    break;
                }

            case "eval":
                {
                    if (Battle == null) break;

                    // Add to input log
                    Battle.InputLog.Add($">{type} {message}");

                    // Format message for display
                    message = message.Replace("\f", "\n");

                    if (Battle.DisplayUi)
                    {
                        Battle.Add("", ">>> " + message.Replace("\n", "\n||"));
                    }

                    try
                    {
                        // Note: Actual eval functionality would need to be implemented
                        // This is a placeholder for the eval logic
                        string result = "eval not implemented in C#";

                        if (Battle.DisplayUi)
                        {
                            Battle.Add("", "<<< " + result);
                        }
                    }
                    catch (Exception e)
                    {
                        if (Battle.DisplayUi)
                        {
                            Battle.Add("", "<<< error: " + e.Message);
                        }
                    }
                    break;
                }

            case "requestlog":
                {
                    if (Battle != null)
                    {
                        _ = PushMessageAsync("requesteddata", string.Join("\n", Battle.InputLog));
                    }
                    break;
                }

            case "requestexport":
                {
                    if (Battle != null)
                    {
                        string export = $"{Battle.Prng.GetSeed()}\n{string.Join("\n", Battle.InputLog)}";
                        _ = PushMessageAsync("requesteddata", export);
                    }
                    break;
                }

            case "requestteam":
                {
                    if (Battle == null) break;

                    message = message.Trim();
                    if (!int.TryParse(message.AsSpan(1), out int slotNum) || slotNum < 1)
                    {
                        throw new InvalidOperationException($"Team requested for slot {message}, but that slot does not exist.");
                    }

                    int slotIndex = slotNum - 1;
                    if (slotIndex >= Battle.Sides.Count)
                    {
                        throw new InvalidOperationException($"Team requested for slot {message}, but that slot does not exist.");
                    }

                    Side side = Battle.Sides[slotIndex];
                    string team = Teams.Pack(side.Team);
                    _ = PushMessageAsync("requesteddata", team);
                    break;
                }

            case "show-openteamsheets":
                {
                    Battle?.ShowOpenTeamSheets();
                    break;
                }

            case "version":
            case "version-origin":
                // Ignore version messages
                break;

            default:
                throw new InvalidOperationException($"Unrecognized command \"> {type} {message}\"");
        }
    }

    private async Task PushErrorAsync(Exception ex)
    {
        string errorMessage = $"error\n{ex.Message}";
        await _outputChannel.Writer.WriteAsync(errorMessage);
        _outputChannel.Writer.Complete(ex);
    }

    private async Task PushEndAsync()
    {
        await _outputChannel.Writer.WriteAsync("end\n");
        _outputChannel.Writer.Complete();
    }

    /// <summary>
    /// Splits a string at the first occurrence of a delimiter.
    /// Returns exactly limit + 1 parts.
    /// </summary>
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

    /// <summary>
    /// Extracts channel-specific messages from battle update data.
    /// </summary>
    private static string ExtractChannelMessages(string data, BattleReplayMode replayMode)
    {
        // This is a simplified implementation
        // TODO: Implement full channel extraction logic based on your needs
        int channel = replayMode switch
        {
            BattleReplayMode.Spectator => 0,
            BattleReplayMode.Full => -1,
            _ => -1,
        };

        // For now, just return the data as-is
        // In the full implementation, you'd parse the log lines and filter by channel
        return data;
    }

    /// <summary>
    /// Parses a choice string into a Choice object.
    /// </summary>
    private static Choice ParseChoice(string message)
    {
        // Parse choice format: "move 1" or "switch 2" or "move 1, switch 2" (for doubles)
        // Can also have "default" for team preview

        var choice = new Choice();
        var actions = new List<ChosenAction>();

        if (string.IsNullOrWhiteSpace(message))
        {
            choice.Error = "Empty choice";
            return choice;
        }

        // Split by comma for multiple Pokemon actions (doubles/triples)
        string[] individualChoices = message.Split(',', StringSplitOptions.TrimEntries);

        foreach (string individualChoice in individualChoices)
        {
            // Skip "default" or "pass" choices
            if (individualChoice.Equals("default", StringComparison.OrdinalIgnoreCase) ||
                individualChoice.Equals("pass", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // Parse individual choice
            string[] parts = individualChoice.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                continue;
            }

            string choiceType = parts[0].ToLowerInvariant();

            switch (choiceType)
            {
                case "move":
                    {
                        if (parts.Length < 2 || !int.TryParse(parts[1], out int moveSlot))
                        {
                            choice.Error = $"Invalid move choice: {individualChoice}";
                            return choice;
                        }

                        // Parse optional target (e.g., "move 1 2" for move slot 1 targeting position 2)
                        int? targetLoc = null;
                        if (parts.Length >= 3 && int.TryParse(parts[2], out int target))
                        {
                            targetLoc = target;
                        }
                        bool hasTerastallize = individualChoice.Contains("terastallize", StringComparison.OrdinalIgnoreCase) ||
                                              individualChoice.Contains("tera", StringComparison.OrdinalIgnoreCase);

                        if (hasTerastallize)
                        {
                            choice.Terastallize = true;
                        }

                        actions.Add(new ChosenAction
                        {
                            Choice = ChoiceType.Move,
                            MoveId = MoveId.None, // Will be resolved later by the battle engine
                            Index = moveSlot - 1, // Convert to 0-based index
                            TargetLoc = targetLoc,
                        });
                        break;
                    }

                case "switch":
                    {
                        if (parts.Length < 2 || !int.TryParse(parts[1], out int switchSlot))
                        {
                            choice.Error = $"Invalid switch choice: {individualChoice}";
                            return choice;
                        }

                        actions.Add(new ChosenAction
                        {
                            Choice = ChoiceType.Switch,
                            MoveId = MoveId.None,
                            Index = switchSlot - 1, // Convert to 0-based index
                        });

                        choice.SwitchIns.Add(switchSlot);
                        break;
                    }

                case "team":
                    {
                        // Team order for team preview (e.g., "team 123456")
                        if (parts.Length < 2)
                        {
                            choice.Error = $"Invalid team choice: {individualChoice}";
                            return choice;
                        }

                        // Parse team order - each digit is a pokemon position
                        string teamOrder = parts[1];
                        for (int i = 0; i < teamOrder.Length; i++)
                        {
                            if (char.IsDigit(teamOrder[i]))
                            {
                                int position = teamOrder[i] - '0';
                                actions.Add(new ChosenAction
                                {
                                    Choice = ChoiceType.Team,
                                    MoveId = MoveId.None,
                                    Index = position - 1, // Convert to 0-based index
                                });
                            }
                        }
                        break;
                    }

                case "shift":
                    {
                        actions.Add(new ChosenAction
                        {
                            Choice = ChoiceType.Shift,
                            MoveId = MoveId.None,
                        });
                        break;
                    }

                default:
                    choice.Error = $"Unknown choice type: {choiceType}";
                    return choice;
            }
        }

        choice.Actions = actions;
        return choice;
    }
}

/// <summary>
/// Options for configuring a BattleStream.
/// </summary>
public record BattleStreamOptions
{
    /// <summary>
    /// Enable debug logging.
    /// </summary>
    public bool Debug { get; init; }

    /// <summary>
    /// If true, exceptions will propagate instead of being caught.
    /// </summary>
    public bool NoCatch { get; init; }

    /// <summary>
    /// If true, the stream will remain open after the battle ends.
    /// </summary>
    public bool KeepAlive { get; init; }

    /// <summary>
    /// Replay mode for filtering output messages.
    /// </summary>
    public BattleReplayMode Replay { get; init; } = BattleReplayMode.None;
}

/// <summary>
/// Replay mode for BattleStream output filtering.
/// </summary>
public enum BattleReplayMode
{
    /// <summary>
    /// No replay mode - send all messages.
    /// </summary>
    None,

    /// <summary>
    /// Spectator view - public information only.
    /// </summary>
    Spectator,

    /// <summary>
    /// Full replay - all information including private data.
    /// </summary>
    Full,
}