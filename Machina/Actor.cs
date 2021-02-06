using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Machina
{
    class Actor
    {
        public Vector2 position;
        public float angle;
        public readonly Scene scene;
        private readonly List<BaseComponent> components = new List<BaseComponent>();

        public Actor(Scene scene)
        {
            this.scene = scene;
            this.scene.AddActor(this);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var component in this.components)
            {
                component.Draw(spriteBatch);
            }
        }

        public void EarlyDraw(SpriteBatch spriteBatch)
        {
            foreach (var component in this.components)
            {
                component.EarlyDraw(spriteBatch);
            }
        }

        public void Update(float dt)
        {
            foreach (var component in this.components)
            {
                component.Update(dt);
            }
        }

        public void OnScroll(int scrollDelta)
        {
            foreach (var component in this.components)
            {
                component.OnScroll(scrollDelta);
            }
        }

        public void Destroy()
        {
            OnRemove();

            foreach (var component in this.components)
            {
                component.OnActorDestroy();
            }

            this.scene.RemoveActor(this);
        }

        public void OnRemove()
        {
            foreach (var component in this.components)
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

            components.Add(component);
            return component;
        }

        /// <summary>
        /// Acquire a component of type T if it exists. Otherwise get null.
        /// </summary>
        /// <typeparam name="T">Component that inherits from BaseComponent</typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : BaseComponent
        {
            foreach (BaseComponent component in this.components)
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
            foreach (BaseComponent component in this.components)
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
            this.components.Remove(comp);
        }

        private BaseComponent GetComponentByName(string fullName)
        {
            foreach (BaseComponent component in this.components)
            {
                if (component.GetType().FullName == fullName)
                {
                    return component;
                }
            }
            return null;
        }
    }
}
