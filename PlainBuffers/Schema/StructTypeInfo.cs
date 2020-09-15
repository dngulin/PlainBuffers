namespace PlainBuffers.Schema {
  public class StructTypeInfo : BaseTypeInfo {
    public readonly FieldInfo[] Fields;
    public StructTypeInfo(string name, int unalignedSize, int alignment, FieldInfo[] fields)
      : base(name, unalignedSize, alignment) {
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

    public bool IsPrimitive => !string.IsNullOrEmpty(DefaultValue);
  }
}