using System;
using System.IO;
using PlainBuffers.CodeGen;
using PlainBuffers.Layout;
using PlainBuffers.Lexer;
using PlainBuffers.Parser;

namespace PlainBuffers {
  public class PlainBuffersCompiler {
    private readonly PlainBuffersLexer _lexer;
    private readonly PlainBuffersParser _parser;
    private readonly IGenerator _generator;

    public PlainBuffersCompiler(IGenerator generator, ExternStructInfo[] externStructs = null) {
      externStructs = externStructs ?? Array.Empty<ExternStructInfo>();

      _lexer = new PlainBuffersLexer();
      _parser = new PlainBuffersParser(externStructs);
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
      if (lexerResult.TryGetError(out var lexerError))
        return (new[] {lexerError}, Array.Empty<string>());

      var parserResult = _parser.Parse(lexerResult.Unwrap());
      if (parserResult.TryGetError(out var parserError))
        return (new[] {parserError}, Array.Empty<string>());

      var codeGenData = PlainBuffersLayout.Calculate(parserResult.Unwrap()); // TODO: Pass external structs

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