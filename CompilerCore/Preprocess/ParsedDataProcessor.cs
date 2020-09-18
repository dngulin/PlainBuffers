using System;
using System.Linq;
using PlainBuffers.CompilerCore.Schema;

namespace PlainBuffers.CompilerCore.Preprocess {
  internal static class ParsedDataProcessor {
    public static CodeGenData Process(ParsedData parsedData) {
      var index = new ProcessingIndex();
      var codeGenTypes = new CodeGenType[parsedData.Types.Length];

      for (var i = 0; i < parsedData.Types.Length; i++) {
        CodeGenType codeGenType;
        switch (parsedData.Types[i]) {
          case ParsedEnumType pdEnum:
            codeGenType = HandleEnum(pdEnum, index);
            break;
          case ParsedArrayType pdArray:
            codeGenType = HandleArray(pdArray, index);
            break;
          case ParsedStruct pdStruct:
            codeGenType = HandleStruct(pdStruct, index);
            break;
          default:
            throw new Exception($"Unknown type variant {parsedData.Types[i].GetType().Name}");
        }

        codeGenTypes[i] = codeGenType;
      }

      return new CodeGenData(parsedData.NameSpace, codeGenTypes);
    }

    private static CodeGenEnum HandleEnum(ParsedEnumType pdEnum, ProcessingIndex index) {
      if (!index.Types.TryGetValue(pdEnum.UnderlyingType, out var memInfo))
        throw new Exception($"Invalid base type `{pdEnum.UnderlyingType}` of enum `{pdEnum.Name}`");

      var items = new CodeGenEnumItem[pdEnum.Items.Length];
      for (var i = 0; i < items.Length; i++) {
        // TODO: Explicitly define all enum values?
        items[i] = new CodeGenEnumItem(pdEnum.Items[i].Name, pdEnum.Items[i].Value);
      }

      index.Types.Add(pdEnum.Name, new TypeMemoryInfo(memInfo.Size));
      index.Enums.Add(pdEnum.Name);

      return new CodeGenEnum(pdEnum.Name, memInfo.Size, pdEnum.UnderlyingType, pdEnum.IsFlags, items);
    }

    private static CodeGenArray HandleArray(ParsedArrayType pdArray, ProcessingIndex index) {
      if (!index.Types.TryGetValue(pdArray.ItemType, out var itemMemInfo))
        throw new Exception($"Unknown item type `{pdArray.ItemType}` of array `{pdArray.Name}`");

      var size = itemMemInfo.Size * pdArray.Length;

      // TODO: Set default value for primitive type if it is not parsed
      var defaultValue = pdArray.ItemDefaultValue;

      var isEnumItem = index.Enums.Contains(pdArray.ItemType);
      index.Types.Add(pdArray.Name, new TypeMemoryInfo(size, itemMemInfo.Alignment));

      return new CodeGenArray(pdArray.Name, size, pdArray.ItemType, pdArray.Length, defaultValue, isEnumItem);
    }

    private static CodeGenStruct HandleStruct(ParsedStruct pdStruct, ProcessingIndex index) {
      if (pdStruct.Fields.Length == 0)
        throw new Exception($"Struct `{pdStruct.Name}` is zero-sized");

      var fieldsMemoryInfo = GetFieldsMemoryInfo(pdStruct, index);

      var offset = 0;
      var fields = new CodeGenField[pdStruct.Fields.Length];
      for (var i = 0; i < fields.Length; i++) {
        var (pdFieldIndex, memInfo) = fieldsMemoryInfo[i];
        var pdField = pdStruct.Fields[pdFieldIndex];

        // TODO: Set default value for primitive type if it is not parsed
        var defaultValue = pdField.DefaultValue;

        var isEnum = index.Enums.Contains(pdField.Type);
        fields[i] = new CodeGenField(pdField.Type, pdField.Name, defaultValue, offset, isEnum);

        offset += memInfo.Size;
      }

      var unalignedSize = fieldsMemoryInfo.Sum(fmi => fmi.TypeMemoryInfo.Size);
      var alignment = fieldsMemoryInfo.Max(fmi => fmi.TypeMemoryInfo.Alignment);

      var reminder = unalignedSize % alignment;
      var padding = reminder == 0 ? 0 : alignment - reminder;
      var size = unalignedSize + padding;

      index.Types.Add(pdStruct.Name, new TypeMemoryInfo(size, alignment));

      return new CodeGenStruct(pdStruct.Name, size, padding, fields);
    }

    private static FieldMemoryInfo[] GetFieldsMemoryInfo(ParsedStruct pdStruct, ProcessingIndex index) {
      var fieldsMemInfo = new FieldMemoryInfo[pdStruct.Fields.Length];

      for (var i = 0; i < fieldsMemInfo.Length; i++) {
        var pdField = pdStruct.Fields[i];
        if (!index.Types.TryGetValue(pdField.Type, out var memInfo))
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