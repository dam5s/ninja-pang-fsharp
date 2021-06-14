module GameApp.Prelude.AnimProps

type Animation =
    { Frames: int list
      FPS: float
      Loops: bool
      Mirrored: bool }

[<RequireQualifiedAccess>]
module Animation =
    let loop frames fps =
        { Frames = frames; FPS = fps; Loops = true; Mirrored = false }

    let mirroredLoop frames fps =
        { Frames = frames; FPS = fps; Loops = true; Mirrored = true }

    let interrupt frames fps =
        { Frames = frames; FPS = fps; Loops = false; Mirrored = false }
