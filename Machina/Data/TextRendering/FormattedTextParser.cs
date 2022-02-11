using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Machina.Data.TextRendering
{
    public static class FormattedTextParser
    {
        enum ParseState
        {
            ConsumingPlainText,
            ConsumingCommand,
            ConsumingArguments
        }

        public static FormattedTextCommand[] GetCommands(string parsableString)
        {
            var result = new List<FormattedTextCommand>();
            var charIndex = 0;
            var pendingToken = new StringBuilder();
            var pendingCommand = new StringBuilder();
            var pendingArguments = new StringBuilder();
            var currentState = ParseState.ConsumingPlainText;

            while (charIndex < parsableString.Length)
            {
                var currentChar = parsableString[charIndex++];

                if (currentState == ParseState.ConsumingPlainText)
                {
                    if (currentChar == '[' && parsableString[charIndex] == '#')
                    {
                        result.Add(FormattedTextCommand.PlainText.WithArguments(pendingToken.ToString()));
                        pendingToken.Clear();
                        charIndex++;
                        currentState = ParseState.ConsumingCommand;
                    }
                    else
                    {
                        pendingToken.Append(currentChar);
                    }
                }
                else if (currentState == ParseState.ConsumingCommand)
                {
                    if (currentChar == ':')
                    {
                        currentState = ParseState.ConsumingArguments;
                    }
                    else
                    {
                        pendingCommand.Append(currentChar);
                    }
                }
                else if (currentState == ParseState.ConsumingArguments)
                {
                    if (currentChar == ']')
                    {
                        var command = pendingCommand.ToString();
                        var argument = pendingArguments.ToString();

                        pendingCommand.Clear();
                        pendingArguments.Clear();
                        currentState = ParseState.ConsumingPlainText;

                        result.Add(FormattedTextCommand.FromName(command).WithArguments(argument));
                    }
                    else
                    {
                        pendingArguments.Append(currentChar);
                    }
                }
            }

            result.Add(FormattedTextCommand.PlainText.WithArguments(pendingToken.ToString()));
            return result.ToArray();
        }

        public static ITextInputFragment[] GetFragmentsFromCommands(FormattedTextCommand[] commands, IFontMetrics startingFont, Color startingColor)
        {
            var result = new List<ITextInputFragment>();

            var currentColor = startingColor;
            var currentFont = startingFont;

            foreach (var command in commands)
            {
                if (command.IsPlainText)
                {
                    result.Add(new FormattedTextFragment(command.Arguments, currentFont, currentColor));
                }
                else
                {
                    if (command.IsSameAsCommand(FormattedTextCommand.Color))
                    {
                        currentColor = ParseStringAsColor(command.Arguments);
                    }
                }
            }

            return result.ToArray();
        }

        public static Color ParseStringAsColor(string arguments)
        {
            var red = byte.Parse(arguments.Substring(0, 2), NumberStyles.HexNumber);
            var green = byte.Parse(arguments.Substring(2, 2), NumberStyles.HexNumber);
            var blue = byte.Parse(arguments.Substring(4, 2), NumberStyles.HexNumber);

            return new Color(red, green, blue);
        }
    }
}
