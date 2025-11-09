using EC.Library.Core;
using System.Text;

namespace EC.Library.Convertors;

public class TextConvertor(EncodingMapper encodingMap) 
{
    public string Convert(string text, EncodingType encodingType)
    {
        var stringBuilder = new StringBuilder(text.Length);

        for (int i = 0; i < text.Length; i++)
        {
            char input = text[i];

            if (!encodingMap.TryConvert(input, encodingType, out var converted))
                converted = input;

            stringBuilder.Append(converted);

        }

        return stringBuilder.ToString();
    }
}
