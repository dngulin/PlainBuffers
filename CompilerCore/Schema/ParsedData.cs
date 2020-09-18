namespace PlainBuffers.CompilerCore.Schema {
  public class ParsedData {
    public string NameSpace;
    public ParsedType[] Types;
  }

  public abstract class ParsedType {
    public readonly string Name;

    protected ParsedType(string name) {
      Name = name;
    }
  }

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

  public class ParsedEnumType : ParsedType {
    public readonly string UnderlyingType;
    public readonly bool IsFlags;
    public readonly ParsedEnumItem[] Items;

    public ParsedEnumType(string name, string underlyingType, bool isFlags, ParsedEnumItem[] items) : base(name) {
      UnderlyingType = underlyingType;
      IsFlags = isFlags;
      Items = items;
    }
  }

  public class ParsedEnumItem {
    public readonly string Name;
    public readonly string Value;

    public ParsedEnumItem(string name, string value) {
      Name = name;
      Value = value;
    }
  }

  public class ParsedStruct : ParsedType {
    public readonly ParsedField[] Fields;
    public ParsedStruct(string name, ParsedField[] fields) : base(name) {
      Fields = fields;
    }
  }

  public class ParsedField {
    public readonly string Type;
    public readonly string Name;
    public readonly string DefaultValue;

    public ParsedField(string type, string name, string defaultValue) {
      Type = type;
      Name = name;
      DefaultValue = defaultValue;
    }
  }
}