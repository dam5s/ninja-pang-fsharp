[<RequireQualifiedAccess>]
module GameApp.Play.PlayState

open System
open GameApp
open GameApp.Play.Ball
open GameApp.Play.Player
open GameApp.Play.Projectile
open GameApp.Prelude.Collisions
open Microsoft.Xna.Framework

module private Conf =
    let shootingDelayInMs = 200
    let ballSpawnDelayInMs = 5_000
    let playerWidth = 32.0f
    let playerHeight = 32.0f
    let screenWidth = 640.0f

type State =
    { Score: int
      Paused: bool
      Player: Player
      TimeSinceLastBallSpawn: int
      TimeSinceLastShot: int
      Projectiles: Projectile list
      Balls: Ball list }

let init (): State =
    { Score = 0
      Paused = false
      Player = Player()
      TimeSinceLastShot = Conf.shootingDelayInMs + 1
      TimeSinceLastBallSpawn = Conf.ballSpawnDelayInMs + 1
      Projectiles = []
      Balls = [] }

[<RequireQualifiedAccess>]
module Behaviors =
    let shoot state =
        let playerState = state.Player.GetState ()
        let hasNotReachedMax = List.length state.Projectiles < 2

        if hasNotReachedMax && state.TimeSinceLastShot > Conf.shootingDelayInMs
        then
            let x = playerState.Position.X + Conf.playerWidth / 2.0f
            let y = playerState.Position.Y + Conf.playerHeight

            { state with
                TimeSinceLastShot = 0
                Projectiles = Projectile (vec2 x y) :: state.Projectiles }
        else
            state

    let updateTimers (time: GameTime) (state: State) =
        let elapsed = time.ElapsedGameTime.Milliseconds

        { state with
            TimeSinceLastBallSpawn = state.TimeSinceLastBallSpawn + elapsed
            TimeSinceLastShot = state.TimeSinceLastShot + elapsed }

    let private gen = Random()

    let spawnBall (state: State) =
        if state.TimeSinceLastBallSpawn >= Conf.ballSpawnDelayInMs
        then
            let direction =
                match gen.Next() % 2 with
                | 0 -> Left
                | _ -> Right
            let x = gen.Next(50, int Conf.screenWidth - 50)
            let y = gen.Next(50, 100)

            let newBall = Ball (Big, direction, vec2 (float32 x) (float32 y))

            { state with
                TimeSinceLastBallSpawn = 0
                Balls = newBall :: state.Balls }
        else
            state

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

    let private collideProjectileWithOneBall (p: Projectile) projectileBox (b: Ball) =
        let ballState = b.GetState()

        if b.Alive() && p.Alive() && Collision.aabbCheck projectileBox ballState.Box
        then
            b.Kill()
            p.Kill()
            Effect.play GameContent.sounds.Pop
            splitBall ballState
        else
            []

    let private collideProjectileWithAllBalls (balls: Ball list) (p: Projectile) =
        let projectileBox = p.GetState().Box

        if projectileBox.Origin.Y <= 0.0f
        then p.Kill()

        if p.Alive()
        then List.collect (collideProjectileWithOneBall p projectileBox) balls
        else []

    let inline private ballIsAlive (b: Ball) = b.Alive()
    let inline private ballPoints (b: Ball) =
        match b.GetState().Size with
        | Big -> 50
        | Medium -> 100
        | Small -> 200

    let applyAll (state: State) =
        let newBalls = List.collect (collideProjectileWithAllBalls state.Balls) state.Projectiles

        let remainingBalls, destroyedBalls = List.partition ballIsAlive state.Balls
        let points = List.sumBy ballPoints destroyedBalls

        { state with
            Projectiles = List.filter (fun x -> x.Alive()) state.Projectiles
            Balls = remainingBalls @ newBalls
            Score = state.Score + points }
