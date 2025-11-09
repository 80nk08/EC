using EC.Library.Convertors;
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
            var encodingMapper = EncodingMapper.FromFile("EncodingMappers\\armenian.map");
            var textConvertor = new TextConvertor(encodingMapper);
            using var wordConvertor = new WordConvertor(textConvertor, true);
            using var excelConvertor = new ExcelConvertor(textConvertor, true);

            var file = parseResult.GetValue(fileOption);
            var directory = parseResult.GetValue(directoryOption);
            var type = parseResult.GetValue(typeOption);

            var fontName = type == EncodingType.ANSIToUnicode ? "Sylfaen" : "Arial";

            if (file != null)
            {
                ProcessFile(file, type, fontName, wordConvertor, excelConvertor);
            }
            else if (directory != null)
            {
                ProcessDirectory(directory, type, fontName, wordConvertor, excelConvertor);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }

        return 0;
    }

    private static void ProcessFile(FileInfo file, EncodingType encodingType, string fontName, WordConvertor wordConvertor, ExcelConvertor excelConvertor)
    {
        var extension = Path.GetExtension(file.FullName);
        if (WordExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Converting Word file: {file.FullName}");
            wordConvertor.Convert(file.FullName, encodingType, fontName);
        }
        else if (ExcelExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Converting Excel file: {file.FullName}");
            excelConvertor.Convert(file.FullName, encodingType, fontName);
        }
        else
        {
            throw new NotSupportedException($"Unsupported file type: {extension}");
        }
    }

    private static void ProcessDirectory(DirectoryInfo directory, EncodingType encodingType, string fontName, WordConvertor wordConvertor, ExcelConvertor excelConvertor)
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
                    wordConvertor.Convert(file, encodingType, fontName);
                }
                else if (ExcelExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Converting Excel file: {file}");
                    excelConvertor.Convert(file, encodingType, fontName);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error processing file {file}: {ex.Message}");
            }
        }
    }
}
