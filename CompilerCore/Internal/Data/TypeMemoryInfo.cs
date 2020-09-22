namespace PlainBuffers.CompilerCore.Internal.Data {
  internal class TypeMemoryInfo {
    public readonly int Size;
    public readonly int Alignment;
    public readonly string DefaultValue;

    public TypeMemoryInfo(int size, string defaultValue) {
      Size = size;
      Alignment = size;
      DefaultValue = defaultValue;
    }

    public TypeMemoryInfo(int size, int alignment, string defaultValue) {
      Size = size;
      Alignment = alignment;
      DefaultValue = defaultValue;
    }
  }
}