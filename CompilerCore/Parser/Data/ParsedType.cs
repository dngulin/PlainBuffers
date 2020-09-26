namespace PlainBuffers.CompilerCore.Parser.Data {
  internal abstract class ParsedType {
    public readonly string Name;

    protected ParsedType(string name) {
      Name = name;
    }
  }
}