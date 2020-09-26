using System.Collections.Generic;

namespace PlainBuffers.CompilerCore.Parser {
  internal class ParserState {
    private readonly Stack<ParserSubState> _stack = new Stack<ParserSubState>();

    public ParserState() {
      _stack.Push(new ParserSubState(ParsingBlockType.None, null));
    }

    public ParserSubState CurrentBlock => _stack.Peek();

    public void StartBlock(ParsingBlockType type, string name) => _stack.Push(new ParserSubState(type, name));
    public void EndBlock() => _stack.Pop();
  }

  internal readonly struct ParserSubState {
    public readonly ParsingBlockType Type;
    public readonly string Name;

    public ParserSubState(ParsingBlockType type, string name) {
      Type = type;
      Name = name;
    }
  }
}