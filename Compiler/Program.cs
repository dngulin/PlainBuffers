using System;
using System.IO;
using PlainBuffers.CompilerCore;
using PlainBuffers.CompilerCore.Generators;
using PlainBuffers.CompilerCore.Parsers;
using PlainBuffers.CompilerCore.Parsing;

namespace PlainBuffers.Compiler {
  public static class Program {
    private static int Main(string[] args) {
      if (args.Length != 2) {
        Console.WriteLine("Usage: compiler <path to schema> <output path>");
        return 1;
      }

      var parser = new XmlParser(); // TODO: Select by argument
      var generator = new CSharpCodeGenerator(); // TODO: Select by argument
      var compiler = new PlainBuffersCompiler(parser, generator);

      try {
        var (errors, warnings) = compiler.Compile(args[0], args[1]);

        if (warnings.Length > 0) {
          Console.WriteLine("Naming warnings:");
          foreach (var message in warnings)
            Console.WriteLine($"  - {message}");
        }

        if (errors.Length > 0) {
          Console.WriteLine("Naming errors:");
          foreach (var message in errors)
            Console.WriteLine($"  - {message}");

          return 5;
        }
      }
      catch (IOException e) {
        Console.WriteLine($"IO Error: {e.Message}");
        return 2;
      }
      catch (ParsingException e) {
        Console.WriteLine($"Parsing error: {e.Message}");
        return 3;
      }
      catch (Exception e) {
        Console.WriteLine($"Internal error: {e.Message}");
        return 4;
      }

      return 0;
    }
  }
}