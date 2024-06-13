using EliasLogAnalyzer.MAUI.Converters;
using Xunit;

namespace UnitTests.ConvertersTests;

public class BoolToPinTextConverterTests
{
    private readonly BoolToPinTextConverter _converter = new();

    [Theory]
    [InlineData(true, "Unpin")]
    [InlineData(false, "Pin")]
    [InlineData(null, "Pin")]
    public void Convert_ReturnsExpectedResult(object value, string expected)
    {
        // Act
        var result = _converter.Convert(value, null!, null, null!);

        // Assert
        Assert.Equal(expected, result);
    }
}