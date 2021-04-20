namespace App

open Feliz
open Feliz.Router
open System
open Fable.Core.JS
open Feliz.UseListener

type Components =
    /// <summary>
    /// The simplest possible React component.
    /// Shows a header with the text Hello World
    /// </summary>
    [<ReactComponent>]
    static member HelloWorld() =
        Html.div [
            Html.h1 [
                prop.text "Hello from Feliz"
                prop.style [ style.color.red ]
            ]
            Html.p "I hope we can make this thing work!"
        ]

    [<ReactComponent>]
    static member MoreElements() =
        Html.div [
            Components.HelloWorld()
            Components.HelloWorld()
        ]

    /// <summary>
    /// A stateful React component that maintains a counter
    /// </summary>
    [<ReactComponent>]
    static member Counter() =
        let (count, setCount) = React.useState(0)
        Html.div [
            Html.h1 [
                prop.text count
                prop.style [ style.color.aquaMarine ]
            ]
            Html.button [
                prop.onClick (fun _ -> setCount(count + 1))
                prop.text "Increment"
            ]
        ]

    /// <summary>
    /// A React component that uses Feliz.Router
    /// to determine what to show based on the current URL
    /// </summary>
    [<ReactComponent>]
    static member Router() =
        let (currentUrl, updateUrl) = React.useState(Router.currentUrl())
        React.router [
            router.onUrlChanged updateUrl
            router.children [
                match currentUrl with
                | [ ] -> Html.h1 "Index"
                | [ "hello" ] -> Components.HelloWorld()
                | [ "counter" ] -> Components.Counter()
                | otherwise -> Html.h1 "Not found"
            ]
        ]

    [<ReactComponent>]
    static member FifteenPuzzle() =
        let (gameStarted, setGameStarted) = React.useState(false)
        let (timeElapsed, setTimeElapsed) = React.useStateWithUpdater(0)
        let (appState, setAppState) = React.useStateWithUpdater(FifteenPuzzle.initialState())
        // use a computed value
        let gameFinished = React.useMemo((fun () -> FifteenPuzzle.gameFinished appState), [| appState |])

        React.useWindowListener.onKeyUp(fun (ev: Browser.Types.KeyboardEvent) ->
            if (ev.ctrlKey && ev.key = "q")
            then setAppState(fun _ -> FifteenPuzzle.finishedState())
        )

        let subscribeToTimer() =
            // tick when game is on-going
            let timerId = setInterval (fun _ ->
                if gameStarted && not gameFinished
                then setTimeElapsed (fun prevTimeElapsed -> prevTimeElapsed + 1)) 1000

            // return IDisposable with cleanup code
            { new IDisposable with
                member this.Dispose() =
                    clearTimeout(timerId) }

        React.useEffect(subscribeToTimer, [| box gameStarted; box gameFinished |])

        let playAgain() =
            setAppState(fun _ -> FifteenPuzzle.initialState())
            setTimeElapsed (fun _ -> 0)

        let stylesheet = FifteenPuzzle.stylesheet
        Html.div [
            prop.style [ style.textAlign.center ]
            prop.children [
                Html.h1 "Fifteen Puzzle"
                if not gameStarted then
                    Html.button [
                        prop.text "Start game"
                        prop.onClick(fun _ -> setGameStarted(true))
                    ]
                else
                    Html.h3 $"Time elapsed {timeElapsed} second(s)"
                    // game
                    Html.div [
                        prop.className stylesheet.["slot-container"]
                        prop.children [
                            for (position, tag) in appState.Slots do
                            Html.div [
                                prop.text (if position = appState.FreePos then "" else tag)
                                prop.onClick (fun _ ->
                                    setAppState(fun prevState ->
                                        if FifteenPuzzle.canMove prevState position
                                        then FifteenPuzzle.slotSelected prevState position tag
                                        else prevState
                                    )
                                )
                                prop.className [
                                    if position = appState.FreePos
                                    then stylesheet.["free-slot"]
                                    else if FifteenPuzzle.inFinalPosition position tag
                                    then stylesheet.["final-slot"]
                                    else  stylesheet.["slot"]
                                ]
                            ]
                        ]
                    ]

                    if gameFinished then
                        Html.p "YOU WIN!"
                        Html.button [
                            prop.text "Play again"
                            prop.onClick (fun _ -> playAgain())
                        ]
            ]
        ]