using System.Collections.Generic;

namespace PlainBuffers {
  public interface IRelatedStructMapper {
    string RemapNamespace(string ns);

    string RemapEnumName(string enumName);
    string RemapArrayName(string arrayName);
    string RemapStructName(string structName);

    string RemapFieldType(string fieldType, Dictionary<string, string> remappedTypes);
    string RemapDefaultFieldValue(string fieldType, string value);
  }
}