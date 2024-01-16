namespace PlainBuffers.Parser.Data {
  internal class ParsedStruct : ParsedType {
    public readonly ParsedField[] Fields;
    public readonly bool IsUnion;
    public ParsedStruct(string name, ParsedField[] fields, bool isUnion) : base(name) {
      Fields = fields;
      IsUnion = isUnion;
    }
  }
}