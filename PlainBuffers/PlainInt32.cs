using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainInt32 {
    public const int Size = sizeof(int);
    public readonly Span<byte> Buffer;
    public PlainInt32(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
    }

    public int Read() => BinaryPrimitives.ReadInt32BigEndian(Buffer);

    public void Write(int value) => BinaryPrimitives.WriteInt32BigEndian(Buffer, value);
    public void Write(PlainInt32 value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainInt32 l, PlainInt32 r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainInt32 l, PlainInt32 r) => l.Buffer != r.Buffer;
  }
}