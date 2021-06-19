module GameApp.Play.Ball

open GameApp
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type Size =
    | Big
    | Medium
    | Small

type Direction =
    | Left
    | Right

type Config =
    { GraphicsSize: float32
      AccelerationY: float32
      MaxVelocityY: float32
      VelocityX: float32 }

type State =
    { Size: Size
      Velocity: Vector2
      Position: Vector2
      Box: Collision.Box }

type Event =
    | None

type BallAnim =
    | Idle
    | Bounce

module private Conf =
    let gravity = 200.0f

    let private bigConfig =
        { GraphicsSize = 36.0f
          AccelerationY = gravity
          MaxVelocityY = 250.0f
          VelocityX = 20.0f }

    let private mediumConfig =
        { GraphicsSize = 20.0f
          AccelerationY = gravity * 1.5f
          MaxVelocityY = 250.0f
          VelocityX = 40.0f }

    let private smallConfig =
        { GraphicsSize = 12.0f
          AccelerationY = gravity * 2.0f
          MaxVelocityY = 250.0f
          VelocityX = 60.0f }

    let get size =
        match size with
        | Big -> bigConfig
        | Medium -> mediumConfig
        | Small -> smallConfig

let inline private boxOrigin position =
    Vec2.offset 2.0f position

let private init size dir position () =
    let conf = Conf.get size
    let boxSize = conf.GraphicsSize - 4.0f
    let box: Collision.Box = { Origin = boxOrigin position; Width = boxSize; Height = boxSize }
    let velocityX = match dir with | Left -> - conf.VelocityX | Right -> conf.VelocityX

    { Size = size
      Position = position
      Velocity = vec2 velocityX (- Conf.gravity / 2.0f)
      Box = box }

let private bounce conf time state =
    let falling = state.Velocity.Y > 0.0f
    let touchingFloor = state.Box.Origin.Y + state.Box.Height >= Conf.Floor.y

    if falling && touchingFloor
    then { state with Velocity = Vec2.withY -Conf.gravity state.Velocity }, SetAnimation Bounce
    else state, NoEffect

let private touchWalls conf time state =
    let goingLeft = state.Velocity.X < 0.0f
    let goingRight = not goingLeft
    let touchingLeftWall = state.Box.Origin.X <= 0.0f
    let touchingRightWall = state.Box.Origin.X + state.Box.Width >= Conf.Screen.width

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
    let newBox = { state.Box with Origin = boxOrigin newPosition }

    { state with Position = newPosition; Velocity = newVelocity; Box = newBox }, NoEffect

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

type Ball (size: Size, dir: Direction, p: Vector2) =
    inherit FunctionalComponent<State, Event, BallAnim>(init size dir p, update, draw, anims size)
