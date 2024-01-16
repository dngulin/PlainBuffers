using PlainBuffers.Tests.Generated;
using Xunit;

namespace PlainBuffers.Tests {
  public class GeneratedCodeTests {
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
      foreach (var item in array.RefReadonlyIter())
        Assert.True(item == -1);

      for (short i = 0; i < HandleArray5.Length; i++) {
        array.RefAt(i) = i;
      }

      Assert.True(array.Item3 == 3);

      for (short i = 0; i < HandleArray5.Length; i++) {
        Assert.True(array.RefReadonlyAt(i) == i);
      }

      foreach (ref var item in array.RefIter())
        item = -2;

      foreach (ref readonly var item in array.RefReadonlyIter())
        Assert.True(item == -2);
    }

    [Fact]
    public void TestUnion() {
      var union = new VecQuat();

      union.Vec.X = 1;
      Assert.Equal(1, union.Quat.X);
      Assert.Equal(union.Quat.X, union.Vec.X);

      union.Vec.Y = 2;
      Assert.Equal(2, union.Quat.Y);
      Assert.Equal(union.Quat.Y, union.Vec.Y);

      union.Vec.Z = 3;
      Assert.Equal(3, union.Quat.Z);
      Assert.Equal(union.Quat.Z, union.Vec.Z);
    }
  }
}