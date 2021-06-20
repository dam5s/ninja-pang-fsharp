[<RequireQualifiedAccess>]
module GameApp.Menu.MainMenuScreen

open GameApp
open GameApp.Menu.Drawables
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type private KeyboardEvent =
    | ToggleFullScreen
    | Up
    | Down
    | Enter

let private event (kb: Keyboard.State) =
    if Keyboard.toggleFullScreen kb
    then Some ToggleFullScreen
    else

    if Keyboard.menuEnter kb
    then Some Enter
    else

    if Keyboard.menuUp kb
    then Some Up
    else

    if Keyboard.menuDown kb
    then Some Down
    else

    None

type Item =
    | Play
    | HighScore
    | Exit

let mutable selectedItem =
    Play

let private up () =
    selectedItem <-
        match selectedItem with
        | Play -> Exit
        | HighScore -> Play
        | Exit -> HighScore

let private down () =
    selectedItem <-
        match selectedItem with
        | Play -> HighScore
        | HighScore -> Exit
        | Exit -> Play

let update (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
    match event kb with
    | Some ToggleFullScreen ->
        g.ToggleFullScreen ()
    | Some Up ->
        Effect.play GameContent.sounds.Shoot
        up ()
    | Some Down ->
        Effect.play GameContent.sounds.Shoot
        down ()
    | Some Enter ->
        Effect.play GameContent.sounds.Pop
        match selectedItem with
        | Play -> GameState.changeScreen GameState.Play
        | HighScore -> GameState.changeScreen GameState.HighScore
        | Exit -> exit 0
    | None ->
        ()

let private menuItems =
    [ ("Play", Play)
      ("HighScore", HighScore)
      ("Exit", Exit) ]

let draw (sb: SpriteBatch) (t: GameTime) =
    sb |> MenuBackground.draw
       |> MenuHeader.draw "Ninja Pang"
       |> MenuFooter.draw
       |> MenuItem.drawAll menuItems selectedItem
       |> ignore
