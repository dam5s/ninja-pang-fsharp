module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type GameLoop () as x =
    inherit Game()

    do x.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(x)

    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable font = Unchecked.defaultof<SpriteFont>

    override x.Initialize() =
        base.Initialize()
        ()

    override this.LoadContent() =
        font <- x.Content.Load<SpriteFont>("Fonts/PressStart_Regular_17")
        spriteBatch <- new SpriteBatch(x.GraphicsDevice)
        ()

    override this.Update (gameTime) =
        ()

    override this.Draw (gameTime) =
        x.GraphicsDevice.Clear Color.CornflowerBlue
        spriteBatch.Begin()
        spriteBatch.DrawString(font, "Hello", Vector2(0.0f, 0.0f), Color.WhiteSmoke)
        spriteBatch.End()
        ()
