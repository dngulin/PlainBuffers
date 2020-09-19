using System.IO;
using PlainBuffers.CompilerCore.Generate.Data;

namespace PlainBuffers.CompilerCore.Generate {
  public interface IGenerator {
    void Generate(CodeGenData data, TextWriter writer);
  }
}