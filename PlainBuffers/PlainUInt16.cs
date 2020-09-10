using System;
using System.Buffers.Binary;

namespace PlainBuffers {
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
    public void Write(PlainUInt16 value) => value._Buffer.CopyTo(_Buffer);

    public static bool operator ==(PlainUInt16 l, PlainUInt16 r) => l._Buffer == r._Buffer;
    public static bool operator !=(PlainUInt16 l, PlainUInt16 r) => l._Buffer != r._Buffer;
  }
}