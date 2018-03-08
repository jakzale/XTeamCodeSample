# Mutable vs Immutable Implementation
This code sample shows the process of writing code by starting with an immutable
implementation, which in F# can be very concise (14 lines in this case, without
the code implementing the interface).

However, immutable data structures can introduce performance penalties in hot
loops, therefore we will use the immutable data structure as a reference to
implement an optimised, mutable implementation.

We will show that mutable implementation conforms to the reference
implementation by using property based testing, and we will show that indeed the
mutable implementation is faster by using some simple benchmarks (by around
factor of 5 on my machine).

# Requirements
.NET Core SDK for .NET Core 2.0

Verified against:

- .NET Core 2.0.5
- .NET Core SDK 2.1.4

# Tests
Tests.fs verifies the properties of the Immutable BoundList implementation, and
then verifies whether the Mutable BoundList implementation conforms to the
reference Immutable BoundList implementation.

All the tests were written using property-based testing, which for each test
will generate 100 random test cases.

In order to run all the tests use the following command
``` sh
$ dotnet test
```

# Benchmarks
Benchmarks.fs compares the performance of two IBoundList implementations used
within a non-trivial example experiment.  As we can see in the sample benchmark
results below, the mutable implementation makes the example experiment run
almost 5 times faster.

In order to run benchmarks use
``` sh
$ dotnet run -c Release
```

## Sample Benchmark Result
```
// * Summary *

BenchmarkDotNet=v0.10.13, OS=macOS 10.13.3 (17D102) [Darwin 17.4.0]
Intel Core i7-6920HQ CPU 2.90GHz (Skylake), 1 CPU, 8 logical cores and 4 physical cores
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (CoreCLR 4.6.0.0, CoreFX 4.6.26018.01), 64bit RyuJIT DEBUG
  DefaultJob : .NET Core 2.0.5 (CoreCLR 4.6.0.0, CoreFX 4.6.26018.01), 64bit RyuJIT


           Method |      Mean |     Error |    StdDev | Scaled |
----------------- |----------:|----------:|----------:|-------:|
        BoundList | 400.66 ms | 2.1352 ms | 1.9973 ms |   1.00 |
 MutableBoundList |  89.08 ms | 0.9915 ms | 0.9275 ms |   0.22 |

// * Legends *
  Mean   : Arithmetic mean of all measurements
  Error  : Half of 99.9% confidence interval
  StdDev : Standard deviation of all measurements
  Scaled : Mean(CurrentBenchmark) / Mean(BaselineBenchmark)
  1 ms   : 1 Millisecond (0.001 sec)
```