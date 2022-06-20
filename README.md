# PlainBuffers

PlainBuffers is a simple code generation library.
It allows you to generate data structures described by the data schema.

The idea of the library is similar to [FlatBuffers](https://github.com/google/flatbuffers),
but PlainBuffers are designed for usage in data oriented designs.
Main differences from FlatBuffers:
- PlainBuffers allows you to work with regular structures.
There aren't weird `Mutate`-methods, but there are usual struct fields. But it is still a bit tricky with array accessors (see C# Generated Arrays Limitations).
- PlainBuffers doesn't have structure size limitations. FlatBuffers' 64KB limit can cause troubles in some cases.
- PlainBuffers allows you to set default values for structure fields. Use the `WriteDefault` method to write them.
- PlainBuffers does an automatic memory layout optimization to reduce paddings.
- PlainBuffers allows you to embed external structures into generated types.
- PlainBuffers doesn't support a lot of FlatBuffers' features like the dynamic memory management, the JSON serialization, unions, etc.

For example, see a sample [schema](PlainBuffers.Tests/Generated/Schema.pbs)
and related [generated code](PlainBuffers.Tests/Generated/Schema.cs).

Currently project supports code generation only for the C# language.

## Schema Example

The example of the schema language is listed below.

```
// This is a commentary

// Only one namespace should be defined in schema
namespace Name.Dot.Separated {

    // Base enum type should be defined explicitly
    enum SampleEnum : byte {
        Foo = 1; // All enum values should be annotated by numbers
        Bar = 2;
    }
    
    // Flag enums are generated with an optional `flags` modifier
    enum SampleFlags : ushort flags {
        A = 1;
        B = 2;
        AB = 3;
        C = 4;
    }
    
    // Struct definition example
    // To set default values you should to call a `WriteDefault` method
    struct SampleStruct {
        bool FieldA = true;
        int FieldB = 42;
        SampleEnum FiledC = Foo; // Enums also support default values
        
        // If default value is not defined for enum field,
        // the first enum variant will be used in the `WriteDefault` method.
        // So, this defeinition equals to the `SampleFlags FiledD = A`
        SampleFlags FiledD;
    }
    
    // Array definition example
    // It will generate struct with all the data stored inplace
    array SampleArray SampleStruct[10];
    
    // Array items can have default values too
    array IndicesArray int[32] = -1;
    
    // Complex truct example
    // Any type declared above can be referenced
    struct ComplexStruct {
        SampleStruct FieldA;
        SampleArray FieldB;
        IndicesArray FieldC;
    }
}
```

You can use primitive types for struct fields, array item types and as underlying types of enum (integers only).
All primitive types are [named same](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types) 
as in the C# language. But `char`, `decimal` and reference types are not supported.

Enum values should be annotated by numbers. Logical and shift expressions aren't supported now.

## C# Generated Arrays Limitations

Generated arrays are not absolutely safe. Be careful with **references to array items** and **iterators**:
ensure that lifetimes of them are shorter then the array lifetime.

Since the 2.0.0 version, array indexers were removed because they can produce defensive copies when the array accessed by readonly reference.
Instead of them you should use `RefAt` and `RefReadonlyAt` extension methods depending on access type.
To get iterators you should use extension methods `RefIter` and `RefReadonlyIter`.
See usage examples in [tests](PlainBuffers.Tests/GeneratedCodeTests.cs#L43).

## Unity Integration

PlainBuffers library provides the `CSharpUnityCodeGenerator` class that uses `UnsafeUtility` instead of `Span<T>` and generates code without any external dependencies. 

Compiler library itself are targeted to NetStandard 2.0 and dependent on the `System.Memory` library.
It can be downloaded from [nuget.org](https://www.nuget.org/packages/System.Memory/) and integrated into unity project.

## TODO

- Write tests for compiler internal code (currently only core library and generated code are covered by tests now)
- Add code generation for languages other then the C#
