//using ApogeeVGC.Data;
//using System.Text;
//using System.Threading.Channels;

//namespace ApogeeVGC.Sim.BattleClasses;

///// <summary>
///// A text-based wrapper around BattleStream that uses plain strings instead of structured messages.
///// Useful for testing or simple integrations.
///// </summary>
//public class BattleTextStream : IDisposable
//{
//    private readonly BattleStream _battleStream;
//    private readonly StringBuilder _currentMessage;
//    private readonly Task _listenTask;
//    private readonly CancellationTokenSource _cancellationTokenSource;
//    private readonly Channel<string> _outputChannel;
//    private bool _disposed;

//    public BattleTextStream(Library library, BattleStreamOptions? options = null)
//    {
//        _battleStream = new BattleStream(library, options);
//        _currentMessage = new StringBuilder();
//        _cancellationTokenSource = new CancellationTokenSource();

//        _outputChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
//        {
//            SingleReader = false,
//            SingleWriter = true,
//        });

//        _listenTask = ListenAsync(_cancellationTokenSource.Token);
//    }

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

//    /// <summary>
//    /// Writes a message to the battle stream.
//    /// Messages are buffered until a newline is encountered.
//    /// </summary>
//    public async Task WriteAsync(string message, CancellationToken cancellationToken = default)
//    {
//        _currentMessage.Append(message);

//        int lastNewlineIndex = _currentMessage.ToString().LastIndexOf('\n');
//        if (lastNewlineIndex >= 0)
//        {
//            string toSend = _currentMessage.ToString()[..lastNewlineIndex];
//            await _battleStream.WriteAsync(toSend, cancellationToken);

//            _currentMessage.Clear();
//            _currentMessage.Append(_currentMessage.ToString()[(lastNewlineIndex + 1)..]);
//        }
//    }

//    /// <summary>
//    /// Signals that writing is complete.
//    /// </summary>
//    public void CompleteWriting()
//    {
//        _battleStream.CompleteWriting();
//    }

//    private async Task ListenAsync(CancellationToken cancellationToken)
//    {
//        try
//        {
//            await foreach (string message in _battleStream.ReadAllAsync(cancellationToken))
//            {
//                string formattedMessage = message.EndsWith('\n') ? message : message + "\n";
//                await _outputChannel.Writer.WriteAsync(formattedMessage + "\n", cancellationToken);
//            }
//        }
//        catch (OperationCanceledException)
//        {
//            // Expected on cancellation
//        }
//        finally
//        {
//            _outputChannel.Writer.Complete();
//        }
//    }

//    public void Dispose()
//    {
//        Dispose(true);
//        GC.SuppressFinalize(this);
//    }

//    protected virtual void Dispose(bool disposing)
//    {
//        if (_disposed) return;

//        if (disposing)
//        {
//            _cancellationTokenSource.Cancel();

//            try
//            {
//                _listenTask.Wait(TimeSpan.FromSeconds(5));
//            }
//            catch
//            {
//                // Ignore timeout
//            }

//            _battleStream.Dispose();
//            _cancellationTokenSource.Dispose();
//        }

//        _disposed = true;
//    }
//}