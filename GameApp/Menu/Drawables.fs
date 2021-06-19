module GameApp.Menu.Drawables

open GameApp
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

[<RequireQualifiedAccess>]
module MenuBackground =
    let draw (sb: SpriteBatch) =
        sb.Draw(GameContent.textures.Backdrop, Vec2.zero, Color.White)
        sb

[<RequireQualifiedAccess>]
module MenuHeader =
    let draw (text: string) (sb: SpriteBatch) =
        Text.center(text, GameContent.fonts.MenuHeader, sb, y = 40.0f)

[<RequireQualifiedAccess>]
module MenuFooter =
    let draw (sb: SpriteBatch) =
        sb.DrawString(GameContent.fonts.MenuFooter, "by Damien Le Berrigaud", vec2 400.0f 320.0f, Color.WhiteSmoke)
        sb

[<RequireQualifiedAccess>]
module MenuItem =
    let draw (text: string) selected y (sb: SpriteBatch) =
        let fonts = GameContent.fonts

        let font, color =
            if selected
            then fonts.MenuItemSelected, Color.WhiteSmoke
            else fonts.MenuItemNotSelected, Color(160, 151, 1)

        Text.center(text, font, sb, y, color)
