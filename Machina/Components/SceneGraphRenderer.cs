using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.Engine.Debugging.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine.Debugging.Components
{
    class SceneGraphRenderer : BaseComponent
    {
        private readonly SceneGraphData sceneGraph;
        private readonly SceneGraphUI ui;
        private readonly BoundingRect boundingRect;
        private readonly Scrollbar scrollbar;
        public readonly SpriteFont font;

        public SceneGraphRenderer(Actor actor, Scrollbar scrollbar) : base(actor)
        {
            this.sceneGraph = RequireComponent<SceneGraphData>();
            this.ui = RequireComponent<SceneGraphUI>();
            this.boundingRect = RequireComponent<BoundingRect>();
            this.scrollbar = scrollbar;
            this.font = MachinaGame.Assets.GetSpriteFont("DefaultFontSmall");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var fontHeight = this.font.LineSpacing;
            var lineNumber = 0;

            void DrawString(string text, int indent, Color textColor)
            {
                var textPos = this.actor.transform.Position + new Vector2(indent * 16, lineNumber * fontHeight);

                if (indent > 0)
                {
                    spriteBatch.DrawLine(textPos + new Vector2(-10, fontHeight / 2), textPos + new Vector2(-2, fontHeight / 2), Color.White, 1f, transform.Depth);
                    spriteBatch.DrawLine(textPos + new Vector2(-10, fontHeight / 2), textPos + new Vector2(-10, 0), Color.White, 1f, transform.Depth);
                }

                spriteBatch.DrawString(this.font, text, textPos, textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, transform.Depth);
                lineNumber++;
            }

            foreach (var node in this.sceneGraph.GetAllNodes())
            {
                if (this.ui.HoveredCrane == node.crane)
                {
                    spriteBatch.DrawRectangle(
                        new RectangleF((this.actor.transform.Position + new Vector2(0, lineNumber * fontHeight)).ToPoint(),
                        new Point(this.actor.scene.camera.UnscaledViewportSize.X, fontHeight)),
                        Color.Cyan, 1f, transform.Depth + 1);
                }

                DrawString(node.displayName, node.depth, Color.White);
            }

            var boxHeight = lineNumber * fontHeight;
            this.scrollbar.SetWorldBounds(new MinMax<int>(0, boxHeight));
            this.boundingRect.SetSize(new Point(this.actor.scene.camera.UnscaledViewportSize.X, boxHeight));
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
