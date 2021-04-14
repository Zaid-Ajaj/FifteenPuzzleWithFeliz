namespace App

open Feliz
open Feliz.Router

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
        let (gameStared, setGameStarted) = React.useState(false)
        let (appState, setAppState) = React.useStateWithUpdater(FifteenPuzzle.initialState())
        Html.div [
            prop.style [ style.textAlign.center ]
            prop.children [
                Html.h1 "Fifteen Puzzle"
                if not gameStared then
                    Html.button [
                        prop.text "Start game"
                        prop.onClick(fun _ -> setGameStarted(true))
                    ]
                else
                    // game
                    Html.div [
                        prop.style [
                            style.display.flex
                            style.flexWrap.wrap
                            style.padding 20
                        ]

                        prop.children [
                            for (position, tag) in appState.Slots do
                            Html.div [
                                prop.text (if position = appState.FreePos then "" else tag)
                                prop.onClick (fun _ -> setAppState(fun prevState -> FifteenPuzzle.slotSelected prevState position tag))
                                prop.style [
                                    style.width (length.perc 22)
                                    style.height 40
                                    style.backgroundColor.lightGreen
                                    style.margin 5
                                    style.paddingTop 10
                                    style.borderRadius 15
                                    style.cursor.pointer
                                    if position = appState.FreePos
                                    then style.backgroundColor.lightGray
                                ]
                            ]
                        ]
                    ]
            ]
        ]