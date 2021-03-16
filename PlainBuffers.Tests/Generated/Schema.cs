// This file is auto-generated by the PlainBuffers compiler
// Generated at 2021-03-16T22:32:26.3632894+03:00

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

    public unsafe struct HandleArray5 {
        public const int SizeOf = 10;
        public const int AlignmentOf = 2;
        public const int Length = 5;

        private fixed byte _buffer[SizeOf];

        public void WriteDefault() {
            for (var i = 0; i < Length; i++) {
                this[i] = -1;
            }
        }

        public ref short this[int index] {
            get {
                if (index < 0 || sizeof(short) * index >= SizeOf) throw new IndexOutOfRangeException();
                return ref At(index);
            }
        }

        private ref short At(int index) {
            fixed (byte* __ptr = _buffer) {
                return ref *((short*)__ptr + index);
            }
        }

        public _EnumeratorOfHandleArray5 GetEnumerator() => new _EnumeratorOfHandleArray5(ref this);

        public unsafe ref struct _EnumeratorOfHandleArray5 {
            private readonly HandleArray5* _arrayPtr;
            private int _index;

            public _EnumeratorOfHandleArray5(ref HandleArray5 array) {
                fixed (HandleArray5* arrayPtr = &array) _arrayPtr = arrayPtr;
                _index = -1;
            }

            public bool MoveNext() => ++_index < Length;
            public ref short Current => ref (*_arrayPtr).At(_index);

            public void Reset() => _index = -1;
            public void Dispose() {}
        }

        public static bool operator ==(in HandleArray5 l, in HandleArray5 r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return new Span<byte>(__l, SizeOf).SequenceEqual(new Span<byte>(__r, SizeOf));
            }
        }
        public static bool operator !=(in HandleArray5 l, in HandleArray5 r) => !(l == r);

        public override bool Equals(object obj) => obj is HandleArray5 casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
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
        [FieldOffset(40)] public bool Aggressive;
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

    public unsafe struct Monsters {
        public const int SizeOf = 220;
        public const int AlignmentOf = 4;
        public const int Length = 5;

        private fixed byte _buffer[SizeOf];

        public void WriteDefault() {
            for (var i = 0; i < Length; i++) {
                this[i].WriteDefault();
            }
        }

        public ref Monster this[int index] {
            get {
                if (index < 0 || sizeof(Monster) * index >= SizeOf) throw new IndexOutOfRangeException();
                return ref At(index);
            }
        }

        private ref Monster At(int index) {
            fixed (byte* __ptr = _buffer) {
                return ref *((Monster*)__ptr + index);
            }
        }

        public _EnumeratorOfMonsters GetEnumerator() => new _EnumeratorOfMonsters(ref this);

        public unsafe ref struct _EnumeratorOfMonsters {
            private readonly Monsters* _arrayPtr;
            private int _index;

            public _EnumeratorOfMonsters(ref Monsters array) {
                fixed (Monsters* arrayPtr = &array) _arrayPtr = arrayPtr;
                _index = -1;
            }

            public bool MoveNext() => ++_index < Length;
            public ref Monster Current => ref (*_arrayPtr).At(_index);

            public void Reset() => _index = -1;
            public void Dispose() {}
        }

        public static bool operator ==(in Monsters l, in Monsters r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return new Span<byte>(__l, SizeOf).SequenceEqual(new Span<byte>(__r, SizeOf));
            }
        }
        public static bool operator !=(in Monsters l, in Monsters r) => !(l == r);

        public override bool Equals(object obj) => obj is Monsters casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }
}
