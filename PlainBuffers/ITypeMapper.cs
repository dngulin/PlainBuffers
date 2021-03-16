namespace PlainBuffers {
  public interface ITypeMapper {
    string RemapNamespace(string ns);

    string RemapEnumName(string enumName);
    string RemapArrayName(string arrayName);
    string RemapStructName(string structName);

    string RemapMemberType(string memberType);
    string RemapMemberDefaultValue(string memberType, string value);
  }
}