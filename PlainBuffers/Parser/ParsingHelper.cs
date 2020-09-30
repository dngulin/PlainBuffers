using System.Linq;
using System.Text.RegularExpressions;

namespace PlainBuffers.Parser {
  internal static class ParsingHelper {
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

    public static bool IsInteger(string type) {
      switch (type) {
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

    public static bool IsPrimitiveValueValid(string type, string value) {
      switch (type) {
        case "bool": return value == "true" || value == "false";
        case "sbyte": return sbyte.TryParse(value, out _);
        case "byte": return byte.TryParse(value, out _);
        case "short": return short.TryParse(value, out _);
        case "ushort": return ushort.TryParse(value, out _);
        case "int": return int.TryParse(value, out _);
        case "uint": return uint.TryParse(value, out _);
        case "long": return long.TryParse(value, out _);
        case "ulong": return ulong.TryParse(value, out _);
        case "float": return float.TryParse(value, out _);
        case "double": return double.TryParse(value, out _);
      }

      return false;
    }
  }
}