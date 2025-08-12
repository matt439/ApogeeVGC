using System.Buffers.Binary;
using System.Collections;
using System.Text;

namespace ApogeeVGC_CS.lib
{
    /// <summary>
    /// A Buffer implementation similar to Node.js Buffer, for binary data manipulation.
    /// </summary>
    public class Buffer : IEnumerable<byte>, IEquatable<Buffer>
    {
        private readonly Memory<byte> _memory;

        #region Constructors

        /// <summary>
        /// Creates a buffer from a string with specified encoding
        /// </summary>
        public Buffer(string str, BufferEncoding encoding = BufferEncoding.Utf8)
        {
            _memory = new Memory<byte>(GetBytesFromString(str, encoding));
        }

        /// <summary>
        /// Creates a buffer of specified size
        /// </summary>
        public Buffer(int size)
        {
            _memory = new Memory<byte>(new byte[size]);
        }

        /// <summary>
        /// Creates a buffer from byte array
        /// </summary>
        public Buffer(byte[] array)
        {
            _memory = new Memory<byte>(array);
        }

        /// <summary>
        /// Creates a buffer from Memory
        /// </summary>
        public Buffer(Memory<byte> memory)
        {
            _memory = memory;
        }

        /// <summary>
        /// Creates a buffer by copying another buffer
        /// </summary>
        public Buffer(Buffer buffer)
        {
            var bytes = new byte[buffer.Length];
            buffer.CopyTo(bytes);
            _memory = new Memory<byte>(bytes);
        }

        #endregion

        #region Static Factory Methods

        /// <summary>
        /// Creates a new Buffer from an array buffer.
        /// </summary>
        public static Buffer From(Memory<byte> arrayBuffer, int byteOffset = 0, int? length = null)
        {
            int actualLength = length ?? (arrayBuffer.Length - byteOffset);
            return new Buffer(arrayBuffer.Slice(byteOffset, actualLength));
        }

        /// <summary>
        /// Creates a new Buffer from a byte array.
        /// </summary>
        public static Buffer From(byte[] data)
        {
            return new Buffer(data);
        }

        /// <summary>
        /// Creates a new Buffer from a string with specified encoding.
        /// </summary>
        public static Buffer From(string str, BufferEncoding encoding = BufferEncoding.Utf8)
        {
            return new Buffer(str, encoding);
        }

        /// <summary>
        /// Creates a new Buffer from individual byte values.
        /// </summary>
        public static Buffer Of(params byte[] items)
        {
            return new Buffer(items);
        }

        /// <summary>
        /// Returns true if the object is a Buffer.
        /// </summary>
        public static bool IsBuffer(object obj)
        {
            return obj is Buffer;
        }

