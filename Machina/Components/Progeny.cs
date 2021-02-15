using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class Progeny : Crane<Actor>, IComponent
    {
        private Actor parent;
        private Actor actor;

        public Progeny(Actor actor)
        {
            this.actor = actor;
        }

        public override void Update(float dt)
        {
            foreach (var child in iterables)
            {
                child.Position = child.LocalToWorldPosition();
            }

            base.Update(dt);
        }

        public void OnActorDestroy()
        {
            foreach (var iterable in iterables)
            {
                DeleteIterable(iterable);
            }
        }

        public Actor Parent => this.parent;
        public int ChildCount => iterables.Count;

        public void SetParent(Actor newParent)
        {
            if (this.actor.HasParent)
            {
                this.actor.Parent.progeny.RemoveChild(this.actor);
            }

            this.parent = newParent;
            newParent.progeny.AddChild(this.actor);
            this.actor.LocalPosition = this.actor.WorldToLocalPosition();
            this.actor.LocalAngle = this.actor.Angle - newParent.Angle;
            this.actor.LocalDepth = this.actor.Depth - newParent.Depth;
        }

        private void AddChild(Actor child)
        {
            child.scene.GentlyRemoveActor(child);
            AddIterable(child);
        }

        private void RemoveChild(Actor child)
        {
            GentlyRemoveIterable(child);
            this.parent = null;
            child.scene.AddActor(child);
        }

        public Actor ChildAt(int index)
        {
            return iterables[index];
        }
    }
}
