using System;
using System.Collections.Generic;
using PlainBuffers.CompilerCore.CodeGen;
using PlainBuffers.CompilerCore.CodeGen.Data;
using PlainBuffers.Core;

namespace PlainBuffers.CompilerCore.Generators {
  public class CSharpNamingChecker : INamingChecker {
    private class CheckingIndex {
      public readonly List<string> Errors = new List<string>();
      public readonly List<string> Warnings = new List<string>();
      public readonly HashSet<string> Enums = new HashSet<string>();
    }

    private static readonly HashSet<string> BuiltInTypes = new HashSet<string> {
      nameof(PlainBool),
      nameof(PlainInt8),
      nameof(PlainUInt8),
      nameof(PlainInt16),
      nameof(PlainUInt16),
      nameof(PlainInt32),
      nameof(PlainUInt32),
      nameof(PlainFloat),
      nameof(PlainInt64),
      nameof(PlainUInt64),
      nameof(PlainDouble)
    };

    public (string[] Errors, string[] Warnings) CheckNaming(CodeGenData data) {
      var index = new CheckingIndex();

      foreach (var typeGenInfo in data.Types) {
        CheckTypeName(typeGenInfo.Name, index);

        switch (typeGenInfo) {
          case CodeGenEnum _:
            index.Enums.Add(typeGenInfo.Name);
            break;
          case CodeGenArray arrayGenInfo:
            ChekArray(arrayGenInfo, index);
            break;
          case CodeGenStruct structGenInfo:
            ChekStruct(structGenInfo, index);
            break;
          default:
            throw new Exception("Unknown data type");
        }
      }

      return (index.Errors.ToArray(), index.Warnings.ToArray());
    }

    private static void CheckTypeName(string type, CheckingIndex index) {
      if (BuiltInTypes.Contains(type))
        index.Errors.Add($"Type `{type}` has the same name with a built-in type");

      if (type == "SizeOf")
        index.Errors.Add($"Type name `{type}` is forbidden");

      if (type.StartsWith("_"))
        index.Warnings.Add($"Type `{type}` starts with `_`. It can cause name clashes in a generated code");
    }

    private static void ChekArray(CodeGenArray arrayInfo, CheckingIndex index) {
      if (arrayInfo.ItemType == "Length")
        index.Errors.Add($"Type with name `{arrayInfo.ItemType}` can't be stored in an array");
    }

    private static void ChekStruct(CodeGenStruct structInfo, CheckingIndex index) {
      foreach (var field in structInfo.Fields) {
        if (field.Name == "SizeOf")
          index.Errors.Add($"Field `{structInfo.Name}.{field.Name}` has forbidden name");

        if (index.Enums.Contains(field.Type) && field.Type == field.Name)
          index.Errors.Add($"Field `{structInfo.Name}.{field.Name}` can't be named same as the own enum-type");

        if (field.Name.StartsWith("_"))
          index.Warnings.Add($"Field name `{structInfo.Name}.{field.Name}` starts with `_`. " +
                             $"It can cause name clashes in a generated code");
      }
    }
  }
}