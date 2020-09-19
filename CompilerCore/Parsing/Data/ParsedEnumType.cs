namespace PlainBuffers.CompilerCore.Parsing.Data {
  public class ParsedEnumType : ParsedType {
    public readonly string UnderlyingType;
    public readonly bool IsFlags;
    public readonly ParsedEnumItem[] Items;

    public ParsedEnumType(string name, string underlyingType, bool isFlags, ParsedEnumItem[] items) : base(name) {
      UnderlyingType = underlyingType;
      IsFlags = isFlags;
      Items = items;
    }
  }
}