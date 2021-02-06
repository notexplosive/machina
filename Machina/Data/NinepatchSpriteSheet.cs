﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Data
{

    class NinepatchSpriteSheet
    {
        public readonly NinepatchRects rects;
        // Ninepatch has at least one empty texture, it might be a valid Threepatch
        private readonly Texture2D originalTexture;
        private readonly Texture2D[] textures;

        public NinepatchSpriteSheet(Texture2D texture, GraphicsDevice graphics, Rectangle outerRect, Rectangle innerRect)
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
                    Texture2D cropTexture = new Texture2D(graphics, rect.Width, rect.Height);
                    Color[] data = new Color[rect.Width * rect.Height];
                    texture.GetData(0, rect, data, 0, data.Length);
                    cropTexture.SetData(data);
                    textures[i] = cropTexture;
                }
            }

        }

        public void DrawDebug(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(originalTexture, this.rects.outer, this.rects.outer, Color.White);

            foreach (var frame in this.rects.raw)
            {
                spriteBatch.DrawRectangle(frame, Color.White);
            }
        }

        public void DrawSection(SpriteBatch spriteBatch, NinepatchIndex index, Rectangle destinationRect)
        {
            var dest = destinationRect;
            var source = new Rectangle(0, 0, dest.Width, dest.Height); // Source is the size of the destination rect so we tile
            spriteBatch.Draw(textures[(int) index], dest.Location.ToVector2(), source, Color.White);
        }

        public void DrawFullNinepatch(SpriteBatch spriteBatch, NinepatchRects destinationRects)
        {
            Debug.Assert(this.rects.isValidNinepatch, "Attempted to draw an invalid Ninepatch.");

            for (int i = 0; i < 9; i++)
            {
                var dest = destinationRects.raw[i];
                var source = new Rectangle(0, 0, dest.Width, dest.Height); // Source is the size of the destination rect so we tile
                spriteBatch.Draw(textures[i], dest.Location.ToVector2(), source, Color.White);
                //if debug: spriteBatch.DrawRectangle(dest, Color.White);
            }
        }

        public void DrawHorizontalThreepatch(SpriteBatch spriteBatch, NinepatchRects destinationRects)
        {
            Debug.Assert(this.rects.IsValidHorizontalThreepatch, "Attempted to draw an invalid horizontal Threepatch");

            DrawSection(spriteBatch, NinepatchIndex.LeftCenter, destinationRects.LeftCenter);
            DrawSection(spriteBatch, NinepatchIndex.Center, destinationRects.Center);
            DrawSection(spriteBatch, NinepatchIndex.RightCenter, destinationRects.RightCenter);
        }

        public void DrawVerticalThreepatch(SpriteBatch spriteBatch, NinepatchRects destinationRects)
        {
            Debug.Assert(this.rects.IsValidVerticalThreepatch, "Attempted to draw an invalid vertical Threepatch");

            DrawSection(spriteBatch, NinepatchIndex.TopCenter, destinationRects.TopCenter);
            DrawSection(spriteBatch, NinepatchIndex.Center, destinationRects.Center);
            DrawSection(spriteBatch, NinepatchIndex.BottomCenter, destinationRects.BottomCenter);
        }
    }
}
