﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Data
{
    public struct PointBool
    {
        public PointBool(bool x, bool y)
        {
            this.x = x;
            this.y = y;
        }
        public bool x;
        public bool y;

        public static PointBool False = new PointBool(false, false);
        public static PointBool True = new PointBool(true, true);
    }

    public class Image : IAsset
    {
        private readonly SpriteSheet spriteSheet;
        private readonly int frame;

        public Image(SpriteSheet spriteSheet, int frame)
        {
            Debug.Assert(frame >= 0 && frame < spriteSheet.FrameCount);
            this.spriteSheet = spriteSheet;
            this.frame = frame;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float scale, float angle, PointBool flip, Depth layerDepth, Color color)
        {
            spriteSheet.DrawFrame(spriteBatch, frame, position, scale, angle, flip, layerDepth, color);
        }

        public void OnCleanup()
        {

        }
    }
}