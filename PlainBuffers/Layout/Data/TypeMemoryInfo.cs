using PlainBuffers.CodeGen.Data;

namespace PlainBuffers.Layout.Data {
  internal readonly struct TypeMemoryInfo {
    public readonly int Size;
    public readonly int Alignment;
    public readonly DefaultValueInfo DefaultValueInfo;

    public TypeMemoryInfo(int size, string defaultValue) {
      Size = size;
      Alignment = size;
      DefaultValueInfo = DefaultValueInfo.AssignValue(defaultValue);
    }

    public TypeMemoryInfo(int size, int alignment, DefaultValueInfo defaultValueInfo) {
      Size = size;
      Alignment = alignment;
      DefaultValueInfo = defaultValueInfo;
    }
  }
}