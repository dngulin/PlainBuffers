namespace PlainBuffers.Parser.Data {
  internal class ParsedEnum : ParsedType {
    public readonly string UnderlyingType;
    public readonly bool IsFlags;
    public readonly ParsedEnumItem[] Items;

    public ParsedEnum(string name, string underlyingType, bool isFlags, ParsedEnumItem[] items) : base(name) {
      UnderlyingType = underlyingType;
      IsFlags = isFlags;
      Items = items;
    }
  }
}