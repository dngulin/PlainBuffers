namespace PlainBuffers.CompilerCore.Parsing.Data {
  public class ParsedStruct : ParsedType {
    public readonly ParsedField[] Fields;
    public ParsedStruct(string name, ParsedField[] fields) : base(name) {
      Fields = fields;
    }
  }
}