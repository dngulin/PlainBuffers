using System;

namespace PlainBuffers.Core {
  public readonly ref struct PlainInt8 {
    public const int Size = sizeof(sbyte);

    private readonly Span<byte> _buffer;
    public Span<byte> GetBuffer() => _buffer;

    public PlainInt8(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException("Buffer size doesn't match to the struct size!");

      _buffer = buffer;
    }

    public sbyte Read() => unchecked((sbyte) _buffer[0]);
    public void Write(sbyte value) => _buffer[0] = (byte) value;

    public void CopyTo(PlainInt8 other) => _buffer.CopyTo(other._buffer);

    public static bool operator ==(PlainInt8 l, PlainInt8 r) => l._buffer.SequenceEqual(r._buffer);
    public static bool operator !=(PlainInt8 l, PlainInt8 r) => !l._buffer.SequenceEqual(r._buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}