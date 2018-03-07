module Tests

open Xunit
open FsUnit.Xunit

open Library

let boundListMaxSizeInvariant (boundList: IBoundList<'a>) (elem: 'a): IBoundList<'a> =
  let result = boundList.Insert elem

  result.Count |> should be (lessThanOrEqualTo result.MaxSize)

  result

let boundListInsertInvariant (boundList : IBoundList<'a>) (elem: 'a): IBoundList<'a> =
  let initialCount = boundList.Count
  let result = boundList.Insert elem

  result.Count |> should equal (min result.MaxSize (initialCount + 1))

  result
 

[<Fact>]
let ``BoundList Count should never be greater than MaxSize`` () =
  let boundList = BoundList.empty 8 :> IBoundList<_>
  [0 .. 100]
  |> List.fold boundListMaxSizeInvariant boundList
  |> ignore

[<Fact>]
let ``BoundList Count should increase with insert up to MaxSize`` () =
  let boundList = BoundList.empty 8 :> IBoundList<_>
  [0 .. 100]
  |> List.fold boundListInsertInvariant boundList

[<Fact>]
let ``MutableBoundList Count should never be greater than MaxSize`` () =
  let boundList = MutableBoundList(8) :> IBoundList<_>
  [0 .. 100]
  |> List.fold boundListMaxSizeInvariant boundList
  |> ignore

[<Fact>]
let ``MutableBoundList Count should increase with insert up to MaxSize`` () =
  let boundList = MutableBoundList(8) :> IBoundList<_>
  [0 .. 100]
  |> List.fold boundListInsertInvariant boundList
