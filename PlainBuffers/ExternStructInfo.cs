namespace PlainBuffers {
  public class ExternStructInfo {
    public readonly string Name;
    public readonly int Size;
    public readonly int Alignment;
    public readonly string[] Values;

    public ExternStructInfo(string name, int size, int alignment, string[] values) {
      Name = name;
      Size = size;
      Alignment = alignment;
      Values = values;
    }
  }
}