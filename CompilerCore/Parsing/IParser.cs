using System.IO;
using PlainBuffers.CompilerCore.Parsing.Data;

namespace PlainBuffers.CompilerCore.Parsing {
  public interface IParser {
    ParsedData Parse(Stream readStream);
  }
}