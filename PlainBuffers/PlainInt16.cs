using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainInt16 {
    public const int Size = sizeof(short);
    public readonly Span<byte> Buffer;

    public PlainInt16(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
    }

    public short Read() => BinaryPrimitives.ReadInt16BigEndian(Buffer);

    public void Write(short value) => BinaryPrimitives.WriteInt16BigEndian(Buffer, value);
    public void Write(PlainInt16 value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainInt16 l, PlainInt16 r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainInt16 l, PlainInt16 r) => l.Buffer != r.Buffer;
  }
}