using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Machina.Engine
{
    public class Actor : Crane<BaseComponent>
    {
        public float depth = 0.5f;
        public readonly Scene scene;
        public readonly Parent parent;
        public readonly Children children;
        public readonly string name;
        public float angle = 0f;
        private Vector2 position;
        private Vector2 localPosition;

        public Vector2 Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
                for (int i = 0; i < this.children.Count; i++)
                {
                    this.children.At(i).Position = this.children.At(i).LocalToWorldPosition();
                }
            }
        }

        public Vector2 LocalPosition
        {
            get
            {
                if (this.parent.Has())
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
                if (this.parent.Has())
                {
                    this.localPosition = value;
                }
                else
                {
                    Position = value;
                }
            }
        }

        public Matrix TransformMatrix
        {
            get
            {
                var parent = this.parent.Get();
                var parentPos = parent.Position;
                return Matrix.CreateRotationZ(parent.angle)
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

        public Actor(string name, Scene scene)
        {
            parent = new Parent(this);
            children = new Children(this);

            this.scene = scene;
            this.scene.AddActor(this);
            this.name = name;
            iterables = new List<BaseComponent>();
        }

        public void Destroy()
        {
            OnRemove();

            foreach (var component in this.iterables)
            {
                component.OnActorDestroy();
            }

            this.scene.RemoveActor(this);
        }

        public override void OnRemove()
        {
            foreach (var component in this.iterables)
            {
                component.OnRemove();
            }
        }

        /// <summary>
        /// SHOULD NOT BE CALLED DIRECTLY UNLESS YOU'RE IN A UNIT TEST
        /// If you want to add a component call `new YourComponentName(actor);`
        /// </summary>
        /// <param name="component">The component who is being constructed</param>
        /// <returns></returns>
        public BaseComponent AddComponent(BaseComponent component)
        {
            Type type = component.GetType();
            Debug.Assert(GetComponentByName(type.FullName) == null, "Attempted to add component that already exists " + type.FullName);

            iterables.Add(component);
            return component;
        }

        /// <summary>
        /// Acquire a component of type T if it exists. Otherwise get null.
        /// </summary>
        /// <typeparam name="T">Component that inherits from BaseComponent</typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : BaseComponent
        {
            foreach (var component in this.iterables)
            {
                T converted = component as T;
                if (converted != null)
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
            comp.OnRemove();
            this.iterables.Remove(comp);
        }

        private BaseComponent GetComponentByName(string fullName)
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

        /// <summary>
        /// This only exists to make tweening easier, you should use the property
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPosition()
        {
            return this.Position;
        }

        /// <summary>
        /// This only exists to make tweening easier, you should use the property
        /// </summary>
        /// <returns></returns>
        public void SetPosition(Vector2 value)
        {
            this.Position = value;
        }

        public override string ToString()
        {
            return this.name;
        }
    }


}
