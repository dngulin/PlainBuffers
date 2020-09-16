using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using PlainBuffers.CompilerCore.Parse.Xml;
using PlainBuffers.CompilerCore.Schema;

namespace PlainBuffers.CompilerCore.Parse {
  public class XmlParser : IParser {
    private readonly XmlSerializer _serializer = new XmlSerializer(typeof(TypesXml));

    public SchemaInfo Parse(Stream readStream) {
      var schemaXml = (TypesXml) _serializer.Deserialize(readStream);

      if (!ParsingHelper.IsDotSeparatedNameValid(schemaXml.NameSpace))
        throw new Exception($"Invalid namespace `{schemaXml.NameSpace}`");

      var types = new BaseTypeInfo[schemaXml.Types.Length];
      var index = new Dictionary<string, BaseTypeInfo>(schemaXml.Types.Length);

      for (var i = 0; i < schemaXml.Types.Length; i++) {
        var typeXml = schemaXml.Types[i];

        if (!ParsingHelper.IsNameValid(typeXml.Name))
          throw new Exception($"Invalid type name `{typeXml.Name}`");

        if (index.ContainsKey(typeXml.Name))
          throw new Exception($"Type `{typeXml.Name}` is defined twice");

        BaseTypeInfo typeInfo;
        switch (typeXml) {
          case EnumXml enumXml:
            typeInfo = HandleEnum(enumXml);
            break;
          case ArrayXml arrayXml:
            typeInfo = HandleArray(arrayXml, index);
            break;
          case StructXml structXml:
            typeInfo = HandleStruct(structXml, index);
            break;
          default:
            throw new Exception("Internal parsing error: unknown type variant");
        }

        types[i] = typeInfo;
        index.Add(typeInfo.Name, typeInfo);
      }

      return new SchemaInfo { NameSpace = schemaXml.NameSpace, Types = types };
    }

    private static BaseTypeInfo HandleEnum(EnumXml enumXml) {
      var underlyingType = enumXml.UnderlyingType ?? "int";
      if (!ParsingHelper.IsInteger(underlyingType))
        throw new Exception($"Enum `{enumXml.Name}` has wrong underlying type `{underlyingType}`");

      var size = ParsingHelper.GetPrimitiveTypeSize(underlyingType);

      var knownItemNames = new HashSet<string>();
      var items = new EnumItemInfo[enumXml.Items.Length];

      for (var i = 0; i < items.Length; i++) {
        var itemXml = enumXml.Items[i];

        if (!ParsingHelper.IsNameValid(itemXml.Name))
          throw new Exception($"Enum `{enumXml.Name}` contains item with invalid name `{itemXml.Name}`");

        if (knownItemNames.Contains(itemXml.Name))
          throw new Exception($"Enum `{enumXml.Name}` contains more then one item with name `{itemXml.Name}`");

        // TODO: Validate item value

        knownItemNames.Add(itemXml.Name);
        items[i] = new EnumItemInfo(itemXml.Name, itemXml.Value);
      }

      return new EnumTypeInfo( enumXml.Name, size, underlyingType, enumXml.IsFlags, items);
    }

    private static BaseTypeInfo HandleArray(ArrayXml arrayXml, IReadOnlyDictionary<string, BaseTypeInfo> index) {
      if (arrayXml.Length <= 0)
        throw new Exception($"Array type `{arrayXml.Name}` has invalid length {arrayXml.Length}");

      // TODO: Handle primitive types, replace type name

      if (!index.TryGetValue(arrayXml.ItemTypeName, out var itemTypeInfo))
        throw new Exception($"Unknown item type `{arrayXml.ItemTypeName}` used in array type `{arrayXml.Name}`");

      // TODO: validate default value (useful only with primitive type and enum)

      var size = itemTypeInfo.Size * arrayXml.Length;

      return new ArrayTypeInfo(
        arrayXml.Name, size, itemTypeInfo.Alignment,
        arrayXml.ItemTypeName, arrayXml.Length, arrayXml.ItemDefaultValue);
    }

    private static BaseTypeInfo HandleStruct(StructXml structXml, IReadOnlyDictionary<string, BaseTypeInfo> index) {
      if (structXml.Fields.Length <= 0)
        throw new Exception($"Type `{structXml.Name}` is zero-sized");

      var unalignedSize = 0;
      var alignment = sizeof(byte);

      var knownFieldNames = new HashSet<string>();
      var fields = new FieldInfo[structXml.Fields.Length];

      for (var i = 0; i < fields.Length; i++) {
        var fieldXml = structXml.Fields[i];

        if (!ParsingHelper.IsNameValid(fieldXml.Name))
          throw new Exception($"Type `{structXml.Name}` has field with invalid name `{fieldXml.Name}`");

        if (knownFieldNames.Contains(fieldXml.Name))
          throw new Exception($"Type `{structXml.Name}` contains more then one field with name `{fieldXml.Name}`");

        knownFieldNames.Add(fieldXml.Name);

        // TODO: Handle primitive field type

        if (!index.TryGetValue(fieldXml.Type, out var fieldTypeInfo))
          throw new Exception($"Type `{structXml.Name}` has field `{fieldXml.Name}` of unknown type `{fieldXml.Type}`");

        unalignedSize += fieldTypeInfo.Size;
        if (fieldTypeInfo.Alignment >= alignment)
          alignment = fieldTypeInfo.Alignment;

        // TODO: validate default value

        fields[i] = new FieldInfo(fieldXml.Type, fieldXml.Name, fieldXml.Default);
      }

      Array.Sort(fields, (a, b) => {
        var alignmentA = index[a.Type].Alignment;
        var alignmentB = index[b.Type].Alignment;
        if (alignmentA != alignmentB)
          return alignmentB.CompareTo(alignmentA);

        return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
      });

      return new StructTypeInfo(structXml.Name, unalignedSize, alignment, fields);
    }
  }
}