module Main

open Feliz
open App
open Browser.Dom

ReactDOM.render(
    Components.FifteenPuzzle(),
    document.getElementById "feliz-app"
)