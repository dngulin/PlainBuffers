using System;

namespace PlainBuffers.Core {
  public readonly ref struct PlainBool {
    public const int Size = sizeof(byte);

    private const byte True = 1;
    private const byte False = 0;

    // ReSharper disable once InconsistentNaming
    public readonly Span<byte> _Buffer;

    public PlainBool(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _Buffer = buffer;
    }

    public bool Read() => _Buffer[0] != False;
    public void Write(bool value) => _Buffer[0] = value ? True : False;

    public void CopyTo(PlainBool other) => _Buffer.CopyTo(other._Buffer);

    public static bool operator ==(PlainBool l, PlainBool r) => l._Buffer.SequenceEqual(r._Buffer);
    public static bool operator !=(PlainBool l, PlainBool r) => !l._Buffer.SequenceEqual(r._Buffer);

    public override bool Equals(object obj) => false;
    public override int GetHashCode() => throw new NotSupportedException();
  }
}