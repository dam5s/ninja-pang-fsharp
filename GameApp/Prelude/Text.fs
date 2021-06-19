[<AutoOpen>]
module Text

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

let inline private centeredX (textSize: Vector2) =
    (Conf.Screen.width - textSize.X) / 2.0f

let inline private centeredY (textSize: Vector2) =
    (Conf.Screen.height - textSize.Y) / 2.0f

type Text private() =

    static member center (text: string, font: SpriteFont, sb: SpriteBatch, ?y: float32, ?color: Color) =
        let textSize = font.MeasureString text
        let x = centeredX textSize

        let resolvedY = y |> Option.defaultValue (centeredY textSize)
        let resolvedColor = color |> Option.defaultValue Color.WhiteSmoke

        sb.DrawString(font, text, vec2 x resolvedY, resolvedColor)
        sb
