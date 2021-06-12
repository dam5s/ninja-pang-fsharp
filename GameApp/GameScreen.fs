[<RequireQualifiedAccess>]
module GameApp.GameScreen

type Screen =
    | MainMenu
    | Play
    | HighScore

let mutable private current = MainMenu

let get () = current

let set newScreen =
    current <- newScreen
