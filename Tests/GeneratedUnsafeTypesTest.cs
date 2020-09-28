using System;
using PlainBuffers.Tests.GeneratedUnsafe;
using Xunit;

namespace PlainBuffers.Tests {
  public unsafe class GeneratedUnsafeTypesTest {
    private readonly byte[] _buffer = new byte[Monster.SizeOf * 2];

    [Fact]
    public void TestChecksForMatchingSize() {
      Assert.Throws<InvalidOperationException>(() => {
        fixed (byte* buffer = _buffer) {
          _ = Vec.CreateSafe(buffer, Vec.SizeOf / 2);
        }
      });

      Assert.Throws<InvalidOperationException>(() => {
        fixed (byte* ptr = _buffer) {
          _ = HandleArray5.CreateSafe(ptr, HandleArray5.SizeOf / 2);
        }
      });
    }

    [Fact]
    public void TestQuatStruct() {
      fixed (byte* buffer = _buffer) {
        var quat1 = Quat.CreateSafe(buffer + Quat.SizeOf * 0, Quat.SizeOf);
        var quat2 = Quat.CreateSafe(buffer + Quat.SizeOf * 1, Quat.SizeOf);

        quat1.X = 1;
        quat1.Y = 2;
        quat1.Z = 3;
        quat1.W = 4;

        Assert.Equal(1, quat1.X);
        Assert.Equal(2, quat1.Y);
        Assert.Equal(3, quat1.Z);
        Assert.Equal(4, quat1.W);

        quat1.CopyTo(quat2);
        Assert.True(quat1 == quat2);

        quat1.WriteDefault();
        Assert.True(quat1 != quat2);

        quat2.WriteDefault();
        Assert.True(quat1 == quat2);
      }
    }

    [Fact]
    public void TestArray() {
      fixed (byte* buffer = _buffer) {
        var array = HandleArray5.CreateSafe(buffer + HandleArray5.SizeOf * 0, HandleArray5.SizeOf);

        array.WriteDefault();
        foreach (var item in array)
          Assert.True(item == -1);

        for (short i = 0; i < HandleArray5.Length; i++) {
          array[i] = i;
        }

        for (short i = 0; i < HandleArray5.Length; i++) {
          Assert.True(array[i] == i);
        }

        foreach (ref var item in array)
          item = -2;

        foreach (var item in array)
          Assert.True(item == -2);
      }
    }
  }
}