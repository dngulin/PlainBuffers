﻿using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainUInt64 {
    public const int Size = sizeof(ulong);

    // ReSharper disable once InconsistentNaming
    public readonly Span<byte> _Buffer;

    public PlainUInt64(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _Buffer = buffer;
    }

    public ulong Read() => BinaryPrimitives.ReadUInt64BigEndian(_Buffer);

    public void Write(ulong value) => BinaryPrimitives.WriteUInt64BigEndian(_Buffer, value);
    public void Write(PlainUInt64 value) => value._Buffer.CopyTo(_Buffer);

    public static bool operator ==(PlainUInt64 l, PlainUInt64 r) => l._Buffer == r._Buffer;
    public static bool operator !=(PlainUInt64 l, PlainUInt64 r) => l._Buffer != r._Buffer;
  }
}