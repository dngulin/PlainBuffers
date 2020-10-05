namespace PlainBuffers.Lexer.Data {
  internal readonly struct Token {
    public readonly TokenType Type;
    public readonly Position Position;

    public Token(TokenType type, Position position) {
      Type = type;
      Position = position;
    }
  }
}