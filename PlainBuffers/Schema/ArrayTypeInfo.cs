namespace PlainBuffers.Schema {
  public class ArrayTypeInfo : BaseTypeInfo {
    public readonly string ItemType;
    public readonly int Length;
    public readonly string ItemDefaultValue;

    public ArrayTypeInfo(string name, int size, int alignment, string itemType, int length, string itemDefaultValue)
      : base(name, size, alignment) {
      ItemType = itemType;
      Length = length;
      ItemDefaultValue = itemDefaultValue;
    }
  }
}