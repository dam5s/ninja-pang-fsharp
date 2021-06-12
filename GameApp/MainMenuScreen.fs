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

let private event (kb: Keyboard.State) =
    let altIsDown = Keyboard.oneIsDown [Keys.LeftAlt; Keys.RightAlt] kb
    let enterWasReleased = Keyboard.wasReleased Keys.Enter kb

    if (altIsDown && enterWasReleased) || Keyboard.wasReleased Keys.F11 kb
    then Some ToggleFullScreen
    else

    if enterWasReleased || Keyboard.wasReleased Keys.Space kb
    then Some Enter
    else

    if Keyboard.wasReleased Keys.Up kb
    then Some Up
    else

    if Keyboard.wasReleased Keys.Down kb
    then Some Down
    else

    None

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
    match event kb with
    | Some ToggleFullScreen ->
        g.ToggleFullScreen ()
    | Some Up ->
        GameContent.sounds.Shoot()
        up ()
    | Some Down ->
        GameContent.sounds.Shoot()
        down ()
    | Some Enter ->
        GameContent.sounds.Pop()
        match MainMenuItem.selected with
        | Play -> GameScreen.set GameScreen.Play
        | HighScore -> GameScreen.set GameScreen.HighScore
        | Exit -> System.Environment.Exit 0
    | None ->
        ()

let draw (sb: SpriteBatch) (t: GameTime) =
    let x = 32.0f
    let mutable y = 120.0f

    sb.Draw(GameContent.textures.MiniNinjaBg, Vector2.Zero, Color.White)
    sb.DrawString(GameContent.fonts.MenuHeader, "Ninja Pang", Vector2(x, 40.0f), Color.WhiteSmoke)
    sb.DrawString(GameContent.fonts.MenuFooter, "by Damien Le Berrigaud", Vector2(400.0f, 320.0f), Color.WhiteSmoke)

    for e in [Play; HighScore; Exit] do
        MainMenuItem.draw e (Vector2(x, y)) sb
        y <- y + 40.0f
