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
        private float depth = 0.5f;
        private float localDepth;
        private Vector2 position;
        private Vector2 localPosition;
        private float angle;
        private float localAngle;
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
            spriteBatch.DrawLine(this.actor.progeny.Position, this.actor.progeny.Position + new Angle(-this.actor.progeny.Angle).ToUnitVector() * 15, Color.LawnGreen, 2);
            spriteBatch.DrawLine(this.actor.progeny.Position, this.actor.progeny.LocalToWorldPosition(this.actor.progeny.LocalPosition + new Vector2(15, 0)), Color.Cyan, 2);
            spriteBatch.DrawLine(this.actor.progeny.Position, this.actor.progeny.LocalToWorldPosition(this.actor.progeny.LocalPosition + new Vector2(0, -15)), Color.OrangeRed, 2);

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

        public float Depth
        {
            get
            {
                return this.depth;
            }
            set
            {
                this.depth = value;
                for (int i = 0; i < ChildCount; i++)
                {
                    var child = GetChildAt(i);
                    child.progeny.Depth = this.depth + child.progeny.localDepth;
                }
            }
        }

        public float LocalDepth
        {
            get
            {
                if (HasParent)
                {
                    return this.localDepth;
                }
                else
                {
                    return depth;
                }
            }

            set
            {
                if (HasParent)
                {
                    this.localDepth = value;
                }
                else
                {
                    Depth = value;
                }
            }
        }


        public float Angle
        {
            get
            {
                return this.angle;
            }
            set
            {
                this.angle = value;
                for (int i = 0; i < ChildCount; i++)
                {
                    var child = GetChildAt(i);
                    child.progeny.Angle = this.angle + child.progeny.localAngle;
                    child.progeny.Position = child.progeny.LocalToWorldPosition(child.progeny.LocalPosition);
                }
            }
        }

        public float LocalAngle
        {
            get
            {
                if (HasParent)
                {
                    return this.localAngle;
                }
                else
                {
                    return Angle;
                }
            }

            set
            {
                if (HasParent)
                {
                    this.localAngle = value;
                }
                else
                {
                    Angle = value;
                }
            }
        }


        public Vector2 Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
                this.localPosition = WorldToLocalPosition(this.position);
                for (int i = 0; i < ChildCount; i++)
                {
                    var child = GetChildAt(i);
                    child.progeny.Position = child.progeny.LocalToWorldPosition(child.progeny.localPosition);
                }
            }
        }

        public Vector2 LocalPosition
        {
            get
            {
                if (HasParent)
                {
                    return this.localPosition;
                }
                else
                {
                    return Position;
                }
            }

            set
            {
                if (HasParent)
                {
                    this.localPosition = value;
                }
                else
                {
                    Position = value;
                }
            }
        }

        public Actor Parent => this.parent;
        public int ChildCount => iterables.Count;

        public void SetParent(Actor newParent)
        {
            if (this.actor.progeny.HasParent)
            {
                this.actor.Parent.progeny.RemoveChild(this.actor);
            }

            this.parent = newParent;
            if (newParent != null)
            {
                newParent.progeny.AddChild(this.actor);
                this.actor.progeny.LocalPosition = this.actor.progeny.WorldToLocalPosition(this.actor.progeny.Position);
                this.actor.progeny.LocalAngle = this.actor.progeny.Angle - newParent.progeny.Angle;
                this.actor.progeny.LocalDepth = this.actor.progeny.Depth - newParent.progeny.Depth;
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


        public Actor GetChildAt(int index) // todo: delete this
        {
            return this.ChildAt(index);
        }

        public Matrix TransformMatrix
        {
            get
            {
                var parent = Parent;
                var parentPos = parent != null ? parent.progeny.Position : Vector2.Zero;
                var parentAngle = parent != null ? parent.progeny.Angle : 0f;
                return Matrix.CreateRotationZ(parentAngle)
                    * Matrix.CreateTranslation(parentPos.X, parentPos.Y, 0);
            }
        }

        public Vector2 LocalToWorldPosition(Vector2 localPos)
        {
            return Vector2.Transform(localPos, TransformMatrix);
        }

        public Vector2 WorldToLocalPosition(Vector2 worldPos)
        {
            return Vector2.Transform(worldPos, Matrix.Invert(TransformMatrix));
        }

        public bool HasParent => this.Parent != null;
    }
}
