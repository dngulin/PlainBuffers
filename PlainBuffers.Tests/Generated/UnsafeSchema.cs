// This file is auto-generated by the PlainBuffers compiler
// Generated at 2020-09-28T20:57:18.7549092+03:00

// ReSharper disable All

using System;

namespace PlainBuffers.Tests.GeneratedUnsafe {
    public enum ColorId : byte {
        Red = 0,
        Green = 1,
        Blue = 2
    }

    public readonly unsafe ref struct Vec {
        public const int SizeOf = 12;

        private readonly byte* _ptr;
        public Span<byte> GetBuffer() => new Span<byte>(_ptr, SizeOf);

        public Vec(byte* ptr) => _ptr = ptr;

        public static Vec WrapBuffer(byte* buffer, int bufferSize, int myIndex = 0) {
            var offset = SizeOf * myIndex;
            if ((bufferSize - offset) < SizeOf) throw new InvalidOperationException("Buffer size ios too small!");
            return new Vec(buffer + offset);
        }

        public ref float X => ref *((float*)(_ptr + 0));
        public ref float Y => ref *((float*)(_ptr + 4));
        public ref float Z => ref *((float*)(_ptr + 8));

        public void CopyTo(Vec other) => GetBuffer().CopyTo(other.GetBuffer());

        public void WriteDefault() {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public static bool operator ==(Vec l, Vec r) => l.GetBuffer().SequenceEqual(r.GetBuffer());
        public static bool operator !=(Vec l, Vec r) => !l.GetBuffer().SequenceEqual(r.GetBuffer());

        public override bool Equals(object obj) => false;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    public readonly unsafe ref struct Quat {
        public const int SizeOf = 16;

        private readonly byte* _ptr;
        public Span<byte> GetBuffer() => new Span<byte>(_ptr, SizeOf);

        public Quat(byte* ptr) => _ptr = ptr;

        public static Quat WrapBuffer(byte* buffer, int bufferSize, int myIndex = 0) {
            var offset = SizeOf * myIndex;
            if ((bufferSize - offset) < SizeOf) throw new InvalidOperationException("Buffer size ios too small!");
            return new Quat(buffer + offset);
        }

        public ref float X => ref *((float*)(_ptr + 0));
        public ref float Y => ref *((float*)(_ptr + 4));
        public ref float Z => ref *((float*)(_ptr + 8));
        public ref float W => ref *((float*)(_ptr + 12));

        public void CopyTo(Quat other) => GetBuffer().CopyTo(other.GetBuffer());

        public void WriteDefault() {
            X = 0;
            Y = 0;
            Z = 0;
            W = 1;
        }

        public static bool operator ==(Quat l, Quat r) => l.GetBuffer().SequenceEqual(r.GetBuffer());
        public static bool operator !=(Quat l, Quat r) => !l.GetBuffer().SequenceEqual(r.GetBuffer());

        public override bool Equals(object obj) => false;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    public readonly unsafe ref struct HandleArray5 {
        public const int SizeOf = 10;
        public const int Length = 5;

        private readonly byte* _ptr;
        public Span<byte> GetBuffer() => new Span<byte>(_ptr, SizeOf);

        public HandleArray5(byte* ptr) => _ptr = ptr;

        public static HandleArray5 WrapBuffer(byte* buffer, int bufferSize, int myIndex = 0) {
            var offset = SizeOf * myIndex;
            if ((bufferSize - offset) < SizeOf) throw new InvalidOperationException("Buffer size ios too small!");
            return new HandleArray5(buffer + offset);
        }

        public void CopyTo(HandleArray5 other) => GetBuffer().CopyTo(other.GetBuffer());

        public void WriteDefault() {
            for (var i = 0; i < Length; i++) {
                this[i] = -1;
            }
        }

        public ref short this[int index] {
            get {
                if (index < 0 || sizeof(short) * index >= SizeOf) throw new IndexOutOfRangeException();
                return ref *((short*)_ptr + index);
            }
        }

        private ref short At(int index) => ref *((short*)_ptr + index);

        public _EnumeratorOfHandleArray5 GetEnumerator() => new _EnumeratorOfHandleArray5(this);

        public ref struct _EnumeratorOfHandleArray5 {
            private readonly HandleArray5 _array;
            private int _index;

            public _EnumeratorOfHandleArray5(HandleArray5 array) {
                _array = array;
                _index = -1;
            }

            public bool MoveNext() => ++_index < Length;
            public ref short Current => ref _array.At(_index);

            public void Reset() => _index = -1;
            public void Dispose() {}
        }

        public static bool operator ==(HandleArray5 l, HandleArray5 r) => l.GetBuffer().SequenceEqual(r.GetBuffer());
        public static bool operator !=(HandleArray5 l, HandleArray5 r) => !l.GetBuffer().SequenceEqual(r.GetBuffer());

        public override bool Equals(object obj) => false;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    public readonly unsafe ref struct Monster {
        public const int SizeOf = 44;
        private const int _Padding = 2;

        private readonly byte* _ptr;
        public Span<byte> GetBuffer() => new Span<byte>(_ptr, SizeOf);

        public Monster(byte* ptr) => _ptr = ptr;

        public static Monster WrapBuffer(byte* buffer, int bufferSize, int myIndex = 0) {
            var offset = SizeOf * myIndex;
            if ((bufferSize - offset) < SizeOf) throw new InvalidOperationException("Buffer size ios too small!");
            return new Monster(buffer + offset);
        }

        public Vec Position => new Vec(_ptr + 0);
        public Quat Rotation => new Quat(_ptr + 12);
        public ref short Hp => ref *((short*)(_ptr + 28));
        public HandleArray5 Inventory => new HandleArray5(_ptr + 30);
        public ref bool Aggressive => ref *((bool*)(_ptr + 40));
        public ref ColorId Color => ref *((ColorId*)(_ptr + 41));

        public void CopyTo(Monster other) => GetBuffer().CopyTo(other.GetBuffer());

        public void WriteDefault() {
            Position.WriteDefault();
            Rotation.WriteDefault();
            Hp = 100;
            Inventory.WriteDefault();
            Aggressive = true;
            Color = ColorId.Blue;
            GetBuffer().Slice(SizeOf - _Padding, _Padding).Fill(0);
        }

        public static bool operator ==(Monster l, Monster r) => l.GetBuffer().SequenceEqual(r.GetBuffer());
        public static bool operator !=(Monster l, Monster r) => !l.GetBuffer().SequenceEqual(r.GetBuffer());

        public override bool Equals(object obj) => false;
        public override int GetHashCode() => throw new NotSupportedException();
    }
}