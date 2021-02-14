using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Engine
{
    public class Parent
    {
        private readonly Actor actor;
        private Actor parentActor;

        public Parent(Actor actor)
        {
            this.actor = actor;
        }

        public void Set(Actor targetActor)
        {
            Debug.Assert(targetActor.scene == this.actor.scene, "Cannot unite two actors from different scenes");

            if (this.parentActor != null)
            {
                Clear();
            }

            this.parentActor = targetActor;
            if (!this.parentActor.children.Has(this.actor))
            {
                this.parentActor.children.Add(this.actor);
            }
        }

        public void Clear()
        {
            var children = parentActor.children;
            this.parentActor = null;
            children.Remove(this.actor);
        }

        public bool Has()
        {
            return this.parentActor != null;
        }

        public Actor Get()
        {
            return this.parentActor;
        }
    }
}
