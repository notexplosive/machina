﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Machina.Components;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Engine
{
    public class Actor : Crane<IComponent>
    {
        public readonly string name;
        public readonly Scene scene;
        public readonly Transform transform;
        private bool isDirectlyVisible;

        /// <summary>
        ///     Create an actor and add them to the given scene.
        /// </summary>
        /// <param name="name">Human readable name (for debugging)</param>
        /// <param name="scene">Scene that the ctor will add the actor to. Should not be null unless you're a test.</param>
        public Actor(string name, Scene scene)
        {
            this.scene = scene;
            this.scene?.AddActor(this);
            this.name = name;

            Visible = true;
            this.transform = new Transform(this);

            // Transform is "Just" a component, it just happens to be the first component and is added to every actor
            // Niche scenario, AddComponent is OK here.
            AddComponent(this.transform);
        }

        public bool Visible
        {
            set
            {
                this.isDirectlyVisible = value;
            }
            get
            {
                var parentIsVisible = true;
                if (transform.Parent != null)
                {
                    parentIsVisible = transform.Parent.actor.Visible;
                }
                return this.isDirectlyVisible && parentIsVisible;
            }
        }

        public bool IsDestroyed { get; private set; }

        public event Action Destroyed;
        public event Action Deleted;

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

        public T GetComponentInImmediateParent<T>() where T : BaseComponent
        {
            var parent = this.transform.Parent;
            if (parent == null)
            {
                return null;
            }

            return parent.actor.GetComponent<T>();
        }

        public List<T> GetComponentsInImmediateChildren<T>() where T : BaseComponent
        {
            var result = new List<T>();
            for (var i = 0; i < this.transform.ChildCount; i++)
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

            Destroyed?.Invoke();
            this.scene.DeleteActor(this);
            IsDestroyed = true;
        }

        public void Delete()
        {
            // We make actors invisible while we're trying to delete them
            // Sometimes they'll linger for a frame... can we fix that?
            Visible = false;
            Deleted?.Invoke();
            this.scene.DeleteActor(this);
        }

        /// <summary>
        ///     SHOULD NOT BE CALLED DIRECTLY UNLESS YOU'RE IN A UNIT TEST
        ///     If you want to add a component call `new YourComponentName(actor);`
        /// </summary>
        /// <param name="component">The component who is being constructed</param>
        /// <returns></returns>
        internal IComponent AddComponent(IComponent component)
        {
            var type = component.GetType();
            Debug.Assert(GetComponentByName(type.FullName) == null,
                "Attempted to add component that already exists " + type.FullName);

            // TODO: This should be AddIterable so we can AddComponent during an update, but then we can't assemble everything on frame 0 and RequireComponent doesn't work
            this.iterables.Add(component);
            return component;
        }

        /// <summary>
        ///     Acquire a component of type T if it exists. Otherwise get null.
        /// </summary>
        /// <typeparam name="T">Component that inherits from IComponent</typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : BaseComponent
        {
            return GetComponentUnsafe<T>();
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
                parentName = this.transform.Parent.actor + "/";
            }

            return parentName + this.name;
        }

        /// <summary>
        ///     Same as GetComponents except T can be any type
        /// </summary>
        /// <typeparam name="T">Any type the component qualifies under</typeparam>
        /// <returns></returns>
        public List<T> GetComponentsUnsafe<T>()
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

        /// <summary>
        ///     Same as GetComponent except without the type safety
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponentUnsafe<T>() where T : class
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

        public T GetFirstComponentInProgeny<T>() where T : BaseComponent
        {
            for (var i = 0; i < this.transform.ChildCount; i++)
            {
                var child = this.transform.ChildAt(i);
                var comp = child.GetComponent<T>();
                if (comp != null)
                {
                    return comp;
                }

                if (child.transform.ChildCount > 0)
                {
                    var comp2 = child.GetFirstComponentInProgeny<T>();
                    if (comp2 != null)
                    {
                        return comp2;
                    }
                }
            }

            return null;
        }

        public static bool AreAllActorsDestroyed(List<Actor> actors)
        {
            foreach (var actor in actors)
            {
                if (!actor.IsDestroyed)
                {
                    return false;
                }
            }

            return true;
        }
    }
}