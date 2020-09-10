using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainUInt32 {
    public const int Size = sizeof(uint);
    public readonly Span<byte> Buffer;

    public PlainUInt32(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
    }

    public uint Read() => BinaryPrimitives.ReadUInt32BigEndian(Buffer);

    public void Write(uint value) => BinaryPrimitives.WriteUInt32BigEndian(Buffer, value);
    public void Write(PlainUInt32 value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainUInt32 l, PlainUInt32 r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainUInt32 l, PlainUInt32 r) => l.Buffer != r.Buffer;
  }
}