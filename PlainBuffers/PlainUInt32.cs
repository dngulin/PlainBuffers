using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainUInt32 {
    public const int Size = sizeof(uint);

    // ReSharper disable once InconsistentNaming
    public readonly Span<byte> _Buffer;

    public PlainUInt32(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _Buffer = buffer;
    }

    public uint Read() => BinaryPrimitives.ReadUInt32BigEndian(_Buffer);

    public void Write(uint value) => BinaryPrimitives.WriteUInt32BigEndian(_Buffer, value);
    public void Write(PlainUInt32 value) => value._Buffer.CopyTo(_Buffer);

    public static bool operator ==(PlainUInt32 l, PlainUInt32 r) => l._Buffer == r._Buffer;
    public static bool operator !=(PlainUInt32 l, PlainUInt32 r) => l._Buffer != r._Buffer;
  }
}