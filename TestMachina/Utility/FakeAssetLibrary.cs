﻿using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestMachina.Utility
{
    public class FakeAssetLibrary : IAssetLibrary
    {
        public T AddMachinaAsset<T>(string name, T asset) where T : IAsset
        {
            return asset;
        }

        public SoundEffectInstance CreateSoundEffectInstance(string name)
        {
            return null;
        }

        public SoundEffectInstance GetSoundEffectInstance(string name)
        {
            return null;
        }

        public SpriteFont GetSpriteFont(string name)
        {
            return null;
        }

        public Texture2D GetTexture(string name)
        {
            return null;
        }

        public void LoadAllContent()
        {
        }

        public void UnloadAssets()
        {
        }

        T IAssetLibrary.GetMachinaAsset<T>(string name)
        {
            return null;
        }
    }
}