using System.Collections.Generic;

namespace PlainBuffers.Lexer.Data {
  internal class LexerData {
    public readonly Queue<(Token Token, Position Pos)> Tokens = new Queue<(Token, Position)>();
    public readonly Queue<string> Identifiers = new Queue<string>();
  }
}