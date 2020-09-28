using System;
using System.Collections.Generic;
using System.IO;
using PlainBuffers.CompilerCore.CodeGen;
using PlainBuffers.CompilerCore.CodeGen.Data;

namespace PlainBuffers.CompilerCore.Generators {
  public class CSharpUnsafeCodeGenerator : IGenerator {
    private const string Indent = "    ";

    private static readonly HashSet<string> CSharpPrimitives = new HashSet<string> {
      "bool",
      "sbyte",
      "byte",
      "short",
      "ushort",
      "int",
      "uint",
      "float",
      "long",
      "ulong",
      "double"
    };

    public INamingChecker NamingChecker { get; } = new CSharpNamingChecker();

    public void Generate(CodeGenData data, TextWriter writer) {
      writer.WriteLine("// This file is auto-generated by the PlainBuffers compiler");
      writer.WriteLine($"// Generated at {DateTimeOffset.Now:O}");
      writer.WriteLine();

      writer.WriteLine("// ReSharper disable All");
      writer.WriteLine();

      writer.WriteLine("using System;");
      writer.WriteLine();

      var valueTypes = new HashSet<string>(CSharpPrimitives);

      using (var nsBlock = new BlockWriter(writer, Indent, 0, $"namespace {data.NameSpace}")) {
        for (var i = 0; i < data.Types.Length; i++) {
          var typeInfo = data.Types[i];
          switch (typeInfo) {
            case CodeGenEnum enumInfo:
              WriteEnum(enumInfo, nsBlock, valueTypes);
              break;
            case CodeGenArray arrayInfo:
              WriteArray(arrayInfo, nsBlock, valueTypes);
              break;
            case CodeGenStruct structInfo:
              WriteStruct(structInfo, nsBlock, valueTypes);
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
      typeBlock.WriteLine("private readonly byte* _ptr;");
      typeBlock.WriteLine("public Span<byte> GetBuffer() => new Span<byte>(_ptr, SizeOf);");

      typeBlock.WriteLine();
      typeBlock.WriteLine($"public {type}(byte* ptr) => _ptr = ptr;");

      typeBlock.WriteLine();
      using (var ctorBlock = typeBlock.Sub($"public static {type} WrapBuffer(byte* buffer, int bufferSize, int myIndex = 0)")) {
        const string msg = "\"Buffer size ios too small!\"";
        ctorBlock.WriteLine("var offset = SizeOf * myIndex;");
        ctorBlock.WriteLine($"if ((bufferSize - offset) < SizeOf) throw new InvalidOperationException({msg});");
        ctorBlock.WriteLine($"return new {type}(buffer + offset);");
      }
    }

    private static void WriteCopyToMethod(string type, BlockWriter typeBlock) {
      typeBlock.WriteLine($"public void CopyTo({type} other) => GetBuffer().CopyTo(other.GetBuffer());");
    }

    private static void WriteEqualityOperators(string type, in BlockWriter typeBlock) {
      typeBlock.WriteLine($"public static bool operator ==({type} l, {type} r) => l.GetBuffer().SequenceEqual(r.GetBuffer());");
      typeBlock.WriteLine($"public static bool operator !=({type} l, {type} r) => !l.GetBuffer().SequenceEqual(r.GetBuffer());");
      typeBlock.WriteLine();
      typeBlock.WriteLine("public override bool Equals(object obj) => false;");
      typeBlock.WriteLine("public override int GetHashCode() => throw new NotSupportedException();");
    }

    private static void WriteEnum(CodeGenEnum enumType, in BlockWriter nsBlock, HashSet<string> valueTypes) {
      if (enumType.IsFlags)
        nsBlock.WriteLine("[Flags]");

      using (var typeBlock = nsBlock.Sub($"public enum {enumType.Name} : {enumType.UnderlyingType}")) {
        for (var i = 0; i < enumType.Items.Length; i++) {
          var item = enumType.Items[i];
          var comma = i < enumType.Items.Length - 1 ? "," : string.Empty;

          typeBlock.WriteLine($"{item.Name} = {item.Value}{comma}");
        }
      }

      valueTypes.Add(enumType.Name);
    }

    private static void WriteArray(CodeGenArray arrayType, in BlockWriter nsBlock, HashSet<string> valueTypes) {
      using (var typeBlock = nsBlock.Sub($"public readonly unsafe ref struct {arrayType.Name}")) {
        typeBlock.WriteLine($"public const int SizeOf = {arrayType.Size};");
        typeBlock.WriteLine($"public const int Length = {arrayType.Length};");

        typeBlock.WriteLine();
        WriteConstructor(arrayType.Name, typeBlock);

        var itemType = arrayType.ItemType;
        var isValueType = valueTypes.Contains(itemType);

        typeBlock.WriteLine();
        WriteCopyToMethod(arrayType.Name, typeBlock);

        typeBlock.WriteLine();
        using (var wdBlock = typeBlock.Sub("public void WriteDefault()"))
        using (var forBlock = wdBlock.Sub("for (var i = 0; i < Length; i++)")) {
          if (isValueType) {
            var isEnum = !CSharpPrimitives.Contains(itemType);
            var defaultValue = isEnum ? $"{itemType}.{arrayType.ItemDefaultValue}" : arrayType.ItemDefaultValue;
            forBlock.WriteLine($"this[i] = {defaultValue};");
          }
          else {
            forBlock.WriteLine("this[i].WriteDefault();");
          }
        }

        typeBlock.WriteLine();
        using (var idxBlock = typeBlock.Sub(isValueType ?
          $"public ref {itemType} this[int index]" :
          $"public {itemType} this[int index]"))
        using (var getBlock = idxBlock.Sub("get")) {
          var itemSize = isValueType ? $"sizeof({itemType})" : $"{itemType}.SizeOf";
          getBlock.WriteLine($"if (index < 0 || {itemSize} * index >= SizeOf) throw new IndexOutOfRangeException();");
          getBlock.WriteLine(isValueType
            ? $"return ref *(({itemType}*)_ptr + index);"
            : $"return new {itemType}(_ptr);");
        }

        typeBlock.WriteLine();
        typeBlock.WriteLine(isValueType
          ? $"private ref {itemType} At(int index) => ref *(({itemType}*)_ptr + index);"
          : $"private {itemType} At(int index) => new {itemType}(_ptr);");

        typeBlock.WriteLine();
        WriteArrayEnumerator(arrayType.Name, itemType, typeBlock, isValueType);

        typeBlock.WriteLine();
        WriteEqualityOperators(arrayType.Name, typeBlock);
      }
    }

    private static void WriteArrayEnumerator(string array, string item, BlockWriter arrayBlock, bool itemIsValueType) {
      var enumeratorType = $"_EnumeratorOf{array}";
      arrayBlock.WriteLine($"public {enumeratorType} GetEnumerator() => new {enumeratorType}(this);");

      arrayBlock.WriteLine();
      using (var enumeratorBlock = arrayBlock.Sub($"public ref struct {enumeratorType}")) {
        enumeratorBlock.WriteLine($"private readonly {array} _array;");
        enumeratorBlock.WriteLine("private int _index;");

        enumeratorBlock.WriteLine();
        using (var ctorBlock = enumeratorBlock.Sub($"public {enumeratorType}({array} array)")) {
          ctorBlock.WriteLine("_array = array;");
          ctorBlock.WriteLine("_index = -1;");
        }

        enumeratorBlock.WriteLine();
        enumeratorBlock.WriteLine("public bool MoveNext() => ++_index < Length;");
        enumeratorBlock.WriteLine(itemIsValueType
          ? $"public ref {item} Current => ref _array.At(_index);"
          : $"public {item} Current => _array.At(_index);");

        enumeratorBlock.WriteLine();
        enumeratorBlock.WriteLine("public void Reset() => _index = -1;");
        enumeratorBlock.WriteLine("public void Dispose() {}");
      }
    }

    private static void WriteStruct(CodeGenStruct structType, in BlockWriter nsBlock, HashSet<string> valueTypes) {
      using (var typeBlock = nsBlock.Sub($"public readonly unsafe ref struct {structType.Name}")) {
        typeBlock.WriteLine($"public const int SizeOf = {structType.Size};");

        if (structType.Padding != 0)
          typeBlock.WriteLine($"private const int _Padding = {structType.Padding};");

        typeBlock.WriteLine();
        WriteConstructor(structType.Name, typeBlock);

        typeBlock.WriteLine();
        foreach (var field in structType.Fields) {
          var isValueType = valueTypes.Contains(field.Type);
          typeBlock.WriteLine(isValueType?
            $"public ref {field.Type} {field.Name} => ref *(({field.Type}*)(_ptr + {field.Offset}));" :
            $"public {field.Type} {field.Name} => new {field.Type}(_ptr + {field.Offset});");
        }

        typeBlock.WriteLine();
        WriteCopyToMethod(structType.Name, typeBlock);

        typeBlock.WriteLine();
        using (var wdBlock = typeBlock.Sub("public void WriteDefault()")) {
          foreach (var field in structType.Fields) {
            var isValueType = valueTypes.Contains(field.Type);
            var isEnum = isValueType && !CSharpPrimitives.Contains(field.Type);
            var defaultValue = isEnum ? $"{field.Type}.{field.DefaultValue}" : field.DefaultValue;

            wdBlock.WriteLine(isValueType
              ? $"{field.Name} = {defaultValue};"
              : $"{field.Name}.WriteDefault();");
          }

          if (structType.Padding != 0) {
            wdBlock.WriteLine("GetBuffer().Slice(SizeOf - _Padding, _Padding).Fill(0);");
          }
        }

        typeBlock.WriteLine();
        WriteEqualityOperators(structType.Name, typeBlock);
      }
    }
  }
}