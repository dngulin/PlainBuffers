namespace PlainBuffers.CompilerCore.Parse.Data {
  public class ParsedEnumItem {
    public readonly string Name;
    public readonly string Value;

    public ParsedEnumItem(string name, string value) {
      Name = name;
      Value = value;
    }
  }
}