module FifteenPuzzle

open System

type Position = { X: int; Y: int }

type Slot = Position * string

type AppState = { Slots : Slot list;  FreePos : Position }

type Msg =
| StartNewGame
| SelectSlot of Slot

let random = Random()

let initialState() : AppState =
    let randomTags = List.sortBy (fun _ -> random.Next()) [1 .. 16]
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

let slotSelected (state:AppState) (position: Position) (tag: string) =
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
                    else slotPosition, slotTag) }