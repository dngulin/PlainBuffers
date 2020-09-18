namespace PlainBuffers.CompilerCore.Preprocess {
  internal readonly struct FieldMemoryInfo {
    public readonly int Index;
    public readonly TypeMemoryInfo TypeMemoryInfo;

    public FieldMemoryInfo(int index, TypeMemoryInfo memInfo) {
      Index = index;
      TypeMemoryInfo = memInfo;
    }

    public void Deconstruct(out int index, out TypeMemoryInfo memInfo) {
      index = Index;
      memInfo = TypeMemoryInfo;
    }
  }
}