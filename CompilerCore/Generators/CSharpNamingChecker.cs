using System;
using PlainBuffers.CompilerCore.CodeGen;
using PlainBuffers.CompilerCore.CodeGen.Data;

namespace PlainBuffers.CompilerCore.Generators {
  public class CSharpNamingChecker : INamingChecker {
    public string[] GetNamingErrors(CodeGenData data) {
      return Array.Empty<string>(); // TODO: Impl validation
    }
  }
}