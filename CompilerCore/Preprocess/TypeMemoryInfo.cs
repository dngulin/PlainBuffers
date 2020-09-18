namespace PlainBuffers.CompilerCore.Preprocess {
  internal readonly struct TypeMemoryInfo {
    public readonly int Size;
    public readonly int Alignment;

    public TypeMemoryInfo(int size) {
      Size = size;
      Alignment = size;
    }

    public TypeMemoryInfo(int size, int alignment) {
      Size = size;
      Alignment = alignment;
    }
  }
}