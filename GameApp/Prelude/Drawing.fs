[<AutoOpen>]
module Drawing

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

[<RequireQualifiedAccess>]
module Draw =
    let rectangle x y w h (color: Color) (sb: SpriteBatch) =
        let data = Array.create (w * h) color

        let texture = new Texture2D(sb.GraphicsDevice, w, h)
        texture.SetData(data)
        sb.Draw(texture, vec2 (float32 x) (float32 y), Color.White)
        sb
