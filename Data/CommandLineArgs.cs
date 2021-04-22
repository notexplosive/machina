using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Data
{
    public class CommandLineArgs
    {
        private readonly List<string> argsTokens;
        private readonly Dictionary<string, ValueArg> valueArgTable = new Dictionary<string, ValueArg>();
        private readonly Dictionary<string, FlagArg> flagArgTable = new Dictionary<string, FlagArg>();


        public CommandLineArgs(string[] args)
        {
            var list = new List<string>(args);
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = list[i].ToLower();
            }
            this.argsTokens = list;
        }

        private string GetArgumentValue(string argName)
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

        private bool HasArgument(string argName)
        {
            return argsTokens.Contains(ToCommandToken(argName));
        }

        public void RegisterValueArg(string argName, Action<string> onExecute)
        {
            Debug.Assert(!IsCommandToken(argName));
            this.valueArgTable.Add(argName, new ValueArg(onExecute));
        }

        public void RegisterFlagArg(string argName, Action onExecute)
        {
            this.flagArgTable.Add(argName, new FlagArg(onExecute));
        }

        public void ExecuteArgs()
        {
            foreach (var argName in this.valueArgTable.Keys)
            {
                if (HasArgument(argName))
                {
                    this.valueArgTable[argName].Execute(GetArgumentValue(argName));
                }
            }

            foreach (var argName in this.flagArgTable.Keys)
            {
                if (HasArgument(argName))
                {
                    this.flagArgTable[argName].Execute();
                }
            }
        }


        private class FlagArg
        {
            private readonly Action onExecute;

            public FlagArg(Action onExecute)
            {
                this.onExecute = onExecute;
            }

            public void Execute()
            {
                this.onExecute();
            }
        }

        private class ValueArg
        {
            private readonly Action<string> onExecute;

            public ValueArg(Action<string> onExecute)
            {
                this.onExecute = onExecute;
            }

            public void Execute(string value)
            {
                this.onExecute(value);
            }
        }
    }
}
