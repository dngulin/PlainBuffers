using System.IO;
using PlainBuffers.CompilerCore.Schema;

namespace PlainBuffers.CompilerCore {
  public interface IParser {
    ParsedData Parse(Stream readStream);
  }
}