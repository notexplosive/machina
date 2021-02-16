using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class SceneGraphRenderer : BaseComponent
    {
        private readonly SceneLayers sceneLayers;
        private readonly Scrollbar scrollbar;

        public SceneGraphRenderer(Actor actor, SceneLayers sceneLayers, Scrollbar scrollbar) : base(actor)
        {
            this.sceneLayers = sceneLayers;
            this.scrollbar = scrollbar;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var font = MachinaGame.Assets.DefaultSmallFont;
            var boxHeight = font.LineSpacing;
            var lineNumber = 0;

            void DrawString(string text, int indent)
            {
                spriteBatch.DrawString(font, text, this.actor.transform.Position + new Vector2(indent * 16, lineNumber * boxHeight), Color.White);
                lineNumber++;
            }

            foreach (var scene in sceneLayers.AllScenesExceptDebug())
            {
                DrawString("Scene", 0);
                foreach (var actor in scene.GetAllActors())
                {
                    DrawString(actor.name, 1);
                    foreach (var component in actor.GetComponents<BaseComponent>())
                    {
                        DrawString(component.GetType().Name.ToString(), 2);
                    }
                }
            }

            this.scrollbar.worldBounds = new MinMax<int>(0, lineNumber * boxHeight);
        }
    }
}
