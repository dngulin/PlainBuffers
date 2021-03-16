namespace PlainBuffers.CodeGen.Data {
  public abstract class CodeGenType {
    public readonly string Name;
    public readonly int Size;
    public readonly int Alignment;

    protected CodeGenType(string name, int size, int alignment) {
      Size = size;
      Alignment = alignment;
      Name = name;
    }
  }
}