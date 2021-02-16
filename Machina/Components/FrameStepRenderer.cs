using Machina.Engine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class FrameStepRenderer : BaseComponent
    {
        private IFrameStep frameStep;
        private SceneLayers sceneLayers;

        public FrameStepRenderer(Actor actor, IFrameStep frameStep, SceneLayers sceneLayers) : base(actor)
        {
            this.frameStep = frameStep;
            this.sceneLayers = sceneLayers;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.frameStep.IsPaused)
            {
                this.frameStep.Draw(spriteBatch);
            }
        }

        public override void OnScroll(int scrollDelta)
        {
            if (this.frameStep.IsPaused && scrollDelta < 0)
            {
                foreach (var scene in sceneLayers.AllScenes())
                {
                    scene.Step();
                }
            }
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (key == Keys.Space && modifiers.control && state == ButtonState.Pressed)
            {
                this.frameStep.IsPaused = !this.frameStep.IsPaused;
            }


        }
    }
}
