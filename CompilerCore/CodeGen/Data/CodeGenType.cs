namespace PlainBuffers.CompilerCore.CodeGen.Data {
  public abstract class CodeGenType {
    public readonly string Name;
    public readonly int Size;

    protected CodeGenType(string name, int size) {
      Size = size;
      Name = name;
    }
  }
}