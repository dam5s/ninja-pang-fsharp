module GameApp.Prelude.GameTime

open Microsoft.Xna.Framework

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
