namespace PlainBuffers.CodeGen.Data {
  public class CodeGenStruct : CodeGenType {
    public readonly int Padding;
    public readonly CodeGenField[] Fields;
    public readonly bool IsUnion;

    public CodeGenStruct(string name, int size, int alignment, int padding, CodeGenField[] fields, bool isUnion) : base(name, size, alignment) {
      Padding = padding;
      Fields = fields;
      IsUnion = isUnion;
    }
  }
}