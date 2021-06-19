[<RequireQualifiedAccess>]
module Collision

open Microsoft.Xna.Framework

type Box =
    { Origin: Vector2
      Width: float32
      Height: float32 }

let inline private left box = box.Origin.X
let inline private right box = box.Origin.X + box.Width
let inline private top box = box.Origin.Y
let inline private bottom box = box.Origin.Y + box.Height

let aabbCheck (a: Box) (b: Box) =
    left a < right b
    && right a > left b
    && top a < bottom b
    && bottom a > top b
