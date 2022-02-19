using System.Diagnostics;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Machina.Data
{
    public class NinepatchSheet : IAsset
    {
        public enum GenerationDirection
        {
            Inner,
            Outer
        }

        private readonly Texture2D originalTexture;
        public readonly NinepatchRects rects;
        private readonly Texture2D[] textures;

        public NinepatchSheet(Texture2D sourceTexture, Rectangle outerRect, Rectangle innerRect, Painter painter)
        {
            this.originalTexture = sourceTexture;
            Debug.Assert(sourceTexture.Width >= outerRect.Width, "Texture is to small");
            Debug.Assert(sourceTexture.Height >= outerRect.Height, "Texture is to small");

            this.rects = new NinepatchRects(outerRect, innerRect);
            this.textures = new Texture2D[9];

            for (var i = 0; i < 9; i++)
            {
                var rect = this.rects.raw[i];
                if (rect.Width * rect.Height > 0)
                {
                    var cropTexture = painter.CropTexture(rect, sourceTexture);
                    this.textures[i] = cropTexture;
                }
            }
        }

        public NinepatchSheet(string textureAssetName, Rectangle outerRect, Rectangle innerRect, Painter painter)
            : this(MachinaClient.Assets.GetTexture(textureAssetName), outerRect, innerRect, painter)
        {
        }

        public void OnCleanup()
        {
            foreach (var texture in this.textures)
            {
                if (texture != null)
                {
                    texture.Dispose();
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

        public NinepatchRects GenerateDestinationRects(Rectangle starter,
            GenerationDirection gen = GenerationDirection.Inner)
        {
            if (gen == GenerationDirection.Inner)
            {
                var inner = GenerateInnerDestinationRect(starter);
                return new NinepatchRects(starter, inner);
            }

            var outer = GenerateOuterDestinationRect(starter);
            return new NinepatchRects(outer, starter);
        }

        public void DrawDebug(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.originalTexture, this.rects.outer, this.rects.outer, Color.White);

            foreach (var frame in this.rects.raw)
            {
                spriteBatch.DrawRectangle(frame, Color.White);
            }
        }

        public void DrawSection(SpriteBatch spriteBatch, NinepatchIndex index, Rectangle destinationRect,
            Depth layerDepth)
        {
            var dest = destinationRect;
            var source =
                new Rectangle(0, 0, dest.Width, dest.Height); // Source is the size of the destination rect so we tile
            spriteBatch.Draw(this.textures[(int) index], dest.Location.ToVector2(), source, Color.White, 0f,
                new Vector2(), Vector2.One, SpriteEffects.None, layerDepth.AsFloat);
        }

        public void DrawFullNinepatch(SpriteBatch spriteBatch, Rectangle starter, GenerationDirection gen,
            Depth layerDepth, float opacity = 1f)
        {
            DrawFullNinepatch(spriteBatch, GenerateDestinationRects(starter, gen), layerDepth, opacity);
        }

        public void DrawFullNinepatch(SpriteBatch spriteBatch, NinepatchRects destinationRects, Depth layerDepth,
            float opacity = 1f)
        {
            Debug.Assert(this.rects.isValidNinepatch, "Attempted to draw an invalid Ninepatch.");

            for (var i = 0; i < 9; i++)
            {
                var dest = destinationRects.raw[i];
                var source =
                    new Rectangle(0, 0, dest.Width,
                        dest.Height); // Source is the size of the destination rect so we tile
                spriteBatch.Draw(this.textures[i], dest.Location.ToVector2(), source, Color.White.WithMultipliedOpacity(opacity),
                    0f, new Vector2(), Vector2.One, SpriteEffects.None, layerDepth.AsFloat);
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