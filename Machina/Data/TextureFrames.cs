using System;
using System.Collections.Generic;
using Machina.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Data
{
    /// <summary>
    ///     Pulls out all frames of a SpriteSheet as their own textures
    /// </summary>
    public class TextureFrames : IDisposable
    {
        public readonly SpriteSheet sourceSpriteSheet;
        private readonly List<Texture2D> textures = new List<Texture2D>();

        public TextureFrames(SpriteSheet spriteSheet)
        {
            this.sourceSpriteSheet = spriteSheet;
            for (var i = 0; i < spriteSheet.FrameCount; i++)
            {
                var rect = spriteSheet.GetSourceRectForFrame(i);
                this.textures.Add(MachinaGame.CropTexture(rect, spriteSheet.SourceTexture));
            }
        }

        public void Dispose()
        {
            foreach (var texture in this.textures)
            {
                texture.Dispose();
            }
        }

        public Texture2D GetTextureAtFrame(int frame)
        {
            return this.textures[frame];
        }

        public IList<Texture2D> GetAllTextures()
        {
            return this.textures.ToArray();
        }
    }
}