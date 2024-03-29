﻿using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Machina.Components
{
    public class Transform : Crane<Actor>, IComponent
    {
        public readonly Actor actor;
        private float angle_impl;

        private Depth depth_impl = Depth.Middle;
        private float localAngle_impl;
        private Depth localDepth_impl;
        private Vector2 localPosition_impl;
        private Vector2 position_impl;

        public Transform(Actor actor)
        {
            this.actor = actor;
        }

        public Transform Parent { get; private set; }

        public Depth Depth
        {
            get => this.depth_impl;
            set
            {
                this.depth_impl = value;
                for (var i = 0; i < ChildCount; i++)
                {
                    var child = ChildAt(i);
                    child.transform.Depth = this.depth_impl + child.transform.localDepth_impl;
                }
            }
        }

        public Depth LocalDepth
        {
            get
            {
                if (HasParent)
                {
                    return this.localDepth_impl;
                }

                return this.depth_impl;
            }

            set
            {
                if (HasParent)
                {
                    this.localDepth_impl = value;
                    Depth = Parent.Depth + this.localDepth_impl;
                }
                else
                {
                    Depth = value;
                }
            }
        }

        public float Angle
        {
            get => this.angle_impl;
            set
            {
                if (float.IsNaN(value))
                {
                    return;
                }

                this.angle_impl = value;
                for (var i = 0; i < ChildCount; i++)
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

                return Angle;
            }

            set
            {
                if (HasParent)
                {
                    this.localAngle_impl = value;
                    Angle = Parent.Angle + this.localAngle_impl;
                }
                else
                {
                    Angle = value;
                }
            }
        }

        public Vector2 Position
        {
            get => this.position_impl;
            set
            {
                this.position_impl = value;
                this.localPosition_impl = WorldToLocalPosition(this.position_impl);
                for (var i = 0; i < ChildCount; i++)
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

                return Position;
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

        public int ChildCount => this.iterables.Count;

        public Matrix TransformMatrix
        {
            get
            {
                if (HasParent)
                {
                    var pos = Parent.Position;
                    return Matrix.CreateRotationZ(Parent.angle_impl)
                           * Matrix.CreateTranslation(pos.X, pos.Y, 0);
                }

                return Matrix.Identity;
            }
        }

        public bool HasParent => Parent != null;

        public override void Update(float dt)
        {
            // Transform needs to call base if we insert any code into Update()
            base.Update(dt);
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(Position, Position + new Angle(-Angle).ToUnitVector() * 15, Color.LawnGreen, 2, Depth);
            spriteBatch.DrawLine(Position, LocalToWorldPosition(LocalPosition + new Vector2(15, 0)), Color.Cyan, 2,
                Depth);
            spriteBatch.DrawLine(Position, LocalToWorldPosition(LocalPosition + new Vector2(0, -15)), Color.OrangeRed,
                2, Depth);

            if (HasParent)
            {
                spriteBatch.DrawLine(Position, Parent.Position, Color.White, 1, Depth);
            }

            // Transform needs to call base
            base.DebugDraw(spriteBatch);
        }

        public void OnActorDestroy()
        {
            foreach (var iterable in this.iterables)
            {
                DeleteIterable(iterable);
            }
        }

        public int GetChildIndex(Transform transform)
        {
            return this.iterables.IndexOf(transform.actor);
        }

        public void DeleteChild(Actor actor)
        {
            DeleteIterable(actor);
        }

        public void SetParent(Actor newParent)
        {
            if (newParent == this.actor)
            {
                return;
            }

            if (HasParent)
            {
                Parent.RemoveChild(this.actor);
            }

            if (newParent != null)
            {
                Parent = newParent.transform;
                newParent.transform.AddChild(this.actor);
                LocalPosition = WorldToLocalPosition(Position);
                LocalAngle = Angle - newParent.transform.Angle;
                LocalDepth = Depth - newParent.transform.Depth;
            }
            else
            {
                Parent = null;
            }
        }

        /// <summary>
        ///     Remove from scene and add to hierarchy
        /// </summary>
        /// <param name="child"></param>
        private void AddChild(Actor child)
        {
            // If the actor is in a scene, remove them
            child.scene?.GentlyRemoveActor(child);
            AddIterable(child);
        }

        /// <summary>
        ///     Remove from hierarchy and re-add to scene
        /// </summary>
        /// <param name="child"></param>
        private void RemoveChild(Actor child)
        {
            GentlyRemoveIterable(child);
            child.transform.Parent = null;
            child.scene?.AddActor(child);
        }

        public bool HasChildAt(int index)
        {
            return index >= 0 && index < this.iterables.Count;
        }

        public Actor ChildAt(int index)
        {
            if (HasChildAt(index))
            {
                return this.iterables[index];
            }

            return null;
        }

        public Vector2 LocalToWorldPosition(Vector2 localPos)
        {
            return Vector2.Transform(localPos, TransformMatrix);
        }

        public Vector2 WorldToLocalPosition(Vector2 worldPos)
        {
            return Vector2.Transform(worldPos, Matrix.Invert(TransformMatrix));
        }

        public Actor AddActorAsChild(string name, Vector2 localPosition = default)
        {
            Actor newActor;
            if (this.actor.scene != null)
            {
                newActor = this.actor.scene.AddActor(name);
            }
            else
            {
                newActor = new Actor(name, null);
            }

            newActor.transform.SetParent(this.actor);
            newActor.transform.LocalPosition = localPosition;
            newActor.transform.LocalDepth = new Depth(0);
            newActor.transform.LocalAngle = 0f;

            FlushBuffers();
            return newActor;
        }

        public override string ToString()
        {
            return this.actor + ".Transform";
        }
    }
}