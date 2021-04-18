module FifteenPuzzle

open System

type Position = { Row: int; Col: int }

type Slot = Position * string

let [<Literal>] RowCount = 4
let [<Literal>] RowLength = 4
let FreeTag = RowCount*RowLength

let indexToPosition (index: int) =
    { Row = index / RowLength; Col = index % RowLength }

let positionToIndex (position: Position) =
    position.Row*RowLength + position.Col

//type AppState = { Slots : Slot list;  FreePos : Position }
type AppState = int []

let solvedState = [| 1..FreeTag |]

let random = Random()

let initialState() : AppState =
    Array.sortBy (fun _ -> random.NextDouble()) [|1 .. FreeTag|]

let canMove (state: AppState) (index: int)  =
    let { Row = x1; Col = y1 } = indexToPosition index
    let { Row = x2; Col = y2 } = indexToPosition (state |> Array.findIndex ((=) FreeTag))
    let xDiff = abs (x2 - x1)
    let yDiff = abs (y2 - y1)
    xDiff + yDiff <= 1

let slotSelected (state:AppState) (index: int) =
    if canMove state index
    then 
        let tag = state.[index]
        [| for t in state do
            if t = FreeTag then tag
            elif t = tag then FreeTag
            else t |]
    else state

let stylesheet = Stylesheet.load "./fitteen-puzzle.module.css"

let inFinalPosition index tag =
    index + 1 = tag

let gameFinished (state: AppState) =
    state = solvedState