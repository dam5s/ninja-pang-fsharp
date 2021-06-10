module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

let canvas = Rectangle (0, 0, 640, 360)
let multiSampleCount = 1

let flooredInt = floor >> int
let calculateScale a b = min a b |> ((*) 2.0) |> floor |> (*) 0.5

type GameLoop () as this =
    inherit Game()

    let graphics = new GraphicsDeviceManager(this)

    let mutable renderTarget = Unchecked.defaultof<RenderTarget2D>
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable font = Unchecked.defaultof<SpriteFont>
    let mutable recordedKeyboardState: KeyboardState option = None

    override this.Initialize() =
        this.Content.RootDirectory <- "Content"
        this.Window.AllowUserResizing <- true

        graphics.PreferMultiSampling <- false
        graphics.SynchronizeWithVerticalRetrace <- true
        graphics.HardwareModeSwitch <- false
        graphics.PreferredBackBufferWidth <- canvas.Width * 2
        graphics.PreferredBackBufferHeight <- canvas.Height * 2
        graphics.ApplyChanges()

        base.Initialize()

    override this.LoadContent() =
        renderTarget <- new RenderTarget2D(this.GraphicsDevice, canvas.Width, canvas.Height)
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        font <- this.Content.Load<SpriteFont>("Fonts/PressStart_Regular_17")
        ()

    override this.Update _ =
        let currentState = Keyboard.GetState()
        let previousState = recordedKeyboardState |> Option.defaultValue currentState

        let altIsDown = currentState.IsKeyDown(Keys.LeftAlt) || currentState.IsKeyDown(Keys.RightAlt)
        let enterWasReleased = currentState.IsKeyUp(Keys.Enter) && previousState.IsKeyDown(Keys.Enter)

        if altIsDown && enterWasReleased then graphics.ToggleFullScreen ()

        recordedKeyboardState <- Some currentState
        ()

    override this.Draw _ =
        this.GraphicsDevice.SetRenderTarget(renderTarget)
        this.GraphicsDevice.Clear Color.CornflowerBlue

        spriteBatch.Begin(samplerState = SamplerState.PointClamp)
        spriteBatch.DrawString(font, "Hello", Vector2.Zero, Color.WhiteSmoke)
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
