using System;

namespace PlainBuffers.Core {
  public readonly ref struct PlainUInt8 {
    public const int Size = sizeof(byte);

    // ReSharper disable once InconsistentNaming
    public readonly Span<byte> _Buffer;

    public PlainUInt8(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _Buffer = buffer;
    }

    public byte Read() => _Buffer[0];
    public void Write(byte value) => _Buffer[0] = value;

    public void CopyTo(PlainUInt8 other) => _Buffer.CopyTo(other._Buffer);

    public static bool operator ==(PlainUInt8 l, PlainUInt8 r) => l._Buffer.SequenceEqual(r._Buffer);
    public static bool operator !=(PlainUInt8 l, PlainUInt8 r) => !l._Buffer.SequenceEqual(r._Buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}