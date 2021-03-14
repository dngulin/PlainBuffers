using PlainBuffers.Tests.Generated;
using Xunit;

namespace PlainBuffers.Tests {
  public class SchemaFixedBuffersTests {
    [Fact]
    public void TestQuatStruct() {
      var quat1 = new Quat {X = 1, Y = 2, Z = 3, W = 4};

      Assert.Equal(1, quat1.X);
      Assert.Equal(2, quat1.Y);
      Assert.Equal(3, quat1.Z);
      Assert.Equal(4, quat1.W);

      var quat2 = quat1;
      Assert.True(quat1 == quat2);

      quat1.WriteDefault();
      Assert.True(quat1 != quat2);

      quat2.WriteDefault();
      Assert.True(quat1 == quat2);
    }

    [Fact]
    public void TestArray() {
      var array = new HandleArray5();

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