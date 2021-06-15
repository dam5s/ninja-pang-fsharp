module GameApp.Play.Projectile

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type Projectile (p: Vector2) =

    let mutable position = p

    member this.Update (t: GameTime) =
        ()

    member this.Draw (sb: SpriteBatch) (t: GameTime) =
        ()
