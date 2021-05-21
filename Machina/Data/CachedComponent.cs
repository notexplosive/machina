using Machina.Components;
using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Data
{
    public class CachedComponent<T> where T : BaseComponent
    {
        private readonly Actor actor;
        private T component;

        public CachedComponent(Actor actor)
        {
            this.actor = actor;
            this.component = null;
        }

        public T GetComponent()
        {
            if (this.component == null)
            {
                this.component = this.actor.GetComponent<T>();
            }

            Debug.Assert(this.component != null);

            return this.component;
        }
    }
}
