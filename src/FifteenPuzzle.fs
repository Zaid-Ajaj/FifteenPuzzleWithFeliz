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
type AppState = 
    { Tags: int [] } 
    member appState.FreePos =
        appState.Tags
        |> Array.findIndex ((=) FreeTag)
        |> indexToPosition
    member appState.Slots =
        appState.Tags
        |> Array.mapi (fun index tag ->
            indexToPosition index, string tag)

let random = Random()

let initialState() : AppState =
    //{ Tags = [|1..FreeTag|] } // Solved State for testing
    { Tags = Array.sortBy (fun _ -> random.NextDouble()) [|1 .. FreeTag|] }

let freePositionTag (state:AppState) =
    state.Slots
    |> Array.find (fun (pos, t) -> pos = state.FreePos)
    |> snd

let canMove (state: AppState) (position: Position)  =
    let { Row = x1; Col = y1 } = position
    let { Row = x2; Col = y2 } = state.FreePos
    let xDiff = abs (x2 - x1)
    let yDiff = abs (y2 - y1)
    xDiff + yDiff <= 1

let slotSelected (state:AppState) (position: Position) (tag: string) =
    let tag = int tag
    if canMove state position 
    then 
        let tags' = 
            state.Tags
            |> Array.map (fun t ->
                if t = FreeTag 
                then tag
                elif t = tag
                then FreeTag
                else t )
        { state with Tags = tags' }
    else state

let stylesheet = Stylesheet.load "./fitteen-puzzle.module.css"

let inFinalPosition (position: Position) (tag: string) =
    let { Row = x; Col = y } = position
    (x * 4) + y + 1 = int tag

let gameFinished (state: AppState) =
    List.forall id [
        for (position, tag) in state.Slots ->
            inFinalPosition position tag
    ]