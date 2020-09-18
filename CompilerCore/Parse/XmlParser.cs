using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using PlainBuffers.CompilerCore.Parse.Xml;
using PlainBuffers.CompilerCore.Schema;

namespace PlainBuffers.CompilerCore.Parse {
  public class XmlParser : IParser {
    private readonly XmlSerializer _serializer = new XmlSerializer(typeof(TypesXml));

    public ParsedData Parse(Stream readStream) {
      var schemaXml = (TypesXml) _serializer.Deserialize(readStream);

      if (!ParsingHelper.IsDotSeparatedNameValid(schemaXml.NameSpace))
        throw new Exception($"Invalid namespace `{schemaXml.NameSpace}`");

      var types = new ParsedType[schemaXml.Types.Length];
      var knownTypes = new HashSet<string>(ParsingHelper.Primitives);

      for (var i = 0; i < schemaXml.Types.Length; i++) {
        var typeXml = schemaXml.Types[i];

        if (ParsingHelper.IsPrimitive(typeXml.Name) || !ParsingHelper.IsNameValid(typeXml.Name))
          throw new Exception($"Invalid type name `{typeXml.Name}`");

        if (knownTypes.Contains(typeXml.Name))
          throw new Exception($"Type `{typeXml.Name}` is defined more then once");

        ParsedType type;
        switch (typeXml) {
          case EnumXml enumXml:
            type = HandleEnum(enumXml);
            break;
          case ArrayXml arrayXml:
            type = HandleArray(arrayXml, knownTypes);
            break;
          case StructXml structXml:
            type = HandleStruct(structXml, knownTypes);
            break;
          default:
            throw new Exception("Internal parsing error: unknown type variant");
        }

        types[i] = type;
        knownTypes.Add(typeXml.Name);
      }

      return new ParsedData { NameSpace = schemaXml.NameSpace, Types = types };
    }

    private static ParsedEnumType HandleEnum(EnumXml enumXml) {
      var underlyingType = enumXml.UnderlyingType;
      if (!ParsingHelper.IsInteger(underlyingType))
        throw new Exception($"Enum `{enumXml.Name}` has the wrong underlying type `{underlyingType}`");

      var knownItemNames = new HashSet<string>();
      var items = new ParsedEnumItem[enumXml.Items.Length];

      for (var i = 0; i < items.Length; i++) {
        var itemXml = enumXml.Items[i];

        if (!ParsingHelper.IsNameValid(itemXml.Name))
          throw new Exception($"Enum `{enumXml.Name}` contains item with the invalid name `{itemXml.Name}`");

        if (knownItemNames.Contains(itemXml.Name))
          throw new Exception($"Enum `{enumXml.Name}` contains more then one item with the name `{itemXml.Name}`");

        // TODO: validate item value syntax

        knownItemNames.Add(itemXml.Name);
        items[i] = new ParsedEnumItem(itemXml.Name, itemXml.Value);
      }

      return new ParsedEnumType(enumXml.Name, underlyingType, enumXml.IsFlags, items);
    }

    private static ParsedArrayType HandleArray(ArrayXml arrayXml, ICollection<string> knownTypes) {
      if (arrayXml.Length <= 0)
        throw new Exception($"Array type `{arrayXml.Name}` has the invalid length {arrayXml.Length}");

      if (!knownTypes.Contains(arrayXml.ItemTypeName))
        throw new Exception($"Unknown item type `{arrayXml.ItemTypeName}` used in the array type `{arrayXml.Name}`");

      // TODO: validate default value syntax

      return new ParsedArrayType(arrayXml.Name, arrayXml.ItemTypeName, arrayXml.Length, arrayXml.ItemDefaultValue);
    }

    private static ParsedStruct HandleStruct(StructXml structXml, ICollection<string> knownTypes) {
      if (structXml.Fields.Length <= 0)
        throw new Exception($"Type `{structXml.Name}` is zero-sized");

      var knownFieldNames = new HashSet<string>();
      var fields = new ParsedField[structXml.Fields.Length];

      for (var i = 0; i < fields.Length; i++) {
        var fieldXml = structXml.Fields[i];

        if (!ParsingHelper.IsNameValid(fieldXml.Name))
          throw new Exception($"Type `{structXml.Name}` has field with an invalid name `{fieldXml.Name}`");

        if (knownFieldNames.Contains(fieldXml.Name))
          throw new Exception($"Type `{structXml.Name}` contains more then one field with the name `{fieldXml.Name}`");

        knownFieldNames.Add(fieldXml.Name);

        if (!knownTypes.Contains(fieldXml.Type))
          throw new Exception($"Type `{structXml.Name}` has the field `{fieldXml.Name}` of the unknown type `{fieldXml.Type}`");

        // TODO: validate default value syntax

        fields[i] = new ParsedField(fieldXml.Type, fieldXml.Name, fieldXml.Default);
      }

      return new ParsedStruct(structXml.Name, fields);
    }
  }
}