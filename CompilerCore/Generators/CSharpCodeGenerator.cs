using System;
using System.Collections.Generic;
using System.IO;
using PlainBuffers.CompilerCore.CodeGen;
using PlainBuffers.CompilerCore.CodeGen.Data;
using PlainBuffers.Core;

namespace PlainBuffers.CompilerCore.Generators {
  public class CSharpCodeGenerator : IGenerator {
    private const string Indent = "    ";

    private static readonly Dictionary<string, string> CSharpTypes = new Dictionary<string, string> {
      {"bool", nameof(PlainBool)},
      {"sbyte", nameof(PlainInt8)},
      {"byte", nameof(PlainUInt8)},
      {"short", nameof(PlainInt16)},
      {"ushort", nameof(PlainUInt16)},
      {"int", nameof(PlainInt32)},
      {"uint", nameof(PlainUInt32)},
      {"float", nameof(PlainFloat)},
      {"long", nameof(PlainInt64)},
      {"ulong", nameof(PlainUInt64)},
      {"double", nameof(PlainDouble)}
    };

    public INamingChecker NamingChecker { get; } = new CSharpNamingChecker();

    public void Generate(CodeGenData data, TextWriter writer) {
      writer.WriteLine("// This file is auto-generated by the PlainBuffers compiler");
      writer.WriteLine($"// Generated at {DateTimeOffset.Now:O}");
      writer.WriteLine();

      writer.WriteLine("// ReSharper disable All");
      writer.WriteLine();

      writer.WriteLine("using System;");
      writer.WriteLine("using PlainBuffers.Core;");
      writer.WriteLine();

      var typesMap = new Dictionary<string, string>(CSharpTypes);

      using (var nsBlock = new BlockWriter(writer, Indent, 0, $"namespace {data.NameSpace}")) {
        for (var i = 0; i < data.Types.Length; i++) {
          var typeInfo = data.Types[i];
          switch (typeInfo) {
            case CodeGenEnum enumGenInfo:
              WriteEnum(enumGenInfo, nsBlock, typesMap);
              break;
            case CodeGenArray arrayGenInfo:
              WriteArray(arrayGenInfo, nsBlock, typesMap);
              break;
            case CodeGenStruct structInfo:
              WriteStruct(structInfo, nsBlock, typesMap);
              break;
            default:
              throw new Exception("Unknown data type");
          }

          if (i < data.Types.Length - 1)
            nsBlock.WriteLine();
        }
      }
    }

    private static void WriteConstructor(string type, BlockWriter typeBlock, string initializer = null) {
      typeBlock.WriteLine("private readonly Span<byte> _buffer;");
      typeBlock.WriteLine("public Span<byte> GetBuffer() => _buffer;");

      typeBlock.WriteLine();
      using (var ctorBlock = typeBlock.Sub($"public {type}(Span<byte> buffer)")) {
        const string msg = "\"Buffer size doesn't match to the struct size!\"";
        ctorBlock.WriteLine($"if (buffer.Length != SizeOf) throw new InvalidOperationException({msg});");

        ctorBlock.WriteLine("_buffer = buffer;");
        if (initializer != null)
          ctorBlock.WriteLine(initializer);
      }
    }

    private static void WriteCopyToMethod(string type, BlockWriter typeBlock) {
      typeBlock.WriteLine($"public void CopyTo({type} other) => _buffer.CopyTo(other._buffer);");
    }

    private static void WriteEqualityOperators(string type, BlockWriter typeBlock) {
      typeBlock.WriteLine($"public static bool operator ==({type} l, {type} r) => l._buffer.SequenceEqual(r._buffer);");
      typeBlock.WriteLine($"public static bool operator !=({type} l, {type} r) => !l._buffer.SequenceEqual(r._buffer);");
      typeBlock.WriteLine();
      typeBlock.WriteLine("public override bool Equals(object obj) => false;");
      typeBlock.WriteLine("public override int GetHashCode() => throw new NotSupportedException();");
    }

    private static string GetCSharpType(string type, IReadOnlyDictionary<string, string> typesMap) {
      return typesMap.TryGetValue(type, out var replacement) ? replacement : type;
    }

    private static string GetCSharpValue(string defaultValue, string type, IReadOnlyDictionary<string, string> typesMap) {
      if (CSharpTypes.ContainsKey(type))
        return defaultValue; // primitive

      if (typesMap.ContainsKey(type))
        return $"{type}.{defaultValue}"; // enum

      throw new Exception($"Failed to resolve default value for type `{type}`");
    }

    private static void WriteEnum(CodeGenEnum enumType, BlockWriter nsBlock, IDictionary<string, string> typesMap) {
      if (enumType.IsFlags)
        nsBlock.WriteLine("[Flags]");

      using (var typeBlock = nsBlock.Sub($"public enum {enumType.Name} : {enumType.UnderlyingType}")) {
        for (var i = 0; i < enumType.Items.Length; i++) {
          var item = enumType.Items[i];
          var comma = i < enumType.Items.Length - 1 ? "," : string.Empty;

          typeBlock.WriteLine($"{item.Name} = {item.Value}{comma}");
        }
      }

      nsBlock.WriteLine();
      var wrapperType = WriteEnumWrapper(enumType, nsBlock);
      typesMap.Add(enumType.Name, wrapperType);
    }

