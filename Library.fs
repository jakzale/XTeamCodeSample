namespace Library

open System

type IBoundList<'a> =
  abstract member MaxSize: int with get
  abstract member Insert: 'a -> IBoundList<'a>
  abstract member Count: int with get
  abstract member Get: int -> 'a


type BoundList<'a> =
  private {
    maxSize: int
    data: 'a list
  }
with 
  static member Empty(maxSize: int): BoundList<_> =
    if maxSize < 1 then
      invalidArg "maxSize" "maxSize must be greater than 0"
    { maxSize = maxSize; data = [] }
  member __.Insert(elem: _): BoundList<_> =
    let newData =
        List.truncate __.maxSize (elem :: List.rev __.data)
        |> List.rev
    {__ with data = newData}

  interface IBoundList<'a> with
    member __.MaxSize = __.maxSize
    
    member __.Insert(elem: _): IBoundList<_> =
      __.Insert(elem) :> IBoundList<_>
 
    member __.Count = List.length __.data

    member __.Get(index): _ =
      if index < 0 || index >= (__ :> IBoundList<_>).Count then
        raise <| IndexOutOfRangeException ()

      __.data.[index]

// Convenience functions for testing
module BoundList =
  let inline empty (maxSize: int) =
    BoundList<_>.Empty maxSize

  let inline insert (elem: _) (boundList: BoundList<_>): BoundList<_> =
    boundList.Insert(elem)


type MutableBoundList<'a>(maxSize: int) =
  do
    if not (maxSize > 0 && maxSize < System.Int32.MaxValue) then
      invalidArg "maxSize" "maxSize must be greater than 0 and less than Int32.MaxValue"

  let size = maxSize + 1
  let storage = Array.zeroCreate<'a> size
  let mutable head = 0
  let mutable tail = 0

  let inc(i) = (i + 1) % size
  let isFull () = inc(head) = tail
    
  interface IBoundList<'a> with
    member __.MaxSize = maxSize

    member __.Insert(elem: _): IBoundList<_> =
      if isFull () then
        tail <- inc tail
      Array.set storage head elem
      head <- inc head
      __ :> IBoundList<_>

    member __.Count = (size + head - tail) % size

    member __.Get(i: int): _ =
      let k = (i + tail) % size
      storage.[k]


