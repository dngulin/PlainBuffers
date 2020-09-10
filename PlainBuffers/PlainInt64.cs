﻿using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainInt64 {
    public const int Size = sizeof(long);

    // ReSharper disable once InconsistentNaming
    public readonly Span<byte> _Buffer;

    public PlainInt64(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _Buffer = buffer;
    }

    public long Read() => BinaryPrimitives.ReadInt64BigEndian(_Buffer);

    public void Write(long value) => BinaryPrimitives.WriteInt64BigEndian(_Buffer, value);
    public void Write(PlainInt64 value) => value._Buffer.CopyTo(_Buffer);

    public static bool operator ==(PlainInt64 l, PlainInt64 r) => l._Buffer == r._Buffer;
    public static bool operator !=(PlainInt64 l, PlainInt64 r) => l._Buffer != r._Buffer;
  }
}