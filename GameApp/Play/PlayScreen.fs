[<RequireQualifiedAccess>]
module GameApp.PlayScreen

open GameApp.Play.Ball
open GameApp.Play.Player
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

module private Conf =
    let screenHeight = 360.0f
    let screenWidth = 640.0f
    let floorHeight = 16.0f

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
      Balls = [ Ball (Big, vec2 100.0f 100.0f); Ball (Big, vec2 300.0f 100.0f); Ball (Big, vec2 500.0f 100.0f) ] }

let mutable private state = initialState ()

let init () =
    state <- initialState ()

let private events kb =
    [ if Keyboard.movingLeft kb then PlayerMoveLeft
      if Keyboard.movingRight kb then PlayerMoveRight
      if Keyboard.shooting kb then PlayerShoot
      if Keyboard.togglePause kb then TogglePause
      if Keyboard.toggleFullScreen kb then ToggleFullScreen ]

let private playingUpdate (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
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

let private pausedUpdate (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
    for e in events kb do
        match e with
        | TogglePause -> state <- { state with Paused = not state.Paused }
        | ToggleFullScreen -> g.ToggleFullScreen()
        | _ -> () // do nothing while paused

let update (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
    if state.Paused
    then pausedUpdate kb g t
    else playingUpdate kb g t

let draw (sb: SpriteBatch) (t: GameTime) =
    sb.Draw(GameContent.textures.Backdrop, Vec2.zero, Color.White)

    state.Player.Draw sb
    for ball in state.Balls do ball.Draw sb

    sb.Draw(GameContent.textures.Floor, vec2 0.0f (Conf.screenHeight - Conf.floorHeight), Color.White)

    if state.Paused
    then
        let width = int Conf.screenWidth
        let height = int Conf.screenHeight
        let color = Color(Color.Black, 0.5f)
        let data = Array.create (width * height) color
        let overlay = new Texture2D(sb.GraphicsDevice, width, height)
        overlay.SetData(data)

        sb.Draw(overlay, Vec2.zero, Color.White)
