module GameApp.Play.PlayState

open System
open GameApp
open GameApp.Play
open GameApp.Play.Ball
open GameApp.Play.Player
open GameApp.Play.Projectile
open Microsoft.Xna.Framework

type PauseMenuItem =
    | Continue
    | MainMenuExit
    | SystemExit

type State =
    { Score: int
      Paused: bool
      SelectedPauseMenuItem: PauseMenuItem
      Player: Player
      Energy: int
      TimeSinceLastBallSpawn: int
      TimeSinceLastShot: int
      Projectiles: Projectile list
      Balls: Ball list }

[<RequireQualifiedAccess>]
module State =

    let init (): State =
        { Score = 0
          Paused = false
          SelectedPauseMenuItem = Continue
          Player = Player ()
          Energy = 100
          TimeSinceLastShot = Conf.Delay.shooting + 1
          TimeSinceLastBallSpawn = Conf.Delay.initialBallSpawn + 1
          Projectiles = []
          Balls = [] }

    let isGameOver s =
        s.Energy <= 0

    let pause s =
        { s with
            Paused = true
            SelectedPauseMenuItem = Continue }

    let pauseMenuUp s =
        let newSelected =
            match s.SelectedPauseMenuItem with
            | Continue -> SystemExit
            | MainMenuExit -> Continue
            | SystemExit -> MainMenuExit
        { s with SelectedPauseMenuItem = newSelected }

    let pauseMenuDown s =
        let newSelected =
            match s.SelectedPauseMenuItem with
            | Continue -> MainMenuExit
            | MainMenuExit -> SystemExit
            | SystemExit -> Continue
        { s with SelectedPauseMenuItem = newSelected }

    let pauseMenuEnter s =
        match s.SelectedPauseMenuItem with
        | Continue -> { s with Paused = false }
        | MainMenuExit -> GameState.changeScreen GameState.MainMenu; s
        | SystemExit -> exit 0

    let shoot state =
        let playerState = state.Player.GetState ()
        let hasNotReachedMax = List.length state.Projectiles < 2

        if hasNotReachedMax && state.TimeSinceLastShot > Conf.Delay.shooting
        then
            let x = playerState.Position.X + Conf.Player.width / 2.0f
            let y = playerState.Position.Y + Conf.Player.height

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

    let inline private spawnDelay (state: State) =
        let levels = state.Score / 10_000
        let levelDelay = Conf.Delay.initialBallSpawn - (levels * 500)
        max Conf.Delay.minBallSpawn levelDelay

    let spawnBall (state: State) =
        if state.TimeSinceLastBallSpawn >= spawnDelay state
        then
            let direction =
                match gen.Next() % 2 with
                | 0 -> Left
                | _ -> Right
            let x = gen.Next(50, int Conf.Screen.width - 50)
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

    let private collidePlayerWith (balls: Ball list) (s: State) =
        let player = s.Player
        let playerBox = player.GetState().Box

        if player.Invulnerable()
        then
            0
        else
            let ballsTouchingPlayer =
                balls
                |> List.filter (fun b -> Collision.aabbCheck (b.GetState()).Box playerBox)
                |> List.length

            if ballsTouchingPlayer > 0
            then player.Dispatch(Hit)

            ballsTouchingPlayer * Conf.Energy.lossPerHit

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

        let energyGain =
            destroyedBalls
            |> List.filter (fun b -> b.GetState().Size = Small)
            |> List.length
            |> (*) Conf.Energy.gainPerSmallBall

        let energyLoss =
            collidePlayerWith remainingBalls state

        let newEnergy =
            state.Energy + energyGain - energyLoss
            |> within 0 100

        { state with
            Projectiles = List.filter (fun x -> x.Alive()) state.Projectiles
            Balls = remainingBalls @ newBalls
            Score = state.Score + points
            Energy = newEnergy }
