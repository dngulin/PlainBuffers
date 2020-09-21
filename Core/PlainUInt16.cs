using System;
using System.Buffers.Binary;

namespace PlainBuffers.Core {
  public readonly ref struct PlainUInt16 {
    public const int SizeOf = sizeof(ushort);

    private readonly Span<byte> _buffer;
    public Span<byte> GetBuffer() => _buffer;

    public PlainUInt16(Span<byte> buffer) {
      if (buffer.Length != SizeOf)
        throw new InvalidOperationException("Buffer size doesn't match to the struct size!");

      _buffer = buffer;
    }

    public ushort Read() => BinaryPrimitives.ReadUInt16BigEndian(_buffer);
    public void Write(ushort value) => BinaryPrimitives.WriteUInt16BigEndian(_buffer, value);

    public void CopyTo(PlainUInt16 other) => _buffer.CopyTo(other._buffer);

    public static bool operator ==(PlainUInt16 l, PlainUInt16 r) => l._buffer.SequenceEqual(r._buffer);
    public static bool operator !=(PlainUInt16 l, PlainUInt16 r) => !l._buffer.SequenceEqual(r._buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}