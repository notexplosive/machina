﻿using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class DestroyWhenAnimationFinished : BaseComponent
    {
        private readonly SpriteRenderer spriteRenderer;

        public DestroyWhenAnimationFinished(Actor actor) : base(actor)
        {
            this.spriteRenderer = RequireComponent<SpriteRenderer>();
        }

        public override void Update(float dt)
        {
            if (this.spriteRenderer.IsAnimationFinished())
            {
                this.actor.Destroy();
            }
        }
    }
}
