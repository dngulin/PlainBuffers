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

Currently project is on an early development stage and supports code generation only for C# language.

## TODO

- Add documentation about usage and schema
- Unify safe and unsafe generated API
- Measure and optimize performance (reduce slicing, fast primitive types reinterpretation)
- Write tests for compiler internal code (currently only core library and generated code are covered by tests now)
- Generate non-wrapping data types serializable into a byte buffer?
- Add code generation for languages other then C#