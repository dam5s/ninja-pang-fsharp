module GameApp.Play.Player

open GameApp
open GameApp.Prelude.AnimProps
open GameApp.Prelude.Animation
open GameApp.Play.Projectile
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

let private horizontalVelocity = 2.0f
let private screenHeight = 360.0f
let private screenWidth = 640.0f
let private floorHeight = 16.0f
let private playerWidth = 32.0f
let private playerHeight = 32.0f

let private positionOverflow = 8.0f
let private minPosition = - positionOverflow
let private maxPosition = screenWidth - playerWidth + positionOverflow

type private PlayerAnim =
    | Idle
    | Shoot
    | RunRight
    | RunLeft

type Player() =

    let animations = AnimationSet(GameContent.textures.Ninja, 1, 13, Idle, Map(seq [
        (Idle, Animation.loop [0; 1; 2; 3] 2.0)
        (Shoot, Animation.interrupt [4] 1.0)
        (RunRight, Animation.loop [5; 6; 7; 8; 9; 10; 11; 12] 14.0)
        (RunLeft, Animation.mirroredLoop [5; 6; 7; 8; 9; 10; 11; 12] 14.0)
    ]))

    let mutable position = Vector2(100.0f, screenHeight - floorHeight - playerHeight)
    let mutable velocityX = 0.0f

    member this.MoveLeft () =
        velocityX <- -horizontalVelocity
        animations.Play RunLeft

    member this.MoveRight () =
        velocityX <- horizontalVelocity
        animations.Play RunRight

    member this.StopMoving () =
        velocityX <- 0.0f
        animations.Play Idle

    member this.Shoot () =
        animations.Play Shoot
        Projectile position

    member this.Update (t: GameTime) =
        let newX =
            position.X + velocityX
            |> max minPosition
            |> min maxPosition

        position.X <- newX
        animations.Update t

    member this.Draw (sb: SpriteBatch) =
        animations.Draw (sb, position)
