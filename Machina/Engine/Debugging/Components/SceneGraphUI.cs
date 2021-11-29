using System;
using Machina.Components;
using Machina.Data;
using Machina.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Machina.Engine.Debugging.Components
{
    public class SceneGraphUI : BaseComponent
    {
        private readonly Scene creatingScene;
        private readonly Hoverable hoverable;
        private readonly SceneGraphData sceneGraph;
        private readonly WindowManager windowManager;
        private SceneGraphRenderer sceneGraphRenderer;

        public SceneGraphUI(Actor actor, WindowManager windowManager, Scene creatingScene) : base(actor)
        {
            this.windowManager = windowManager;
            this.sceneGraph = RequireComponent<SceneGraphData>();
            this.hoverable = RequireComponent<Hoverable>();
            this.creatingScene = creatingScene;
        }

        public ICrane HoveredCrane { private set; get; }

        public override void Start()
        {
            this.sceneGraphRenderer = RequireComponent<SceneGraphRenderer>();
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            if (this.hoverable.IsHovered)
            {
                var result =
                    this.sceneGraph.GetElementAt(
                        (int) MathF.Floor(currentPosition.Y / this.sceneGraphRenderer.font.LineSpacing));
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
                if (HoveredCrane is Actor hoveredActor)
                {
                    var windowSize = new Point(250, 250);
                    var actorBoundingRectIfExists = hoveredActor.GetComponent<BoundingRect>();
                    if (actorBoundingRectIfExists != null)
                    {
                        windowSize = actorBoundingRectIfExists.Size;
                    }

                    this.windowManager.CreateWindow(this.creatingScene,
                        new WindowBuilder(windowSize)
                            .DestroyViaCloseButton()
                            .CanBeResized()
                            .Title(hoveredActor.name)
                            .OnLaunch(window =>
                            {
                                var renderActor = window.PrimaryScene.AddActor("renderActor");
                                new RemoteActorRenderer(renderActor, hoveredActor, true);
                            })
                    );
                }
            }
        }
    }
}