using System;
using System.Buffers.Binary;

namespace PlainBuffers.Core {
  public readonly ref struct PlainInt32 {
    public const int SizeOf = sizeof(int);

    private readonly Span<byte> _buffer;
    public Span<byte> GetBuffer() => _buffer;

    public PlainInt32(Span<byte> buffer) {
      if (buffer.Length != SizeOf)
        throw new InvalidOperationException("Buffer size doesn't match to the struct size!");

      _buffer = buffer;
    }

    public int Read() => BinaryPrimitives.ReadInt32BigEndian(_buffer);
    public void Write(int value) => BinaryPrimitives.WriteInt32BigEndian(_buffer, value);

    public void CopyTo(PlainInt32 other) => _buffer.CopyTo(other._buffer);

    public static bool operator ==(PlainInt32 l, PlainInt32 r) => l._buffer.SequenceEqual(r._buffer);
    public static bool operator !=(PlainInt32 l, PlainInt32 r) => !l._buffer.SequenceEqual(r._buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}