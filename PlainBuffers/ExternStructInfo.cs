using System;

namespace PlainBuffers {
  public class ExternStructInfo {
    internal enum StructKind {
      WithoutValues,
      WithEnumeratedValues,
      PlainBuffersStruct
    }

    internal readonly StructKind Kind;
    public readonly string Name;
    public readonly int Size;
    public readonly int Alignment;
    public readonly string[] Values;

    private ExternStructInfo(StructKind kind, string name, int size, int alignment, string[] values) {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentException("Name is not defined for the extern struct");

      if (size <= 0)
        throw new ArgumentException($"Extern struct {name} has wrong size");

      if (alignment <= 0 || alignment > size)
        throw new ArgumentException($"Extern struct {name} has wrong alignment size");

      if (values == null)
        throw new ArgumentException($"Possible values list is not set for for extern struct {name}!");

      Kind = kind;
      Name = name;
      Size = size;
      Alignment = alignment;
      Values = values;
    }

    public static ExternStructInfo WithoutValues(string name, int size, int alignment)
    {
      return new ExternStructInfo(StructKind.WithoutValues, name, size, alignment, Array.Empty<string>());
    }

    public static ExternStructInfo WithEnumeratedValues(string name, int size, int alignment, string[] values)
    {
      if (values == null || values.Length <= 0)
        throw new ArgumentException();

      return new ExternStructInfo(StructKind.WithEnumeratedValues, name, size, alignment, values);
    }

    public static ExternStructInfo PlainBuffersStruct(string name, int size, int alignment)
    {
      return new ExternStructInfo(StructKind.PlainBuffersStruct, name, size, alignment, Array.Empty<string>());
    }
  }
}