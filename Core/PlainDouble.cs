using System;
using System.Buffers.Binary;

namespace PlainBuffers.Core {
  public readonly ref struct PlainDouble {
    public const int Size = sizeof(double);

    private readonly Span<byte> _buffer;
    public Span<byte> GetBuffer() => _buffer;

    public PlainDouble(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _buffer = buffer;
    }

    public unsafe double Read() {
      var value = BinaryPrimitives.ReadUInt64BigEndian(_buffer);
      return *(double*) &value;
    }
    public unsafe void Write(double value) => BinaryPrimitives.WriteUInt64BigEndian(_buffer, *(ulong*) &value);

    public void CopyTo(PlainDouble other) => _buffer.CopyTo(other._buffer);

    public static bool operator ==(PlainDouble l, PlainDouble r) => l._buffer.SequenceEqual(r._buffer);
    public static bool operator !=(PlainDouble l, PlainDouble r) => !l._buffer.SequenceEqual(r._buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}