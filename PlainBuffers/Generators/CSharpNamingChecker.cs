using System;
using System.Collections.Generic;
using PlainBuffers.CodeGen;
using PlainBuffers.CodeGen.Data;

namespace PlainBuffers.Generators {
  public class CSharpNamingChecker : INamingChecker {
    private class CheckingIndex {
      public readonly List<string> Errors = new List<string>();
      public readonly List<string> Warnings = new List<string>();
    }

    private static readonly HashSet<string> Keywords = new HashSet<string> {
      "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const",
      "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern",
      "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface",
      "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
      "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc",
      "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked",
      "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
    };

    public (string[] Errors, string[] Warnings) CheckNaming(CodeGenData data) {
      var index = new CheckingIndex();

      foreach (var typeGenInfo in data.Types) {
        CheckTypeName(typeGenInfo.Name, index);

        switch (typeGenInfo) {
          case CodeGenEnum enumGenInfo:
            CheckEnum(enumGenInfo, index);
            break;
          case CodeGenArray arrayGenInfo:
            CheckArray(arrayGenInfo, index);
            break;
          case CodeGenStruct structGenInfo:
            CheckStruct(structGenInfo, index);
            break;
          default:
            throw new Exception("Unknown data type");
        }
      }

      return (index.Errors.ToArray(), index.Warnings.ToArray());
    }

    private static void CheckTypeName(string type, CheckingIndex index) {
      if (Keywords.Contains(type))
        index.Errors.Add($"Type `{type}` has the same name with a C# keyword");

      if (type == "SizeOf")
        index.Errors.Add($"Type name `{type}` is forbidden");

      if (type.StartsWith("_"))
        index.Warnings.Add($"Type `{type}` starts with `_`. It can cause name clashes in a generated code");
    }

    private static void CheckEnum(CodeGenEnum enumInfo, CheckingIndex index) {
      foreach (var item in enumInfo.Items) {
        if (Keywords.Contains(item.Name))
          index.Errors.Add($"Enum item `{enumInfo.Name}.{item.Name}` has the same name with a C# keyword");
      }
    }

    private static void CheckArray(CodeGenArray arrayInfo, CheckingIndex index) {
      if (arrayInfo.ItemType == "Length")
        index.Errors.Add($"Type with name `{arrayInfo.ItemType}` can't be stored in an array");
    }

    private static void CheckStruct(CodeGenStruct structInfo, CheckingIndex index) {
      foreach (var field in structInfo.Fields) {
        if (Keywords.Contains(structInfo.Name))
          index.Errors.Add($"Field `{structInfo.Name}.{field.Name}` has the same name with a C# keyword");

        if (field.Name == "SizeOf")
          index.Errors.Add($"Field `{structInfo.Name}.{field.Name}` has forbidden name");

        if (field.Name.StartsWith("_"))
          index.Warnings.Add($"Field name `{structInfo.Name}.{field.Name}` starts with `_`. " +
                             "It can cause name clashes in a generated code");
      }
    }
  }
}