using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Machina
{
    class Actor
    {
        public Vector2 position = new Vector2();
        public Color color = new Color();
        private readonly List<BaseComponent> components = new List<BaseComponent>();

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw texture centered

            foreach (var component in this.components)
            {
                component.Draw(spriteBatch);
            }
        }

        public void Update(float dt)
        {
            var curKeys = Keyboard.GetState();

            if (curKeys.IsKeyDown(Keys.Up))
                position.Y -= 500f * dt;

            if (curKeys.IsKeyDown(Keys.Down))
                position.Y += 500f * dt;

            if (curKeys.IsKeyDown(Keys.Left))
                position.X -= 500f * dt;

            if (curKeys.IsKeyDown(Keys.Right))
                position.X += 500f * dt;
        }

        /// <summary>
        /// SHOULD NOT BE CALLED DIRECTLY UNLESS YOU'RE IN A UNIT TEST
        /// If you want to add a component call `new YourComponentName(actor);`
        /// </summary>
        /// <param name="component">The component who is being constructed</param>
        /// <returns></returns>
        public BaseComponent AddComponent(BaseComponent component)
        {
            components.Add(component);
            return component;
        }

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
    }
}
