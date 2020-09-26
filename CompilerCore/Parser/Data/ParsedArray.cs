namespace PlainBuffers.CompilerCore.Parser.Data {
  internal class ParsedArray : ParsedType {
    public readonly string ItemType;
    public readonly int Length;
    public readonly string ItemDefaultValue;

    public ParsedArray(string name, string itemType, int length, string itemDefaultValue) : base(name) {
      ItemType = itemType;
      Length = length;
      ItemDefaultValue = itemDefaultValue;
    }
  }
}