using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public interface ICoroutineAction
    {
        public bool IsComplete(float dt);
    }

    public class WaitUntil : ICoroutineAction
    {
        private Func<bool> condition;

        public WaitUntil(Func<bool> condition)
        {
            this.condition = condition;
        }

        public bool IsComplete(float dt)
        {
            return this.condition();
        }
    }

    public class WaitSeconds : ICoroutineAction
    {
        private float seconds;

        public WaitSeconds(float seconds)
        {
            this.seconds = seconds;
        }
        public bool IsComplete(float dt)
        {
            this.seconds -= dt;
            return this.seconds <= 0f;
        }
    }
}
