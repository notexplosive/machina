using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class SingleSelector : BaseComponent
    {
        public SingleSelectable Selected
        {
            get; private set;
        }

        public SingleSelector(Actor actor) : base(actor)
        {
        }

        public void BuildSelectable(Actor actor)
        {
            new SingleSelectable(actor, this);
        }

        public void Select(SingleSelectable target)
        {
            if (!IsSelected(target))
            {
                Selected?.onDeselect?.Invoke();
                Selected = target;
                Selected.onSelect?.Invoke();
            }
        }

        private bool IsSelected(SingleSelectable target)
        {
            return Selected == target;
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(MachinaGame.Assets.GetSpriteFont("DefaultFont"), Selected != null ? Selected.actor.ToString() : "(null)", this.actor.transform.Position, Color.Yellow);
        }
    }
}
