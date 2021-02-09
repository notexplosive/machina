using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
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

    class Logger : BaseComponent
    {
        // There is only one listener, it's usually the console output overlay but if you want to implement a different listener you can
        private IDebugOutputListener listener;

        public DebugLevel DebugLevel
        {
            get; private set;
        }


        public Logger(Actor actor, IDebugOutputListener listener) : base(actor)
        {
            this.listener = listener;
#if DEBUG
            this.DebugLevel = DebugLevel.Passive;
            this.Log("Debug build detected");
#else
            this.debugLevel = DebugLevel.Off;
#endif
            this.Log("DebugLevel set to:", this.DebugLevel);
        }


        public void Log(params object[] objects)
        {
            if (this.DebugLevel == DebugLevel.Off)
            {
                return;
            }

            var strings = new List<string>();
            foreach (var obj in objects)
            {
                strings.Add(obj.ToString());
            }

            var output = string.Join("   ", strings);

            this.listener.OnMessageLog(output);
            Console.WriteLine(output);
        }
    }
}
