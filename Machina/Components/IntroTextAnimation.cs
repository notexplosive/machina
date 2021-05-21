﻿using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class IntroTextAnimation : BaseComponent
    {
        private readonly BoundedTextRenderer textRenderer;
        private bool toggle;
        private bool spinning;
        private float totalTime;
        private float dampening = 1;
        private SoundEffectInstance tikSound;

        public IntroTextAnimation(Actor actor) : base(actor)
        {
            this.textRenderer = RequireComponent<BoundedTextRenderer>();
            this.actor.scene.StartCoroutine(IntroAnimation());
            tikSound = MachinaGame.Assets.GetSoundEffectInstance("bbox-tik");
            SwapColor();
        }

        public override void Update(float dt)
        {
            if (this.spinning)
            {
                this.totalTime += dt;
                dampening -= dt / 2;

                dampening = Math.Clamp(dampening, 0, 1);
                this.actor.scene.camera.Rotation = MathF.Sin(totalTime * 7) * dampening / 3;
                this.actor.scene.camera.Zoom = 1 + MathF.Sin(totalTime * 5) * dampening / 5;

                this.actor.scene.sceneLayers.BackgroundColor *= 0.99f;
            }
        }

        private void SwapColor()
        {
            var primary = new Color(0, 11, 173);
            var secondary = new Color(186, 219, 173);

            if (toggle)
            {
                this.textRenderer.TextColor = primary;
                this.actor.scene.sceneLayers.BackgroundColor = secondary;
            }
            else
            {
                this.textRenderer.TextColor = secondary;
                this.actor.scene.sceneLayers.BackgroundColor = primary;
            }
            toggle = !toggle;
        }

        void PlayTick(float pitch = 0)
        {
            tikSound.Pitch = pitch;
            tikSound.Stop();
            tikSound.Play();
        }

        private IEnumerator<ICoroutineAction> IntroAnimation()
        {
            var camera = this.actor.scene.camera;
            var speed = 1.5f;
            var ouch = MachinaGame.Assets.GetSoundEffectInstance("ouch");
            ouch.Pitch = 0.25f;
            ouch.Volume = 0.25f;



            yield return new WaitSeconds(0.5f / speed);

            this.textRenderer.Text = "Not";
            PlayTick();
            camera.Zoom = 5;

            yield return new WaitSeconds(0.5f / speed);

            this.textRenderer.Text = "NotEx";
            PlayTick(-0.75f);
            camera.Zoom = 4;
            SwapColor();

            yield return new WaitSeconds(0.5f / speed);

            this.textRenderer.Text = "NotExplo";
            PlayTick(-0.25f);
            camera.Zoom = 3;

            yield return new WaitSeconds(0.5f / speed);

            this.textRenderer.Text = "NotExplosive";
            PlayTick(-0.5f);
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

            yield return new WaitUntil(() => dampening == 0);

            yield return new WaitSeconds(0.5f / speed);
            this.textRenderer.Text = "NotExplosive";
            PlayTick(1);
            yield return new WaitSeconds(0.15f / speed);
            this.textRenderer.Text = "NotExpl";
            PlayTick(1);
            yield return new WaitSeconds(0.15f / speed);
            this.textRenderer.Text = "No";
            PlayTick(1);
            yield return new WaitSeconds(0.15f / speed);
            this.textRenderer.Text = "";
            PlayTick(1);
            yield return new WaitSeconds(1f / speed);
            this.actor.Destroy();
        }
    }
}