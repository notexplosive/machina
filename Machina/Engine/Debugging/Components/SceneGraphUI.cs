using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.Engine.Debugging.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine.Debugging.Components
{
    public class SceneGraphUI : BaseComponent
    {
        private readonly SceneGraphData sceneGraph;
        private readonly Hoverable hoverable;
        private SceneGraphRenderer sceneGraphRenderer;

        public ICrane HoveredCrane
        {
            private set; get;
        }

        public SceneGraphUI(Actor actor) : base(actor)
        {
            this.sceneGraph = RequireComponent<SceneGraphData>();
            this.hoverable = RequireComponent<Hoverable>();
        }

        public override void Start()
        {
            this.sceneGraphRenderer = RequireComponent<SceneGraphRenderer>();
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            if (this.hoverable.IsHovered)
            {
                var result = this.sceneGraph.GetElementAt((int) MathF.Floor(currentPosition.Y / this.sceneGraphRenderer.font.LineSpacing));
                if (result.HasValue)
                {
                    HoveredCrane = result.Value.crane;
                }
            }
            else
            {
                HoveredCrane = null;
            }
        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state)
        {
            if (button == MouseButton.Left && state == ButtonState.Pressed)
            {
                if (HoveredCrane != null)
                {
                    MachinaGame.Print(HoveredCrane);
                }
            }
        }
    }
}
