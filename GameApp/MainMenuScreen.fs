[<RequireQualifiedAccess>]
module GameApp.MainMenuScreen

open GameApp.MainMenuItem
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type private KeyboardEvent =
    | ToggleFullScreen
    | Up
    | Down
    | Enter

let private events (kb: Keyboard.State) =
    let altIsDown = Keyboard.oneIsDown [Keys.LeftAlt; Keys.RightAlt] kb
    let enterWasReleased = Keyboard.wasReleased Keys.Enter kb

    if altIsDown && enterWasReleased
    then [ToggleFullScreen]
    else

    if enterWasReleased
    then [Enter]
    else

    if Keyboard.wasReleased Keys.Up kb
    then [Up]
    else

    if Keyboard.wasReleased Keys.Down kb
    then [Down]
    else

    []

let private up () =
    MainMenuItem.selected <-
        match MainMenuItem.selected with
        | Play -> Exit
        | HighScore -> Play
        | Exit -> HighScore

let private down () =
    MainMenuItem.selected <-
        match MainMenuItem.selected with
        | Play -> HighScore
        | HighScore -> Exit
        | Exit -> Play

let update (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
    for e in events kb do
        match e with
        | ToggleFullScreen -> g.ToggleFullScreen ()
        | Up -> up ()
        | Down -> down ()
        | Enter ->
            match MainMenuItem.selected with
            | Play -> GameScreen.set GameScreen.Play
            | HighScore -> GameScreen.set GameScreen.HighScore
            | Exit -> System.Environment.Exit 0

let draw (sb: SpriteBatch) (t: GameTime) =
    let x = 32.0f
    let mutable y = 120.0f

    sb.Draw(GameContent.textures.MiniNinjaBg, Vector2.Zero, Color.White)
    sb.DrawString(GameContent.fonts.MenuHeader, "Ninja Pang", Vector2(x, 40.0f), Color.WhiteSmoke)
    sb.DrawString(GameContent.fonts.MenuFooter, "by Damien Le Berrigaud", Vector2(400.0f, 320.0f), Color.WhiteSmoke)

    for e in [Play; HighScore; Exit] do
        MainMenuItem.draw e (Vector2(x, y)) sb
        y <- y + 40.0f
