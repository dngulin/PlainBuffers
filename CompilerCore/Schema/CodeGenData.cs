namespace PlainBuffers.CompilerCore.Schema {
  public class CodeGenData {
    public readonly string NameSpace;
    public readonly CodeGenType[] Types;

    public CodeGenData(string nameSpace, CodeGenType[] types) {
      NameSpace = nameSpace;
      Types = types;
    }
  }

  public abstract class CodeGenType {
    public readonly string Name;
    public readonly int Size;

    protected CodeGenType(string name, int size) {
      Size = size;
      Name = name;
    }
  }

  public class CodeGenEnum : CodeGenType {
    public readonly string UnderlyingType;
    public readonly bool IsFlags;
    public readonly CodeGenEnumItem[] Items;

    public CodeGenEnum(string name, int size, string underlyingType, bool isFlags, CodeGenEnumItem[] items) : base(name, size) {
      UnderlyingType = underlyingType;
      IsFlags = isFlags;
      Items = items;
    }
  }

  public class CodeGenEnumItem {
    public readonly string Name;
    public readonly string Value;
    public CodeGenEnumItem(string name, string value) {
      Name = name;
      Value = value;
    }
  }

  public class CodeGenArray : CodeGenType {
    public readonly string ItemType;
    public readonly int Length;
    public readonly string ItemDefaultValue;
    public readonly bool IsItemTypeEnum;

    public CodeGenArray(string name, int size, string itemType, int length, string itemDefaultValue, bool isItemTypeEnum) : base(name, size) {
      ItemType = itemType;
      Length = length;
      ItemDefaultValue = itemDefaultValue;
      IsItemTypeEnum = isItemTypeEnum;
    }
  }

  public class CodeGenStruct : CodeGenType {
    public readonly int Padding;
    public readonly CodeGenField[] Fields;

    public CodeGenStruct(string name, int size, int padding, CodeGenField[] fields) : base(name, size) {
      Padding = padding;
      Fields = fields;
    }
  }

  public class CodeGenField {
    public readonly string Type;
    public readonly string Name;
    public readonly string DefaultValue;
    public readonly int Offset;
    public readonly bool IsFieldTypeEnum;

    public CodeGenField(string type, string name, string defaultValue, int offset, bool isFieldTypeEnum) {
      Type = type;
      Name = name;
      DefaultValue = defaultValue;
      Offset = offset;
      IsFieldTypeEnum = isFieldTypeEnum;
    }
  }
}