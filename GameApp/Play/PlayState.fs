[<RequireQualifiedAccess>]
module GameApp.Play.PlayState

open GameApp
open GameApp.Play.Ball
open GameApp.Play.Player
open GameApp.Play.Projectile
open GameApp.Prelude.Collisions
open Microsoft.Xna.Framework

module private Conf =
    let shootingDelayInMs = 200
    let playerWidth = 32.0f
    let playerHeight = 32.0f

type State =
    { Score: int
      Paused: bool
      Player: Player
      TimeSinceLastShot: int
      Projectiles: Projectile list
      Balls: Ball list }

let init (): State =
    { Score = 0
      Paused = false
      Player = Player()
      TimeSinceLastShot = Conf.shootingDelayInMs + 1
      Projectiles = []
      Balls = [ Ball (Big, Right, vec2 100.0f 100.0f)
                Ball (Big, Left, vec2 300.0f 60.0f)
                Ball (Big, Right, vec2 500.0f 80.0f) ] }

[<RequireQualifiedAccess>]
module Shooting =
    let trigger state =
        let playerState = state.Player.GetState ()
        let hasNotReachedMax = List.length state.Projectiles < 2

        if hasNotReachedMax && state.TimeSinceLastShot > Conf.shootingDelayInMs
        then
            let x = playerState.Position.X + Conf.playerWidth / 2.0f
            let y = playerState.Position.Y + Conf.playerHeight

            Effect.play GameContent.sounds.Shoot

            { state with
                TimeSinceLastShot = 0
                Projectiles = Projectile (vec2 x y) :: state.Projectiles }
        else
            state

    let updateTimeSinceLastShot (time: GameTime) (state: State) =
        let elapsed = time.ElapsedGameTime.Milliseconds
        let newState = { state with TimeSinceLastShot = state.TimeSinceLastShot + elapsed }

        if newState.TimeSinceLastShot <= 100
        then state.Player.Dispatch(Shoot)

        newState

[<RequireQualifiedAccess>]
module Collisions =

    let private splitBall (state: Ball.State) =
        let pos = state.Position

        match state.Size with
        | Big ->
            [ Ball(Medium, Left, vec2 (pos.X - 10.0f) pos.Y)
              Ball(Medium, Right, vec2 (pos.X + 10.0f) pos.Y) ]
        | Medium ->
            [ Ball(Small, Left, vec2 (pos.X - 10.0f) pos.Y)
              Ball(Small, Right, vec2 (pos.X + 10.0f) pos.Y) ]
        | Small -> []

    let private collideProjectileWithBalls (balls: Ball list) (p: Projectile) =
        let projectileBox = p.GetState().Box

        if projectileBox.Origin.Y <= 0.0f
        then p.Kill()

        if p.Alive()
        then
            balls
            |> List.collect (fun b ->
                let ballState = b.GetState()

                if b.Alive() && p.Alive() && Collision.aabbCheck projectileBox ballState.Box
                then
                    b.Kill()
                    p.Kill()
                    Effect.play GameContent.sounds.Pop
                    splitBall ballState
                else
                    []
            )
        else
            []

    let inline private ballIsAlive (b: Ball) = b.Alive()
    let inline private ballPoints (b: Ball) =
        match b.GetState().Size with
        | Big -> 50
        | Medium -> 100
        | Small -> 200

    let applyAll (state: State) =
        let newBalls =
            state.Projectiles
            |> List.collect (collideProjectileWithBalls state.Balls)

        let remainingBalls, destroyedBalls = List.partition ballIsAlive state.Balls
        let points = destroyedBalls |> List.sumBy ballPoints

        { state with
            Projectiles = List.filter (fun x -> x.Alive()) state.Projectiles
            Balls = remainingBalls @ newBalls
            Score = state.Score + points }
