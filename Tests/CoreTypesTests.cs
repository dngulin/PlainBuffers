using System;
using System.Diagnostics.CodeAnalysis;
using PlainBuffers.Core;
using Xunit;

namespace PlainBuffers.Tests {
  public class CoreTypesTests {
    private readonly byte[] _buffer = new byte[PlainUInt64.Size * 2];

    [Fact]
    public void TestPlainBool() {
      Assert.Throws<InvalidOperationException>(() => _ = new PlainBool(_buffer.AsSpan(0, PlainBool.Size / 2)));

      var value1 = new PlainBool(_buffer.AsSpan(PlainBool.Size * 0, PlainBool.Size));
      var value2 = new PlainBool(_buffer.AsSpan(PlainBool.Size * 1, PlainBool.Size));

      Assert.True(value1.GetBuffer().Length == PlainBool.Size);

      value1.Write(true);
      value1.CopyTo(value2);
      Assert.True(value1 == value2, "operator `==` failed");

      value2.Write(false);
      Assert.True(value1 != value2, "operator `!=` failed");

      Assert.True(value1.Read(), "value1 read failed");
      Assert.False(value2.Read(), "value2 read failed");
    }

    [Fact]
    public void TestPlainInt8() {
      Assert.Throws<InvalidOperationException>(() => _ = new PlainInt8(_buffer.AsSpan(0, PlainInt8.Size / 2)));

      var value1 = new PlainInt8(_buffer.AsSpan(PlainInt8.Size * 0, PlainInt8.Size));
      var value2 = new PlainInt8(_buffer.AsSpan(PlainInt8.Size * 1, PlainInt8.Size));

      Assert.True(value1.GetBuffer().Length == PlainInt8.Size);

      value1.Write(sbyte.MaxValue / 3);
      value1.CopyTo(value2);
      Assert.True(value1 == value2, "operator `==` failed");

      value2.Write(sbyte.MinValue / 3);
      Assert.True(value1 != value2, "operator `!=` failed");

      Assert.True(value1.Read() == sbyte.MaxValue / 3, "value1 read failed");
      Assert.True(value2.Read() == sbyte.MinValue / 3, "value2 read failed");
    }

    [Fact]
    public void TestPlainUInt8() {
      Assert.Throws<InvalidOperationException>(() => _ = new PlainUInt8(_buffer.AsSpan(0, PlainUInt8.Size / 2)));

      var value1 = new PlainUInt8(_buffer.AsSpan(PlainUInt8.Size * 0, PlainUInt8.Size));
      var value2 = new PlainUInt8(_buffer.AsSpan(PlainUInt8.Size * 1, PlainUInt8.Size));

      Assert.True(value1.GetBuffer().Length == PlainUInt8.Size);

      value1.Write(byte.MaxValue / 3);
      value1.CopyTo(value2);
      Assert.True(value1 == value2, "operator `==` failed");

      value2.Write(byte.MaxValue / 7);
      Assert.True(value1 != value2, "operator `!=` failed");

      Assert.True(value1.Read() == byte.MaxValue / 3, "value1 read failed");
      Assert.True(value2.Read() == byte.MaxValue / 7, "value2 read failed");
    }

    [Fact]
    public void TestPlainInt16() {
      Assert.Throws<InvalidOperationException>(() => _ = new PlainInt16(_buffer.AsSpan(0, PlainInt16.Size / 2)));

      var value1 = new PlainInt16(_buffer.AsSpan(PlainInt16.Size * 0, PlainInt16.Size));
      var value2 = new PlainInt16(_buffer.AsSpan(PlainInt16.Size * 1, PlainInt16.Size));

      Assert.True(value1.GetBuffer().Length == PlainInt16.Size);

      value1.Write(short.MaxValue / 17);
      value1.CopyTo(value2);
      Assert.True(value1 == value2, "operator `==` failed");

      value2.Write(short.MinValue / 17);
      Assert.True(value1 != value2, "operator `!=` failed");

      Assert.True(value1.Read() == short.MaxValue / 17, "value1 read failed");
      Assert.True(value2.Read() == short.MinValue / 17, "value2 read failed");
    }

    [Fact]
    public void TestPlainUInt16() {
      Assert.Throws<InvalidOperationException>(() => _ = new PlainUInt16(_buffer.AsSpan(0, PlainUInt16.Size / 2)));

      var value1 = new PlainUInt16(_buffer.AsSpan(PlainUInt16.Size * 0, PlainUInt16.Size));
      var value2 = new PlainUInt16(_buffer.AsSpan(PlainUInt16.Size * 1, PlainUInt16.Size));

      Assert.True(value1.GetBuffer().Length == PlainUInt16.Size);

      value1.Write(ushort.MaxValue / 17);
      value1.CopyTo(value2);
      Assert.True(value1 == value2, "operator `==` failed");

      value2.Write(ushort.MaxValue / 19);
      Assert.True(value1 != value2, "operator `!=` failed");

      Assert.True(value1.Read() == ushort.MaxValue / 17, "value1 read failed");
      Assert.True(value2.Read() == ushort.MaxValue / 19, "value2 read failed");
    }

