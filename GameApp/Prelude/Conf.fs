[<RequireQualifiedAccess>]
module Conf

[<RequireQualifiedAccess>]
module Screen =
    let width = 640.0f
    let height = 360.0f

[<RequireQualifiedAccess>]
module Floor =
    let height = 16.0f
    let y = Screen.height - height

[<RequireQualifiedAccess>]
module Delay =
    // All delays are in milliseconds
    let shooting = 200
    let initialBallSpawn = 5_000
    let minBallSpawn = 2_000

[<RequireQualifiedAccess>]
module Player =
    let width = 32.0f
    let height = 32.0f
    let velocity = 150.0f
    let positionOverflow = 8.0f
    let minPosition = - positionOverflow
    let maxPosition = Screen.width - width + positionOverflow

[<RequireQualifiedAccess>]
module Energy =
    let lossPerHit = 25
    let gainPerSmallBall = 2
