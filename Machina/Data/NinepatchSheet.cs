﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Machina.Engine;

namespace Machina.Data
{

    public class NinepatchSheet : IAsset
    {
        public readonly NinepatchRects rects;
        private readonly Texture2D originalTexture;
        private readonly Texture2D[] textures;

        public NinepatchSheet(Texture2D sourceTexture, Rectangle outerRect, Rectangle innerRect)
        {
            this.originalTexture = sourceTexture;
            Debug.Assert(sourceTexture.Width >= outerRect.Width, "Texture is to small");
            Debug.Assert(sourceTexture.Height >= outerRect.Height, "Texture is to small");

            this.rects = new NinepatchRects(outerRect, innerRect);
            this.textures = new Texture2D[9];

            for (int i = 0; i < 9; i++)
            {
                var rect = rects.raw[i];
                if (rect.Width * rect.Height > 0)
                {
                    var cropTexture = MachinaGame.CropTexture(rect, sourceTexture);
                    textures[i] = cropTexture;
                }
            }
        }

        private Rectangle GenerateInnerDestinationRect(Rectangle outerDestinationRect)
        {
            return new Rectangle(
                outerDestinationRect.Left + this.rects.LeftBuffer,
                outerDestinationRect.Top + this.rects.TopBuffer,
                outerDestinationRect.Width - this.rects.LeftBuffer - this.rects.RightBuffer,
                outerDestinationRect.Height - this.rects.TopBuffer - this.rects.BottomBuffer);
        }

        private Rectangle GenerateOuterDestinationRect(Rectangle innerDestinationRect)
        {
            return new Rectangle(
                innerDestinationRect.Left - this.rects.LeftBuffer,
                innerDestinationRect.Top - this.rects.TopBuffer,
                innerDestinationRect.Width + this.rects.LeftBuffer + this.rects.RightBuffer,
                innerDestinationRect.Height + this.rects.TopBuffer + this.rects.BottomBuffer);
        }

        public enum GenerationDirection
        {
            Inner,
            Outer
        }

        public NinepatchRects GenerateDestinationRects(Rectangle starter, GenerationDirection gen = GenerationDirection.Inner)
        {
            if (gen == GenerationDirection.Inner)
            {
                var inner = GenerateInnerDestinationRect(starter);
                return new NinepatchRects(starter, inner);
            }
            else
            {
                var outer = GenerateOuterDestinationRect(starter);
                return new NinepatchRects(outer, starter);
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

        public void DrawSection(SpriteBatch spriteBatch, NinepatchIndex index, Rectangle destinationRect, Depth layerDepth)
        {
            var dest = destinationRect;
            var source = new Rectangle(0, 0, dest.Width, dest.Height); // Source is the size of the destination rect so we tile
            spriteBatch.Draw(textures[(int) index], dest.Location.ToVector2(), source, Color.White, 0f, new Vector2(), Vector2.One, SpriteEffects.None, layerDepth.AsFloat);
        }

        public void DrawFullNinepatch(SpriteBatch spriteBatch, Rectangle starter, GenerationDirection gen, Depth layerDepth, float opacity = 1f)
        {
            DrawFullNinepatch(spriteBatch, GenerateDestinationRects(starter, gen), layerDepth, opacity);
        }

        public void DrawFullNinepatch(SpriteBatch spriteBatch, NinepatchRects destinationRects, Depth layerDepth, float opacity = 1f)
        {
            Debug.Assert(this.rects.isValidNinepatch, "Attempted to draw an invalid Ninepatch.");

            for (int i = 0; i < 9; i++)
            {
                var dest = destinationRects.raw[i];
                var source = new Rectangle(0, 0, dest.Width, dest.Height); // Source is the size of the destination rect so we tile
                spriteBatch.Draw(textures[i], dest.Location.ToVector2(), source, new Color(Color.White, opacity), 0f, new Vector2(), Vector2.One, SpriteEffects.None, layerDepth.AsFloat);
            }
        }

        public void DrawHorizontalThreepatch(SpriteBatch spriteBatch, Rectangle outer, Depth layerDepth)
        {
            DrawHorizontalThreepatch(spriteBatch, GenerateDestinationRects(outer), layerDepth);
        }

        public void DrawVerticalThreepatch(SpriteBatch spriteBatch, Rectangle outer, Depth layerDepth)
        {
            DrawVerticalThreepatch(spriteBatch, GenerateDestinationRects(outer), layerDepth);
        }

        public void DrawHorizontalThreepatch(SpriteBatch spriteBatch, NinepatchRects destinationRects, Depth layerDepth)
        {
            Debug.Assert(this.rects.IsValidHorizontalThreepatch, "Attempted to draw an invalid horizontal Threepatch");

            DrawSection(spriteBatch, NinepatchIndex.LeftCenter, destinationRects.LeftCenter, layerDepth);
            DrawSection(spriteBatch, NinepatchIndex.Center, destinationRects.Center, layerDepth);
            DrawSection(spriteBatch, NinepatchIndex.RightCenter, destinationRects.RightCenter, layerDepth);
        }

        public void DrawVerticalThreepatch(SpriteBatch spriteBatch, NinepatchRects destinationRects, Depth layerDepth)
        {
            Debug.Assert(this.rects.IsValidVerticalThreepatch, "Attempted to draw an invalid vertical Threepatch");

            DrawSection(spriteBatch, NinepatchIndex.TopCenter, destinationRects.TopCenter, layerDepth);
            DrawSection(spriteBatch, NinepatchIndex.Center, destinationRects.Center, layerDepth);
            DrawSection(spriteBatch, NinepatchIndex.BottomCenter, destinationRects.BottomCenter, layerDepth);
        }
    }
}
