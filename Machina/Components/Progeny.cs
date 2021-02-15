using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class Progeny : Crane<Actor>, IComponent
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
        }

        public void AddChild(Actor child)
        {
            child.scene.RemoveActor(child);
            AddIterable(child);
        }

        public void RemoveChild(Actor child)
        {
            DeleteIterable(child);
            child.scene.AddActor(child);
        }

        public void OnActorDestroy()
        {
            foreach (var iterable in iterables)
            {
                DeleteIterable(iterable);
            }
        }
    }
}
