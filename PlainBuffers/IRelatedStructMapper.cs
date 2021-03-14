using System.Collections.Generic;

namespace PlainBuffers {
  public interface IRelatedStructMapper {
    string RemapNamespace(string ns);

    string RemapEnumName(string enumName);
    string RemapArrayName(string arrayName);
    string RemapStructName(string structName);

    string RemapMemberType(string memberType, Dictionary<string, string> remappedTypes);
    string RemapMemberDefaultValue(string memberType, string value);
  }
}