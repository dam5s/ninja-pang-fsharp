[<AutoOpen>]
module AnimationSet

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type AnimationState<'T when 'T : comparison> =
    { FrameIndex: int
      PreviousAnimation: Animation
      CurrentAnimation: Animation
      MsSinceLastFrameChange: float
      MsBetweenFrames: float
      Animations: Map<'T, Animation> }

let private play name state =
    let newAnimation = state.Animations.[name]

    { state with
        PreviousAnimation = state.CurrentAnimation
        CurrentAnimation = newAnimation
        MsBetweenFrames = 1_000.0 / float newAnimation.FPS }

let private updateMsSinceLastFrameChange (time: GameTime) state =
    { state with
        MsSinceLastFrameChange = state.MsSinceLastFrameChange + float time.ElapsedGameTime.Milliseconds }

let private updateFrame state =
    if state.MsSinceLastFrameChange >= state.MsBetweenFrames
    then
        let maxFrame = List.length state.CurrentAnimation.Frames - 1
        let pastLastFrame = state.FrameIndex + 1 > maxFrame

        let newFrameIndex =
            if pastLastFrame
            then 0
            else state.FrameIndex + 1

        let newCurrentAnimation =
            if pastLastFrame && not state.CurrentAnimation.Loops
            then state.PreviousAnimation
            else state.CurrentAnimation

        { state with
              MsSinceLastFrameChange = 0.0
              FrameIndex = newFrameIndex
              CurrentAnimation = newCurrentAnimation }
    else
        state

type AnimationSet<'T when 'T : comparison>
    (texture: Texture2D,
     rows: int,
     columns: int,
     defaultAnimation: 'T,
     animations: Map<'T, Animation>) =

    let mutable state =
        { FrameIndex = 0
          PreviousAnimation = animations.[defaultAnimation]
          CurrentAnimation = animations.[defaultAnimation]
          MsSinceLastFrameChange = 0.0
          MsBetweenFrames = 0.0
          Animations = animations }

    let width = texture.Width / columns
    let height = texture.Height / rows

    member this.Play(name: 'T) =
        state <- state |> play name

    member this.Update (time: GameTime) =
        state <- state |> updateMsSinceLastFrameChange time |> updateFrame

    member this.Draw (sb: SpriteBatch, location: Vector2, ?mask: Color) =
        let frame =
            state.CurrentAnimation.Frames
            |> List.tryItem state.FrameIndex
            |> Option.defaultWith (fun _ -> List.head state.CurrentAnimation.Frames)

        let row = int (float frame / float columns)
        let column = frame % columns

        let source = Rectangle(width * column, height * row, width, height);
        let destination = Rectangle(int location.X, int location.Y, width, height)
        let resolvedMask = defaultArg mask Color.White

        if state.CurrentAnimation.Mirrored
        then sb.Draw (texture, destination, source, resolvedMask, 0.0f, Vec2.zero, SpriteEffects.FlipHorizontally, 0.0f)
        else sb.Draw (texture, destination, source, resolvedMask)
