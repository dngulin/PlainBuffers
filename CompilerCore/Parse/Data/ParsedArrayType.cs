namespace PlainBuffers.CompilerCore.Parse.Data {
  public class ParsedArrayType : ParsedType {
    public readonly string ItemType;
    public readonly int Length;
    public readonly string ItemDefaultValue;

    public ParsedArrayType(string name, string itemType, int length, string itemDefaultValue) : base(name) {
      ItemType = itemType;
      Length = length;
      ItemDefaultValue = itemDefaultValue;
    }
  }
}