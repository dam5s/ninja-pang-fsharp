module GameApp.Play.Player

open GameApp
open GameApp.Prelude.AnimProps
open GameApp.Prelude.Animation
open GameApp.Play.Projectile
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

let private HORIZONTAL_VELOCITY = 1.0f
let private GRAVITY = 200.0f

type private PlayerAnim =
    | Idle
    | Shoot
    | RunRight
    | RunLeft

type Player() =

    let animations = AnimationSet(GameContent.textures.MiniNinja, 1, 13, Idle, Map(seq [
        (Idle, Animation.loop [0; 1; 2; 3] 2.0)
        (Shoot, Animation.interrupt [4] 12.0)
        (RunRight, Animation.loop [5; 6; 7; 8; 9; 10; 11; 12] 12.0)
        (RunLeft, Animation.mirroredLoop [5; 6; 7; 8; 9; 10; 11; 12] 12.0)
    ]))

    let mutable position = Vector2(100.0f, 300.0f)
    let mutable velocity = Vector2(0.0f, 0.0f)//, GRAVITY)

    member this.MoveLeft () =
        velocity.X <- -HORIZONTAL_VELOCITY
        animations.Play(RunLeft)

    member this.MoveRight () =
        velocity.X <- HORIZONTAL_VELOCITY
        animations.Play(RunRight)

    member this.StopMoving () =
        velocity.X <- 0.0f
        animations.Play(Idle)

    member this.Shoot () =
        animations.Play(Shoot)
        Projectile(position)

    member this.Update (t: GameTime) =
        position <- position + velocity
        animations.Update (t)

    member this.Draw (sb: SpriteBatch) =
        animations.Draw (sb, position)
        ()
