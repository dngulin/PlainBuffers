using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainDouble {
    public const int Size = sizeof(double);

    public readonly Span<byte> Buffer;
    private readonly double _defaultValue;

    public PlainDouble(byte[] buffer, double defaultValue) : this(new Span<byte>(buffer), defaultValue) {}

    public PlainDouble(Span<byte> buffer, double defaultValue) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
      _defaultValue = defaultValue;
    }

    public unsafe double Read() {
      var value = BinaryPrimitives.ReadUInt64BigEndian(Buffer);
      return *(double*) &value;
    }

    public void WriteDefault() => Write(_defaultValue);
    public unsafe void Write(double value) => BinaryPrimitives.WriteUInt64BigEndian(Buffer, *(uint*) &value);

    public void Write(PlainDouble value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainDouble l, PlainDouble r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainDouble l, PlainDouble r) => l.Buffer != r.Buffer;
  }
}