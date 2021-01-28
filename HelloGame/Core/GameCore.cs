using Machina;
using Machina.Components;
using Machina.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PrimitiveBuddy;

namespace HelloGame
{
    public class GameCore : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        Texture2D ballTexture;
        private Actor firstActor;
        Primitive prim;
        private KeyboardState previousKeys;
        PrimitiveShapes primitiveShapes;

        Scene gameScene;

        public GameCore()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            firstActor = new Actor
            {
                position = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2)
            };

            gameScene = new Scene();
            gameScene.AddActor(firstActor);

            base.Initialize();

            // Initialize PrimitiveBuddy
            prim = new Primitive(graphics.GraphicsDevice, spriteBatch)
            {
                NumCircleSegments = 16
            };

            // Initialize Helpers.PrimitiveShapes
            primitiveShapes = new PrimitiveShapes(graphics.GraphicsDevice, spriteBatch);

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load all textures and other content
            ballTexture = Content.Load<Texture2D>("ball");

            new TextureRenderer(firstActor, ballTexture);
            new KeyboardMovement(firstActor);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            gameScene.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            gameScene.Draw(spriteBatch);
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
