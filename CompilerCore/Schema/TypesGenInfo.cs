namespace PlainBuffers.CompilerCore.Schema {
  public class TypesGenInfo {
    public readonly string NameSpace;
    public readonly TypeGenInfo[] Types;

    public TypesGenInfo(string nameSpace, TypeGenInfo[] types) {
      NameSpace = nameSpace;
      Types = types;
    }
  }

  public class TypeGenInfo {
    public readonly int Size;

    protected TypeGenInfo(int size) {
      Size = size;
    }
  }

  public class EnumGenInfo : TypeGenInfo {
    public readonly EnumTypeInfo TypeInfo;
    public EnumGenInfo(EnumTypeInfo typeInfo, int size) : base(size) {
      TypeInfo = typeInfo;
    }
  }

  public class ArrayGenInfo : TypeGenInfo {
    public readonly ArrayTypeInfo TypeInfo;
    public readonly bool IsItemTypeEnum;

    public ArrayGenInfo(ArrayTypeInfo typeInfo, int size, bool isItemTypeEnum) : base(size) {
      TypeInfo = typeInfo;
      IsItemTypeEnum = isItemTypeEnum;
    }
  }

  public class StructGenInfo : TypeGenInfo {
    public readonly string Type;
    public readonly int Padding;
    public readonly FieldGenInfo[] Fields;

    public StructGenInfo(string type, int size, int padding, FieldGenInfo[] fields) : base(size) {
      Type = type;
      Padding = padding;
      Fields = fields;
    }
  }

  public class FieldGenInfo {
    public readonly FieldInfo FieldInfo;
    public readonly int Offset;
    public readonly bool IsFieldTypeEnum;

    public FieldGenInfo(FieldInfo fieldInfo, int offset, bool isFieldTypeEnum) {
      FieldInfo = fieldInfo;
      Offset = offset;
      IsFieldTypeEnum = isFieldTypeEnum;
    }
  }
}