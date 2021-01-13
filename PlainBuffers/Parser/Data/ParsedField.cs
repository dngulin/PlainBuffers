namespace PlainBuffers.Parser.Data {
  internal class ParsedField {
    public string Type;
    public readonly string Name;
    public string DefaultValue;

    public ParsedField(string type, string name, string defaultValue) {
      Type = type;
      Name = name;
      DefaultValue = defaultValue;
    }
  }
}