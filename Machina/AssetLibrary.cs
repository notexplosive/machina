using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.IO;

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
        public List<string> GetFilesAtContentDirectory(string contentFolder)
        {
            var result = new List<string>();

            DirectoryInfo dir = new DirectoryInfo(this.content.RootDirectory + "\\" + contentFolder);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException();
            }

            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                result.Add(Path.GetFileNameWithoutExtension(file.FullName));
            }

            return result;
        }

        public void LoadAllTextures()
        {
            foreach (var imageName in GetFilesAtContentDirectory("images"))
            {
                LoadTexture("images/" + imageName);
            }
        }

        private void LoadTexture(string fullName)
        {
            var splitName = fullName.Split('/');
            var name = splitName[^1];

            var texture = this.content.Load<Texture2D>(fullName);
            textures.Add(name, texture);
            Console.WriteLine(string.Format("Loaded asset: {0}", fullName));
        }

        public Texture2D GetTexture(string name)
        {
            Debug.Assert(textures.ContainsKey(name));
            return textures[name];
        }
    }
}
