using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainDouble {
    public const int Size = sizeof(double);

    // ReSharper disable once InconsistentNaming
    public readonly Span<byte> _Buffer;

    public PlainDouble(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _Buffer = buffer;
    }

    public unsafe double Read() {
      var value = BinaryPrimitives.ReadUInt64BigEndian(_Buffer);
      return *(double*) &value;
    }

    public unsafe void Write(double value) => BinaryPrimitives.WriteUInt64BigEndian(_Buffer, *(uint*) &value);
    public void Write(PlainDouble value) => value._Buffer.CopyTo(_Buffer);

    public static bool operator ==(PlainDouble l, PlainDouble r) => l._Buffer == r._Buffer;
    public static bool operator !=(PlainDouble l, PlainDouble r) => l._Buffer != r._Buffer;
  }
}