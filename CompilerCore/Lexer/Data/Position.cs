namespace PlainBuffers.CompilerCore.Lexer.Data {
  internal readonly struct Position {
    public readonly int Line;
    public readonly int Column;

    public Position(int line, int column) {
      Line = line;
      Column = column;
    }

    public override string ToString() => $"{Line + 1}:{Column + 1}";
  }
}