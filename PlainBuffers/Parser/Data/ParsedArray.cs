namespace PlainBuffers.Parser.Data {
  internal class ParsedArray : ParsedType {
    public string ItemType;
    public readonly int Length;
    public string ItemDefaultValue;

    public ParsedArray(string name, string itemType, int length, string itemDefaultValue) : base(name) {
      ItemType = itemType;
      Length = length;
      ItemDefaultValue = itemDefaultValue;
    }
  }
}