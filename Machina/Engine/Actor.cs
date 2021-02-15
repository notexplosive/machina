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
        public float depth = 0.5f;
        public readonly Scene scene;
        public readonly Parent parent;
        public readonly Children children;
        public readonly string name;
        private readonly Progeny progeny;
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
            parent = new Parent(this);
            children = new Children(this);

            this.scene = scene;
            this.scene?.AddActor(this);
            this.name = name;

            this.progeny = new Progeny(this);
            // Niche scenario, AddComponent is OK here.
            AddComponent(progeny);
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
                    child.Position = child.LocalToWorldPosition();
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

        public int ChildCount => this.children.Count;
        public bool HasParent => this.parent.Has();
        public Actor Parent => this.parent.Get();

        public Actor GetChildAt(int index)
        {
            return this.children.At(index);
        }

        public Matrix TransformMatrix
        {
            get
            {
                var parent = Parent;
                var parentPos = parent.Position;
                return Matrix.CreateRotationZ(parent.Angle)
                    * Matrix.CreateTranslation(parentPos.X, parentPos.Y, 0);
            }
        }

        public Vector2 LocalToWorldPosition()
        {
            return Vector2.Transform(localPosition, TransformMatrix);
        }

        public Vector2 WorldToLocalPosition()
        {
            return Vector2.Transform(Position, Matrix.Invert(TransformMatrix));
        }

        public void Destroy()
        {
            foreach (var component in this.iterables)
            {
                component.OnActorDestroy();
            }

            this.scene.RemoveActor(this);
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
