using System.Diagnostics;
using Machina.Components;
using Machina.Engine;

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
            AttemptCacheComponent();
            Debug.Assert(HasComponent());
            return this.component;
        }

        public bool HasComponent()
        {
            AttemptCacheComponent();
            return this.component != null;
        }

        private void AttemptCacheComponent()
        {
            if (this.component == null)
            {
                this.component = this.actor.GetComponent<T>();
            }
        }
    }
}