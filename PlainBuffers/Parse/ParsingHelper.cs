using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PlainBuffers.Parse {
  public class ParsingHelper {
    private const string NameRegex = @"^[A-Za-z_][A-Za-z0-9_]*$";

    public static bool IsInteger(string typeName) {
      switch (typeName) {
        case "sbyte":
        case "byte":
        case "short":
        case "ushort":
        case "int":
        case "uint":
        case "long":
        case "ulong":
          return true;
      }

      return false;
    }

    public static int GetPrimitiveTypeSize(string typeName) {
      switch (typeName) {
        case "sbyte":
        case "byte":
        case "bool":
          return 1;

        case "short":
        case "ushort":
          return 2;

        case "int":
        case "uint":
        case "float":
          return 4;

        case "long":
        case "ulong":
        case "double":
          return 8;

        default:
          throw new Exception($"Unknown primitive type `{typeName}`");
      }
    }

    public static bool IsNameValid(string name) => !string.IsNullOrEmpty(name) && Regex.IsMatch(name, NameRegex);

    public static bool IsDotSeparatedNameValid(string name) => name.Split('.').All(IsNameValid);
  }
}