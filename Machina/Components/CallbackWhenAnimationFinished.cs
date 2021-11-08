using System;
using Machina.Engine;

namespace Machina.Components
{
    public class CallbackWhenAnimationFinished : BaseComponent
    {
        private readonly Action callback;
        private readonly SpriteRenderer spriteRenderer;

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