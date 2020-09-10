using System;

namespace PlainBuffers {
  public readonly ref struct PlainUInt8 {
    public const int Size = sizeof(byte);

    public readonly Span<byte> Buffer;
    private readonly byte _defaultValue;

    public PlainUInt8(byte[] buffer, byte defaultValue) : this(new Span<byte>(buffer), defaultValue) {}

    public PlainUInt8(Span<byte> buffer, byte defaultValue) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
      _defaultValue = defaultValue;
    }

    public byte Read() => Buffer[0];

    public void WriteDefault() => Write(_defaultValue);
    public void Write(byte value) => Buffer[0] = value;

    public void Write(PlainUInt8 value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainUInt8 l, PlainUInt8 r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainUInt8 l, PlainUInt8 r) => l.Buffer != r.Buffer;
  }
}