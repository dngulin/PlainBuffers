using System;

namespace PlainBuffers {
  public readonly ref struct PlainInt8 {
    public const int Size = sizeof(sbyte);

    // ReSharper disable once InconsistentNaming
    public readonly Span<byte> _Buffer;

    public PlainInt8(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _Buffer = buffer;
    }

    public sbyte Read() => unchecked((sbyte) _Buffer[0]);


    public void Write(sbyte value) => _Buffer[0] = (byte) value;
    public void Write(PlainInt8 value) => value._Buffer.CopyTo(_Buffer);

    public static bool operator ==(PlainInt8 l, PlainInt8 r) => l._Buffer == r._Buffer;
    public static bool operator !=(PlainInt8 l, PlainInt8 r) => l._Buffer != r._Buffer;
  }
}