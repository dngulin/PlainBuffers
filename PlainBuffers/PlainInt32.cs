using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainInt32 {
    public const int Size = sizeof(int);

    public readonly Span<byte> Buffer;
    private readonly int _defaultValue;

    public PlainInt32(byte[] buffer, int defaultValue) : this(new Span<byte>(buffer), defaultValue) {}

    public PlainInt32(Span<byte> buffer, int defaultValue) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
      _defaultValue = defaultValue;
    }

    public int Read() => BinaryPrimitives.ReadInt32BigEndian(Buffer);

    public void WriteDefault() => Write(_defaultValue);
    public void Write(int value) => BinaryPrimitives.WriteInt32BigEndian(Buffer, value);

    public void Write(PlainInt32 value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainInt32 l, PlainInt32 r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainInt32 l, PlainInt32 r) => l.Buffer != r.Buffer;
  }
}