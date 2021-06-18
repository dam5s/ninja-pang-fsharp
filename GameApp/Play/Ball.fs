module GameApp.Play.Ball

open GameApp
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type Size =
    | Big
    | Medium
    | Small

type Config =
    { HitBoxSize: float32
      GraphicsSize: float32
      AccelerationY: float32
      MaxVelocityY: float32
      VelocityY: float32
      Score: int }

type State =
    { Size: Size
      Velocity: Vector2
      Position: Vector2 }

type Event =
    | None

type BallAnim =
    | Idle
    | Bounce

module private Conf =
    let gravity = 200.0f
    let screenWidth = 640.0f
    let screenHeight = 360.0f
    let floorHeight = 16.0f
    let floorY = screenHeight - floorHeight

    let private bigConfig =
        { HitBoxSize = 32.0f
          GraphicsSize = 36.0f
          AccelerationY = gravity
          MaxVelocityY = 250.0f
          VelocityY = 20.0f
          Score = 50 }

    let private mediumConfig =
        { HitBoxSize = 16.0f
          GraphicsSize = 20.0f
          AccelerationY = gravity * 1.5f
          MaxVelocityY = 250.0f
          VelocityY = 40.0f
          Score = 100 }

    let private smallConfig =
        { HitBoxSize = 8.0f
          GraphicsSize = 12.0f
          AccelerationY = gravity * 2.0f
          MaxVelocityY = 250.0f
          VelocityY = 60.0f
          Score = 200 }

    let get size =
        match size with
        | Big -> bigConfig
        | Medium -> mediumConfig
        | Small -> smallConfig

let private init size position () =
    let conf = Conf.get size

    { Size = size
      Position = position
      Velocity = vec2 conf.VelocityY (- Conf.gravity / 2.0f)  }

let private bounce conf time state =
    let falling = state.Velocity.Y > 0.0f
    let touchingFloor = state.Position.Y + conf.HitBoxSize >= Conf.floorY

    if falling && touchingFloor
    then { state with Velocity = Vec2.withY -Conf.gravity state.Velocity }, SetAnimation Bounce
    else state, NoEffect

let private touchWalls conf time state =
    let goingLeft = state.Velocity.X < 0.0f
    let goingRight = not goingLeft
    let touchingLeftWall = state.Position.X <= 0.0f
    let touchingRightWall = state.Position.X + conf.HitBoxSize >= Conf.screenWidth

    let newX =
        if goingLeft && touchingLeftWall
        then -state.Velocity.X
        else
            if goingRight && touchingRightWall
            then -state.Velocity.X
            else state.Velocity.X

    { state with Velocity = Vec2.withX newX state.Velocity }, NoEffect

let private updatePosition (conf: Config) (time: GameTime) (state: State) =
    let elapsedSeconds = float32 time.ElapsedGameTime.Milliseconds / 1_000.0f
    let effectiveAcceleration = conf.AccelerationY * elapsedSeconds
    let newVelocityY = min (state.Velocity.Y + effectiveAcceleration) conf.MaxVelocityY

    let newVelocity = Vec2.withY newVelocityY state.Velocity
    let newPosition = state.Position + VelocityPerSecond.vec2ForGameTime newVelocity time

    { state with Position = newPosition; Velocity = newVelocity }, NoEffect

open UpdateComposition.Operators

let private onUpdate conf =
    bounce conf >-> touchWalls conf >-> updatePosition conf

let private update event state =
    let conf = Conf.get state.Size

    match event with
    | Event None -> state, NoEffect
    | OnUpdate t -> onUpdate conf t state

let private draw state (animations: AnimationSet<BallAnim>) (sb: SpriteBatch) =
    animations.Draw (sb, state.Position)

let private anims size () =
    let texture =
        match size with
        | Big -> GameContent.textures.BigBall
        | Medium -> GameContent.textures.MediumBall
        | Small -> GameContent.textures.SmallBall

    AnimationSet(texture, 1, 3, Idle, Map(set [
        (Idle, Animation.loop [0] 1.0)
        (Bounce, Animation.interrupt [0; 1; 2] 24.0)
    ]))

type Ball (size: Size, p: Vector2) =
    inherit FunctionalComponent<State, Event, BallAnim>(init size p, update, draw, anims size)
