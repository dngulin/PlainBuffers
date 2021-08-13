using System.IO;
using PlainBuffers.CodeGen;

namespace PlainBuffers.Generators
{
  public class CSharpUnityCodeGenerator : CSharpCodeGenerator
  {
    public CSharpUnityCodeGenerator(string[] namespaces) : base(namespaces)
    {
    }

    protected override void WriteHeader(TextWriter writer)
    {
      base.WriteHeader(writer);
      writer.WriteLine("using Unity.Collections.LowLevel.Unsafe;");
    }

    protected override void WriteEqualityOperators(string type, in BlockWriter typeBlock) {
      using (var eqBlock = typeBlock.Sub($"public static bool operator ==(in {type} l, in {type} r)")) {
        using (var rFxd = eqBlock.Sub("fixed (byte* __l = l._buffer, __r = r._buffer)")) {
          rFxd.WriteLine("return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;");
        }
      }

      typeBlock.WriteLine($"public static bool operator !=(in {type} l, in {type} r) => !(l == r);");
      typeBlock.WriteLine();
      typeBlock.WriteLine($"public override bool Equals(object obj) => obj is {type} casted && this == casted;");
      typeBlock.WriteLine("public override int GetHashCode() => throw new NotSupportedException();");
    }

    protected override void WritePaddingFiller(in BlockWriter writeDefaultBlock)
    {
      using (var fxdBlock = writeDefaultBlock.Sub("fixed (byte* __ptr = _buffer)")) {
        fxdBlock.WriteLine("UnsafeUtility.MemClear(__ptr + (SizeOf - _Padding), _Padding);");
      }
    }
  }
}