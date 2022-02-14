using System;

namespace Machina.Data.TextRendering
{
    public readonly struct FormattedTextCommand
    {
        public FormattedTextCommand(string commandName, string arguments = default)
        {
            CommandName = commandName;
            Arguments = arguments;
        }

        public FormattedTextCommand WithArguments(string arguments)
        {
            return new FormattedTextCommand(CommandName, arguments);
        }

        public static FormattedTextCommand FromName(string commandName)
        {
            switch (commandName)
            {
                case "color":
                    return Color;
                case "font":
                    return Font;
                case "spriteframe":
                    return SpriteFrame;
                default: throw new Exception($"Unrecognized command {commandName}");
            }
        }

        public bool IsSameAsCommand(FormattedTextCommand command)
        {
            return command.CommandName == CommandName;
        }

        public static readonly FormattedTextCommand Color = new FormattedTextCommand("color");
        public static readonly FormattedTextCommand Font = new FormattedTextCommand("font");
        public static readonly FormattedTextCommand SpriteFrame = new FormattedTextCommand("spriteframe");
        public static readonly FormattedTextCommand PlainText = new FormattedTextCommand("plaintext");

        public string CommandName { get; }
        public string Arguments { get; }
        public bool IsPlainText => CommandName == PlainText.CommandName;

        public override string ToString()
        {
            if (IsPlainText)
            {
                return $"'{Arguments}'";
            }
            return $"{CommandName} {Arguments}";
        }
    }
}
