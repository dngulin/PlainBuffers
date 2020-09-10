using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainUInt16 {
    public const int Size = sizeof(ushort);

    public readonly Span<byte> Buffer;
    private readonly ushort _defaultValue;

    public PlainUInt16(byte[] buffer, ushort defaultValue) : this(new Span<byte>(buffer), defaultValue) {}

    public PlainUInt16(Span<byte> buffer, ushort defaultValue) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
      _defaultValue = defaultValue;
    }

    public ushort Read() => BinaryPrimitives.ReadUInt16BigEndian(Buffer);

    public void WriteDefault() => Write(_defaultValue);
    public void Write(ushort value) => BinaryPrimitives.WriteUInt16BigEndian(Buffer, value);

    public void Write(PlainUInt16 value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainUInt16 l, PlainUInt16 r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainUInt16 l, PlainUInt16 r) => l.Buffer != r.Buffer;
  }
}