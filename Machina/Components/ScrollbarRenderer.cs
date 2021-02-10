using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class ScrollbarRenderer : BaseComponent
    {
        private readonly BoundingRect myBoundingRect;
        private readonly BoundingRect containerBoundingRect;
        private readonly PanCameraOnScroll cameraPanner;
        private readonly Camera targetCamera;

        public ScrollbarRenderer(Actor actor, BoundingRect containerBoundingRect, PanCameraOnScroll cameraPanner) : base(actor)
        {
            this.myBoundingRect = RequireComponent<BoundingRect>();
            this.containerBoundingRect = containerBoundingRect;
            this.cameraPanner = cameraPanner;
            this.targetCamera = cameraPanner.actor.scene.camera;

            this.myBoundingRect.SetOffsetToTopLeft();
            this.actor.parent.Set(containerBoundingRect.actor);
        }

        public override void Update(float dt)
        {
            this.myBoundingRect.Height = this.containerBoundingRect.Height;
            this.actor.LocalPosition = new Vector2(this.containerBoundingRect.Width - this.containerBoundingRect.Offset.X, -this.containerBoundingRect.Offset.Y);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float scrollPercent = this.targetCamera.Position.Y / this.cameraPanner.bounds.max;
            float viewBounds = this.containerBoundingRect.Height * this.targetCamera.Zoom;
            float fullBounds = this.cameraPanner.bounds.max - this.cameraPanner.bounds.min;
            float percentThatRepresentsAreaOnScreen = viewBounds / fullBounds;

            int thumbHeight = (int) (this.containerBoundingRect.Height * percentThatRepresentsAreaOnScreen);
            int thumbYPosition = (int) ((this.containerBoundingRect.Height - thumbHeight) * scrollPercent);

            spriteBatch.DrawRectangle(new Rectangle(
                this.myBoundingRect.Rect.Location + new Point(0, thumbYPosition),
                new Point(this.myBoundingRect.Width, thumbHeight)),
                Color.Orange, 1, this.actor.depth);
        }
    }
}
