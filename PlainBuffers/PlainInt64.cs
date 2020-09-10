using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainInt64 {
    public const int Size = sizeof(long);
    public readonly Span<byte> Buffer;

    public PlainInt64(Span<byte> buffer, long defaultValue) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
    }

    public long Read() => BinaryPrimitives.ReadInt64BigEndian(Buffer);

    public void Write(long value) => BinaryPrimitives.WriteInt64BigEndian(Buffer, value);
    public void Write(PlainInt64 value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainInt64 l, PlainInt64 r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainInt64 l, PlainInt64 r) => l.Buffer != r.Buffer;
  }
}