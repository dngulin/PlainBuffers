using System;
using System.Collections.Generic;
using System.IO;
using PlainBuffers.CodeGen;
using PlainBuffers.Layout;
using PlainBuffers.Lexer;
using PlainBuffers.Parser;
using PlainBuffers.Parser.Data;

namespace PlainBuffers {
  public class PlainBuffersCompiler {
    private readonly ExternStructInfo[] _externStructs;
    private readonly IRelatedStructMapper _mapper;

    private readonly PlainBuffersLexer _lexer;
    private readonly PlainBuffersParser _parser;
    private readonly IGenerator _generator;

    public PlainBuffersCompiler(IGenerator generator, ExternStructInfo[] externStructs = null, IRelatedStructMapper mapper = null) {
      _externStructs = externStructs ?? Array.Empty<ExternStructInfo>();
      _mapper = mapper;

      _lexer = new PlainBuffersLexer();
      _parser = new PlainBuffersParser(_externStructs);
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

      var parsedData = parserResult.Unwrap();
      if (_mapper != null)
        ApplyRemappings(_mapper, parsedData);

      var codeGenData = PlainBuffersLayout.Calculate(parsedData, _externStructs);

      var (errors, warnings) = _generator.NamingChecker.CheckNaming(codeGenData);
      if (errors.Length > 0)
        return (errors, warnings);

      using (var writer = new StreamWriter(writeStream)) {
        _generator.Generate(codeGenData, writer);
      }

      return (errors, warnings);
    }

    private static void ApplyRemappings(IRelatedStructMapper mapper, ParsedData parsedData) {
      var remappedTypes = new Dictionary<string, string>();

      parsedData.Namespace = mapper.RemapNamespace(parsedData.Namespace);

      foreach (var parsedType in parsedData.Types) {
        switch (parsedType) {
          case ParsedEnum pEnum:
            pEnum.Name = RemapTypeName(pEnum.Name, mapper.RemapEnumName(pEnum.Name), remappedTypes);
            break;

          case ParsedArray pArray:
            pArray.Name = RemapTypeName(pArray.Name, mapper.RemapArrayName(pArray.Name), remappedTypes);
            break;

          case ParsedStruct pStruct:
            pStruct.Name = RemapTypeName(pStruct.Name, mapper.RemapStructName(pStruct.Name), remappedTypes);

            foreach (var field in pStruct.Fields) {
              field.Type = mapper.RemapFieldType(field.Type, remappedTypes);
              field.DefaultValue = mapper.RemapDefaultFieldValue(field.Type, field.DefaultValue);
            }
            break;
        }
      }
    }

    private static string RemapTypeName(string oldName, string newName, Dictionary<string, string> remappedTypes) {
      remappedTypes.Add(oldName, newName);
      return newName;
    }
  }
}