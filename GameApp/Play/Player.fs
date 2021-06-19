module GameApp.Play.Player

open GameApp
open Microsoft.Xna.Framework

type PlayerAnim =
    | Idle
    | Shooting
    | RunRight
    | RunLeft

type State =
    { Position: Vector2
      VelocityX: float32
      Box: Collision.Box
      InvulnerabilityDuration: int }

type Event =
    | MoveLeft
    | MoveRight
    | StopMoving
    | Shoot
    | Hit

let inline private box (position: Vector2): Collision.Box =
    { Origin = vec2 (position.X + 10.0f) (position.Y + 5.0f)
      Width = 12.0f
      Height = 27.0f }

let private init () =
    let position = vec2 100.0f (Conf.Screen.height - Conf.Floor.height - Conf.Player.height)
    { Position = position
      VelocityX = 0.0f
      Box = box position
      InvulnerabilityDuration = 0 }

let private onUpdate (time: GameTime) state =
    let newX =
        state.Position.X + VelocityPerSecond.forGameTime state.VelocityX time
        |> max Conf.Player.minPosition
        |> min Conf.Player.maxPosition

    let newPosition = Vec2.withX newX state.Position

    let invulnerability =
        state.InvulnerabilityDuration - time.ElapsedGameTime.Milliseconds

    { state with
        Position = newPosition
        Box = box newPosition
        InvulnerabilityDuration = invulnerability }, NoEffect

let private update event state: State * Effect<PlayerAnim> =
    match event with
    | Event MoveLeft -> { state with VelocityX = -Conf.Player.velocity }, SetAnimation RunLeft
    | Event MoveRight -> { state with VelocityX = Conf.Player.velocity }, SetAnimation RunRight
    | Event StopMoving -> { state with VelocityX = 0.0f }, SetAnimation Idle
    | Event Shoot -> state, SetAnimation Shooting
    | Event Hit -> { state with InvulnerabilityDuration = 2_000 }, PlaySound GameContent.sounds.Hit
    | OnUpdate time -> onUpdate time state

let private animations () = AnimationSet(GameContent.textures.Ninja, 1, 13, Idle, Map(seq [
    (Idle, Animation.loop [0; 1; 2; 3] 2.0)
    (Shooting, Animation.interrupt [4] 1.0)
    (RunRight, Animation.loop [5; 6; 7; 8; 9; 10; 11; 12] 14.0)
    (RunLeft, Animation.mirroredLoop [5; 6; 7; 8; 9; 10; 11; 12] 14.0)
]))

let private draw state (animations: AnimationSet<PlayerAnim>) sb =
    let mask =
        if state.InvulnerabilityDuration > 0
        then Color(Color.Black, 0.2f)
        else Color.White

    animations.Draw (sb, state.Position, mask)

type Player () =
    inherit FunctionalComponent<State, Event, PlayerAnim>(init, update, draw, animations)

    member this.Invulnerable() =
        this.GetState().InvulnerabilityDuration > 0
