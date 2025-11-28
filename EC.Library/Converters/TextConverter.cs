using EC.Library.Core;
using System.Text;

namespace EC.Library.Converters;

public class TextConverter(EncodingMapper encodingMap)
{
    public string Convert(string text, EncodingType encodingType)
    {
        if (text.Length == 0) return string.Empty;

        return text.Length > 200000 ? 
            ConvertParallel(text, encodingType) : 
            ConvertSequential(text, encodingType);
    }

    private string ConvertParallel(string text, EncodingType encodingType)
    {
        var result = new char[text.Length];

        Parallel.For(0, text.Length, i =>
        {
            var input = text[i];
            result[i] = encodingMap.TryConvert(input, encodingType, out var converted) ? converted : input;
        });

        return new string(result);
    }

    private string ConvertSequential(string text, EncodingType encodingType)
    {
        return string.Create(text.Length, (text, encodingType, encodingMap), (span, state) =>
        {
            var (sourceText, encType, mapper) = state;
            for (var i = 0; i < sourceText.Length; i++)
            {
                span[i] = mapper.TryConvert(sourceText[i], encType, out var converted) 
                    ? converted 
                    : sourceText[i];
            }
        });
    }
}