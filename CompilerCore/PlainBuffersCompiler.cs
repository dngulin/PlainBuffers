using System.IO;
using PlainBuffers.CompilerCore.CodeGen;
using PlainBuffers.CompilerCore.Internal;
using PlainBuffers.CompilerCore.Parsing;

namespace PlainBuffers.CompilerCore {
  public class PlainBuffersCompiler {
    private readonly IParser _parser;
    private readonly IGenerator _generator;

    public PlainBuffersCompiler(IParser parser, IGenerator generator) {
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
      var parsedData = _parser.Parse(readStream);
      var codeGenData = PlainBuffersLayoutCalculator.Calculate(parsedData);

      using (var writer = new StreamWriter(writeStream)) {
        _generator.Generate(codeGenData, writer);
      }
    }
  }
}