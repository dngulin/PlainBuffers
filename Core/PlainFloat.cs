using System;
using System.Buffers.Binary;

namespace PlainBuffers.Core {
  public readonly ref struct PlainFloat {
    public const int Size = sizeof(float);

    private readonly Span<byte> _buffer;
    public Span<byte> GetBuffer() => _buffer;

    public PlainFloat(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException("Buffer size doesn't match to the struct size!");

      _buffer = buffer;
    }

    public unsafe float Read() {
      var value = BinaryPrimitives.ReadUInt32BigEndian(_buffer);
      return *(float*) &value;
    }
    public unsafe void Write(float value) => BinaryPrimitives.WriteUInt32BigEndian(_buffer, *(uint*) &value);

    public void CopyTo(PlainFloat other) => _buffer.CopyTo(other._buffer);

    public static bool operator ==(PlainFloat l, PlainFloat r) => l._buffer.SequenceEqual(r._buffer);
    public static bool operator !=(PlainFloat l, PlainFloat r) => !l._buffer.SequenceEqual(r._buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}