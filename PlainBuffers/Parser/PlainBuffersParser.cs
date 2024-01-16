using System;
using System.Collections.Generic;
using System.Linq;
using PlainBuffers.Lexer.Data;
using ParsingResult = PlainBuffers.ErrorHandling.Result<PlainBuffers.Parser.Data.ParsedData, string>;
using OpResult = PlainBuffers.ErrorHandling.VoidResult<string>;

namespace PlainBuffers.Parser {
  internal class PlainBuffersParser {
    private const string NamespaceId = "namespace";

    private const string EnumId = "enum";
    private const string ArrayId = "array";
    private const string StructId = "struct";
    private const string UnionId = "union";

    private const string FlagsId = "flags";

    private readonly ITypeMapper _mapper;
    private readonly Dictionary<string, ExternTypeInfo> _externTypes;

    public PlainBuffersParser(ExternTypeInfo[] externTypes, ITypeMapper mapper) {
      _mapper = mapper;
      _externTypes = externTypes.ToDictionary(s => s.Name);
    }

    public ParsingResult Parse(LexerData data) {
      var state = new ParserState();
      var index = new ParsingIndex();

      while (data.Tokens.Count > 0) {
        var opResult = ParseData(state, data, index);
        if (opResult.HasError)
          return ParsingResult.Fail(opResult.Error);
      }

      return ParsingResult.Ok(index.BuildParsedData());
    }

