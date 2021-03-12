using System;
using System.Collections.Generic;
using System.IO;
using PlainBuffers.CodeGen;
using PlainBuffers.CodeGen.Data;

namespace PlainBuffers.Generators {
  public abstract class CSharpAbstractGenerator : IGenerator {
    protected static readonly HashSet<string> CSharpPrimitives = new HashSet<string> {
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

    private const string Indent = "    ";

    private readonly string[] _namespaces;

    protected CSharpAbstractGenerator(string[] namespaces) => _namespaces = namespaces ?? Array.Empty<string>();

    public INamingChecker NamingChecker { get; } = new CSharpNamingChecker();

    public void Generate(CodeGenData data, TextWriter writer, ExternStructInfo[] externStructs) {
      WriteHeader(writer);

      if (_namespaces.Length > 0)
        WriteNamespaces(writer);

      var valueTypes = new HashSet<string>(CSharpPrimitives);
      foreach (var externStruct in externStructs)
        valueTypes.Add(externStruct.Name);

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

    private void WriteNamespaces(TextWriter writer) {
      foreach (var ns in _namespaces)
        writer.WriteLine($"using {ns};");

      writer.WriteLine();
    }

    protected abstract void WriteHeader(TextWriter writer);

    protected abstract void WriteEnum(CodeGenEnum enumInfo, in BlockWriter nsBlock, HashSet<string> valueTypes);

    protected abstract void WriteArray(CodeGenArray arrayInfo, in BlockWriter nsBlock, HashSet<string> valueTypes);

    protected abstract void WriteStruct(CodeGenStruct structInfo, in BlockWriter nsBlock, HashSet<string> valueTypes);

    protected static void PutWriteDefaultLine(BlockWriter block, string lhs, string type, in DefaultValueInfo valInfo) {
      switch (valInfo.Variant) {
        case DefaultValueVariant.Assign:
          block.WriteLine($"{lhs} = {valInfo.Identifier};");
          break;
        case DefaultValueVariant.AssignTypeMember:
          block.WriteLine($"{lhs} = {type}.{valInfo.Identifier};");
          break;
        case DefaultValueVariant.CallInstanceMethod:
          block.WriteLine($"{lhs}.{valInfo.Identifier}();");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}