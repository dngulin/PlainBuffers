namespace PlainBuffers.CompilerCore.CodeGen.Data {
  public class CodeGenStruct : CodeGenType {
    public readonly int Padding;
    public readonly CodeGenField[] Fields;

    public CodeGenStruct(string name, int size, int padding, CodeGenField[] fields) : base(name, size) {
      Padding = padding;
      Fields = fields;
    }
  }
}