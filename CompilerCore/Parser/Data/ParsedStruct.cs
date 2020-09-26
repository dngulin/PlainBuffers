namespace PlainBuffers.CompilerCore.Parser.Data {
  internal class ParsedStruct : ParsedType {
    public readonly ParsedField[] Fields;
    public ParsedStruct(string name, ParsedField[] fields) : base(name) {
      Fields = fields;
    }
  }
}