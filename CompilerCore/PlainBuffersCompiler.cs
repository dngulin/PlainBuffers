using System;
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

    public (bool Success, string[] Errors) Compile(string schemaPath, string generatePath) {
      using (var readStream = File.OpenRead(schemaPath))
      using (var writeStream = File.Create(generatePath)) {
        return Compile(readStream, writeStream);
      }
    }

    public (bool Success, string[] Errors) Compile(Stream readStream, Stream writeStream) {
      var parsedData = _parser.Parse(readStream);
      var codeGenData = PlainBuffersLayoutCalculator.Calculate(parsedData);

      var namingErrors = _generator.NamingChecker.GetNamingErrors(codeGenData);
      if (namingErrors.Length > 0)
        return (false, namingErrors);

      using (var writer = new StreamWriter(writeStream)) {
        _generator.Generate(codeGenData, writer);
      }

      return (true, Array.Empty<string>());
    }
  }
}