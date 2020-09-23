using System;

namespace PlainBuffers.CompilerCore.Parsing {
  public class ParsingException : Exception {
    public ParsingException(string msg) : base(msg) {}
  }
}