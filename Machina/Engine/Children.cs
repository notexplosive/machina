using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Engine
{
    public class Children
    {
        private readonly Actor actor;
        private readonly List<Actor> content;

        public Children(Actor actor)
        {
            this.actor = actor;
            this.content = new List<Actor>();
        }

        /// <summary>
        /// Returns a copy of the list of children
        /// </summary>
        /// <returns></returns>
        public List<Actor> GetAll()
        {
            return new List<Actor>(content);
        }

        public int Count => content.Count;
        public Actor At(int index)
        {
            return this.content[index];
        }

        public void Add(Actor addedActor)
        {
            Debug.Assert(addedActor.scene == this.actor.scene, "Cannot unite two actors from different scenes");
            Debug.Assert(addedActor != this.actor, "Cannot parent the same actor to itself");

            MachinaGame.Print(this.actor.name, " now has child: ", addedActor.name);
            content.Add(addedActor);

            if (addedActor.parent.Get() != this.actor)
            {
                addedActor.parent.Set(this.actor);
            }

            addedActor.LocalPosition = addedActor.WorldToLocalPosition();
        }

        public void Remove(Actor removedActor)
        {
            if (removedActor.parent.Has())
            {
                removedActor.parent.Clear();
            }

            content.Remove(removedActor);
        }

        public bool Has(Actor targetActor)
        {
            return content.Contains(targetActor);
        }
    }
}

