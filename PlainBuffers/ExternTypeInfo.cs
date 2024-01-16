using System;

namespace PlainBuffers {
  public class ExternTypeInfo {
    internal enum StructKind {
      WithoutValues,
      WithEnumeratedValues,
      WithWriteDefaultMethod
    }

    internal readonly StructKind Kind;
    public readonly string Name;
    public readonly int Size;
    public readonly int Alignment;
    public readonly string[] Values;

    private ExternTypeInfo(StructKind kind, string name, int size, int alignment, string[] values) {
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

    public static ExternTypeInfo WithoutValues(string name, int size, int alignment)
    {
      return new ExternTypeInfo(StructKind.WithoutValues, name, size, alignment, Array.Empty<string>());
    }

    public static ExternTypeInfo WithEnumeratedValues(string name, int size, int alignment, string[] values)
    {
      if (values == null || values.Length <= 0)
        throw new ArgumentException();

      return new ExternTypeInfo(StructKind.WithEnumeratedValues, name, size, alignment, values);
    }

    public static ExternTypeInfo WithWriteDefaultMethod(string name, int size, int alignment)
    {
      return new ExternTypeInfo(StructKind.WithWriteDefaultMethod, name, size, alignment, Array.Empty<string>());
    }
  }
}