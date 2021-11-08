using Machina.Engine;

namespace Machina.Components
{
    public class BoundingRectToViewportSize : BaseComponent
    {
        private readonly BoundingRect boundingRect;

        public BoundingRectToViewportSize(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.boundingRect.SetSize(this.actor.scene.sceneLayers.gameCanvas.ViewportSize);
        }

        public override void Update(float dt)
        {
            this.boundingRect.SetSize(this.actor.scene.sceneLayers.gameCanvas.ViewportSize);
        }
    }
}