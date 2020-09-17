using System;
using System.Buffers.Binary;

namespace PlainBuffers.Core {
  public readonly ref struct PlainUInt16 {
    public const int Size = sizeof(ushort);

    // ReSharper disable once InconsistentNaming
    public readonly Span<byte> _Buffer;

    public PlainUInt16(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _Buffer = buffer;
    }

    public ushort Read() => BinaryPrimitives.ReadUInt16BigEndian(_Buffer);
    public void Write(ushort value) => BinaryPrimitives.WriteUInt16BigEndian(_Buffer, value);

    public void CopyTo(PlainUInt16 other) => _Buffer.CopyTo(other._Buffer);

    public static bool operator ==(PlainUInt16 l, PlainUInt16 r) => l._Buffer.SequenceEqual(r._Buffer);
    public static bool operator !=(PlainUInt16 l, PlainUInt16 r) => !l._Buffer.SequenceEqual(r._Buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}