using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainFloat {
    public const int Size = sizeof(float);
    public readonly Span<byte> Buffer;

    public PlainFloat(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      Buffer = buffer;
    }

    public unsafe float Read() {
      var value = BinaryPrimitives.ReadUInt32BigEndian(Buffer);
      return *(float*) &value;
    }

    public unsafe void Write(float value) => BinaryPrimitives.WriteUInt32BigEndian(Buffer, *(uint*) &value);
    public void Write(PlainFloat value) => value.Buffer.CopyTo(Buffer);

    public static bool operator ==(PlainFloat l, PlainFloat r) => l.Buffer == r.Buffer;
    public static bool operator !=(PlainFloat l, PlainFloat r) => l.Buffer != r.Buffer;
  }
}