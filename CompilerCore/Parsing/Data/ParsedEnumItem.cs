namespace PlainBuffers.CompilerCore.Parsing.Data {
  public class ParsedEnumItem {
    public readonly string Name;
    public readonly string Value;

    public ParsedEnumItem(string name, string value) {
      Name = name;
      Value = value;
    }
  }
}