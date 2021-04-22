using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public class CommandLineArgs
    {
        private readonly List<string> argsTokens;

        public CommandLineArgs(string[] args)
        {
            var list = new List<string>(args);
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = list[i].ToLower();
            }
            this.argsTokens = list;
        }

        public string GetArgumentValue(string argName)
        {
            var index = argsTokens.IndexOf(ToCommandToken(argName));
            if (index == -1)
            {
                MachinaGame.Print("Command line switch", argName, "not found");
                return null;
            }

            if (argsTokens.Count == index + 1)
            {
                MachinaGame.Print("Argument value missing for", argName);
                return null;
            }
            else
            {
                var val = argsTokens[index + 1];
                if (IsCommandToken(val))
                {
                    MachinaGame.Print("Argument value missing for", argName);
                    return null;
                }
                return val;
            }
        }

        private bool IsCommandToken(string val)
        {
            if (val.Length <= 2)
            {
                return false;
            }

            return val.Substring(0, 2) == "--";
        }

        private string ToCommandToken(string val)
        {
            return "--" + val.ToLower();
        }

        public bool HasArgument(string argName)
        {
            return argsTokens.Contains(ToCommandToken(argName));
        }

        public string GetArgumentValueIfExists(string argName)
        {
            if (HasArgument(argName))
            {
                return GetArgumentValue(argName);
            }

            return null;
        }
    }
}
