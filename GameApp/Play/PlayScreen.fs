[<RequireQualifiedAccess>]
module GameApp.PlayScreen

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type KeyboardEvent =
    | MoveLeft
    | MoveRight
    | Shoot
    | TogglePause

type State =
    { Score: int
      Paused: bool }

let mutable private state =
    { Score = 0; Paused = false }

let private shouldMoveLeft = Keyboard.oneIsDown [Keys.Left; Keys.A]
let private shouldMoveRight = Keyboard.oneIsDown [Keys.Right; Keys.D]
let private shouldShoot = Keyboard.oneIsDown [Keys.Up; Keys.Space; Keys.W; Keys.LeftControl; Keys.RightControl]
let private shouldTogglePause = Keyboard.oneIsDown [Keys.P; Keys.Escape]

let private events kb =
    [ if shouldMoveLeft kb then MoveLeft
      if shouldMoveRight kb then MoveRight
      if shouldShoot kb then Shoot
      if shouldTogglePause kb then TogglePause ]

let update (kb: Keyboard.State) (g: GraphicsDeviceManager) (t: GameTime) =
    for e in events kb do
        //
    ()

let draw (sb: SpriteBatch) (t: GameTime) =
    sb.DrawString(GameContent.fonts.MenuItemSelected, "Play", Vector2.Zero, Color.WhiteSmoke)
