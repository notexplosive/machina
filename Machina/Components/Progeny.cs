using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
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
                //child.Position = child.LocalToWorldPosition(child.LocalPosition);
            }

            // Progeny needs to call base
            base.Update(dt);
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(this.actor.Position, this.actor.Position + new Angle(-this.actor.Angle).ToUnitVector() * 15, Color.LawnGreen, 2);
            spriteBatch.DrawLine(this.actor.Position, this.actor.LocalToWorldPosition(this.actor.LocalPosition + new Vector2(15, 0)), Color.Cyan, 2);
            spriteBatch.DrawLine(this.actor.Position, this.actor.LocalToWorldPosition(this.actor.LocalPosition + new Vector2(0, -15)), Color.OrangeRed, 2);

            // Progeny needs to call base
            base.DebugDraw(spriteBatch);
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
            if (newParent != null)
            {
                newParent.progeny.AddChild(this.actor);
                this.actor.LocalPosition = this.actor.WorldToLocalPosition(this.actor.Position);
                this.actor.LocalAngle = this.actor.Angle - newParent.Angle;
                this.actor.LocalDepth = this.actor.Depth - newParent.Depth;
            }
        }

        /// <summary>
        /// Remove from scene and add to hierarchy
        /// </summary>
        /// <param name="child"></param>
        private void AddChild(Actor child)
        {
            // If the actor is in a scene, remove them
            child.scene.GentlyRemoveActor(child);
            AddIterable(child);
        }

        /// <summary>
        /// Remove from hierarchy and re-add to scene
        /// </summary>
        /// <param name="child"></param>
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
