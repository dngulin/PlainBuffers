using System;
using Xunit;

namespace PlainBuffers.Core.Tests {
  public class PrimitiveTypesTests {
    private readonly byte[] _buffer = new byte[16];

    [Fact]
    public void TestPlainBool() {
      var value1 = new PlainBool(_buffer.AsSpan(PlainBool.Size * 0, PlainBool.Size));
      var value2 = new PlainBool(_buffer.AsSpan(PlainBool.Size * 1, PlainBool.Size));

      value1.Write(true);
      value1.CopyTo(value2);
      Assert.True(value1 == value2, "operator `==` failed");

      value2.Write(false);
      Assert.True(value1 != value2, "operator `!=` failed");

      Assert.True(value1.Read(), "value1 read failed");
      Assert.False(value2.Read(), "value2 read failed");

      Assert.Throws<InvalidOperationException>(() => _ = new PlainBool(_buffer.AsSpan(0, 0)));
    }
  }
}