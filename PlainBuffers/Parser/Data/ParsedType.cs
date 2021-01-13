namespace PlainBuffers.Parser.Data {
  internal abstract class ParsedType {
    public string Name;

    protected ParsedType(string name) {
      Name = name;
    }
  }
}