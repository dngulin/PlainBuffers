namespace PlainBuffers.CompilerCore.Schema {
  public class StructTypeInfo : BaseTypeInfo {
    public readonly FieldInfo[] Fields;
    public StructTypeInfo(string name, FieldInfo[] fields) : base(name) {
      Fields = fields;
    }
  }

  public class FieldInfo {
    public readonly string Type;
    public readonly string Name;
    public readonly string DefaultValue;

    public FieldInfo(string type, string name, string defaultValue) {
      Type = type;
      Name = name;
      DefaultValue = defaultValue;
    }
  }
}