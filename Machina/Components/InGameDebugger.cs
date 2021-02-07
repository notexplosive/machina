using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Components
{
    enum DebugLevel
    {
        Off,        // Completely disabled, can be enabled with hotkey
        Passive,    // Show Console Output
        Active      // Render DebugDraws
    }

    class InGameDebugger : BaseComponent
    {
        public static InGameDebugger current;
        public DebugLevel DebugLevel
        {
            get; private set;
        }
        private float fadeTimer;

        public InGameDebugger(Actor actor) : base(actor)
        {
            Debug.Assert(current == null, "Should only have one InGameDebugger");
            current = this;

#if DEBUG
            this.DebugLevel = DebugLevel.Passive;
            this.Log("Debug build detected");
#else
            this.debugLevel = DebugLevel.Off;
#endif
            this.Log("DebugLevel set to:", this.DebugLevel);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }

        public override void Update(float dt)
        {
            if (this.fadeTimer > 0)
            {
                this.fadeTimer -= dt;
            }
            else
            {
                this.fadeTimer = 0;
            }
        }

        void Log(params object[] objects)
        {
            if (this.DebugLevel == DebugLevel.Off)
            {
                return;
            }

            this.fadeTimer = 5f;
            var strings = new List<string>();
            foreach (var obj in objects)
            {
                strings.Add(obj.ToString());
            }

            var output = string.Join('\t', strings);

            Console.WriteLine(output);
        }
    }
}
