using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.BattleClasses;

/// <summary>
/// Supports interacting with a Pokemon battle in Stream format.
/// Provides asynchronous message-based communication with the battle engine.
/// </summary>
public class BattleStream : IDisposable
{
    private readonly Channel<string> _inputChannel;
    private readonly Channel<string> _outputChannel;
    private bool _disposed;
    private readonly Task? _processingTask;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public bool Debug { get; }
    public bool NoCatch { get; }
    public bool KeepAlive { get; }
    public BattleReplayMode Replay { get; }
    private IBattle? Battle { get; set; }
    private Library Library { get; }

    public BattleStream(Library lib, BattleStreamOptions? options = null)
    {
        Library = lib;
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
        if (!_disposed)
            await _inputChannel.Writer.WriteAsync(message, cancellationToken);
        else
            throw new ObjectDisposedException(nameof(BattleStream));
    }

    /// <summary>
    /// Reads the next output message from the battle stream.
    /// </summary>
    public async Task<string?> ReadAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(BattleStream));
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
            await foreach (string chunk in _inputChannel.Reader.ReadAllAsync(cancellationToken))
            {
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
                Battle?.SendUpdates();
            }

            // Input completed, signal end if needed
            if (!KeepAlive)
            {
                await PushEndAsync();
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
        catch (Exception ex)
        {
            await PushErrorAsync(ex);
        }
        finally
        {
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

                    Battle = new BattleAsync(options, Library);
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
            case "p3":
            case "p4":
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
                    Battle?.Add("chat", message);
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
                throw new InvalidOperationException($"Unrecognized command \">{type} {message}\"");
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
        // TODO: Implement full choice parsing
        // This is a placeholder - you'll need to parse the choice format
        // Examples: "move 1", "switch 2", "team 1234"
        throw new NotImplementedException("Choice parsing not yet implemented");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _cancellationTokenSource.Cancel();
            _inputChannel.Writer.Complete();

            try
            {
                _processingTask?.Wait(TimeSpan.FromSeconds(5));
            }
            catch
            {
                // Ignore timeout
            }

            Battle?.Dispose();
            _cancellationTokenSource.Dispose();
        }

        _disposed = true;
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

/// <summary>
/// Abstract base class for battle players that interact with a BattleStream.
/// </summary>
public abstract class BattlePlayer(BattleStream stream, bool debug = false)
{
    protected readonly BattleStream Stream = stream ?? throw new ArgumentNullException(nameof(stream));
    protected readonly List<string> Log = [];
    protected readonly bool Debug = debug;

    /// <summary>
    /// Starts reading from the stream and processing messages.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await foreach (string chunk in Stream.ReadAllAsync(cancellationToken))
        {
            Receive(chunk);
        }
    }

    /// <summary>
    /// Receives and processes a chunk of messages.
    /// </summary>
    public void Receive(string chunk)
    {
        foreach (string line in chunk.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            ReceiveLine(line);
        }
    }

    /// <summary>
    /// Processes a single line from the battle stream.
    /// </summary>
    public void ReceiveLine(string line)
    {
        if (Debug)
        {
            Console.WriteLine(line);
        }

        if (!line.StartsWith('|')) return;

        var parts = line[1..].Split('|', 2);
        if (parts.Length < 1) return;

        string cmd = parts[0];
        string rest = parts.Length > 1 ? parts[1] : string.Empty;

        switch (cmd)
        {
            case "request":
                {
                    var request = JsonSerializer.Deserialize<IChoiceRequest>(rest);
                    if (request != null)
                    {
                        ReceiveRequest(request);
                    }
                    return;
                }

            case "error":
                ReceiveError(new Exception(rest));
                return;
        }

        Log.Add(line);
    }

    /// <summary>
    /// Called when a choice request is received from the battle.
    /// Derived classes must implement this to provide battle choices.
    /// </summary>
    public abstract void ReceiveRequest(IChoiceRequest request);

    /// <summary>
    /// Called when an error is received from the battle.
    /// Default implementation throws the error.
    /// </summary>
    public virtual void ReceiveError(Exception error)
    {
        throw error;
    }

    /// <summary>
    /// Sends a choice to the battle stream.
    /// </summary>
    public async Task ChooseAsync(string choice, CancellationToken cancellationToken = default)
    {
        await Stream.WriteAsync(choice, cancellationToken);
    }
}

/// <summary>
/// A text-based wrapper around BattleStream that uses plain strings instead of structured messages.
/// Useful for testing or simple integrations.
/// </summary>
public class BattleTextStream : IDisposable
{
    private readonly BattleStream _battleStream;
    private readonly StringBuilder _currentMessage;
    private readonly Task _listenTask;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Channel<string> _outputChannel;
    private bool _disposed;

    public BattleTextStream(Library library, BattleStreamOptions? options = null)
    {
        _battleStream = new BattleStream(library, options);
        _currentMessage = new StringBuilder();
        _cancellationTokenSource = new CancellationTokenSource();

        _outputChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = true,
        });

        _listenTask = ListenAsync(_cancellationTokenSource.Token);
    }

    /// <summary>
    /// Reads the next message from the stream.
    /// </summary>
    public async Task<string?> ReadAsync(CancellationToken cancellationToken = default)
    {
        if (await _outputChannel.Reader.WaitToReadAsync(cancellationToken))
        {
            if (_outputChannel.Reader.TryRead(out string? message))
            {
                return message;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all messages as an async enumerable.
    /// </summary>
    public IAsyncEnumerable<string> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _outputChannel.Reader.ReadAllAsync(cancellationToken);
    }

    /// <summary>
    /// Writes a message to the battle stream.
    /// Messages are buffered until a newline is encountered.
    /// </summary>
    public async Task WriteAsync(string message, CancellationToken cancellationToken = default)
    {
        _currentMessage.Append(message);

        int lastNewlineIndex = _currentMessage.ToString().LastIndexOf('\n');
        if (lastNewlineIndex >= 0)
        {
            string toSend = _currentMessage.ToString()[..lastNewlineIndex];
            await _battleStream.WriteAsync(toSend, cancellationToken);

            _currentMessage.Clear();
            _currentMessage.Append(_currentMessage.ToString()[(lastNewlineIndex + 1)..]);
        }
    }

    /// <summary>
    /// Signals that writing is complete.
    /// </summary>
    public void CompleteWriting()
    {
        _battleStream.CompleteWriting();
    }

    private async Task ListenAsync(CancellationToken cancellationToken)
    {
        try
        {
            await foreach (string message in _battleStream.ReadAllAsync(cancellationToken))
            {
                string formattedMessage = message.EndsWith('\n') ? message : message + "\n";
                await _outputChannel.Writer.WriteAsync(formattedMessage + "\n", cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected on cancellation
        }
        finally
        {
            _outputChannel.Writer.Complete();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _cancellationTokenSource.Cancel();

            try
            {
                _listenTask.Wait(TimeSpan.FromSeconds(5));
            }
            catch
            {
                // Ignore timeout
            }

            _battleStream.Dispose();
            _cancellationTokenSource.Dispose();
        }

        _disposed = true;
    }
}