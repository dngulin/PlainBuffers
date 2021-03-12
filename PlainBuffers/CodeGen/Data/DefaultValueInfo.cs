namespace PlainBuffers.CodeGen.Data {
  public enum DefaultValueVariant {
    Assign,
    AssignTypeMember,
    CallInstanceMethod
  }

  public readonly struct DefaultValueInfo {
    public readonly DefaultValueVariant Variant;
    public readonly string Identifier;

    public DefaultValueInfo(DefaultValueVariant variant, string identifier) {
      Variant = variant;
      Identifier = identifier;
    }

    public DefaultValueInfo WithCustomDefaultValue(string customDefaultValue) {
      return new DefaultValueInfo(Variant, customDefaultValue ?? Identifier);
    }
  }
}