using EC.Library.Converters;
using EC.Library.Core;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace EC.CLI;

internal class Program
{
    static readonly string[] TextExtensions = [".txt"];
    static readonly string[] WordExtensions = [".doc", ".docx"];
    static readonly string[] ExcelExtensions = [".xls", ".xlsx"];
    static readonly string[] SupportedExtensions = [.. TextExtensions, .. WordExtensions, .. ExcelExtensions];

    static int Main(string[] args)
    {
       return ParseCommandLine(args);
    }

    internal static int ParseCommandLine(string[] args)
    {
        var mapFileOption = new Option<FileInfo>("--map", "-m")
        {
            Description = "Path to the encoding map file",
            Required = true
        };

        var fileOption = new Option<FileInfo?>("--file", "-f")
        {
            Description = "Path to the file to convert"
        };
        var directoryOption = new Option<DirectoryInfo?>("--directory", "-d")
        {
            Description = "Path to the directory to convert all supported files within"
        };
        var typeOption = new Option<EncodingType>("--type", "-t")
        {
            Description = "Encoding conversion type (ANSIToUnicode = 0 or UnicodeToANSI = 1)",
            Required = true
        };


        var rootCommand = new RootCommand("Encoding Converter CLI")
        {
            mapFileOption,
            fileOption,
            directoryOption,
            typeOption,
        };
        rootCommand.Validators.Add(commandResult =>
        {
            var mapFile = commandResult.GetValue(mapFileOption);
            var type = commandResult.GetValue(typeOption);
            var file = commandResult.GetValue(fileOption);
            var directory = commandResult.GetValue(directoryOption);

            if (mapFile is null || !mapFile.Exists)
            {
                commandResult.AddError("The specified map file does not exist.");
            }

            if (!(file is not null && file.Exists) && !(directory is not null && directory.Exists))
            {
                commandResult.AddError("You must specify either --file or --directory.");
            }

        });

        var parseResult = rootCommand.Parse(args);
        if (parseResult.Errors.Count > 0)
        {
            foreach (ParseError parseError in parseResult.Errors)
                Console.Error.WriteLine(parseError.Message);
            return 1;
        }

        try
        {
            var mapFile = parseResult.GetValue(mapFileOption);
            ArgumentNullException.ThrowIfNull(mapFile);
            var encodingMapper = EncodingMapper.FromFile(mapFile.FullName);
            var textConverter = new TextConverter(encodingMapper);
            using var wordConverter = new WordConverter(textConverter, true);
            using var excelConverter = new ExcelConverter(textConverter, true);

            var file = parseResult.GetValue(fileOption);
            var directory = parseResult.GetValue(directoryOption);
            var type = parseResult.GetValue(typeOption);

            var fontName = type == EncodingType.ANSIToUnicode ? "Sylfaen" : "Arial";

            if (file != null)
            {
                ProcessFile(file, type, fontName, wordConverter, excelConverter);
            }
            else if (directory != null)
            {
                ProcessDirectory(directory, type, fontName, wordConverter, excelConverter);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }

        return 0;
    }

    internal static void ProcessFile(FileInfo file, EncodingType encodingType, string fontName, WordConverter wordConverter, ExcelConverter excelConverter)
    {
        var extension = Path.GetExtension(file.FullName);
        if (WordExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Converting Word file: {file.FullName}");
            wordConverter.Convert(file.FullName, encodingType, fontName);
        }
        else if (ExcelExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Converting Excel file: {file.FullName}");
            excelConverter.Convert(file.FullName, encodingType, fontName);
        }
        else if (TextExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Converting Text file: {file.FullName}");
            var content = File.ReadAllText(file.FullName);
            var convertedContent = new TextConverter(EncodingMapper.FromFile(file.FullName)).Convert(content, encodingType);
            File.WriteAllText(file.FullName, convertedContent);
        }
        else
        {
            throw new NotSupportedException($"Unsupported file type: {extension}");
        }
    }

    internal static void ProcessDirectory(DirectoryInfo directory, EncodingType encodingType, string fontName, WordConverter wordConverter, ExcelConverter excelConverter)
    {
        var files = Directory.GetFiles(directory.FullName, "*.*", SearchOption.AllDirectories)
            .Where(f => SupportedExtensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
            .ToArray();

        foreach (var file in files)
        {
            try
            {
                var extension = Path.GetExtension(file);
                if (WordExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Converting Word file: {file}");
                    wordConverter.Convert(file, encodingType, fontName);
                }
                else if (ExcelExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Converting Excel file: {file}");
                    excelConverter.Convert(file, encodingType, fontName);
                }
                else if (TextExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Converting Text file: {file}");
                    var content = File.ReadAllText(file);
                    var convertedContent = new TextConverter(EncodingMapper.FromFile(file)).Convert(content, encodingType);
                    File.WriteAllText(file, convertedContent);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error processing file {file}: {ex.Message}");
            }
        }
    }
}
