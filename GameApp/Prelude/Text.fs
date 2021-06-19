[<AutoOpen>]
module Text

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

let inline private centeredX (textSize: Vector2) =
    (Conf.Screen.width - textSize.X) / 2.0f

let inline private centeredY (textSize: Vector2) =
    (Conf.Screen.height - textSize.Y) / 2.0f

type Text(sb: SpriteBatch) =

    member this.Default (text: string, font: SpriteFont, ?x: float32, ?y: float32, ?color: Color) =
        let textSize = font.MeasureString text

        let resolvedX = defaultArg x 0.0f
        let resolvedY = defaultArg y (centeredY textSize)
        let resolvedColor = defaultArg color Color.WhiteSmoke

        sb.DrawString(font, text, vec2 resolvedX resolvedY, resolvedColor)
        this

    member this.Center (text: string, font: SpriteFont, ?y: float32, ?color: Color) =
        let textSize = font.MeasureString text

        let x = centeredX textSize
        let resolvedY = defaultArg y (centeredY textSize)
        let resolvedColor = defaultArg color Color.WhiteSmoke

        sb.DrawString(font, text, vec2 x resolvedY, resolvedColor)
        this

    member this.BottomRight (text: string, font: SpriteFont, ?margin: float32, ?color: Color) =
        let textSize = font.MeasureString text

        let resolvedMargin = defaultArg margin 0.0f
        let resolvedColor = defaultArg color Color.WhiteSmoke

        let x = Conf.Screen.width - textSize.X - resolvedMargin
        let y = Conf.Screen.height - textSize.Y - resolvedMargin

        sb.DrawString(font, text, vec2 x y, resolvedColor)
        this

    member this.Done =
        sb
