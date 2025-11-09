using System.Collections.ObjectModel;

namespace EC.Library.Core;

public class EncodingMapper
{
    private ReadOnlyDictionary<char, char> AnsiToUnicode { get; }
    private ReadOnlyDictionary<char, char> UnicodeToAnsi { get; }

    public int Count => AnsiToUnicode.Count;

    public EncodingMapper(char[] ansi, char[] unicode)
    {
        if (ansi.Length != unicode.Length)
            throw new ArgumentException("ANSI and Unicode arrays must have the same length.");

        if (ansi.Length == 0 || unicode.Length == 0)
            throw new ArgumentException("ANSI and Unicode arrays must not be empty.");

        if (ansi.Distinct().Count() != ansi.Length || unicode.Distinct().Count() != unicode.Length)
            throw new ArgumentException("Duplicate mappings are not allowed in ANSI or Unicode arrays.");

        var ansiToUnicode = new Dictionary<char, char>();
        var unicodeToAnsi = new Dictionary<char, char>();

        for (int i = 0; i < ansi.Length; i++)
        {
            ansiToUnicode[ansi[i]] = unicode[i];
            unicodeToAnsi[unicode[i]] = ansi[i];
        }

        AnsiToUnicode = new ReadOnlyDictionary<char, char>(ansiToUnicode);
        UnicodeToAnsi = new ReadOnlyDictionary<char, char>(unicodeToAnsi);
    }

    public bool TryConvert(char input, EncodingType encodingType, out char output)
    {
        return encodingType switch
        {
            EncodingType.ANSIToUnicode => AnsiToUnicode.TryGetValue(input, out output),
            EncodingType.UnicodeToANSI => UnicodeToAnsi.TryGetValue(input, out output),
            _ => throw new ArgumentException("Invalid encoding type.", nameof(encodingType)),
        };
    }

    public static EncodingMapper FromFile(string filePath)
    {
        var ansi = new List<char>();
        var unicode = new List<char>();

        foreach (var rawLine in File.ReadLines(filePath))
        {
            var line = rawLine.Trim();

            if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                continue;

            int eq = line.IndexOf('=');
            if (eq <= 0 || eq == line.Length - 1)
                continue; 

            var left = line.AsSpan(0, eq).Trim();
            var right = line.AsSpan(eq + 1).Trim();

            if (left.Length == 0 || right.Length == 0)
                continue;

            if (!ansi.Contains(left[0]) && !unicode.Contains(right[0]))
            {
                ansi.Add(left[0]);
                unicode.Add(right[0]);
            }
        }

        return new EncodingMapper(ansi.ToArray(), unicode.ToArray());
    }
}
