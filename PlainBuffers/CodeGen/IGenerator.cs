using System.IO;
using PlainBuffers.CodeGen.Data;

namespace PlainBuffers.CodeGen {
  public interface IGenerator {
    INamingChecker NamingChecker { get; }
    void Generate(CodeGenData data, TextWriter writer, ExternStructInfo[] externStructs);
  }
}