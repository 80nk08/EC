using EC.Library.Convertors;
using EC.Library.Core;
using Xunit;

public class TextConvertorTests
{
    [Fact]
    public void Convert_ConvertsText_AccordingToEncodingType()
    {
        var mapper = new EncodingMapper(new[] { 'a', 'b' }, new[] { '?', '!' });
        var convertor = new TextConvertor(mapper);

        var result = convertor.Convert("ab", EncodingType.ANSIToUnicode);

        Assert.Equal("?!", result);
    }

    [Fact]
    public void Convert_UnknownChar_ReturnsOriginalChar()
    {
        var mapper = new EncodingMapper(new[] { 'a' }, new[] { '?' });
        var convertor = new TextConvertor(mapper);

        var result = convertor.Convert("az", EncodingType.ANSIToUnicode);

        Assert.Equal("?z", result);
    }

    [Fact]
    public void Convert_EmptyString_ReturnsEmptyString()
    {
        var mapper = new EncodingMapper(new[] { 'a' }, new[] { '?' });
        var convertor = new TextConvertor(mapper);

        var result = convertor.Convert(string.Empty, EncodingType.ANSIToUnicode);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Convert_UnmappedCharacters_ReturnsOriginalString()
    {
        var mapper = new EncodingMapper(new[] { 'a' }, new[] { '?' });
        var convertor = new TextConvertor(mapper);

        var result = convertor.Convert("xyz", EncodingType.ANSIToUnicode);

        Assert.Equal("xyz", result);
    }

}
