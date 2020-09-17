namespace PlainBuffers.CompilerCore.Schema {
  public class ArrayTypeInfo : BaseTypeInfo {
    public readonly string ItemType;
    public readonly int Length;
    public readonly string ItemDefaultValue;

    public ArrayTypeInfo(string name, string itemType, int length, string itemDefaultValue) : base(name) {
      ItemType = itemType;
      Length = length;
      ItemDefaultValue = itemDefaultValue;
    }
  }
}