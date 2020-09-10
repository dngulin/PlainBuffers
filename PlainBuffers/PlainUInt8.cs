using System;

namespace PlainBuffers {
  public readonly ref struct PlainUInt8 {
    public const int Size = sizeof(byte);
    public readonly Span<byte> Buffer;

    public PlainUInt8(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
    }

    public byte Read() => Buffer[0];

    public void Write(byte value) => Buffer[0] = value;
    public void Write(PlainUInt8 value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainUInt8 l, PlainUInt8 r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainUInt8 l, PlainUInt8 r) => l.Buffer != r.Buffer;
  }
}