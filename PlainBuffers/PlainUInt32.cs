using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainUInt32 {
    public const int Size = sizeof(uint);

    public readonly Span<byte> Buffer;
    private readonly uint _defaultValue;

    public PlainUInt32(byte[] buffer, uint defaultValue) : this(new Span<byte>(buffer), defaultValue) {}

    public PlainUInt32(Span<byte> buffer, uint defaultValue) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
      _defaultValue = defaultValue;
    }

    public uint Read() => BinaryPrimitives.ReadUInt32BigEndian(Buffer);

    public void WriteDefault() => Write(_defaultValue);
    public void Write(uint value) => BinaryPrimitives.WriteUInt32BigEndian(Buffer, value);

    public void Write(PlainUInt32 value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainUInt32 l, PlainUInt32 r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainUInt32 l, PlainUInt32 r) => l.Buffer != r.Buffer;
  }
}