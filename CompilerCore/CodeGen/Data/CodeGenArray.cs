namespace PlainBuffers.CompilerCore.CodeGen.Data {
  public class CodeGenArray : CodeGenType {
    public readonly string ItemType;
    public readonly int Length;
    public readonly string ItemDefaultValue;

    public CodeGenArray(string name, int size, string itemType, int length, string itemDefaultValue) : base(name, size) {
      ItemType = itemType;
      Length = length;
      ItemDefaultValue = itemDefaultValue;
    }
  }
}