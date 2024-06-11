using EliasLogAnalyzer.MAUI.Converters;
using Xunit;

namespace UnitTests.ConvertersTests;

public class FileSizeFormatConverterTests
{
    private readonly FileSizeFormatConverter _converter = new();

    [Theory]
    [InlineData(0, "0 B")]
    [InlineData(500, "500 B")]
    [InlineData(1024, "1 kB")]
    [InlineData(1048576, "1 MB")]
    [InlineData(5242880, "5 MB")]
    public void Convert_ReturnsExpectedResult(long bytes, string expected)
    {
        // Act
        var result = _converter.Convert(bytes, null!, null, null!);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Convert_ReturnsDefaultValue_ForInvalidInput()
    {
        // Act
        var result = _converter.Convert("invalid", null!, null, null!);

        // Assert
        Assert.Equal("0 Bytes", result);
    }
}