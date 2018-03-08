module Tests

//open Xunit
open FsUnit.Xunit
open FsCheck.Xunit
open FsCheck

open Library

/// Inserting an element increases the Count by 1 up to MaxCount
let boundListInsertInvariant (boundList : IBoundList<'a>) (elem: 'a): IBoundList<'a> =
  let initialCount = boundList.Count
  let result = boundList.Insert elem

  result.Count |> should equal (min result.MaxSize (initialCount + 1))

  result

/// Test for equality between two IBoundLists
let boundListEqual (a: IBoundList<'a>) (b: IBoundList<'a>) =
  a.MaxSize |> should equal b.MaxSize
  a.Count |> should equal b.Count

  for i in 0 .. a.Count - 1 do
    let aI = a.Get(i)
    let bI = b.Get(i)
    aI |> should equal bI
 
/// After inserting the same element, two IBoundList should remain equal
let boundListSameInvariant (a : IBoundList<_>, b : IBoundList<_>) (elem : _) : IBoundList<_> * IBoundList<_> =
  let newA = a.Insert(elem)
  let newB = b.Insert(elem)

  boundListEqual newA newB

  (newA, newB)

[<Property>]
let ``A new BoundList should be empty with proper MaxSize`` (maxSize: int) =
  maxSize > 0 ==>
  lazy (let boundList: IBoundList<int> = BoundList.empty maxSize :> IBoundList<_>
        boundList.MaxSize |> should equal maxSize
        boundList.Count |> should equal 0)

[<Property>]
let ``BoundList should contain last MaxSize elements of a sequence`` (maxSize: int) (elems: int array) =
  maxSize > 0 ==>
  lazy (let initialBoundList: IBoundList<int> = BoundList.empty maxSize :> IBoundList<_>
        // Get the boundList that should contain the last maxSize elements
        let boundList =
          elems
          |> Array.fold (fun (boundList: IBoundList<_>) item -> boundList.Insert(item)) initialBoundList
        // Calculate last maxSize elements directly
        let lastElems =
          elems
          |> Array.rev
          |> Array.truncate maxSize
          |> Array.rev

        for i in 0 .. Array.length lastElems - 1 do
          boundList.Get(i) |> should equal lastElems.[i])

[<Property>]
let ``BoundList Count should increase with insert up to MaxSize`` (maxSize: int) (elems: int array) =
  maxSize > 0 ==>
  lazy (let boundList = BoundList.empty maxSize :> IBoundList<_>
        elems
        |> Array.fold boundListInsertInvariant boundList
        |> ignore)

[<Property>]
let ``MutableBoundList behaves the same way as BoundList`` (maxSize: int) (elems: int array) =
  (maxSize > 0 && maxSize < System.Int32.MaxValue) ==>
  lazy (let boundList = BoundList.empty maxSize :> IBoundList<_>
        let mutableBoundList = MutableBoundList(maxSize) :> IBoundList<_>

        // Check for the initial case
        boundListEqual boundList mutableBoundList

        elems
        |> Array.fold boundListSameInvariant (boundList, mutableBoundList)
        |> ignore)