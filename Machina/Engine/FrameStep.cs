using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public interface IFrameStep
    {
        public bool IsPaused
        {
            get; set;
        }

        public void Step(Scene scene);
    }

    public class EmptyFrameStep : IFrameStep
    {
        public bool IsPaused
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public void Step(Scene scene)
        {
            // No op
        }
    }

    public class FrameStep : IFrameStep
    {
        public bool IsPaused
        {
            get; set;
        }

        public void Step(Scene scene)
        {
            scene.Update(1f / 60f);
        }
    }
}
