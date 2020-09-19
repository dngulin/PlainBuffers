using System.Linq;
using System.Text.RegularExpressions;

namespace PlainBuffers.CompilerCore.Parsing {
  public static class SyntaxHelper {
    private const string NameRegex = @"^[A-Za-z_][A-Za-z0-9_]*$";

    public static readonly string[] Primitives = {
      "sbyte",
      "byte",
      "bool",
      "short",
      "ushort",
      "int",
      "uint",
      "float",
      "long",
      "ulong",
      "double"
    };

    public static bool IsPrimitive(string type) => Primitives.Contains(type);

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

    public static bool IsNameValid(string name) => !string.IsNullOrEmpty(name) && Regex.IsMatch(name, NameRegex);

    public static bool IsDotSeparatedNameValid(string name) => name.Split('.').All(IsNameValid);
  }
}