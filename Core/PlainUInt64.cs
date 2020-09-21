using System;
using System.Buffers.Binary;

namespace PlainBuffers.Core {
  public readonly ref struct PlainUInt64 {
    public const int SizeOf = sizeof(ulong);

    private readonly Span<byte> _buffer;
    public Span<byte> GetBuffer() => _buffer;

    public PlainUInt64(Span<byte> buffer) {
      if (buffer.Length != SizeOf)
        throw new InvalidOperationException("Buffer size doesn't match to the struct size!");

      _buffer = buffer;
    }

    public ulong Read() => BinaryPrimitives.ReadUInt64BigEndian(_buffer);
    public void Write(ulong value) => BinaryPrimitives.WriteUInt64BigEndian(_buffer, value);

    public void CopyTo(PlainUInt64 other) => _buffer.CopyTo(other._buffer);

    public static bool operator ==(PlainUInt64 l, PlainUInt64 r) => l._buffer.SequenceEqual(r._buffer);
    public static bool operator !=(PlainUInt64 l, PlainUInt64 r) => !l._buffer.SequenceEqual(r._buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}