using Machina;
using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HelloGame
{
    public class GameCore : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        static AssetLibrary assets;

        Scene gameScene;

        public GameCore()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            assets = new AssetLibrary(this);
        }

        protected override void Initialize()
        {
            gameScene = new Scene();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            assets.LoadAllTextures();

            Actor ballActor = new Actor
            {
                position = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2)
            };

            Actor linkin = new Actor
            {
                position = new Vector2(200, 200)
            };

            gameScene.AddActor(linkin);
            new SpriteRenderer(linkin, new GridBasedSpriteSheet(assets.GetTexture("linkin"), new Point(16, 16)));

            gameScene.AddActor(ballActor);
            new TextureRenderer(ballActor, assets.GetTexture("ball"));
            new KeyboardMovement(ballActor);
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
