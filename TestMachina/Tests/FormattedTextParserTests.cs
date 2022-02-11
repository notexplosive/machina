using FluentAssertions;
using Machina.Data.TextRendering;
using Microsoft.Xna.Framework;
using Xunit;

namespace TestMachina.Tests
{
    public class FormattedTextParserTests
    {
        [Fact]
        public void get_commands_basic()
        {
            FormattedTextCommand.PlainText.IsPlainText.Should().BeTrue();
            FormattedTextCommand.Color.IsPlainText.Should().BeFalse();
        }

        [Fact]
        public void plain_text_is_plain_text()
        {
            var commands = FormattedTextParser.GetCommands("words [#color:aeaeae]words words [#color:a3ef37]words [#color:aaaaaa]words [#color:bbbbbb]words");

            commands.Should().ContainInOrder(
                FormattedTextCommand.PlainText.WithArguments("words "),
                FormattedTextCommand.Color.WithArguments("aeaeae"),
                FormattedTextCommand.PlainText.WithArguments("words words "),
                FormattedTextCommand.Color.WithArguments("a3ef37"),
                FormattedTextCommand.PlainText.WithArguments("words "),
                FormattedTextCommand.Color.WithArguments("aaaaaa"),
                FormattedTextCommand.PlainText.WithArguments("words "),
                FormattedTextCommand.Color.WithArguments("bbbbbb"),
                FormattedTextCommand.PlainText.WithArguments("words")
            );
        }

        [Fact]
        public void get_fragments_from_commands()
        {
            var commands = new FormattedTextCommand[]
            {
                FormattedTextCommand.PlainText.WithArguments("words "),
                FormattedTextCommand.Color.WithArguments("aeaeae"),
                FormattedTextCommand.PlainText.WithArguments("words words "),
                FormattedTextCommand.Color.WithArguments("a3ef37"),
                FormattedTextCommand.PlainText.WithArguments("words "),
                FormattedTextCommand.Color.WithArguments("aaaaaa"),
                FormattedTextCommand.PlainText.WithArguments("words "),
                FormattedTextCommand.Color.WithArguments("bbbbbb"),
                FormattedTextCommand.PlainText.WithArguments("words")
            };

            var font = new MonospacedFontMetrics(new Point(4, 4));
            var fragments = FormattedTextParser.GetFragmentsFromCommands(commands, font, Color.White);

            fragments.Should().HaveCount(5);
        }

        [Fact]
        public void get_color_from_string()
        {
            var color = FormattedTextParser.ParseStringAsColor("a1b2c3");

            color.R.Should().Be(0xa1);
            color.G.Should().Be(0xb2);
            color.B.Should().Be(0xc3);
        }
    }
}