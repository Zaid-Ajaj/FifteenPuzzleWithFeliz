module FifteenPuzzle

open System

type Position = { X: int; Y: int }

type Slot = Position * string

type AppState = { Slots : Slot list;  FreePos : Position }

let random = Random()

let initialState() : AppState =
    let randomTags = List.sortBy (fun _ -> random.NextDouble()) [1 .. 16]
    // generate slot positions
    [
        for x in 0 .. 3 do
        for y in 0 .. 3 do
        yield { X = x; Y = y }  ]
    // give each position a random tag, making it a slot
    |> List.mapi (fun i pos -> pos, string (List.item i randomTags))
    |> fun slots ->
        // find the free slot, it has tag "16"
        let (pos, _) = Seq.find (fun (p, tag) -> tag = "16") slots
        // return initial state
        { Slots = slots; FreePos = pos }

let freePositionTag (state:AppState) =
    state.Slots
    |> List.find (fun (pos, t) -> pos = state.FreePos)
    |> snd

let slotSelected (state:AppState)  (position: Position) (tag: string) =
    { state with
        FreePos = position
        Slots =
            if position = state.FreePos
            then state.Slots
            else
                state.Slots
                |> List.map (fun (slotPosition, slotTag) ->
                    if slotPosition = state.FreePos
                    then slotPosition, tag
                    else if slotPosition = position
                    then
                        let oldFreePositionTag = freePositionTag state
                        slotPosition, oldFreePositionTag
                    else slotPosition, slotTag) }

let stylesheet = Stylesheet.load "./fitteen-puzzle.module.css"

let canMove (state:AppState) (position: Position)  =
    let { X = x1; Y = y1 } = position
    let { X = x2; Y = y2 } = state.FreePos
    let xDiff = abs (x2 - x1)
    let yDiff = abs (y2 - y1)
    xDiff + yDiff <= 1

let inFinalPosition (position: Position) (tag: string) =
    let { X = x; Y = y } = position
    (x * 4) + y + 1 = int tag

let gameFinished (state: AppState) =
    List.forall id [
        for (position, tag) in state.Slots ->
            inFinalPosition position tag
    ]