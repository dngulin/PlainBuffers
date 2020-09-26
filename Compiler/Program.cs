using System;
using System.IO;
using PlainBuffers.CompilerCore;
using PlainBuffers.CompilerCore.Generators;

namespace PlainBuffers.Compiler {
  public static class Program {
    private static int Main(string[] args) {
      if (args.Length != 2) {
        Console.WriteLine("Usage: compiler <path to schema> <output path>");
        return 1;
      }

      var generator = new CSharpCodeGenerator(); // TODO: Select by argument
      var compiler = new PlainBuffersCompiler(generator);

      try {
        var (errors, warnings) = compiler.Compile(args[0], args[1]);

        if (warnings.Length > 0) {
          Console.WriteLine("Warnings:");
          foreach (var message in warnings)
            Console.WriteLine($"  - {message}");
        }

        if (errors.Length > 0) {
          Console.WriteLine("Errors:");
          foreach (var message in errors)
            Console.WriteLine($"  - {message}");

          return 2;
        }
      }
      catch (IOException e) {
        Console.WriteLine($"IO Error: {e.Message}");
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