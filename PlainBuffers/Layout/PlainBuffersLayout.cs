using System;
using System.Collections.Generic;
using System.Linq;
using PlainBuffers.CodeGen.Data;
using PlainBuffers.Layout.Data;
using PlainBuffers.Parser.Data;

namespace PlainBuffers.Layout {
  internal static class PlainBuffersLayout {
    private static readonly Dictionary<string, TypeMemoryInfo> Primitives = new Dictionary<string, TypeMemoryInfo> {
      {"bool", new TypeMemoryInfo(1, "false")},
      {"sbyte", new TypeMemoryInfo(1, "0")},
      {"byte", new TypeMemoryInfo(1, "0")},
      {"short", new TypeMemoryInfo(2, "0")},
      {"ushort", new TypeMemoryInfo(2, "0")},
      {"int", new TypeMemoryInfo(4, "0")},
      {"uint", new TypeMemoryInfo(4, "0")},
      {"float", new TypeMemoryInfo(4, "0")},
      {"long", new TypeMemoryInfo(8, "0")},
      {"ulong", new TypeMemoryInfo(8, "0")},
      {"double", new TypeMemoryInfo(8, "0")}
    };

    public static CodeGenData Calculate(ParsedData parsedData, ExternStructInfo[] externStructs) {
      var typesMemInfo = new Dictionary<string, TypeMemoryInfo>(Primitives);
      foreach (var structInfo in externStructs)
      {
        DefaultValueInfo dvi;
        switch (structInfo.Kind)
        {
          case ExternStructInfo.StructKind.WithoutValues:
            dvi = DefaultValueInfo.WriteZeroes();
            break;
          case ExternStructInfo.StructKind.WithEnumeratedValues:
            dvi = DefaultValueInfo.AssignTypeMember(structInfo.Values[0]);
            break;
          case ExternStructInfo.StructKind.PlainBuffersStruct:
            dvi = DefaultValueInfo.CallWriteDefaultMethod();
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        typesMemInfo.Add(structInfo.Name, new TypeMemoryInfo(structInfo.Size, structInfo.Alignment, dvi));
      }

      var codeGenTypes = new CodeGenType[parsedData.Types.Length];

      for (var i = 0; i < parsedData.Types.Length; i++) {
        CodeGenType codeGenType;
        switch (parsedData.Types[i]) {
          case ParsedEnum pdEnum:
            codeGenType = HandleEnum(pdEnum, typesMemInfo);
            break;
          case ParsedArray pdArray:
            codeGenType = HandleArray(pdArray, typesMemInfo);
            break;
          case ParsedStruct pdStruct:
            codeGenType = HandleStruct(pdStruct, typesMemInfo);
            break;
          default:
            throw new Exception($"Unknown type variant {parsedData.Types[i].GetType().Name}");
        }

        codeGenTypes[i] = codeGenType;
      }

      return new CodeGenData(parsedData.Namespace, codeGenTypes);
    }

    private static CodeGenEnum HandleEnum(ParsedEnum pdEnum, IDictionary<string, TypeMemoryInfo> typesMemInfo) {
      if (!typesMemInfo.TryGetValue(pdEnum.UnderlyingType, out var memInfo))
        throw new Exception($"Invalid base type `{pdEnum.UnderlyingType}` of enum `{pdEnum.Name}`");

      var items = new CodeGenEnumItem[pdEnum.Items.Length];
      for (var i = 0; i < items.Length; i++) {
        items[i] = new CodeGenEnumItem(pdEnum.Items[i].Name, pdEnum.Items[i].Value);
      }

      var defaultValue = DefaultValueInfo.AssignTypeMember(pdEnum.Items[0].Name);
      typesMemInfo.Add(pdEnum.Name, new TypeMemoryInfo(memInfo.Size, memInfo.Size, defaultValue));

      return new CodeGenEnum(pdEnum.Name, memInfo.Size, pdEnum.UnderlyingType, pdEnum.IsFlags, items);
    }

    private static CodeGenArray HandleArray(ParsedArray pdArray, IDictionary<string, TypeMemoryInfo> typesMemInfo) {
      if (!typesMemInfo.TryGetValue(pdArray.ItemType, out var itemMemInfo))
        throw new Exception($"Unknown item type `{pdArray.ItemType}` of array `{pdArray.Name}`");

      var size = itemMemInfo.Size * pdArray.Length;

      var arrayDefaultValue = DefaultValueInfo.CallWriteDefaultMethod();
      typesMemInfo.Add(pdArray.Name, new TypeMemoryInfo(size, itemMemInfo.Alignment, arrayDefaultValue));

      var itemDefaultValue = itemMemInfo.DefaultValueInfo.WithCustomDefaultValueIfPossible(pdArray.ItemDefaultValue);
      return new CodeGenArray(pdArray.Name, size, itemMemInfo.Alignment, pdArray.ItemType, pdArray.Length, itemDefaultValue);
    }

    private static CodeGenStruct HandleStruct(ParsedStruct pdStruct, IDictionary<string, TypeMemoryInfo> typesMemInfo) {
      if (pdStruct.Fields.Length == 0)
        throw new Exception($"Struct `{pdStruct.Name}` is zero-sized");

      var fieldsMemInfo = GetFieldsMemoryInfo(pdStruct, typesMemInfo);

      var offset = 0;
      var fields = new CodeGenField[pdStruct.Fields.Length];
      for (var i = 0; i < fields.Length; i++) {
        var (pdFieldIndex, memInfo) = fieldsMemInfo[i];
        var pdField = pdStruct.Fields[pdFieldIndex];

        var defaultValue = memInfo.DefaultValueInfo.WithCustomDefaultValueIfPossible(pdField.DefaultValue);
        fields[i] = new CodeGenField(pdField.Type, pdField.Name, defaultValue, offset);

        offset += memInfo.Size;
      }

      var unalignedSize = fieldsMemInfo.Sum(fmi => fmi.TypeMemoryInfo.Size);
      var alignment = fieldsMemInfo.Max(fmi => fmi.TypeMemoryInfo.Alignment);

      var reminder = unalignedSize % alignment;
      var padding = reminder == 0 ? 0 : alignment - reminder;
      var size = unalignedSize + padding;

      var structDefaultValue = DefaultValueInfo.CallWriteDefaultMethod();
      typesMemInfo.Add(pdStruct.Name, new TypeMemoryInfo(size, alignment, structDefaultValue));

      return new CodeGenStruct(pdStruct.Name, size, alignment, padding, fields);
    }

    private static FieldMemoryInfo[] GetFieldsMemoryInfo(ParsedStruct pdStruct, IDictionary<string, TypeMemoryInfo> typesMemInfo) {
      var fieldsMemInfo = new FieldMemoryInfo[pdStruct.Fields.Length];

      for (var i = 0; i < fieldsMemInfo.Length; i++) {
        var pdField = pdStruct.Fields[i];
        if (!typesMemInfo.TryGetValue(pdField.Type, out var memInfo))
          throw new Exception($"Unknown field type `{pdField.Type}` defined in the struct `{pdStruct.Name}`");

        fieldsMemInfo[i] = new FieldMemoryInfo(i, memInfo);
      }

      Array.Sort(fieldsMemInfo, (fieldA, fieldB) => {
        var (fieldIndexA, memInfoA) = fieldA;
        var (fieldIndexB, memInfoB) = fieldB;

        if (memInfoA.Alignment != memInfoB.Alignment)
          return memInfoB.Alignment.CompareTo(memInfoA.Alignment);

        return fieldIndexA.CompareTo(fieldIndexB);
      });

      return fieldsMemInfo;
    }
  }
}