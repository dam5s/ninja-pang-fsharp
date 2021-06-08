module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type GameLoop () as x =
    inherit Game()

    do x.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(x)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>

    override x.Initialize() =
        base.Initialize()

        ()

    override this.LoadContent() =

        spriteBatch <- new SpriteBatch(x.GraphicsDevice)
         // TODO: use x.Content to load your game content here
         // On Windows you can load any PNG file directly as Texture2D

         // Read more about MonoGame's Content Pipeline: https://docs.monogame.net/articles/tools/mgcb_editor.html
         // or install it with package manager console: [dotnet tool install -g dotnet-mgcb-editor]

        ()

    override this.Update (gameTime) =

         // TODO: Add your update logic here

        ()

    override this.Draw (gameTime) =

        x.GraphicsDevice.Clear Color.CornflowerBlue

        // TODO: Add your drawing code here

        ()
