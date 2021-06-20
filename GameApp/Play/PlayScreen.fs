[<RequireQualifiedAccess>]
module GameApp.Play.PlayScreen

open GameApp
open GameApp.Play.PlayState
open GameApp.Play.Player
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

let mutable private state = State.init ()

let init () =
    state <- State.init ()

module Playing =
    type KeyboardEvent =
        | PlayerMoveLeft
        | PlayerMoveRight
        | PlayerShoot
        | TogglePause
        | ToggleFullScreen

    let private events kb =
        [ if Keyboard.playerLeft kb then PlayerMoveLeft
          if Keyboard.playerRight kb then PlayerMoveRight
          if Keyboard.playerShoot kb then PlayerShoot
          if Keyboard.pause kb then TogglePause
          if Keyboard.toggleFullScreen kb then ToggleFullScreen ]

    let update (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
        state <- State.updateTimers t state
        state.Player.Dispatch(StopMoving)

        for e in events kb do
            match e with
            | PlayerMoveLeft -> state.Player.Dispatch(MoveLeft)
            | PlayerMoveRight -> state.Player.Dispatch(MoveRight)
            | PlayerShoot -> state <- State.shoot state
            | TogglePause -> state <- State.pause state
            | ToggleFullScreen -> g.ToggleFullScreen()

        if state.TimeSinceLastShot = 0
        then Effect.play GameContent.sounds.Shoot

        // Display shooting animation for 100 ms
        if state.TimeSinceLastShot <= 100
        then state.Player.Dispatch(Shoot)

        state.Player.Update t
        for x in state.Projectiles do x.Update t
        for x in state.Balls do x.Update t

        state <- Collisions.applyAll state
        state <- State.spawnBall state

        if State.isGameOver state
        then Effect.play GameContent.sounds.GameOver

module Paused =
    type private KeyboardEvent =
        | MenuUp
        | MenuDown
        | MenuEnter
        | MenuReturn
        | ToggleFullScreen

    let private events kb =
        [ if Keyboard.menuUp kb then MenuUp
          if Keyboard.menuDown kb then MenuDown
          if Keyboard.menuEnter kb then MenuEnter
          if Keyboard.menuReturn kb then MenuReturn
          if Keyboard.toggleFullScreen kb then ToggleFullScreen ]

    let update (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
        for e in events kb do
            match e with
            | MenuUp -> state <- State.pauseMenuUp state
            | MenuDown -> state <- State.pauseMenuDown state
            | MenuEnter -> state <- State.pauseMenuEnter state
            | MenuReturn -> state <- { state with Paused = not state.Paused }
            | ToggleFullScreen -> g.ToggleFullScreen()

let update (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
    if state.Paused
    then Paused.update kb g t
    else

    if State.isGameOver state
    then () // game over
    else

    Playing.update kb g t

let draw (sb: SpriteBatch) (t: GameTime) =
    sb.Draw(GameContent.textures.Backdrop, Vec2.zero, Color.White)

    state.Player.Draw sb
    for x in state.Projectiles do x.Draw sb
    for x in state.Balls do x.Draw sb

    sb.Draw(GameContent.textures.Floor, vec2 0.0f (Conf.Screen.height - Conf.Floor.height), Color.White)

    sb
    |> HUD.draw state
    |> HUD.drawPaused state
    |> HUD.drawGameOver state
    |> ignore
