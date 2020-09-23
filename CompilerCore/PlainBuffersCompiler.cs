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

    public (string[] Errors, string[] Warnings) Compile(string schemaPath, string generatePath) {
      using (var readStream = File.OpenRead(schemaPath))
      using (var writeStream = File.Create(generatePath)) {
        return Compile(readStream, writeStream);
      }
    }

    public (string[] Errors, string[] Warnings) Compile(Stream readStream, Stream writeStream) {
      var parsedData = _parser.Parse(readStream);
      var codeGenData = PlainBuffersLayoutCalculator.Calculate(parsedData);

      var (errors, warnings) = _generator.NamingChecker.CheckNaming(codeGenData);
      if (errors.Length > 0)
        return (errors, warnings);

      using (var writer = new StreamWriter(writeStream)) {
        _generator.Generate(codeGenData, writer);
      }

      return (errors, warnings);
    }
  }
}