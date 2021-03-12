using System;
using System.IO;
using PlainBuffers.CodeGen;
using PlainBuffers.Generators;

namespace PlainBuffers.Compiler {
  public static class Program {
    private const string CSharpSafeBuffers = "csharp-safe-buffers";
    private const string CSharpUnsafeBuffers = "csharp-unsafe-buffers";
    private const string CSharpUnsafeStructs = "csharp-unsafe-structs";

    private static int Main(string[] args) {
      if (args.Length != 3) {
        Console.WriteLine("Usage: compiler <generator> <path to schema> <output path>");
        return 1;
      }

      var genName = args[0];
      var generator = GetGenerator(genName);
      if (generator == null) {
        Console.WriteLine($"Unknown generator name: {genName}");
        Console.WriteLine($"Available generators: {CSharpSafeBuffers}, {CSharpUnsafeBuffers}, {CSharpUnsafeStructs}");
        return 2;
      }

      var compiler = new PlainBuffersCompiler(generator);
      try {
        var (errors, warnings) = compiler.Compile(args[1], args[2]);

        if (warnings.Length > 0) {
          Console.WriteLine("Warnings:");
          foreach (var message in warnings)
            Console.WriteLine($"  - {message}");
        }

        if (errors.Length > 0) {
          Console.WriteLine("Errors:");
          foreach (var message in errors)
            Console.WriteLine($"  - {message}");

          return 3;
        }
      }
      catch (IOException e) {
        Console.WriteLine($"IO Error: {e.Message}");
        return 4;
      }
      catch (Exception e) {
        Console.WriteLine($"Internal error: {e.Message}");
        return 5;
      }

      return 0;
    }

    private static IGenerator GetGenerator(string generator) {
      var namespaces = Array.Empty<string>();
      switch (generator) {
        case CSharpSafeBuffers: return new CSharpSafeBuffersGenerator(namespaces);
        case CSharpUnsafeBuffers: return new CSharpUnsafeBuffersGenerator(namespaces);
        case CSharpUnsafeStructs: return new CSharpUnsafeStructsGenerator(namespaces);
      }

      return null;
    }
  }
}