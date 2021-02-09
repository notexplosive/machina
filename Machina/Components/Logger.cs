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

    class Logger : BaseComponent, ILogger
    {
        // There is only one listener, it's usually the console output overlay but if you want to implement a different listener you can
        private IDebugOutputRenderer renderer;


        public Logger(Actor actor, IDebugOutputRenderer renderer) : base(actor)
        {
            this.renderer = renderer;

        }


        public void Log(params object[] objects)
        {
            if (MachinaGame.DebugLevel == DebugLevel.Off)
            {
                return;
            }

            var strings = new List<string>();
            foreach (var obj in objects)
            {
                strings.Add(obj.ToString());
            }

            var output = string.Join("   ", strings);

            Console.WriteLine(output);

            this.renderer.OnMessageLog(output);
        }
    }
}
