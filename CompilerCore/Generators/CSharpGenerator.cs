using System;
using System.Collections.Generic;
using System.IO;
using PlainBuffers.CompilerCore.CodeGen;
using PlainBuffers.CompilerCore.CodeGen.Data;
using PlainBuffers.Core;

namespace PlainBuffers.CompilerCore.Generators {
  public class CSharpGenerator : IGenerator {
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

    public void Generate(CodeGenData data, TextWriter writer) {
      writer.WriteLine("// This file is auto-generated by the PlainBuffers compiler");
      writer.WriteLine($"// Generated at {DateTimeOffset.Now:O}");
      writer.WriteLine();

      writer.WriteLine("// ReSharper disable All");
      writer.WriteLine();

      writer.WriteLine("using System;");
      writer.WriteLine("using PlainBuffers.Core;");
      writer.WriteLine();

      using (var nsBlock = new BlockWriter(writer, Indent, 0, $"namespace {data.NameSpace}")) {
        for (var i = 0; i < data.Types.Length; i++) {
          var typeInfo = data.Types[i];
          switch (typeInfo) {
            case CodeGenEnum enumGenInfo:
              WriteEnum(enumGenInfo, nsBlock);
              break;
            case CodeGenArray arrayGenInfo:
              WriteArray(arrayGenInfo, nsBlock);
              break;
            case CodeGenStruct structInfo:
              WriteStruct(structInfo, nsBlock);
              break;
            default:
              throw new Exception("Unknown data type");
          }

          if (i < data.Types.Length - 1)
            nsBlock.WriteLine();
        }
      }
    }

    private static void WriteConstructor(string type, BlockWriter typeBlock) {
      typeBlock.WriteLine("public readonly Span<byte> _Buffer;");

      typeBlock.WriteLine();
      using (var ctorBlock = typeBlock.Sub($"public {type}(Span<byte> buffer)")) {
        using (var ifBlock = ctorBlock.Sub("if (buffer.Length != Size)")) {
          ifBlock.WriteLine("throw new InvalidOperationException();"); // TODO: message
        }
        ctorBlock.WriteLine("_Buffer = buffer;");
      }
    }

    private static void WriteCopyToMethod(string type, BlockWriter typeBlock) {
      typeBlock.WriteLine($"public void CopyTo({type} other) => _Buffer.CopyTo(other._Buffer);");
    }

    private static void WriteEqualityOperators(string type, BlockWriter typeBlock) {
      typeBlock.WriteLine($"public static bool operator ==({type} l, {type} r) => l._Buffer.SequenceEqual(r._Buffer);");
      typeBlock.WriteLine($"public static bool operator !=({type} l, {type} r) => !l._Buffer.SequenceEqual(r._Buffer);");
      typeBlock.WriteLine();
      typeBlock.WriteLine("public override bool Equals(object obj) => false;");
      typeBlock.WriteLine("public override int GetHashCode() => throw new NotSupportedException();");
    }

    private static void WriteEnum(CodeGenEnum enumType, BlockWriter nsBlock) {
      if (enumType.IsFlags)
        nsBlock.WriteLine("[Flags]");

      using (var typeBlock = nsBlock.Sub($"public enum {enumType.Name} : {enumType.UnderlyingType}")) {
        for (var i = 0; i < enumType.Items.Length; i++) {
          var item = enumType.Items[i];

          var assignment = !string.IsNullOrEmpty(item.Value) ? $" = {item.Value}" : string.Empty;
          var comma = i < enumType.Items.Length - 1 ? "," : string.Empty;

          typeBlock.WriteLine($"{item.Name}{assignment}{comma}");
        }
      }
    }

