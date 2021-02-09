using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Machina.Engine
{
    public class Actor : Crane
    {
        public Vector2 position;
        public float angle = 0f;
        public float depth = 0.5f;
        public readonly Scene scene;

        public Actor(Scene scene)
        {
            this.scene = scene;
            this.scene.AddActor(this);
            iterables = new List<Crane>();
        }

        public void Destroy()
        {
            OnRemove();

            foreach (var component in this.iterables)
            {
                (component as BaseComponent).OnActorDestroy();
            }

            this.scene.RemoveActor(this);
        }

        public override void OnRemove()
        {
            foreach (var component in this.iterables)
            {
                (component as BaseComponent).OnRemove();
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
                    return component as BaseComponent;
                }
            }
            return null;
        }

        public Vector2 GetPosition()
        {
            return this.position;
        }

        public void SetPosition(Vector2 value)
        {
            this.position = value;
        }
    }


}
