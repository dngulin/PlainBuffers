using PlainBuffers.CompilerCore.CodeGen.Data;

namespace PlainBuffers.CompilerCore.CodeGen {
  public interface INamingChecker {
    (string[] Errors, string[] Warnings) CheckNaming(CodeGenData data);
  }
}