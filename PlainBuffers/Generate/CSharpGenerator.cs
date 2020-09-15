using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlainBuffers.BuiltIn;
using PlainBuffers.Schema;

namespace PlainBuffers.Generate {
  public class CSharpGenerator {
    private const string Indent = "    ";

    private static readonly Dictionary<string, string> PrimitiveTypes = new Dictionary<string, string> {
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
      {"double", nameof(PlainDouble)},
    };

    private static readonly Dictionary<string, int> PrimitiveTypeSizes = new Dictionary<string, int> {
      {nameof(PlainBool), 1},
      {nameof(PlainInt8), 1},
      {nameof(PlainUInt8), 1},
      {nameof(PlainInt16), 2},
      {nameof(PlainUInt16), 2},
      {nameof(PlainInt32), 4},
      {nameof(PlainUInt32), 4},
      {nameof(PlainFloat), 4},
      {nameof(PlainInt64), 8},
      {nameof(PlainUInt64), 8},
      {nameof(PlainDouble), 8}
    };

    public void Generate(SchemaInfo schema, TextWriter writer) {
      writer.WriteLine("using System;");
      writer.WriteLine("using PlainBuffers.BuiltIn;");
      writer.WriteLine();

      writer.WriteLine("using System;");
      writer.WriteLine();

      var typeSizes = schema.Types.ToDictionary(t => t.Name, t => t.Size);

      using (var nsBlock = new BlockWriter(writer, Indent, 0, $"namespace {schema.NameSpace}")) {
        for (var i = 0; i < schema.Types.Length; i++) {
          var typeInfo = schema.Types[i];
          switch (typeInfo) {
            case EnumTypeInfo enumInfo:
              WriteEnum(enumInfo, nsBlock);
              break;

            case ArrayTypeInfo arrayInfo:
              WriteArray(arrayInfo, nsBlock);
              break;

            case StructTypeInfo structInfo:
              WriteStruct(structInfo, nsBlock, typeSizes);
              break;

            default:
              throw new Exception("Unknown data type");
          }

          if (i < schema.Types.Length - 1)
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

        ctorBlock.WriteLine();
        ctorBlock.WriteLine("_Buffer = buffer;");
      }
    }

    private static void WriteCopyToMethod(string type, BlockWriter typeBlock) {
      using (var wrBlock = typeBlock.Sub($"public void CopyTo({type} other)")) {
        wrBlock.WriteLine("_Buffer.CopyTo(other._Buffer);");
      }
    }

    private static void WriteEnum(EnumTypeInfo typeInfo, BlockWriter nsBlock) {
      if (typeInfo.IsFlags)
        nsBlock.WriteLine("[Flags]");

      using (var typeBlock = nsBlock.Sub($"public enum {typeInfo.Name} : {typeInfo.UnderlyingType}")) {
        for (var i = 0; i < typeInfo.Items.Length; i++) {
          var item = typeInfo.Items[i];

          var assignment = !string.IsNullOrEmpty(item.Value) ? $" = {item.Value}" : string.Empty;
          var comma = i < typeInfo.Items.Length - 1 ? "," : string.Empty;

          typeBlock.WriteLine($"{item.Name}{assignment}{comma}");
        }
      }
    }

    private static void WriteArray(ArrayTypeInfo typeInfo, BlockWriter nsBlock) {
      using (var typeBlock = nsBlock.Sub($"public ref struct {typeInfo.Name}")) {
        typeBlock.WriteLine($"public const int Size = {typeInfo.Size};");
        typeBlock.WriteLine($"public const int Lenght = {typeInfo.Length};");

        typeBlock.WriteLine();
        WriteConstructor(typeInfo.Name, typeBlock);

        typeBlock.WriteLine();
        WriteCopyToMethod(typeInfo.Name, typeBlock);

        if (!PrimitiveTypes.TryGetValue(typeInfo.ItemType, out var itemType))
          itemType = typeInfo.ItemType;

        // Indexer
        typeBlock.WriteLine();
        using (var idxBlock = typeBlock.Sub($"public {itemType} this[int index]"))
        using (var getBlock = idxBlock.Sub("get")) {
          using (var ifBlock = getBlock.Sub("if (index < 0 || index >= Length)")) {
            ifBlock.WriteLine("throw new IndexOutOfRangeException();"); // TODO: message
          }

          getBlock.WriteLine();
          getBlock.WriteLine($"var offset = {itemType}.Size * index;");
          getBlock.WriteLine($"var slice = _Buffer.Slice(offset, {itemType}.Size);");
          getBlock.WriteLine($"return new {itemType}(slice);");
        }

        // WriteDefault
        typeBlock.WriteLine();
        if (!string.IsNullOrEmpty(typeInfo.ItemDefaultValue)) {
          using (var wdBlock = typeBlock.Sub("public void WriteDefault()"))
          using (var forBlock = wdBlock.Sub("for (var i = 0; i < Length; i++)")) {
            forBlock.WriteLine($"this[i].Write({typeInfo.ItemDefaultValue});"); // TODO: exponential size copy
          }
        }

        typeBlock.WriteLine();
        WriteArrayEnumerator(typeInfo.Name, itemType, typeBlock);
      }
    }

    private static void WriteArrayEnumerator(string arrayType, string itemType, BlockWriter arrayBlock) {
      using (var getEnumeratorBlock = arrayBlock.Sub("public Enumerator GetEnumerator()")) {
        getEnumeratorBlock.WriteLine("return new Enumerator(this);");
      }

      arrayBlock.WriteLine();
      using (var enumeratorBlock = arrayBlock.Sub("public ref struct Enumerator")) {
        enumeratorBlock.WriteLine($"private readonly {arrayType} _array;");

        enumeratorBlock.WriteLine();
        enumeratorBlock.WriteLine("private int _index;");

        enumeratorBlock.WriteLine();
        using (var ctorBlock = enumeratorBlock.Sub($"public Enumerator({arrayType} array)")) {
          ctorBlock.WriteLine("_array = array;");
          ctorBlock.WriteLine("_index = -1;");
        }

        enumeratorBlock.WriteLine();
        using (var nxtBlock = enumeratorBlock.Sub("public bool MoveNext()")) {
          nxtBlock.WriteLine($"return (++_index < {arrayType}.Length);");
        }

        enumeratorBlock.WriteLine();
        using (var curBlock = enumeratorBlock.Sub($"public {itemType} Current"))
        using (var getBlock = curBlock.Sub("get")) {
          getBlock.WriteLine("return _array[_index];");
        }

        enumeratorBlock.WriteLine();
        using (var rstBlock = enumeratorBlock.Sub("public void Reset()")) {
          rstBlock.WriteLine("_index = -1;");
        }

        enumeratorBlock.WriteLine();
        using (enumeratorBlock.Sub("public void Dispose()")) { }
      }
    }

    private void WriteStruct(StructTypeInfo typeInfo, BlockWriter nsBlock, IReadOnlyDictionary<string, int> typeSizes) {
      using (var typeBlock = nsBlock.Sub($"public ref struct {typeInfo.Name}")) {
        typeBlock.WriteLine($"public const int Size = {typeInfo.Size};");

        typeBlock.WriteLine();
        var offset = 0;
        foreach (var fieldInfo in typeInfo.Fields) {
          typeBlock.WriteLine($"private const int _{fieldInfo.Name}Offset = {offset};");
          offset += GetTypeSize(fieldInfo.Type, typeSizes);
        }

        typeBlock.WriteLine();
        WriteConstructor(typeInfo.Name, typeBlock);

        // Fields
        typeBlock.WriteLine();
        foreach (var fieldInfo in typeInfo.Fields) {
          if (!PrimitiveTypes.TryGetValue(fieldInfo.Type, out var fieldType))
            fieldType = fieldInfo.Type;

          using (var propBlock = typeBlock.Sub($"public {fieldType} Current"))
          using (var getBlock = propBlock.Sub("get")) {
            getBlock.WriteLine($"var slice = _Buffer.Slice(_{fieldInfo.Name}Offset, {fieldType}.Size);");
            getBlock.WriteLine($"return new {fieldType}(slice);");
          }
        }

        typeBlock.WriteLine();
        WriteCopyToMethod(typeInfo.Name, typeBlock);

        // WriteDefault
        typeBlock.WriteLine();
        using (var wdBlock = typeBlock.Sub("public void WriteDefault()")) {
          foreach (var fieldInfo in typeInfo.Fields) {
            wdBlock.WriteLine(string.IsNullOrEmpty(fieldInfo.DefaultValue)
              ? $"{fieldInfo.Name}.WriteDefault();"
              : $"{fieldInfo.Name}.Write({fieldInfo.DefaultValue});");
          }

          if (typeInfo.PaddingSize != 0) {
            wdBlock.WriteLine();
            wdBlock.WriteLine($"const int paddingOffset = {offset};");
            wdBlock.WriteLine($"const int paddingSize = {typeInfo.PaddingSize};");
            wdBlock.WriteLine("_Buffer.Slice(paddingOffset, paddingSize).Fill(0);");
          }
        }
      }
    }

    private static int GetTypeSize(string type, IReadOnlyDictionary<string, int> typeSizes) {
      if (PrimitiveTypes.TryGetValue(type, out var primitiveType))
        return PrimitiveTypeSizes[primitiveType];

      return typeSizes[type];
    }
  }
}