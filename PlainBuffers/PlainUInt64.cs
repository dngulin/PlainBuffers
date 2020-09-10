using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainUInt64 {
    public const int Size = sizeof(ulong);

    public readonly Span<byte> Buffer;
    private readonly ulong _defaultValue;

    public PlainUInt64(byte[] buffer, ulong defaultValue) : this(new Span<byte>(buffer), defaultValue) {}

    public PlainUInt64(Span<byte> buffer, ulong defaultValue) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
      _defaultValue = defaultValue;
    }

    public ulong Read() => BinaryPrimitives.ReadUInt64BigEndian(Buffer);

    public void WriteDefault() => Write(_defaultValue);
    public void Write(ulong value) => BinaryPrimitives.WriteUInt64BigEndian(Buffer, value);

    public void Write(PlainUInt64 value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainUInt64 l, PlainUInt64 r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainUInt64 l, PlainUInt64 r) => l.Buffer != r.Buffer;
  }
}