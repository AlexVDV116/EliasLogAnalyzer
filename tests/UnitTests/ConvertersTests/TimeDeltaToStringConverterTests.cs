using EliasLogAnalyzer.MAUI.Converters;
using Xunit;

namespace UnitTests.ConvertersTests;

public class TimeDeltaToStringConverterTests
{
    private readonly TimeDeltaToStringConverter _converter = new();

    [Theory]
    [InlineData(0, "0 ms")]
    [InlineData(500, "+500 ms")]
    [InlineData(-500, "-500 ms")]
    [InlineData(15000, "+15 s")]
    [InlineData(-15000, "-15 s")]
    [InlineData(90000, "+1.5 m")]
    [InlineData(-90000, "-1.5 m")]
    [InlineData(7200000, "+2 h")]
    [InlineData(-7200000, "-2 h")]
    public void Convert_ReturnsExpectedString(int timeDelta, string expected)
    {
        // Act
        var result = _converter.Convert(timeDelta, null!, null, null!);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Convert_ReturnsEmptyString_ForInvalidInput()
    {
        // Act
        var result = _converter.Convert("invalid", null!, null, null!);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}