    [Fact]
    public void TestPlainInt32() {
      Assert.Throws<InvalidOperationException>(() => _ = new PlainInt32(_buffer.AsSpan(0, PlainInt32.Size / 2)));

      var value1 = new PlainInt32(_buffer.AsSpan(PlainInt32.Size * 0, PlainInt32.Size));
      var value2 = new PlainInt32(_buffer.AsSpan(PlainInt32.Size * 1, PlainInt32.Size));

      Assert.True(value1.GetBuffer().Length == PlainInt32.Size);

      value1.Write(int.MaxValue / 29);
      value1.CopyTo(value2);
      Assert.True(value1 == value2, "operator `==` failed");

      value2.Write(int.MinValue / 29);
      Assert.True(value1 != value2, "operator `!=` failed");

      Assert.True(value1.Read() == int.MaxValue / 29, "value1 read failed");
      Assert.True(value2.Read() == int.MinValue / 29, "value2 read failed");
    }

    [Fact]
    public void TestPlainUInt32() {
      Assert.Throws<InvalidOperationException>(() => _ = new PlainUInt32(_buffer.AsSpan(0, PlainUInt32.Size / 2)));

      var value1 = new PlainUInt32(_buffer.AsSpan(PlainUInt32.Size * 0, PlainUInt32.Size));
      var value2 = new PlainUInt32(_buffer.AsSpan(PlainUInt32.Size * 1, PlainUInt32.Size));

      Assert.True(value1.GetBuffer().Length == PlainUInt32.Size);

      value1.Write(uint.MaxValue / 29);
      value1.CopyTo(value2);
      Assert.True(value1 == value2, "operator `==` failed");

      value2.Write(uint.MaxValue / 31);
      Assert.True(value1 != value2, "operator `!=` failed");

      Assert.True(value1.Read() == uint.MaxValue / 29, "value1 read failed");
      Assert.True(value2.Read() == uint.MaxValue / 31, "value2 read failed");
    }

    [Fact]
    public void TestPlainInt64() {
      Assert.Throws<InvalidOperationException>(() => _ = new PlainInt64(_buffer.AsSpan(0, PlainInt64.Size / 2)));

      var value1 = new PlainInt64(_buffer.AsSpan(PlainInt64.Size * 0, PlainInt64.Size));
      var value2 = new PlainInt64(_buffer.AsSpan(PlainInt64.Size * 1, PlainInt64.Size));

      Assert.True(value1.GetBuffer().Length == PlainInt64.Size);

      value1.Write(long.MaxValue / 127);
      value1.CopyTo(value2);
      Assert.True(value1 == value2, "operator `==` failed");

      value2.Write(long.MinValue / 127);
      Assert.True(value1 != value2, "operator `!=` failed");

      Assert.True(value1.Read() == long.MaxValue / 127, "value1 read failed");
      Assert.True(value2.Read() == long.MinValue / 127, "value2 read failed");
    }

    [Fact]
    public void TestPlainUInt64() {
      Assert.Throws<InvalidOperationException>(() => _ = new PlainUInt64(_buffer.AsSpan(0, PlainUInt64.Size / 2)));

      var value1 = new PlainUInt64(_buffer.AsSpan(PlainUInt64.Size * 0, PlainUInt64.Size));
      var value2 = new PlainUInt64(_buffer.AsSpan(PlainUInt64.Size * 1, PlainUInt64.Size));

      Assert.True(value1.GetBuffer().Length == PlainUInt64.Size);

      value1.Write(ulong.MaxValue / 127);
      value1.CopyTo(value2);
      Assert.True(value1 == value2, "operator `==` failed");

      value2.Write(ulong.MaxValue / 131);
      Assert.True(value1 != value2, "operator `!=` failed");

      Assert.True(value1.Read() == ulong.MaxValue / 127, "value1 read failed");
      Assert.True(value2.Read() == ulong.MaxValue / 131, "value2 read failed");
    }

    [Fact]
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public void TestPlainFloat() {
      Assert.Throws<InvalidOperationException>(() => _ = new PlainFloat(_buffer.AsSpan(0, PlainFloat.Size / 2)));

      var value1 = new PlainFloat(_buffer.AsSpan(PlainFloat.Size * 0, PlainFloat.Size));
      var value2 = new PlainFloat(_buffer.AsSpan(PlainFloat.Size * 1, PlainFloat.Size));

      Assert.True(value1.GetBuffer().Length == PlainFloat.Size);

      value1.Write(float.MaxValue / 2707);
      value1.CopyTo(value2);
      Assert.True(value1 == value2, "operator `==` failed");

      value2.Write(float.MinValue / 2707);
      Assert.True(value1 != value2, "operator `!=` failed");

      Assert.True(value1.Read() == float.MaxValue / 2707, "value1 read failed");
      Assert.True(value2.Read() == float.MinValue / 2707, "value2 read failed");
    }

    [Fact]
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public void TestPlainDouble() {
      Assert.Throws<InvalidOperationException>(() => _ = new PlainDouble(_buffer.AsSpan(0, PlainDouble.Size / 2)));

      var value1 = new PlainDouble(_buffer.AsSpan(PlainDouble.Size * 0, PlainDouble.Size));
      var value2 = new PlainDouble(_buffer.AsSpan(PlainDouble.Size * 1, PlainDouble.Size));

      Assert.True(value1.GetBuffer().Length == PlainDouble.Size);

      value1.Write(double.MaxValue / 3571);
      value1.CopyTo(value2);
      Assert.True(value1 == value2, "operator `==` failed");

      value2.Write(double.MinValue / 3571);
      Assert.True(value1 != value2, "operator `!=` failed");

      Assert.True(value1.Read() == double.MaxValue / 3571, "value1 read failed");
      Assert.True(value2.Read() == double.MinValue / 3571, "value2 read failed");
    }
  }
}