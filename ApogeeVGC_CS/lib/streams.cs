using System.Reflection;
using ApogeeVGC_CS.sim;
using System.Runtime.InteropServices.JavaScript;
using static ApogeeVGC_CS.lib.ReadStream;

namespace ApogeeVGC_CS.lib
{
    public enum BufferEncoding
    {
        Ascii,
        Utf8,
        Utf8Dash, // for 'utf-8'
        Utf16Le,
        Ucs2,
        Ucs2Dash, // for 'ucs-2'
        Base64,
        Latin1,
        Binary,
        Hex
    }

    public class ReadStream
    {
        public static int BufSizeConst => 65535 * 4; // 256 KiB

        public Buffer Buf { get; }
        public int BufStart { get; }
        public int BufEnd { get; }
        public int BufCapacity { get; }
        public int ReadSize { get; }
        public bool AtEof { get; set; }
        public List<Exception>? ErrorBuf { get; }
        public BufferEncoding Encoding { get; }
        public bool IsReadable { get; }
        public bool IsWritable { get; }
        public Stream? NetStream { get; }
        public TaskCompletionSource<bool>? NextPushResolver { get; }
        public Task NextPush { get; }
        public bool AwaitingPush { get; }

        // Delegate types for customizable behavior
        public delegate Task ReadDelegate(ReadStream stream, int bytes);

        public delegate Task PauseDelegate(ReadStream stream, int bytes);

        public delegate Task DestroyDelegate(ReadStream stream);

        //public ReadDelegate? Read { get; }
        //public PauseDelegate? Pause { get; }
        //public DestroyDelegate? Destroy { get; }

        public ReadStream(object? optionsOrStreamLike = null)
        {
            // Initialize buffer properties (equivalent to TypeScript initialization)
            Buf = Buffer.AllocUnsafe(BufSizeConst);
            BufStart = 0;
            BufEnd = 0;
            BufCapacity = BufSizeConst; // Use the constant instead of BufSize property
            ReadSize = 0;
            AtEof = false;
            ErrorBuf = null;
            Encoding = BufferEncoding.Utf8;
            IsReadable = true;
            IsWritable = false;
            NetStream = null;
            NextPushResolver = new TaskCompletionSource<bool>();
            NextPush = NextPushResolver.Task;
            AwaitingPush = false;

            // Process different input types to create options dictionary
            Dictionary<string, object> options = new();

            if (optionsOrStreamLike is string stringValue)
            {
                options["buffer"] = stringValue;
            }
            else if (optionsOrStreamLike is byte[] byteArray)
            {
                options["buffer"] = byteArray;
            }
            else if (optionsOrStreamLike is Buffer bufferValue)
            {
                options["buffer"] = bufferValue;
            }
            else if (optionsOrStreamLike is Stream stream && HasReadableState(stream))
            {
                options["nodeStream"] = stream;
            }
            else if (optionsOrStreamLike is Dictionary<string, object> optionsDict)
            {
                options = optionsDict;
            }
            else if (optionsOrStreamLike != null)
            {
                // Try to convert other objects to options dictionary
                options = ConvertToOptionsDictionary(optionsOrStreamLike);
            }

            // Handle Node.js stream integration
            if (options.TryGetValue("nodeStream", out object? nodeStreamObj) && nodeStreamObj is Stream nodeStream)
            {
                NetStream = nodeStream;
                SetupStreamHandling(nodeStream, options);
            }

            // Apply custom handlers if provided
            ApplyCustomHandlers(options);

            // Handle initial buffer data
            if (options.TryGetValue("buffer", out object? bufferObj))
            {
                PushInitialBuffer(bufferObj);
            }
        }

        #region Helper Methods for Constructor

        /// <summary>
        /// Checks if a stream has readable state (similar to Node.js _readableState check)
        /// </summary>
        private static bool HasReadableState(Stream stream)
        {
            // Check if it's a readable stream
            return stream.CanRead;
        }

        /// <summary>
        /// Sets up event handling for .NET Stream (equivalent to Node.js stream event handlers)
        /// </summary>
        private void SetupStreamHandling(Stream nodeStream, Dictionary<string, object> options)
        {
            // Set up async reading from the stream (equivalent to 'data' and 'end' events)
            _ = Task.Run(async () =>
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    while (true)
                    {
                        int bytesRead = await nodeStream.ReadAsync(buffer);
                        if (bytesRead == 0)
                        {
                            // Equivalent to 'end' event
                            PushEnd();
                            break;
                        }

                        // Equivalent to 'data' event
                        byte[] data = new byte[bytesRead];
                        Array.Copy(buffer, data, bytesRead);
                        Push(Buffer.From(data), Encoding);
                    }
                }
                catch (Exception ex)
                {
                    PushError(ex);
                }
            });

            // Set up default read function (equivalent to nodeStream.resume())
            if (!options.ContainsKey("read"))
            {
                options["read"] = new ReadDelegate(async (_, _) =>
                {
                    // In .NET, we don't need to explicitly resume streams
                    // The async reading loop handles this automatically
                    await Task.CompletedTask;
                });
            }

