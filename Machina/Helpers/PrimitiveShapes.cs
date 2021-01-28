using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Helpers
{
    class PrimitiveShapes
    {
        private Texture2D rectangleTexture;
        private SpriteBatch spriteBatch;

        public PrimitiveShapes(GraphicsDevice graphics, SpriteBatch spriteBatch)
        {
            this.rectangleTexture = new Texture2D(graphics, 1, 1);
            this.rectangleTexture.SetData(new[] { Color.White });

            this.spriteBatch = spriteBatch;
        }

        public void DrawRectangle(Rectangle rect, Color color)
        {
            spriteBatch.Draw(rectangleTexture, rect, color);
        }
    }
}
