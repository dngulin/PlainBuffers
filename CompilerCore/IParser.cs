using System.IO;
using PlainBuffers.CompilerCore.Schema;

namespace PlainBuffers.CompilerCore {
  public interface IParser {
    SchemaInfo Parse(Stream readStream);
  }
}