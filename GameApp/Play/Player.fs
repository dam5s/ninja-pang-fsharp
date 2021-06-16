module GameApp.Play.Player

open GameApp
open GameApp.Play.Projectile
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

module private Conf =
    let horizontalVelocity = 150.0f
    let screenHeight = 360.0f
    let screenWidth = 640.0f
    let floorHeight = 16.0f
    let playerWidth = 32.0f
    let playerHeight = 32.0f
    let positionOverflow = 8.0f
    let minPosition = - positionOverflow
    let maxPosition = screenWidth - playerWidth + positionOverflow
    let shootingDelayInMs = 200

type private PlayerAnim =
    | Idle
    | Shooting
    | RunRight
    | RunLeft

type private State =
    { Projectiles: Projectile list
      Position: Vector2
      VelocityX: float32
      HasNotShotFor: int }

let private init () =
    { Projectiles = []
      Position = vec2 100.0f (Conf.screenHeight - Conf.floorHeight - Conf.playerHeight)
      VelocityX = 0.0f
      HasNotShotFor = Conf.shootingDelayInMs + 1 }

type Action =
    | MoveLeft
    | MoveRight
    | StopMoving
    | Shoot
    | UpdateHasNotShotFor of GameTime
    | UpdatePosition of GameTime

let private shoot state =
    let hasNotReachedMax = List.length state.Projectiles < 2

    if hasNotReachedMax && state.HasNotShotFor > Conf.shootingDelayInMs
    then
        let x = state.Position.X + Conf.playerWidth / 2.0f
        let y = state.Position.Y + Conf.playerHeight

        { state with HasNotShotFor = 0
                     Projectiles = Projectile (vec2 x y) :: state.Projectiles },
        PlaySound GameContent.sounds.Shoot
    else
        state, NoEffect

let private updateHasNotShotFor state (time: GameTime) =
    let elapsed = time.ElapsedGameTime.Milliseconds
    let newState = { state with HasNotShotFor = state.HasNotShotFor + elapsed }

    if newState.HasNotShotFor <= 100
    then newState, SetAnimation Shooting
    else newState, NoEffect

let private updatePosition state (time: GameTime) =
    let newX =
        state.Position.X + VelocityPerSecond.forGameTime state.VelocityX time
        |> max Conf.minPosition
        |> min Conf.maxPosition
    { state with Position = Vec2.withX newX state.Position }, NoEffect

let private update action state: State * Effect<PlayerAnim> =
    match action with
    | MoveLeft -> { state with VelocityX = -Conf.horizontalVelocity }, SetAnimation RunLeft
    | MoveRight -> { state with VelocityX = Conf.horizontalVelocity }, SetAnimation RunRight
    | StopMoving -> { state with VelocityX = 0.0f }, SetAnimation Idle
    | Shoot -> shoot state
    | UpdateHasNotShotFor time -> updateHasNotShotFor state time
    | UpdatePosition time -> updatePosition state time

type Player() =

    let animations = AnimationSet(GameContent.textures.Ninja, 1, 13, Idle, Map(seq [
        (Idle, Animation.loop [0; 1; 2; 3] 2.0)
        (Shooting, Animation.interrupt [4] 1.0)
        (RunRight, Animation.loop [5; 6; 7; 8; 9; 10; 11; 12] 14.0)
        (RunLeft, Animation.mirroredLoop [5; 6; 7; 8; 9; 10; 11; 12] 14.0)
    ]))

    let mutable state = init ()

    member this.Dispatch action =
        let newState, effect = update action state
        state <- newState
        Effect.execute animations effect

    member this.Update (t: GameTime) =
        this.Dispatch (UpdateHasNotShotFor t)
        this.Dispatch (UpdatePosition t)

        let updatedProjectiles = state.Projectiles |> List.filter (fun p -> p.Update(t); p.Alive())
        state <- { state with Projectiles = updatedProjectiles }
        animations.Update t

    member this.Draw (sb: SpriteBatch) =
        for p in state.Projectiles do (p.Draw sb)
        animations.Draw (sb, state.Position)
