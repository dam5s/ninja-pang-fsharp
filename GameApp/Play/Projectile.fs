module GameApp.Play.Projectile

open GameApp
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type Projectile (p: Vector2) =

    let velocityY = -300.0f
    let mutable position = p

    member this.Alive () =
        position.Y > 0.0f

    member this.Update (t: GameTime) =
        position.Y <- position.Y + VelocityPerSecond.forGameTime velocityY t

    member this.Draw (sb: SpriteBatch) =
        sb.Draw(GameContent.textures.Grappling, position, Color.White)
