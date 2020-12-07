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
    public INamingChecker NamingChecker { get; } = new CSharpNamingChecker();

    public void Generate(CodeGenData data, TextWriter writer) {
      WriteHeader(writer);

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

    protected abstract void WriteHeader(TextWriter writer);

    protected abstract void WriteEnum(CodeGenEnum enumInfo, in BlockWriter nsBlock, HashSet<string> valueTypes);

    protected abstract void WriteArray(CodeGenArray arrayInfo, in BlockWriter nsBlock, HashSet<string> valueTypes);

    protected abstract void WriteStruct(CodeGenStruct structInfo, in BlockWriter nsBlock, HashSet<string> valueTypes);
  }
}