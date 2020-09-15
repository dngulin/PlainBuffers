using System;
using System.Buffers.Binary;

namespace PlainBuffers.BuiltIn {
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

    public void CopyTo(PlainUInt32 other) => _Buffer.CopyTo(other._Buffer);

    public static bool operator ==(PlainUInt32 l, PlainUInt32 r) => l._Buffer == r._Buffer;
    public static bool operator !=(PlainUInt32 l, PlainUInt32 r) => l._Buffer != r._Buffer;
  }
}