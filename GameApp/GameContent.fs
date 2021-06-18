[<RequireQualifiedAccess>]
module GameApp.GameContent

open Microsoft.Xna.Framework.Audio
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics

type Sounds (c: ContentManager) =
    member this.GameOver = c.Load<SoundEffect> "Sounds/game-over"
    member this.Hit = c.Load<SoundEffect> "Sounds/hit"
    member this.Pop = c.Load<SoundEffect> "Sounds/pop"
    member this.Shoot = c.Load<SoundEffect> "Sounds/shoot"

type Fonts (c: ContentManager) =
    member this.MenuItemNotSelected = c.Load<SpriteFont> "Fonts/MenuItemNotSelected"
    member this.MenuItemSelected = c.Load<SpriteFont> "Fonts/MenuItemSelected"
    member this.MenuFooter = c.Load<SpriteFont> "Fonts/MenuFooter"
    member this.MenuHeader = c.Load<SpriteFont> "Fonts/MenuHeader"

type Textures (c: ContentManager) =
    member this.Backdrop = c.Load<Texture2D> "Images/mini-ninja-bg"
    member this.Ninja = c.Load<Texture2D> "Images/mini-ninja"
    member this.Floor = c.Load<Texture2D> "Images/mini-ninja-floor"
    member this.Grappling = c.Load<Texture2D> "Images/grappling"
    member this.BigBall = c.Load<Texture2D> "Images/big-ball"
    member this.MediumBall = c.Load<Texture2D> "Images/medium-ball"
    member this.SmallBall = c.Load<Texture2D> "Images/small-ball"

let mutable fonts = Unchecked.defaultof<Fonts>
let mutable textures = Unchecked.defaultof<Textures>
let mutable sounds = Unchecked.defaultof<Sounds>

let load (c: ContentManager) =
    fonts <- Fonts(c)
    textures <- Textures(c)
    sounds <- Sounds(c)
