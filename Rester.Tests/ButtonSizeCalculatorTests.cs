using FluentAssertions;
using Rester.Control;
using Xunit;

namespace Rester.Tests
{
    public class ButtonSizeCalculatorTests
    {
        [Theory]
        [InlineData(83.9, 0.0)] //Smaller than margin should get size of zero
        [InlineData(84.0, 0.0)]
        [InlineData(194.0, 53.0)]
        [InlineData(707.0, 100.5)]
        public void AtColumnWidthTheSizeShouldBeTheExpected(double columnWidth, double expectedSize)
        {
            var sut = new ButtonSizeCalculator();
            sut.GetButtonSize(columnWidth).Should().BeApproximately(expectedSize, double.Epsilon);
        }
    }
}
