// This file is auto-generated by the PlainBuffers compiler
// Generated at 2024-01-16T21:24:52.4941141+01:00

// ReSharper disable All

using System;
using System.Runtime.InteropServices;

#pragma warning disable 649

namespace PlainBuffers.Tests.Generated {
    public enum ColorId : byte {
        Red = 0,
        Green = 1,
        Blue = 2
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Vec {
        public const int SizeOf = 12;
        public const int AlignmentOf = 4;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public float X;
        [FieldOffset(4)] public float Y;
        [FieldOffset(8)] public float Z;

        public void WriteDefault() {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public static bool operator ==(in Vec l, in Vec r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return new Span<byte>(__l, SizeOf).SequenceEqual(new Span<byte>(__r, SizeOf));
            }
        }
        public static bool operator !=(in Vec l, in Vec r) => !(l == r);

        public override bool Equals(object obj) => obj is Vec casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Quat {
        public const int SizeOf = 16;
        public const int AlignmentOf = 4;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public float X;
        [FieldOffset(4)] public float Y;
        [FieldOffset(8)] public float Z;
        [FieldOffset(12)] public float W;

        public void WriteDefault() {
            X = 0;
            Y = 0;
            Z = 0;
            W = 1;
        }

        public static bool operator ==(in Quat l, in Quat r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return new Span<byte>(__l, SizeOf).SequenceEqual(new Span<byte>(__r, SizeOf));
            }
        }
        public static bool operator !=(in Quat l, in Quat r) => !(l == r);

        public override bool Equals(object obj) => obj is Quat casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct HandleArray5 {
        public const int SizeOf = 10;
        public const int AlignmentOf = 2;
        public const int Length = 5;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public short Item0;
        [FieldOffset(2)] public short Item1;
        [FieldOffset(4)] public short Item2;
        [FieldOffset(6)] public short Item3;
        [FieldOffset(8)] public short Item4;

        public void WriteDefault() {
            fixed (byte* ptr = _buffer) {
                for (var i = 0; i < Length; i++) {
                    (*((short*)ptr + i)) = -1;
                }
            }
        }

        public static bool operator ==(in HandleArray5 l, in HandleArray5 r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return new Span<byte>(__l, SizeOf).SequenceEqual(new Span<byte>(__r, SizeOf));
            }
        }
        public static bool operator !=(in HandleArray5 l, in HandleArray5 r) => !(l == r);

        public override bool Equals(object obj) => obj is HandleArray5 casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();

        public unsafe readonly ref struct RefIterator {
            private readonly HandleArray5* _ptr;
            public RefIterator(ref HandleArray5 array) {
                fixed (HandleArray5* ptr = &array) _ptr = ptr;
            }
            public RefEnumerator GetEnumerator() => new RefEnumerator(_ptr);
        }

        public unsafe readonly ref struct RefReadonlyIterator {
            private readonly HandleArray5* _ptr;
            public RefReadonlyIterator(in HandleArray5 array) {
                fixed (HandleArray5* ptr = &array) _ptr = ptr;
            }
            public RefReadonlyEnumerator GetEnumerator() => new RefReadonlyEnumerator(_ptr);
        }

        public unsafe ref struct RefEnumerator {
            private readonly HandleArray5* _ptr;
            private int _index;
            public RefEnumerator(HandleArray5* ptr) {
                _ptr = ptr;
                _index = -1;
            }
            public ref short Current => ref *((short*)_ptr + _index);
            public bool MoveNext() => ++_index < HandleArray5.Length;
            public void Reset() => _index = -1;
            public void Dispose() {}
        }

        public unsafe ref struct RefReadonlyEnumerator {
            private readonly HandleArray5* _ptr;
            private int _index;
            public RefReadonlyEnumerator(HandleArray5* ptr) {
                _ptr = ptr;
                _index = -1;
            }
            public ref readonly short Current => ref *((short*)_ptr + _index);
            public bool MoveNext() => ++_index < HandleArray5.Length;
            public void Reset() => _index = -1;
            public void Dispose() {}
        }
    }

    public static unsafe class _HandleArray5_IndexExtensions {
        public static ref short RefAt(this ref HandleArray5 array, int index) {
            if (index < 0 || sizeof(short) * index >= HandleArray5.SizeOf) throw new IndexOutOfRangeException();
            fixed (HandleArray5* ptr = &array) {
                return ref *((short*)ptr + index);
            }
        }
        public static HandleArray5.RefIterator RefIter(this ref HandleArray5 array) => new HandleArray5.RefIterator(ref array);

