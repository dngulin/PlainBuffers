using System;
using System.Buffers.Binary;

namespace PlainBuffers.Core {
  public readonly ref struct PlainUInt32 {
    public const int SizeOf = sizeof(uint);

    private readonly Span<byte> _buffer;
    public Span<byte> GetBuffer() => _buffer;

    public PlainUInt32(Span<byte> buffer) {
      if (buffer.Length != SizeOf)
        throw new InvalidOperationException("Buffer size doesn't match to the struct size!");

      _buffer = buffer;
    }

    public uint Read() => BinaryPrimitives.ReadUInt32BigEndian(_buffer);
    public void Write(uint value) => BinaryPrimitives.WriteUInt32BigEndian(_buffer, value);

    public void CopyTo(PlainUInt32 other) => _buffer.CopyTo(other._buffer);

    public static bool operator ==(PlainUInt32 l, PlainUInt32 r) => l._buffer.SequenceEqual(r._buffer);
    public static bool operator !=(PlainUInt32 l, PlainUInt32 r) => !l._buffer.SequenceEqual(r._buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}