        /// <summary>
        /// Returns true if the encoding is valid.
        /// </summary>
        public static bool IsEncoding(string encoding)
        {
            try
            {
                GetEncodingFromBufferEncoding(ParseEncoding(encoding));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the byte length of a string with specified encoding.
        /// </summary>
        public static int ByteLength(string str, BufferEncoding encoding = BufferEncoding.Utf8)
        {
            return GetEncodingFromBufferEncoding(encoding).GetByteCount(str);
        }

        /// <summary>
        /// Concatenates multiple buffers into a new buffer.
        /// </summary>
        public static Buffer Concat(IReadOnlyCollection<Buffer> list, int? totalLength = null)
        {
            switch (list.Count)
            {
                case 0:
                    return new Buffer(0);
                case 1:
                    return list.First();
            }

            int length = totalLength ?? list.Sum(b => b.Length);
            var result = new byte[length];
            var offset = 0;

            foreach (var buffer in list)
            {
                buffer.CopyTo(result, offset);
                offset += buffer.Length;
            }

            return new Buffer(result);
        }

        /// <summary>
        /// Compares two buffers.
        /// </summary>
        public static int Compare(Buffer buf1, Buffer buf2)
        {
            return buf1.CompareTo(buf2);
        }

        /// <summary>
        /// Allocates a new Buffer of specified size.
        /// </summary>
        public static Buffer Alloc(int size, object? fill = null, BufferEncoding encoding = BufferEncoding.Utf8)
        {
            var buffer = new Buffer(size);

            if (fill != null)
            {
                buffer.Fill(fill, encoding: encoding);
            }

            return buffer;
        }

        /// <summary>
        /// Allocates a new Buffer of specified size without initialization.
        /// </summary>
        public static Buffer AllocUnsafe(int size)
        {
            return new Buffer(size);
        }

        /// <summary>
        /// Allocates a new non-pooled Buffer of specified size without initialization.
        /// </summary>
        public static Buffer AllocUnsafeSlow(int size)
        {
            return new Buffer(size);
        }

        #endregion

        #region Properties and Indexer

        public static int PoolSize { get; set; } = 8192;

        public int Length => _memory.Length;

        public byte this[int index]
        {
            get => _memory.Span[index];
            set => _memory.Span[index] = value;
        }

        #endregion

        #region Methods

        public int Write(string str, BufferEncoding encoding = BufferEncoding.Utf8)
        {
            return Write(str, 0, encoding);
        }

        public int Write(string str, int offset, BufferEncoding encoding = BufferEncoding.Utf8)
        {
            byte[] bytes = GetBytesFromString(str, encoding);
            int length = Math.Min(bytes.Length, Length - offset);
            bytes.AsMemory(0, length).CopyTo(_memory[offset..]);
            return length;
        }

        public int Write(string str, int offset, int length, BufferEncoding encoding = BufferEncoding.Utf8)
        {
            byte[] bytes = GetBytesFromString(str, encoding);
            int actualLength = Math.Min(Math.Min(bytes.Length, length), Length - offset);
            bytes.AsMemory(0, actualLength).CopyTo(_memory[offset..]);
            return actualLength;
        }

        public string ToString(BufferEncoding encoding = BufferEncoding.Utf8, int start = 0, int? end = null)
        {
            int actualEnd = end ?? Length;
            return GetEncodingFromBufferEncoding(encoding).GetString(_memory.Slice(start, actualEnd - start).Span);
        }

        public object ToJson()
        {
            return new
            {
                type = "Buffer",
                data = _memory.ToArray()
            };
        }

        public bool Equals(Buffer? other)
        {
            if (other == null || Length != other.Length)
                return false;

            return _memory.Span.SequenceEqual(other._memory.Span);
        }

        public override bool Equals(object? obj)
        {
            return obj is Buffer buffer && Equals(buffer);
        }

        public override int GetHashCode()
        {
            return _memory.GetHashCode();
        }

        public int CompareTo(Buffer other, int targetStart = 0, int? targetEnd = null,
            int sourceStart = 0, int? sourceEnd = null)
        {
            int actualTargetEnd = targetEnd ?? Length;
            int actualSourceEnd = sourceEnd ?? other.Length;

            ReadOnlySpan<byte> thisSpan = _memory.Slice(targetStart, actualTargetEnd - targetStart).Span;
            ReadOnlySpan<byte> otherSpan = other._memory.Slice(sourceStart, actualSourceEnd - sourceStart).Span;

            return thisSpan.SequenceCompareTo(otherSpan);
        }

        public int Copy(Buffer targetBuffer, int targetStart = 0, int sourceStart = 0, int? sourceEnd = null)
        {
            int actualSourceEnd = sourceEnd ?? Length;
            int count = Math.Min(actualSourceEnd - sourceStart, targetBuffer.Length - targetStart);

            _memory.Slice(sourceStart, count).CopyTo(targetBuffer._memory.Slice(targetStart));
            return count;
        }

        public void CopyTo(byte[] array, int offset = 0)
        {
            _memory.Span.CopyTo(array.AsSpan(offset));
        }

        public Buffer Slice(int begin = 0, int? end = null)
        {
            int actualEnd = end ?? Length;
            return new Buffer(_memory.Slice(begin, actualEnd - begin));
        }

        public Buffer Subarray(int begin = 0, int? end = null)
        {
            return Slice(begin, end);
        }

        // Additional methods for reading/writing various numeric types
        // (Implementation of the many read/write methods would go here)
        // For example:

        public ulong ReadBigUInt64Be(int offset = 0)
        {
            return BitConverter.IsLittleEndian
                ? BinaryPrimitives.ReadUInt64BigEndian(_memory.Slice(offset, 8).Span)
                : BitConverter.ToUInt64(_memory.Slice(offset, 8).Span);
        }

        public int WriteUInt8(byte value, int offset = 0)
        {
            _memory.Span[offset] = value;
            return offset + 1;
        }

        // And so on for all the other read/write methods...

        public Buffer Fill(object value, int offset = 0, int? end = null, BufferEncoding encoding = BufferEncoding.Utf8)
        {
            int actualEnd = end ?? Length;
            var targetSpan = _memory.Slice(offset, actualEnd - offset).Span;

            switch (value)
            {
                case byte byteValue:
                    targetSpan.Fill(byteValue);
                    break;
                case string strValue:
                {
                    byte[] bytes = GetBytesFromString(strValue, encoding);
                    for (var i = 0; i < targetSpan.Length; i++)
                    {
                        targetSpan[i] = bytes[i % bytes.Length];
                    }

                    break;
                }
                case Buffer bufValue:
                {
                    for (var i = 0; i < targetSpan.Length; i++)
                    {
                        targetSpan[i] = bufValue[i % bufValue.Length];
                    }

                    break;
                }
                case int intValue:
                    targetSpan.Fill((byte)intValue);
                    break;
            }

            return this;
        }

        public int IndexOf(object value, int byteOffset = 0, BufferEncoding encoding = BufferEncoding.Utf8)
        {
            // Implementation for IndexOf
            // This would need to handle different types and perform appropriate searches
            throw new NotImplementedException();
        }

        // IEnumerable implementation
        public IEnumerator<byte> GetEnumerator()
        {
            for (var i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Helper Methods

        private static byte[] GetBytesFromString(string str, BufferEncoding encoding)
        {
            return GetEncodingFromBufferEncoding(encoding).GetBytes(str);
        }

        private static BufferEncoding ParseEncoding(string encodingStr)
        {
            return encodingStr.ToLowerInvariant() switch
            {
                "ascii" => BufferEncoding.Ascii,
                "utf8" => BufferEncoding.Utf8,
                "utf-8" => BufferEncoding.Utf8Dash,
                "utf16le" => BufferEncoding.Utf16Le,
                "ucs2" => BufferEncoding.Ucs2,
                "ucs-2" => BufferEncoding.Ucs2Dash,
                "base64" => BufferEncoding.Base64,
                "latin1" => BufferEncoding.Latin1,
                "binary" => BufferEncoding.Binary,
                "hex" => BufferEncoding.Hex,
                _ => throw new ArgumentException($"Unknown encoding: {encodingStr}")
            };
        }

        private static Encoding GetEncodingFromBufferEncoding(BufferEncoding encoding)
        {
            return encoding switch
            {
                BufferEncoding.Ascii => Encoding.ASCII,
                BufferEncoding.Utf8 => Encoding.UTF8,
                BufferEncoding.Utf8Dash => Encoding.UTF8,
                BufferEncoding.Utf16Le => Encoding.Unicode,
                BufferEncoding.Ucs2 => Encoding.Unicode,
                BufferEncoding.Ucs2Dash => Encoding.Unicode,
                BufferEncoding.Latin1 => Encoding.GetEncoding("iso-8859-1"),
                BufferEncoding.Binary => Encoding.GetEncoding("iso-8859-1"),
                _ => throw new ArgumentException($"Unsupported encoding: {encoding}")
            };
        }

        #endregion
    }
}