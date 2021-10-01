using System;
using System.Collections.Generic;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Machina.Components
{
    public class IntroTextAnimation : BaseComponent
    {
        private readonly BoundedTextRenderer textRenderer;
        private readonly SoundEffectInstance tikSound;
        private float dampening = 1;
        private bool spinning;
        private bool toggle;
        private float totalTime;

        public IntroTextAnimation(Actor actor) : base(actor)
        {
            this.textRenderer = RequireComponent<BoundedTextRenderer>();
            this.actor.scene.StartCoroutine(IntroAnimation());
            this.tikSound = MachinaGame.Assets.GetSoundEffectInstance("bbox-chape");
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

        private void PlayTick(float pitch = 0)
        {
            this.tikSound.Pitch = pitch;
            this.tikSound.Volume = 0.25f;
            this.tikSound.Stop();
            this.tikSound.Play();
        }

        private IEnumerator<ICoroutineAction> IntroAnimation()
        {
            var camera = this.actor.scene.camera;
            var speed = 1.5f;
            var ouch = MachinaGame.Assets.GetSoundEffectInstance("ouch");
            ouch.Pitch = 0.25f;
            ouch.Volume = 0.25f;

            SwapColor();

            this.textRenderer.Text = "";

            yield return new WaitSeconds(0.5f / speed);
            PlayTick(-0.5f);
            this.textRenderer.Text = "NOTEXPLOSIVE";
            yield return new WaitSeconds(0.1f / speed);
            this.textRenderer.Text = "NotExplosive";
            PlayTick();
            camera.Zoom = 2;

            yield return new WaitSeconds(1.5f / speed);

            this.textRenderer.Text = "NotExplosive.";
            camera.Zoom = 1.5f;
            PlayTick(0.5f);

            yield return new WaitSeconds(0.25f / speed);

            this.textRenderer.Text = "NotExplosive.net";
            PlayTick(0.5f);
            camera.Zoom = 1;
            SwapColor();

            yield return new WaitSeconds(0.75f / speed);

            ouch.Play();
            this.spinning = true;

            yield return new WaitUntil(() => this.dampening == 0);

            yield return new WaitSeconds(1f / speed);
            this.actor.Destroy();
        }
    }
}
