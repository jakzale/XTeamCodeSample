open Benchmarks
open BenchmarkDotNet.Running

module Program =
    let [<EntryPoint>] main _ = 
        BenchmarkRunner.Run<SimpleBenchmark>() |> ignore
        0
