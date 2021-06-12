module GameApp.MainMenuItem

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type MainMenuItem =
    | Play
    | HighScore
    | Exit

[<RequireQualifiedAccess>]
module MainMenuItem =
    let mutable selected =
        Play

    let private text item =
        match item with
        | Play -> "Play"
        | HighScore -> "High Score"
        | Exit -> "Exit"

    let draw item position (sb: SpriteBatch) =
        let fonts = GameContent.fonts

        let font, color =
            if item = selected
            then fonts.MenuItemSelected, Color.WhiteSmoke
            else fonts.MenuItemNotSelected, Color(160, 151, 1)

        sb.DrawString(font, text item, position, color)
