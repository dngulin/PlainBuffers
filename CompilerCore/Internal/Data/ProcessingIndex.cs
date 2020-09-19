using System.Collections.Generic;

namespace PlainBuffers.CompilerCore.Internal.Data {
  internal class ProcessingIndex {
    public readonly Dictionary<string, TypeMemoryInfo> Types = new Dictionary<string, TypeMemoryInfo> {
      {"bool", new TypeMemoryInfo(1)},
      {"sbyte", new TypeMemoryInfo(1)},
      {"byte", new TypeMemoryInfo(1)},
      {"short", new TypeMemoryInfo(2)},
      {"ushort", new TypeMemoryInfo(2)},
      {"int", new TypeMemoryInfo(4)},
      {"uint", new TypeMemoryInfo(4)},
      {"float", new TypeMemoryInfo(4)},
      {"long", new TypeMemoryInfo(8)},
      {"ulong", new TypeMemoryInfo(8)},
      {"double", new TypeMemoryInfo(8)}
    };

    public readonly HashSet<string> Enums = new HashSet<string>();
  }
}