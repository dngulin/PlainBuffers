using System;
using System.Collections.Generic;
using System.IO;
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
        throw new Exception($"Invalid namespace `{schemaXml.NameSpace}`");

      var types = new ParsedType[schemaXml.Types.Length];
      var knownTypes = new HashSet<string>(SyntaxHelper.Primitives);

      for (var i = 0; i < schemaXml.Types.Length; i++) {
        var typeXml = schemaXml.Types[i];

        if (SyntaxHelper.IsPrimitive(typeXml.Name) || !SyntaxHelper.IsNameValid(typeXml.Name))
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
      if (!SyntaxHelper.IsInteger(underlyingType))
        throw new Exception($"Enum `{enumXml.Name}` has the wrong underlying type `{underlyingType}`");

      var knownItemNames = new HashSet<string>();
      var items = new ParsedEnumItem[enumXml.Items.Length];

      for (var i = 0; i < items.Length; i++) {
        var itemXml = enumXml.Items[i];

        if (!SyntaxHelper.IsNameValid(itemXml.Name))
          throw new Exception($"Enum `{enumXml.Name}` contains item with the invalid name `{itemXml.Name}`");

        if (knownItemNames.Contains(itemXml.Name))
          throw new Exception($"Enum `{enumXml.Name}` contains more then one item with the name `{itemXml.Name}`");

        if (string.IsNullOrEmpty(itemXml.Value))
          throw new Exception($"Value of `{enumXml.Name}.{itemXml.Name}` is not defined");

        if (!SyntaxHelper.IsPrimitiveValueValid(itemXml.Value, enumXml.UnderlyingType))
          throw new Exception($"Value of `{enumXml.Name}.{itemXml.Name}` is invalid");

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

      var hasDefaultValue = string.IsNullOrEmpty(arrayXml.ItemDefaultValue);
      if (hasDefaultValue && !SyntaxHelper.IsPrimitive(arrayXml.ItemTypeName))
        throw new Exception($"Default value is set for non-primitive type in the array type `{arrayXml.Name}`");

      if (hasDefaultValue && !SyntaxHelper.IsPrimitiveValueValid(arrayXml.ItemDefaultValue, arrayXml.ItemTypeName))
        throw new Exception($"Invalid default item value is defined for array `{arrayXml.Name}`");

      return new ParsedArrayType(arrayXml.Name, arrayXml.ItemTypeName, arrayXml.Length, arrayXml.ItemDefaultValue);
    }

    private static ParsedStruct HandleStruct(StructXml structXml, ICollection<string> knownTypes) {
      if (structXml.Fields.Length <= 0)
        throw new Exception($"Type `{structXml.Name}` is zero-sized");

      var knownFieldNames = new HashSet<string>();
      var fields = new ParsedField[structXml.Fields.Length];

      for (var i = 0; i < fields.Length; i++) {
        var fieldXml = structXml.Fields[i];

        if (!SyntaxHelper.IsNameValid(fieldXml.Name))
          throw new Exception($"Type `{structXml.Name}` has field with an invalid name `{fieldXml.Name}`");

        if (knownFieldNames.Contains(fieldXml.Name))
          throw new Exception($"Type `{structXml.Name}` contains more then one field with the name `{fieldXml.Name}`");

        knownFieldNames.Add(fieldXml.Name);

        if (!knownTypes.Contains(fieldXml.Type))
          throw new Exception($"Field `{structXml.Name}.{fieldXml.Name}` has the invalid type `{fieldXml.Type}`");

        var hasDefaultValue = string.IsNullOrEmpty(fieldXml.Default);
        if (hasDefaultValue && !SyntaxHelper.IsPrimitive(fieldXml.Type))
          throw new Exception($"Field `{structXml.Name}.{fieldXml.Name}` of the non-primitive type `{fieldXml.Type}`" +
                              $" has the default value `{fieldXml.Default}`");

        if (hasDefaultValue && !SyntaxHelper.IsPrimitiveValueValid(fieldXml.Default, fieldXml.Type))
          throw new Exception($"Field `{structXml.Name}.{fieldXml.Name}` " +
                              $"has the invalid default value `{fieldXml.Default}`");

        fields[i] = new ParsedField(fieldXml.Type, fieldXml.Name, fieldXml.Default);
      }

      return new ParsedStruct(structXml.Name, fields);
    }
  }
}