    private static string WriteEnumWrapper(CodeGenEnum enumType, BlockWriter nsBlock) {
      if (!CSharpTypes.TryGetValue(enumType.UnderlyingType, out var pbType))
        throw new Exception($"Enum `{enumType.Name}` has invalid underlying type `{enumType.UnderlyingType}`");

      var primitiveType = enumType.UnderlyingType;
      var wrapperType = $"_Plain{enumType.Name}";

      using (var typeBlock = nsBlock.Sub($"public readonly ref struct {wrapperType}")) {
        typeBlock.WriteLine($"public const int SizeOf = {enumType.Size};");
        typeBlock.WriteLine($"private readonly {pbType} _primitive;");

        typeBlock.WriteLine();
        WriteConstructor(wrapperType, typeBlock, $"_primitive = new {pbType}(_buffer);");

        typeBlock.WriteLine();
        typeBlock.WriteLine($"public {enumType.Name} Read() => ({enumType.Name}) _primitive.Read();");
        typeBlock.WriteLine($"public void Write({enumType.Name} value) => _primitive.Write(({primitiveType}) value);");

        typeBlock.WriteLine();
        WriteCopyToMethod(wrapperType, typeBlock);

        typeBlock.WriteLine();
        WriteEqualityOperators(wrapperType, typeBlock);
      }

      return wrapperType;
    }

    private static void WriteArray(CodeGenArray arrayType, BlockWriter nsBlock, IReadOnlyDictionary<string, string> typesMap) {
      using (var typeBlock = nsBlock.Sub($"public readonly ref struct {arrayType.Name}")) {
        typeBlock.WriteLine($"public const int SizeOf = {arrayType.Size};");
        typeBlock.WriteLine($"public const int Length = {arrayType.Length};");

        typeBlock.WriteLine();
        WriteConstructor(arrayType.Name, typeBlock);
        var cSharpItemType = GetCSharpType(arrayType.ItemType, typesMap);

        typeBlock.WriteLine();
        var sliceExpr = $"_buffer.Slice({cSharpItemType}.SizeOf * index, {cSharpItemType}.SizeOf)";
        typeBlock.WriteLine($"public {cSharpItemType} this[int index] => new {cSharpItemType}({sliceExpr});");

        typeBlock.WriteLine();
        WriteCopyToMethod(arrayType.Name, typeBlock);

        typeBlock.WriteLine();
        using (var wdBlock = typeBlock.Sub("public void WriteDefault()"))
        using (var forBlock = wdBlock.Sub("for (var i = 0; i < Length; i++)")) {
          if (arrayType.ItemDefaultValue == null) {
            forBlock.WriteLine("this[i].WriteDefault();");
          }
          else {
            var cSharpValue = GetCSharpValue(arrayType.ItemDefaultValue, arrayType.ItemType, typesMap);
            forBlock.WriteLine($"this[i].Write({cSharpValue});");
          }
        }

        typeBlock.WriteLine();
        WriteArrayEnumerator(arrayType.Name, cSharpItemType, typeBlock);

        typeBlock.WriteLine();
        WriteEqualityOperators(arrayType.Name, typeBlock);
      }
    }

    private static void WriteArrayEnumerator(string arrayType, string itemType, BlockWriter arrayBlock) {
      var enumeratorType = $"_EnumeratorOf{arrayType}";
      arrayBlock.WriteLine($"public {enumeratorType} GetEnumerator() => new {enumeratorType}(this);");

      arrayBlock.WriteLine();
      using (var enumeratorBlock = arrayBlock.Sub($"public ref struct {enumeratorType}")) {
        enumeratorBlock.WriteLine($"private readonly {arrayType} _array;");
        enumeratorBlock.WriteLine("private int _index;");

        enumeratorBlock.WriteLine();
        using (var ctorBlock = enumeratorBlock.Sub($"public {enumeratorType}({arrayType} array)")) {
          ctorBlock.WriteLine("_array = array;");
          ctorBlock.WriteLine("_index = -1;");
        }

        enumeratorBlock.WriteLine();
        enumeratorBlock.WriteLine("public bool MoveNext() => ++_index < Length;");
        enumeratorBlock.WriteLine($"public {itemType} Current => _array[_index];");

        enumeratorBlock.WriteLine();
        enumeratorBlock.WriteLine("public void Reset() => _index = -1;");
        enumeratorBlock.WriteLine("public void Dispose() {}");
      }
    }

    private static void WriteStruct(CodeGenStruct structType, BlockWriter nsBlock, IReadOnlyDictionary<string, string> typesMap) {
      using (var typeBlock = nsBlock.Sub($"public readonly ref struct {structType.Name}")) {
        typeBlock.WriteLine($"public const int SizeOf = {structType.Size};");

        if (structType.Padding != 0)
          typeBlock.WriteLine($"private const int _Padding = {structType.Padding};");

        typeBlock.WriteLine();
        WriteConstructor(structType.Name, typeBlock);

        typeBlock.WriteLine();
        foreach (var field in structType.Fields) {
          var cSharpFieldType = GetCSharpType(field.Type, typesMap);
          var sliceExpr = $"_buffer.Slice({field.Offset}, {cSharpFieldType}.SizeOf)";
          typeBlock.WriteLine($"public {cSharpFieldType} {field.Name} => new {cSharpFieldType}({sliceExpr});");
        }

        typeBlock.WriteLine();
        WriteCopyToMethod(structType.Name, typeBlock);

        typeBlock.WriteLine();
        using (var wdBlock = typeBlock.Sub("public void WriteDefault()")) {
          foreach (var field in structType.Fields) {
            if (field.DefaultValue == null) {
              wdBlock.WriteLine($"{field.Name}.WriteDefault();");
            }
            else {
              var cSharpValue = GetCSharpValue(field.DefaultValue, field.Type, typesMap);
              wdBlock.WriteLine($"{field.Name}.Write({cSharpValue});");
            }
          }

          if (structType.Padding != 0) {
            wdBlock.WriteLine("_buffer.Slice(SizeOf - _Padding, _Padding).Fill(0);");
          }
        }

        typeBlock.WriteLine();
        WriteEqualityOperators(structType.Name, typeBlock);
      }
    }
  }
}