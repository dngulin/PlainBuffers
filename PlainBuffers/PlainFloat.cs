using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainFloat {
    public const int Size = sizeof(float);

    public readonly Span<byte> Buffer;
    private readonly float _defaultValue;

    public PlainFloat(byte[] buffer, float defaultValue) : this(new Span<byte>(buffer), defaultValue) {}

    public PlainFloat(Span<byte> buffer, float defaultValue) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
      _defaultValue = defaultValue;
    }

    public unsafe float Read() {
      var value = BinaryPrimitives.ReadUInt32BigEndian(Buffer);
      return *(float*) &value;
    }

    public void WriteDefault() => Write(_defaultValue);
    public unsafe void Write(float value) => BinaryPrimitives.WriteUInt32BigEndian(Buffer, *(uint*) &value);

    public void Write(PlainFloat value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainFloat l, PlainFloat r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainFloat l, PlainFloat r) => l.Buffer != r.Buffer;
  }
}