namespace PlainBuffers.CompilerCore.Generate.Data {
  public class CodeGenData {
    public readonly string NameSpace;
    public readonly CodeGenType[] Types;

    public CodeGenData(string nameSpace, CodeGenType[] types) {
      NameSpace = nameSpace;
      Types = types;
    }
  }
}