            // Set up default pause function (equivalent to nodeStream.pause())
            if (!options.ContainsKey("pause"))
            {
                options["pause"] = new PauseDelegate(async (_, _) =>
                {
                    // In .NET, pausing would involve cancellation tokens or other mechanisms
                    // For now, this is a placeholder
                    await Task.CompletedTask;
                });
            }
        }

        /// <summary>
        /// Applies custom read, pause, destroy handlers from options
        /// </summary>
        private static void ApplyCustomHandlers(Dictionary<string, object> options)
        {
            // Note: Since the fields are readonly, we'd need to modify the class design
            // to store these handlers. For now, this shows the pattern.

            if (options.TryGetValue("read", out object? readObj) && readObj is ReadDelegate readDelegate)
            {
                // Would assign to _read field if it existed
                // this._read = readDelegate;
            }

            if (options.TryGetValue("pause", out object? pauseObj) && pauseObj is PauseDelegate pauseDelegate)
            {
                // Would assign to _pause field if it existed
                // this._pause = pauseDelegate;
            }

            if (options.TryGetValue("destroy", out object? destroyObj) && destroyObj is DestroyDelegate destroyDelegate)
            {
                // Would assign to _destroy field if it existed
                // this._destroy = destroyDelegate;
            }

            if (options.TryGetValue("encoding", out object? encodingObj) && encodingObj is BufferEncoding encoding)
            {
                // Would need to make Encoding mutable
                // this.Encoding = encoding;
            }
        }

        /// <summary>
        /// Pushes initial buffer data and ends the stream
        /// </summary>
        private void PushInitialBuffer(object bufferObj)
        {
            switch (bufferObj)
            {
                case string str:
                    Push(str, Encoding);
                    break;
                case byte[] bytes:
                    Push(Buffer.From(bytes), Encoding);
                    break;
                case Buffer buffer:
                    Push(buffer, Encoding);
                    break;
            }

            PushEnd();
        }

        /// <summary>
        /// Converts arbitrary objects to options dictionary using reflection
        /// </summary>
        private static Dictionary<string, object> ConvertToOptionsDictionary(object obj)
        {
            Dictionary<string, object> options = new Dictionary<string, object>();

            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                object? value = prop.GetValue(obj);
                if (value != null)
                {
                    options[prop.Name.ToLowerInvariant()] = value;
                }
            }

            return options;
        }

        #endregion

        public int BufSize => BufEnd - BufStart;

        public void MoveBuf()
        {
            throw new NotImplementedException("MoveBuf method is not implemented yet.");
        }

        public void ExpandBuf(int newCapacity)
        {
            throw new NotImplementedException("ExpandBuf method is not implemented yet.");
        }

        public void EnsureCapacity(int additionalCapacity)
        {
            throw new NotImplementedException("EnsureCapacity method is not implemented yet.");
        }

        public void Push(Buffer buf, BufferEncoding encoding)
        {
            throw new NotImplementedException("Push method is not implemented yet.");
        }

        public void Push(string buf, BufferEncoding encoding)
        {
            throw new NotImplementedException("Push method is not implemented yet.");
        }

        public void PushEnd()
        {
            throw new NotImplementedException("PushEnd method is not implemented yet.");
        }

        public void PushError(JSType.Error err, bool? recoverable = false)
        {
            throw new NotImplementedException("PushError method is not implemented yet.");
        }

        public void PushError(Exception exception, bool? recoverable = false)
        {
            throw new NotImplementedException("PushError method is not implemented yet.");
        }

        public void ReadError()
        {
            throw new NotImplementedException("ReadError method is not implemented yet.");
        }

        public void PeakError()
        {
            throw new NotImplementedException("PeakError method is not implemented yet.");
        }

        public void ResolvePush()
        {
            throw new NotImplementedException("ResolvePush method is not implemented yet.");
        }

        /// <summary>
        /// Abstract method that subclasses must implement to handle reading data.
        /// Should be overridden to provide the actual reading logic.
        /// </summary>
        /// <param name="size">Number of bytes to read (0 means read available data)</param>
        /// <returns>Task representing the async read operation</returns>
        protected virtual Task ReadInternalAsync(int size = 0)
        {
            throw new NotImplementedException(
                "ReadStream needs to be subclassed and the ReadInternalAsync method needs to be implemented.");
        }

        /// <summary>
        /// Virtual method for cleanup operations. Can be overridden by subclasses.
        /// </summary>
        /// <returns>Task representing the async destroy operation</returns>
        protected virtual Task DestroyInternalAsync()
        {
            // Default empty implementation - subclasses can override for cleanup
            return Task.CompletedTask;
        }

        /// <summary>
        /// Virtual method for pausing stream operations. Can be overridden by subclasses.
        /// </summary>
        protected virtual void PauseInternal()
        {
            // Default empty implementation - subclasses can override for pause logic
        }

        /// <summary>
        /// Reads until the internal buffer is non-empty. Does nothing if the
        /// internal buffer is already non-empty.
        ///
        /// If `byteCount` is a number, instead read until the internal buffer
        /// contains at least `byteCount` bytes.
        ///
        /// If `byteCount` is `true`, reads even if the internal buffer is non-empty.
        /// </summary>
        public Task LoadIntoBuffer(IntTrueUnion? byteCount = null, bool? readError = false)
        {
            throw new NotImplementedException("LoadIntoBuffer method is not implemented yet.");
        }

        public Task DoLoad(int? chunkSize = null, bool? readError = false)
        {
            throw new NotImplementedException("DoLoad method is not implemented yet.");
        }

        public StringTaskUnion? Peek(int? byteCount = null, BufferEncoding? encoding = null)
        {
            throw new NotImplementedException("Peek method is not implemented yet.");
        }

        public StringTaskUnion? Peek(BufferEncoding encoding)
        {
            throw new NotImplementedException("Peek method with encoding is not implemented yet.");
        }

        public StringTaskUnion? Peek(BufferEncoding encoding, StringIntUnion byteCount)
        {
            throw new NotImplementedException("Peek method without parameters is not implemented yet.");
        }

        public BufferTaskBuffer? PeekBuffer(int? byteCount = null)
        {
            throw new NotImplementedException("PeekBuffer method is not implemented yet.");
        }

        public Task<string?> Read(int? byteCount = null, BufferEncoding? encoding = null)
        {
            throw new NotImplementedException("Read method is not implemented yet.");
        }

        public Task<string?> Read(BufferEncoding encoding)
        {
            throw new NotImplementedException("Read method with encoding is not implemented yet.");
        }

        public Task<string?> Read(BufferEncoding encoding, StringIntUnion? byteCount)
        {
            throw new NotImplementedException("Read method with StringIntUnion is not implemented yet.");
        }


        // TODO: ByChunk, ByLine, DelimitedBy


        public Task<Buffer?> ReadBuffer(int? byteCount = null)
        {
            throw new NotImplementedException("ReadBuffer method is not implemented yet.");
        }

        public Task<int> IndexOf(string symbol, BufferEncoding encoding)
        {
            throw new NotImplementedException("IndexOf method is not implemented yet.");
        }

        public Task<string> ReadAll(BufferEncoding encoding)
        {
            throw new NotImplementedException("ReadAll method is not implemented yet.");
        }

        public StringTaskUnion? PeekAll(BufferEncoding encoding)
        {
            throw new NotImplementedException("PeekAll method is not implemented yet.");
        }

        public Task<string?> ReadDelimitedBy(string symbol, BufferEncoding encoding)
        {
            throw new NotImplementedException("ReadDelimitedBy method is not implemented yet.");
        }

        public Task<string?> ReadLine(BufferEncoding encoding)
        {
            throw new NotImplementedException("ReadLine method is not implemented yet.");
        }

        public Task Destroy()
        {
            throw new NotImplementedException("Destroy method is not implemented yet.");
        }

        /// <summary>
        /// Iterator method that reads the next chunk of data from the stream.
        /// Returns a result indicating whether data was available and the value read.
        /// </summary>
        /// <param name="byteCount">Number of bytes to read, or null to read available data</param>
        /// <returns>Task containing an IteratorResult with the read value and completion status</returns>
        public async Task<IteratorResult<string>> NextAsync(int? byteCount = null)
        {
            string? value = await Read(byteCount);
            return new IteratorResult<string>(value, value == null);
        }

        /// <summary>
        /// Represents the result of an iterator operation, similar to JavaScript's iterator protocol.
        /// </summary>
        /// <typeparam name="T">The type of value being iterated</typeparam>
        public readonly struct IteratorResult<T>
        {
            /// <summary>
            /// The value returned by the iterator, or default(T) if done is true.
            /// </summary>
            public T? Value { get; }

            /// <summary>
            /// Indicates whether the iterator has been exhausted.
            /// </summary>
            public bool Done { get; }

            /// <summary>
            /// Creates a new IteratorResult.
            /// </summary>
            /// <param name="value">The value to return</param>
            /// <param name="done">Whether the iterator is exhausted</param>
            public IteratorResult(T? value, bool done)
            {
                Value = value;
                Done = done;
            }

            /// <summary>
            /// Creates a result indicating the iterator has a value and is not done.
            /// </summary>
            public static IteratorResult<T> Yield(T value) => new(value, false);

            /// <summary>
            /// Creates a result indicating the iterator is exhausted.
            /// </summary>
            public static IteratorResult<T> Complete() => new(default, true);
        }

        /// <summary>
        /// Pipes data from this ReadStream to the specified WriteStream.
        /// Reads chunks using the iterator pattern and writes them to the output stream.
        /// </summary>
        /// <param name="outStream">The WriteStream to pipe data to</param>
        /// <param name="options">Optional configuration for the pipe operation</param>
        /// <returns>Task representing the async pipe operation</returns>
        public async Task PipeToAsync(WriteStream outStream, PipeToOptions? options = null)
        {
            options ??= new PipeToOptions();

            IteratorResult<string> next;
            while (!(next = await NextAsync()).Done)
            {
                await outStream.WriteAsync(next.Value!);
            }

            if (!options.NoEnd)
            {
                await outStream.WriteEndAsync();
            }
        }

        /// <summary>
        /// Configuration options for the PipeTo operation.
        /// </summary>
        public class PipeToOptions
        {
            /// <summary>
            /// If true, does not end the output stream when the input stream is exhausted.
            /// Default is false.
            /// </summary>
            public bool NoEnd { get; set; } = false;
        }
    }


    /// <summary>
    /// Configuration options for WriteStream initialization.
    /// Allows customization of write behavior and stream sources.
    /// </summary>
    public class WriteStreamOptions
    {
        /// <summary>
        /// Optional .NET Stream to wrap as the underlying writable stream.
        /// </summary>
        public Stream? NetStream { get; set; }

        /// <summary>
        /// Optional custom write function that handles data writing.
        /// </summary>
        public WriteDelegate? Write { get; set; }

        /// <summary>
        /// Optional custom write end function that handles stream finalization.
        /// </summary>
        public WriteEndDelegate? WriteEnd { get; set; }
    }

    /// <summary>
    /// Delegate for custom write operations.
    /// </summary>
    /// <param name="writeStream">The WriteStream instance</param>
    /// <param name="data">The data to write (string or Buffer)</param>
    /// <returns>Task representing the async write operation</returns>
    public delegate Task WriteDelegate(WriteStream writeStream, object data);

    /// <summary>
    /// Delegate for custom write end operations.
    /// </summary>
    /// <param name="writeStream">The WriteStream instance</param>
    /// <returns>Task representing the async end operation</returns>
    public delegate Task WriteEndDelegate(WriteStream writeStream);

    public class WriteStream
    {
        private readonly WriteDelegate? _customWrite;
        private readonly WriteEndDelegate? _customWriteEnd;
        private readonly List<TaskCompletionSource> _drainWaiters;

        public bool IsReadable { get; }
        public bool IsWritable { get; }
        public BufferEncoding Encoding { get; set; }
        public Stream? NetWritableStream { get; }

        public WriteStream(object? optionsOrStream = null)
        {
            IsReadable = false;
            IsWritable = true;
            Encoding = BufferEncoding.Utf8;
            NetWritableStream = null;
            _drainWaiters = [];

            // Process options
            WriteStreamOptions options;

            if (optionsOrStream is Stream stream)
            {
                // Direct stream provided
                options = new WriteStreamOptions { NetStream = stream };
            }
            else if (optionsOrStream is WriteStreamOptions opts)
            {
                options = opts;
            }
            else
            {
                options = new WriteStreamOptions();
            }

            if (options.NetStream != null)
            {
                NetWritableStream = options.NetStream;

                // Set up default write behavior with backpressure handling
                options.Write ??= async (_, data) =>
                {
                    byte[] bytes = ConvertToBytes(data, Encoding);

                    await NetWritableStream.WriteAsync(bytes);

                    // Simulate backpressure check (in real scenarios, you'd check stream state)
                    if (ShouldApplyBackpressure())
                    {
                        await WaitForDrainAsync();
                    }
                };

                // Set up default writeEnd behavior
                // Special handling for standard streams (don't close them)
                if (!IsStandardStream(NetWritableStream))
                {
                    options.WriteEnd ??= async (_) =>
                    {
                        await NetWritableStream.FlushAsync();

                        // Use TaskCompletionSource to handle async completion
                        TaskCompletionSource tcs = new TaskCompletionSource();

                        try
                        {
                            NetWritableStream.Close();
                            tcs.SetResult();
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }

                        await tcs.Task;
                    };
                }
            }

            // Apply custom handlers if provided
            _customWrite = options.Write;
            _customWriteEnd = options.WriteEnd;
        }

        /// <summary>
        /// Writes data to the stream.
        /// </summary>
        /// <param name="chunk">Data to write (Buffer or string)</param>
        /// <returns>Task representing the async write operation</returns>
        public async Task WriteAsync(object chunk)
        {
            if (_customWrite != null)
            {
                await _customWrite(this, chunk);
            }
            else
            {
                await WriteInternalAsync(chunk);
            }
        }

        /// <summary>
        /// Writes a line of text to the stream (adds newline).
        /// </summary>
        /// <param name="chunk">Text to write, or null to end the stream</param>
        /// <returns>Task representing the async write operation</returns>
        public async Task WriteLineAsync(string? chunk)
        {
            if (chunk == null)
            {
                await WriteEndAsync();
                return;
            }

            await WriteAsync(chunk + "\n");
        }

        /// <summary>
        /// Virtual method for custom write implementation.
        /// Should be overridden by subclasses to provide actual write logic.
        /// </summary>
        /// <param name="chunk">Data to write</param>
        /// <returns>Task representing the async write operation</returns>
        protected virtual Task WriteInternalAsync(object chunk)
        {
            throw new NotImplementedException(
                "WriteStream needs to be subclassed and the WriteInternalAsync method needs to be implemented.");
        }

        /// <summary>
        /// Virtual method for custom writeEnd implementation.
        /// Can be overridden by subclasses for cleanup logic.
        /// </summary>
        /// <returns>Task representing the async end operation</returns>
        protected virtual Task WriteEndInternalAsync()
        {
            // Default empty implementation
            return Task.CompletedTask;
        }

        /// <summary>
        /// Ends the stream, optionally writing a final chunk.
        /// </summary>
        /// <param name="chunk">Optional final chunk to write before ending</param>
        /// <returns>Task representing the async end operation</returns>
        public async Task WriteEndAsync(string? chunk = null)
        {
            if (chunk != null)
            {
                await WriteAsync(chunk);
            }

            if (_customWriteEnd != null)
            {
                await _customWriteEnd(this);
            }
            else
            {
                await WriteEndInternalAsync();
            }
        }

        #region Helper Methods

        private static byte[] ConvertToBytes(object data, BufferEncoding encoding)
        {
            return data switch
            {
                string str => GetEncodingFromBufferEncoding(encoding).GetBytes(str),
                Buffer buffer => buffer.ToArray(),
                byte[] bytes => bytes,
                _ => throw new ArgumentException($"Unsupported data type: {data.GetType()}")
            };
        }

        private static System.Text.Encoding GetEncodingFromBufferEncoding(BufferEncoding encoding)
        {
            return encoding switch
            {
                BufferEncoding.Ascii => System.Text.Encoding.ASCII,
                BufferEncoding.Utf8 or BufferEncoding.Utf8Dash => System.Text.Encoding.UTF8,
                BufferEncoding.Utf16Le => System.Text.Encoding.Unicode,
                BufferEncoding.Ucs2 or BufferEncoding.Ucs2Dash => System.Text.Encoding.Unicode,
                BufferEncoding.Latin1 or BufferEncoding.Binary => System.Text.Encoding.GetEncoding("iso-8859-1"),
                _ => throw new ArgumentException($"Unsupported encoding: {encoding}")
            };
        }

        private bool ShouldApplyBackpressure()
        {
            // In a real implementation, you'd check if the underlying stream's buffer is full
            // For now, this is a placeholder
            return false;
        }

        private async Task WaitForDrainAsync()
        {
            TaskCompletionSource tcs = new TaskCompletionSource();
            _drainWaiters.Add(tcs);

            // In a real implementation, you'd set up an event handler for when the stream can accept more data
            // For now, we'll simulate immediate drain
            _ = Task.Run(async () =>
            {
                await Task.Delay(10); // Simulate brief wait
                ResolveDrainWaiters();
            });

            await tcs.Task;
        }

        private void ResolveDrainWaiters()
        {
            foreach (TaskCompletionSource waiter in _drainWaiters)
            {
                waiter.SetResult();
            }

            _drainWaiters.Clear();
        }

        private static bool IsStandardStream(Stream stream)
        {
            // Check if this is a standard stream (stdout, stderr, stdin)
            // This is a simplified check - in practice you might compare against Console streams
            return stream == Console.OpenStandardOutput() ||
                   stream == Console.OpenStandardError() ||
                   stream == Console.OpenStandardInput();
        }

        #endregion
    }



    /// <summary>
    /// A duplex stream that supports both reading and writing operations.
    /// Extends ReadStream and provides WriteStream functionality.
    /// </summary>
    public class ReadWriteStream : ReadStream
    {
        private readonly WriteDelegate? _customWrite;
        private readonly WriteEndDelegate? _customWriteEnd;
        private readonly List<TaskCompletionSource> _drainWaiters;

        // Override to ensure both are true for duplex streams
        public new bool IsReadable { get; } = true;
        public new bool IsWritable { get; } = true;

        public Stream? NodeWritableStream { get; private set; }

        /// <summary>
        /// Creates a new ReadWriteStream (duplex stream).
        /// </summary>
        /// <param name="options">Configuration options for the duplex stream</param>
        public ReadWriteStream(Dictionary<string, object>? options = null) : base(options)
        {
            _drainWaiters = new List<TaskCompletionSource>();
            options ??= new Dictionary<string, object>();

            if (options.TryGetValue("nodeStream", out object? streamObj) && streamObj is Stream nodeStream)
            {
                NodeWritableStream = nodeStream;

                // Set up write behavior with backpressure handling
                WriteDelegate writeFunc = new WriteDelegate(async (_, data) =>
                {
                    byte[] bytes = ConvertToBytes(data, Encoding);

                    await NodeWritableStream.WriteAsync(bytes);

                    // Simulate backpressure check
                    if (ShouldApplyBackpressure())
                    {
                        await WaitForDrainAsync();
                    }
                });

                // Set up writeEnd behavior for non-standard streams
                WriteEndDelegate? writeEndFunc = null;
                if (!IsStandardStream(nodeStream))
                {
                    writeEndFunc = async (_) =>
                    {
                        await NodeWritableStream.FlushAsync();

                        TaskCompletionSource tcs = new TaskCompletionSource();
                        try
                        {
                            NodeWritableStream.Close();
                            tcs.SetResult();
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }

                        await tcs.Task;
                    };
                }

                _customWrite = writeFunc;
                _customWriteEnd = writeEndFunc;
            }

            // Apply custom handlers if provided in options
            if (options.TryGetValue("write", out object? writeObj) && writeObj is WriteDelegate writeDelegate)
                _customWrite = writeDelegate;

            if (options.TryGetValue("writeEnd", out object? writeEndObj) &&
                writeEndObj is WriteEndDelegate writeEndDelegate)
                _customWriteEnd = writeEndDelegate;
        }

        #region Write Operations

        /// <summary>
        /// Writes data to the stream.
        /// </summary>
        /// <param name="chunk">Data to write (Buffer or string)</param>
        /// <returns>Task representing the async write operation</returns>
        public async Task WriteAsync(object chunk)
        {
            if (_customWrite != null)
            {
                await _customWrite(new WriteStream(this), chunk);
            }
            else
            {
                await WriteInternalAsync(chunk);
            }
        }

        /// <summary>
        /// Writes a line of text to the stream (adds newline).
        /// </summary>
        /// <param name="chunk">Text to write</param>
        /// <returns>Task representing the async write operation</returns>
        public async Task WriteLineAsync(string chunk)
        {
            await WriteAsync(chunk + '\n');
        }

        /// <summary>
        /// Virtual method for custom write implementation.
        /// Should be overridden by subclasses to provide actual write logic.
        /// </summary>
        /// <param name="chunk">Data to write</param>
        /// <returns>Task representing the async write operation</returns>
        protected virtual Task WriteInternalAsync(object chunk)
        {
            throw new NotImplementedException(
                "ReadWriteStream needs to be subclassed and the WriteInternalAsync method needs to be implemented.");
        }

        /// <summary>
        /// In a ReadWriteStream, ReadInternalAsync does not need to be implemented,
        /// because it's valid for the read stream buffer to be filled only by WriteInternalAsync.
        /// </summary>
        /// <param name="size">Number of bytes to read (ignored in duplex streams)</param>
        /// <returns>Completed task</returns>
        protected override Task ReadInternalAsync(int size = 0)
        {
            // In duplex streams, reading can be fulfilled by writes, so no implementation needed
            return Task.CompletedTask;
        }

        /// <summary>
        /// Virtual method for custom writeEnd implementation.
        /// </summary>
        /// <returns>Task representing the async end operation</returns>
        protected virtual Task WriteEndInternalAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Ends the write side of the stream.
        /// </summary>
        /// <returns>Task representing the async end operation</returns>
        public async Task WriteEndAsync()
        {
            if (_customWriteEnd != null)
            {
                await _customWriteEnd(new WriteStream(this));
            }
            else
            {
                await WriteEndInternalAsync();
            }
        }

        #endregion

        #region Helper Methods

        private static byte[] ConvertToBytes(object data, BufferEncoding encoding)
        {
            return data switch
            {
                string str => GetEncodingFromBufferEncoding(encoding).GetBytes(str),
                Buffer buffer => buffer.ToArray(),
                byte[] bytes => bytes,
                _ => throw new ArgumentException($"Unsupported data type: {data.GetType()}")
            };
        }

        private static System.Text.Encoding GetEncodingFromBufferEncoding(BufferEncoding encoding)
        {
            return encoding switch
            {
                BufferEncoding.Ascii => System.Text.Encoding.ASCII,
                BufferEncoding.Utf8 or BufferEncoding.Utf8Dash => System.Text.Encoding.UTF8,
                BufferEncoding.Utf16Le => System.Text.Encoding.Unicode,
                BufferEncoding.Ucs2 or BufferEncoding.Ucs2Dash => System.Text.Encoding.Unicode,
                BufferEncoding.Latin1 or BufferEncoding.Binary => System.Text.Encoding.GetEncoding("iso-8859-1"),
                _ => throw new ArgumentException($"Unsupported encoding: {encoding}")
            };
        }

        private bool ShouldApplyBackpressure()
        {
            // In a real implementation, you'd check if the underlying stream's buffer is full
            // For now, this is a placeholder
            return false;
        }

        private async Task WaitForDrainAsync()
        {
            TaskCompletionSource tcs = new TaskCompletionSource();
            _drainWaiters.Add(tcs);

            // Simulate drain handling
            _ = Task.Run(async () =>
            {
                await Task.Delay(10); // Simulate brief wait
                ResolveDrainWaiters();
            });

            await tcs.Task;
        }

        private void ResolveDrainWaiters()
        {
            foreach (TaskCompletionSource waiter in _drainWaiters)
            {
                waiter.SetResult();
            }

            _drainWaiters.Clear();
        }

        private static bool IsStandardStream(Stream stream)
        {
            return stream == Console.OpenStandardOutput() ||
                   stream == Console.OpenStandardError() ||
                   stream == Console.OpenStandardInput();
        }

        #endregion
    }

    /// <summary>
    /// Base class for ObjectReadStream configuration options.
    /// </summary>
    /// <typeparam name="T">The type of objects in the stream</typeparam>
    public abstract class ObjectReadStreamOptions<T>
    {
    }

    /// <summary>
    /// Configuration for manually controlled ObjectReadStream.
    /// Provides buffer data and custom behavior functions.
    /// </summary>
    /// <typeparam name="T">The type of objects in the stream</typeparam>
    public class ManualObjectReadStreamOptions<T> : ObjectReadStreamOptions<T>
    {
        /// <summary>
        /// Optional initial buffer of objects.
        /// </summary>
        public T[]? Buffer { get; set; }

        /// <summary>
        /// Optional custom read function.
        /// </summary>
        public Func<ObjectReadStream<T>, Task>? Read { get; set; }

        /// <summary>
        /// Optional custom pause function.
        /// </summary>
        public Func<ObjectReadStream<T>, Task>? Pause { get; set; }

        /// <summary>
        /// Optional custom destroy function.
        /// </summary>
        public Func<ObjectReadStream<T>, Task>? Destroy { get; set; }
    }

    /// <summary>
    /// Configuration for ObjectReadStream that wraps a .NET Stream.
    /// </summary>
    /// <typeparam name="T">The type of objects in the stream</typeparam>
    public class StreamObjectReadStreamOptions<T> : ObjectReadStreamOptions<T>
    {
        /// <summary>
        /// The underlying .NET Stream to read from.
        /// </summary>
        public required Stream NetStream { get; set; }
    }


    /// <summary>
    /// A readable stream for typed objects that provides buffering and async iteration.
    /// </summary>
    /// <typeparam name="T">The type of objects in the stream</typeparam>
    public class ObjectReadStream<T> : IAsyncEnumerable<T>
    {
        private readonly List<T> _buffer;
        private readonly Queue<Exception> _errorBuffer;
        private Func<ObjectReadStream<T>, Task>? _customRead;
        private Func<ObjectReadStream<T>, Task>? _customPause;
        private readonly Func<ObjectReadStream<T>, Task>? _customDestroy;

        private TaskCompletionSource? _nextPushResolver;
        private Task _nextPush;
        private int _readSize;
        private bool _atEof;
        private bool _awaitingPush;

        public bool IsReadable { get; } = true;
        public bool IsWritable { get; } = false;
        public Stream? NodeReadableStream { get; private set; }

        public ObjectReadStream(List<T> buffer, Queue<Exception> errorBuffer, Task nextPush,
            object? optionsOrStreamLike = null)
        {
            _buffer = [];
            _errorBuffer = new Queue<Exception>();
            _readSize = 0;
            _atEof = false;
            _awaitingPush = false;

            _nextPushResolver = new TaskCompletionSource();
            _nextPush = _nextPushResolver.Task;

            // Process different input types
            ObjectReadStreamOptions<T> options = ProcessOptions(optionsOrStreamLike);

            switch (options)
            {
                // Handle Stream integration
                case StreamObjectReadStreamOptions<T> streamOptions:
                    NodeReadableStream = streamOptions.NetStream;
                    SetupStreamHandling(streamOptions.NetStream);
                    break;
                case ManualObjectReadStreamOptions<T> manualOptions:
                {
                    _customRead = manualOptions.Read;
                    _customPause = manualOptions.Pause;
                    _customDestroy = manualOptions.Destroy;

                    // Initialize buffer if provided
                    if (manualOptions.Buffer != null)
                    {
                        _buffer.AddRange(manualOptions.Buffer);
                        PushEnd();
                    }

                    break;
                }
            }
        }

        public ObjectReadStream(bool awaitingPush, List<T> buffer, Queue<Exception> errorBuffer, Task nextPush)
        {
            _awaitingPush = awaitingPush;
            _buffer = buffer;
            _errorBuffer = errorBuffer;
            _nextPush = nextPush;
        }

        #region Push Operations

        /// <summary>
        /// Pushes an element into the stream buffer.
        /// </summary>
        /// <param name="element">The element to push</param>
        public void Push(T element)
        {
            if (_atEof) return;

            _buffer.Add(element);

            // Apply backpressure if buffer is getting large
            if (_buffer.Count > _readSize && _buffer.Count >= 16)
            {
                _ = Task.Run(async () => await PauseInternalAsync());
            }

            ResolvePush();
        }

        /// <summary>
        /// Signals the end of the stream.
        /// </summary>
        public void PushEnd()
        {
            _atEof = true;
            ResolvePush();
        }

        /// <summary>
        /// Pushes an error into the stream.
        /// </summary>
        /// <param name="error">The error to push</param>
        /// <param name="recoverable">Whether the error is recoverable</param>
        public void PushError(Exception error, bool recoverable = false)
        {
            _errorBuffer.Enqueue(error);
            if (!recoverable) _atEof = true;
            ResolvePush();
        }

        #endregion

        #region Error Handling

        /// <summary>
        /// Reads and throws the next error from the error buffer.
        /// </summary>
        public void ReadError()
        {
            if (_errorBuffer.Count > 0)
            {
                Exception error = _errorBuffer.Dequeue();
                throw error;
            }
        }

        /// <summary>
        /// Peeks at the next error without removing it from the buffer.
        /// </summary>
        public void PeekError()
        {
            if (_errorBuffer.Count > 0)
            {
                throw _errorBuffer.Peek();
            }
        }

        #endregion

        #region Internal Operations

        private void ResolvePush()
        {
            if (_nextPushResolver == null)
                throw new InvalidOperationException("Push after end of read stream");

            _nextPushResolver.SetResult();

            if (_atEof)
            {
                _nextPushResolver = null;
                return;
            }

            _nextPushResolver = new TaskCompletionSource();
            _nextPush = _nextPushResolver.Task;
        }

        /// <summary>
        /// Virtual method for custom read implementation.
        /// </summary>
        /// <param name="size">Suggested read size</param>
        /// <returns>Task representing the read operation</returns>
        protected virtual async Task ReadInternalAsync(int size = 0)
        {
            if (_customRead != null)
            {
                await _customRead(this);
            }
            else
            {
                throw new NotImplementedException(
                    "ObjectReadStream needs to be subclassed and the ReadInternalAsync method needs to be implemented.");
            }
        }

        /// <summary>
        /// Virtual method for pausing the stream.
        /// </summary>
        protected virtual async Task PauseInternalAsync()
        {
            if (_customPause != null)
            {
                await _customPause(this);
            }
            // Default empty implementation
        }

        /// <summary>
        /// Virtual method for destroying the stream.
        /// </summary>
        protected virtual async Task DestroyInternalAsync()
        {
            if (_customDestroy != null)
            {
                await _customDestroy(this);
            }
            // Default empty implementation
        }

        #endregion

        #region Loading and Reading

        /// <summary>
        /// Loads data into the buffer until the specified count is available.
        /// </summary>
        /// <param name="count">Number of elements to load, or true to force load</param>
        /// <param name="readError">Whether to read errors or just peek at them</param>
        public async Task LoadIntoBufferAsync(object count = null!, bool readError = false)
        {
            // Handle error checking
            if (readError) ReadError();
            else PeekError();

            // Process count parameter
            int targetCount = count switch
            {
                true => _buffer.Count + 1,
                int intCount => intCount,
                _ => 1
            };

            if (_buffer.Count >= targetCount) return;

            _readSize = Math.Max(targetCount, _readSize);

            while (_errorBuffer.Count == 0 && !_atEof && _buffer.Count < _readSize)
            {
                try
                {
                    await ReadInternalAsync();
                }
                catch
                {
                    // If read fails, wait for next push
                    await _nextPush;
                }

                // Check for errors again
                if (readError) ReadError();
                else PeekError();
            }
        }

        /// <summary>
        /// Peeks at the next element without removing it.
        /// </summary>
        /// <returns>The next element or default if none available</returns>
        public async Task<T?> PeekAsync()
        {
            if (_buffer.Count > 0) return _buffer[0];

            await LoadIntoBufferAsync();
            return _buffer.Count > 0 ? _buffer[0] : default;
        }

        /// <summary>
        /// Reads and removes the next element.
        /// </summary>
        /// <returns>The next element or default if stream is exhausted</returns>
        public async Task<T?> ReadAsync()
        {
            if (_buffer.Count > 0)
            {
                T item = _buffer[0];
                _buffer.RemoveAt(0);
                return item;
            }

            await LoadIntoBufferAsync(1, true);

            if (_buffer.Count == 0) return default;

            T result = _buffer[0];
            _buffer.RemoveAt(0);
            return result;
        }

        /// <summary>
        /// Peeks at multiple elements without removing them.
        /// </summary>
        /// <param name="count">Number of elements to peek, or null for all available</param>
        /// <returns>Array of peeked elements</returns>
        public async Task<T[]> PeekArrayAsync(int? count = null)
        {
            await LoadIntoBufferAsync(count ?? 1);

            int takeCount = count ?? _buffer.Count;
            return _buffer.Take(takeCount).ToArray();
        }

        /// <summary>
        /// Reads and removes multiple elements.
        /// </summary>
        /// <param name="count">Number of elements to read, or null for all available</param>
        /// <returns>Array of read elements</returns>
        public async Task<T[]> ReadArrayAsync(int? count = null)
        {
            await LoadIntoBufferAsync(count ?? 1, true);

            int takeCount = count ?? _buffer.Count;
            T[] result = _buffer.Take(takeCount).ToArray();
            _buffer.RemoveRange(0, result.Length);

            return result;
        }

        /// <summary>
        /// Reads all remaining elements.
        /// </summary>
        /// <returns>Array of all remaining elements</returns>
        public async Task<T[]> ReadAllAsync()
        {
            await LoadIntoBufferAsync(int.MaxValue, true);

            T[] result = _buffer.ToArray();
            _buffer.Clear();

            return result;
        }

        /// <summary>
        /// Peeks at all available elements without removing them.
        /// </summary>
        /// <returns>Array of all available elements</returns>
        public async Task<T[]> PeekAllAsync()
        {
            await LoadIntoBufferAsync(int.MaxValue);
            return _buffer.ToArray();
        }

        #endregion

        #region Stream Management

        /// <summary>
        /// Destroys the stream and cleans up resources.
        /// </summary>
        public async Task DestroyAsync()
        {
            _atEof = true;
            _buffer.Clear();
            ResolvePush();
            await DestroyInternalAsync();
        }

        #endregion

        #region Async Enumerable Implementation

        /// <summary>
        /// Gets an async enumerator for the stream.
        /// </summary>
        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                T? item = await ReadAsync();
                if (item == null && _atEof) yield break;
                if (item != null) yield return item;
            }
        }

        /// <summary>
        /// Iterator method compatible with the base ReadStream pattern.
        /// </summary>
        public async Task<IteratorResult<T>> NextAsync()
        {
            if (_buffer.Count > 0)
            {
                T item = _buffer[0];
                _buffer.RemoveAt(0);
                return new IteratorResult<T>(item, false);
            }

            await LoadIntoBufferAsync(1, true);

            if (_buffer.Count == 0)
                return new IteratorResult<T>(default, true);

            T result = _buffer[0];
            _buffer.RemoveAt(0);
            return new IteratorResult<T>(result, false);
        }

        #endregion

        #region Pipe Operations

        /// <summary>
        /// Pipes this stream to an ObjectWriteStream.
        /// </summary>
        /// <param name="outStream">The output stream</param>
        /// <param name="options">Pipe options</param>
        public async Task PipeToAsync(ObjectWriteStream<T> outStream, PipeToOptions? options = null)
        {
            options ??= new PipeToOptions();

            await foreach (T item in this)
            {
                await outStream.WriteAsync(item);
            }

            if (!options.NoEnd)
            {
                await outStream.WriteEndAsync();
            }
        }

        #endregion

        #region Helper Methods

        private ObjectReadStreamOptions<T> ProcessOptions(object? optionsOrStreamLike)
        {
            return optionsOrStreamLike switch
            {
                T[] array => new ManualObjectReadStreamOptions<T> { Buffer = array },
                Stream stream => new StreamObjectReadStreamOptions<T> { NetStream = stream },
                ObjectReadStreamOptions<T> options => options,
                null => new ManualObjectReadStreamOptions<T>(),
                _ => throw new ArgumentException($"Invalid options type: {optionsOrStreamLike.GetType()}")
            };
        }

        private void SetupStreamHandling(Stream stream)
        {
            // Set up async reading from the underlying stream
            _ = Task.Run(async () =>
            {
                try
                {
                    // This would need custom deserialization logic for type T
                    // For demonstration, assuming T is string
                    if (typeof(T) == typeof(string))
                    {
                        using StreamReader reader = new StreamReader(stream);
                        while (await reader.ReadLineAsync() is { } line)
                        {
                            if (line is T item)
                                Push(item);
                        }
                    }

                    PushEnd();
                }
                catch (Exception ex)
                {
                    PushError(ex);
                }
            });

            // Set up custom handlers for stream control
            _customRead = _ => Task.CompletedTask;

            _customPause = _ => Task.CompletedTask;

            #endregion
        }

        /// <summary>
        /// Enhanced ObjectWriteStream with configurable serialization.
        /// </summary>
        /// <typeparam name="T">The type of objects to write</typeparam>
        public class ObjectWriteStream<T> : IAsyncDisposable
        {
            private readonly Func<T, byte[]> _serializer;
            private readonly Func<ObjectWriteStream<T>, T, Task>? _customWrite;
            private readonly Func<ObjectWriteStream<T>, Task>? _customWriteEnd;
            private readonly List<TaskCompletionSource> _drainWaiters;
            private bool _disposed;

            public bool IsReadable { get; } = false;
            public bool IsWritable { get; private set; } = true;
            public Stream? NodeWritableStream { get; private set; }

            /// <summary>
            /// Creates a new ObjectWriteStream with custom serialization.
            /// </summary>
            /// <param name="optionsOrStream">Configuration options or direct Stream</param>
            /// <param name="serializer">Custom serializer function</param>
            public ObjectWriteStream(object? optionsOrStream = null, Func<T, byte[]>? serializer = null)
            {
                _drainWaiters = new List<TaskCompletionSource>();
                _serializer = serializer ?? DefaultSerializer;

                // Process options
                ObjectWriteStreamOptions<T> options = ProcessOptions(optionsOrStream);

                if (options.NodeStream != null)
                {
                    NodeWritableStream = options.NodeStream;
                    SetupStreamHandling(options);
                }

                _customWrite = options.Write;
                _customWriteEnd = options.WriteEnd;
            }

            #region Write Operations

            /// <summary>
            /// Writes an element to the stream. If element is null/default, ends the stream.
            /// </summary>
            /// <param name="element">The element to write, or null to end the stream</param>
            /// <returns>Task representing the async write operation</returns>
            public async Task WriteAsync(T? element)
            {
                if (!IsWritable)
                    throw new InvalidOperationException("Stream is not writable");

                if (element == null || element.Equals(default(T)))
                {
                    await WriteEndAsync();
                    return;
                }

                await WriteInternalAsync(element);
            }

            /// <summary>
            /// Virtual method for custom write implementation.
            /// Should be overridden by subclasses to provide actual write logic.
            /// </summary>
            /// <param name="element">The element to write</param>
            /// <returns>Task representing the async write operation</returns>
            protected virtual async Task WriteInternalAsync(T element)
            {
                if (_customWrite != null)
                {
                    await _customWrite(this, element);
                }
                else if (NodeWritableStream != null)
                {
                    byte[] bytes = _serializer(element);
                    await NodeWritableStream.WriteAsync(bytes);
                    await NodeWritableStream.FlushAsync();
                }
                else
                {
                    throw new NotImplementedException(
                        "ObjectWriteStream needs to be subclassed and the WriteInternalAsync method needs to be implemented.");
                }
            }

            /// <summary>
            /// Virtual method for custom writeEnd implementation.
            /// Can be overridden by subclasses for cleanup logic.
            /// </summary>
            /// <returns>Task representing the async end operation</returns>
            protected virtual async Task WriteEndInternalAsync()
            {
                if (_customWriteEnd != null)
                {
                    await _customWriteEnd(this);
                }
                // Default empty implementation
            }

            /// <summary>
            /// Ends the stream, optionally writing a final element.
            /// </summary>
            /// <param name="element">Optional final element to write before ending</param>
            /// <returns>Task representing the async end operation</returns>
            public async Task WriteEndAsync(T? element = default)
            {
                if (element != null && !element.Equals(default(T)))
                {
                    await WriteAsync(element);
                }

                await WriteEndInternalAsync();
                IsWritable = false;
            }

            #endregion

            #region Helper Methods

            private ObjectWriteStreamOptions<T> ProcessOptions(object? optionsOrStream)
            {
                return optionsOrStream switch
                {
                    Stream stream => new ObjectWriteStreamOptions<T> { NodeStream = stream },
                    ObjectWriteStreamOptions<T> options => options,
                    null => new ObjectWriteStreamOptions<T>(),
                    _ when HasWritableState(optionsOrStream) => new ObjectWriteStreamOptions<T>
                    {
                        NodeStream = optionsOrStream as Stream
                    },
                    _ => throw new ArgumentException($"Invalid options type: {optionsOrStream?.GetType()}")
                };
            }

            private static bool HasWritableState(object? obj)
            {
                // Check if the object has a _writableState property (similar to Node.js streams)
                return obj?.GetType().GetProperty("_writableState") != null ||
                       obj?.GetType().GetProperty("WritableState") != null;
            }

            private void SetupStreamHandling(ObjectWriteStreamOptions<T> options)
            {
                if (NodeWritableStream == null) return;

                // Set up default write behavior with backpressure handling
                options.Write ??= async (writeStream, data) =>
                {
                    try
                    {
                        // Convert object to bytes for writing
                        byte[] bytes = _serializer(data);

                        await NodeWritableStream.WriteAsync(bytes);
                        await NodeWritableStream.FlushAsync();

                        // Simulate backpressure check
                        if (ShouldApplyBackpressure())
                        {
                            await WaitForDrainAsync();
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                };

                // Set up writeEnd behavior for non-standard streams
                if (!IsStandardStream(NodeWritableStream))
                {
                    options.WriteEnd ??= async (writeStream) =>
                    {
                        await NodeWritableStream.FlushAsync();

                        TaskCompletionSource tcs = new TaskCompletionSource();
                        try
                        {
                            NodeWritableStream.Close();
                            tcs.SetResult();
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }

                        await tcs.Task;
                    };
                }
            }

            private byte[] DefaultSerializer(T data)
            {
                if (data is string str)
                    return System.Text.Encoding.UTF8.GetBytes(str + Environment.NewLine);

                if (data is byte[] bytes)
                    return bytes;

                // Use System.Text.Json for complex objects
                string json = System.Text.Json.JsonSerializer.Serialize(data);
                return System.Text.Encoding.UTF8.GetBytes(json + Environment.NewLine);
            }

            private bool ShouldApplyBackpressure()
            {
                // In a real implementation, check if the underlying stream's buffer is full
                return false;
            }

            private async Task WaitForDrainAsync()
            {
                TaskCompletionSource tcs = new TaskCompletionSource();
                _drainWaiters.Add(tcs);

                // Simulate drain handling
                _ = Task.Run(async () =>
                {
                    await Task.Delay(10); // Simulate brief wait
                    ResolveDrainWaiters();
                });

                await tcs.Task;
            }

            private void ResolveDrainWaiters()
            {
                foreach (TaskCompletionSource waiter in _drainWaiters)
                {
                    waiter.SetResult();
                }

                _drainWaiters.Clear();
            }

            private static bool IsStandardStream(Stream stream)
            {
                return stream == Console.OpenStandardOutput() ||
                       stream == Console.OpenStandardError() ||
                       stream == Console.OpenStandardInput();
            }

            public async ValueTask DisposeAsync()
            {
                if (!_disposed)
                {
                    if (IsWritable)
                        await WriteEndAsync();
                    NodeWritableStream?.Dispose();
                    _disposed = true;
                    IsWritable = false;
                }
            }

            #endregion
        }

        /// <summary>
        /// Configuration options for ObjectWriteStream initialization.
        /// </summary>
        /// <typeparam name="T">The type of objects to write</typeparam>
        public class ObjectWriteStreamOptions<T>
        {
            /// <summary>
            /// Internal writable state (for compatibility with underlying streams).
            /// </summary>
            public object? WritableState { get; set; }

            /// <summary>
            /// Optional .NET Stream to wrap as the underlying writable stream.
            /// </summary>
            public Stream? NodeStream { get; set; }

            /// <summary>
            /// Optional custom write function that handles data writing.
            /// </summary>
            public Func<ObjectWriteStream<T>, T, Task>? Write { get; set; }

            /// <summary>
            /// Optional custom write end function that handles stream finalization.
            /// </summary>
            public Func<ObjectWriteStream<T>, Task>? WriteEnd { get; set; }
        }
    }


    /// <summary>
    /// Configuration options for ObjectReadWriteStream initialization.
    /// Provides customizable handlers for read and write operations.
    /// </summary>
    /// <typeparam name="T">The type of objects in the stream</typeparam>
    public class ObjectReadWriteStreamOptions<T>
    {
        /// <summary>
        /// Optional custom read function.
        /// </summary>
        public Func<ObjectReadStream<T>, Task>? Read { get; set; }

        /// <summary>
        /// Optional custom pause function.
        /// </summary>
        public Func<ObjectReadStream<T>, Task>? Pause { get; set; }

        /// <summary>
        /// Optional custom destroy function.
        /// </summary>
        public Func<ObjectReadStream<T>, Task>? Destroy { get; set; }

        /// <summary>
        /// Optional custom write function that handles individual elements.
        /// </summary>
        public Func<ObjectReadStream<T>.ObjectWriteStream<T>, T, Task>? Write { get; set; }

        /// <summary>
        /// Optional custom write end function that handles stream finalization.
        /// </summary>
        public Func<Task>? WriteEnd { get; set; }

        /// <summary>
        /// Optional .NET Stream to wrap for writing operations.
        /// </summary>
        public Stream? NodeWritableStream { get; set; }

        /// <summary>
        /// Initial buffer of objects for reading.
        /// </summary>
        public T[]? Buffer { get; set; }
    }



    /// <summary>
    /// A duplex stream that supports both reading and writing of typed objects.
    /// Combines the functionality of ObjectReadStream and ObjectWriteStream.
    /// </summary>
    /// <typeparam name="T">The type of objects in the stream</typeparam>
    public class ObjectReadWriteStream<T> : ObjectReadStream<T>, IAsyncDisposable
    {
        private readonly Func<ObjectWriteStream<T>, T, Task>? _customWrite;
        private readonly Func<Task>? _customWriteEnd;
        private readonly List<TaskCompletionSource> _drainWaiters;
        private bool _disposed;

        // Override to ensure both are true for duplex streams
        public new bool IsReadable { get; } = true;
        public new bool IsWritable { get; private set; } = true;
        public Stream? NodeWritableStream { get; private set; }

        /// <summary>
        /// Creates a new ObjectReadWriteStream (duplex stream).
        /// </summary>
        /// <param name="options">Configuration options for the duplex stream</param>
        public ObjectReadWriteStream(ObjectReadWriteStreamOptions<T>? options = null)
            : base(CreateReadStreamOptions(options), new Queue<Exception>(), Task.CompletedTask)
        {
            _drainWaiters = new List<TaskCompletionSource>();
            options ??= new ObjectReadWriteStreamOptions<T>();

            // Set up writable stream integration
            if (options.NodeWritableStream != null)
            {
                NodeWritableStream = options.NodeWritableStream;
                SetupWritableStreamHandling(options);
            }

            // Apply custom handlers
            _customWrite = options.Write;
            _customWriteEnd = options.WriteEnd;

            // Initialize buffer if provided
            if (options.Buffer != null)
            {
                foreach (T item in options.Buffer)
                {
                    Push(item);
                }
                PushEnd();
            }
        }

        #region Write Operations

        /// <summary>
        /// Writes an element to the stream. If element is null/default, ends the stream.
        /// </summary>
        /// <param name="element">The element to write, or null to end the stream</param>
        /// <returns>Task representing the async write operation</returns>
        public async Task WriteAsync(T? element)
        {
            if (!IsWritable)
                throw new InvalidOperationException("Stream is not writable");

            if (element == null || element.Equals(default(T)))
            {
                await WriteEndAsync();
                return;
            }

            await WriteInternalAsync(element);
        }

        /// <summary>
        /// Virtual method for custom write implementation.
        /// Should be overridden by subclasses to provide actual write logic.
        /// </summary>
        /// <param name="element">The element to write</param>
        /// <returns>Task representing the async write operation</returns>
        protected virtual async Task WriteInternalAsync(T element)
        {
            if (_customWrite != null)
            {
                // Create a temporary ObjectWriteStream wrapper for the callback
                ObjectWriteStream<T> writeStream = new ObjectWriteStream<T>();
                await _customWrite(writeStream, element);
            }
            else if (NodeWritableStream != null)
            {
                // Default behavior: write to underlying stream
                byte[] bytes = SerializeElement(element);
                await NodeWritableStream.WriteAsync(bytes);
                await NodeWritableStream.FlushAsync();

                // Handle backpressure if needed
                if (ShouldApplyBackpressure())
                {
                    await WaitForDrainAsync();
                }
            }
            else
            {
                throw new NotImplementedException(
                    "ObjectReadWriteStream needs to be subclassed and the WriteInternalAsync method needs to be implemented.");
            }
        }

        /// <summary>
        /// Virtual method for custom writeEnd implementation.
        /// In a ReadWriteStream, this only affects the write side.
        /// </summary>
        /// <returns>Task representing the async end operation</returns>
        protected virtual async Task WriteEndInternalAsync()
        {
            if (_customWriteEnd != null)
            {
                await _customWriteEnd();
            }
            else if (NodeWritableStream != null && !IsStandardStream(NodeWritableStream))
            {
                await NodeWritableStream.FlushAsync();
                NodeWritableStream.Close();
            }
        }

        /// <summary>
        /// Ends the write side of the stream, optionally writing a final element.
        /// </summary>
        /// <param name="element">Optional final element to write before ending</param>
        /// <returns>Task representing the async end operation</returns>
        public async Task WriteEndAsync(T? element = default)
        {
            if (element != null && !element.Equals(default(T)))
            {
                await WriteAsync(element);
            }

            await WriteEndInternalAsync();
            IsWritable = false;
        }

        #endregion

        #region Read Operations Override

        /// <summary>
        /// In a ReadWriteStream, ReadInternalAsync does not need to be implemented,
        /// because it's valid for the read stream buffer to be filled only by WriteInternalAsync.
        /// </summary>
        /// <param name="size">Number of elements to read (ignored in duplex streams)</param>
        /// <returns>Completed task</returns>
        protected override Task ReadInternalAsync(int size = 0)
        {
            // In duplex streams, reading can be fulfilled by writes, so no implementation needed
            return Task.CompletedTask;
        }

        #endregion

        #region Helper Methods

        private static List<T> CreateReadStreamOptions(ObjectReadWriteStreamOptions<T>? options)
        {
            List<T> buffer = new List<T>();
            
            if (options?.Buffer != null)
            {
                buffer.AddRange(options.Buffer);
            }
            
            return buffer;
        }

        private void SetupWritableStreamHandling(ObjectReadWriteStreamOptions<T> options)
        {
            if (NodeWritableStream == null) return;

            // Set up default write behavior if not provided
            options.Write ??= async (writeStream, data) =>
            {
                byte[] bytes = SerializeElement(data);
                await NodeWritableStream.WriteAsync(bytes);
                await NodeWritableStream.FlushAsync();

                if (ShouldApplyBackpressure())
                {
                    await WaitForDrainAsync();
                }
            };

            // Set up writeEnd behavior for non-standard streams
            if (!IsStandardStream(NodeWritableStream))
            {
                options.WriteEnd ??= async () =>
                {
                    await NodeWritableStream.FlushAsync();
                    NodeWritableStream.Close();
                };
            }
        }

        private byte[] SerializeElement(T element)
        {
            if (element is string str)
                return System.Text.Encoding.UTF8.GetBytes(str + Environment.NewLine);

            if (element is byte[] bytes)
                return bytes;

            // Use System.Text.Json for complex objects
            string json = System.Text.Json.JsonSerializer.Serialize(element);
            return System.Text.Encoding.UTF8.GetBytes(json + Environment.NewLine);
        }

        private bool ShouldApplyBackpressure()
        {
            // In a real implementation, check if the underlying stream's buffer is full
            return false;
        }

        private async Task WaitForDrainAsync()
        {
            TaskCompletionSource tcs = new TaskCompletionSource();
            _drainWaiters.Add(tcs);

            // Simulate drain handling
            _ = Task.Run(async () =>
            {
                await Task.Delay(10); // Simulate brief wait
                ResolveDrainWaiters();
            });

            await tcs.Task;
        }

        private void ResolveDrainWaiters()
        {
            foreach (TaskCompletionSource waiter in _drainWaiters)
            {
                waiter.SetResult();
            }
            _drainWaiters.Clear();
        }

        private static bool IsStandardStream(Stream stream)
        {
            return stream == Console.OpenStandardOutput() ||
                   stream == Console.OpenStandardError() ||
                   stream == Console.OpenStandardInput();
        }

        #endregion

        #region Disposal

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                if (IsWritable)
                    await WriteEndAsync();

                await DestroyAsync();
                NodeWritableStream?.Dispose();
                _disposed = true;
                IsWritable = false;
            }
        }

        #endregion
    }
}
