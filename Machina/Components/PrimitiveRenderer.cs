using Microsoft.Xna.Framework.Graphics;
using PrimitiveBuddy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class PrimitiveRenderer : DataComponent
    {
        private Primitive prim;

        public PrimitiveRenderer(Actor actor, Primitive prim) : base(actor)
        {
            this.prim = prim;
        }
    }
}
