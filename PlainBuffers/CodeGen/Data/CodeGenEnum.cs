namespace PlainBuffers.CodeGen.Data {
  public class CodeGenEnum : CodeGenType {
    public readonly string UnderlyingType;
    public readonly bool IsFlags;
    public readonly CodeGenEnumItem[] Items;

    public CodeGenEnum(string name, int size, string underlyingType, bool isFlags, CodeGenEnumItem[] items) : base(name, size, size) {
      UnderlyingType = underlyingType;
      IsFlags = isFlags;
      Items = items;
    }
  }
}