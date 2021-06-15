module GameApp.Prelude.Animation

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open GameApp.Prelude.AnimProps

type AnimationSet<'T when 'T : comparison>
    (texture: Texture2D,
     rows: int,
     columns: int,
     defaultAnimation: 'T,
     animations: Map<'T, Animation>) =

    let mutable frameIndex = 0
    let mutable previousAnimation = animations.[defaultAnimation]
    let mutable currentAnimation = animations.[defaultAnimation]
    let mutable msSinceLastFrameChange = 0.0
    let mutable msBetweenFrames = 0.0

    let width = texture.Width / columns
    let height = texture.Height / rows

    member this.Play(name: 'T) =
        previousAnimation <- currentAnimation
        currentAnimation <- animations.[name]
        msBetweenFrames <- 1_000.0 / float currentAnimation.FPS

    member this.Update (time: GameTime) =
        msSinceLastFrameChange <- msSinceLastFrameChange + float time.ElapsedGameTime.Milliseconds

        if msSinceLastFrameChange >= msBetweenFrames
        then
            msSinceLastFrameChange <- 0.0
            let maxFrame = List.length currentAnimation.Frames - 1
            let pastLastFrame = frameIndex + 1 > maxFrame

            if pastLastFrame
            then frameIndex <- 0
            else frameIndex <- frameIndex + 1

            if pastLastFrame && not currentAnimation.Loops
            then currentAnimation <- previousAnimation

    member this.Draw(sb: SpriteBatch, location: Vector2) =
        let frame =
            currentAnimation.Frames
            |> List.tryItem frameIndex
            |> Option.defaultWith (fun _ -> List.head currentAnimation.Frames)
        let row = int (float frame / float columns)
        let column = frame % columns

        let source = Rectangle(width * column, height * row, width, height);
        let destination = Rectangle(int location.X, int location.Y, width, height);

        if currentAnimation.Mirrored
        then sb.Draw (texture, destination, source, Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f)
        else sb.Draw (texture, destination, source, Color.White)
