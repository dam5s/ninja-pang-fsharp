[<RequireQualifiedAccess>]
module GameApp.Play.PlayScreen

open GameApp
open GameApp.Play.Player
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

module private Conf =
    let screenHeight = 360.0f
    let floorHeight = 16.0f

type KeyboardEvent =
    | PlayerMoveLeft
    | PlayerMoveRight
    | PlayerShoot
    | TogglePause
    | ToggleFullScreen

let mutable private state = PlayState.init ()

let init () =
    state <- PlayState.init ()

let private events kb =
    [ if Keyboard.movingLeft kb then PlayerMoveLeft
      if Keyboard.movingRight kb then PlayerMoveRight
      if Keyboard.shooting kb then PlayerShoot
      if Keyboard.togglePause kb then TogglePause
      if Keyboard.toggleFullScreen kb then ToggleFullScreen ]

let private playingUpdate (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
    state <- PlayState.Shooting.updateTimeSinceLastShot t state
    state.Player.Dispatch(StopMoving)

    for e in events kb do
        match e with
        | PlayerMoveLeft -> state.Player.Dispatch(MoveLeft)
        | PlayerMoveRight -> state.Player.Dispatch(MoveRight)
        | PlayerShoot -> state <- PlayState.Shooting.trigger state
        | TogglePause -> state <- { state with Paused = not state.Paused }
        | ToggleFullScreen -> g.ToggleFullScreen()

    state.Player.Update t
    for x in state.Projectiles do x.Update t
    for x in state.Balls do x.Update t

    state <- PlayState.Collisions.applyAll state

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
    for x in state.Projectiles do x.Draw sb
    for x in state.Balls do x.Draw sb

    sb.Draw(GameContent.textures.Floor, vec2 0.0f (Conf.screenHeight - Conf.floorHeight), Color.White)

    HUD.draw state sb

    if state.Paused
    then HUD.drawPausedOverlay sb
