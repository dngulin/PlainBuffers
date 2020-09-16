using System;
using System.Buffers.Binary;

namespace PlainBuffers.Core {
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

    public void CopyTo(PlainFloat other) => _Buffer.CopyTo(other._Buffer);

    public static bool operator ==(PlainFloat l, PlainFloat r) => l._Buffer == r._Buffer;
    public static bool operator !=(PlainFloat l, PlainFloat r) => l._Buffer != r._Buffer;
  }
}