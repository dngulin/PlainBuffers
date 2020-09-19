namespace PlainBuffers.CompilerCore.Generate.Data {
  public class CodeGenField {
    public readonly string Type;
    public readonly string Name;
    public readonly string DefaultValue;
    public readonly int Offset;
    public readonly bool IsFieldTypeEnum;

    public CodeGenField(string type, string name, string defaultValue, int offset, bool isFieldTypeEnum) {
      Type = type;
      Name = name;
      DefaultValue = defaultValue;
      Offset = offset;
      IsFieldTypeEnum = isFieldTypeEnum;
    }
  }
}