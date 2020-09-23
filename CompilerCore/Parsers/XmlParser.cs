using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using PlainBuffers.CompilerCore.Parsers.Xml;
using PlainBuffers.CompilerCore.Parsing;
using PlainBuffers.CompilerCore.Parsing.Data;

namespace PlainBuffers.CompilerCore.Parsers {
  public class XmlParser : IParser {
    private readonly XmlSerializer _serializer = new XmlSerializer(typeof(TypesXml));

    public ParsedData Parse(Stream readStream) {
      var schemaXml = (TypesXml) _serializer.Deserialize(readStream);

      if (!SyntaxHelper.IsDotSeparatedNameValid(schemaXml.NameSpace))
        throw new ParsingException($"Invalid namespace `{schemaXml.NameSpace}`");

      var types = new ParsedType[schemaXml.Types.Length];
      var index = new ParsingIndex(SyntaxHelper.Primitives);

      for (var i = 0; i < schemaXml.Types.Length; i++) {
        var typeXml = schemaXml.Types[i];

        if (SyntaxHelper.IsPrimitive(typeXml.Name) || !SyntaxHelper.IsNameValid(typeXml.Name))
          throw new ParsingException($"Invalid type name `{typeXml.Name}`");

        if (index.KnownTypes.Contains(typeXml.Name))
          throw new ParsingException($"Type `{typeXml.Name}` is defined more then once");

        ParsedType type;
        switch (typeXml) {
          case EnumXml enumXml:
            type = HandleEnum(enumXml, index);
            break;
          case ArrayXml arrayXml:
            type = HandleArray(arrayXml, index);
            break;
          case StructXml structXml:
            type = HandleStruct(structXml, index);
            break;
          default:
            throw new ParsingException("Internal parsing error: unknown type variant");
        }

        types[i] = type;
        index.KnownTypes.Add(typeXml.Name);
      }

      return new ParsedData {NameSpace = schemaXml.NameSpace, Types = types};
    }

    private static ParsedEnumType HandleEnum(EnumXml enumXml, ParsingIndex index) {
      var underlyingType = enumXml.UnderlyingType;
      if (!SyntaxHelper.IsInteger(underlyingType))
        throw new ParsingException($"Enum `{enumXml.Name}` has the wrong underlying type `{underlyingType}`");

      if (enumXml.Items.Length == 0)
        throw new ParsingException($"Enum `{enumXml.Name}` is empty. Default value is not assignable");

      var knownItemNames = new HashSet<string>();
      var items = new ParsedEnumItem[enumXml.Items.Length];

      for (var i = 0; i < items.Length; i++) {
        var itemXml = enumXml.Items[i];

        if (!SyntaxHelper.IsNameValid(itemXml.Name))
          throw new ParsingException($"Enum `{enumXml.Name}` contains item with the invalid name `{itemXml.Name}`");

        if (knownItemNames.Contains(itemXml.Name))
          throw new ParsingException(
            $"Enum `{enumXml.Name}` contains more then one item with the name `{itemXml.Name}`");

        if (string.IsNullOrEmpty(itemXml.Value))
          throw new ParsingException($"Value of `{enumXml.Name}.{itemXml.Name}` is not defined");

        if (!SyntaxHelper.IsPrimitiveValueValid(itemXml.Value, enumXml.UnderlyingType))
          throw new ParsingException($"Value of `{enumXml.Name}.{itemXml.Name}` is invalid");

        knownItemNames.Add(itemXml.Name);
        items[i] = new ParsedEnumItem(itemXml.Name, itemXml.Value);
      }

      index.EnumValues.Add(enumXml.Name, items.Select(i => i.Name).ToArray());

      return new ParsedEnumType(enumXml.Name, underlyingType, enumXml.IsFlags, items);
    }

    private static ParsedArrayType HandleArray(ArrayXml arrayXml, ParsingIndex index) {
      if (arrayXml.Length <= 0)
        throw new ParsingException($"Array type `{arrayXml.Name}` has the invalid length {arrayXml.Length}");

      if (!index.KnownTypes.Contains(arrayXml.ItemType))
        throw new ParsingException($"Unknown item type `{arrayXml.ItemType}` used in the array type `{arrayXml.Name}`");

      if (arrayXml.ItemDefault != null && !IsValueValid(arrayXml.ItemDefault, arrayXml.ItemType, index))
        throw new ParsingException($"Array `{arrayXml.Name}` has invalid default item value `{arrayXml.ItemDefault}`");

      return new ParsedArrayType(arrayXml.Name, arrayXml.ItemType, arrayXml.Length, arrayXml.ItemDefault);
    }

    private static ParsedStruct HandleStruct(StructXml structXml, ParsingIndex index) {
      if (structXml.Fields.Length <= 0)
        throw new ParsingException($"Type `{structXml.Name}` is zero-sized");

      var knownFieldNames = new HashSet<string>();
      var fields = new ParsedField[structXml.Fields.Length];

      for (var i = 0; i < fields.Length; i++) {
        var fieldXml = structXml.Fields[i];

        if (!SyntaxHelper.IsNameValid(fieldXml.Name))
          throw new ParsingException($"Type `{structXml.Name}` has field with an invalid name `{fieldXml.Name}`");

        if (knownFieldNames.Contains(fieldXml.Name))
          throw new ParsingException(
            $"Type `{structXml.Name}` contains more then one field with the name `{fieldXml.Name}`");

        knownFieldNames.Add(fieldXml.Name);

        if (!index.KnownTypes.Contains(fieldXml.Type))
          throw new ParsingException(
            $"Field `{structXml.Name}.{fieldXml.Name}` has the invalid type `{fieldXml.Type}`");

        if (fieldXml.Default != null && !IsValueValid(fieldXml.Default, fieldXml.Type, index))
          throw new ParsingException($"Field `{structXml.Name}.{fieldXml.Name}` " +
                                     $"has invalid default value `{fieldXml.Default}`");

        fields[i] = new ParsedField(fieldXml.Type, fieldXml.Name, fieldXml.Default);
      }

      return new ParsedStruct(structXml.Name, fields);
    }

    private static bool IsValueValid(string value, string type, ParsingIndex index) {
      if (SyntaxHelper.IsPrimitive(type))
        return SyntaxHelper.IsPrimitiveValueValid(value, type);

      if (index.EnumValues.TryGetValue(type, out var validValues))
        return validValues.Contains(value);

      return false;
    }
  }
}