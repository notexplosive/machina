using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HelloGame.Hello
{
    class Actor
    {
        public Texture2D texture;
        public Vector2 position = new Vector2();
        public Color color = new Color();

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw texture centered
            spriteBatch.Draw(texture, position, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
        }

        public void Update(float dt)
        {
            var curKeys = Keyboard.GetState();

            if (curKeys.IsKeyDown(Keys.Up))
                position.Y -= 120f * dt;

            if (curKeys.IsKeyDown(Keys.Down))
                position.Y += 120f * dt;

            if (curKeys.IsKeyDown(Keys.Left))
                position.X -= 120f * dt;

            if (curKeys.IsKeyDown(Keys.Right))
                position.X += 120f * dt;
        }
    }
}
