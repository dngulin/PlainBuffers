using PlainBuffers.CodeGen.Data;

namespace PlainBuffers.CodeGen {
  public interface INamingChecker {
    (string[] Errors, string[] Warnings) CheckNaming(CodeGenData data);
  }
}