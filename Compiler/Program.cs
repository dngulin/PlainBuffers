using System;
using PlainBuffers.CompilerCore;
using PlainBuffers.CompilerCore.Generators;
using PlainBuffers.CompilerCore.Parsers;

namespace PlainBuffers.Compiler {
  public static class Program {
    private static int Main(string[] args) {
      if (args.Length != 2) {
        Console.WriteLine("Usage: compiler <path to schema> <output path>");
        return 1;
      }

      var parser = new XmlParser(); // TODO: Select by argument
      var generator = new CSharpGenerator(); // TODO: Select by argument
      var compiler = new PlainBuffersCompiler(parser, generator);

      try {
        compiler.Compile(args[0], args[1]);
      }
      catch (Exception e) {
        Console.WriteLine($"Failed to compile scheme: {e.Message}");
        return 1;
      }

      return 0;
    }
  }
}