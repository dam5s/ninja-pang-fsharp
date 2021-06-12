[<RequireQualifiedAccess>]
module GameApp.HighScoreScreen

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

let update (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
    ()

let draw (sb: SpriteBatch) (t: GameTime) =
    sb.DrawString(GameContent.fonts.MenuItemSelected, "High Score", Vector2.Zero, Color.WhiteSmoke)
