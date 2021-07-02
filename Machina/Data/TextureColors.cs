using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public class TextureColors
    {
        private readonly Texture2D sourceTexture;
        private readonly HashSet<Color> colorMap = new HashSet<Color>();

        public TextureColors(Texture2D texture)
        {
            this.sourceTexture = texture;

            var rawData = new Color[this.sourceTexture.Width * this.sourceTexture.Height];
            sourceTexture.GetData(rawData);
            foreach (var color in rawData)
            {
                if (!colorMap.Contains(color))
                {
                    colorMap.Add(color);
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
