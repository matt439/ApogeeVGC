//using ApogeeVGC.Sim.BattleClasses;
//using System.Threading.Channels;

//namespace ApogeeVGC.Sim.Player;

///// <summary>
///// Container for player-specific battle streams.
///// </summary>
//public class PlayerStreams : IDisposable
//{
//    /// <summary>
//    /// Stream with all information (omniscient view).
//    /// </summary>
//    public PlayerReadWriteStream Omniscient { get; }

//    /// <summary>
//    /// Stream with public information only (spectator view).
//    /// </summary>
//    public PlayerReadStream Spectator { get; }

//    /// <summary>
//    /// Stream for player 1.
//    /// </summary>
//    public PlayerReadWriteStream P1 { get; }

//    /// <summary>
//    /// Stream for player 2.
//    /// </summary>
//    public PlayerReadWriteStream P2 { get; }

//    internal PlayerStreams(
//        PlayerReadWriteStream omniscient,
//        PlayerReadStream spectator,
//        PlayerReadWriteStream p1,
//        PlayerReadWriteStream p2)
//    {
//        Omniscient = omniscient;
//        Spectator = spectator;
//        P1 = p1;
//        P2 = p2;
//    }

//    public void Dispose()
//    {
//        Omniscient.Dispose();
//        Spectator.Dispose();
//        P1.Dispose();
//        P2.Dispose();
//    }
//}

///// <summary>
///// A read-only stream for a specific player perspective.
///// </summary>
//public class PlayerReadStream : IDisposable
//{
//    private readonly Channel<string> _outputChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
//    {
//        SingleReader = false,
//        SingleWriter = true,
//    });
//    private bool _disposed;

//    internal Channel<string> OutputChannel => _outputChannel;

//    /// <summary>
//    /// Reads the next message from the stream.
//    /// </summary>
//    public async Task<string?> ReadAsync(CancellationToken cancellationToken = default)
//    {
//        if (await _outputChannel.Reader.WaitToReadAsync(cancellationToken))
//        {
//            if (_outputChannel.Reader.TryRead(out string? message))
//            {
//                return message;
//            }
//        }
//        return null;
//    }

//    /// <summary>
//    /// Gets all messages as an async enumerable.
//    /// </summary>
//    public IAsyncEnumerable<string> ReadAllAsync(CancellationToken cancellationToken = default)
//    {
//        return _outputChannel.Reader.ReadAllAsync(cancellationToken);
//    }

//    internal async Task PushAsync(string message)
//    {
//        await _outputChannel.Writer.WriteAsync(message);
//    }

//    internal void PushEnd()
//    {
//        _outputChannel.Writer.Complete();
//    }

//    internal void PushError(Exception exception, bool end = false)
//    {
//        if (end)
//        {
//            _outputChannel.Writer.Complete(exception);
//        }
//    }

//    public void Dispose()
//    {
//        if (_disposed) return;
//        try
//        {
//            _outputChannel.Writer.Complete();
//        }
//        catch (ChannelClosedException)
//        {
//            // Channel already completed by background task - safe to ignore
//        }
//        _disposed = true;
//    }
//}

///// <summary>
///// A read-write stream for a specific player perspective.
///// </summary>
//public class PlayerReadWriteStream : PlayerReadStream
//{
//    private readonly BattleStream _battleStream;
//    private readonly string? _playerPrefix;

//    internal PlayerReadWriteStream(BattleStream battleStream, string? playerPrefix = null)
//    {
//        _battleStream = battleStream;
//        _playerPrefix = playerPrefix;
//    }

//    /// <summary>
//    /// Writes a message to the battle stream.
//    /// </summary>
//    public async Task WriteAsync(string data, CancellationToken cancellationToken = default)
//    {
//        string message = data;
//        if (!string.IsNullOrEmpty(_playerPrefix))
//        {
//            // Add player prefix to each line
//            message = System.Text.RegularExpressions.Regex.Replace(
//                data,
//                @"(^|\n)",
//                $"$1>{_playerPrefix} ");
//        }

//        await _battleStream.WriteAsync(message, cancellationToken);
//    }

//    /// <summary>
//    /// Signals that writing is complete.
//    /// </summary>
//    public void CompleteWriting()
//    {
//        _battleStream.CompleteWriting();
//    }
//}