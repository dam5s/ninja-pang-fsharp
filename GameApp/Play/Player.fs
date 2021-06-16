module GameApp.Play.Player

open GameApp
open GameApp.Play.Projectile
open Microsoft.Xna.Framework

type PlayerAnim =
    | Idle
    | Shooting
    | RunRight
    | RunLeft

type State =
    { Projectiles: Projectile list
      Position: Vector2
      VelocityX: float32
      TimeSinceLastShot: int }

type Event =
    | MoveLeft
    | MoveRight
    | StopMoving
    | Shoot

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

let private init () =
    { Projectiles = []
      Position = vec2 100.0f (Conf.screenHeight - Conf.floorHeight - Conf.playerHeight)
      VelocityX = 0.0f
      TimeSinceLastShot = Conf.shootingDelayInMs + 1 }

let private shoot state =
    let hasNotReachedMax = List.length state.Projectiles < 2

    if hasNotReachedMax && state.TimeSinceLastShot > Conf.shootingDelayInMs
    then
        let x = state.Position.X + Conf.playerWidth / 2.0f
        let y = state.Position.Y + Conf.playerHeight

        { state with TimeSinceLastShot = 0
                     Projectiles = Projectile (vec2 x y) :: state.Projectiles },
        PlaySound GameContent.sounds.Shoot
    else
        state, NoEffect

let private updateTimeSinceLastShot (time: GameTime) state =
    let elapsed = time.ElapsedGameTime.Milliseconds
    let newState = { state with TimeSinceLastShot = state.TimeSinceLastShot + elapsed }

    if newState.TimeSinceLastShot <= 100
    then newState, SetAnimation Shooting
    else newState, NoEffect

let private updatePosition (time: GameTime) state =
    let newX =
        state.Position.X + VelocityPerSecond.forGameTime state.VelocityX time
        |> max Conf.minPosition
        |> min Conf.maxPosition
    { state with Position = Vec2.withX newX state.Position }, NoEffect

let private updateProjectiles (time: GameTime) state =
    let updatedProjectiles = state.Projectiles |> List.filter (fun p -> p.Update(time); p.Alive())
    { state with Projectiles = updatedProjectiles }, NoEffect

open UpdateComposition.Operators

let private onUpdate =
    updateTimeSinceLastShot
        >-> updatePosition
        >-> updateProjectiles

let private update event state: State * Effect<PlayerAnim> =
    match event with
    | Event MoveLeft -> { state with VelocityX = -Conf.horizontalVelocity }, SetAnimation RunLeft
    | Event MoveRight -> { state with VelocityX = Conf.horizontalVelocity }, SetAnimation RunRight
    | Event StopMoving -> { state with VelocityX = 0.0f }, SetAnimation Idle
    | Event Shoot -> shoot state
    | OnUpdate time -> onUpdate time state

let private  animations () = AnimationSet(GameContent.textures.Ninja, 1, 13, Idle, Map(seq [
    (Idle, Animation.loop [0; 1; 2; 3] 2.0)
    (Shooting, Animation.interrupt [4] 1.0)
    (RunRight, Animation.loop [5; 6; 7; 8; 9; 10; 11; 12] 14.0)
    (RunLeft, Animation.mirroredLoop [5; 6; 7; 8; 9; 10; 11; 12] 14.0)
]))

let private draw state (animations: AnimationSet<PlayerAnim>) sb =
    for p in state.Projectiles do (p.Draw sb)
    animations.Draw (sb, state.Position)

type Player () =
    inherit FunctionalComponent<State, Event, PlayerAnim>(init, update, draw, animations)
