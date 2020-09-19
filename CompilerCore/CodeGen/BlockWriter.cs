using System;
using System.IO;

namespace PlainBuffers.CompilerCore.CodeGen {
  public readonly struct BlockWriter : IDisposable {
    private readonly TextWriter _writer;
    private readonly string _indent;
    private readonly int _depth;


    public BlockWriter(TextWriter writer, string indent, int depth, string header) {
      _writer = writer;
      _indent = indent;
      _depth = depth;

      for (var i = 0; i < _depth; i++)
        _writer.Write(_indent);

      _writer.Write(header);
      _writer.WriteLine(" {");
    }

    public BlockWriter Sub(string header) => new BlockWriter(_writer, _indent, _depth + 1, header);

    public void WriteLine(string line = "") {
      if (string.IsNullOrEmpty(line)) {
        _writer.WriteLine();
        return;
      }

      for (var i = 0; i < _depth + 1; i++)
        _writer.Write(_indent);

      _writer.WriteLine(line);
    }

    public void Dispose() {
      for (var i = 0; i < _depth; i++)
        _writer.Write(_indent);

      _writer.WriteLine("}");
    }
  }
}