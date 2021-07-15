using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class DoubleClickable : BaseComponent
    {
        public event ClickAction DoubleClick;

        private readonly float totalWaitTime = 0.5f;
        private readonly Clickable clickable;
        private float timer;
        private MouseButton? mostRecentButton;
        private int numberOfHits;

        public DoubleClickable(Actor actor) : base(actor)
        {
            this.clickable = RequireComponent<Clickable>();
            this.clickable.OnClick += OnClick;
        }

        private void OnClick(MouseButton button)
        {
            if (button == this.mostRecentButton)
            {
                this.numberOfHits++;
                if (this.numberOfHits == 2)
                {
                    DoubleClick?.Invoke(button);
                }
            }
            else
            {
                this.mostRecentButton = button;
                this.timer = this.totalWaitTime;
                this.numberOfHits = 1;
            }
        }

        public override void Update(float dt)
        {

            if (this.timer > 0)
            {
                this.timer -= dt;
            }
            else
            {
                this.mostRecentButton = null;
                this.numberOfHits = 0;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
