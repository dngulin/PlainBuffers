using System;

namespace PlainBuffers.Core {
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

    public void CopyTo(PlainInt8 other) => _Buffer.CopyTo(other._Buffer);

    public static bool operator ==(PlainInt8 l, PlainInt8 r) => l._Buffer.SequenceEqual(r._Buffer);
    public static bool operator !=(PlainInt8 l, PlainInt8 r) => !l._Buffer.SequenceEqual(r._Buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}