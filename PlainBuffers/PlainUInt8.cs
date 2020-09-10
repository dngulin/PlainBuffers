﻿using System;

namespace PlainBuffers {
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
    public void Write(PlainUInt8 value) => value._Buffer.CopyTo(_Buffer);

    public static bool operator ==(PlainUInt8 l, PlainUInt8 r) => l._Buffer == r._Buffer;
    public static bool operator !=(PlainUInt8 l, PlainUInt8 r) => l._Buffer != r._Buffer;
  }
}