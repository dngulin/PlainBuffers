using System.Collections.Generic;

namespace PlainBuffers.Lexer.Data {
  internal class LexerData {
    public readonly Queue<Token> Tokens = new Queue<Token>();
    public readonly Queue<string> Values = new Queue<string>();
  }
}