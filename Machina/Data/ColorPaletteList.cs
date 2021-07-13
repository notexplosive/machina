using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    /// <summary>
    /// Expects a texture that is just rows of pixels describing a color palette.
    /// Each row of pixels is one "palette"
    /// </summary>
    public class ColorPaletteList
    {
        public readonly int rowCount;
        private readonly Color[,] colors;
        private readonly Texture2D sourceTexture;

        public ColorPaletteList(Texture2D texture)
        {
            this.sourceTexture = texture;
            var rawData = new Color[texture.Width * texture.Height];
            texture.GetData(rawData);

            this.colors = new Color[texture.Height, texture.Width];
            this.rowCount = texture.Height;

            for (int row = 0; row < texture.Height; row++)
            {
                for (int i = 0; i < texture.Width; i++)
                {
                    this.colors[row, i] = rawData[texture.Width * row + i];
                }
            }
        }

        public Color[] GetRow(int row)
        {
            var content = new Color[this.sourceTexture.Width];
            for (int i = 0; i < sourceTexture.Width; i++)
            {
                content[i] = this.colors[row, i];
            }
            return content;
        }

        public Color[] FindBestMatch(TextureColors colorsFromOriginalTexture)
        {
            int rowIndex = 0;
            while (rowIndex < this.rowCount)
            {
                var row = new List<Color>(GetRow(rowIndex));
                bool fail = false;

                foreach (var colorFromOriginalTexture in colorsFromOriginalTexture.GetColors())
                {
                    if (colorFromOriginalTexture.A == 0)
                    {
                        continue;
                    }

                    if (!row.Contains(colorFromOriginalTexture))
                    {
                        fail = true;
                        break;
                    }
                }

                if (!fail)
                {
                    return GetRow(rowIndex);
                }
                rowIndex++;
            }

            return null;
        }

        public Texture2D MakeTexture(Texture2D source, int targetRowIndex)
        {
            var sourceData = new Color[source.Width * source.Height];
            var newData = new Color[source.Width * source.Height];
            sourceData.CopyTo(newData, 0);
            source.GetData(sourceData);
            var match = FindBestMatch(new TextureColors(source));
            var sourcePalette = new List<Color>(match != null ? match : new Color[0]);
            var targetPalette = new List<Color>(GetRow(targetRowIndex));

            int paletteIndex = 0;
            foreach (var currentSourceColor in sourcePalette)
            {
                var currentDestColor = targetPalette[paletteIndex];
                for (int i = 0; i < source.Width * source.Height; i++)
                {
                    var oldColor = sourceData[i];
                    if (oldColor == currentSourceColor)
                    {
                        newData[i] = currentDestColor;
                    }
                }
                paletteIndex++;
            }

            var outputTexture = new Texture2D(MachinaGame.Current.GraphicsDevice, source.Width, source.Height);
            outputTexture.SetData(newData);
            return outputTexture;
        }
    }
}
