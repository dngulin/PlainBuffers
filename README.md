# PlainBuffers

PlainBuffers is a simple code generation library.
It allows you to generate data structures described by the data schema.

The idea of the library is similar to [FlatBuffers](https://github.com/google/flatbuffers),
but PlainBuffers are designed for usage in data oriented designs.
Main differences from FlatBuffers:
- PlainBuffers allows you to work with regular structures.
There aren't weird `Mutate`-methods, but there are normal getters, setters, indexers and iterators.
- PlainBuffers doesn't have structure size limitations. FlatBuffers' 64KB limit can cause troubles in some cases.
- PlainBuffers allows you to set default values for structure fields. Use the `WriteDefault` method to write them.
- PlainBuffers does an automatic memory layout optimization to reduce paddings.
- PlainBuffers allows you to embed external structures into generated types.
- PlainBuffers doesn't support a lot of FlatBuffers' features like the dynamic memory management, the JSON serialization, unions, etc.

For example, see a sample [schema](PlainBuffers.Tests/Generated/Schema.pbs)
and related [generated code](PlainBuffers.Tests/Generated/Schema.cs).

Currently project is at an early development stage and supports code generation for the C# language only.

## Schema Syntax

The syntax of the schema language is described below.

```
// This is a commentary
namespace @name.dot.separated {
    enum @name : @underlyingType *flags { // The flags modifier is oprtional
        @name = @value;
        @name = @value;
    }
    
    struct @name {
        @type @name;
        @type @name = @default; // Default values are allowed only for primitive types
    }
    
    array @name @itemType[@length] = @default; // Array items can have default values too
}
```

- Commentary is started from `//`
- Identifiers started form `@` should be defined by user.
- Identifiers started from `*` are optional (currently it is only `flags` modifier for enum types).

Only one namespace should be defined in schema.

You can use primitive types for struct fields, array item types and as underlying types of enum (integers only).
All primitive types are [named same](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types) 
as in the C# language. But `char`, `decimal` and reference types are not supported.

Enum values should be annotated by numbers. Logical and shift expressions aren't supported now.

## C# Safety Limitations

Generated arrays are not absolutely safe. Be careful with _references to array items_ and _iterators_:
ensure that lifetimes of them are shorter then the array lifetime.

## Unity Integration

All libraries are targeted to NetStandard 2.0 and can be easily integrated into modern unity projects.
Note that generated code is dependent on the `System.Memory` library.
It is not provided by unity runtime and can be downloaded from [nuget.org](https://www.nuget.org/packages/System.Memory/).

## TODO

- Write tests for compiler internal code (currently only core library and generated code are covered by tests now)
- Add code generation for languages other then the C#