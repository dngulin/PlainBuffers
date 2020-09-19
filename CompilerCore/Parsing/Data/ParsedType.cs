namespace PlainBuffers.CompilerCore.Parsing.Data {
  public abstract class ParsedType {
    public readonly string Name;

    protected ParsedType(string name) {
      Name = name;
    }
  }
}