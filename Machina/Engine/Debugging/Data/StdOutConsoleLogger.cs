using System.Collections.Generic;

namespace Machina.Engine.Debugging.Data
{
    public interface ILogger
    {
        public void Log(LogBuffer.Message message);
    }

    /// <summary>
    ///     A logger that just prints to standard out
    /// </summary>
    public class StdOutConsoleLogger : ILogger
    {
        public void Log(LogBuffer.Message message)
        {
            var strings = new List<string>();
            foreach (var obj in message.Content)
            {
                strings.Add(obj == null ? "null" : obj.ToString());
            }

            var output = string.Join("   ", strings);
        }
    }
}