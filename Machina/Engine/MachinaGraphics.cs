using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Engine
{
    public class MachinaGraphics
    {
        /// <summary>
        /// Make sure you dispose the texture when you're done!
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="sourceTexture"></param>
        /// <param name="graphicsDevice"></param>
        /// <returns></returns>
        public static Texture2D CropTexture(Rectangle rect, Texture2D sourceTexture, GraphicsDevice graphicsDevice)
        {
            if (rect.Width * rect.Height == 0)
            {
                return null;
            }

            var cropTexture = new Texture2D(graphicsDevice, rect.Width, rect.Height);
            var data = new Color[rect.Width * rect.Height];
            sourceTexture.GetData(0, rect, data, 0, data.Length);
            cropTexture.SetData(data);
            return cropTexture;
        }
    }
}