using System.IO;
using PlainBuffers.CompilerCore.Schema;

namespace PlainBuffers.CompilerCore {
  public interface IGenerator {
    void Generate(CodeGenData data, TextWriter writer);
  }
}