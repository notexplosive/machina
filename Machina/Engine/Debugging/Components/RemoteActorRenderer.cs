using Machina.Components;
using Machina.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Machina.Engine.Debugging.Components
{
    public class RemoteActorRenderer : BaseComponent
    {
        private readonly Actor remoteActor;
        private Vector2 cameraOffset;
        private bool useDebugDraw;
        private bool isDragging;

        public RemoteActorRenderer(Actor actor, Actor remoteActor, bool useDebugDraw = false) : base(actor)
        {
            this.remoteActor = remoteActor;
            this.cameraOffset = Vector2.Zero;
            this.useDebugDraw = useDebugDraw;
        }

        public override void Update(float dt)
        {
            this.actor.scene.camera.UnscaledPosition = this.remoteActor.transform.Position - this.cameraOffset;
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            if (this.isDragging)// todo: extract DragPanCamera component, use that instead of including this in RemoteActorRenderer
            {
                this.cameraOffset += positionDelta;
            }
        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state)
        {
            if (button == MouseButton.Right)
            {
                this.isDragging = state == ButtonState.Pressed;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.remoteActor.Draw(spriteBatch);
            if (this.useDebugDraw)
            {
                this.remoteActor.DebugDraw(spriteBatch);
            }
        }
    }
}