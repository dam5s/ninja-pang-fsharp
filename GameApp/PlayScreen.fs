[<RequireQualifiedAccess>]
module GameApp.PlayScreen

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

let update (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
    ()

let draw (sb: SpriteBatch) (t: GameTime) =
    sb.DrawString(GameContent.fonts.MenuItemSelected, "Play", Vector2.Zero, Color.WhiteSmoke)
