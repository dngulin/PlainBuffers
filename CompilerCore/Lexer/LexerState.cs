using PlainBuffers.CompilerCore.Lexer.Data;

namespace PlainBuffers.CompilerCore.Lexer {
  internal class LexerState {
    public int Line;
    public int Column;

    public Position Position => new Position(Line, Column);

    public LexicalBlock CurrentBlock;
    public int CurrBlockDataLen;
  }
}