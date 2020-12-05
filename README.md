# PlainBuffers

PlainBuffers is a simple serialization library based on code generation.
It allows you to work directly with serialized data placed in a byte buffer.

The idea of the library is similar to [FlatBuffers](https://github.com/google/flatbuffers),
but PlainBuffers are designed for usage in data oriented designs.
Main differences from FlatBuffers:
- PlainBuffers allows you to work with buffers like with regular structures.
There aren't weird `Mutate`-methods, but there are normal getters, setters, indexers and iterators.
- PlainBuffers doesn't have structure size limitations. FlatBuffers' 64KB limit can cause troubles in some cases.
- PlainBuffers allows you to set default values for structure fields. Use the `WriteDefault` method to write them.
- PlainBuffers does an automatic memory layout optimization to reduce paddings.
- PlainBuffers doesn't support a lot of FlatBuffers' features like the dynamic memory management, the JSON serialization, unions, etc.

For example, see a sample [schema](PlainBuffers.Tests/Generated/SchemaSafe.pbs)
and related [generated code](PlainBuffers.Tests/Generated/SchemaSafe.cs).

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

## C# Code Generation

There are three modes for C# code generation:
- **Safe Buffer Wrapping**.
The mode produces `ref`-structures based on `Span<byte>` type.
Generated code is absolutely safe but contains a lot of safety checks.
- **Unsafe Buffer Wrapping**.
The mode produces `ref`-structures based on raw pointers.
Generated code requires to write some unsafe code for a buffer wrapping, but it is very fast.
You should guarantee that a lifetime of a buffer is greater then a lifetime of any wrapping structure.
- **Unsafe Regular Structs**.
The mode produces regular C# structures, with a bit of unsafe magic for arrays.
Be careful with lifetimes of references to array items and lifetimes of iterators.  

You can compare code generation results:
[safe wrappers](PlainBuffers.Tests/Generated/SchemaSafe.cs),
[unsafe wrappers](PlainBuffers.Tests/Generated/SchemaUnsafe.cs) and
[unsafe regular structures](PlainBuffers.Tests/Generated/SchemaFixedBuffers.cs).

## Unity Integration

All libraries are targeted to NetStandard 2.0 and can be easily integrated into modern unity projects.
Note that generated code is dependent on the `System.Memory` library.
It is not provided by unity runtime and can be downloaded from [nuget.org](https://www.nuget.org/packages/System.Memory/).

## TODO

- Mention the external types feature
- Generate a `ReverseEndianness` methods 
- Write tests for compiler internal code (currently only core library and generated code are covered by tests now)
- Add code generation for languages other then the C#