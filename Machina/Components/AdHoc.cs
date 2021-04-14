using Machina.Engine;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class AdHoc : BaseComponent
    {
        public Action<float> onUpdate;
        public Action<SpriteBatch> onDraw;

        public AdHoc(Actor actor) : base(actor)
        {

        }

        public override void Update(float dt)
        {
            this.onUpdate?.Invoke(dt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.onDraw?.Invoke(spriteBatch);
        }
    }
}
