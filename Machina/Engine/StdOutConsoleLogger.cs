using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    interface ILogger
    {
        public void Log(params object[] objects);
    }

    /// <summary>
    /// A logger that just prints to standard out
    /// </summary>
    class StdOutConsoleLogger : ILogger
    {
        public void Log(params object[] objects)
        {
            var strings = new List<string>();
            foreach (var obj in objects)
            {
                strings.Add(obj.ToString());
            }

            var output = string.Join("   ", strings);

            Console.WriteLine(output);
        }
    }
}
