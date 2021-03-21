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
        public readonly Transform transform;
        public bool Visible
        {
            get; set;
        }

        public bool IsDestroyed
        {
            get; private set;
        }

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

            this.Visible = true;
            this.transform = new Transform(this);

            // Transform is "Just" a component, it just happens to be the first component and is added to every actor
            // Niche scenario, AddComponent is OK here.
            AddComponent(transform);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                base.Draw(spriteBatch);
            }
        }

        public override void PreDraw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                base.PreDraw(spriteBatch);
            }
        }

        public List<T> GetComponentsInImmediateChildren<T>() where T : BaseComponent
        {
            var result = new List<T>();
            for (int i = 0; i < this.transform.ChildCount; i++)
            {
                var child = this.transform.ChildAt(i);
                var comp = child.GetComponent<T>();
                if (comp != null)
                {
                    result.Add(comp);
                }
            }
            return result;
        }

        public void Destroy()
        {
            foreach (var component in this.iterables)
            {
                component.OnActorDestroy();
            }

            this.scene.DeleteActor(this);
            this.IsDestroyed = true;
        }

        public void Delete()
        {
            // We make actors invisible while we're trying to delete them
            // Sometimes they'll linger for a frame... can we fix that?
            Visible = false;
            this.scene.DeleteActor(this);
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

            // TODO: This should be AddIterable so we can AddComponent during an update, but then we can't assemble everything on frame 0
            this.iterables.Add(component);
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
            return GetComponentsUnsafe<T>();
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
            var parentName = "";
            if (this.transform.HasParent)
            {
                parentName = this.transform.Parent.actor.ToString() + "/";
            }
            return parentName + this.name;
        }

        /// <summary>
        /// Same as GetComponents except T can be any type
        /// </summary>
        /// <typeparam name="T">Any type the component qualifies under</typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetComponentsUnsafe<T>()
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
    }


}
