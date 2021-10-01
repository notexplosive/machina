using System;
using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Machina.Engine.Debugging.Components
{
    internal class SceneGraphRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        public readonly SpriteFont font;
        private readonly SceneGraphData sceneGraph;
        private readonly Scrollbar scrollbar;
        private readonly SceneGraphUI ui;

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
                    spriteBatch.DrawLine(textPos + new Vector2(-10, fontHeight / 2),
                        textPos + new Vector2(-2, fontHeight / 2), Color.White, 1f, transform.Depth);
                    spriteBatch.DrawLine(textPos + new Vector2(-10, fontHeight / 2), textPos + new Vector2(-10, 0),
                        Color.White, 1f, transform.Depth);
                }

                spriteBatch.DrawString(this.font, text, textPos, textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None,
                    transform.Depth);
                lineNumber++;
            }

            foreach (var node in this.sceneGraph.GetAllNodes())
            {
                if (this.ui.HoveredCrane == node.crane)
                {
                    spriteBatch.DrawRectangle(
                        new RectangleF(
                            (this.actor.transform.Position + new Vector2(0, lineNumber * fontHeight)).ToPoint(),
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
            for (var i = 0; i < targetActor.transform.ChildCount; i++)
            {
                var child = targetActor.transform.ChildAt(i);
                DrawString(child.name + ": " + child.transform.LocalPosition, indent + 1);
                DrawChildren(child, indent + 1, DrawString);
            }
        }
    }
}
