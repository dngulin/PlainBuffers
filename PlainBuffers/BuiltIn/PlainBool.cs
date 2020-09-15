using System;

namespace PlainBuffers.BuiltIn {
  public readonly ref struct PlainBool {
    public const int Size = sizeof(byte);

    // ReSharper disable once InconsistentNaming
    public readonly Span<byte> _Buffer;

    public PlainBool(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _Buffer = buffer;
    }

    public unsafe bool Read() => *(bool*) _Buffer[0];
    public unsafe void Write(bool value) => _Buffer[0] = *(byte*) &value;
    
    public void CopyTo(PlainBool other) => _Buffer.CopyTo(other._Buffer);

    public static bool operator ==(PlainBool l, PlainBool r) => l._Buffer == r._Buffer;
    public static bool operator !=(PlainBool l, PlainBool r) => l._Buffer != r._Buffer;
  }
}