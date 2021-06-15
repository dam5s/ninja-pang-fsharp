[<RequireQualifiedAccess>]
module GameApp.PlayScreen

open GameApp.Play.Ball
open GameApp.Play.Player
open GameApp.Play.Projectile
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type KeyboardEvent =
    | MoveLeft
    | MoveRight
    | Shoot
    | TogglePause
    | ToggleFullScreen

type State =
    { Score: int
      Paused: bool
      Player: Player
      Projectiles: Projectile list
      Balls: Ball list }

let initialState () =
    { Score = 0
      Paused = false
      Player = Player()
      Projectiles = []
      Balls = [] }

let mutable private state = initialState ()

let init () =
    state <- initialState ()

let private events kb =
    [ if Keyboard.movingLeft kb then MoveLeft
      if Keyboard.movingRight kb then MoveRight
      if Keyboard.shooting kb then Shoot
      if Keyboard.togglePause kb then TogglePause
      if Keyboard.toggleFullScreen kb then ToggleFullScreen ]

let update (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
    state.Player.StopMoving()

    for e in events kb do
        match e with
        | MoveLeft -> state.Player.MoveLeft()
        | MoveRight -> state.Player.MoveRight()
        | Shoot -> state <- { state with Projectiles = state.Player.Shoot() :: state.Projectiles }
        | TogglePause -> state <- { state with Paused = not state.Paused }
        | ToggleFullScreen -> g.ToggleFullScreen ()

    state.Player.Update t
    for projectile in state.Projectiles do projectile.Update t
    for ball in state.Balls do ball.Update t

    // TODO collisions here

let draw (sb: SpriteBatch) (t: GameTime) =
    sb.Draw(GameContent.textures.Backdrop, Vector2.Zero, Color.White)

    state.Player.Draw sb
    for projectile in state.Projectiles do projectile.Draw sb t
    for ball in state.Balls do ball.Draw sb t

    sb.Draw(GameContent.textures.Floor, Vector2(0.0f, 360.0f - 16.0f), Color.White)
