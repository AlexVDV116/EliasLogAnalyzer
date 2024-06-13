using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Converters;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests.ConvertersTests;

public class LogTypeToColorConverterTests(ITestOutputHelper output)
{
    private readonly LogTypeToColorConverter _converter = new();

    [Theory]
    [InlineData(LogType.Error, 255, 0, 0, 25)]
    [InlineData(LogType.Warning, 255, 165, 0, 25)]
    [InlineData(LogType.Information, 0, 255, 0, 25)]
    [InlineData(LogType.Debug, 0, 0, 0, 25)]
    [InlineData(LogType.None, 128, 128, 128, 25)]
    public void Convert_ReturnsExpectedColor(LogType logType, byte r, byte g, byte b, byte a)
    {
        // Act
        var result = _converter.Convert(logType, null!, null, null!);
        output.WriteLine("Result: " + result);
        // Assert
        AssertColorEqual((Color)result, r, g, b, a);
    }

    [Fact]
    public void Convert_ReturnsDefaultColor_ForInvalidInput()
    {
        // Act
        var result = _converter.Convert("invalid", null!, null, null!);

        // Assert
        AssertColorEqual((Color)result, 128, 128, 128, 51); // 20% opacity
    }

    private void AssertColorEqual(Color color, byte expectedR, byte expectedG, byte expectedB, byte expectedA)
    {
        output.WriteLine("Color: " + color);
        
        Assert.Equal(expectedR, (byte)(color.Red * expectedR));
        Assert.Equal(expectedG, (byte)(color.Green * expectedG));
        Assert.Equal(expectedB, (byte)(color.Blue * expectedB));
        Assert.Equal(expectedA, (byte)(color.Alpha * 255));
    }
}