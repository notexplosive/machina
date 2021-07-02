using Machina.Engine;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    /// <summary>
    /// Pulls out all frames of a SpriteSheet as their own textures
    /// </summary>
    public class TextureFrames : IAsset
    {
        public readonly SpriteSheet sourceSpriteSheet;
        private readonly List<Texture2D> textures = new List<Texture2D>();

        public TextureFrames(SpriteSheet spriteSheet)
        {
            this.sourceSpriteSheet = spriteSheet;
            for (int i = 0; i < spriteSheet.FrameCount; i++)
            {
                var rect = spriteSheet.GetSourceRectForFrame(i);
                this.textures.Add(MachinaGame.CropTexture(rect, spriteSheet.SourceTexture));
            }
        }

        public Texture2D GetTextureAtFrame(int frame)
        {
            return this.textures[frame];
        }

        public void OnCleanup()
        {
            foreach (var texture in this.textures)
            {
                texture.Dispose();
            }
        }

        public IList<Texture2D> GetAllTextures()
        {
            return this.textures.ToArray();
        }
    }
}
