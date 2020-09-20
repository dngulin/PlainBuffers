using System;
using System.Buffers.Binary;

namespace PlainBuffers.Core {
  public readonly ref struct PlainInt64 {
    public const int Size = sizeof(long);

    private readonly Span<byte> _buffer;
    public Span<byte> GetBuffer() => _buffer;

    public PlainInt64(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _buffer = buffer;
    }

    public long Read() => BinaryPrimitives.ReadInt64BigEndian(_buffer);
    public void Write(long value) => BinaryPrimitives.WriteInt64BigEndian(_buffer, value);

    public void CopyTo(PlainInt64 other) => _buffer.CopyTo(other._buffer);

    public static bool operator ==(PlainInt64 l, PlainInt64 r) => l._buffer.SequenceEqual(r._buffer);
    public static bool operator !=(PlainInt64 l, PlainInt64 r) => !l._buffer.SequenceEqual(r._buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}