namespace PlainBuffers.CompilerCore.CodeGen.Data {
  public class CodeGenField {
    public readonly string Type;
    public readonly string Name;
    public readonly string DefaultValue;
    public readonly int Offset;

    public CodeGenField(string type, string name, string defaultValue, int offset) {
      Type = type;
      Name = name;
      DefaultValue = defaultValue;
      Offset = offset;
    }
  }
}