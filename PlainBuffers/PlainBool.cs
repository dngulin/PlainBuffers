using System;

namespace PlainBuffers {
  public readonly ref struct PlainBool {
    public const int Size = sizeof(byte);

    public readonly Span<byte> Buffer;
    private readonly bool _defaultValue;

    public PlainBool(byte[] buffer, bool defaultValue) : this(new Span<byte>(buffer), defaultValue) {}

    public PlainBool(Span<byte> buffer, bool defaultValue) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
      _defaultValue = defaultValue;
    }

    public unsafe bool Read() => *(bool*) Buffer[0];

    public void WriteDefault() => Write(_defaultValue);
    public unsafe void Write(bool value) => Buffer[0] = *(byte*) &value;

    public void Write(PlainBool value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainBool l, PlainBool r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainBool l, PlainBool r) => l.Buffer != r.Buffer;
  }
}