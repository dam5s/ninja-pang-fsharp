module Game

open GameApp
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

let private canvas = Rectangle (0, 0, 640, 360)
let private multiSampleCount = 1
let private flooredInt = floor >> int
let private calculateScale a b = min a b |> ((*) 2.0) |> floor |> (*) 0.5

type GameLoop () as this =
    inherit Game()

    let graphics = new GraphicsDeviceManager(this)

    let mutable renderTarget = Unchecked.defaultof<RenderTarget2D>
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable recordedKbState: KeyboardState option = None

    override this.Initialize() =
        this.Content.RootDirectory <- "Content"
        this.Window.AllowUserResizing <- true

        graphics.PreferMultiSampling <- false
        graphics.SynchronizeWithVerticalRetrace <- true
        graphics.HardwareModeSwitch <- false
        graphics.PreferredBackBufferWidth <- canvas.Width * 2
        graphics.PreferredBackBufferHeight <- canvas.Height * 2
        graphics.ApplyChanges() // this should not be necessary here,
                                // but preferred size is not set without it
                                // even when I remove *all* the code from the project but the 2 lines above.

        base.Initialize()

    override this.LoadContent() =
        renderTarget <- new RenderTarget2D(this.GraphicsDevice, canvas.Width, canvas.Height)
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        GameContent.load this.Content
        ()

    override this.Update time =
        let currentKbState = Keyboard.GetState()
        let kb = Keyboard.createState currentKbState recordedKbState

        match GameScreen.get() with
        | GameScreen.MainMenu -> MainMenuScreen.update kb graphics time
        | GameScreen.Play -> PlayScreen.update kb graphics time
        | GameScreen.HighScore -> HighScoreScreen.update kb graphics time

        recordedKbState <- Some currentKbState
        ()

    override this.Draw time =
        this.GraphicsDevice.SetRenderTarget(renderTarget)
        this.GraphicsDevice.Clear Color.CornflowerBlue

        spriteBatch.Begin(samplerState = SamplerState.PointClamp)

        match GameScreen.get() with
        | GameScreen.MainMenu -> MainMenuScreen.draw spriteBatch time
        | GameScreen.Play -> PlayScreen.draw spriteBatch time
        | GameScreen.HighScore -> HighScoreScreen.draw spriteBatch time

        spriteBatch.End()

        let viewBounds = this.Window.ClientBounds
        let viewWidth = float viewBounds.Width
        let viewHeight = float viewBounds.Height

        let scale = calculateScale (viewWidth / (float canvas.Width)) (viewHeight / (float canvas.Height))

        let targetWidth = (float canvas.Width) * scale |> flooredInt
        let targetHeight = (float canvas.Height) * scale |> flooredInt
        let x = (viewWidth - float targetWidth) / 2.0 |> flooredInt
        let y = (viewHeight - float targetHeight) / 2.0 |> flooredInt

        let destination = Rectangle (x, y, targetWidth, targetHeight)

        this.GraphicsDevice.SetRenderTarget(null)
        this.GraphicsDevice.Clear Color.Black

        spriteBatch.Begin(samplerState = SamplerState.PointClamp)
        spriteBatch.Draw(renderTarget, destination, Color.White)
        spriteBatch.End()
        ()
