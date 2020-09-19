namespace PlainBuffers.CompilerCore.Parse.Data {
  public class ParsedField {
    public readonly string Type;
    public readonly string Name;
    public readonly string DefaultValue;

    public ParsedField(string type, string name, string defaultValue) {
      Type = type;
      Name = name;
      DefaultValue = defaultValue;
    }
  }
}