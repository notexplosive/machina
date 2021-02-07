using Machina.Data;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class MoveTween : UpdateOnlyComponent
    {
        TweenChain<Vector2> chain;
        public MoveTween(Actor actor) : base(actor)
        {
            chain = new TweenChain<Vector2>(Vector2.Lerp, () => this.actor.position, (value) => this.actor.position = value);
        }

        public override void Update(float dt)
        {
            chain.Update(dt);
        }

        public MoveTween AddTween(Vector2 targetPos, float duration, ScaleFunc func)
        {
            chain.AddTween(targetPos, duration, func);
            return this;
        }
    }
}
