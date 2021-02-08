using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class ZoomCameraOnScroll : DataComponent
    {
        public ZoomCameraOnScroll(Actor actor) : base(actor)
        {
        }

        public override void OnScroll(int scrollDelta)
        {
            this.actor.scene.camera.AdjustZoom((float) scrollDelta / 4);
        }
    }
}
