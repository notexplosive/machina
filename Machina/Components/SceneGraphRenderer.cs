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

            foreach (var scene in sceneLayers.AllScenes())
            {
                DrawString("Scene", 0);
                foreach (var targetActor in scene.GetRootLevelActors())
                {
                    DrawString(targetActor.name + ": " + targetActor.transform.Position.ToString(), 1);

                    DrawChildren(targetActor, 1, DrawString);

                    foreach (var component in targetActor.GetComponents<BaseComponent>())
                    {
                        // DrawString(component.GetType().Name.ToString(), 2);
                    }
                }
            }

            this.scrollbar.worldBounds = new MinMax<int>(0, lineNumber * boxHeight);
        }

        public void DrawChildren(Actor targetActor, int indent, Action<string, int> DrawString)
        {

            for (int i = 0; i < targetActor.transform.ChildCount; i++)
            {
                var child = targetActor.transform.ChildAt(i);
                DrawString(child.name + ": " + child.transform.LocalPosition.ToString(), indent + 1);
                DrawChildren(child, indent + 1, DrawString);
            }
        }
    }
}
