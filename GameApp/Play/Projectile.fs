module GameApp.Play.Projectile

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type Projectile (position: Vector2) =

    member this.Update (t: GameTime) =
        ()

    member this.Draw (sb: SpriteBatch) (t: GameTime) =
        ()
