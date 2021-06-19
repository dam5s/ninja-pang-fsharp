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
        Text(sb)
            .Center(text, GameContent.fonts.MenuHeader, y = 40.0f)
            .Done

[<RequireQualifiedAccess>]
module MenuFooter =
    let draw (sb: SpriteBatch) =
        let text = "by Damien Le Berrigaud"
        let font = GameContent.fonts.MenuFooter

        Text(sb)
            .BottomRight(text, font, margin = 16.0f)
            .Done

[<RequireQualifiedAccess>]
module MenuItem =
    let draw (text: string) selected y (sb: SpriteBatch) =
        let fonts = GameContent.fonts

        let resolvedY, font, color =
            if selected
            then y - 5.0f, fonts.MenuItemSelected, Color.WhiteSmoke
            else y, fonts.MenuItemNotSelected, Color(160, 151, 1)

        Text(sb)
            .Center(text, font, resolvedY, color)
            .Done
