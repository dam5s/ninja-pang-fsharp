[<RequireQualifiedAccess>]
module GameApp.Keyboard

open Microsoft.Xna.Framework.Input

type State =
    { Current: KeyboardState
      Previous: KeyboardState }

let createState current previous =
    { Current = current
      Previous = previous |> Option.defaultValue current }

let private oneIsDown keys kb =
    keys |> List.exists kb.Current.IsKeyDown

let private isDown key kb =
    kb.Current.IsKeyDown key

let private wasReleased key kb =
    kb.Current.IsKeyUp(key) && kb.Previous.IsKeyDown(key)

let private oneWasReleased keys kb =
    keys |> List.exists (fun k -> wasReleased k kb)

let togglePause = oneWasReleased [Keys.P; Keys.Escape]
let toggleFullScreen kb =
    let altIsDown = oneIsDown [Keys.LeftAlt; Keys.RightAlt] kb
    let enterWasReleased = wasReleased Keys.Enter kb

    (altIsDown && enterWasReleased) || wasReleased Keys.F11 kb

let menuReturn = oneWasReleased [Keys.Escape; Keys.X; Keys.NumPad0]
let menuEnter kb = oneWasReleased [Keys.Enter; Keys.Z] kb
let menuUp kb = wasReleased Keys.Up kb
let menuDown kb = wasReleased Keys.Down kb

let playerLeft = oneIsDown [Keys.Left]
let playerRight = oneIsDown [Keys.Right]
let playerShoot = oneIsDown [Keys.Up; Keys.Z]
