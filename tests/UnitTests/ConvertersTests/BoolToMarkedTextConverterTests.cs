using EliasLogAnalyzer.MAUI.Converters;
using Xunit;

namespace UnitTests.ConvertersTests;

public class BoolToMarkedTextConverterTests
{
    private readonly BoolToMarkedTextConverter _converter = new();

    [Theory]
    [InlineData(true, "Unmark")]
    [InlineData(false, "Mark")]
    [InlineData(null, "Mark")]
    public void Convert_ReturnsExpectedResult(object value, string expected)
    {
        // Act
        var result = _converter.Convert(value, null!, null, null!);

        // Assert
        Assert.Equal(expected, result);
    }
}