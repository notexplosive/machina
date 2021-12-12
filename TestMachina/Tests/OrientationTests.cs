using FluentAssertions;
using Machina.Data;
using Xunit;

namespace TestMachina.Tests
{
    public class OrientationTests
    {
        [Fact]
        public void orientation_sanity()
        {
            Orientation.Horizontal.Should().Be(Orientation.Horizontal);
            Orientation.Horizontal.Should().NotBe(Orientation.Vertical);

            Orientation.Vertical.Should().Be(Orientation.Vertical);
            Orientation.Vertical.Should().NotBe(Orientation.Horizontal);

            (Orientation.Vertical != Orientation.Horizontal).Should().BeTrue();
            (Orientation.Horizontal != Orientation.Vertical).Should().BeTrue();
        }
    }
}