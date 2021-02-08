using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Data
{

    class NinepatchSheet : IAsset
    {
        public readonly NinepatchRects rects;
        private readonly Texture2D originalTexture;
        private readonly Texture2D[] textures;

        public NinepatchSheet(Texture2D texture, Rectangle outerRect, Rectangle innerRect)
        {
            this.originalTexture = texture;
            Debug.Assert(texture.Width >= outerRect.Width, "Texture is to small");
            Debug.Assert(texture.Height >= outerRect.Height, "Texture is to small");

            this.rects = new NinepatchRects(outerRect, innerRect);
            this.textures = new Texture2D[9];

            for (int i = 0; i < 9; i++)
            {
                var rect = rects.raw[i];
                if (rect.Width * rect.Height > 0)
                {
                    Texture2D cropTexture = new Texture2D(MachinaGame.Current.GraphicsDevice, rect.Width, rect.Height);
                    Color[] data = new Color[rect.Width * rect.Height];
                    texture.GetData(0, rect, data, 0, data.Length);
                    cropTexture.SetData(data);
                    textures[i] = cropTexture;
                }
            }
        }

        public NinepatchSheet(string textureAssetName, Rectangle outerRect, Rectangle innerRect)
            : this(MachinaGame.Assets.GetTexture(textureAssetName), outerRect, innerRect)
        {
        }

        public void OnCleanup()
        {
            foreach (var texture in textures)
            {
                if (texture != null)
                {
                    texture.Dispose();
                }
            }
        }

        public void DrawDebug(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(originalTexture, this.rects.outer, this.rects.outer, Color.White);

            foreach (var frame in this.rects.raw)
            {
                spriteBatch.DrawRectangle(frame, Color.White, 1, 0);
            }
        }

        public void DrawSection(SpriteBatch spriteBatch, NinepatchIndex index, Rectangle destinationRect, float layerDepth)
        {
            var dest = destinationRect;
            var source = new Rectangle(0, 0, dest.Width, dest.Height); // Source is the size of the destination rect so we tile
            spriteBatch.Draw(textures[(int) index], dest.Location.ToVector2(), source, Color.White, 0f, new Vector2(), Vector2.One, SpriteEffects.None, layerDepth);
        }

        public void DrawFullNinepatch(SpriteBatch spriteBatch, NinepatchRects destinationRects, float layerDepth)
        {
            Debug.Assert(this.rects.isValidNinepatch, "Attempted to draw an invalid Ninepatch.");

            for (int i = 0; i < 9; i++)
            {
                var dest = destinationRects.raw[i];
                var source = new Rectangle(0, 0, dest.Width, dest.Height); // Source is the size of the destination rect so we tile
                spriteBatch.Draw(textures[i], dest.Location.ToVector2(), source, Color.White, 0f, new Vector2(), Vector2.One, SpriteEffects.None, layerDepth);
            }
        }

        public void DrawHorizontalThreepatch(SpriteBatch spriteBatch, NinepatchRects destinationRects, float layerDepth)
        {
            Debug.Assert(this.rects.IsValidHorizontalThreepatch, "Attempted to draw an invalid horizontal Threepatch");

            DrawSection(spriteBatch, NinepatchIndex.LeftCenter, destinationRects.LeftCenter, layerDepth);
            DrawSection(spriteBatch, NinepatchIndex.Center, destinationRects.Center, layerDepth);
            DrawSection(spriteBatch, NinepatchIndex.RightCenter, destinationRects.RightCenter, layerDepth);
        }

        public void DrawVerticalThreepatch(SpriteBatch spriteBatch, NinepatchRects destinationRects, float layerDepth)
        {
            Debug.Assert(this.rects.IsValidVerticalThreepatch, "Attempted to draw an invalid vertical Threepatch");

            DrawSection(spriteBatch, NinepatchIndex.TopCenter, destinationRects.TopCenter, layerDepth);
            DrawSection(spriteBatch, NinepatchIndex.Center, destinationRects.Center, layerDepth);
            DrawSection(spriteBatch, NinepatchIndex.BottomCenter, destinationRects.BottomCenter, layerDepth);
        }
    }
}
