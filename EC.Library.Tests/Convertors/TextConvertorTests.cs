using EC.Library.Converters;
using EC.Library.Core;
using Xunit;

namespace EC.Library.Tests.Converters;

public class TextConverterTests
{
    [Fact]
    public void Convert_ConvertsText_AccordingToEncodingType()
    {
        var mapper = new EncodingMapper(new[] { 'a', 'b' }, new[] { '?', '!' });
        var converter = new TextConverter(mapper);

        var result = converter.Convert("ab", EncodingType.ANSIToUnicode);

        Assert.Equal("?!", result);
    }

    [Fact]
    public void Convert_UnknownChar_ReturnsOriginalChar()
    {
        var mapper = new EncodingMapper(new[] { 'a' }, new[] { '?' });
        var converter = new TextConverter(mapper);

        var result = converter.Convert("az", EncodingType.ANSIToUnicode);

        Assert.Equal("?z", result);
    }

    [Fact]
    public void Convert_EmptyString_ReturnsEmptyString()
    {
        var mapper = new EncodingMapper(new[] { 'a' }, new[] { '?' });
        var converter = new TextConverter(mapper);

        var result = converter.Convert(string.Empty, EncodingType.ANSIToUnicode);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Convert_UnmappedCharacters_ReturnsOriginalString()
    {
        var mapper = new EncodingMapper(new[] { 'a' }, new[] { '?' });
        var converter = new TextConverter(mapper);

        var result = converter.Convert("xyz", EncodingType.ANSIToUnicode);

        Assert.Equal("xyz", result);
    }

}