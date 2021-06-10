module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

let pixelWidth = 640
let pixelHeight = 360
let pixelAspect = float pixelWidth / float pixelHeight

type GameLoop () as x =
    inherit Game()

    let graphics = new GraphicsDeviceManager(x)

    do x.Content.RootDirectory <- "Content"
    do x.Window.AllowUserResizing <- true

    let mutable renderTarget = Unchecked.defaultof<RenderTarget2D>
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable font = Unchecked.defaultof<SpriteFont>

    let mutable recordedKeyboardState: KeyboardState option = None

    override x.Initialize() =
        base.Initialize()
        ()

    override this.LoadContent() =
        let pp = graphics.GraphicsDevice.PresentationParameters

        renderTarget <- new RenderTarget2D(x.GraphicsDevice, pixelWidth, pixelHeight, false,
            SurfaceFormat.Color, DepthFormat.None, pp.MultiSampleCount, RenderTargetUsage.DiscardContents)
        spriteBatch <- new SpriteBatch(x.GraphicsDevice)
        font <- x.Content.Load<SpriteFont>("Fonts/PressStart_Regular_17")
        ()

    override this.Update (gameTime) =
        let currentState = Keyboard.GetState()
        let previousState = recordedKeyboardState |> Option.defaultValue currentState

        let altIsDown = currentState.IsKeyDown(Keys.LeftAlt) || currentState.IsKeyDown(Keys.RightAlt)
        let enterWasReleased = currentState.IsKeyUp(Keys.Enter) && previousState.IsKeyDown(Keys.Enter)

        if altIsDown && enterWasReleased then graphics.ToggleFullScreen ()

        recordedKeyboardState <- Some currentState
        ()

    override this.Draw (gameTime) =
        x.GraphicsDevice.SetRenderTarget(renderTarget)
        x.GraphicsDevice.Clear Color.CornflowerBlue

        spriteBatch.Begin()
        spriteBatch.DrawString(font, "Hello", Vector2.Zero, Color.WhiteSmoke)
        spriteBatch.End()

        let windowBounds = x.Window.ClientBounds
        let windowWidth = float windowBounds.Width
        let windowHeight = float windowBounds.Height

        let roundDown = floor >> int
        let multiplier = roundDown (windowHeight / float pixelHeight)
        let targetWidth = pixelWidth * multiplier
        let targetHeight = pixelHeight * multiplier

        let destination =
            Rectangle(
                roundDown ((windowWidth - float targetWidth) / 2.0),
                roundDown ((windowHeight - float targetHeight) / 2.0),
                targetWidth,
                targetHeight
            )

        x.GraphicsDevice.SetRenderTarget(null)

        x.GraphicsDevice.Clear Color.Black
        spriteBatch.Begin()
        spriteBatch.Draw(renderTarget, destination, Color.White)
        spriteBatch.End()
        ()
