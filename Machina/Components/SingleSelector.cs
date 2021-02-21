using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class SingleSelector : BaseComponent
    {
        private SingleSelectable selected;

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
                this.selected?.onDeselect?.Invoke();
                this.selected = target;
                this.selected.onSelect?.Invoke();
            }
        }

        private bool IsSelected(SingleSelectable target)
        {
            return this.selected == target;
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(MachinaGame.Assets.MachinaDefault, this.selected != null ? this.selected.actor.ToString() : "(null)", this.actor.transform.Position, Color.Yellow);
        }
    }
}
