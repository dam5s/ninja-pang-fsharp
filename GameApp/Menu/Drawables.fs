module GameApp.Menu.Drawables

open GameApp
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

[<RequireQualifiedAccess>]
module MenuBackground =
    let draw (sb: SpriteBatch) =
        sb.Draw(GameContent.textures.MiniNinjaBg, Vector2.Zero, Color.White)
        sb

[<RequireQualifiedAccess>]
module MenuHeader =
    let draw (text: string) (sb: SpriteBatch) =
        sb.DrawString(GameContent.fonts.MenuHeader, text, Vector2(32.0f, 40.0f), Color.WhiteSmoke)
        sb

[<RequireQualifiedAccess>]
module MenuFooter =
    let draw (sb: SpriteBatch) =
        sb.DrawString(GameContent.fonts.MenuFooter, "by Damien Le Berrigaud", Vector2(400.0f, 320.0f), Color.WhiteSmoke)
        sb

[<RequireQualifiedAccess>]
module MenuItem =
    let draw (text: string) selected position (sb: SpriteBatch) =
        let fonts = GameContent.fonts

        let font, color =
            if selected
            then fonts.MenuItemSelected, Color.WhiteSmoke
            else fonts.MenuItemNotSelected, Color(160, 151, 1)

        sb.DrawString(font, text, position, color)
        sb
