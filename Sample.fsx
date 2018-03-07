
open System

type IBoundList<'a> =
    abstract member MaxSize: int with get
    abstract member Insert: 'a -> IBoundList<'a>
    abstract member Count: int with get
    abstract member Get: int -> 'a


type BoundList<'a> =
  {
    maxSize: int
    data: 'a list
  }
with 
  member __.Empty(maxSize: int): BoundList<_> =
    if maxSize < 1 then
      invalidArg "maxSize" "maxSize must be greater than 0"
    { maxSize = 0; data = [] }

  interface IBoundList<'a> with
    member __.MaxSize = __.maxSize
    
    member __.Insert(elem: _): IBoundList<_> =
      let newData = List.truncate __.maxSize (elem :: __.data)
      {__ with data = newData} :> IBoundList<'a>
 
    member __.Count = List.length __.data

    member __.Get(index): _ =
      if index < 0 || index >= (__ :> IBoundList<_>).Count then
        raise <| IndexOutOfRangeException ()

      __.data.[index]

type MutableBoundList<'a>(maxSize: int) =
  do
    if maxSize < 1 then
      invalidArg "maxSize" "maxSize must be greater than 0"

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

