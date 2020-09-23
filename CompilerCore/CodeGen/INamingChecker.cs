using PlainBuffers.CompilerCore.CodeGen.Data;

namespace PlainBuffers.CompilerCore.CodeGen {
  public interface INamingChecker {
    string[] GetNamingErrors(CodeGenData data);
  }
}