# PlainBuffers

PlainBuffers is a simple serialization library based on code generation.
It allows you to work directly with serialized data placed in a byte buffer using generated wrapping types.

An idea of the library is similar to [FlatBuffers](https://github.com/google/flatbuffers),
but focused on code generation for data-oriented designs.
Main differences to FlatBuffers:
- Better C# API: simple initialization, fields generated as fields (no weird `Mutate`-methods),
arrays have indexers and iterators, `WriteDefault` methods
- No struct size limitations. FlatBuffers' 64KB limit can be painful in some cases
- Automatic memory layout optimization to reduce paddings
- Buffer should be pre-allocated. There are no `table` analogs in terms of FlatBuffers
- Default values are allowed. FlatBuffers doesn't support it for `struct` types
- No other FlatBuffers features like JSON serialization, unions, etc

For example see a sample [schema](Tests/Generated/Schema.pbs) and related [generated code](Tests/Generated/Schema.cs).
Note that a `PlainBuffers.Core` library is required by generated code. It contains wrappers for primitive types.

Currently project is on an early development stage and supports code generation only for the C# language.

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

There are safe and unsafe modes for C# code generation.

Code generated in the _safe_ mode is based on the `Core` library, 
have additional bounds checking (slow), and have guaranties about endianness (big endian).

Code generated in the _unsafe_ mode is pointer-based and not dependent on the `Core` library,
fast and have more useful API. But it requires to write unsafe code for buffer wrapping and uses local endianness.

For example you can compare tests for [safe](Tests/GeneratedTypesTest.cs) and
[unsafe](Tests/GeneratedUnsafeTypesTest.cs) generated code.

## Unity Integration

All libraries are targeted to NetStandard 2.0 and can be easily integrated into modern unity projects.
Note that generated code is dependent on the `System.Memory` library.
It is not provided by unity runtime and can be downloaded from [nuget.org](https://www.nuget.org/packages/System.Memory/).

## TODO

- Unify safe and unsafe generated API
- Improve name checking for C# (add language keywords, split safe and unsafe naming checkers?)
- Measure and optimize performance (reduce slicing, fast primitive types reinterpretation)
- Write tests for compiler internal code (currently only core library and generated code are covered by tests now)
- Generate non-wrapping data types serializable into a byte buffer?
- Add code generation for languages other then C#