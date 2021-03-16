namespace PlainBuffers.CodeGen.Data {
  public class CodeGenStruct : CodeGenType {
    public readonly int Padding;
    public readonly CodeGenField[] Fields;

    public CodeGenStruct(string name, int size, int alignment, int padding, CodeGenField[] fields) : base(name, size, alignment) {
      Padding = padding;
      Fields = fields;
    }
  }
}