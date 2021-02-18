using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class Transform : Crane<Actor>, IComponent
    {
        public readonly Actor actor;

        private float depth_impl = 0.5f;
        private float localDepth_impl;
        private Vector2 position_impl;
        private Vector2 localPosition_impl;
        private float angle_impl;
        private float localAngle_impl;
        public Actor Parent
        {
            get; private set;
        }

        public Transform(Actor actor)
        {
            this.actor = actor;
        }

        public override void Update(float dt)
        {
            // Transform needs to call base if we insert any code into Update()
            base.Update(dt);
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(this.Position, this.Position + new Angle(-this.Angle).ToUnitVector() * 15, Color.LawnGreen, 2);
            spriteBatch.DrawLine(this.Position, this.LocalToWorldPosition(this.LocalPosition + new Vector2(15, 0)), Color.Cyan, 2);
            spriteBatch.DrawLine(this.Position, this.LocalToWorldPosition(this.LocalPosition + new Vector2(0, -15)), Color.OrangeRed, 2);

            // Transform needs to call base
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
                return this.depth_impl;
            }
            set
            {
                this.depth_impl = value;
                for (int i = 0; i < ChildCount; i++)
                {
                    var child = ChildAt(i);
                    child.transform.Depth = this.depth_impl + child.transform.localDepth_impl;
                }
            }
        }

        public float LocalDepth
        {
            get
            {
                if (HasParent)
                {
                    return this.localDepth_impl;
                }
                else
                {
                    return depth_impl;
                }
            }

            set
            {
                if (HasParent)
                {
                    this.localDepth_impl = value;
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
                return this.angle_impl;
            }
            set
            {
                this.angle_impl = value;
                for (int i = 0; i < ChildCount; i++)
                {
                    var child = ChildAt(i);
                    child.transform.Angle = this.angle_impl + child.transform.localAngle_impl;
                    child.transform.Position = child.transform.LocalToWorldPosition(child.transform.LocalPosition);
                }
            }
        }

        public float LocalAngle
        {
            get
            {
                if (HasParent)
                {
                    return this.localAngle_impl;
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
                    this.localAngle_impl = value;
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
                return this.position_impl;
            }
            set
            {
                this.position_impl = value;
                this.localPosition_impl = WorldToLocalPosition(this.position_impl);
                for (int i = 0; i < ChildCount; i++)
                {
                    var child = ChildAt(i);
                    child.transform.Position = child.transform.LocalToWorldPosition(child.transform.localPosition_impl);
                }
            }
        }

        public Vector2 LocalPosition
        {
            get
            {
                if (HasParent)
                {
                    return this.localPosition_impl;
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
                    this.localPosition_impl = value;
                    Position = LocalToWorldPosition(this.localPosition_impl);
                }
                else
                {
                    Position = value;
                }
            }
        }

        public int ChildCount => iterables.Count;

        public void SetParent(Actor newParent)
        {
            if (this.HasParent)
            {
                this.actor.Parent.transform.RemoveChild(this.actor);
            }

            this.Parent = newParent;
            if (newParent != null)
            {
                newParent.transform.AddChild(this.actor);
                this.LocalPosition = this.WorldToLocalPosition(this.Position);
                this.LocalAngle = this.Angle - newParent.transform.Angle;
                this.LocalDepth = this.Depth - newParent.transform.Depth;
            }
        }

        /// <summary>
        /// Remove from scene and add to hierarchy
        /// </summary>
        /// <param name="child"></param>
        private void AddChild(Actor child)
        {
            // If the actor is in a scene, remove them
            child.scene?.GentlyRemoveActor(child);
            AddIterable(child);
        }

        /// <summary>
        /// Remove from hierarchy and re-add to scene
        /// </summary>
        /// <param name="child"></param>
        private void RemoveChild(Actor child)
        {
            GentlyRemoveIterable(child);
            this.Parent = null;
            child.scene?.AddActor(child);
        }

        public Actor ChildAt(int index)
        {
            return iterables[index];
        }

        public Matrix TransformMatrix
        {
            get
            {
                if (HasParent)
                {
                    var pos = Parent.transform.Position;
                    return Matrix.CreateRotationZ(Parent.transform.angle_impl)
                        * Matrix.CreateTranslation(pos.X, pos.Y, 0);
                }
                else
                {
                    return Matrix.Identity;
                }
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

        public Actor AddActorAsChild(string name, Vector2 position = default)
        {
            var newActor = this.actor.scene.AddActor(name);
            newActor.SetParent(this.actor);
            newActor.transform.LocalPosition = position;
            return newActor;
        }
    }
}
