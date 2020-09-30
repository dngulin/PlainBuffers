using System;
using System.IO;
using PlainBuffers.CodeGen;
using PlainBuffers.Layout;
using PlainBuffers.Lexer;
using PlainBuffers.Parser;

namespace PlainBuffers {
  public class PlainBuffersCompiler {
    private readonly PlainBuffersLexer _lexer = new PlainBuffersLexer();
    private readonly PlainBuffersParser _parser = new PlainBuffersParser();
    private readonly IGenerator _generator;

    public PlainBuffersCompiler(IGenerator generator) {
      _generator = generator;
    }

    public (string[] Errors, string[] Warnings) Compile(string schemaPath, string generatePath) {
      using (var readStream = File.OpenRead(schemaPath))
      using (var writeStream = File.Create(generatePath)) {
        return Compile(readStream, writeStream);
      }
    }

    public (string[] Errors, string[] Warnings) Compile(Stream readStream, Stream writeStream) {
      var lexerResult = _lexer.Read(readStream);
      if (lexerResult.HasError(out var lexerError))
        return (new[] {lexerError}, Array.Empty<string>());

      var parserResult = _parser.Parse(lexerResult.Unwrap());
      if (parserResult.HasError(out var parserError))
        return (new[] {parserError}, Array.Empty<string>());

      var codeGenData = PlainBuffersLayout.Calculate(parserResult.Unwrap());

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