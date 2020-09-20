using System;
using System.Buffers.Binary;

namespace PlainBuffers.Core {
  public readonly ref struct PlainInt16 {
    public const int Size = sizeof(short);

    private readonly Span<byte> _buffer;
    public Span<byte> GetBuffer() => _buffer;

    public PlainInt16(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _buffer = buffer;
    }

    public short Read() => BinaryPrimitives.ReadInt16BigEndian(_buffer);
    public void Write(short value) => BinaryPrimitives.WriteInt16BigEndian(_buffer, value);

    public void CopyTo(PlainInt16 other) => _buffer.CopyTo(other._buffer);

    public static bool operator ==(PlainInt16 l, PlainInt16 r) => l._buffer.SequenceEqual(r._buffer);
    public static bool operator !=(PlainInt16 l, PlainInt16 r) => !l._buffer.SequenceEqual(r._buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}