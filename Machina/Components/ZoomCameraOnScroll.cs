﻿using System;
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
            Console.WriteLine(scrollDelta);
            this.actor.scene.camera.AdjustZoom(scrollDelta);
        }
    }
}