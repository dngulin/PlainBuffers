using System.IO;
using PlainBuffers.CompilerCore.Parse.Data;

namespace PlainBuffers.CompilerCore.Parse {
  public interface IParser {
    ParsedData Parse(Stream readStream);
  }
}