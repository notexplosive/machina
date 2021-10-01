using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Data
{
    public class TextureColors
    {
        private readonly HashSet<Color> colorMap = new HashSet<Color>();
        private readonly Texture2D sourceTexture;

        public TextureColors(Texture2D texture)
        {
            this.sourceTexture = texture;

            var rawData = new Color[this.sourceTexture.Width * this.sourceTexture.Height];
            this.sourceTexture.GetData(rawData);
            foreach (var color in rawData)
            {
                if (!this.colorMap.Contains(color))
                {
                    this.colorMap.Add(color);
                }
            }
        }

        public Color[] GetColors()
        {
            var colors = new List<Color>();
            foreach (var color in this.colorMap)
            {
                colors.Add(color);
            }

            colors.Sort((a, b) => { return GetLightnessValue(a) - GetLightnessValue(b); });
            return colors.ToArray();
        }

        private int GetLightnessValue(Color color)
        {
            // Average RGB values
            return (color.R + color.G + color.B) / 3;
        }
    }
}
