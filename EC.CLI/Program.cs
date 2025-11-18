using EC.Library.Converters;
using EC.Library.Core;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace EC.CLI;

internal class Program
{
    static readonly string[] WordExtensions = [".doc", ".docx"];
    static readonly string[] ExcelExtensions = [".xls", ".xlsx"];
    static readonly string[] SupportedExtensions = [.. WordExtensions, .. ExcelExtensions];

    static int Main(string[] args)
    {
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
            fileOption,
            directoryOption,
            typeOption
        };
        rootCommand.Validators.Add(commandResult =>
        {
            var file = commandResult.GetValue(fileOption);
            var directory = commandResult.GetValue(directoryOption);
            if (file is null && directory is null)
            {
                commandResult.AddError("You must specify either --file or --directory.");
            }
            else if (file is not null && directory is not null)
            {
                commandResult.AddError("You cannot specify both --file and --directory.");
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
            var encodingMapper = EncodingMapper.FromFile(@"..\..\..\..\Encodings\armenian.map");
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

    private static void ProcessFile(FileInfo file, EncodingType encodingType, string fontName, WordConverter wordConverter, ExcelConverter excelConverter)
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
        else
        {
            throw new NotSupportedException($"Unsupported file type: {extension}");
        }
    }

    private static void ProcessDirectory(DirectoryInfo directory, EncodingType encodingType, string fontName, WordConverter wordConverter, ExcelConverter excelConverter)
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
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error processing file {file}: {ex.Message}");
            }
        }
    }
}
