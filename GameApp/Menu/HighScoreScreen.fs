[<RequireQualifiedAccess>]
module GameApp.Menu.HighScoreScreen

open GameApp
open GameApp.Menu.Drawables
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

let update (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
    if Keyboard.wasReleased Keys.Escape kb
    then GameState.changeScreen GameState.MainMenu

let draw (sb: SpriteBatch) (t: GameTime) =
    let gameState = GameState.get()
    let highScore = System.String.Format("{0:#,0}", gameState.HighScore)

    sb |> MenuBackground.draw
       |> MenuHeader.draw "High Score"
       |> MenuItem.draw highScore false (Vector2 (32.0f, 120.0f))
       |> ignore
