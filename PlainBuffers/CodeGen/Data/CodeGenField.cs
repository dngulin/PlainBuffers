namespace PlainBuffers.CodeGen.Data {
  public class CodeGenField {
    public readonly string Type;
    public readonly string Name;
    public readonly DefaultValueInfo DefaultValueInfo;
    public readonly int Offset;

    public CodeGenField(string type, string name, DefaultValueInfo defaultValueInfo, int offset) {
      Type = type;
      Name = name;
      DefaultValueInfo = defaultValueInfo;
      Offset = offset;
    }
  }
}