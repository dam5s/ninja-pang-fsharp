[<RequireQualifiedAccess>]
module GameApp.Play.HUD

open GameApp
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

module EnergyBar =

    let private width = 100
    let private height = 16
    let private fromX = 16
    let private fromY = 16
    let private border = 1
    let private toX = fromX + width - border
    let private toY = fromY + height - border
    let private borderColor = Color.WhiteSmoke
    let private fillColor = Color(122, 38, 168)

    let private drawBorder (sb: SpriteBatch) =
        sb
        |> Draw.rectangle fromX (fromY - border) width border borderColor
        |> Draw.rectangle fromX (toY + border) width border borderColor
        |> Draw.rectangle (fromX - border) fromY border height borderColor
        |> Draw.rectangle (toX + border) fromY border height borderColor

    let private fillBar energy (sb: SpriteBatch) =
        if energy > 0
        then Draw.rectangle fromX fromY energy height fillColor sb
        else sb

    let draw energy (sb: SpriteBatch) =
        sb
        |> drawBorder
        |> fillBar energy

let draw (state: PlayState.State) (sb: SpriteBatch) =
    let font = GameContent.fonts.Score
    let topLineY = 14.0f

    let score = System.String.Format ("{0:#,0}", state.Score)

    Text(sb).Center(score, font, y = topLineY).Done
    |> EnergyBar.draw state.Energy

let inline private drawOverlay (sb: SpriteBatch) =
    let color = Color(Color.Black, 0.5f)
    let width = int Conf.Screen.width
    let height = int Conf.Screen.height

    Draw.rectangle 0 0 width height color sb |> ignore

let drawPaused (state: PlayState.State) (sb: SpriteBatch) =
    if state.Paused
    then
        drawOverlay sb
        Text(sb).Center("Game Paused", GameContent.fonts.MenuHeader).Done
    else
        sb

let drawGameOver (state: PlayState.State) (sb: SpriteBatch) =
    if state.Energy <= 0
    then
        drawOverlay sb
        Text(sb).Center("Game Over", GameContent.fonts.MenuHeader).Done
    else
        sb
