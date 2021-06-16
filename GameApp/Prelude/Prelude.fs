[<AutoOpen>]
module Prelude

open Microsoft.Xna.Framework

let vec2 x y = Vector2 (x, y)

[<RequireQualifiedAccess>]
module Vec2 =
    let zero = Vector2.Zero
    let inline withX x (v: Vector2) = Vector2(x, v.Y)
    let inline withY y (v: Vector2) = Vector2(v.X, y)
