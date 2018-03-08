namespace Benchmarks

open Library
open BenchmarkDotNet.Attributes

module Experiment =

  /// Sample non-trivial calculation on up to last 8 elements
  let polynomial (store: IBoundList<int>): double =
    let p = [| 1.0; -2.0; 3.0; -5.0; 7.0; -11.0; 13.0; -17.0 |]
    let mutable result = 0.0

    let count = min (Array.length p) store.Count

    for i in 0 .. count - 1 do
      let a = store.Get(i) |> float
      let b = p.[i]
      result <- result + (a * b)
    
    result

  /// Run the experiment, and collect all the results
  let run (store: IBoundList<int>) (input: int array): double array =
    let result = ResizeArray()
    let mutable store = store
      
    for i in input do
        store <- store.Insert(i)
        if store.Count = store.MaxSize then
            polynomial store 
            |> result.Add
    
    result.ToArray()

type SimpleBenchmark () =
  let input =
    let random = System.Random(42)
    [| for _ in 0 .. 1000000 -> random.Next() |]

  /// Baseline Benchmark uising the Immutable BoundList
  [<Benchmark(Baseline = true)>]
  member __.BoundList() =
    let boundList = BoundList.empty 8 :> IBoundList<_>
    Experiment.run boundList input

  /// Benchmark using the Mutablex BoundList
  [<Benchmark>]
  member __.MutableBoundList() =
    let boundList = MutableBoundList(8) :> IBoundList<_>
    Experiment.run boundList input