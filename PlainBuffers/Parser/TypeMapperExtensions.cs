using System;
using System.Collections.Generic;

namespace PlainBuffers.Parser {
  internal static class TypeMapperExtensions {
    public static string RemapTypeName(this ITypeMapper mapper, TypeKind typeKind, string typeName, Dictionary<string, string> remappedTypes) {
      var newTypeName = mapper.GetRemappedTypeName(typeKind, typeName);
      if (newTypeName != typeName)
        remappedTypes.Add(typeName, newTypeName);

      return newTypeName;
    }

    public static string RemapMemberType(this ITypeMapper mapper, string memberType, Dictionary<string, string> remappedTypes) {
      if (remappedTypes.TryGetValue(memberType, out var remappedType))
        return remappedType;

      return mapper.RemapMemberType(memberType);
    }

    private static string GetRemappedTypeName(this ITypeMapper mapper, TypeKind typeKind, string typeName) {
      switch (typeKind) {
        case TypeKind.Enum: return mapper.RemapEnumName(typeName);
        case TypeKind.Array: return mapper.RemapArrayName(typeName);
        case TypeKind.Struct: return mapper.RemapStructName(typeName);
        default:
          throw new ArgumentOutOfRangeException(nameof(typeKind), typeKind, null);
      }
    }
  }
}