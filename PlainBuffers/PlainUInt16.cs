using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainUInt16 {
    public const int Size = sizeof(ushort);

    public readonly Span<byte> Buffer;

    public PlainUInt16(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
    }

    public ushort Read() => BinaryPrimitives.ReadUInt16BigEndian(Buffer);

    public void Write(ushort value) => BinaryPrimitives.WriteUInt16BigEndian(Buffer, value);
    public void Write(PlainUInt16 value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainUInt16 l, PlainUInt16 r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainUInt16 l, PlainUInt16 r) => l.Buffer != r.Buffer;
  }
}