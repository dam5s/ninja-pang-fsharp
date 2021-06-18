module GameApp.Play.Player

open GameApp
open Microsoft.Xna.Framework

type PlayerAnim =
    | Idle
    | Shooting
    | RunRight
    | RunLeft

type State =
    { Position: Vector2
      VelocityX: float32 }

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
    { Position = vec2 100.0f (Conf.screenHeight - Conf.floorHeight - Conf.playerHeight)
      VelocityX = 0.0f }

let private onUpdate (time: GameTime) state =
    let newX =
        state.Position.X + VelocityPerSecond.forGameTime state.VelocityX time
        |> max Conf.minPosition
        |> min Conf.maxPosition
    { state with Position = Vec2.withX newX state.Position }, NoEffect

let private update event state: State * Effect<PlayerAnim> =
    match event with
    | Event MoveLeft -> { state with VelocityX = -Conf.horizontalVelocity }, SetAnimation RunLeft
    | Event MoveRight -> { state with VelocityX = Conf.horizontalVelocity }, SetAnimation RunRight
    | Event StopMoving -> { state with VelocityX = 0.0f }, SetAnimation Idle
    | Event Shoot -> state, SetAnimation Shooting
    | OnUpdate time -> onUpdate time state

let private animations () = AnimationSet(GameContent.textures.Ninja, 1, 13, Idle, Map(seq [
    (Idle, Animation.loop [0; 1; 2; 3] 2.0)
    (Shooting, Animation.interrupt [4] 1.0)
    (RunRight, Animation.loop [5; 6; 7; 8; 9; 10; 11; 12] 14.0)
    (RunLeft, Animation.mirroredLoop [5; 6; 7; 8; 9; 10; 11; 12] 14.0)
]))

let private draw state (animations: AnimationSet<PlayerAnim>) sb =
    animations.Draw (sb, state.Position)

type Player () =
    inherit FunctionalComponent<State, Event, PlayerAnim>(init, update, draw, animations)
