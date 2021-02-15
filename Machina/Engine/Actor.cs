using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Machina.Components;

namespace Machina.Engine
{
    public class Actor : Crane<IComponent>
    {
        public readonly Scene scene;
        public readonly string name;
        public readonly Progeny progeny;
        private float depth = 0.5f;
        private float localDepth;
        private Vector2 position;
        private Vector2 localPosition;
        private float angle;
        private float localAngle;

        /// <summary>
        /// Create an actor and add them to the given scene.
        /// </summary>
        /// <param name="name">Human readable name (for debugging)</param>
        /// <param name="scene">Scene that the ctor will add the actor to. Should not be null unless you're a test.</param>
        public Actor(string name, Scene scene)
        {
            this.scene = scene;
            this.scene?.AddActor(this);
            this.name = name;

            this.progeny = new Progeny(this);
            // Niche scenario, AddComponent is OK here.
            AddComponent(progeny);
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
                    child.Depth = this.depth + child.localDepth;
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
                    child.Angle = this.angle + child.localAngle;
                    child.Position = child.LocalToWorldPosition(child.LocalPosition);
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
                for (int i = 0; i < ChildCount; i++)
                {
                    var child = GetChildAt(i);
                    child.Position = child.LocalToWorldPosition(child.localPosition);
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

        public int ChildCount => this.progeny.ChildCount;
        public bool HasParent => this.progeny.Parent != null;
        public Actor Parent => this.progeny.Parent;

        public Actor GetChildAt(int index)
        {
            return this.progeny.ChildAt(index);
        }

        public Matrix TransformMatrix
        {
            get
            {
                var parent = Parent;
                var parentPos = parent != null ? parent.Position : Vector2.Zero;
                var parentAngle = parent != null ? parent.Angle : 0f;
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

        public void Destroy()
        {
            foreach (var component in this.iterables)
            {
                component.OnActorDestroy();
            }

            this.scene.DeleteActor(this);
        }

        internal void SetParent(Actor parent)
        {
            this.progeny.SetParent(parent);
        }

        /// <summary>
        /// SHOULD NOT BE CALLED DIRECTLY UNLESS YOU'RE IN A UNIT TEST
        /// If you want to add a component call `new YourComponentName(actor);`
        /// </summary>
        /// <param name="component">The component who is being constructed</param>
        /// <returns></returns>
        public IComponent AddComponent(IComponent component)
        {
            Type type = component.GetType();
            Debug.Assert(GetComponentByName(type.FullName) == null, "Attempted to add component that already exists " + type.FullName);

            iterables.Add(component);
            return component;
        }

        /// <summary>
        /// Acquire a component of type T if it exists. Otherwise get null.
        /// </summary>
        /// <typeparam name="T">Component that inherits from IComponent</typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : BaseComponent
        {
            foreach (var component in this.iterables)
            {
                if (component is T converted)
                {
                    return converted;
                }
            }

            return null;
        }

        public IEnumerable<T> GetComponents<T>() where T : BaseComponent
        {
            var result = new List<T>();
            foreach (var component in this.iterables)
            {
                if (component is T converted)
                {
                    result.Add(converted);
                }
            }

            return result;
        }

        public void RemoveComponent<T>() where T : BaseComponent
        {
            var comp = GetComponent<T>();
            DeleteIterable(comp);
        }

        private IComponent GetComponentByName(string fullName)
        {
            foreach (var component in this.iterables)
            {
                if (component.GetType().FullName == fullName)
                {
                    return component;
                }
            }
            return null;
        }

        public override string ToString()
        {
            return this.name;
        }
    }


}
