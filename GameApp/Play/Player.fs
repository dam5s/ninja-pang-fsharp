module GameApp.Play.Player

open GameApp
open GameApp.Play.Projectile
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

let private horizontalVelocity = 150.0f
let private screenHeight = 360.0f
let private screenWidth = 640.0f
let private floorHeight = 16.0f
let private playerWidth = 32.0f
let private playerHeight = 32.0f

let private positionOverflow = 8.0f
let private minPosition = - positionOverflow
let private maxPosition = screenWidth - playerWidth + positionOverflow

type private ms = int

type private PlayerAnim =
    | Idle
    | Shoot
    | RunRight
    | RunLeft

type Player() =

    let shootingDelay: ms = 200

    let animations = AnimationSet(GameContent.textures.Ninja, 1, 13, Idle, Map(seq [
        (Idle, Animation.loop [0; 1; 2; 3] 2.0)
        (Shoot, Animation.interrupt [4] 1.0)
        (RunRight, Animation.loop [5; 6; 7; 8; 9; 10; 11; 12] 14.0)
        (RunLeft, Animation.mirroredLoop [5; 6; 7; 8; 9; 10; 11; 12] 14.0)
    ]))

    let mutable projectiles: Projectile list = []
    let mutable position = vec2 100.0f (screenHeight - floorHeight - playerHeight)
    let mutable velocityX = 0.0f

    let mutable hasNotShotFor: ms = shootingDelay + 1

    let newProjectile () =
        let x = position.X + playerWidth / 2.0f
        let y = position.Y + playerHeight
        Projectile (Vector2 (x, y))

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
        let hasNotReachedMax = List.length projectiles < 2

        if hasNotReachedMax && hasNotShotFor > shootingDelay
        then
            GameContent.sounds.Shoot()
            hasNotShotFor <- 0
            projectiles <- newProjectile() :: projectiles

    member this.Update (t: GameTime) =
        hasNotShotFor <- hasNotShotFor + t.ElapsedGameTime.Milliseconds

        if hasNotShotFor <= 100
        then animations.Play Shoot

        let newX =
            position.X + VelocityPerSecond.forGameTime velocityX t
            |> max minPosition
            |> min maxPosition

        position.X <- newX
        projectiles <- projectiles |> List.filter (fun p -> p.Update(t); p.Alive())
        animations.Update t

    member this.Draw (sb: SpriteBatch) =
        for p in projectiles do (p.Draw sb)
        animations.Draw (sb, position)
