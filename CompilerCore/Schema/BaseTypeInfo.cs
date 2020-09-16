namespace PlainBuffers.CompilerCore.Schema {
  public class BaseTypeInfo {
    public readonly string Name;

    public readonly int Size;
    public readonly int Alignment;
    public readonly int PaddingSize;

    protected BaseTypeInfo(string name, int unalignedSize, int alignment) {
      Name = name;
      Alignment = alignment;

      var reminder = unalignedSize % alignment;
      PaddingSize = reminder == 0 ? 0 : alignment - reminder;

      Size = unalignedSize + PaddingSize;
    }
  }
}