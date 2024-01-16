using System;
using System.Collections.Generic;
using PlainBuffers.Parser.Data;

namespace PlainBuffers.Parser {
  internal class ParsingIndex {
    private string _namespace;

    private readonly List<(TypeKind, string)> _typesOrder;
    private readonly HashSet<string> _completedTypes;

    private readonly Dictionary<string, (string UnderlyingType, bool IsFlags)> _enums;
    private readonly Dictionary<string, List<ParsedEnumItem>> _enumItems;

    private readonly Dictionary<string, List<ParsedField>> _structFields;
    private readonly HashSet<string> _unions;

    private readonly Dictionary<string, ParsedArray> _arrays;

    public ParsingIndex() {
      _typesOrder = new List<(TypeKind, string)>();
      _completedTypes = new HashSet<string>();

      _enums = new Dictionary<string, (string, bool)>();
      _enumItems = new Dictionary<string, List<ParsedEnumItem>>();

      _arrays = new Dictionary<string, ParsedArray>();

      _structFields = new Dictionary<string, List<ParsedField>>();
      _unions = new HashSet<string>();
    }

    public void SetNamespace(string ns) => _namespace = ns;

    public void BeginEnum(string name, string underlyingType, bool isFlags) {
      _typesOrder.Add((TypeKind.Enum, name));

      _enums.Add(name, (underlyingType, isFlags));
      _enumItems.Add(name, new List<ParsedEnumItem>());
    }

    public void PutEnumItem(string enumName, string itemName, string itemValue) {
      _enumItems[enumName].Add(new ParsedEnumItem(itemName, itemValue));
    }

    public void EndEnum(string name) {
      if (!_enums.ContainsKey(name) || _completedTypes.Contains(name))
        throw new InvalidOperationException();

      _completedTypes.Add(name);
    }

    public void PutArray(string name, string itemType, int length, string defaultItemValue) {
      _typesOrder.Add((TypeKind.Array, name));
      _arrays.Add(name, new ParsedArray(name, itemType, length, defaultItemValue));

      _completedTypes.Add(name);
    }

    public void BeginStruct(string name, bool isUnion) {
      _typesOrder.Add((TypeKind.Struct, name));
      _structFields.Add(name, new List<ParsedField>());

      if (isUnion)
        _unions.Add(name);
    }

    public void PutStructField(string structName, string fieldType, string fieldName, string fieldValue) {
      _structFields[structName].Add(new ParsedField(fieldType, fieldName, fieldValue));
    }

    public void EndStruct(string name) {
      if (!_structFields.ContainsKey(name) || _completedTypes.Contains(name))
        throw new InvalidOperationException();

      _completedTypes.Add(name);
    }

    public bool ContainsCompletedType(string type) => _completedTypes.Contains(type);

    public bool ContainsEnum(string enumName) => _enums.ContainsKey(enumName);

    public string GetEnumUnderlyingType(string enumName) => _enums[enumName].UnderlyingType;

    public bool IsEnumNonEmpty(string structName) => _enumItems[structName].Count > 0;

    public bool IsEnumContainsItem(string enumName, string itemName) {
      foreach (var i in _enumItems[enumName]) {
        if (i.Name == itemName) return true;
      }

      return false;
    }

    public bool IsStructNonEmpty(string structName) => _structFields[structName].Count > 0;

    public bool IsStructContainsField(string structName, string fieldName) {
      foreach (var f in _structFields[structName]) {
        if (f.Name == fieldName) return true;
      }

      return false;
    }

    public ParsedData BuildParsedData() {
      var types = new ParsedType[_typesOrder.Count];

      for (var i = 0; i < _typesOrder.Count; i++) {
        var (typeKind, typeName) = _typesOrder[i];
        switch (typeKind) {
          case TypeKind.Enum:
            var (underlyingType, isFlags) = _enums[typeName];
            types[i] = new ParsedEnum(typeName, underlyingType, isFlags, _enumItems[typeName].ToArray());
            break;

          case TypeKind.Array:
            types[i] = _arrays[typeName];
            break;

          case TypeKind.Struct:
            types[i] = new ParsedStruct(typeName, _structFields[typeName].ToArray(), _unions.Contains(typeName));
            break;

          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      return new ParsedData {Namespace = _namespace, Types = types};
    }
  }
}