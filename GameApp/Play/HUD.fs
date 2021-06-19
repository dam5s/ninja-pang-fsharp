[<RequireQualifiedAccess>]
module GameApp.Play.HUD

open GameApp
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

let draw (state: PlayState.State) (sb: SpriteBatch) =
    let font = GameContent.fonts.Score
    let topLineY = 20.0f

    let score = System.String.Format ("{0:#,0}", state.Score)
    let energy = System.String.Format ("{0:#,0}", state.Energy)

    Text(sb)
        .Center(score, font, y = topLineY)
        .Default(energy, font, x = 32.0f, y = topLineY)
        .Done

let drawPausedOverlay (state: PlayState.State) (sb: SpriteBatch) =
    if state.Paused
    then
        let width = int Conf.Screen.width
        let height = int Conf.Screen.height
        let color = Color(Color.Black, 0.5f)
        let data = Array.create (width * height) color

        let overlay = new Texture2D(sb.GraphicsDevice, width, height)
        overlay.SetData(data)
        sb.Draw(overlay, Vec2.zero, Color.White)

        let text = "Game Paused"
        let font = GameContent.fonts.MenuHeader

        Text(sb).Center(text, font).Done
    else
        sb
