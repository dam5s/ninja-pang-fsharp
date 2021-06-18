[<RequireQualifiedAccess>]
module GameApp.Play.HUD

open GameApp
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

module private Conf =
    let screenHeight = 360.0f
    let screenWidth = 640.0f

let draw (state: PlayState.State) (sb: SpriteBatch) =
    let score = System.String.Format("{0:#,0}", state.Score)
    let font = GameContent.fonts.Score
    let scoreSize = font.MeasureString(score)
    let x = (Conf.screenWidth - scoreSize.X) / 2.0f

    sb.DrawString(font, score, vec2 x 20.0f, Color.WhiteSmoke)

let drawPausedOverlay (sb: SpriteBatch) =
    let width = int Conf.screenWidth
    let height = int Conf.screenHeight
    let color = Color(Color.Black, 0.5f)
    let data = Array.create (width * height) color

    let overlay = new Texture2D(sb.GraphicsDevice, width, height)
    overlay.SetData(data)

    sb.Draw(overlay, Vec2.zero, Color.White)
