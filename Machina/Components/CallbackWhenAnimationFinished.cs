using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class CallbackWhenAnimationFinished : BaseComponent
    {
        private readonly SpriteRenderer spriteRenderer;
        private readonly Action callback;

        public CallbackWhenAnimationFinished(Actor actor, Action callback) : base(actor)
        {
            this.spriteRenderer = RequireComponent<SpriteRenderer>();
            this.callback = callback;
        }

        public override void Update(float dt)
        {
            if (this.spriteRenderer.IsAnimationFinished())
            {
                // TODO: this will fire every update after animation is done, kind of assumes DestroyWhenAnimationFinished... hmm
                this.callback?.Invoke();
            }
        }
    }
}
