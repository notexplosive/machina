using System;
using System.Collections.Generic;
using Machina.Components;
using Machina.Engine.Debugging.Data;

namespace Machina.Engine.Debugging.Components
{
    public class LoggerComponent : BaseComponent, ILogger
    {
        // There is only one listener, it's usually the console output overlay but if you want to implement a different listener you can
        private readonly IDebugOutputRenderer renderer;

        public LoggerComponent(Actor actor, IDebugOutputRenderer renderer) : base(actor)
        {
            this.renderer = renderer;
        }

        public void Log(LogBuffer.Message message)
        {
            if (Runtime.DebugLevel == DebugLevel.Off)
            {
                return;
            }

            var strings = new List<string>();
            foreach (var obj in message.Content)
            {
                if (obj == null)
                {
                    strings.Add("null");
                }
                else
                {
                    strings.Add(obj.ToString());
                }
            }

            var output = string.Join("   ", strings);

            Console.WriteLine(output);

            var splitOnNewlines = output.Split("\n");

            foreach (var split in splitOnNewlines)
            {
                this.renderer.OnMessageLog(split);
            }
        }
    }
}