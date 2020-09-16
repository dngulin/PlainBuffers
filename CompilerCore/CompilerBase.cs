using System.IO;

namespace PlainBuffers.CompilerCore {
  public class CompilerBase {
    private readonly IParser _parser;
    private readonly IGenerator _generator;

    public CompilerBase(IParser parser, IGenerator generator) {
      _parser = parser;
      _generator = generator;
    }

    public void Compile(string schemaPath, string generatePath) {
      using (var readStream = File.OpenRead(schemaPath))
      using (var writeStream = File.Create(generatePath)) {
        Compile(readStream, writeStream);
      }
    }

    public void Compile(Stream readStream, Stream writeStream) {
      var schema = _parser.Parse(readStream);
      using (var writer = new StreamWriter(writeStream)) {
        _generator.Generate(schema, writer);
      }
    }
  }
}