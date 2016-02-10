using FluentAssertions;
using Rester.Control;
using Xunit;

namespace Rester.Tests
{
    public class ButtonSizeCalculatorTests
    {
        [Theory]
        [InlineData(90.9, 0.0)] //Smaller than margin should get size of zero
        [InlineData(91.0, 0.0)]
        [InlineData(201.0, 54.0)]
        [InlineData(817.0, 102.0)]
        public void AtColumnWidthTheSizeShouldBeTheExpected(double columnWidth, double expectedSize)
        {
            var sut = new ButtonSizeCalculator();
            sut.GetButtonSize(columnWidth).Should().BeApproximately(expectedSize, double.Epsilon);
        }
    }
}
