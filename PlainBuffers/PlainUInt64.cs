using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainUInt64 {
    public const int Size = sizeof(ulong);
    public readonly Span<byte> Buffer;

    public PlainUInt64(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
    }

    public ulong Read() => BinaryPrimitives.ReadUInt64BigEndian(Buffer);

    public void Write(ulong value) => BinaryPrimitives.WriteUInt64BigEndian(Buffer, value);
    public void Write(PlainUInt64 value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainUInt64 l, PlainUInt64 r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainUInt64 l, PlainUInt64 r) => l.Buffer != r.Buffer;
  }
}