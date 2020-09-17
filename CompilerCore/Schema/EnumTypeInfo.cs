namespace PlainBuffers.CompilerCore.Schema {
  public class EnumTypeInfo : BaseTypeInfo {
    public readonly string UnderlyingType;
    public readonly bool IsFlags;
    public readonly EnumItemInfo[] Items;

    public EnumTypeInfo(string name, string underlyingType, bool isFlags, EnumItemInfo[] items) : base(name) {
      UnderlyingType = underlyingType;
      IsFlags = isFlags;
      Items = items;
    }
  }

  public class EnumItemInfo {
    public readonly string Name;
    public readonly string Value;

    public EnumItemInfo(string name, string value) {
      Name = name;
      Value = value;
    }
  }
}