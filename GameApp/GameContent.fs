[<RequireQualifiedAccess>]
module GameApp.GameContent

open Microsoft.Xna.Framework.Audio
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics

type Sounds (c: ContentManager) =
    let gameOver = c.Load<SoundEffect> "Sounds/game-over"
    let hit = c.Load<SoundEffect> "Sounds/hit"
    let pop = c.Load<SoundEffect> "Sounds/pop"
    let shoot = c.Load<SoundEffect> "Sounds/shoot"

    let play (sound: SoundEffect) = sound.Play() |> ignore

    member this.GameOver() = play gameOver
    member this.Hit() = play hit
    member this.Pop() = play pop
    member this.Shoot() = play shoot

type Fonts (c: ContentManager) =
    member this.MenuItemNotSelected = c.Load<SpriteFont> "Fonts/MenuItemNotSelected"
    member this.MenuItemSelected = c.Load<SpriteFont> "Fonts/MenuItemSelected"
    member this.MenuFooter = c.Load<SpriteFont> "Fonts/MenuFooter"
    member this.MenuHeader = c.Load<SpriteFont> "Fonts/MenuHeader"

type Textures (c: ContentManager) =
    member this.MiniNinjaBg = c.Load<Texture2D> "Images/mini-ninja-bg"
    member this.MiniNinja = c.Load<Texture2D> "Images/mini-ninja"

let mutable fonts = Unchecked.defaultof<Fonts>
let mutable textures = Unchecked.defaultof<Textures>
let mutable sounds = Unchecked.defaultof<Sounds>

let load (c: ContentManager) =
    fonts <- Fonts(c)
    textures <- Textures(c)
    sounds <- Sounds(c)
