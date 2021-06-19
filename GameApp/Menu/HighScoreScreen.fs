[<RequireQualifiedAccess>]
module GameApp.Menu.HighScoreScreen

open GameApp
open GameApp.Menu.Drawables
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

let update (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
    if Keyboard.escape kb
    then GameState.changeScreen GameState.MainMenu

let draw (sb: SpriteBatch) (t: GameTime) =
    let gameState = GameState.get()
    let highScore = System.String.Format("{0:#,0}", gameState.HighScore)

    sb |> MenuBackground.draw
       |> MenuHeader.draw "High Score"
       |> MenuItem.draw highScore false 120.0f
       |> ignore
