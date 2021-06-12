[<RequireQualifiedAccess>]
module GameApp.GameState

type Screen =
    | MainMenu
    | Play
    | HighScore

type GameState =
    { Screen: Screen
      HighScore: int }

let mutable private current =
    { Screen = MainMenu
      HighScore = 1_000 }

let get () = current

let changeScreen screen =
    current <- { current with Screen = screen }

let recordScore score =
    current <- { current with HighScore = max current.HighScore score }
