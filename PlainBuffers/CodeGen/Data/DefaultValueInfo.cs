using System;

namespace PlainBuffers.CodeGen.Data {
  public enum DefaultValueVariant {
    WriteZeroes,
    AssignValue,
    AssignTypeMember,
    CallWriteDefaultMethod
  }

  public readonly struct DefaultValueInfo {
    public readonly DefaultValueVariant Variant;
    public readonly string Identifier;

    private DefaultValueInfo(DefaultValueVariant variant, string identifier) {
      Variant = variant;
      Identifier = identifier;
    }

    public DefaultValueInfo WithCustomDefaultValueIfPossible(string customDefaultValue) {
      switch (Variant)
      {
        case DefaultValueVariant.AssignValue:
        case DefaultValueVariant.AssignTypeMember:
          return new DefaultValueInfo(Variant, customDefaultValue ?? Identifier);
      }

      return this;
    }

    public static DefaultValueInfo WriteZeroes()
    {
      return new DefaultValueInfo(DefaultValueVariant.WriteZeroes, null);
    }

    public static DefaultValueInfo AssignValue(string value)
    {
      if (string.IsNullOrEmpty(value))
        throw new ArgumentException();

      return new DefaultValueInfo(DefaultValueVariant.AssignValue, value);
    }

    public static DefaultValueInfo AssignTypeMember(string memberName)
    {
      if (string.IsNullOrEmpty(memberName))
        throw new ArgumentException();

      return new DefaultValueInfo(DefaultValueVariant.AssignTypeMember, memberName);
    }

    public static DefaultValueInfo CallWriteDefaultMethod()
    {
      return new DefaultValueInfo(DefaultValueVariant.CallWriteDefaultMethod, null);
    }
  }
}