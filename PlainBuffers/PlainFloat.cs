using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainFloat {
    public const int Size = sizeof(float);

    // ReSharper disable once InconsistentNaming
    public readonly Span<byte> _Buffer;

    public PlainFloat(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _Buffer = buffer;
    }

    public unsafe float Read() {
      var value = BinaryPrimitives.ReadUInt32BigEndian(_Buffer);
      return *(float*) &value;
    }

    public unsafe void Write(float value) => BinaryPrimitives.WriteUInt32BigEndian(_Buffer, *(uint*) &value);
    public void Write(PlainFloat value) => value._Buffer.CopyTo(_Buffer);

    public static bool operator ==(PlainFloat l, PlainFloat r) => l._Buffer == r._Buffer;
    public static bool operator !=(PlainFloat l, PlainFloat r) => l._Buffer != r._Buffer;
  }
}