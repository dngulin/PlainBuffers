using System;

namespace PlainBuffers {
  public readonly ref struct PlainInt8 {
    public const int Size = sizeof(sbyte);
    public readonly Span<byte> Buffer;

    public PlainInt8(Span<byte> buffer, sbyte defaultValue) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
    }

    public sbyte Read() => unchecked((sbyte) Buffer[0]);


    public void Write(sbyte value) => Buffer[0] = (byte) value;
    public void Write(PlainInt8 value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainInt8 l, PlainInt8 r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainInt8 l, PlainInt8 r) => l.Buffer != r.Buffer;
  }
}