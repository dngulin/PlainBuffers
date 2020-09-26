using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PlainBuffers.CompilerCore.Lexer.Data;
using LexerResult = PlainBuffers.CompilerCore.ErrorHandling.Result<PlainBuffers.CompilerCore.Lexer.Data.LexerData, string>;
using OpResult = PlainBuffers.CompilerCore.ErrorHandling.VoidResult<string>;

namespace PlainBuffers.CompilerCore.Lexer {
  internal class PlainBuffersLexer {
    private const byte NewLine = (byte) '\n';
    private const byte Slash = (byte) '/';

    private static readonly Dictionary<byte, Token> PrimitiveLexemes = new Dictionary<byte, Token> {
      {(byte) ':', Token.Colon},
      {(byte) ';', Token.Semicolon},
      {(byte) '=', Token.Assignment},
      {(byte) '{', Token.CurlyBraceLeft},
      {(byte) '}', Token.CurlyBraceRight},
      {(byte) '[', Token.SquareBraceLeft},
      {(byte) ']', Token.SquareBraceRight}
    };

    private static readonly HashSet<byte> WhiteSpaces = new HashSet<byte> {(byte) ' ', (byte) '\t', NewLine};

    private byte[] _buffer = new byte[1024];

    public LexerResult Read(Stream input) {
      var state = new LexerState();
      var data = new LexerData();

      var offset = 0;
      int bytesRead;

      while ((bytesRead = input.Read(_buffer, offset, _buffer.Length - offset)) > 0) {
        var length = offset + bytesRead;
        var span = _buffer.AsSpan(0, length);

        var opResult = ReadTokens(state, span, offset, data);
        if (opResult.HasError)
          return LexerResult.Fail(opResult.Error);

        offset = state.CurrBlockDataLen;
        if (offset == _buffer.Length)
          Array.Resize(ref _buffer, _buffer.Length * 2);
      }

      return LexerResult.Ok(data);
    }

    private static OpResult ReadTokens(LexerState state, in Span<byte> span, int offset, LexerData data) {
      for (var index = offset; index < span.Length; index++) {
        var value = span[index];

        var opResult = OpResult.Ok();
        switch (state.CurrentBlock) {
          case LexicalBlock.None:
            opResult = TryStartBlock(state, value, data);
            break;
          case LexicalBlock.Identifier:
            opResult = TryEndIdentifier(state, span, index, data);
            break;
          case LexicalBlock.CommentaryOpener:
            opResult = TryEndCommentaryOpener(state, value);
            break;
          case LexicalBlock.CommentaryBody:
            TryEndCommentaryBody(state, value);
            break;
          default:
            throw new ArgumentOutOfRangeException($"Unknown lexical block: {state.CurrentBlock}");
        }

        if (opResult.HasError)
          return opResult;

        if (value == NewLine) {
          state.Line++;
          state.Column = 0;
        }
        else {
          state.Column++;
        }
      }

      // Move not processed data to buffer start
      if (state.CurrBlockDataLen > 0) {
        var reminder = span.Slice(span.Length - state.CurrBlockDataLen, state.CurrBlockDataLen);
        reminder.CopyTo(span);
      }

      return OpResult.Ok();
    }

    private static OpResult TryStartBlock(LexerState state, byte value, LexerData data) {
      if (WhiteSpaces.Contains(value))
        return OpResult.Ok();

      if (PrimitiveLexemes.TryGetValue(value, out var token)) {
        data.Tokens.Enqueue((token, state.Position));
        return OpResult.Ok();
      }

      if (value == Slash) {
        StartBlock(state, LexicalBlock.CommentaryOpener);
        return OpResult.Ok();
      }

      if (!CheckIdentifierSymbol(value))
        return OpResult.Fail($"Invalid symbol at {state.Position}");

      StartBlock(state, LexicalBlock.Identifier);
      return OpResult.Ok();
    }

    private static OpResult TryEndCommentaryOpener(LexerState state, byte value) {
      if (value == Slash) {
        state.CurrentBlock = LexicalBlock.CommentaryBody;
        state.CurrBlockDataLen = 0; // Commentary has no data
        return OpResult.Ok();
      }

      return OpResult.Fail($"Invalid symbol at {state.Position}. Missing `/` at commentary start?");
    }

    private static void TryEndCommentaryBody(LexerState state, byte value) {
      if (value == NewLine)
        EndBlock(state);
    }

    private static OpResult TryEndIdentifier(LexerState state, Span<byte> span, int index, LexerData data) {
      var value = span[index];

      if (WhiteSpaces.Contains(value)) {
        EnqueueIdentifier(state, span, index, data);
        EndBlock(state);
        return OpResult.Ok();
      }

      if (PrimitiveLexemes.TryGetValue(value, out var token)) {
        EnqueueIdentifier(state, span, index, data);
        EndBlock(state);
        data.Tokens.Enqueue((token, state.Position));
        return OpResult.Ok();
      }

      if (value == Slash) {
        EnqueueIdentifier(state, span, index, data);
        StartBlock(state, LexicalBlock.CommentaryOpener);
        return OpResult.Ok();
      }

      if (!CheckIdentifierSymbol(value))
        return OpResult.Fail($"Invalid symbol at {state.Position}");

      state.CurrBlockDataLen++;
      return OpResult.Ok();
    }

    private static unsafe void EnqueueIdentifier(LexerState state, Span<byte> span, int index, LexerData data) {
      var strSlice = span.Slice(index - state.CurrBlockDataLen, state.CurrBlockDataLen);

      string value;
      fixed (byte* ptr = strSlice) {
        value = Encoding.ASCII.GetString(ptr, strSlice.Length);
      }

      data.Tokens.Enqueue((Token.Identifier, new Position(state.Line, state.Column - strSlice.Length)));
      data.Identifiers.Enqueue(value);
    }

    private static void StartBlock(LexerState state, LexicalBlock block) {
      state.CurrentBlock = block;
      state.CurrBlockDataLen = 1;
    }

    private static void EndBlock(LexerState state) {
      state.CurrentBlock = LexicalBlock.None;
      state.CurrBlockDataLen = 0;
    }

    private static bool CheckIdentifierSymbol(int value) {
      if (value >= 'a' && value <= 'z')
        return true;

      if (value >= 'A' && value <= 'Z')
        return true;

      if (value >= '0' && value <= '9')
        return true;

      switch (value) {
        case '.':
        case '-': // For numbers
        case '_':
          return true;
      }

      return false;
    }
  }
}