using System;
using System.Collections.Generic;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;

namespace Machina.Components
{
    public class IntroTextAnimation : BaseComponent
    {
        private readonly BoundedTextRenderer textRenderer;
        private float dampening = 1;
        private bool spinning;
        private bool toggle;
        private float totalTime;

        public IntroTextAnimation(Actor actor) : base(actor)
        {
            this.textRenderer = RequireComponent<BoundedTextRenderer>();
            this.actor.scene.StartCoroutine(IntroAnimation());
            SwapColor();
        }

        public override void Update(float dt)
        {
            if (this.spinning)
            {
                this.totalTime += dt;
                this.dampening -= dt / 2;

                this.dampening = Math.Clamp(this.dampening, 0, 1);
                this.actor.scene.camera.Rotation = MathF.Sin(this.totalTime * 7) * this.dampening / 3;
                this.actor.scene.camera.Zoom = 1 + MathF.Sin(this.totalTime * 5) * this.dampening / 5;

                this.actor.scene.sceneLayers.BackgroundColor *= 0.99f;
            }
        }

        private void SwapColor()
        {
            var primary = new Color(0, 11, 173);
            var secondary = new Color(186, 219, 173);

            if (this.toggle)
            {
                this.textRenderer.TextColor = primary;
                this.actor.scene.sceneLayers.BackgroundColor = secondary;
            }
            else
            {
                this.textRenderer.TextColor = secondary;
                this.actor.scene.sceneLayers.BackgroundColor = primary;
            }

            this.toggle = !this.toggle;
        }

        private IEnumerator<ICoroutineAction> IntroAnimation()
        {
            var camera = this.actor.scene.camera;
            var speed = 1.5f;

            SwapColor();

            yield return new WaitSeconds(1f);

            this.textRenderer.Text = "";

            MachinaClient.SoundEffectPlayer.PlaySound("blblblbl");

            var name = "NotExplosive";
            yield return new WaitSeconds(0.25f / speed);

            for (var i = 0; i < name.Length + 1; i++)
            {
                this.textRenderer.Text = name.Substring(0, i);
                yield return new WaitSeconds(0.075f / speed);
            }

            yield return new WaitSeconds(1f / speed);

            MachinaClient.SoundEffectPlayer.PlaySound("ouch", 0.25f);
            this.spinning = true;
            SwapColor();

            this.textRenderer.Text = "NotExplosive.net";
            camera.Zoom = 1.5f;

            yield return new WaitSeconds(0.25f / speed);

            camera.Zoom = 1;

            yield return new WaitSeconds(0.75f / speed);

            yield return new WaitUntil(() => this.dampening == 0);

            yield return new WaitSeconds(1f / speed);
            this.actor.Destroy();
        }
    }
}