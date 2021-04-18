module FifteenPuzzle

open System

type Position = { Row: int; Col: int }

type Slot = Position * string

let [<Literal>] RowCount = 4
let [<Literal>] RowLength = 4
let SlotCount = RowCount*RowLength
let FreeTag = SlotCount

type AppState = int list

let solvedState = [ 1..SlotCount ]

let random = Random()

let initialState() : AppState =
    List.sortBy (fun _ -> random.NextDouble()) [1 .. SlotCount]

let canMove (state: AppState) (index: int)  =
    let freeIndex = state |> List.findIndex ((=) FreeTag)
    let possibleMoves =
        [ // Left is only possible if not at the beginning of the row
          if index%RowLength > 0 
          then index-1
          // Right is only possible if not at the end of the row
          if index%RowLength <> 3 
          then index+1
          // Up is only possible if not in the first row
          if index - RowLength >= 0
          then index - RowLength
          // Down is only possible if not in the last row
          if index + RowLength <= SlotCount
          then index + RowLength ]
    List.contains freeIndex possibleMoves

let slotSelected (state:AppState) (index: int) =
    if canMove state index
    then 
        let tag = state.[index]
        [ for t in state do
            if t = FreeTag then tag
            elif t = tag then FreeTag
            else t ]
    else state

let stylesheet = Stylesheet.load "./fitteen-puzzle.module.css"

let inFinalPosition index tag =
    index + 1 = tag

let gameFinished (state: AppState) =
    state = solvedState