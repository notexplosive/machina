using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Machina.Engine
{
    public class CommandLineArgs
    {
        private readonly List<string> argsStrings;
        private readonly Dictionary<string, FlagArg> earlyFlagArgTable = new Dictionary<string, FlagArg>();
        private readonly Dictionary<string, ValueArg> earlyValueArgTable = new Dictionary<string, ValueArg>();
        private readonly Dictionary<string, FlagArg> flagArgTable = new Dictionary<string, FlagArg>();
        private readonly Dictionary<string, ValueArg> valueArgTable = new Dictionary<string, ValueArg>();
        public Action onFinishExecute;

        public CommandLineArgs(string[] args)
        {
            var list = new List<string>(args);
            for (var i = 0; i < list.Count; i++)
            {
                list[i] = list[i].ToLower();
            }

            this.argsStrings = list;
        }

        private string GetArgumentValueFromInitialString(string argName)
        {
            var index = this.argsStrings.IndexOf(ConvertToCommandToken(argName));
            if (index == -1)
            {
                MachinaClient.Print("Command line switch", argName, "not found");
                return null;
            }

            if (this.argsStrings.Count == index + 1)
            {
                MachinaClient.Print("Argument value missing for", argName);
                return null;
            }

            var val = this.argsStrings[index + 1];
            if (IsStringACommand(val))
            {
                MachinaClient.Print("Argument value missing for", argName);
                return null;
            }

            return val;
        }

        /// <summary>
        ///     Is the string in the form `--commandName`
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private bool IsStringACommand(string val)
        {
            if (val.Length <= 2)
            {
                return false;
            }

            return val.Substring(0, 2) == "--";
        }

        /// <summary>
        ///     Convert `foo` to `--foo`
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private string ConvertToCommandToken(string val)
        {
            return "--" + val.ToLower();
        }

        private bool InitialStringHasArgument(string argName)
        {
            return this.argsStrings.Contains(ConvertToCommandToken(argName));
        }

        /// <summary>
        ///     Command line arg that is evaluated just AFTER the game is loaded.
        ///     This is useful for the user's game since users can register args
        ///     during game load and have them execute right after
        /// </summary>
        /// <param name="argName">String to invoke the argument, not including the `--`</param>
        /// <param name="onExecute">Callback when argument is used, passes string directly following the arg</param>
        public void RegisterValueArg(string argName, Action<string> onExecute)
        {
            Debug.Assert(!IsStringACommand(argName));
            this.valueArgTable.Add(argName, new ValueArg(onExecute));
        }

        /// <summary>
        ///     Command line Arg that is evalulated BEFORE the game is loaded.
        ///     This is only useful in MachinaGame internally.
        /// </summary>
        /// <param name="argName">String to invoke the argument, not including the `--`</param>
        /// <param name="onExecute">Callback when argument is used, passes string directly following the arg</param>
        public void RegisterEarlyValueArg(string argName, Action<string> onExecute)
        {
            Debug.Assert(!IsStringACommand(argName));
            this.earlyValueArgTable.Add(argName, new ValueArg(onExecute));
        }

        public void RegisterEarlyFlagArg(string argName, Action onExecute)
        {
            Debug.Assert(!IsStringACommand(argName));
            this.earlyFlagArgTable.Add(argName, new FlagArg(onExecute));
        }

        /// <summary>
        ///     Command line arg that is evaluated just AFTER the game is loaded.
        ///     Users can register flag args during OnGameLoad and they'll execute right after
        /// </summary>
        /// <param name="argName">String to invoke the argument, not including the `--`</param>
        /// <param name="onExecute">Callback that will execute if the command is present</param>
        public void RegisterFlagArg(string argName, Action onExecute)
        {
            this.flagArgTable.Add(argName, new FlagArg(onExecute));
        }

        /// <summary>
        ///     Executes the args, users don't need to do this, MachinaGame does this for you.
        /// </summary>
        public void ExecuteArgs()
        {
            foreach (var argName in this.valueArgTable.Keys)
            {
                if (InitialStringHasArgument(argName))
                {
                    this.valueArgTable[argName].Execute(GetArgumentValueFromInitialString(argName));
                }
            }

            foreach (var argName in this.flagArgTable.Keys)
            {
                if (InitialStringHasArgument(argName))
                {
                    this.flagArgTable[argName].Execute();
                }
            }

            this.onFinishExecute?.Invoke();
        }

        /// <summary>
        ///     Executes early args, MachinaGame does this for you.
        /// </summary>
        public void ExecuteEarlyArgs()
        {
            foreach (var argName in this.earlyValueArgTable.Keys)
            {
                if (InitialStringHasArgument(argName))
                {
                    this.earlyValueArgTable[argName].Execute(GetArgumentValueFromInitialString(argName));
                }
            }

            foreach (var argName in this.earlyFlagArgTable.Keys)
            {
                if (InitialStringHasArgument(argName))
                {
                    this.earlyFlagArgTable[argName].Execute();
                }
            }

            this.onFinishExecute?.Invoke();
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