    private OpResult ParseData(ParserState state, LexerData data, ParsingIndex index) {
      switch (state.CurrentBlock.Type) {
        case ParsingBlockType.None:
          return ParseSchemaData(state, data, index);
        case ParsingBlockType.Namespace:
          return ParseNamespaceContent(state, data, index);
        case ParsingBlockType.Enum:
          return ParseEnumContent(state, data, index);
        case ParsingBlockType.Struct:
          return ParseStructContent(state, data, index);
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private OpResult ParseSchemaData(ParserState state, LexerData data, ParsingIndex index) {
      if (!TryReadToken(data, TokenType.Identifier, out _, out var nsId) || nsId != NamespaceId)
        return OpResult.Fail("Schema does not contain a namespace");

      if (!TryReadToken(data, TokenType.Identifier, out var nsNamePos, out var nsName))
        return OpResult.Fail("Namespace name is not defined");

      if (_mapper != null)
        nsName = _mapper.RemapNamespace(nsName);

      if (!ParsingHelper.IsDotSeparatedNameValid(nsName))
        return OpResult.Fail($"Invalid namespace name `{nsName}` at {nsNamePos}");

      if (!TryReadToken(data, TokenType.CurlyBraceLeft, out var bracePos, out _))
        return OpResult.Fail($"Missing `{{` after namespace declaration at {bracePos}");

      state.StartBlock(ParsingBlockType.Namespace, nsName);
      index.SetNamespace(nsName);

      return OpResult.Ok();
    }

    private OpResult ParseNamespaceContent(ParserState state, LexerData data, ParsingIndex index) {
      if (TryReadToken(data, TokenType.Identifier, out var pos, out var typeId)) {
        return TryParseType(typeId, pos, state, data, index);
      }

      if (TryReadToken(data, TokenType.CurlyBraceRight, out _, out _)) {
        state.EndBlock();
        return data.Tokens.Count == 0 ?
          OpResult.Ok() :
          OpResult.Fail("Schema contains definitions outside of namespace");
      }

      var token = data.Tokens.Peek();
      return OpResult.Fail($"Invalid token `{token.Type}` found at {token.Position}");
    }

    private OpResult TryParseType(string id, Position p, ParserState state, LexerData data, ParsingIndex index) {
      switch (id) {
        case EnumId:
          return TryParseEnum(state, data, index);
        case ArrayId:
          return TryParseArray(state, data, index);
        case StructId:
          return TryParseStruct(state, data, index, false);
        case UnionId:
          return TryParseStruct(state, data, index, true);
      }

      return OpResult.Fail($"Unknown datatype `{id}` declaration at {p}");
    }

    private OpResult TryParseEnum(ParserState state, LexerData data, ParsingIndex index) {
      if (!TryReadToken(data, TokenType.Identifier, out var namePos, out var enumName))
        return OpResult.Fail($"Enum name is not defined at {namePos}");

      if (_mapper != null)
        enumName = _mapper.RemapTypeName(TypeKind.Enum, enumName, state.RemappedTypes);

      var validationResult = ValidateTypeName(enumName, namePos, index);
      if (validationResult.HasError)
        return validationResult;

      if (!TryReadToken(data, TokenType.Colon, out var colonPos, out _))
        return OpResult.Fail($"Missing : at {colonPos}");

      if (!TryReadToken(data, TokenType.Identifier, out var undTypePos, out var underlyingType))
        return OpResult.Fail($"Missing enum `{enumName}` type definition at {undTypePos}");

      if (!ParsingHelper.IsInteger(underlyingType))
        return OpResult.Fail($"Enum `{enumName}` has invalid underlying type `{underlyingType}`");

      var isFlags = false;
      if (TryReadToken(data, TokenType.Identifier, out var flagsPos, out var keyword)) {
        if (keyword != FlagsId)
          return OpResult.Fail($"Unknown enum `{enumName}` optional keyword `{keyword}` at {flagsPos}." +
                               " Only `flags` keyword is supported.");

        isFlags = true;
      }

      if (!TryReadToken(data, TokenType.CurlyBraceLeft, out var bracePos, out _))
        return OpResult.Fail($"Missing `{{` after enum `{enumName}` declaration at {bracePos}");

      state.StartBlock(ParsingBlockType.Enum, enumName);
      index.BeginEnum(enumName, underlyingType, isFlags);

      return OpResult.Ok();
    }

    private static OpResult ParseEnumContent(ParserState state, LexerData data, ParsingIndex index) {
      var enumName = state.CurrentBlock.Name;

      if (TryReadToken(data, TokenType.CurlyBraceRight, out _, out _)) {
        state.EndBlock();
        index.EndEnum(enumName);

        return index.IsEnumNonEmpty(enumName) ?
          OpResult.Ok() :
          OpResult.Fail($"Enum `{enumName}` is empty. No default value is available");
      }

      if (!TryReadToken(data, TokenType.Identifier, out var namePos, out var name))
        return OpResult.Fail($"Failed to read enum name at {namePos}");

      if (!ParsingHelper.IsNameValid(name))
        return OpResult.Fail($"Enum item `{enumName}.{name}` has invalid name at {namePos}");

      if (index.IsEnumContainsItem(enumName, name))
        return OpResult.Fail($"Enum item `{enumName}.{name}` is defined second time at {namePos}");

      if (!TryReadToken(data, TokenType.Assignment, out var assignPos, out _))
        return OpResult.Fail($"Missing `=` at {assignPos}");

      if (!TryReadToken(data, TokenType.Identifier, out var valPos, out var value))
        return OpResult.Fail($"Missing enum item `{enumName}.{name}` default value at {valPos}");

      if (!IsEnumValueValid(enumName, value, index))
        return OpResult.Fail($"Enum item `{enumName}.{name}` has invalid default value at {valPos}");

      if (!TryReadToken(data, TokenType.Semicolon, out var semicolonPos, out _))
        return OpResult.Fail($"Missing ; at {semicolonPos}");

      index.PutEnumItem(enumName, name, value);

      return OpResult.Ok();
    }

    private OpResult TryParseArray(ParserState state, LexerData data, ParsingIndex index) {
      if (!TryReadToken(data, TokenType.Identifier, out var namePos, out var arrayName))
        return OpResult.Fail($"Array name is not defined at {namePos}");

      if (_mapper != null)
        arrayName = _mapper.RemapTypeName(TypeKind.Array, arrayName, state.RemappedTypes);

      var validationResult = ValidateTypeName(arrayName, namePos, index);
      if (validationResult.HasError)
        return validationResult;

      if (!TryReadToken(data, TokenType.Identifier, out var typePos, out var itemType))
        return OpResult.Fail($"Array `{arrayName}` items type is not defined at {typePos}");

      if (_mapper != null)
        itemType = _mapper.RemapMemberType(itemType, state.RemappedTypes);

      if (!IsTypeKnown(itemType, index))
        return OpResult.Fail($"Array `{arrayName}` has unknown items type `{itemType}` at {typePos}");

      if (!TryReadToken(data, TokenType.SquareBraceLeft, out var lBracePos, out _))
        return OpResult.Fail($"Missing [ at {lBracePos}");

      if (!TryReadToken(data, TokenType.Identifier, out var lengthPos, out var lengthString))
        return OpResult.Fail($"Array `{arrayName}` length is not defined at {lengthPos}");

      if (!int.TryParse(lengthString, out var length))
        return OpResult.Fail($"Failed to parse `{arrayName}` length at {lengthPos}");

      if (length <= 0)
        return OpResult.Fail($"Array `{arrayName}` has invalid length {length}");

      if (!TryReadToken(data, TokenType.SquareBraceRight, out var rBracePos, out _))
        return OpResult.Fail($"Missing ] at {rBracePos}");

      string defValue = null;
      if (TryReadToken(data, TokenType.Assignment, out _, out _)) {
        if (!TryReadToken(data, TokenType.Identifier, out var valuePos, out defValue))
          return OpResult.Fail($"Missing default item value of array `{arrayName}` at {valuePos}");

        if (_mapper != null)
          defValue = _mapper.RemapMemberDefaultValue(itemType, defValue);

        if (!IsDefaultValueValid(itemType, defValue, index))
          return OpResult.Fail($"Invalid default value `{defValue}` is defined for array `{arrayName}` at {valuePos}");
      }

      if (!TryReadToken(data, TokenType.Semicolon, out var semicolonPos, out _))
        return OpResult.Fail($"Missing ; at {semicolonPos}");

      index.PutArray(arrayName, itemType, length, defValue);

      return OpResult.Ok();
    }

    private OpResult TryParseStruct(ParserState state, LexerData data, ParsingIndex index, bool isUnion) {
      if (!TryReadToken(data, TokenType.Identifier, out var namePos, out var structName))
        return OpResult.Fail($"Struct/union name is not defined at {namePos}");

      if (_mapper != null)
        structName = _mapper.RemapTypeName(TypeKind.Struct, structName, state.RemappedTypes);

      var validationResult = ValidateTypeName(structName, namePos, index);
      if (validationResult.HasError)
        return validationResult;

      if (!TryReadToken(data, TokenType.CurlyBraceLeft, out var bracePos, out _))
        return OpResult.Fail($"Missing `{{` after struct/union `{structName}` declaration at {bracePos}");

      state.StartBlock(ParsingBlockType.Struct, structName);
      index.BeginStruct(structName, isUnion);

      return OpResult.Ok();
    }

    private OpResult ParseStructContent(ParserState state, LexerData data, ParsingIndex index) {
      var structName = state.CurrentBlock.Name;

      if (TryReadToken(data, TokenType.CurlyBraceRight, out _, out _)) {
        state.EndBlock();
        index.EndStruct(structName);

        return index.IsStructNonEmpty(structName) ?
          OpResult.Ok() :
          OpResult.Fail($"Struct/union `{structName}` is zero-sized");
      }

      if (!TryReadToken(data, TokenType.Identifier, out var typePos, out var fieldType))
        return OpResult.Fail($"Missing field type definition at {typePos}");

      if (_mapper != null)
        fieldType = _mapper.RemapMemberType(fieldType, state.RemappedTypes);

      if (!TryReadToken(data, TokenType.Identifier, out var namePos, out var fieldName))
        return OpResult.Fail($"Missing field name definition at {namePos}");

      if (!ParsingHelper.IsNameValid(fieldName))
        return OpResult.Fail($"Field `{structName}.{fieldName}` has invalid name at {namePos}");

      if (index.IsStructContainsField(structName, fieldName))
        return OpResult.Fail($"Field `{structName}.{fieldName}` is defined second time at {namePos}");

      if (!IsTypeKnown(fieldType, index))
        return OpResult.Fail($"Field `{structName}.{fieldName}` has unknown type `{fieldType}` at {typePos}");

      string fieldDefaultValue = null;
      if (TryReadToken(data, TokenType.Assignment, out _, out _)) {
        if (!TryReadToken(data, TokenType.Identifier, out var valuePos, out fieldDefaultValue))
          return OpResult.Fail($"Missing default value of field `{structName}.{fieldName}` at {valuePos}");

        if (_mapper != null)
          fieldDefaultValue = _mapper.RemapMemberDefaultValue(fieldType, fieldDefaultValue);

        if (!IsDefaultValueValid(fieldType, fieldDefaultValue, index))
          return OpResult.Fail($"Invalid default value `{fieldDefaultValue}` is defined for " +
                               $"field `{structName}.{fieldName}` at {valuePos}");
      }

      if (!TryReadToken(data, TokenType.Semicolon, out var semicolonPos, out _))
        return OpResult.Fail($"Missing ; after field `{structName}.{fieldName}` declaration at {semicolonPos}");

      index.PutStructField(structName, fieldType, fieldName, fieldDefaultValue);
      return OpResult.Ok();
    }

    private static bool TryReadToken(LexerData data, TokenType tokenType, out Position position, out string value) {
      var token = data.Tokens.Peek();

      position = token.Position;
      value = default;

      if (token.Type != tokenType)
        return false;

      data.Tokens.Dequeue();

      if (tokenType == TokenType.Identifier)
        value = data.Values.Dequeue();

      return true;
    }

    private OpResult ValidateTypeName(string type, Position pos, ParsingIndex index) {
      if (!ParsingHelper.IsNameValid(type))
        return OpResult.Fail($"Type `{type}` has invalid name");

      if (ParsingHelper.Primitives.Contains(type))
        return OpResult.Fail($"Try to redefine built-in type `{type}` at {pos}");

      if (_externTypes.ContainsKey(type))
        return OpResult.Fail($"Try to redefine extern struct `{type}` at {pos}");

      if (index.ContainsCompletedType(type))
        return OpResult.Fail($"Try to define type `{type}` second time at {pos}");

      return OpResult.Ok();
    }

    private bool IsTypeKnown(string itemType, ParsingIndex index) {
      return ParsingHelper.Primitives.Contains(itemType) ||
             _externTypes.ContainsKey(itemType) ||
             index.ContainsCompletedType(itemType);
    }

    private bool IsDefaultValueValid(string itemType, string value, ParsingIndex index) {
      if (ParsingHelper.IsPrimitive(itemType))
        return ParsingHelper.IsPrimitiveValueValid(itemType, value);

      if (_externTypes.TryGetValue(itemType, out var externStructInfo))
        return externStructInfo.Values.Contains(value);

      if (index.ContainsEnum(itemType))
        return index.IsEnumContainsItem(itemType, value);

      return false;
    }

    private static bool IsEnumValueValid(string enumName, string value, ParsingIndex index) {
      var underlyingType = index.GetEnumUnderlyingType(enumName);
      return ParsingHelper.IsPrimitiveValueValid(underlyingType, value);
    }
  }
}