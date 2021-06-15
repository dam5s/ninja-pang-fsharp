module GameApp.Prelude.GameTime

open Microsoft.Xna.Framework

[<RequireQualifiedAccess>]
module VelocityPerSecond =
    // Takes velocity per second `v`
    // returns the velocity for the given elapsed time.
    let forGameTime (v: float32) (t: GameTime) =
        v * (float32 t.ElapsedGameTime.Milliseconds / 1_000.0f)
