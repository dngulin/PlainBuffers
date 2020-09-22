using System.Collections.Generic;

namespace PlainBuffers.CompilerCore.Parsing {
  public class ParsingIndex {
    public readonly HashSet<string> KnownTypes;
    public readonly Dictionary<string, string[]> EnumValues = new Dictionary<string, string[]>();

    public ParsingIndex(IEnumerable<string> primitives) {
      KnownTypes = new HashSet<string>(primitives);
    }
  }
}