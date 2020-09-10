﻿using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainInt16 {
    public const int Size = sizeof(short);

    // ReSharper disable once InconsistentNaming
    public readonly Span<byte> _Buffer;

    public PlainInt16(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _Buffer = buffer;
    }

    public short Read() => BinaryPrimitives.ReadInt16BigEndian(_Buffer);

    public void Write(short value) => BinaryPrimitives.WriteInt16BigEndian(_Buffer, value);
    public void Write(PlainInt16 value) => value._Buffer.CopyTo(_Buffer);

    public static bool operator ==(PlainInt16 l, PlainInt16 r) => l._Buffer == r._Buffer;
    public static bool operator !=(PlainInt16 l, PlainInt16 r) => l._Buffer != r._Buffer;
  }
}