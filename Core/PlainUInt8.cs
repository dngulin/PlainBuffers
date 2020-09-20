using System;

namespace PlainBuffers.Core {
  public readonly ref struct PlainUInt8 {
    public const int Size = sizeof(byte);

    private readonly Span<byte> _buffer;
    public Span<byte> GetBuffer() => _buffer;

    public PlainUInt8(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _buffer = buffer;
    }

    public byte Read() => _buffer[0];
    public void Write(byte value) => _buffer[0] = value;

    public void CopyTo(PlainUInt8 other) => _buffer.CopyTo(other._buffer);

    public static bool operator ==(PlainUInt8 l, PlainUInt8 r) => l._buffer.SequenceEqual(r._buffer);
    public static bool operator !=(PlainUInt8 l, PlainUInt8 r) => !l._buffer.SequenceEqual(r._buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}