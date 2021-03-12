using System;

namespace PlainBuffers {
  public class ExternStructInfo {
    public readonly string Name;
    public readonly int Size;
    public readonly int Alignment;
    public readonly string[] Values;

    public ExternStructInfo(string name, int size, int alignment, string[] values) {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentException("Name is not defined for the extern struct");

      if (size <= 0)
        throw new ArgumentException($"Extern struct {name} has wrong size");

      if (alignment <= 0 || alignment > size)
        throw new ArgumentException($"Extern struct {name} has wrong alignment size");

      if (values == null)
        throw new ArgumentException($"Possible values list is not set for for extern struct {name}!");

      Name = name;
      Size = size;
      Alignment = alignment;
      Values = values;
    }
  }
}