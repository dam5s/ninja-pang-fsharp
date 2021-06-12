[<RequireQualifiedAccess>]
module GameApp.Keyboard

open Microsoft.Xna.Framework.Input

type State =
    { Current: KeyboardState
      Previous: KeyboardState }

let createState current previous =
    { Current = current
      Previous = previous |> Option.defaultValue current }

let oneIsDown keys kb =
    keys |> List.exists kb.Current.IsKeyDown

let wasReleased key kb =
    kb.Current.IsKeyUp(key) && kb.Previous.IsKeyDown(key)
