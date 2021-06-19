[<AutoOpen>]
module Prelude

open Microsoft.Xna.Framework

let inline flooredInt a = int (floor a)
let inline vec2 x y = Vector2 (x, y)
let inline within minValue maxValue x = x |> max minValue |> min maxValue

[<RequireQualifiedAccess>]
module Vec2 =
    let zero = Vector2.Zero
    let inline withX x (v: Vector2) = Vector2(x, v.Y)
    let inline withY y (v: Vector2) = Vector2(v.X, y)
    let inline offset o (v: Vector2) = Vector2(v.X + o, v.Y + o)

[<RequireQualifiedAccess>]
module VelocityPerSecond =
    /// <summary>
    /// Returns the velocity for a given elapsed time.
    /// </summary>
    /// <param name="v">
    /// The velocity per second
    /// </param>
    /// <param name="t">
    /// The GameTime to apply to the velocity per second
    /// </param>
    let forGameTime (v: float32) (t: GameTime) =
        v * (float32 t.ElapsedGameTime.Milliseconds / 1_000.0f)

    let vec2ForGameTime (v: Vector2) (t: GameTime) =
        v * (float32 t.ElapsedGameTime.Milliseconds / 1_000.0f)
