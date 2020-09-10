﻿using System;
using System.Buffers.Binary;

namespace PlainBuffers {
  public readonly ref struct PlainInt32 {
    public const int Size = sizeof(int);

    // ReSharper disable once InconsistentNaming
    public readonly Span<byte> _Buffer;

    public PlainInt32(Span<byte> buffer) {
      if (buffer.Length != Size)
        throw new InvalidOperationException();

      _Buffer = buffer;
    }

    public int Read() => BinaryPrimitives.ReadInt32BigEndian(_Buffer);

    public void Write(int value) => BinaryPrimitives.WriteInt32BigEndian(_Buffer, value);
    public void Write(PlainInt32 value) => value._Buffer.CopyTo(_Buffer);

    public static bool operator ==(PlainInt32 l, PlainInt32 r) => l._Buffer == r._Buffer;
    public static bool operator !=(PlainInt32 l, PlainInt32 r) => l._Buffer != r._Buffer;
  }
}