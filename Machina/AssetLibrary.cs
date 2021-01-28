using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Machina
{
    class AssetLibrary
    {
        private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private ContentManager content;
        public AssetLibrary(Game game)
        {
            this.content = game.Content;
        }

        // Called by the game
        public Texture2D LoadTexture(string name)
        {
            var texture = this.content.Load<Texture2D>(name);
            textures.Add(name, texture);
            Console.WriteLine(string.Format("Loaded asset: {0}", name));
            return texture;
        }

        public Texture2D GetTexture(string name)
        {
            Debug.Assert(textures.ContainsKey(name));
            return textures[name];
        }
    }
}
