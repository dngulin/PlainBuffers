using System;

namespace PlainBuffers {
  public readonly ref struct PlainBool {
    public const int Size = sizeof(byte);
    public readonly Span<byte> Buffer;

    public PlainBool(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
    }

    public unsafe bool Read() => *(bool*) Buffer[0];

    public unsafe void Write(bool value) => Buffer[0] = *(byte*) &value;
    public void Write(PlainBool value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainBool l, PlainBool r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainBool l, PlainBool r) => l.Buffer != r.Buffer;
  }
}