using System.IO;
using PlainBuffers.CompilerCore.CodeGen.Data;

namespace PlainBuffers.CompilerCore.CodeGen {
  public interface IGenerator {
    INamingChecker NamingChecker { get; }
    void Generate(CodeGenData data, TextWriter writer);
  }
}