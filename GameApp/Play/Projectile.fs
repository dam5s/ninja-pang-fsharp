module GameApp.Play.Projectile

open GameApp
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

let private velocity = -300.0f

let inline private boxOrigin (position: Vector2) =
    Vec2.withX (position.X + 2.0f) position

type State =
    { Position: Vector2
      Box: Collision.Box }

type Projectile (p: Vector2) =
    inherit KillableComponent()

    let mutable state =
        { Position = p
          Box = { Origin = boxOrigin p; Width = 2.0f; Height = 360.0f } }

    member this.GetState () =
        state

    member this.Update (t: GameTime) =
        let newY = state.Position.Y + VelocityPerSecond.forGameTime velocity t
        let newPosition = Vec2.withY newY state.Position
        let newBox = { state.Box with Origin = boxOrigin newPosition }

        state <- { Position = newPosition; Box = newBox }

    member this.Draw (sb: SpriteBatch) =
        sb.Draw(GameContent.textures.Grappling, state.Position, Color.White)
