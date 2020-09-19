using System.IO;
using PlainBuffers.CompilerCore.CodeGen.Data;

namespace PlainBuffers.CompilerCore.CodeGen {
  public interface IGenerator {
    void Generate(CodeGenData data, TextWriter writer);
  }
}