using System;
using System.Buffers.Binary;

namespace PlainBuffers.Core {
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
    public unsafe void Write(double value) => BinaryPrimitives.WriteUInt64BigEndian(_Buffer, *(ulong*) &value);

    public void CopyTo(PlainDouble other) => _Buffer.CopyTo(other._Buffer);

    public static bool operator ==(PlainDouble l, PlainDouble r) => l._Buffer.SequenceEqual(r._Buffer);
    public static bool operator !=(PlainDouble l, PlainDouble r) => !l._Buffer.SequenceEqual(r._Buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}