[<RequireQualifiedAccess>]
module GameApp.PlayScreen

open GameApp.Play.Ball
open GameApp.Play.Player
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type KeyboardEvent =
    | PlayerMoveLeft
    | PlayerMoveRight
    | PlayerShoot
    | TogglePause
    | ToggleFullScreen

type State =
    { Score: int
      Paused: bool
      Player: Player
      Balls: Ball list }

let initialState () =
    { Score = 0
      Paused = false
      Player = Player()
      Balls = [ Ball(vec2 100.0f 100.0f) ] }

let mutable private state = initialState ()

let init () =
    state <- initialState ()

let private events kb =
    [ if Keyboard.movingLeft kb then PlayerMoveLeft
      if Keyboard.movingRight kb then PlayerMoveRight
      if Keyboard.shooting kb then PlayerShoot
      if Keyboard.togglePause kb then TogglePause
      if Keyboard.toggleFullScreen kb then ToggleFullScreen ]

let update (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
    state.Player.Dispatch(StopMoving)

    for e in events kb do
        match e with
        | PlayerMoveLeft -> state.Player.Dispatch(MoveLeft)
        | PlayerMoveRight -> state.Player.Dispatch(MoveRight)
        | PlayerShoot -> state.Player.Dispatch(Shoot)
        | TogglePause -> state <- { state with Paused = not state.Paused }
        | ToggleFullScreen -> g.ToggleFullScreen()

    state.Player.Update t
    for ball in state.Balls do ball.Update t

let draw (sb: SpriteBatch) (t: GameTime) =
    sb.Draw(GameContent.textures.Backdrop, Vec2.zero, Color.White)

    state.Player.Draw sb
    for ball in state.Balls do ball.Draw sb t

    sb.Draw(GameContent.textures.Floor, vec2 0.0f (360.0f - 16.0f), Color.White)
