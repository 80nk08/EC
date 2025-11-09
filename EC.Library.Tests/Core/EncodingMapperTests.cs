using EC.Library.Core;
using System;
using System.IO;
using Xunit;

public class EncodingMapperTests
{
    [Fact]
    public void Constructor_ValidArrays_MapsCorrectly()
    {
        var ansi = new[] { 'a', 'b' };
        var unicode = new[] { '?', '?' };
        var mapper = new EncodingMapper(ansi, unicode);

        Assert.Equal(2, mapper.Count);
    }

    [Fact]
    public void Constructor_InvalidArrays_ThrowsArgumentException()
    {
        var ansi = new[] { 'a' };
        var unicode = new[] { '?', '?' };
        Assert.Throws<ArgumentException>(() => new EncodingMapper(ansi, unicode));
    }

    [Theory]
    [InlineData('a', EncodingType.ANSIToUnicode, '?', true)]
    [InlineData('?', EncodingType.UnicodeToANSI, 'a', true)]
    [InlineData('x', EncodingType.ANSIToUnicode, 'x', false)]
    public void TryConvert_ValidAndInvalidInputs_ReturnsExpected(
        char input, EncodingType type, char expected, bool shouldConvert)
    {
        var mapper = new EncodingMapper(new[] { 'a' }, new[] { '?' });
        var result = mapper.TryConvert(input, type, out var output);

        Assert.Equal(shouldConvert, result);
        if (result)
            Assert.Equal(expected, output);
    }

    [Fact]
    public void TryConvert_InvalidEncodingType_ThrowsArgumentException()
    {
        var mapper = new EncodingMapper(new[] { 'a' }, new[] { '?' });
        Assert.Throws<ArgumentException>(() =>
            mapper.TryConvert('a', (EncodingType)99, out _));
    }

    [Fact]
    public void FromFile_ValidFile_ParsesCorrectly()
    {
        var lines = new[]
        {
            "# comment",
            "a = ?",
            "b=?",
            "  ",
            "malformedline"
        };
        var file = Path.GetTempFileName();
        File.WriteAllLines(file, lines);

        var mapper = EncodingMapper.FromFile(file);

        Assert.Equal(2, mapper.Count);
        Assert.True(mapper.TryConvert('a', EncodingType.ANSIToUnicode, out var u1));
        Assert.Equal('?', u1);
        Assert.True(mapper.TryConvert('?', EncodingType.UnicodeToANSI, out var a2));
        Assert.Equal('b', a2);

        File.Delete(file);
    }
}