        public static ref readonly short RefReadonlyAt(this in HandleArray5 array, int index) {
            if (index < 0 || sizeof(short) * index >= HandleArray5.SizeOf) throw new IndexOutOfRangeException();
            fixed (HandleArray5* ptr = &array) {
                return ref *((short*)ptr + index);
            }
        }
        public static HandleArray5.RefReadonlyIterator RefReadonlyIter(this in HandleArray5 array) => new HandleArray5.RefReadonlyIterator(in array);
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Monster {
        public const int SizeOf = 44;
        public const int AlignmentOf = 4;
        private const int _Padding = 2;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public Vec Position;
        [FieldOffset(12)] public Quat Rotation;
        [FieldOffset(28)] public short Hp;
        [FieldOffset(30)] public HandleArray5 Inventory;
        [FieldOffset(40), MarshalAs(UnmanagedType.U1)] public bool Aggressive;
        [FieldOffset(41)] public ColorId Color;

        public void WriteDefault() {
            Position.WriteDefault();
            Rotation.WriteDefault();
            Hp = 100;
            Inventory.WriteDefault();
            Aggressive = true;
            Color = ColorId.Blue;
            fixed (byte* __ptr = _buffer) {
                new Span<byte>(__ptr + (SizeOf - _Padding), _Padding).Fill(0);
            }
        }

        public static bool operator ==(in Monster l, in Monster r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return new Span<byte>(__l, SizeOf).SequenceEqual(new Span<byte>(__r, SizeOf));
            }
        }
        public static bool operator !=(in Monster l, in Monster r) => !(l == r);

        public override bool Equals(object obj) => obj is Monster casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Monsters {
        public const int SizeOf = 220;
        public const int AlignmentOf = 4;
        public const int Length = 5;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public Monster Item0;
        [FieldOffset(44)] public Monster Item1;
        [FieldOffset(88)] public Monster Item2;
        [FieldOffset(132)] public Monster Item3;
        [FieldOffset(176)] public Monster Item4;

        public void WriteDefault() {
            fixed (byte* ptr = _buffer) {
                for (var i = 0; i < Length; i++) {
                    (*((Monster*)ptr + i)).WriteDefault();
                }
            }
        }

        public static bool operator ==(in Monsters l, in Monsters r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return new Span<byte>(__l, SizeOf).SequenceEqual(new Span<byte>(__r, SizeOf));
            }
        }
        public static bool operator !=(in Monsters l, in Monsters r) => !(l == r);

        public override bool Equals(object obj) => obj is Monsters casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();

        public unsafe readonly ref struct RefIterator {
            private readonly Monsters* _ptr;
            public RefIterator(ref Monsters array) {
                fixed (Monsters* ptr = &array) _ptr = ptr;
            }
            public RefEnumerator GetEnumerator() => new RefEnumerator(_ptr);
        }

        public unsafe readonly ref struct RefReadonlyIterator {
            private readonly Monsters* _ptr;
            public RefReadonlyIterator(in Monsters array) {
                fixed (Monsters* ptr = &array) _ptr = ptr;
            }
            public RefReadonlyEnumerator GetEnumerator() => new RefReadonlyEnumerator(_ptr);
        }

        public unsafe ref struct RefEnumerator {
            private readonly Monsters* _ptr;
            private int _index;
            public RefEnumerator(Monsters* ptr) {
                _ptr = ptr;
                _index = -1;
            }
            public ref Monster Current => ref *((Monster*)_ptr + _index);
            public bool MoveNext() => ++_index < Monsters.Length;
            public void Reset() => _index = -1;
            public void Dispose() {}
        }

        public unsafe ref struct RefReadonlyEnumerator {
            private readonly Monsters* _ptr;
            private int _index;
            public RefReadonlyEnumerator(Monsters* ptr) {
                _ptr = ptr;
                _index = -1;
            }
            public ref readonly Monster Current => ref *((Monster*)_ptr + _index);
            public bool MoveNext() => ++_index < Monsters.Length;
            public void Reset() => _index = -1;
            public void Dispose() {}
        }
    }

    public static unsafe class _Monsters_IndexExtensions {
        public static ref Monster RefAt(this ref Monsters array, int index) {
            if (index < 0 || sizeof(Monster) * index >= Monsters.SizeOf) throw new IndexOutOfRangeException();
            fixed (Monsters* ptr = &array) {
                return ref *((Monster*)ptr + index);
            }
        }
        public static Monsters.RefIterator RefIter(this ref Monsters array) => new Monsters.RefIterator(ref array);

        public static ref readonly Monster RefReadonlyAt(this in Monsters array, int index) {
            if (index < 0 || sizeof(Monster) * index >= Monsters.SizeOf) throw new IndexOutOfRangeException();
            fixed (Monsters* ptr = &array) {
                return ref *((Monster*)ptr + index);
            }
        }
        public static Monsters.RefReadonlyIterator RefReadonlyIter(this in Monsters array) => new Monsters.RefReadonlyIterator(in array);
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct VecQuat {
        public const int SizeOf = 16;
        public const int AlignmentOf = 4;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public Vec Vec;
        [FieldOffset(0)] public Quat Quat;

        public static bool operator ==(in VecQuat l, in VecQuat r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return new Span<byte>(__l, SizeOf).SequenceEqual(new Span<byte>(__r, SizeOf));
            }
        }
        public static bool operator !=(in VecQuat l, in VecQuat r) => !(l == r);

        public override bool Equals(object obj) => obj is VecQuat casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }
}