    private static void WriteArray(CodeGenArray arrayType, BlockWriter nsBlock) {
      using (var typeBlock = nsBlock.Sub($"public readonly ref struct {arrayType.Name}")) {
        typeBlock.WriteLine($"public const int Size = {arrayType.Size};");
        typeBlock.WriteLine($"public const int Length = {arrayType.Length};");

        typeBlock.WriteLine();
        WriteConstructor(arrayType.Name, typeBlock);

        if (!CSharpTypes.TryGetValue(arrayType.ItemType, out var itemType))
          itemType = arrayType.ItemType;

        typeBlock.WriteLine();
        var sizeExpr = arrayType.IsItemTypeEnum ? $"sizeof({itemType})" : $"{itemType}.Size";
        var sliceExpr = $"_Buffer.Slice({sizeExpr} * index, {sizeExpr})";
        typeBlock.WriteLine($"public {itemType} this[int index] => new {itemType}({sliceExpr});");

        typeBlock.WriteLine();
        WriteCopyToMethod(arrayType.Name, typeBlock);

        // TODO: optimize WriteDefault
        typeBlock.WriteLine();
        using (var wdBlock = typeBlock.Sub("public void WriteDefault()"))
        using (var forBlock = wdBlock.Sub("for (var i = 0; i < Length; i++)")) {
          var isPrimitive = !string.IsNullOrEmpty(arrayType.ItemDefaultValue);
          forBlock.WriteLine(isPrimitive ? $"this[i].Write({arrayType.ItemDefaultValue});" : "this[i].WriteDefault();");
        }

        typeBlock.WriteLine();
        WriteArrayEnumerator(arrayType.Name, itemType, typeBlock);

        typeBlock.WriteLine();
        WriteEqualityOperators(arrayType.Name, typeBlock);
      }
    }

    private static void WriteArrayEnumerator(string arrayType, string itemType, BlockWriter arrayBlock) {
      arrayBlock.WriteLine("public Enumerator GetEnumerator() => new Enumerator(this);");

      arrayBlock.WriteLine();
      using (var enumeratorBlock = arrayBlock.Sub("public ref struct Enumerator")) {
        enumeratorBlock.WriteLine($"private readonly {arrayType} _array;");
        enumeratorBlock.WriteLine("private int _index;");

        enumeratorBlock.WriteLine();
        using (var ctorBlock = enumeratorBlock.Sub($"public Enumerator({arrayType} array)")) {
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

    private static void WriteStruct(CodeGenStruct structType, BlockWriter nsBlock) {
      using (var typeBlock = nsBlock.Sub($"public readonly ref struct {structType.Name}")) {
        typeBlock.WriteLine($"public const int Size = {structType.Size};");

        typeBlock.WriteLine();
        foreach (var field in structType.Fields) {
          var fieldName = field.Name;
          typeBlock.WriteLine($"private const int _{fieldName}Offset = {field.Offset};");
        }

        if (structType.Padding != 0) {
          typeBlock.WriteLine($"private const int _PaddingStart = {structType.PaddingOffset};");
          typeBlock.WriteLine($"private const int _PaddingSize = {structType.Padding};");
        }

        typeBlock.WriteLine();
        WriteConstructor(structType.Name, typeBlock);

        typeBlock.WriteLine();
        foreach (var field in structType.Fields) {
          if (!CSharpTypes.TryGetValue(field.Type, out var fieldType))
            fieldType = field.Type;

          var sizeExpr = field.IsFieldTypeEnum ? $"sizeof({fieldType})" : $"{fieldType}.Size";
          var sliceExpr = $"_Buffer.Slice(_{field.Name}Offset, {sizeExpr})";
          typeBlock.WriteLine($"public {fieldType} {field.Name} => new {fieldType}({sliceExpr});");
        }

        typeBlock.WriteLine();
        WriteCopyToMethod(structType.Name, typeBlock);

        typeBlock.WriteLine();
        using (var wdBlock = typeBlock.Sub("public void WriteDefault()")) {
          foreach (var field in structType.Fields) {
            wdBlock.WriteLine(string.IsNullOrEmpty(field.DefaultValue)
              ? $"{field.Name}.WriteDefault();"
              : $"{field.Name}.Write({field.DefaultValue});");
          }

          if (structType.Padding != 0) {
            wdBlock.WriteLine("_Buffer.Slice(_PaddingStart, _PaddingSize).Fill(0);");
          }
        }

        typeBlock.WriteLine();
        WriteEqualityOperators(structType.Name, typeBlock);
      }
    }
  }
}