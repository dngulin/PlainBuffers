using System;

namespace PlainBuffers.Core {
  public readonly ref struct PlainBool {
    public const int Size = sizeof(byte);

    private const byte True = 1;
    private const byte False = 0;

    private readonly Span<byte> _buffer;
    public Span<byte> GetBuffer() => _buffer;

    public PlainBool(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _buffer = buffer;
    }

    public bool Read() => _buffer[0] != False;
    public void Write(bool value) => _buffer[0] = value ? True : False;

    public void CopyTo(PlainBool other) => _buffer.CopyTo(other._buffer);

    public static bool operator ==(PlainBool l, PlainBool r) => l._buffer.SequenceEqual(r._buffer);
    public static bool operator !=(PlainBool l, PlainBool r) => !l._buffer.SequenceEqual(r._buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}