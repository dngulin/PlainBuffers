namespace PlainBuffers.CodeGen.Data {
  public class CodeGenArray : CodeGenType {
    public readonly string ItemType;
    public readonly int Length;
    public readonly DefaultValueInfo ItemDefaultValueInfo;

    public CodeGenArray(string name, int size, int alignment, string itemType, int length, DefaultValueInfo itemDefaultValueInfo)
      : base(name, size, alignment) {
      ItemType = itemType;
      Length = length;
      ItemDefaultValueInfo = itemDefaultValueInfo;
    }
  }
}